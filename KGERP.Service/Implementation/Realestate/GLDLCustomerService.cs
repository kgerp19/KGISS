using KGERP.Data.Models;
using KGERP.Service.Implementation.Accounting;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.FTP_Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate
{
    public class GLDLCustomerService : IGLDLCustomerService
    {
        private bool disposed = false;
        ERPEntities context = new ERPEntities();

        public async Task<string> GenerateBookingNo(int ProductId)
        {
            var lastBookings = context.ProductBookingInfoes.Count() + 1;

            string count = lastBookings == 0 ? "000001" : lastBookings.ToString().PadLeft(6, '0');
            var Data = await context.Products.Include(e => e.ProductCategory).Include(o => o.ProductSubCategory).SingleOrDefaultAsync(i => i.ProductId == ProductId);
            var BookingNo = (Data.ProductCategory.Name != Data.ProductSubCategory.Name ? Data.ProductCategory.Name + "-" + Data.ProductSubCategory.Name : Data.ProductCategory.Name) + "-" + Data.ProductName + "-" + count;
            return BookingNo;
        }
        public async Task<HeadGL> CustomerHeadIntegrationAdd(VMHeadIntegration vmHeadIntegration)
        {
            long result = -1;

            string newAccountCode = "";
            int orderNo = 0;

            Head5 parentHead = context.Head5.Where(x => x.Id == vmHeadIntegration.ParentId).FirstOrDefault();

            IQueryable<HeadGL> childHeads = context.HeadGLs.Where(x => x.ParentId == vmHeadIntegration.ParentId);

            if (childHeads.Count() > 0)
            {
                string lastAccCode = childHeads.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                string parentPart = lastAccCode.Substring(0, 10);
                string childPart = lastAccCode.Substring(10, 3);
                newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');
                orderNo = childHeads.Count();
            }

            else
            {
                newAccountCode = parentHead.AccCode + "001";
                orderNo = orderNo + 1;
            }


            HeadGL headGL = new HeadGL
            {
                Id = context.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = newAccountCode,
                LayerNo = vmHeadIntegration.LayerNo,

                CompanyId = vmHeadIntegration.CompanyFK,
                CreateDate = vmHeadIntegration.CreatedDate,
                CreatedBy = vmHeadIntegration.CreatedBy,
                AccName = vmHeadIntegration.AccName,
                ParentId = vmHeadIntegration.ParentId,
                OrderNo = orderNo,
                IsActive = true,
                Remarks = vmHeadIntegration.Remarks
            };
            context.HeadGLs.Add(headGL);
            if (await context.SaveChangesAsync() > 0)
            {
                result = headGL.Id;
            }
            return headGL;
        }
        public async Task<GLDLBookingViewModel> UpdateCustomerBokking(GLDLBookingViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                ProductBookingInfo pBooking =  context.ProductBookingInfoes.FirstOrDefault(f => f.CGId == model.CGId);
                CustomerGroupInfo groupInfo =  context.CustomerGroupInfoes.FirstOrDefault(f => f.CGId == model.CGId);

                try
                {
                    groupInfo.GroupName = model.CustomerGroupName;
                    groupInfo.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    groupInfo.ModifiedDate = DateTime.Now;
                    groupInfo.PrimaryContactAddr = model.PrimaryContactAddr;
                    groupInfo.PrimaryContactNo = model.PrimaryContactNo;
                    groupInfo.PrimaryEmail = model.PrimaryEmail;
                    groupInfo.PrimaryClientId = model.PrimaryClientId;
                    context.Entry(groupInfo).State = EntityState.Modified;
                    context.SaveChanges();
                    HeadGL headGlId = context.HeadGLs.FirstOrDefault(f=>f.Id==groupInfo.HeadGLId);
                    headGlId.AccName = model.ClientName + "(" + model.FileNo + ")";
                    context.Entry(headGlId).State = EntityState.Modified;
                    context.SaveChanges();
                    pBooking.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    pBooking.ModifiedDate = DateTime.Now;
                    pBooking.FileNo = model.FileNo;
                    pBooking.SoldBy = model.EmployeeId;
                    pBooking.ApplicationDate = model.ApplicationDate;   
                    pBooking.BookingDate = model.BookingDate;   
                    pBooking.TeamLeadId = model.TeamLeadId;
                    context.Entry(pBooking).State = EntityState.Modified;
                    context.SaveChanges();
                    scope.Commit();
                    return model;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return model;
                }
            }
        }
        public async Task<GLDLBookingViewModel> CustomerBokking(GLDLBookingViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    VMHeadIntegration integration = new VMHeadIntegration();
                    HeadGL headGlId = new HeadGL();
                    Vendor vendor = context.Vendors.FirstOrDefault(f => f.VendorId == model.ClientId);
                    if (vendor.HeadGLId > 0)
                    {
                        headGlId.Id = (int)vendor.HeadGLId;

                        headGlId = context.HeadGLs.Find((int)vendor.HeadGLId);
                        headGlId.AccName = headGlId.AccName + "(" + model.FileNo + ")";
                        headGlId.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        headGlId.ModifiedDate = DateTime.Now;
                    }

                    // change
                    else
                    {
                        if (model.CompanyId == (int)CompanyName.KrishibidPropertiesLimited)
                        {
                            var productCategorie = context.ProductCategories.FirstOrDefault(x => x.ProductCategoryId == model.ProductCategoryId);
                            integration = new VMHeadIntegration
                            {
                                AccName = model.ClientName + "(" + model.FileNo + ")",
                                LayerNo = 6,
                                Remarks = "GL Layer",
                                IsIncomeHead = false,
                                ParentId = productCategorie.AccountingHeadId.Value,

                                CompanyFK = productCategorie.CompanyId,
                                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                CreatedDate = DateTime.Now,
                            };
                        }
                        if (model.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited)
                        {
                            var productSubCategorie = context.ProductSubCategories.FirstOrDefault(x => x.ProductSubCategoryId == model.ProductSubCategoryId);
                            integration = new VMHeadIntegration
                            {
                                AccName = model.ClientName + "(" + model.FileNo + ")",
                                LayerNo = 6,
                                Remarks = "GL Layer",
                                IsIncomeHead = false,
                                ParentId = productSubCategorie.AccountingHeadId.Value,

                                CompanyFK = productSubCategorie.CompanyId,
                                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                CreatedDate = DateTime.Now,
                            };
                        }

                        headGlId = await CustomerHeadIntegrationAdd(integration);
                    }

                    //Customer Group Info
                    CustomerGroupInfo customerGroupInfo = MapCustomerGroupInfo(model);
                    customerGroupInfo.HeadGLId = headGlId.Id;
                    context.CustomerGroupInfoes.Add(customerGroupInfo);
                    context.SaveChanges();



                    //Customer Group Info
                    List<CustomerGroupMapping> groupMapping = MapCustomerGroup(model, customerGroupInfo.CGId);
                    context.CustomerGroupMappings.AddRange(groupMapping);
                    context.SaveChanges();
                    model.BookingNo = await this.GenerateBookingNo(model.ProductId.Value);

                    //ProductBookingInfo
                    ProductBookingInfo BookingInfo = MapProductBooking(model, customerGroupInfo.CGId);
                    context.ProductBookingInfoes.Add(BookingInfo);
                    context.SaveChanges();

                    //Booking Cost Mapping
                    List<BookingCostMapping> costMapping = GetCostMapping(model, BookingInfo.BookingId);
                    context.BookingCostMappings.AddRange(costMapping);
                    context.SaveChanges();
                     

                    //Booking Installment Schedule
                    List<BookingInstallmentSchedule> schedules = ConvertShortToDbModel(customerGroupInfo.CGId, BookingInfo.BookingId, model.Schedule);
                    context.BookingInstallmentSchedules.AddRange(schedules);
                    context.SaveChanges();


                    // product Update



                    scope.Commit();
                    model.CGId = customerGroupInfo.CGId;
                    return model;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return model;
                }
            }
        }
        public async Task<GLDLBookingViewModel> SPCustomerBokking(GLDLBookingViewModel model)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    VMHeadIntegration integration = new VMHeadIntegration();
                    HeadGL headGlId = new HeadGL();
                    Vendor vendor = context.Vendors.FirstOrDefault(f => f.VendorId == model.ClientId);
                    if (vendor.HeadGLId > 0)
                    {
                        headGlId.Id = (int)vendor.HeadGLId;

                        headGlId = context.HeadGLs.Find((int)vendor.HeadGLId);
                        headGlId.AccName = headGlId.AccName + "(" + model.FileNo + ")";
                        headGlId.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        headGlId.ModifiedDate = DateTime.Now;
                    }

                    // change
                    else
                    {
                        if (model.CompanyId == (int)CompanyName.KrishibidPropertiesLimited)
                        {
                            var productCategorie = context.ProductCategories.FirstOrDefault(x => x.ProductCategoryId == model.ProductCategoryId);
                            integration = new VMHeadIntegration
                            {
                                AccName = model.ClientName + "(" + model.FileNo + ")",
                                LayerNo = 6,
                                Remarks = "GL Layer",
                                IsIncomeHead = false,
                                ParentId = productCategorie.AccountingHeadId.Value,

                                CompanyFK = productCategorie.CompanyId,
                                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                CreatedDate = DateTime.Now,
                            };
                        }
                        if (model.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited)
                        {
                            var productSubCategorie = context.ProductSubCategories.FirstOrDefault(x => x.ProductSubCategoryId == model.ProductSubCategoryId);
                            integration = new VMHeadIntegration
                            {
                                AccName = model.ClientName + "(" + model.FileNo + ")",
                                LayerNo = 6,
                                Remarks = "GL Layer",
                                IsIncomeHead = false,
                                ParentId = productSubCategorie.AccountingHeadId.Value,

                                CompanyFK = productSubCategorie.CompanyId,
                                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                CreatedDate = DateTime.Now,
                            };
                        }

                        headGlId = await CustomerHeadIntegrationAdd(integration);
                    }

                    //Customer Group Info
                    CustomerGroupInfo customerGroupInfo = MapCustomerGroupInfo(model);
                    customerGroupInfo.HeadGLId = headGlId.Id;
                    context.CustomerGroupInfoes.Add(customerGroupInfo);
                    context.SaveChanges();



                    //Customer Group Info
                    List<CustomerGroupMapping> groupMapping = MapCustomerGroup(model, customerGroupInfo.CGId);
                    context.CustomerGroupMappings.AddRange(groupMapping);
                    context.SaveChanges();
                    model.BookingNo = await this.GenerateBookingNo(model.ProductId.Value);

                    //ProductBookingInfo
                    ProductBookingInfo BookingInfo = MapSeaPalaceProductBooking(model, customerGroupInfo.CGId);
                    context.ProductBookingInfoes.Add(BookingInfo);
                    context.SaveChanges();

                    //Booking Cost Mapping
                    //List<BookingCostMapping> costMapping = GetCostMapping(model, BookingInfo.BookingId);
                    //context.BookingCostMappings.AddRange(costMapping);
                    //context.SaveChanges();

                    //Booking Installment Schedule
                    List<BookingInstallmentSchedule> schedules = ConvertShortToDbModel(customerGroupInfo.CGId, BookingInfo.BookingId, model.Schedule);
                    context.BookingInstallmentSchedules.AddRange(schedules);
                    context.SaveChanges();


                    // product Update



                    scope.Commit();
                    model.CGId = customerGroupInfo.CGId;
                    return model;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return model;
                }
            }
        }

        public async Task<GLDLBookingViewModel> PductChangingStatus(GLDLBookingViewModel model)
        {
            var obj =context.Products.Where(p=>p.ProductId==model.ProductId).FirstOrDefault();
            if (obj != null)
            {
                if (obj.CompanyId == 7)
                {
                    obj.Status = (int)ProductStatusEnumGLDL.UnSold;
                }
                else
                {
                    obj.Status = (int)ProductStatusEnumKPL.VacantFlat;
                }        
                context.SaveChanges();
            }
            return model;
        
        }

            private List<BookingInstallmentSchedule> ConvertShortToDbModel(long cGId, long bookingId, List<InstallmentScheduleShortModel> schedule)
        {
            List<BookingInstallmentSchedule> model = new List<BookingInstallmentSchedule>();
            foreach (var sm in schedule)
            {

                sm.InstallmentDate = Convert.ToDateTime(sm.StringDate);
                BookingInstallmentSchedule m = new BookingInstallmentSchedule()
                {
                    Amount = sm.PayableAmount,
                    BookingId = bookingId,
                    CGID = cGId,
                    CreatedBy = "",
                    CreatedDate = DateTime.Now,
                    Date = sm.InstallmentDate,
                    InstallmentId = 0,
                    IsActive = true,
                    IsLate = false,
                    IsPaid = sm.PaidAmount > 0 ? true : false,
                    IsPartlyPaid = sm.PayableAmount > sm.PaidAmount ? false : true,
                    PaidAmount = sm.PaidAmount,
                    Remarks = "",
                    InstallmentTitle = sm.Title,
                    InstallmentTypeId = sm.InstallmentId
                };
                model.Add(m);
            }
            return model;

        }

        private List<BookingCostMapping> GetCostMapping(GLDLBookingViewModel model, long bookingId)
        {
            List<BookingCostMapping> models = new List<BookingCostMapping>();
            foreach (var item in model.LstPurchaseCostHeads)
            {
                BookingCostMapping costMapping = new BookingCostMapping();
                costMapping.BookingId = bookingId;
                costMapping.Amount = item.Amount;
                costMapping.CostId = item.CostId;
                costMapping.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                costMapping.CreatedDate = DateTime.Now;
                costMapping.IsActive = true;
                costMapping.IsSnstallmentInclude = item.IsSnstallmentInclude;
                costMapping.Percentage = item.Percentage;
                models.Add(costMapping);
            }
            return models;
        }

        private ProductBookingInfo MapProductBooking(GLDLBookingViewModel model, long cGId)
        {
            ProductBookingInfo pBooking = new ProductBookingInfo();
            pBooking.CGId = cGId;
            pBooking.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            pBooking.CreatedDate = DateTime.Now;
            pBooking.ProductId = (int)model.ProductId;
            pBooking.BookingAmt = model.BookingMoney;
            pBooking.DiscountPercentage = model.Discount;
            pBooking.InstallmentTypeId = model.BookingInstallmentTypeId == 0 ? model.BookingInstallmentTypeManualId : model.BookingInstallmentTypeId;
            pBooking.SoldBy = model.EmployeeId;
            pBooking.Step = 1;
            pBooking.Status = 1;
            pBooking.EntryBy = model.EntryBy;
            pBooking.OtherInformation = model.OtherInformation;
            pBooking.RatePerKatha = model.RatePerKatha;
            pBooking.RestofAmount = model.RestofAmount;
            pBooking.ApplicationDate = model.ApplicationDate;
            pBooking.IsActive = true;
            pBooking.BookingNo = model.BookingNo;
            pBooking.TeamLeadId = model.TeamLeadId;
            pBooking.SpecialDiscountAmt = model.SpecialDiscountAmt;
            pBooking.BookingDate = model.BookingDate;
            pBooking.FileNo = model.FileNo;
            pBooking.LandValue = model.LandValue;
            pBooking.FileStatus = 1;
            pBooking.InstallmentAmount = model.InstallmentAmount;
            pBooking.Remarks = model.Remarks;
            pBooking.ResaleLogId = model.PurchaseAmount > 0 ? model.CGId : 0;
            pBooking.PurchaseAmount = model.PurchaseAmount;
            pBooking.DownPayment = model.DownPayment;
            pBooking.DownPaymentDate = model.DownPaymentDate;
            pBooking.BookingMoneyPaymentDate = model.BookingMoneyPaymentDate;
            pBooking.IsSeaPalace = 0;
            return pBooking;
        }
        private ProductBookingInfo MapSeaPalaceProductBooking(GLDLBookingViewModel model, long cGId)
        {
            ProductBookingInfo pBooking = new ProductBookingInfo();
            pBooking.CGId = cGId;
            pBooking.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            pBooking.CreatedDate = DateTime.Now;
            pBooking.ProductId = (int)model.ProductId;
            pBooking.BookingAmt = model.BookingMoney;
            pBooking.DiscountPercentage = model.Discount;
            pBooking.InstallmentTypeId = model.BookingInstallmentTypeId == 0 ? model.BookingInstallmentTypeManualId : model.BookingInstallmentTypeId;
            pBooking.SoldBy = model.EmployeeId;
            pBooking.Step = 1;
            pBooking.Status = 1;
            pBooking.EntryBy = model.EntryBy;
            pBooking.OtherInformation = model.OtherInformation;
            pBooking.RatePerKatha = model.RatePerKatha;
            pBooking.RestofAmount = model.RestofAmount;
            pBooking.ApplicationDate = model.ApplicationDate;
            pBooking.IsActive = true;
            pBooking.BookingNo = model.BookingNo;
            pBooking.TeamLeadId = model.TeamLeadId;
            pBooking.SpecialDiscountAmt = model.SpecialDiscountAmt;
            pBooking.BookingDate = model.BookingDate;
            pBooking.FileNo = model.FileNo;
            pBooking.LandValue = model.LandValue;
            pBooking.FileStatus = 1;
            pBooking.InstallmentAmount = model.InstallmentAmount;
            pBooking.Remarks = model.Remarks;
            pBooking.ResaleLogId = model.PurchaseAmount > 0 ? model.CGId : 0;
            pBooking.PurchaseAmount = model.PurchaseAmount;
            pBooking.DownPayment = model.DownPayment;
            pBooking.DownPaymentDate = model.DownPaymentDate;
            pBooking.BookingMoneyPaymentDate = model.BookingMoneyPaymentDate;
            pBooking.IsSeaPalace = 1;
            pBooking.NoOfShare = (int)model.PlotSize;

            return pBooking;
        }

        private List<CustomerGroupMapping> MapCustomerGroup(GLDLBookingViewModel model, long cGId)
        {
            List<CustomerGroupMapping> models = new List<CustomerGroupMapping>();
            foreach (var item in model.Cutomers)
            {
                CustomerGroupMapping mapping = new CustomerGroupMapping();
                mapping.CGId = cGId;
                mapping.SharePercentage = (double)item.SharePercentage;
                mapping.CustomerId = item.VendorId;
                mapping.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                mapping.CreatedDate = DateTime.Now;
                mapping.IsActive = true;
                models.Add(mapping);
            }
            return models;
        }

        private CustomerGroupInfo MapCustomerGroupInfo(GLDLBookingViewModel model)
        {
            CustomerGroupInfo groupInfo = new CustomerGroupInfo();
            groupInfo.GroupName = model.CustomerGroupName;
            groupInfo.CompanyId = model.CompanyId;
            groupInfo.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            groupInfo.CreatedDate = DateTime.Now;
            groupInfo.PrimaryContactAddr = model.PrimaryContactAddr;
            groupInfo.PrimaryContactNo = model.PrimaryContactNo;
            groupInfo.PrimaryEmail = model.PrimaryEmail;
            groupInfo.PrimaryClientId = model.ClientId;
            groupInfo.IsActive = true;

            return groupInfo;
        }


        public async Task<List<SelectModelType>> GetbyEmployee(int companyId)
        {
            List<SelectModelType> selectModelLiat = new List<SelectModelType>();
            var v = await context.Employees.Where(e => e.Active && e.FaxNo == "KGRE".Trim()).Select(x => new SelectModelType()
            {
                Text = x.Name,
                Value = (int)x.Id
            }).ToListAsync();
            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }


        public async Task<CustomerNominee> AddCustomerNominee(CustomerNominee nominee)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    CustomerNomineeInfo model = new CustomerNomineeInfo();
                    model.NomineeName = nominee.NomineeName;
                    model.CustomerId = nominee.CustomerId;
                    model.Email = nominee.NomineeEmail;
                    model.PhoneNo = nominee.NomineeMobile;
                    model.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    model.CreatedDate = DateTime.Now;
                    model.RelationId = nominee.RelationId;
                    model.IsActive = true;
                    context.CustomerNomineeInfoes.Add(model);
                    context.SaveChanges();
                    NomineePercentageMapping mapping = new NomineePercentageMapping();
                    mapping.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    mapping.CreatedDate = DateTime.Now;
                    mapping.NomineeId = model.NomineeId;
                    mapping.ProductId = nominee.ProductId;
                    mapping.GroupId = nominee.CGId;
                    mapping.IsActive = true;
                    mapping.Percentage = Convert.ToDouble(nominee.NomineeSharePercentage);
                    context.NomineePercentageMappings.Add(mapping);
                    context.SaveChanges();
                    scope.Commit();
                    return nominee;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return nominee;
                }
            }
        }

        public async Task<CustomerNominee> DeleteNominee(CustomerNominee nominee)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    CustomerNomineeInfo model = await context.CustomerNomineeInfoes.FirstOrDefaultAsync(d => d.NomineeId == nominee.NomineeId);
                    model.IsActive = false;
                    context.Entry(model).State = EntityState.Modified;
                    NomineePercentageMapping mapping = await context.NomineePercentageMappings.FirstOrDefaultAsync(d => d.NomineeId == nominee.NomineeId);
                    mapping.IsActive = false;
                    context.Entry(mapping).State = EntityState.Modified;
                    context.SaveChanges();
                    scope.Commit();
                    return nominee;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return nominee;
                }
            }

        }

        public async Task<CustomerNominee> GetByNominee(long id)
        {
            try
            {
                CustomerNomineeInfo nominee = await context.CustomerNomineeInfoes.FirstOrDefaultAsync(d => d.NomineeId == id);
                NomineePercentageMapping mapping = await context.NomineePercentageMappings.FirstOrDefaultAsync(d => d.NomineeId == id);
                CustomerNominee model = new CustomerNominee();
                model.NomineeName = nominee.NomineeName;
                model.NomineeId = nominee.NomineeId;
                model.CustomerId = nominee.CustomerId;
                model.NomineeEmail = nominee.Email;
                model.NomineeMobile = nominee.PhoneNo;
                model.RelationId = nominee.RelationId;
                model.NomineeSharePercentage = mapping.Percentage;
                model.NomineeId = nominee.NomineeId;
                model.ImageDocId = nominee.ImageDocId == null ? 0 : (long)nominee.ImageDocId;
                model.NIDDocId = nominee.NIDDocId == null ? 0 : (long)nominee.NIDDocId;

                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CustomerNominee> UpdateNominee(CustomerNominee nominee)
        {
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    CustomerNomineeInfo model = await context.CustomerNomineeInfoes.FirstOrDefaultAsync(d => d.NomineeId == nominee.NomineeId);
                    model.NomineeName = nominee.NomineeName;
                    model.Email = nominee.NomineeEmail;
                    model.PhoneNo = nominee.NomineeMobile;
                    model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    model.ModifiedDate = DateTime.Now;
                    model.RelationId = nominee.RelationId;
                    context.Entry(model).State = EntityState.Modified;
                    context.SaveChanges();

                    NomineePercentageMapping mapping = await context.NomineePercentageMappings.FirstOrDefaultAsync(d => d.NomineeId == nominee.NomineeId);
                    mapping.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    mapping.ModifiedDate = DateTime.Now;
                    mapping.Percentage = Convert.ToDouble(nominee.NomineeSharePercentage);
                    context.Entry(model).State = EntityState.Modified;
                    context.SaveChanges();
                    scope.Commit();
                    return nominee;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return nominee;
                }
            }
        }

        public async Task<NomineeFile> FileUpdateNominee(NomineeFile nomineeFile, long ImageDocId, long NIDDocId)
        {
            CustomerNomineeInfo model = await context.CustomerNomineeInfoes.FirstOrDefaultAsync(d => d.NomineeId == nomineeFile.CNomineeId);
            if (ImageDocId != 0)
            {
                model.ImageDocId = ImageDocId;
            }
            if (NIDDocId != 0)
            {
                model.NIDDocId = NIDDocId;
            }


            context.Entry(model).State = EntityState.Modified;
            context.SaveChanges();
            return nomineeFile;
        }

        public async Task<bool> UpdateNomineeImageDociId(long id, long docId)
        {
            try
            {
                CustomerNomineeInfo model = await context.CustomerNomineeInfoes.FirstOrDefaultAsync(d => d.NomineeId == id && d.ImageDocId == docId);
                model.ImageDocId = 0;
                context.Entry(model).State = EntityState.Modified;
                var res = await context.SaveChangesAsync();
                if (res == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }


        }

        public async Task<bool> UpdateNomineeNIDDociId(long id, long docId)
        {
            try
            {
                CustomerNomineeInfo model = await context.CustomerNomineeInfoes.FirstOrDefaultAsync(d => d.NomineeId == id && d.NIDDocId == docId);
                model.NIDDocId = 0;
                context.Entry(model).State = EntityState.Modified;
                var res = await context.SaveChangesAsync();
                if (res == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<bool> FileMapping(List<FileItem> itemlist, long CGId)
        {
            try
            {
                List<CustomerBookingFileMapping> fileMappings = new List<CustomerBookingFileMapping>();
                foreach (var item in itemlist)
                {
                    fileMappings.Add(new CustomerBookingFileMapping
                    {
                        DocId = item.docid,
                        CGId = CGId,
                        FileTitel = item.docdesc,
                        IsActive = true
                    });
                }
                context.CustomerBookingFileMappings.AddRange(fileMappings);
                var res = await context.SaveChangesAsync();
                if (res == 0) { return false; } else { return true; }
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        public async Task<bool> DeleteCGFile(long docId, long CGId)
        {
            try
            {
                var data = await context.CustomerBookingFileMappings.Where(f => f.DocId == docId && f.CGId == CGId).FirstOrDefaultAsync();
                data.IsActive = false;
                context.Entry(data).State = EntityState.Modified;
                var res = await context.SaveChangesAsync();
                if (res == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<int> GetByclient(long clientId)
        {
            var data = await context.CustomerGroupInfoes.Where(f => f.PrimaryClientId == clientId).CountAsync();
            return data;
        }

        public object GetCustomerAutoComplete(string prefix, int companyId)
        {

            return context.Vendors.Where(x => x.CompanyId == companyId && x.IsActive && x.VendorTypeId == (int)Provider.Customer
            && (x.Name.Contains(prefix) || x.Phone.Contains(prefix) || x.Email.Contains(prefix))).Select(x => new
            {

                label = x.Name + (x.Code != null ? " " + x.Code : "") + (x.Phone != null ? " Phone: " + x.Phone : "") + (x.NID != null ? " NID: " + x.NID : "") + (x.BusinessAddress != null ? " Project: " + x.BusinessAddress : ""),
                val = x.VendorId
            }).OrderBy(x => x.label).ToList();
        }

        public async Task<List<SelectModelType>> GetbyMember(int companyId)
        {
            List<SelectModelType> selectModelList = new List<SelectModelType>();

            var y = await Task.Run(() => context.GetEmployeeListForTeam(companyId).ToList());

            selectModelList = y.Select(o => new SelectModelType()
            {
                Text = $"[{o.EmployeeId}]-{o.Name} ({o.Designation})",
                Value = (int)o.Id
            }).ToList();
            return selectModelList;
        }

        public GLDLBookingViewModel bookingFilecheck(int companyId, string prefix)
        {
            GLDLBookingViewModel vm = new GLDLBookingViewModel();
            var data = context.ProductBookingInfoes
      .Where(f => (f.IsActive == true) && (f.FileNo.Replace(" ", "").Trim() == prefix.Replace(" ", "").Trim() || f.FileNo.Contains(prefix)))
      .FirstOrDefault();

            if (data != null)
            {
                vm.FileNo = data.FileNo;
                vm.CGId = data.CGId;
                vm.BookingDate = data.BookingDate;
            }

            return vm;
        }


    }
}
