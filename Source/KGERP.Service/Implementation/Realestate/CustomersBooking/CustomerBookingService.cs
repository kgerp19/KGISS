using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KGERP.Service.Implementation.Realestate.CustomersBooking
{
    public class CustomerBookingService : ICustomerBookingService
    {
        ERPEntities context = new ERPEntities();

        private readonly AccountingService _accountingService;
        public CustomerBookingService(AccountingService accountingService)
        {
            _accountingService = accountingService;
        }

        public async Task<GLDLBookingViewModel> CustomerBookingList(int companyId, DateTime? fromDate, DateTime? toDate, int? ProductCategoryId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model.CompanyId = companyId;
            model.datalist = await Task.Run(() => (from t1 in context.CustomerGroupInfoes.Where(f => f.IsActive == true && f.CompanyId == companyId)
                                                   join t2 in context.Companies.Where(f => f.IsActive == true) on t1.CompanyId equals t2.CompanyId
                                                   join t3 in context.ProductBookingInfoes.Where(f => f.IsActive == true) on t1.CGId equals t3.CGId
                                                   join t4 in context.Products.Where(f => f.IsActive == true) on t3.ProductId equals t4.ProductId
                                                   join t5 in context.ProductSubCategories.Where(f => f.IsActive == true) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                   join t6 in context.ProductCategories.Where(f => f.IsActive == true) on t5.ProductCategoryId equals t6.ProductCategoryId
                                                   where t3.BookingDate >= fromDate
                                                  && t3.BookingDate <= toDate && (!ProductCategoryId.HasValue || ProductCategoryId <= 0 || t5.ProductCategoryId == ProductCategoryId) && t3.IsSeaPalace == 0
                                                   select new GLDLBookingViewModel
                                                   {
                                                       CGId = t1.CGId,
                                                       BookingId = t3.BookingId,
                                                       CustomerGroupName = t1.GroupName,
                                                       KGRECompanyName = t2.Name,
                                                       PrimaryContactNo = t1.PrimaryContactNo,
                                                       PrimaryEmail = t1.PrimaryEmail,
                                                       PrimaryContactAddr = t1.PrimaryContactAddr,
                                                       ProjectName = t6.Name,
                                                       ProductCategoryId = t6.ProductCategoryId,
                                                       BlockName = t5.Name,
                                                       ProductSubCategoryId = t5.ProductSubCategoryId,
                                                       PlotName = t4.ProductName,
                                                       ProductId = t3.ProductId,
                                                       PlotNo = t4.ProductCode,
                                                       PlotSize = t4.PackSize,
                                                       RatePerKatha = t3.RatePerKatha,
                                                       BookingMoney = t3.BookingAmt,
                                                       Discount = t3.DiscountPercentage,
                                                       //LandValue = (decimal)((double)t4.PackSize * (double)t3.RatePerKatha),
                                                       RestofAmount = t3.RestofAmount + t3.BookingAmt + t3.DownPayment,
                                                       CompanyId = t2.CompanyId,
                                                       Status = t3.Status,
                                                       Step = t3.Step,
                                                       ApplicationDate = t3.ApplicationDate,
                                                       BookingDate = t3.BookingDate,
                                                       SpecialDiscountAmt = t3.SpecialDiscountAmt,
                                                       BookingNo = t3.BookingNo,
                                                       IsSubmited = t3.IsSubmitted,
                                                       FileNo = t3.FileNo,
                                                       FileStatus = t3.FileStatus
                                                   }).OrderByDescending(f => f.CGId).ToListAsync());
            return model;
        }
        public async Task<GLDLBookingViewModel> SPCustomerBookingList(int companyId, DateTime? fromDate, DateTime? toDate, int? ProductCategoryId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model.CompanyId = companyId;
            model.datalist = await Task.Run(() => (from t1 in context.CustomerGroupInfoes.Where(f => f.IsActive == true && f.CompanyId == companyId)
                                                   join t2 in context.Companies.Where(f => f.IsActive == true) on t1.CompanyId equals t2.CompanyId
                                                   join t3 in context.ProductBookingInfoes.Where(f => f.IsActive == true) on t1.CGId equals t3.CGId
                                                   join t4 in context.Products.Where(f => f.IsActive == true) on t3.ProductId equals t4.ProductId
                                                   join t5 in context.ProductSubCategories.Where(f => f.IsActive == true) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                   join t6 in context.ProductCategories.Where(f => f.IsActive == true) on t5.ProductCategoryId equals t6.ProductCategoryId
                                                   where t3.BookingDate >= fromDate
                                                  && t3.BookingDate <= toDate && (!ProductCategoryId.HasValue || ProductCategoryId <= 0 || t5.ProductCategoryId == ProductCategoryId) && t3.IsSeaPalace == 1
                                                   select new GLDLBookingViewModel
                                                   {
                                                       CGId = t1.CGId,
                                                       BookingId = t3.BookingId,
                                                       CustomerGroupName = t1.GroupName,
                                                       KGRECompanyName = t2.Name,
                                                       PrimaryContactNo = t1.PrimaryContactNo,
                                                       PrimaryEmail = t1.PrimaryEmail,
                                                       PrimaryContactAddr = t1.PrimaryContactAddr,
                                                       ProjectName = t6.Name,
                                                       ProductCategoryId = t6.ProductCategoryId,
                                                       BlockName = t5.Name,
                                                       ProductSubCategoryId = t5.ProductSubCategoryId,
                                                       PlotName = t4.ProductName,
                                                       ProductId = t3.ProductId,
                                                       PlotNo = t4.ProductCode,
                                                       PlotSize = t4.PackSize,
                                                       RatePerKatha = t3.RatePerKatha,
                                                       BookingMoney = t3.BookingAmt,
                                                       Discount = t3.DiscountPercentage,
                                                       //LandValue = (decimal)((double)t4.PackSize * (double)t3.RatePerKatha),
                                                       RestofAmount = t3.RestofAmount + t3.BookingAmt + t3.DownPayment,
                                                       CompanyId = t2.CompanyId,
                                                       Status = t3.Status,
                                                       Step = t3.Step,
                                                       ApplicationDate = t3.ApplicationDate,
                                                       BookingDate = t3.BookingDate,
                                                       SpecialDiscountAmt = t3.SpecialDiscountAmt,
                                                       BookingNo = t3.BookingNo,
                                                       IsSubmited = t3.IsSubmitted,
                                                       FileNo = t3.FileNo,
                                                       FileStatus = t3.FileStatus
                                                   }).OrderByDescending(f => f.CGId).ToListAsync());
            return model;
        }



        public async Task<GLDLBookingViewModel> CustomerBookingListInactive(int companyId, DateTime? fromDate, DateTime? toDate, int? ProductCategoryId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model.CompanyId = companyId;
            model.datalist = await Task.Run(() => (from t1 in context.CustomerGroupInfoes.Where(f => f.IsActive == true && f.CompanyId == companyId)
                                                   join t2 in context.Companies.Where(f => f.IsActive == true) on t1.CompanyId equals t2.CompanyId
                                                   join t3 in context.ProductBookingInfoes.Where(f => f.IsActive == false) on t1.CGId equals t3.CGId
                                                   join t4 in context.Products.Where(f => f.IsActive == true) on t3.ProductId equals t4.ProductId
                                                   join t5 in context.ProductSubCategories.Where(f => f.IsActive == true) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                   join t6 in context.ProductCategories.Where(f => f.IsActive == true) on t5.ProductCategoryId equals t6.ProductCategoryId
                                                   where t3.BookingDate >= fromDate
                                                  && t3.BookingDate <= toDate && (!ProductCategoryId.HasValue || ProductCategoryId <= 0 || t5.ProductCategoryId == ProductCategoryId)
                                                   select new GLDLBookingViewModel
                                                   {
                                                       CGId = t1.CGId,
                                                       BookingId = t3.BookingId,
                                                       CustomerGroupName = t1.GroupName,
                                                       KGRECompanyName = t2.Name,
                                                       PrimaryContactNo = t1.PrimaryContactNo,
                                                       PrimaryEmail = t1.PrimaryEmail,
                                                       PrimaryContactAddr = t1.PrimaryContactAddr,
                                                       ProjectName = t6.Name,
                                                       ProductCategoryId = t6.ProductCategoryId,
                                                       BlockName = t5.Name,
                                                       ProductSubCategoryId = t5.ProductSubCategoryId,
                                                       PlotName = t4.ProductName,
                                                       ProductId = t3.ProductId,
                                                       PlotNo = t4.ProductCode,
                                                       PlotSize = t4.PackSize,
                                                       RatePerKatha = t3.RatePerKatha,
                                                       BookingMoney = t3.BookingAmt,
                                                       Discount = t3.DiscountPercentage,
                                                       //LandValue = (decimal)((double)t4.PackSize * (double)t3.RatePerKatha),
                                                       RestofAmount = t3.RestofAmount + t3.BookingAmt,
                                                       CompanyId = t2.CompanyId,
                                                       Status = t3.Status,
                                                       Step = t3.Step,
                                                       ApplicationDate = t3.ApplicationDate,
                                                       BookingDate = t3.BookingDate,
                                                       SpecialDiscountAmt = t3.SpecialDiscountAmt,
                                                       BookingNo = t3.BookingNo,
                                                       IsSubmited = t3.IsSubmitted,
                                                       FileNo = t3.FileNo,
                                                       FileStatus = t3.FileStatus
                                                   }).OrderByDescending(f => f.CGId).ToListAsync());
            return model;
        }
        public async Task<GLDLBookingViewModel> ReSaleBookingList(int companyId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model.CompanyId = companyId;
            model.datalist = await Task.Run(() => (from t1 in context.CustomerGroupInfoes.Where(f => f.IsActive == true && f.CompanyId == companyId)
                                                   join t2 in context.Companies.Where(f => f.IsActive == true) on t1.CompanyId equals t2.CompanyId
                                                   join t3 in context.ProductBookingInfoes.Where(f => f.IsActive == true) on t1.CGId equals t3.CGId
                                                   join t4 in context.Products.Where(f => f.IsActive == true) on t3.ProductId equals t4.ProductId
                                                   join t5 in context.ProductSubCategories.Where(f => f.IsActive == true) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                   join t6 in context.ProductCategories.Where(f => f.IsActive == true) on t5.ProductCategoryId equals t6.ProductCategoryId
                                                   where t3.FileStatus == 4
                                                   select new GLDLBookingViewModel
                                                   {
                                                       CGId = t1.CGId,
                                                       BookingId = t3.BookingId,
                                                       CustomerGroupName = t1.GroupName,
                                                       KGRECompanyName = t2.Name,
                                                       PrimaryContactNo = t1.PrimaryContactNo,
                                                       PrimaryEmail = t1.PrimaryEmail,
                                                       PrimaryContactAddr = t1.PrimaryContactAddr,
                                                       ProjectName = t6.Name,
                                                       ProductCategoryId = t6.ProductCategoryId,
                                                       BlockName = t5.Name,
                                                       ProductSubCategoryId = t5.ProductSubCategoryId,
                                                       PlotName = t4.ProductName,
                                                       ProductId = t3.ProductId,
                                                       PlotNo = t4.ProductCode,
                                                       PlotSize = t4.PackSize,
                                                       RatePerKatha = t3.RatePerKatha,
                                                       BookingMoney = t3.BookingAmt,
                                                       Discount = t3.DiscountPercentage,
                                                       //LandValue = (decimal)((double)t4.PackSize * (double)t3.RatePerKatha),
                                                       RestofAmount = t3.RestofAmount + t3.BookingAmt,
                                                       CompanyId = t2.CompanyId,
                                                       Status = t3.Status,
                                                       Step = t3.Step,
                                                       ApplicationDate = t3.ApplicationDate,
                                                       BookingDate = t3.BookingDate,
                                                       SpecialDiscountAmt = t3.SpecialDiscountAmt,
                                                       BookingNo = t3.BookingNo,
                                                       IsSubmited = t3.IsSubmitted,
                                                       FileNo = t3.FileNo,
                                                       FileStatus = t3.FileStatus
                                                   }).OrderByDescending(f => f.CGId).ToListAsync());
            return model;
        }
        public async Task<GLDLBookingViewModel> CustomerBookingView(int companyId, long CGId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model = await Task.Run(() => (from t1 in context.CustomerGroupInfoes
                                          join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                          join t3 in context.ProductBookingInfoes on t1.CGId equals t3.CGId
                                          join t4 in context.Products on t3.ProductId equals t4.ProductId into t4_join
                                          from t4 in t4_join.DefaultIfEmpty()
                                          join t9 in context.Units on t4.UnitId equals t9.UnitId into t9_join
                                          from t9 in t9_join.DefaultIfEmpty()
                                          join t5 in context.ProductSubCategories on t4.ProductSubCategoryId equals t5.ProductSubCategoryId into t5_join
                                          from t5 in t5_join.DefaultIfEmpty()
                                          join t6 in context.ProductCategories on t5.ProductCategoryId equals t6.ProductCategoryId into t6_join
                                          from t6 in t6_join.DefaultIfEmpty()
                                          join t7 in context.Employees on t3.SoldBy equals t7.Id into t7_join
                                          from t7 in t7_join.DefaultIfEmpty()
                                          join t8 in context.Employees on t3.TeamLeadId equals t8.Id into t8_join
                                          from t8 in t8_join.DefaultIfEmpty()
                                          join t10 in context.FacingInfoes on t4.FacingId equals t10.FacingId into t10_join
                                          from t10 in t10_join.DefaultIfEmpty()
                                          join t11 in context.HeadGLs on t1.HeadGLId equals t11.Id into t11_join
                                          from t11 in t11_join.DefaultIfEmpty()
                                          where t1.CGId == CGId
                                          select new GLDLBookingViewModel
                                          {
                                              AccCode = t11.AccCode,
                                              ProductStatus = t4.Status,
                                              IntegratedFrom = "ProductBookingInfo",
                                              HeadGLId = t1.HeadGLId,
                                              AccountingIncomeHeadId = companyId == (int)CompanyName.KrishibidPropertiesLimited ? t6.AccountingIncomeHeadId : t5.AccountingIncomeHeadId,
                                              CGId = t1.CGId,
                                              ResaleLogId = t3.ResaleLogId,
                                              CustomerGroupName = t1.GroupName,
                                              KGRECompanyName = t2.Name,
                                              PrimaryContactNo = t1.PrimaryContactNo,
                                              PrimaryClientId = t1.PrimaryClientId,
                                              PrimaryEmail = t1.PrimaryEmail,
                                              PrimaryContactAddr = t1.PrimaryContactAddr,
                                              ProjectName = t6.Name,
                                              ProductCategoryId = t6.ProductCategoryId,
                                              BlockName = t5.Name,
                                              ProductSubCategoryId = t5.ProductSubCategoryId,
                                              PlotName = t4.ProductName,
                                              ProductId = t3.ProductId,
                                              BookingNo = t3.BookingNo,
                                              BookingId = t3.BookingId,
                                              PlotNo = t4.ProductCode,
                                              PlotSize = t3.NoOfShare,
                                              FacingName = t10.Title,
                                              RatePerKatha = t3.RatePerKatha,
                                              BookingMoney = t3.BookingAmt,
                                              Discount = t3.DiscountPercentage,
                                              LandValue = t3.LandValue,
                                              isSeaPalace = t3.IsSeaPalace,
                                              RestofAmount = t3.RestofAmount,
                                              CompanyId = t2.CompanyId,
                                              UnitName = t9.Name,
                                              SalesPerson = t7.Name,
                                              SalesPersonPhone = t7.MobileNo,
                                              SalesPersonEmail = t7.Email,
                                              SalesPersonAddress = t7.PermanentAddress,
                                              Status = t3.Status,
                                              Step = t3.Step,

                                              ApplicationDate = t3.ApplicationDate,
                                              BookingDate = t3.BookingDate,
                                              SpecialDiscountAmt = t3.SpecialDiscountAmt,
                                              TeamLeadName = t8.Name,
                                              TeamLeadPhone = t8.MobileNo,
                                              TeamLeadEmail = t8.Email,
                                              TeamLeadAddress = t8.PermanentAddress,
                                              EmployeeId = (int)t7.Id,
                                              IsSubmited = t3.IsSubmitted,
                                              FileNo = t3.FileNo,
                                              InstallmentAmount = t3.InstallmentAmount,
                                              AcCostCenterId = t6.CostCenterId,
                                              FileStatus = t3.FileStatus,
                                              PurchaseAmount = t3.PurchaseAmount,
                                              Remarks = t3.Remarks,
                                              BookingMoneyPaymentDate = t3.BookingMoneyPaymentDate,
                                              DownPaymentDate = t3.DownPaymentDate,
                                              DownPayment = t3.DownPayment,
                                              CreatedDate = t3.CreatedDate,
                                              CreateBy = t3.CreatedBy,
                                              TotalBookingMoney = t3.BookingAmt + t3.DownPayment,
                                              TotalBookingMoneyPaid = t3.PaidBookingAmt + t3.PaidDownPaymentAmt


                                          }).FirstOrDefaultAsync());

            model.LstPurchaseCostHeads = await Task.Run(() => (from t1 in context.ProductBookingInfoes.Where(x => x.CGId == CGId)
                                                               join t2 in context.BookingCostMappings on t1.BookingId equals t2.BookingId
                                                               join t3 in context.BookingCostHeads on t2.CostId equals t3.CostId
                                                               select new BookingHeadServiceModel
                                                               {
                                                                   CostId = t2.CostId,
                                                                   Amount = t2.Amount,
                                                                   BookingId = t2.BookingId,
                                                                   CostName = t3.CostName,
                                                                   IsSnstallmentInclude = t2.IsSnstallmentInclude,
                                                                   PaidAmount = t2.PaidAmount
                                                               }).ToListAsync());
            decimal totalcost = model.LstPurchaseCostHeads.Select(d => d.Amount).Sum();
            model.TotalCost = totalcost;
            model.TotalAmount = model.LandValue + totalcost;
            decimal TotalDiscount = ((model.LandValue / 100m) * Convert.ToDecimal(model.Discount));
            model.TotalDiscount = (decimal)(TotalDiscount + model.SpecialDiscountAmt);
            model.GrandTotalAmount = model.TotalAmount - model.TotalDiscount;

            var nominee = await Task.Run(() => (from t1 in context.CustomerGroupMappings
                                                join t2 in context.Vendors on t1.CustomerId equals t2.VendorId
                                                join t3 in context.CustomerNomineeInfoes.Where(d => d.IsActive == true) on t1.CustomerId equals t3.CustomerId
                                                join t4 in context.NomineePercentageMappings.Where(d => d.IsActive == true) on t3.NomineeId equals t4.NomineeId
                                                join t5 in context.PRM_Relation on t3.RelationId equals t5.Id
                                                where t1.CGId == CGId
                                                select new CustomerNominee
                                                {
                                                    NomineeId = t3.NomineeId,
                                                    CustomerId = t1.CustomerId,
                                                    NomineeName = t3.NomineeName,
                                                    CustomerName = t2.Name,
                                                    NomineeMobile = t3.PhoneNo,
                                                    NomineeSharePercentage = t4.Percentage,
                                                    RelationName = t5.Name
                                                }).ToListAsync());


            model.Cutomers = await Task.Run(() => (from t1 in context.CustomerGroupMappings
                                                   join t2 in context.Vendors on t1.CustomerId equals t2.VendorId
                                                   where t1.CGId == CGId
                                                   select new CutomerListForBooking
                                                   {
                                                       MapId = t1.MapId,
                                                       VendorId = t1.CustomerId,
                                                       VendorName = t2.Name,
                                                       VendorMobile = t2.Phone,
                                                       SharePercentage = t1.SharePercentage
                                                   }).ToListAsync());
            for (var i = 0; i < model.Cutomers.Count(); i++)
            {
                model.Cutomers[i].CustomerNominee = nominee.Where(e => e.CustomerId == model.Cutomers[i].VendorId).ToList();
                model.Cutomers[i].TotalPercentage = nominee.Where(e => e.CustomerId == model.Cutomers[i].VendorId).Select(d => d.NomineeSharePercentage).Sum();
            }

            model.Attachments = await context.CustomerBookingFileMappings.Where(t2 => t2.CGId == CGId && t2.IsActive).
                                                         Select(o => new GLDLBookingAttachment
                                                         {
                                                             DocId = o.DocId,
                                                             Title = o.FileTitel
                                                         }).ToListAsync();

            model.Schedule = await context.BookingInstallmentSchedules.Where(f => f.CGID == CGId && f.IsActive).
                Select(o => new InstallmentScheduleShortModel
                {
                    InstallmentId = o.InstallmentId,
                    Title = o.InstallmentTitle,
                    PayableAmount = o.Amount,
                    InstallmentDate = o.Date,
                    PaidAmount = o.PaidAmount,
                    IsPaid = o.IsPaid,

                }).ToListAsync();
            model.InstallmentSumOfAmount = model.Schedule.Sum(g => g.PayableAmount);
            var result = await context.ProductBookingInfoes.Where(f => f.CGId == CGId).FirstOrDefaultAsync();
            model.approval = await Task.Run(() => (from t1 in context.CustomerGroupInfoes.Where(f => f.CGId == CGId)
                                                   join t2 in context.ProductBookingInfoes on t1.CGId equals t2.CGId
                                                   join t3 in context.Employees on t2.EntryBy equals t3.Id into t3_join
                                                   from t3 in t3_join.DefaultIfEmpty()
                                                   join t4 in context.Employees on t2.CheckedBy equals t4.Id into t4_join
                                                   from t4 in t4_join.DefaultIfEmpty()
                                                   join t5 in context.Employees on t2.ApprovedBy equals t5.Id into t5_join
                                                   from t5 in t5_join.DefaultIfEmpty()
                                                   join t6 in context.Employees on t2.FinalApproverBy equals t6.Id into t6_join
                                                   from t6 in t6_join.DefaultIfEmpty()

                                                   join t7 in context.Designations on t3.DesignationId equals t7.DesignationId into t7_join
                                                   from t7 in t7_join.DefaultIfEmpty()
                                                   join t8 in context.Designations on t4.DesignationId equals t8.DesignationId into t8_join
                                                   from t8 in t8_join.DefaultIfEmpty()
                                                   join t9 in context.Designations on t5.DesignationId equals t9.DesignationId into t9_join
                                                   from t9 in t9_join.DefaultIfEmpty()
                                                   join t10 in context.Designations on t6.DesignationId equals t10.DesignationId into t10_join
                                                   from t10 in t10_join.DefaultIfEmpty()


                                                   select new ApprovalInfoViewModel
                                                   {
                                                       EntryBy = t2.EntryBy,
                                                       EntryName = t3.Name,
                                                       EntryBySignatureFileName = t3.SignatureFileName,
                                                       CheckedBy = t2.CheckedBy,
                                                       CheckeName = t4.Name,
                                                       ApprovedBy = t2.ApprovedBy,
                                                       ApproveName = t5.Name,
                                                       FinalApproverBy = t2.FinalApproverBy,
                                                       FinalApproveName = t6.Name,
                                                       EntryDate = t2.EntryDate,
                                                       CheckDate = t2.CheckDate,
                                                       ApproveDate = t2.ApproveDate,
                                                       FinalApvDate = t2.FinalApvDate,
                                                       EntryDesignation = t7.Name,
                                                       CheckedDesignation = t8.Name,
                                                       ApprovedDesignation = t9.Name,
                                                       FinalApproverDesignation = t10.Name
                                                   }).FirstOrDefaultAsync());


            return model;
        }
        public async Task<GLDLBookingViewModel> SaveNewInstallMent(GLDLBookingViewModel model)
        {
            if (model != null)
            {

                BookingInstallmentSchedule newModel = new BookingInstallmentSchedule();
                newModel.BookingId = model.BookingId;
                newModel.CGID = model.CGId;
                newModel.InstallmentTitle = model.BookingInstallmentTitle;
                newModel.InstallmentTypeId = model.BookingInstallmentTypeId;
                newModel.Date = model.BookingInstallmentDate;
                newModel.Amount = model.BookingInstallmenAmmount;
                newModel.IsActive = true;
                newModel.IsPaid = false;
                newModel.IsPartlyPaid = false;
                newModel.PaidAmount = 0;
                newModel.IsLate = false;
                newModel.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                newModel.CreatedDate = DateTime.Now;
                newModel.Remarks = model.InstalmentRemarks;

                context.BookingInstallmentSchedules.Add(newModel);
                await context.SaveChangesAsync();

            }


            return model;
        }
        public async Task<bool> deleteInstallment(int InstallmentId)
        {
            var obj = context.BookingInstallmentSchedules.Where(x => x.InstallmentId == InstallmentId).FirstOrDefault();
            if (obj != null)
            {
                obj.IsActive = false;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public async Task<List<SelectModelType>> PRMRelation()
        {
            List<SelectModelType> selectModelLiat = new List<SelectModelType>();
            var v = await context.PRM_Relation.Select(x => new SelectModelType()
            {
                Text = x.Name,
                Value = x.Id
            }).ToListAsync();
            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }

        public async Task<long> SubmitBooking(GLDLBookingViewModel bookingViewModel)
        {
            var booking = await CustomerBookingView(bookingViewModel.CompanyId.Value, bookingViewModel.CGId);
            booking.Accounting_BankOrCashId = bookingViewModel.Accounting_BankOrCashId;
            int voucherTypeId = 0;
            if (bookingViewModel.CompanyId == (int)CompanyName.KrishibidPropertiesLimited)
            {
                voucherTypeId = (int)KPLJournalEnum.SalesVoucher;
            }
            else if (bookingViewModel.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited)
            {
                voucherTypeId = (int)GLDLJournalEnum.SalesVoucher;
            }
            long sales = 0;
            if (booking.PurchaseAmount == 0)
            {
                sales = await _accountingService.AccountingSalesPushGLDL(booking.CompanyId.Value, booking, voucherTypeId);
            }
            if (booking.PurchaseAmount > 0)
            {
                sales = await _accountingService.AccountingReSalesPush(booking.CompanyId.Value, booking, voucherTypeId);
            }
            //var bookingCollection = await _accountingService.BookingMoneyCollectionPushGLDL(booking.CompanyId.Value, booking, 90);
            if (sales > 0)
            {
                var res = context.Products.FirstOrDefault(f => f.ProductId == bookingViewModel.ProductId);
                if (bookingViewModel.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited && res.Status == (int)ProductStatusEnumGLDL.UnSold)
                {
                    res.Status = (int)ProductStatusEnumGLDL.Booked;
                    context.Entry(res).State = EntityState.Modified;
                    context.SaveChanges();
                }
                if (bookingViewModel.CompanyId == (int)CompanyName.KrishibidPropertiesLimited && res.Status == (int)ProductStatusEnumKPL.VacantFlat && booking.isSeaPalace == 0)
                {
                    res.Status = (int)ProductStatusEnumKPL.Booked;
                    context.Entry(res).State = EntityState.Modified;
                    context.SaveChanges();
                }
                if (bookingViewModel.CompanyId == (int)CompanyName.KrishibidPropertiesLimited && res.Status == (int)ProductStatusEnumKPL.VacantFlat && booking.isSeaPalace == 1)
                {
                    //res.Status = (int)ProductStatusEnumKPL.Booked;
                    context.Entry(res).State = EntityState.Modified;
                    context.SaveChanges();
                }
                ProductBookingInfo productBookingInfo = context.ProductBookingInfoes.Find(booking.BookingId);
                productBookingInfo.IsSubmitted = true;
                productBookingInfo.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                productBookingInfo.ModifiedDate = DateTime.Now;
                context.SaveChanges();

                RealStateMessage message = new RealStateMessage();
                var extation = bookingViewModel.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited ? "GLDL" : "KPL";
                // var extation = bookingViewModel.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited ? "Krishibid City" : "krishibid Properties";
                string productname = bookingViewModel.CompanyId == 7 ? "plot" : "flat";
                var clientinfo = context.Vendors.FirstOrDefault(x => x.VendorId == booking.PrimaryClientId);
                message.CompanyId = (int)bookingViewModel.CompanyId;
                message.Agenda = 1;
                message.Address = clientinfo.Phone;
                //message.Address = "01700729282";
                message.MediaType = 1;
                message.CreateDate = DateTime.Now;
                message.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                message.Status = 1;
                message.IsActive = true;
                message.Message = "Dear " + clientinfo.Name + "," + System.Environment.NewLine + System.Environment.NewLine + "Congratulations on becoming a proud " + productname + " owner in " + booking.ProjectName + ".We are proud to have you as a member of our " + extation + " family." + System.Environment.NewLine + System.Environment.NewLine + "Regards," + extation;
                context.RealStateMessages.Add(message);
                context.SaveChanges();
            }
            return sales;
        }

        public async Task<BookingInstallmentSchedule> InstallmentScheduleById(long id)
        {
            BookingInstallmentSchedule res = await context.BookingInstallmentSchedules.FirstOrDefaultAsync(f => f.InstallmentId == id);
            return res;
        }

        public async Task<CollactionBillViewModel> CustomerInstallmentSchedule(int companyId, long CGId)
        {
            CollactionBillViewModel model = new CollactionBillViewModel();
            var List = new List<SceduleInstallment>();
            var res = await context.ProductBookingInfoes.FirstOrDefaultAsync(g => g.CGId == CGId);
            var res2 = await context.CustomerGroupInfoes.FirstOrDefaultAsync(g => g.CGId == CGId);
            model.BookingNo = res.BookingNo;
            model.BookingId = res.BookingId;
            model.RestofAmount = res.RestofAmount;
            model.CGId = CGId;
            model.CompanyId = companyId;
            model.HeadGLId = res2.HeadGLId;
            var v = (from t1 in context.BookingInstallmentSchedules.Where(f => f.CGID == CGId && f.IsPaid == false && f.IsActive)

                     select new
                     {
                         Value = t1.InstallmentId,
                         Date = t1.Date,
                         Amount = t1.IsPartlyPaid == true ? t1.Amount - t1.PaidAmount : t1.Amount,
                         Title = t1.InstallmentTitle
                     }).ToList();
            foreach (var item in v)
            {
                SceduleInstallment scedule = new SceduleInstallment();
                scedule.Text = item.Title + "-" + item.Date.ToString("dd-MMM-yyyy") + "(" + item.Amount + "TK" + ")";
                scedule.Title = item.Title;
                scedule.Value = item.Value;
                scedule.Amount = item.Amount;
                scedule.Date = item.Date;
                scedule.StringDate = item.Date.ToString("dd-MMM-yyyy");
                List.Add(scedule);
            }
            model.Schedule = List;
            model.DueAmount = model.Schedule.Select(d => d.Amount).Sum();
            return model;
        }

        public List<SceduleInstallment> GetByInstallmentSchedule(int companyId, long CGId)

        {
            List<SceduleInstallment> List = new List<SceduleInstallment>();
            var v = (from t1 in context.BookingInstallmentSchedules.Where(f => f.CGID == CGId && f.IsPaid == false && f.IsActive)

                     select new
                     {
                         Value = t1.InstallmentId,
                         Date = t1.Date,
                         Amount = t1.IsPartlyPaid == true ? t1.Amount - t1.PaidAmount : t1.Amount,
                         Title = t1.InstallmentTitle
                     }).ToList();
            foreach (var item in v)
            {
                SceduleInstallment scedule = new SceduleInstallment();
                scedule.Text = item.Title + "-" + item.Date.ToString("dd-MMM-yyyy") + "(" + item.Amount + "TK" + ")";
                scedule.Title = item.Title;
                scedule.Value = item.Value;
                scedule.Amount = item.Amount;
                scedule.Date = item.Date;
                scedule.StringDate = item.Date.ToString("dd-MMM-yyyy");
                List.Add(scedule);
            }
            return List;
        }

        public async Task<long> SubmitStatusChange(GLDLBookingViewModel bookingViewModel)
        {
            var res = context.Products.FirstOrDefault(f => f.ProductId == bookingViewModel.ProductId);
            var subcategory = context.ProductSubCategories.FirstOrDefault(f => f.ProductSubCategoryId == res.ProductSubCategoryId);
            if (bookingViewModel.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited && res.Status == (int)ProductStatusEnumGLDL.UnSold)
            {
                res.Status = (int)ProductStatusEnumGLDL.Booked;
                context.Entry(res).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (bookingViewModel.CompanyId == (int)CompanyName.KrishibidPropertiesLimited && res.Status == (int)ProductStatusEnumKPL.VacantFlat  && subcategory.ProductCategoryId != 124)
            {
                res.Status = (int)ProductStatusEnumKPL.Booked;
                context.Entry(res).State = EntityState.Modified;
                context.SaveChanges();
            }
            ProductBookingInfo productBookingInfo = await context.ProductBookingInfoes.FirstOrDefaultAsync(f => f.BookingId == bookingViewModel.BookingId);
            productBookingInfo.IsSubmitted = true;
            productBookingInfo.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            productBookingInfo.ModifiedDate = DateTime.Now;
            context.SaveChanges();
            return bookingViewModel.CGId;
        }

        public async Task<InstallmentScheduleShortModel> getInstallmetClient(long id)
        {
            try
            {
                BookingInstallmentSchedule bookingInstallmentSchedule = await context.BookingInstallmentSchedules.FirstOrDefaultAsync(u => u.InstallmentId == id);
                InstallmentScheduleShortModel model = new InstallmentScheduleShortModel();
                model.InstallmentId = bookingInstallmentSchedule.InstallmentId;
                model.InstallmentDate = bookingInstallmentSchedule.Date;
                model.InstallmentName = bookingInstallmentSchedule.InstallmentTitle;
                model.PayableAmount = bookingInstallmentSchedule.Amount;
                model.PaidAmount = bookingInstallmentSchedule.PaidAmount;
                model.IsPaid = bookingInstallmentSchedule.IsPaid;
                return model;
            }
            catch (Exception ex)
            {
                var msg = ex;
            }

            return null;

        }

        //ani
        public async Task<int> UpdateInsatllment(InstallmentScheduleShortModel model)
        {
            BookingInstallmentSchedule data = await context.BookingInstallmentSchedules.FirstOrDefaultAsync(u => u.InstallmentId == model.InstallmentId);
            data.InstallmentTitle = model.InstallmentName;
            data.Date = model.InstallmentDate;
            if (model.PaidAmount != 0)
            {
                data.Amount = model.PaidAmount;
            }
            else
            {
                data.Amount = model.PayableAmount;
            }
            data.ModifiedDate = DateTime.Now;
            data.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            var res = await context.SaveChangesAsync();

            return res;

        }


        public async Task<bool> DeleteProductBookingInfo(long CGID)
        {

            var productBookingInfo = await context.ProductBookingInfoes.FirstOrDefaultAsync(x => x.CGId == CGID);

            if (productBookingInfo != null)
            {

                productBookingInfo.IsActive = false;
                productBookingInfo.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                productBookingInfo.ModifiedDate = DateTime.Now;
                var bookingInstallments = await context.BookingInstallmentSchedules
                                                    .Where(c => c.CGID == CGID)
                                                    .ToListAsync();
                foreach (var installment in bookingInstallments)
                {
                    installment.IsActive = false;
                    installment.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    installment.ModifiedDate = DateTime.Now;
                }
                var bookingCostMaps = await context.BookingCostMappings
                                                    .Where(c => c.BookingId == productBookingInfo.BookingId)
                                                    .ToListAsync();
                foreach (var bookingCostMap in bookingCostMaps)
                {
                    bookingCostMap.IsActive = false;
                    bookingCostMap.ModifiedDate = DateTime.Now;
                    bookingCostMap.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                }
                var voucherMap = await context.VoucherMaps.Where(x => x.IntegratedId == productBookingInfo.BookingId).FirstOrDefaultAsync();
                if (voucherMap != null)
                {
                    var voucher = await context.Vouchers.Where(x => x.VoucherId == voucherMap.VoucherId).FirstOrDefaultAsync();
                    if (voucher != null)
                    {
                        voucher.IsActive = false;
                        voucher.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        voucher.ModifiedDate = DateTime.Now;
                    }
                    var voucherDetails = await context.VoucherDetails
                                                        .Where(c => c.VoucherId == voucherMap.VoucherId)
                                                        .ToListAsync();
                    foreach (var voucherDetail in voucherDetails)
                    {
                        voucherDetail.IsActive = false;
                    }
                }

                await context.SaveChangesAsync();

                return true;
            }


            return false;
        }

        public async Task<bool> UndoDeleteProductBookingInfo(long CGID)
        {

            var productBookingInfo = await context.ProductBookingInfoes.FirstOrDefaultAsync(x => x.CGId == CGID);

            if (productBookingInfo != null)
            {

                productBookingInfo.IsActive = true;
                productBookingInfo.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                productBookingInfo.ModifiedDate = DateTime.Now;


                var bookingInstallments = await context.BookingInstallmentSchedules
                                                    .Where(c => c.CGID == CGID)
                                                    .ToListAsync();
                foreach (var installment in bookingInstallments)
                {
                    installment.IsActive = true;
                    installment.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    installment.ModifiedDate = DateTime.Now;
                }

                var bookingCostMaps = await context.BookingCostMappings
                                                    .Where(c => c.BookingId == productBookingInfo.BookingId)
                                                    .ToListAsync();
                foreach (var bookingCostMap in bookingCostMaps)
                {
                    bookingCostMap.IsActive = true;
                    bookingCostMap.ModifiedDate = DateTime.Now;
                    bookingCostMap.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                }


                var voucherMap = await context.VoucherMaps.Where(x => x.IntegratedId == productBookingInfo.BookingId).FirstOrDefaultAsync();
                if (voucherMap != null)
                {
                    var voucher = await context.Vouchers.Where(x => x.VoucherId == voucherMap.VoucherId).FirstOrDefaultAsync();
                    if (voucher != null)
                    {
                        voucher.IsActive = true;
                        voucher.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        voucher.ModifiedDate = DateTime.Now;
                    }


                    var voucherDetails = await context.VoucherDetails
                                                        .Where(c => c.VoucherId == voucherMap.VoucherId)
                                                        .ToListAsync();
                    foreach (var voucherDetail in voucherDetails)
                    {
                        voucherDetail.IsActive = true;
                    }
                }


                await context.SaveChangesAsync();

                return true;
            }


            return false;
        }



    }
}
