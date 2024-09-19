using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Implementation.Accounting;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.Vendor;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class VendorService : IVendorService
    {
        private readonly ERPEntities context;
        ConfigurationService _configurationService;
        private readonly AccountingService _accountingService;

        public VendorService(ERPEntities context)
        {
            this.context = context;
            _configurationService = new ConfigurationService(context);
            _accountingService = new AccountingService(context);

        }

        public List<VendorModel> GetAllCustomers(int vendorTypeId)
        {
            return context.Database.SqlQuery<VendorModel>("exec spGetAllCustomerList {0}", vendorTypeId).ToList();
        }

        //public IEnumerable<VendorModel> GetVendors(int companyId, int vendorTypeId, string searchText, bool isActive)
        //{
        //    IQueryable<VendorModel> vendors = context.Database.SqlQuery<VendorModel>("exec spGetVendorList {0},{1},{2}", companyId, vendorTypeId, isActive).AsQueryable();
        //    return vendors.Where(x =>
        //                  (x.Name.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText)) ||
        //                  (x.Name.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText)) ||
        //                  (x.Phone.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText)) ||
        //                  (x.Code.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText)) ||
        //                  (x.CustomerType.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText))).ToList();
        //}
        public async Task<VendorModel> GetVendors(int companyId, int vendorTypeId)
        {
            VendorModel vmCommonCustomer = new VendorModel();
            vmCommonCustomer.CompanyId = companyId;
            vmCommonCustomer.DataList = await Task.Run(() => (from t1 in context.Vendors.Where(x => x.IsActive == true && x.VendorTypeId == vendorTypeId && x.CompanyId == companyId)
                                                              join t2 in context.Upazilas on t1.UpazilaId equals t2.UpazilaId
                                                              join t3 in context.Districts on t2.DistrictId equals t3.DistrictId
                                                              join t4 in context.Divisions on t3.DivisionId equals t4.DivisionId
                                                              join t6 in context.Zones on t1.ZoneId equals t6.ZoneId
                                                              join t7 in context.HeadGLs on t1.HeadGLId equals t7.Id
                                                              join t8 in context.Head5 on t7.ParentId equals t8.Id
                                                              join t9 in context.Head4 on t8.ParentId equals t9.Id


                                                              select new VendorModel
                                                              {
                                                                  CountryId = t8.Id,
                                                                  NomineeName = t8.AccName + " " + t9.AccName,
                                                                  VendorId = t1.VendorId,
                                                                  Name = t1.Name,
                                                                  Email = t1.Email,
                                                                  ContactName = t1.ContactName,
                                                                  Address = t1.Address,
                                                                  Code = t1.Code,
                                                                  DistrictId = t2.DistrictId,
                                                                  UpazilaId = t1.UpazilaId.Value,
                                                                  DistrictName = t3.Name,
                                                                  UpazilaName = t2.Name,
                                                                  CountryName = t4.Name,
                                                                  CreatedBy = t1.CreatedBy,
                                                                  Remarks = t1.Remarks,
                                                                  CompanyId = t1.CompanyId,
                                                                  Phone = t1.Phone,
                                                                  ZoneName = t6.Name,
                                                                  CreditLimit = t1.CreditLimit,
                                                                  NID = t1.NID,
                                                                  SecurityAmount = t1.SecurityAmount,
                                                                  CustomerStatus = t1.CustomerStatus ?? 1,
                                                                  Propietor = t1.Propietor,
                                                                  HeadGLId = t1.HeadGLId,
                                                                  VendorTypeId = t1.VendorTypeId
                                                              }).OrderByDescending(x => x.VendorId).AsEnumerable());



            return vmCommonCustomer;
        }



        public VendorModel GetVendor(int id)
        {
            Vendor vendor = context.Vendors.Find(id);
            if (vendor == null)
            {
                return new VendorModel();
            }
            return ObjectConverter<Vendor, VendorModel>.Convert(vendor);
        }
        public async Task<bool> SaveVendor(int id, VendorModel model, string message)
        {
            int noOfRowsAffected = 0;
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

            if (model.Code == null)
            {
                HeadGL gl = context.HeadGLs.Find(model.HeadGLId);
                if (gl != null)
                {
                    vendor.Code = gl.AccCode;
                }
            }
            else
            {
                vendor.Code = model.Code;
            }


            vendor.HeadGLId = model.HeadGLId;
            vendor.CustomerType = model.CustomerType;
            vendor.CreditCommission = model.CreditCommission ?? 0;
            vendor.DistrictId = model.DistrictId;
            vendor.UpazilaId = model.UpazilaId;
            vendor.VendorTypeId = model.VendorTypeId;
            vendor.ZoneId = model.ZoneId;
            vendor.SubZoneId = model.SubZoneId;
            vendor.Name = model.Name;
            vendor.NID = model.NID;
            vendor.CountryId = model.CountryId;
            vendor.State = model.State;
            vendor.ContactName = model.ContactName;
            vendor.Phone = model.Phone;
            vendor.Email = model.Email;
            vendor.Address = model.Address;
            vendor.Remarks = model.Remarks;

            vendor.NomineeName = model.NomineeName;
            vendor.NomineePhone = model.NomineePhone;
            vendor.CreditRatioFrom = model.CreditRatioFrom;
            vendor.CreditRatioTo = model.CreditRatioTo;
            vendor.CreditLimit = model.CreditLimit;
            vendor.MonthlyTarget = model.MonthlyTarget;
            vendor.YearlyTarget = model.YearlyTarget;
            vendor.MonthlyIncentive = model.MonthlyIncentive;
            vendor.YearlyIncentive = model.YearlyIncentive;
            vendor.ImageUrl = model.ImageUrl;
            vendor.NomineeImageUrl = model.NomineeImageUrl;
            vendor.ClosingTime = model.ClosingTime;
            vendor.NoOfCheck = model.NoOfCheck;
            vendor.CheckNo = model.CheckNo;
            vendor.IsActive = model.IsActive;
            vendor.IsForeign = model.CountryId == 19 ? false : true;//19=Bangladesh
            context.Entry(vendor).State = vendor.VendorId == 0 ? EntityState.Added : EntityState.Modified;

            try
            {
                noOfRowsAffected = context.SaveChanges();
                if (noOfRowsAffected > 0 && model.VendorTypeId == (int)Provider.Customer)
                {
                    //Setting up Customer Offer
                    context.Database.ExecuteSqlCommand("exec spSetCustomerCommission {0},{1},{2}", vendor.CompanyId, vendor.VendorId, vendor.CreatedBy);
                }
                int ParentId = 0;


                if (vendor.CompanyId == (int)CompanyName.KrishibidFeedLimited)
                {
                    var zone = context.Zones.Find(vendor.ZoneId);
                    ParentId = zone.HeadGLId;
                    VMHeadIntegration integration = new VMHeadIntegration
                    {
                        AccName = vendor.Name,
                        LayerNo = 6,
                        Remarks = "GL Layer",
                        IsIncomeHead = false,
                        ParentId = ParentId,

                        CompanyFK = vendor.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                    };
                    HeadGL headGlId = await _configurationService.PayableHeadIntegrationAdd(integration);
                    //if (headGlId != null)
                    //{
                    //    await VendorsCodeAndHeadGLIdEdit(commonCustomer.VendorId, headGlId);
                    //}
                }

                return noOfRowsAffected > 0;
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
                message = "Data not saved";
                return false;

            }
        }

        public List<CustomerReceivableCustomModel> GetCustomerReceivables(int vendorId)
        {
            List<CustomerReceivableCustomModel> CustomerReceivable = context.Database.SqlQuery<CustomerReceivableCustomModel>("exec spGetCustomerReceiableLadger {0}", vendorId).ToList();
            return CustomerReceivable;
        }

        public List<CustomerLedgerCustomModel> GetCustomerLedger(int id)
        {
            List<CustomerLedgerCustomModel> CustomerLedgers = context.Database.SqlQuery<CustomerLedgerCustomModel>("exec spGetCustomerLedger {0}", id).ToList();
            return CustomerLedgers;
        }

        public object GetCustomerAutoComplete(string prefix, int companyId)
        {

            return context.Vendors.Where(x => x.CompanyId == companyId && x.IsActive && x.VendorTypeId == (int)Provider.Customer && x.Name.Contains(prefix)).Select(x => new
            {
                label = x.Name,
                val = x.VendorId


            }).OrderBy(x => x.label).Take(20).ToList();

        }

        public object GetCustomerAutoCompleteForShortcredit(string prefix, int companyId)
        {
            var result = context.Vendors
                .Join(
                    context.HeadGLs,
                    vendor => vendor.HeadGLId,
                    headGL => headGL.Id,
                    (vendor, headGL) => new { Vendor = vendor, HeadGL = headGL }
                )
                .Where(joined => joined.Vendor.CompanyId == companyId
                                 && joined.Vendor.IsActive
                                 && joined.Vendor.VendorTypeId == (int)Provider.Customer
                                 && (joined.Vendor.Name.Contains(prefix) || joined.HeadGL.AccCode.Contains(prefix)))
                .Select(joined => new
                {
                    label = joined.Vendor.Name + "-" + joined.HeadGL.AccCode,
                    val = joined.Vendor.VendorId
                })
                .OrderBy(x => x.label)
                .Take(20)
                .ToList();

            return result;
        }

        public object GetCustomerAutoCompleteForShortcreditInt(string prefix, int companyId)
        {
            var result = context.Vendors
                .Join(
                    context.HeadGLs,
                    vendor => vendor.HeadGLId,
                    headGL => headGL.Id,
                    (vendor, headGL) => new { Vendor = vendor, HeadGL = headGL }
                )
                .Where(joined => joined.Vendor.CompanyId == companyId
                                 && joined.Vendor.IsActive
                                 && joined.Vendor.VendorTypeId == (int)Provider.Customer
                                 && joined.HeadGL.AccCode.Contains(prefix))
                .Select(joined => new
                {
                    label = joined.Vendor.Name + "-" + joined.HeadGL.AccCode,
                    val = joined.Vendor.VendorId


                })
                .OrderBy(x => x.label)
                .Take(20)
                .ToList();

            return result;
        }





        public object GetCustomerByMarketingOfficerId(string prefix, int companyId, long? salePersonId)
        {

            return context.Vendors.Where(x => x.CompanyId == companyId && x.SalesOfficerEmpId == salePersonId && x.IsActive && x.VendorTypeId == (int)Provider.Customer && x.Name.Contains(prefix)).Select(x => new
            {
                label = x.Name,
                val = x.VendorId
            }).OrderBy(x => x.label).Take(20).ToList();

        }

        public object CustomerAssociatesCustomerId(int customerId)
        {

            return context.Vendors.Where(x => (x.VendorReferenceId == customerId || x.VendorId == customerId) && x.IsActive).Select(x => new
            {
                label = x.Name + (x.Phone != null ? " Phone: " + x.Phone : "") + (x.NID != null ? " NID: " + x.NID : ""),
                val = x.VendorId
            }).OrderBy(x => x.label).ToList();

        }
        public object GetClientAutoComplete(string prefix, int companyId)
        {

            return context.KGRECustomers.Where(x => x.CompanyId == companyId && x.FullName.Contains(prefix)).Select(x => new
            {
                label = x.FullName,
                val = x.ClientId
            }).OrderBy(x => x.label).Take(20).ToList();
        }
        public object GetSupplierAutoComplete(string prefix, int companyId)
        {
            return context.Vendors.Where(x => x.CompanyId == companyId
            && x.IsActive
            && x.VendorTypeId == 1
            && x.Name.Contains(prefix))
                .Select(x => new
                {
                    label = x.Name,
                    val = x.VendorId
                }).Take(20).ToList();
        }

        public VendorModel GetVendorByType(int id, int vendorTypeId)
        {
            Vendor vendor = context.Vendors.Where(x => x.VendorId == id && x.VendorTypeId == vendorTypeId).FirstOrDefault();

            if (vendor == null)
            {
                if (vendorTypeId == (int)Provider.Supplier)
                {
                    return new VendorModel { IsActive = true, VendorTypeId = vendorTypeId, SupplierOrCustomer = "Supplier" };
                }
                else if (vendorTypeId == (int)Provider.Customer)
                {
                    return new VendorModel
                    {
                        IsActive = true,
                        VendorTypeId = vendorTypeId,
                        SupplierOrCustomer = "Customer",

                        Condition = "Condition : If customer fails to 100% closing, any incentive, carrying and any other adjustment will not be adjusted."
                    };
                }
                else
                {
                    return new VendorModel { IsActive = true, VendorTypeId = vendorTypeId, SupplierOrCustomer = "RentCompany" };
                }


            }

            vendor.ImageUrl = vendor.ImageUrl ?? "~/Images/VendorImage/default.png";
            vendor.NomineeImageUrl = vendor.NomineeImageUrl ?? "~/Images/VendorImage/default.png";

            return ObjectConverter<Vendor, VendorModel>.Convert(vendor);
        }
        public async Task<VendorModel> GetCustomerPayments(int companyId)
        {
            VendorModel vendorModel = new VendorModel();

            vendorModel.DataList = await Task.Run(() => (from t1 in context.Vendors
                                                         join t2 in context.Payments on t1.VendorId equals t2.PaymentId
                                                         into pv
                                                         from t2 in pv.DefaultIfEmpty()

                                                         where t1.CompanyId == companyId
                                                         select new VendorModel
                                                         {
                                                             VendorId = t1.VendorId,
                                                             CompanyId = t1.CompanyId,
                                                             Name = t1.Name,
                                                             Code = t1.Code,
                                                             Address = t1.Address,
                                                             Phone = t1.Phone,
                                                             LastPaymentDate = t1.LastPaymentDate,
                                                             Balance = (t2.OutAmount ?? 0) - t2.InAmount,
                                                             IsActive = t1.IsActive,
                                                         }).AsEnumerable());

            return vendorModel;
        }
        public List<VendorModel> GetCustomerPayments(string searchText, int companyId, int customer)
        {
            return context.Database.SqlQuery<VendorModel>("spGetCustomerAccounts {0},{1}", searchText, companyId).ToList();

            //IQueryable<Vendor> vendors = context.Vendors
            //    .Include(x => x.Payments)
            //    .Where(x => x.CompanyId == companyId && x.VendorTypeId == customer && x.IsActive &&
            //    (x.Name.Contains(searchText) ||
            //     x.ContactName.Contains(searchText) || x.Phone.Contains(searchText) ||
            //     x.Code.Contains(searchText) || x.CustomerType.Contains(searchText)));
            //if (vendors == null)
            //{
            //    return new List<VendorModel>();
            //}
            //List<VendorModel> models = ObjectConverter<Vendor, VendorModel>.ConvertList(vendors.ToList()).ToList();

            //return models;
        }

        public List<SelectModel> GetVendorSelectModels(int vendorTyepId)
        {
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            return context.Vendors.Where(x => x.CompanyId == companyId && x.VendorTypeId == vendorTyepId).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.VendorId
            }).ToList();
        }

        public List<SelectModel> GetCustomerSelectModel(int companyId, int vendorType)
        {
            return context.Vendors.ToList().Where(x => x.CompanyId == companyId && x.VendorTypeId == vendorType).OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = string.Format("[{0}] {1}", x.Code, x.Name),
                Value = x.VendorId
            }).OrderBy(x => x.Text).ToList();
        }


        public string GetAutoGeneratedVendorCode(int companyId, int upazilaId, int vendorTypeId)
        {
            string code = string.Empty;
            Vendor lastVendor = context.Vendors.Where(x => x.CompanyId == companyId && x.UpazilaId == upazilaId && x.VendorTypeId == vendorTypeId).OrderByDescending(x => x.Code).FirstOrDefault();

            string customerPrefix = vendorTypeId == 1 ? "S" : "C";
            if (lastVendor == null)
            {
                Upazila upazila = context.Upazilas.Find(upazilaId);
                return customerPrefix + upazila.Code + "01";

            }
            code = GenerateCustomerCode(lastVendor.Code);


            return code;
        }

        private string GenerateCustomerCode(string lastVendorCode)
        {
            string prefix = lastVendorCode.Substring(0, 5);
            int code = Convert.ToInt32(lastVendorCode.Substring(5, 2));
            code = ++code;
            return prefix + code.ToString().PadLeft(2, '0');

        }

        public VendorModel GetSupplier(int supplierId)
        {
            Vendor vendor = context.Vendors.Find(supplierId);
            if (vendor == null)
            {
                return new VendorModel();
            }
            VendorModel model = ObjectConverter<Vendor, VendorModel>.Convert(vendor);
            model.Message = context.Database.SqlQuery<string>(@"exec spPurchaseOrderOpenBySupplier {0}", model.VendorId).FirstOrDefault();
            return model;
        }

        public VendorModel GetVendorPaymentStatus(int id)
        {
            Vendor vendor = context.Vendors.Find(id);
            if (vendor == null)
            {
                return new VendorModel();
            }

            vendor.PaymentDue = context.Database.SqlQuery<decimal>(@"select isnull( cast ((sum(OutAmount)-sum(InAmount)) as decimal),0) as PaymentDue from Erp.Payment
where VendorId={0}", vendor.VendorId).ToList().FirstOrDefault();
            return ObjectConverter<Vendor, VendorModel>.Convert(vendor);
        }

        public List<MonthSelectModel> GetMonthSelectModes()
        {
            return new List<MonthSelectModel>
            {
                new MonthSelectModel{Text="January",Value="January" },
                new MonthSelectModel{Text="February",Value="February" },
                new MonthSelectModel{Text="March",Value="March" },
                new MonthSelectModel{Text="April",Value="April" },
                new MonthSelectModel{Text="May",Value="May" },
                new MonthSelectModel{Text="June",Value="June" },
                new MonthSelectModel{Text="July",Value="July" },
                new MonthSelectModel{Text="August",Value="August" },
                new MonthSelectModel{Text="September",Value="September" },
                new MonthSelectModel{Text="October",Value="October" },
                new MonthSelectModel{Text="November",Value="November" },
                new MonthSelectModel{Text="December",Value="December" },
            };
        }

        public bool BulkSave(List<VendorModel> models)
        {
            if (!models.Any())
            {
                return false;
            }
            List<Vendor> vendors = ObjectConverter<VendorModel, Vendor>.ConvertList(models.ToList()).ToList();
            context.Vendors.AddRange(vendors);
            return context.SaveChanges() > 0;
        }

        public List<SelectModel> GetCustomerSelectModelsByCompany(int companyId, int customer)
        {
            return context.Vendors.OrderBy(x => x.Name).Where(x => x.CompanyId == companyId && x.VendorTypeId == customer && x.IsActive).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.VendorId
            }).ToList();

        }


        public List<SelectModel> GetCustomerNameSelectModel(int? companyId, int customer)
        {
            return context.Vendors.ToList().Where(x => x.CompanyId == companyId && x.VendorTypeId == customer).OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.VendorId
            }).OrderBy(x => x.Text).ToList();
        }

        public bool DeleteVendor(int id)
        {
            int noOfRowsDeleted = 0;
            Vendor vendor = context.Vendors.Where(x => x.VendorId == id).First();
            if (vendor == null)
            {
                return false;
            }
            context.Vendors.Remove(vendor);
            try
            {
                noOfRowsDeleted = context.SaveChanges();
            }
            catch (Exception)
            {

                noOfRowsDeleted = 0;
                return noOfRowsDeleted > 0;
            }
            return noOfRowsDeleted > 0;

        }

        public List<SelectModel> GetCustomerSelectModels(int companyId, string productType)
        {
            List<SelectModel> selectModels = new List<SelectModel>();
            selectModels.Add(new SelectModel { Text = "ALL", Value = 0 });
            List<VendorModel> customers = context.Database.SqlQuery<VendorModel>(@"SELECT     DISTINCT 
                                                                            V.VendorId,
	  	                                                                    V.Name 
                                                                  FROM       Erp.OrderDeliver OD
                                                                  INNER JOIN Erp.OrderDeliverDetail ODD ON OD.OrderDeliverId = ODD.OrderDeliverId
                                                                  INNER JOIN Erp.OrderMaster OM ON OD.OrderMasterId = OM.OrderMasterId
                                                                  INNER JOIN Erp.Vendor V ON OM.CustomerId = V.VendorId
                                                                  WHERE OD.ProductType = {0} AND OD.CompanyId = {1} 
                                                                  ORDER BY V.Name", productType, companyId).ToList();



            List<SelectModel> customerSelectModels = customers.Select(x => new SelectModel { Text = x.Name, Value = x.VendorId }).ToList();
            selectModels.AddRange(customerSelectModels);
            return selectModels;
        }

        public object GetRentCompanyAutoComplete(string prefix, int companyId)
        {
            return context.Vendors.Where(x => x.CompanyId == companyId && x.IsActive && x.VendorTypeId == 3 && x.Name.Contains(prefix)).Select(x => new
            {
                label = x.Name,
                val = x.VendorId
            }).Take(20).ToList();
        }

        public async Task<VendorDeedListVm> GetAllVendorDeed(int companyId, int customerId)
        {
            var dataList = new VendorDeedListVm();
            dataList.DataList = await Task.Run(() => (from t1 in context.VendorDeeds
                                                      join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                                      where t1.CompanyId == companyId && t2.VendorId == customerId
                                                      && t1.IsActive
                                                      select new VendorDeedVm
                                                      {
                                                          CompanyId = t1.CompanyId,
                                                          VendorDeedId = t1.VendorDeedId,
                                                          MonthlyTarget = t1.MonthlyTarget ?? 0,
                                                          YearlyTarget = t1.YearlyTarget ?? 0,
                                                          VendorId = t1.VendorId,
                                                          VendorName = t2.Name,
                                                          CreditRatioFrom = t1.CreditRatioFrom ?? 0,
                                                          CreditRatioTo = t1.CreditRatioTo ?? 0,
                                                          CreditLimit = t1.CreditLimit ?? 0,
                                                          Days = t1.Days ?? 0,
                                                          Transport = t1.Transport ?? 0,
                                                          ClosingDate = t1.ClosingDate,
                                                          ExtraCondition1 = t1.ExtraCondition1 ?? 0,
                                                          ExtraBenifite = t1.ExtraBenifite ?? 0,
                                                          DepositRate = t1.DepositRate ?? 0
                                                      }
                                            ).OrderByDescending(o => o.VendorDeedId).AsEnumerable());
            dataList.CompanyId = companyId;
            return dataList;
        }

        public async Task<int> SaveVendorDeed(VendorDeedVm model)
        {

            try
            {
                var obj = await context.VendorDeeds.SingleOrDefaultAsync(q => q.VendorDeedId == model.VendorDeedId);

                if (obj == null)
                {
                    obj = new VendorDeed()
                    {
                        CompanyId = model.CompanyId,
                        VendorId = model.VendorId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };

                    context.VendorDeeds.Add(obj);

                }
                else
                {
                    obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    obj.ModifiedDate = DateTime.Now;
                }
                obj.MonthlyTarget = model.MonthlyTarget;
                obj.YearlyTarget = model.YearlyTarget;
                obj.CreditRatioFrom = model.CreditRatioFrom;
                obj.CreditRatioTo = model.CreditRatioTo;
                obj.CreditLimit = model.CreditLimit;
                obj.Days = model.Days;
                obj.Transport = model.Transport;
                obj.ClosingDate = model.ClosingDate;
                obj.ExtraCondition1 = model.ExtraCondition1;
                obj.ExtraBenifite = model.ExtraBenifite;
                obj.DepositRate = model.DepositRate;

                await context.SaveChangesAsync();

                return obj.VendorDeedId;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<VendorDeedVm> GetVendorDeedById(int vendorDeedId, int customerId)
        {
            var data = new VendorDeedVm();
            data = await Task.Run(() => (from t1 in context.VendorDeeds
                                         join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                         where t1.VendorDeedId == vendorDeedId && t1.VendorId == customerId
                                         select new VendorDeedVm
                                         {
                                             CompanyId = t1.CompanyId,
                                             VendorDeedId = t1.VendorDeedId,
                                             MonthlyTarget = t1.MonthlyTarget ?? 0,
                                             YearlyTarget = t1.YearlyTarget ?? 0,
                                             VendorId = t1.VendorId,
                                             CreditRatioFrom = t1.CreditRatioFrom ?? 0,
                                             CreditRatioTo = t1.CreditRatioTo ?? 0,
                                             CreditLimit = t1.CreditLimit ?? 0,
                                             Days = t1.Days ?? 0,
                                             Transport = t1.Transport ?? 0,
                                             ClosingDate = t1.ClosingDate,
                                             ExtraCondition1 = t1.ExtraCondition1 ?? 0,
                                             ExtraBenifite = t1.ExtraBenifite ?? 0,
                                             DepositRate = t1.DepositRate ?? 0,
                                             VendorName = t2.Name
                                         }).SingleOrDefaultAsync());
            data.ClosingDateText = data.ClosingDate == null ? "" : data.ClosingDate.Value.ToShortDateString();
            return data;
        }

        public async Task<bool> RemoveVendorDeed(int vendorDeedId)
        {

            var obj = await context.VendorDeeds.SingleOrDefaultAsync(q => q.VendorDeedId == vendorDeedId);

            if (obj == null)
            {
                return false;
            }
            else
            {
                obj.IsActive = false;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;
                await context.SaveChangesAsync();
                return true;
            }
        }


        #region Vendor Adjustment GCCL

        public List<SelectModelType> GetZonewiseCustomerSelectModels(int? zoneId, int? subZoneId)
        {
            List<SelectModelType> selectModels = new List<SelectModelType>();
            selectModels = (from z in context.Zones
                            join subzone in context.SubZones on z.ZoneId equals subzone.ZoneId
                            join c in context.Vendors on z.ZoneId equals c.ZoneId
                            where z.IsActive && c.IsActive
                            && (zoneId == null || c.ZoneId == zoneId)
                            && (subzone == null || c.SubZoneId == subZoneId)
                            select new { Id = c.VendorId, Name = c.Name }).Select(x => new SelectModelType()
                            {
                                Text = x.Name,
                                Value = x.Id
                            }).OrderBy(x => x.Text).ToList();
            return selectModels;
        }

        public MethodFeedbackVM InsertVendorAdjustment(VendorAdjustmentVM model)
        {
            MethodFeedbackVM result = new MethodFeedbackVM();
            var vendorAdjustment = new VendorAdjustment()
            {
                Debit = model.Debit,
                Credit = model.Credit,
                IsExisting = model.IsExisting,
                OrderDeliverId = model.OrderDeliverId,
                VendorId = model.VendorId,
                Narration = model.Narration,
                IsActive = true,
                CreatedBy = Common.GetUserId(),
                CreateDate = DateTime.Now,
            };
            context.VendorAdjustments.Add(vendorAdjustment);
            if (context.SaveChanges() > 0)
            {
                result.Status = true;
                result.Message = "Customer Adjustment has been successfully saved";
                return result;
            }
            result.Message = "Sorry! Unable to Save Customer Adjustment";
            return result;
        }
        public MethodFeedbackVM UpdateVendorAdjustment(VendorAdjustmentVM model)
        {
            MethodFeedbackVM result = new MethodFeedbackVM();
            var exist = context.VendorAdjustments.FirstOrDefault(x => x.IsActive && x.AdjustmentId == model.AdjustmentId);
            if (model.AdjustmentId <= 0 || exist == null)
            {
                result.Message = "Sorry! No record found";
                return result;
            }
            exist.Debit = model.Debit;
            exist.Credit = model.Credit;
            exist.Narration = model.Narration;
            exist.VendorId = model.VendorId;
            exist.IsActive = true;
            exist.ModifiedDate = DateTime.Now;
            exist.IsExisting = model.IsExisting;
            exist.OrderDeliverId = model.OrderDeliverId;
            exist.ModifiedBy = Common.GetUserId();
            if (context.SaveChanges() > 0)
            {
                result.Status = true;
                result.Message = "Customer Adjustment has been successfully updated";
                return result;
            }
            result.Message = "Sorry! Unable to update Customer Adjustment";
            return result;
        }
        public MethodFeedbackVM UpdateVendorAdjustmentStatus(VendorAdjustmentVM model)
        {
            MethodFeedbackVM result = new MethodFeedbackVM();
            var exist = context.VendorAdjustments.Find(model.AdjustmentId);
            if (model.AdjustmentId <= 0 || exist == null)
            {
                result.Message = "Sorry! No record found";
                return result;
            }

            if (model.ActionId == (int)ActionEnum.Approve)
            {

                exist.IsSubmit = true;
                exist.ModifiedDate = DateTime.Now;
                exist.ModifiedBy = Common.GetUserId();

            }
            else if (model.ActionId == (int)ActionEnum.Delete)
            {
                exist.IsActive = false;
                exist.ModifiedDate = DateTime.Now;
                exist.ModifiedBy = Common.GetUserId();
            }

            if (context.SaveChanges() > 0)
            {
                #region Accounts Intrigration
                if (!exist.IsExisting)
                {
                    var Accdata = GetVendorAdjustmentById(exist.AdjustmentId);
                    var v = _accountingService.CustomerAdjustmentPushGccl(Accdata.UserCompnayId, Accdata, (int)GCCLJournalEnum.JournalVoucher);

                }
                #endregion

                result.Status = true;
                result.Message = "Customer Adjustment has been successfully submitted";
                return result;
            }
            result.Message = "Sorry! Unable to submitted Customer Adjustment";
            return result;
        }

        public VendorAdjustmentVM GetVendorAdjustmentList(int companyId)
        {
            VendorAdjustmentVM vendorAdjustment = new VendorAdjustmentVM();
            vendorAdjustment.VendorAdjustmentList = (from t1 in context.VendorAdjustments
                                                     join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                                     join t3 in context.SubZones on t2.SubZoneId equals t3.SubZoneId
                                                     join t4 in context.Zones on t3.ZoneId equals t4.ZoneId
                                                     where t1.IsActive && t2.CompanyId == companyId
                                                     select new VendorAdjustmentVM()
                                                     {
                                                         AdjustmentId = t1.AdjustmentId,
                                                         VendorId = t1.VendorId,
                                                         VendorName = t2.Name,
                                                         Debit = t1.Debit,
                                                         Credit = t1.Credit,
                                                         CreatedBy = t1.CreatedBy,
                                                         CreateDate = t1.CreateDate,
                                                         Narration = t1.Narration,
                                                         ZoneName = t4.Name,
                                                         SubZoneName = t3.Name,
                                                         IsSubmit = t1.IsSubmit
                                                     }).OrderByDescending(x => x.AdjustmentId).AsEnumerable();
            return vendorAdjustment;

        }
        public VendorAdjustmentVM GetVendorAdjustmentById(long adjustmentId)
        {
            VendorAdjustmentVM vendorAdjustment = new VendorAdjustmentVM();
            vendorAdjustment = (from t1 in context.VendorAdjustments
                                join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                join t3 in context.SubZones on t2.SubZoneId equals t3.SubZoneId
                                join t4 in context.Zones on t3.ZoneId equals t4.ZoneId
                                where t1.IsActive && t1.AdjustmentId == adjustmentId
                                select new VendorAdjustmentVM()
                                {
                                    HeadGLId = t2.HeadGLId,
                                    AdjustmentId = t1.AdjustmentId,
                                    VendorId = t1.VendorId,
                                    VendorName = t2.Name,
                                    Debit = t1.Debit,
                                    Credit = t1.Credit,
                                    CreatedBy = t1.CreatedBy,
                                    CreateDate = t1.CreateDate,
                                    Narration = t1.Narration,
                                    ZoneName = t4.Name,
                                    SubZoneName = t3.Name,
                                    IsSubmit = t1.IsSubmit,
                                    IntegratedFrom = "VendorAdjustment"
                                }).FirstOrDefault();
            return vendorAdjustment;

        }

        #endregion Vendor Adjust ment
    }
}
