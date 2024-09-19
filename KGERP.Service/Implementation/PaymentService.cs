using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly ERPEntities context;
        public PaymentService(ERPEntities context)
        {
            this.context = context;
        }
        public async Task<PaymentModel> GetPayments(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            PaymentModel paymentModel = new PaymentModel();
            paymentModel.CompanyId = companyId;
            paymentModel.DataList = await Task.Run(() => (from t1 in context.Payments
                                                              join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                                              join t3 in context.PaymentModes on t1.PaymentModeId equals t3.PaymentModeId
                                                              into od
                                                              from t3 in od.DefaultIfEmpty()
                                                              where t1.CompanyId == companyId          
                                                             && t1.TransactionDate >= fromDate
                                                             && t1.TransactionDate <= toDate
                                                              select new PaymentModel
                                                              {
                                                                  TransactionDate = t1.TransactionDate,
                                                                  CustomerCode = t2.Code,
                                                                  Customer = t2.Name,
                                                                  PaymentMode = t3 != null ? t3.Name : "",
                                                                  ReferenceNo= t1.ReferenceNo,
                                                                  MoneyReceiptNo= t1.MoneyReceiptNo,
                                                                  InAmount= t1.InAmount,
                                                                  OutAmount= t1.OutAmount
                                                              }).OrderByDescending(o => o.TransactionDate).AsEnumerable());

            return paymentModel;

        }
        public List<PaymentModel> GetPayments(string searchDate, string searchText, int companyId)
        {
            DateTime? dateSearch = null;
            dateSearch = !string.IsNullOrEmpty(searchDate) ? Convert.ToDateTime(searchDate) : dateSearch;

            List<PaymentModel> payments = context.Database.SqlQuery<PaymentModel>("spGetCustomerPayments {0}", companyId).ToList();

            if (dateSearch == null)
            {
                return payments.Where(x => (x.Customer.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                     (x.PaymentMode.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                     (x.ReferenceNo.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                     (x.CustomerCode.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText))
                                    ).OrderByDescending(x => x.PaymentId).ToList();
            }
            if (string.IsNullOrEmpty(searchText) && dateSearch != null)
            {
                return payments.Where(x => x.TransactionDate == dateSearch).OrderByDescending(x => x.PaymentId).ToList();
            }


            return payments.Where(x => x.TransactionDate == dateSearch &&
                                     ((x.Customer.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                     (x.Customer.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                     (x.PaymentMode.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                     (x.ReferenceNo.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                     (x.CustomerCode.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText))
                                    )).OrderByDescending(x => x.PaymentId).ToList();

        }
        public async Task<PaymentModel> GetPayment(int id)
        {
            if (id <= 0)
            {
                return new PaymentModel() { TransactionDate = DateTime.Now };
            }

            Payment payment = context.Payments.Find(id);
            return ObjectConverter<Payment, PaymentModel>.Convert(payment);
        }

        public List<SelectModel> GetPaymentMethodSelectModels()
        {
            List<SelectModel> paymentMethods = Enum.GetValues(typeof(PaymentMethod)).Cast<PaymentMethod>().Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt32(v) }).ToList();
            return paymentMethods;
        }



        public List<PaymentModel> GetPaymentsByVendor(int vendorId)
        {
            List<SelectModel> paymentMethods = Enum.GetValues(typeof(PaymentMethod)).Cast<PaymentMethod>().Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt32(v) }).ToList();
            List<Payment> payments = context.Payments.Include("Vendor").Where(x => x.VendorId == vendorId).ToList();
            List<PaymentModel> paymentModels = ObjectConverter<Payment, PaymentModel>.ConvertList(payments).ToList();

            paymentModels = paymentModels.Select(p => new PaymentModel
            {
                PaymentId = p.PaymentId,
                Vendor = p.Vendor,
                VendorId = p.VendorId,
                //PaymentMethodName = paymentMethods.Where(x => x.Value.ToString() == .ToString()).FirstOrDefault().Text.ToString(),
                InAmount = p.InAmount,
                ReferenceNo = p.ReferenceNo,
                TransactionDate = p.TransactionDate
            }).OrderByDescending(x => x.PaymentId).ToList();
            return paymentModels;
        }

        public bool SavePayment(int id, PaymentModel model, out string message)
        {
            message = string.Empty;
            Payment payment = ObjectConverter<PaymentModel, Payment>.Convert(model);
            if (id > 0)
            {
                payment = context.Payments.Where(x => x.PaymentId == id).FirstOrDefault();
                if (payment == null)
                {
                    throw new Exception("Data not found!");
                }
                payment.ModifiedDate = DateTime.Now;
                payment.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                payment.CreatedDate = DateTime.Now;
                payment.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                payment.IsActive = true;
            }
            payment.CompanyId = model.CompanyId;
            payment.ProductType = model.ProductType;
            payment.VendorId = model.VendorId;
            payment.PaymentModeId = model.PaymentModeId;
            payment.InAmount = model.InAmount;
            payment.OutAmount = 0;
            payment.ReferenceNo = model.ReferenceNo;
            payment.TransactionDate = model.TransactionDate;

            payment.BankId = model.BankId;
            payment.DepositType = model.DepositType;
            payment.ReceiveLocation = model.ReceiveLocation;
            payment.MoneyReceiptNo = model.MoneyReceiptNo;
            payment.BranchName = model.BranchName;
            payment.ChequeNo = model.ChequeNo;

            context.Entry(payment).State = payment.PaymentId == 0 ? EntityState.Added : EntityState.Modified;
            try
            {
                if (context.SaveChanges() > 0)
                {
                    message = "Payment completed successfully";
                    Vendor customer = context.Vendors.Where(x => x.VendorId == payment.VendorId).FirstOrDefault();
                    customer.PaymentDue = (customer.PaymentDue - payment.InAmount);
                    try
                    {
                        context.SaveChanges();
                    }


                    catch (Exception ex)
                    {
                        context.Payments.Remove(payment);
                        context.SaveChanges();
                        return false;
                    }

                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;

            }
            return true;
        }

        public bool SaveVendor(int id, VendorModel model, out string message)
        {
            message = string.Empty;
            Vendor vendor = ObjectConverter<VendorModel, Vendor>.Convert(model);
            if (id > 0)
            {
                vendor = context.Vendors.Where(x => x.VendorId == id).FirstOrDefault();
                if (vendor == null)
                {
                    throw new Exception("Data not found!");
                }
                vendor.ModifiedDate = DateTime.Now;
                vendor.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                vendor.CreatedDate = DateTime.Now;
                vendor.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            vendor.CustomerType = model.CustomerType;
            vendor.CreditCommission = model.CreditCommission ?? 0;
            vendor.DistrictId = model.DistrictId;
            vendor.UpazilaId = model.UpazilaId;
            vendor.VendorTypeId = (int)Provider.Customer;
            vendor.ZoneId = model.ZoneId;
            vendor.SubZoneId = model.SubZoneId;
            vendor.Name = model.Name;
            vendor.ContactName = model.ContactName;
            vendor.Phone = model.Phone;
            vendor.IsActive = model.IsActive;
            vendor.Address = model.Address;
            vendor.Remarks = model.Remarks;

            context.Entry(vendor).State = vendor.VendorId == 0 ? EntityState.Added : EntityState.Modified;
            try
            {
                return context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                message = "Something went wrong when insert data";
                return false;

            }
        }
    }
}
