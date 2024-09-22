using KGERP.Data.Models;
using KGERP.Service.Implementation.Realestate.CustomersBooking;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.RealStateCostHeadService
{
    public class RealStateCostHead_Service
    {
        private ERPEntities context;
        private readonly AccountingService _accountingService;
        private readonly ICustomerBookingService customerBookingService;
        public RealStateCostHead_Service(ERPEntities db, AccountingService accountingService, ICustomerBookingService customerBookingService)
        {
            context = db;
            _accountingService = accountingService;
            this.customerBookingService = customerBookingService;
        }

        public async Task<GLDLBookingViewModel> getcosthead(GLDLBookingViewModel model)
        {
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
                                          where t1.CGId == model.CGId
                                          select new GLDLBookingViewModel
                                          {
                                              ProductStatus = t4.Status,
                                              IntegratedFrom = "ProductBookingInfo",
                                              HeadGLId = t1.HeadGLId,
                                              AccountingIncomeHeadId = model.CompanyId == (int)CompanyName.KrishibidPropertiesLimited ? t6.AccountingIncomeHeadId : t5.AccountingIncomeHeadId,
                                              CGId = t1.CGId,
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
                                              BookingNo = t3.BookingNo,
                                              BookingId = t3.BookingId,
                                              PlotNo = t4.ProductCode,
                                              PlotSize = t4.PackSize,
                                              RatePerKatha = t3.RatePerKatha,
                                              BookingMoney = t3.BookingAmt,
                                              Discount = t3.DiscountPercentage,
                                              LandValue = t3.LandValue,
                                              RestofAmount = t3.RestofAmount,
                                              CompanyId = t2.CompanyId,
                                              UnitName = t9.Name,
                                              Status = t3.Status,
                                              Step = t3.Step,

                                              ApplicationDate = t3.ApplicationDate,
                                              BookingDate = t3.BookingDate,
                                              SpecialDiscountAmt = t3.SpecialDiscountAmt,
                                              IsSubmited = t3.IsSubmitted,
                                              FileNo = t3.FileNo,
                                              InstallmentAmount = t3.InstallmentAmount,
                                              AcCostCenterId = t6.CostCenterId
                                          }).FirstOrDefaultAsync());
            model.LstPurchaseCostHeads = await Task.Run(() => (from t1 in context.ProductBookingInfoes.Where(x => x.CGId == model.CGId)
                                                               join t2 in context.BookingCostMappings on t1.BookingId equals t2.BookingId
                                                               join t3 in context.BookingCostHeads on t2.CostId equals t3.CostId
                                                               where t2.IsSnstallmentInclude == false
                                                               select new BookingHeadServiceModel
                                                               {
                                                                   CostId = t2.CostId,
                                                                   Amount = t2.Amount,
                                                                   BookingId = t2.BookingId,
                                                                   CostName = t3.CostName,
                                                                   CostsMappingId = t2.CostsMappingId,
                                                                   ReceiveAmount = t2.PaidAmount
                                                               }).ToListAsync());

            var result = await context.BookingCostHeads.Where(c => c.IsActive == true && c.CompanyId == model.CompanyId).ToListAsync();
            List<SelectDDLModel> list = new List<SelectDDLModel>();
            foreach (var items in result)
            {
                var count = context.BookingCostMappings.Where(f => f.CostId == items.CostId && f.BookingId == model.BookingId).Count();
                if (count == 0)
                {
                    SelectDDLModel models = new SelectDDLModel();
                    models.Text = items.CostName;
                    models.Value = items.CostId;
                    list.Add(models);
                }
            }
            model.CostHeads = list;
            return model;
        }


        public GLDLBookingViewModel GetCostheadsMapping(GLDLBookingViewModel model)
        {
            BookingCostMapping costMapping = new BookingCostMapping();
            costMapping.BookingId = model.BookingId;
            costMapping.Amount = 0;
            costMapping.CostId = model.CostId;
            costMapping.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            costMapping.CreatedDate = DateTime.Now;
            costMapping.IsActive = true;
            costMapping.IsSnstallmentInclude = false;
            costMapping.Percentage = 0;
            context.BookingCostMappings.Add(costMapping);
            context.SaveChanges();
            return model;
        }

        public async Task<GLDLBookingViewModel> addamount(GLDLBookingViewModel model)
        {
            BookingCostMapping costMapping = context.BookingCostMappings.FirstOrDefault(g => g.CostsMappingId == model.CostId && g.BookingId == model.BookingId);
            costMapping.Amount = costMapping.Amount + model.CostAmount;
            costMapping.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            costMapping.ModifiedDate = DateTime.Now;
            context.SaveChanges();
            if (model.CostAmount > 0)
            {
                ProductBookingInfo productBookingInfo = context.ProductBookingInfoes.Where(d => d.CGId == model.CGId).FirstOrDefault();
                productBookingInfo.RestofAmount = productBookingInfo.RestofAmount + model.CostAmount;
                context.SaveChanges();
            }
            var booking = CustomerBookingView2(model.CompanyId.Value, model.CGId);
            booking.CostHeadName = context.BookingCostHeads.FirstOrDefault(f => f.CostId == costMapping.CostId).CostName;
            booking.CostAmount = model.CostAmount;
            booking.AmandmentDate = model.AmandmentDate;
            var res = _accountingService.AccountingCostHeadUpdateGLDL((int)model.CompanyId, booking, 147);
            return model;
        }
        public GLDLBookingViewModel CustomerBookingView2(int companyId, long CGId)
        {
            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model = (from t1 in context.CustomerGroupInfoes
                     join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                     join t3 in context.ProductBookingInfoes on t1.CGId equals t3.CGId
                     join t4 in context.Products on t3.ProductId equals t4.ProductId into t4_join
                     from t4 in t4_join.DefaultIfEmpty()
                     join t5 in context.ProductSubCategories on t4.ProductSubCategoryId equals t5.ProductSubCategoryId into t5_join
                     from t5 in t5_join.DefaultIfEmpty()
                     join t6 in context.ProductCategories on t5.ProductCategoryId equals t6.ProductCategoryId into t6_join
                     from t6 in t6_join.DefaultIfEmpty()
                     where t1.CGId == CGId
                     select new GLDLBookingViewModel
                     {
                         IntegratedFrom = "BookingCostMapping",
                         HeadGLId = t1.HeadGLId,
                         AccountingIncomeHeadId = companyId == (int)CompanyName.KrishibidPropertiesLimited ? t6.AccountingIncomeHeadId : t5.AccountingIncomeHeadId,
                         CGId = t1.CGId,
                         CustomerGroupName = t1.GroupName,
                         KGRECompanyName = t2.Name,
                         PrimaryContactNo = t1.PrimaryContactNo,
                         PrimaryEmail = t1.PrimaryEmail,
                         PrimaryContactAddr = t1.PrimaryContactAddr,
                         ProductId = t3.ProductId,
                         BookingNo = t3.BookingNo,
                         RatePerKatha = t3.RatePerKatha,
                         BookingMoney = t3.BookingAmt,
                         Discount = t3.DiscountPercentage,
                         LandValue = t3.LandValue,
                         RestofAmount = t3.RestofAmount,
                         CompanyId = t2.CompanyId,
                         Status = t3.Status,
                         Step = t3.Step,
      
                         ProjectName = t6.Name,
                         ProductCategoryId = t6.ProductCategoryId,
                         BlockName = t5.Name,
                         ProductSubCategoryId = t5.ProductSubCategoryId,
                         PlotName = t4.ProductName,
                         ApplicationDate = t3.ApplicationDate,
                         BookingDate = t3.BookingDate,
                         SpecialDiscountAmt = t3.SpecialDiscountAmt,
                         IsSubmited = t3.IsSubmitted,
                         FileNo = t3.FileNo,
                         InstallmentAmount = t3.InstallmentAmount,
                         AcCostCenterId = t6.CostCenterId
                     }).FirstOrDefault();

            model.LstPurchaseCostHeads = (from t1 in context.ProductBookingInfoes.Where(x => x.CGId == CGId)
                                          join t2 in context.BookingCostMappings on t1.BookingId equals t2.BookingId
                                          join t3 in context.BookingCostHeads on t2.CostId equals t3.CostId
                                          select new BookingHeadServiceModel
                                          {
                                              CostId = t2.CostId,
                                              Amount = t2.Amount,
                                              BookingId = t2.BookingId,
                                              CostName = t3.CostName,
                                          }).ToList();
            return model;
        }

    }
}
