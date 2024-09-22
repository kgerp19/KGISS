using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate.BookingAprovalList
{
    public class BookingAprovalStatusService : IBookingAprovalStatus
    {
        ERPEntities context = new ERPEntities();

        public async Task<GLDLBookingViewModel> BookingAprovalDraft(int status, int companyId)
        {

            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model.CompanyId = companyId;
            model.datalist = await Task.Run(() => (from t1 in context.CustomerGroupInfoes.Where(f => f.IsActive == true && f.CompanyId == companyId)
                                                   join t7 in context.ProductBookingInfoes.Where(f => f.IsActive == true && (f.Step == 1 || f.Status == 5)) on t1.CGId equals t7.CGId
                                                   join t2 in context.Companies.Where(f => f.IsActive == true) on t1.CompanyId equals t2.CompanyId
                                                   join t3 in context.ProductBookingInfoes.Where(f => f.IsActive == true) on t1.CGId equals t3.CGId
                                                   join t4 in context.Products.Where(f => f.IsActive == true) on t3.ProductId equals t4.ProductId
                                                   join t5 in context.ProductSubCategories.Where(f => f.IsActive == true) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                   join t6 in context.ProductCategories.Where(f => f.IsActive == true) on t5.ProductCategoryId equals t6.ProductCategoryId

                                                   select new GLDLBookingViewModel
                                                   {
                                                       CGId = t1.CGId,
                                                       BookingId = t7.BookingId,
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
                                                       PlotSize = (int)t4.PackSize,
                                                       RatePerKatha = (decimal)t4.UnitPrice,
                                                       BookingMoney = t3.BookingAmt,
                                                       Discount = t3.DiscountPercentage,
                                                       LandValue = (decimal)((int)t4.PackSize * t4.UnitPrice),
                                                       RestofAmount = t3.RestofAmount,
                                                       CompanyId = t2.CompanyId,
                                                       Status = t7.Status,
                                                       Step = t7.Step,
                                                   }).ToListAsync());
            return model;
        }
        public async Task<GLDLBookingViewModel> BookingAprovalRecheck(int status, int companyId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model.CompanyId = companyId;
            model.datalist = await Task.Run(() => (from t1 in context.CustomerGroupInfoes.Where(f => f.IsActive == true && f.CompanyId == companyId)
                                                   join t7 in context.ProductBookingInfoes.Where(f => f.IsActive == true && f.Step == 2) on t1.CGId equals t7.CGId
                                                   join t2 in context.Companies.Where(f => f.IsActive == true) on t1.CompanyId equals t2.CompanyId
                                                   join t3 in context.ProductBookingInfoes.Where(f => f.IsActive == true) on t1.CGId equals t3.CGId
                                                   join t4 in context.Products.Where(f => f.IsActive == true) on t3.ProductId equals t4.ProductId
                                                   join t5 in context.ProductSubCategories.Where(f => f.IsActive == true) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                   join t6 in context.ProductCategories.Where(f => f.IsActive == true) on t5.ProductCategoryId equals t6.ProductCategoryId

                                                   select new GLDLBookingViewModel
                                                   {
                                                       CGId = t1.CGId,
                                                       BookingId = t7.BookingId,
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
                                                       PlotSize = (int)t4.PackSize,
                                                       RatePerKatha = (decimal)t4.UnitPrice,
                                                       BookingMoney = t3.BookingAmt,
                                                       Discount = t3.DiscountPercentage,
                                                       LandValue = (decimal)((int)t4.PackSize * t4.UnitPrice),
                                                       RestofAmount = t3.RestofAmount,
                                                       CompanyId = t2.CompanyId,
                                                       Status = t7.Status,
                                                       Step = t7.Step,
                                                   }).ToListAsync());
            return model;
        }
        public async Task<GLDLBookingViewModel> BookingAprovalApprove(int status, int companyId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model.CompanyId = companyId;
            model.datalist = await Task.Run(() => (from t1 in context.CustomerGroupInfoes.Where(f => f.IsActive == true && f.CompanyId == companyId)
                                                   join t7 in context.ProductBookingInfoes.Where(f => f.IsActive == true && f.Step == 3) on t1.CGId equals t7.CGId
                                                   join t2 in context.Companies.Where(f => f.IsActive == true) on t1.CompanyId equals t2.CompanyId
                                                   join t3 in context.ProductBookingInfoes.Where(f => f.IsActive == true) on t1.CGId equals t3.CGId
                                                   join t4 in context.Products.Where(f => f.IsActive == true) on t3.ProductId equals t4.ProductId
                                                   join t5 in context.ProductSubCategories.Where(f => f.IsActive == true) on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                   join t6 in context.ProductCategories.Where(f => f.IsActive == true) on t5.ProductCategoryId equals t6.ProductCategoryId

                                                   select new GLDLBookingViewModel
                                                   {
                                                       CGId = t1.CGId,
                                                       BookingId = t7.BookingId,
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
                                                       PlotSize = (int)t4.PackSize,
                                                       RatePerKatha = (decimal)t4.UnitPrice,
                                                       BookingMoney = t3.BookingAmt,
                                                       Discount = t3.DiscountPercentage,
                                                       LandValue = (decimal)((int)t4.PackSize * t4.UnitPrice),
                                                       RestofAmount = t3.RestofAmount,
                                                       CompanyId = t2.CompanyId,
                                                       Status = t7.Status,
                                                       Step = t7.Step,
                                                   }).ToListAsync());
            return model;
        }

        public async Task<GLDLBookingViewModel> BookingStatusChange(GLDLBookingViewModel model)
        {
            var item = await context.ProductBookingInfoes.FirstOrDefaultAsync(d => d.BookingId == model.BookingId);
            item.Status = model.Status;
            if (model.Status == 1)
            {
                item.Step = 1;
                item.CheckedBy = 0;
                item.ApprovedBy = 0;
                item.FinalApproverBy = 0;
            }
            if (model.Status == 2)
            {
                item.Step = 2;
                item.CheckedBy = model.EntryBy;
                item.ApprovedBy = 0;
                item.FinalApproverBy = 0;
                item.CheckDate = DateTime.Now;
            }
            if (model.Status == 3)
            {
                item.Step = 3;
                item.ApprovedBy = model.EntryBy;
                item.ApproveDate = DateTime.Now;
                item.FinalApproverBy = 0;
            }
            if (model.Status == 4)
            {
                item.Step = 4;
                item.FinalApproverBy = model.EntryBy;
                item.FinalApvDate = DateTime.Now;
            }

            if (model.Status == 5)
            {
                item.Step = 1;
                item.CheckedBy = 0;
                item.ApprovedBy = 0;
            }
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
            return model;
        }

        public async Task<GLDLBookingViewModel> BookingforDealingOfficer(long EmployeeId, int companyId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model.CompanyId = companyId;
            model.datalist = await Task.Run(() => (from t3 in context.ProductBookingInfoes.Where(f => f.IsActive == true && f.SoldBy == EmployeeId)
                                                   join t1 in context.CustomerGroupInfoes.Where(f => f.IsActive == true && f.CompanyId == companyId) on t3.CGId equals t1.CGId
                                                   join t2 in context.Companies.Where(f => f.IsActive == true) on t1.CompanyId equals t2.CompanyId
                                                   join t4 in context.Products on t3.ProductId equals t4.ProductId
                                                   join t5 in context.ProductSubCategories on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                   join t6 in context.ProductCategories on t5.ProductCategoryId equals t6.ProductCategoryId

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
                                                       PlotSize = (int)t4.PackSize,
                                                       RatePerKatha = (decimal)t4.UnitPrice,
                                                       BookingMoney = t3.BookingAmt,
                                                       Discount = t3.DiscountPercentage,
                                                       LandValue = (decimal)((int)t4.PackSize * t4.UnitPrice),
                                                       RestofAmount = t3.RestofAmount,
                                                       CompanyId = t2.CompanyId,
                                                       Status = t3.Status,
                                                       Step = t3.Step,
                                                       ApplicationDate = t3.ApplicationDate,
                                                       BookingDate = t3.BookingDate,
                                                       SpecialDiscountAmt = t3.SpecialDiscountAmt,
                                                       BookingNo = t3.BookingNo,
                                                   }).ToListAsync());
            return model;
        }

        public async Task<GLDLBookingViewModel> BookingforTeamLead(long EmployeeId, int companyId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model.CompanyId = companyId;
            model.datalist = await Task.Run(() => (from t3 in context.ProductBookingInfoes.Where(f => f.IsActive == true && f.TeamLeadId == EmployeeId)
                                                   join t1 in context.CustomerGroupInfoes.Where(f => f.IsActive == true && f.CompanyId == companyId) on t3.CGId equals t1.CGId
                                                   join t2 in context.Companies.Where(f => f.IsActive == true) on t1.CompanyId equals t2.CompanyId
                                                   join t4 in context.Products on t3.ProductId equals t4.ProductId
                                                   join t5 in context.ProductSubCategories on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                   join t6 in context.ProductCategories on t5.ProductCategoryId equals t6.ProductCategoryId

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
                                                       PlotSize = (int)t4.PackSize,
                                                       RatePerKatha = (decimal)t4.UnitPrice,
                                                       BookingMoney = t3.BookingAmt,
                                                       Discount = t3.DiscountPercentage,
                                                       LandValue = (decimal)((int)t4.PackSize * t4.UnitPrice),
                                                       RestofAmount = t3.RestofAmount,
                                                       CompanyId = t2.CompanyId,
                                                       Status = t3.Status,
                                                       Step = t3.Step,
                                                       ApplicationDate = t3.ApplicationDate,
                                                       BookingDate = t3.BookingDate,
                                                       SpecialDiscountAmt = t3.SpecialDiscountAmt,
                                                       BookingNo = t3.BookingNo,
                                                   }).ToListAsync());
            return model;
        }

        public async Task<GLDLBookingViewModel> MdAprovalApprove(int status, int companyId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model.CompanyId = companyId;
            model.datalist = await Task.Run(() => (from t3 in context.ProductBookingInfoes.Where(f => f.IsActive == true)
                                                   join t1 in context.CustomerGroupInfoes.Where(f => f.IsActive == true && f.CompanyId == companyId) on t3.CGId equals t1.CGId
                                                   join t2 in context.Companies.Where(f => f.IsActive == true) on t1.CompanyId equals t2.CompanyId
                                                   join t4 in context.Products on t3.ProductId equals t4.ProductId
                                                   join t5 in context.ProductSubCategories on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                   join t6 in context.ProductCategories on t5.ProductCategoryId equals t6.ProductCategoryId

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
                                                       PlotSize = (int)t4.PackSize,
                                                       RatePerKatha = (decimal)t4.UnitPrice,
                                                       BookingMoney = t3.BookingAmt,
                                                       Discount = t3.DiscountPercentage,
                                                       LandValue = (decimal)((int)t4.PackSize * t4.UnitPrice),
                                                       RestofAmount = t3.RestofAmount,
                                                       CompanyId = t2.CompanyId,
                                                       Status = t3.Status,
                                                       Step = t3.Step,
                                                       ApplicationDate = t3.ApplicationDate,
                                                       BookingDate = t3.BookingDate,
                                                       SpecialDiscountAmt = t3.SpecialDiscountAmt,
                                                       BookingNo = t3.BookingNo,
                                                   }).OrderByDescending(f=>f.Status==3).ToListAsync());
            return model;
        }

        public async Task<GLDLBookingViewModel> DMdAprovalApprove(int status, int companyId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model.CompanyId = companyId;
            model.datalist = await Task.Run(() => (from t3 in context.ProductBookingInfoes.Where(f => f.IsActive == true)
                                                   join t1 in context.CustomerGroupInfoes.Where(f => f.IsActive == true && f.CompanyId == companyId) on t3.CGId equals t1.CGId
                                                   join t2 in context.Companies.Where(f => f.IsActive == true) on t1.CompanyId equals t2.CompanyId
                                                   join t4 in context.Products on t3.ProductId equals t4.ProductId
                                                   join t5 in context.ProductSubCategories on t4.ProductSubCategoryId equals t5.ProductSubCategoryId
                                                   join t6 in context.ProductCategories on t5.ProductCategoryId equals t6.ProductCategoryId

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
                                                       PlotSize = (int)t4.PackSize,
                                                       RatePerKatha = (decimal)t4.UnitPrice,
                                                       BookingMoney = t3.BookingAmt,
                                                       Discount = t3.DiscountPercentage,
                                                       LandValue = (decimal)((int)t4.PackSize * t4.UnitPrice),
                                                       RestofAmount = t3.RestofAmount,
                                                       CompanyId = t2.CompanyId,
                                                       Status = t3.Status,
                                                       Step = t3.Step,
                                                       ApplicationDate = t3.ApplicationDate,
                                                       BookingDate = t3.BookingDate,
                                                       SpecialDiscountAmt = t3.SpecialDiscountAmt,
                                                       BookingNo = t3.BookingNo,
                                                   }).OrderByDescending(f => f.Status == 3).ToListAsync());
            return model;
        }
    }
}
