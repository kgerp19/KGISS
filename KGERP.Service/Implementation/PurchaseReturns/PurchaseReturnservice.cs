using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.PurchaseReturns
{
    public class PurchaseReturnservice
    {
        private readonly ERPEntities context;
        public AccountingService _accountingService { get; set; }
        public PurchaseReturnservice(ERPEntities context)
        {
            this.context = context;
            _accountingService = new AccountingService(context);
        }
        public PurchaseReturnnewViewModel SavePurchaseReturn(PurchaseReturnnewViewModel model)
        {
            string message = "";
            bool returnExists = context.PurchaseReturns.Any(x => x.CompanyId == model.CompanyId && x.ReturnNo == model.ReturnNo && x.SupplierId == model.SupplierId);
            if (returnExists)
            {
                message = "Data already exists !";
                return model;
            }

            using (var scope = context.Database.BeginTransaction())
            {
                try
                {

                    PurchaseReturn purchaseReturn = new PurchaseReturn();
                    purchaseReturn.Active = true;
                    purchaseReturn.IsSubmited = false;
                    purchaseReturn.CreatedDate = DateTime.Now;
                    purchaseReturn.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    purchaseReturn.CompanyId = model.CompanyId;
                    purchaseReturn.SupplierId = model.SupplierId;
                    purchaseReturn.ProductType = model.ProductType;
                    purchaseReturn.ReturnNo = model.ReturnNo;
                    purchaseReturn.ReturnDate = model.ReturnDate;
                    purchaseReturn.ReturnReason = model.ReturnReason;
                    purchaseReturn.StockInfoId = model.StockInfoId;
                    purchaseReturn.ReturnBy = model.ReturnBy;
                    context.PurchaseReturns.Add(purchaseReturn);
                    context.SaveChanges();
                    PurchaseReturnDetail purchaseReturnDetail = new PurchaseReturnDetail();
                    purchaseReturnDetail.PurchaseReturnId = purchaseReturn.PurchaseReturnId;
                    purchaseReturnDetail.ProductId = model.ProductId;
                    purchaseReturnDetail.Qty = model.Qty;
                    purchaseReturnDetail.Rate = model.Rate;
                    var pc = context.Products.SingleOrDefault(f => f.ProductId == model.ProductId);
                    purchaseReturnDetail.COGS = pc.CostingPrice;
                    purchaseReturnDetail.IsActive = true;

                    context.PurchaseReturnDetails.Add(purchaseReturnDetail);
                    context.SaveChanges();
                    scope.Commit();
                    message = "Purchase return completed successfully !";
                    model.PurchaseReturnId = purchaseReturn.PurchaseReturnId;
                    model.message = message;
                    return model;
                }
                catch (Exception ex)
                {
                    message = "Purchase return failed !";
                    model.message = message;
                    scope.Rollback();
                    model.PurchaseReturnId = 0;
                    return model;
                }
            }
        }
        public async Task<PurchaseReturnnewViewModel> PurchaseReturnSubmit(PurchaseReturnnewViewModel model)
        {
            PurchaseReturnnewViewModel purchaseReturn = new PurchaseReturnnewViewModel();
            var returns = context.PurchaseReturns.Find(model.PurchaseReturnId);
            returns.IsSubmited = true;
            context.SaveChanges();


            purchaseReturn = PurchaseReturn(model);
            await _accountingService.AccountingPurchaseReturnPushFeed(purchaseReturn.CompanyId, purchaseReturn, 1);
            return purchaseReturn;
        }


        public PurchaseReturnnewViewModel PurchaseReturn(PurchaseReturnnewViewModel model)
        {
            PurchaseReturnnewViewModel purchaseReturn = new PurchaseReturnnewViewModel();
            if (model.PurchaseReturnId != 0)
            {

                purchaseReturn = (from t1 in context.PurchaseReturns.Where(x => x.Active && x.PurchaseReturnId == model.PurchaseReturnId && x.CompanyId == model.CompanyId)
                                  join t2 in context.Vendors on t1.SupplierId equals t2.VendorId
                                  join t3 in context.Companies on t1.CompanyId equals t3.CompanyId
                                  join t4 in context.StockInfoes on t1.StockInfoId equals t4.StockInfoId
                                  select new PurchaseReturnnewViewModel
                                  {
                                      PurchaseReturnId = t1.PurchaseReturnId,
                                      CompanyId = (int)t1.CompanyId,
                                      SupplierId = (int)t1.SupplierId,
                                      SupplierName = t2.Name,
                                      ReturnBy = t1.ReturnBy,
                                      ReturnNo = t1.ReturnNo,
                                      ProductType = t1.ProductType,
                                      ReturnDate = t1.ReturnDate,
                                      ReturnReason = t1.ReturnReason,
                                      StockInfoId = (int)t1.StockInfoId,
                                      StockInfoName = t4.Name,
                                      CreatedBy = t1.CreatedBy,
                                      CreatedDate = t1.CreatedDate,
                                      IsSubmited = t1.IsSubmited,
                                      AccoutHeadId = t2.HeadGLId,
                                      IntegratedFrom = "PurchaseReturn",
                                  }).FirstOrDefault();

                purchaseReturn.PurchaseReturnDetailItem = (from t1 in context.PurchaseReturnDetails.Where(x => x.PurchaseReturnId == model.PurchaseReturnId)
                                                           join t3 in context.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                           join t4 in context.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                           join t5 in context.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                           join t6 in context.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                           select new PurchaseReturnDetailViewModel
                                                           {
                                                               PurchaseReturnId = t1.PurchaseReturnId,
                                                               ProductId = t1.ProductId,
                                                               PurchaseReturnDetailId = t1.PurchaseReturnDetailId,
                                                               Qty = t1.Qty,
                                                               Rate = t1.Rate,
                                                               TotalRate = t1.Qty * t1.Rate,
                                                               ProductName = t3.ProductName,
                                                               SubCatagoryName = t4.Name,
                                                               CatagoryName = t5.Name,
                                                               COGS = t1.COGS,
                                                               AccountingHeadId = t3.AccountingHeadId,
                                                               AccountingExpenseHeadId = t3.AccountingExpenseHeadId
                                                           }).OrderByDescending(x => x.PurchaseReturnDetailId).AsEnumerable();

            }

            purchaseReturn.GrossAmount = (decimal)purchaseReturn.PurchaseReturnDetailItem.Where(f => f.PurchaseReturnId == model.PurchaseReturnId).Select(f => f.TotalRate).DefaultIfEmpty(0).Sum();
            return purchaseReturn;
        }


        public PurchaseReturnnewViewModel SaveItemPurchaseReturn(PurchaseReturnnewViewModel model)
        {
            string message = "";

            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    PurchaseReturnDetail purchaseReturnDetail = new PurchaseReturnDetail();
                    purchaseReturnDetail.PurchaseReturnId = model.PurchaseReturnId;
                    purchaseReturnDetail.ProductId = model.ProductId;
                    purchaseReturnDetail.Qty = model.Qty;
                    purchaseReturnDetail.Rate = model.Rate;

                    context.PurchaseReturnDetails.Add(purchaseReturnDetail);
                    context.SaveChanges();
                    scope.Commit();
                    message = "Purchase return completed successfully !";
                    model.PurchaseReturnId = purchaseReturnDetail.PurchaseReturnId;
                    model.message = message;
                    return model;
                }
                catch (Exception ex)
                {
                    message = "Purchase return failed !";
                    model.message = message;
                    scope.Rollback();
                    return model;
                }
            }
        }

        public PurchaseReturnnewViewModel UpdateItemPurchaseReturn(PurchaseReturnnewViewModel model)
        {
            string message = "";

            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    var findItem = context.PurchaseReturnDetails.FirstOrDefault(f => f.PurchaseReturnDetailId == model.PurchaseReturnDetailId);
                    findItem.ProductId = model.ProductId;
                    findItem.Qty = model.Qty;
                    findItem.Rate = model.Rate;
                    context.Entry(findItem).State = EntityState.Modified;
                    context.SaveChanges();
                    scope.Commit();
                    message = "Purchase return completed successfully !";
                    model.message = message;
                    model.PurchaseReturnDetailId = 0;
                    return model;
                }
                catch (Exception ex)
                {
                    message = "Purchase return failed !";
                    model.message = message;
                    scope.Rollback();
                    return model;
                }
            }
        }


        public PurchaseReturnnewViewModel ItemPurchaseReturnDelete(PurchaseReturnnewViewModel model)
        {
            var res = context.PurchaseReturnDetails.FirstOrDefault(x => x.PurchaseReturnDetailId == model.PurchaseReturnDetailId);
            context.PurchaseReturnDetails.Remove(res);
            context.SaveChanges();
            return model;
        }

        public PurchaseReturnnewViewModel DeletePurchaseReturnitem(PurchaseReturnnewViewModel model)
        {
            var res = context.PurchaseReturns.FirstOrDefault(x => x.PurchaseReturnId == model.PurchaseReturnId);
            res.Active = false;
            context.Entry(res).State = EntityState.Modified;
            context.SaveChanges();
            return model;
        }

        public async Task<PurchaseReturnnewViewModel> PurchaseReturnList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            PurchaseReturnnewViewModel purchaseReturnnewViewModel = new PurchaseReturnnewViewModel();
            purchaseReturnnewViewModel.ReturnList = await Task.Run(()=> (from t1 in context.PurchaseReturns.Where(x => x.Active == true)
                                                     join t2 in context.Vendors on t1.SupplierId equals t2.VendorId
                                                     join t3 in context.Companies on t1.CompanyId equals t3.CompanyId
                                                     join t4 in context.StockInfoes on t1.StockInfoId equals t4.StockInfoId
                                                     join t5 in context.Employees on t1.ReturnBy equals t5.Id
                                                     where t1.CompanyId==companyId && t1.ReturnDate>= fromDate && t1.ReturnDate<= toDate
                                                     select new PurchaseReturnnewViewModel
                                                     {
                                                         PurchaseReturnId = t1.PurchaseReturnId,
                                                         CompanyId = (int)t1.CompanyId,
                                                         SupplierId = (int)t1.SupplierId,
                                                         SupplierName = t2.Name,
                                                         ReturnBy = t1.ReturnBy,
                                                         ReturnNo = t1.ReturnNo,
                                                         ProductType = t1.ProductType,
                                                         ReturnDate = t1.ReturnDate,
                                                         ReturnReason = t1.ReturnReason,
                                                         StockInfoId = (int)t1.StockInfoId,
                                                         StockInfoName = t4.Name,
                                                         CreatedBy = t1.CreatedBy,
                                                         CreatedDate = t1.CreatedDate,
                                                         ReturnByName = t5.Name + "(" + t5.EmployeeId + ")",
                                                         ReturnById = t5.EmployeeId,
                                                         IsSubmited = t1.IsSubmited
                                                     }).OrderByDescending(x => x.PurchaseReturnId).AsEnumerable());
            return purchaseReturnnewViewModel;
        }


        public PurchaseReturnnewViewModel UpdatePurchaseReturn(PurchaseReturnnewViewModel model)
        {
            string message = "";
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {

                    var purchaseReturn = context.PurchaseReturns.FirstOrDefault(f => f.PurchaseReturnId == model.PurchaseReturnId);
                    purchaseReturn.Active = true;
                    purchaseReturn.CreatedDate = DateTime.Now;
                    purchaseReturn.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    purchaseReturn.CompanyId = model.CompanyId;
                    purchaseReturn.SupplierId = model.SupplierId;
                    purchaseReturn.ProductType = model.ProductType;
                    purchaseReturn.ReturnNo = model.ReturnNo;
                    purchaseReturn.ReturnDate = model.ReturnDate;
                    purchaseReturn.ReturnReason = model.ReturnReason;
                    purchaseReturn.StockInfoId = model.StockInfoId;
                    purchaseReturn.ReturnBy = model.ReturnBy;
                    context.Entry(purchaseReturn).State = EntityState.Modified;
                    context.SaveChanges();
                    scope.Commit();
                    message = "Purchase return completed successfully !";
                    return model;
                }
                catch (Exception ex)
                {
                    message = "Purchase return failed !";
                    model.message = message;
                    scope.Rollback();
                    model.PurchaseReturnId = 0;
                    return model;
                }
            }
        }
    }
}
