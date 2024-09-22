using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class EmiService : IEmiService
    {
        private bool disposed = false;
        private readonly ERPEntities context;
        public EmiService(ERPEntities context)
        {
            this.context = context;
        }

        public List<EMIModel> GetEmiInfoList()
        {
            var emi = context.EMIs.Include(x => x.Vendor).ToList();
            return ObjectConverter<EMI, EMIModel>.ConvertList(emi).ToList();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public List<SelectModel> GetOrderinvoiceByCustomer(int customerId, int companyId)
        {
            List<OrderMasterModel> orderNo = context.Database.SqlQuery<OrderMasterModel>(@"select OrderMasterId,OrderNo from Erp.OrderMaster where   CompanyId ={0} and CustomerId={1} and  OrderMasterId not in(select OrderId from Erp.EMI where  CompanyId={0} and VendorId={1})", companyId, customerId).ToList();

            return orderNo.Select(x => new SelectModel { Text = x.OrderNo, Value = x.OrderMasterId }).OrderBy(x => x.Text).ToList();

        }


        public decimal GetSalesValue(int orderId)
        {
            var salesValue = context.OrderMasters.Where(x => x.OrderMasterId == orderId).Select(x => x.GrandTotal).FirstOrDefault();
            return salesValue ?? 0;
        }




        public List<EmiDetailModel> GetEmiDetails(DateTime installmentDate, int noOfInstallment, int installmentAmount)
        {
            List<EmiDetailModel> vm = new List<EmiDetailModel>();
            for (int i = 0; i < noOfInstallment; i++)
            {
                var emi = new EmiDetailModel()
                {
                    InstallmentDate = Convert.ToDateTime(installmentDate.AddMonths(i).ToString("dd-MMM-yyyy")),
                    InstallmentAmount = installmentAmount
                };
                vm.Add(emi);
            }
            return vm;
        }

        public bool SaveOrEditEmi(EMIModel model)
        {

            EMI emi = ObjectConverter<EMIModel, EMI>.Convert(model);


            if (model.EmiId > 0)
            {
                emi = context.EMIs.FirstOrDefault(x => x.EmiId == model.EmiId);
                if (emi == null)
                {
                    throw new Exception("EMI not found!");
                }
                emi.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                emi.ModifiedDate = DateTime.Now;
                context.EmiDetails.RemoveRange(emi.EmiDetails);
                foreach (var ed in model.EmiDetails)
                {

                    var detail = new EmiDetail
                    {
                        IsPaid = 0,
                        InstallmentDate = ed.InstallmentDate,
                        InstallmentAmount = ed.InstallmentAmount,
                        EmiId = model.EmiId,

                    };
                    context.EmiDetails.Add(detail);

                }
                context.SaveChanges();

            }
            else
            {

                emi.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                emi.CreatedDate = DateTime.Now;

            }

            emi.CompanyId = model.CompanyId;
            emi.VendorId = model.VendorId;
            emi.OrderId = model.OrderId;
            emi.SaleValue = model.SaleValue;
            emi.Dp = model.Dp;
            emi.DpValue = model.DpValue;
            emi.OutStandingPrinciple = model.OutStandingPrinciple;
            emi.NoOfInstallment = model.NoOfInstallment;
            emi.FlatRatePerYear = model.FlatRatePerYear;
            emi.BankCharge = model.BankCharge;
            emi.NetOutStanding = model.NetOutStanding;
            emi.InstallmentAmount = model.InstallmentAmount;
            emi.InstallmentStartDate = model.InstallmentStartDate;
            emi.Vendor = null;

            context.Entry(emi).State = emi.EmiId == 0 ? EntityState.Added : EntityState.Modified;
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {

                return false;
            }


            return true;

        }




        public EMIModel GetEmi(long id)
        {
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            string emiNo = string.Empty;
            if (id <= 0)
            {
                IQueryable<EMI> emis = context.EMIs.Where(x => x.CompanyId == companyId);
                int count = emis.Count();
                if (count == 0)
                {
                    return new EMIModel()
                    {
                        EMINo = GenerateSequenceNumber(0),
                        CompanyId = companyId
                    };
                }

                emis = emis.Where(x => x.CompanyId == companyId).OrderByDescending(x => x.EmiId).Take(1);
                emiNo = emis.ToList().FirstOrDefault().EMINo;
                long lastEMINo = Convert.ToInt64(emiNo.Substring(1, 8));
                emiNo = GenerateSequenceNumber(lastEMINo);
                return new EMIModel()
                {
                    EMINo = emiNo,
                    CompanyId = companyId
                };

            }
            EMI emi = context.EMIs.Include(x => x.EmiDetails).Include(x => x.Vendor).Include(x => x.OrderMaster).Where(x => x.EmiId == id).FirstOrDefault();
            if (emi == null)
            {
                throw new Exception("Store not found");
            }
            EMIModel model = ObjectConverter<EMI, EMIModel>.Convert(emi);
            return model;
        }
        private string GenerateSequenceNumber(long lastEMINo)
        {
            string input = string.Empty;
            long num = ++lastEMINo;
            input = num.ToString();
            if (input != string.Empty)
            {
                num = Convert.ToInt32(input);
            }
            return "E" + num.ToString().PadLeft(8, '0');
        }

    }
}
