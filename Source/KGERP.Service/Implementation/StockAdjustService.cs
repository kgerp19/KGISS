using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class StockAdjustService : IStockAdjustService
    {
        private bool disposed = false;
        private readonly ERPEntities context;
        public StockAdjustService(ERPEntities context)
        {
            this.context = context;
        }
        public async Task<StockAdjustModel> GetStockAdjusts(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            StockAdjustModel stockAdjustModel = new StockAdjustModel();
            stockAdjustModel.CompanyId = companyId;
            stockAdjustModel.DataList = await Task.Run(() => (from t1 in context.StockAdjusts
                                                              where t1.CompanyId == companyId
                                                              && t1.AdjustDate >= fromDate
                                                              && t1.AdjustDate <= toDate
                                                              && t1.TypeId == 0 && t1.IsActive == true
                                                              select new StockAdjustModel
                                                              {
                                                                  StockAdjustId = t1.StockAdjustId,
                                                                  AdjustDate = t1.AdjustDate,
                                                                  InvoiceNo = t1.InvoiceNo,
                                                                  Remarks = t1.Remarks,
                                                                  CompanyId = t1.CompanyId,
                                                                  IsSubmited = t1.IsFinalized
                                                              }).OrderByDescending(o => o.AdjustDate)
                                                                .AsEnumerable());

            return stockAdjustModel;
        }
        public async Task<IssueVm> GetStockIssues(int companyId, DateTime? fromDate, DateTime? toDate, int status)
        {
            IssueVm model = new IssueVm();
            model.CompanyId = companyId;
            model.DataList = await Task.Run(() => (from t1 in context.StockAdjusts
                                                   join t2 in context.Employees on t1.IssueTo equals t2.Id
                                                   where t1.CompanyId == companyId
                                                   && t1.AdjustDate >= fromDate
                                                   && t1.AdjustDate <= toDate
                                                   && t1.TypeId == 1
                                                   && (status == 1 ? t1.IsFinalized : !t1.IsFinalized)
                                                   select new IssueVm
                                                   {

                                                       IssueDate = t1.AdjustDate,
                                                       IssueNo = t1.InvoiceNo,
                                                       Remarks = t1.Remarks,
                                                       IssueTo = t1.IssueTo,
                                                       IssueToName = t2.Name,
                                                       TypeId = t1.TypeId,
                                                       CompanyId = t1.CompanyId,
                                                       IssueId = t1.StockAdjustId,
                                                       IsSubmited = t1.IsFinalized

                                                   }).OrderByDescending(o => o.IssueId).AsEnumerable());

            return model;
        }
        public async Task<IssueVm> IssueSlaveGet(int companyId, int issueId)
        {
            IssueVm model = new IssueVm();
            model = await Task.Run(() => (from t1 in context.StockAdjusts.Where(x => x.IsActive && x.StockAdjustId == issueId
                                          && x.CompanyId == companyId && x.TypeId == 1)
                                          join t2 in context.Employees on t1.IssueTo equals t2.Id


                                          select new IssueVm
                                          {
                                              IssueNo = t1.InvoiceNo,
                                              IssueId = t1.StockAdjustId,
                                              IssueDate = t1.AdjustDate,
                                              CompanyId = t1.CompanyId,
                                              Remarks = t1.Remarks,
                                              IssueTo = t1.IssueTo,
                                              IssueToName = t2.Name,
                                              IsSubmited = t1.IsFinalized,
                                              CreatedBy = t1.CreatedBy,
                                              CreatedDate = t1.CreatedDate
                                          }).FirstOrDefault());

            model.Items = await Task.Run(() => (from t1 in context.StockAdjustDetails
                                                .Where(x => x.IsActive && x.StockAdjustId == issueId)
                                                join t3 in context.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                join t4 in context.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                join t5 in context.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                join t6 in context.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                select new IssusDetailVm
                                                {
                                                    IssueDetailId = t1.StockAdjustDetailId,
                                                    IssueId = t1.StockAdjustId,
                                                    Quantity = t1.LessQty,
                                                    UnitPrice = t1.UnitPrice,
                                                    UnitName = t6.Name,
                                                    ProductId = t3.ProductId,
                                                    ProductName = t4.Name + " " + t3.ProductName

                                                }).OrderByDescending(x => x.IssueDetailId).ToListAsync());


            return model;
        }

        public async Task<int> IssueAdd(IssueVm model)
        {
            int result = -1;
            var issueMax = context.StockAdjusts.Where(x => x.CompanyId == model.CompanyId).Count() + 1;
            string issueNo = @"IS-" +
                            DateTime.Now.ToString("yy") +
                            DateTime.Now.ToString("MM") +
                            DateTime.Now.ToString("dd") + "-" +
                            issueMax.ToString().PadLeft(2, '0');

            StockAdjust obj = new StockAdjust
            {
                InvoiceNo = issueNo,
                IsFinalized = model.IsSubmited,
                AdjustDate = model.IssueDate,
                CompanyId = model.CompanyId,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreatedDate = DateTime.Now,
                IsActive = true,
                TypeId = 1,
                Remarks = model.Remarks,
                IssueTo = model.IssueTo
            };
            context.StockAdjusts.Add(obj);
            if (await context.SaveChangesAsync() > 0)
            {
                result = obj.StockAdjustId;
            }
            return result;
        }
        public async Task<int> IssueSlaveAdd(IssueVm model)
        {
            int result = -1;
            StockAdjustDetail objDetail = new StockAdjustDetail
            {
                StockAdjustId = model.IssueId,
                UnitPrice = model.UnitPrice,
                LessQty = model.Quantity,
                ProductId = model.ProductId,
                CreatedBy = System.Web.HttpContext.Current.Session["EmployeeName"].ToString(),
                CreatedDate = DateTime.Now,
                IsActive = true
            };
            context.StockAdjustDetails.Add(objDetail);
            if (await context.SaveChangesAsync() > 0)
            {
                result = objDetail.StockAdjustId;
            }
            return result;
        }
        public async Task<int> IssueSlaveEdit(IssueVm model)
        {
            var result = -1;
            StockAdjustDetail objDetail = await context.StockAdjustDetails.FindAsync(model.IssueDetailId);

            objDetail.ProductId = model.ProductId;
            objDetail.LessQty = model.Quantity;
            objDetail.UnitPrice = model.UnitPrice;


            objDetail.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            objDetail.ModifiedDate = DateTime.Now;
            if (await context.SaveChangesAsync() > 0)
            {
                result = objDetail.StockAdjustDetailId;
            }

            return result;
        }
        public async Task<int> IssueSlaveDelete(int id)
        {
            int result = -1;
            StockAdjustDetail objDetail = await context.StockAdjustDetails.FindAsync(id);

            if (objDetail != null)
            {
                objDetail.IsActive = false;
                if (await context.SaveChangesAsync() > 0)
                {
                    result = objDetail.StockAdjustDetailId;
                }
            }
            return result;
        }
        public async Task<IssueVm> GetSingleIssueSlave(int id)
        {
            var v = await Task.Run(() => (from t1 in context.StockAdjustDetails
                                          join t2 in context.Products on t1.ProductId equals t2.ProductId
                                          join t3 in context.Units on t2.UnitId equals t3.UnitId

                                          where t1.StockAdjustDetailId == id
                                          select new IssueVm
                                          {
                                              ProductName = t2.ProductName,
                                              ProductId = t2.ProductId,
                                              IssueId = t1.StockAdjustId,

                                              IssueDetailId = t1.StockAdjustDetailId,
                                              Quantity = t1.LessQty,
                                              UnitPrice = t1.UnitPrice,
                                              UnitName = t3.Name,
                                              CompanyId = t2.CompanyId ?? 0,


                                          }).FirstOrDefault());
            return v;
        }
        public async Task<IssueVm> GetSingleIssue(int id)
        {

            var v = await Task.Run(() => (from t1 in context.StockAdjusts
                                          join t2 in context.Employees on t1.IssueTo equals t2.Id
                                          where t1.StockAdjustId == id
                                          select new IssueVm
                                          {
                                              IssueId = t1.StockAdjustId,
                                              IssueToName = t2.Name,
                                              IssueTo = t1.IssueTo,
                                              IssueNo = t1.InvoiceNo,
                                              IssueDate = t1.AdjustDate,
                                              Remarks = t1.Remarks,
                                              IsSubmited = t1.IsFinalized,
                                              CompanyId = t1.CompanyId,
                                              Status = t1.Status,
                                              TypeId = t1.TypeId,
                                          }).FirstOrDefault());
            return v;
        }
        public async Task<int> IssueSubmit(int? id = 0)
        {
            int result = -1;
            StockAdjust model = await context.StockAdjusts.FindAsync(id);
            if (model != null)
            {
                if (model.Status == (int)EnumPOStatus.Draft)
                {
                    model.Status = (int)EnumPOStatus.Submitted;
                }
                else
                {
                    model.Status = (int)EnumPOStatus.Draft;
                }
                model.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                model.ModifiedDate = DateTime.Now;
                if (await context.SaveChangesAsync() > 0)
                {
                    result = model.StockAdjustId;
                }
            }
            return result;
        }
        public object GetAutoCompleteEmployee(string prefix)
        {
            var v = (from t1 in context.Employees
                     where t1.Name.StartsWith(prefix) || t1.EmployeeId.StartsWith(prefix)
                     select new
                     {
                         label = t1.Name + "[" + t1.EmployeeId + "]",
                         val = t1.Id
                     }).OrderBy(x => x.label).Take(20).ToList();
            return v;
        }
        public List<StockAdjustModel> GetStockAdjusts(int companyId, string searchDate, string searchText)
        {
            DateTime? dateSearch = null;
            dateSearch = !string.IsNullOrEmpty(searchDate) ? Convert.ToDateTime(searchDate) : dateSearch;

            List<StockAdjustModel> stockAdjusts = context.Database.SqlQuery<StockAdjustModel>("select StockAdjustId,InvoiceNo,AdjustDate,Remarks from Erp.StockAdjust where CompanyId={0}", companyId).ToList();

            if (dateSearch == null)
            {
                return stockAdjusts.Where(x => (x.InvoiceNo.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                    (x.Remarks.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText))
                                    ).OrderByDescending(x => x.AdjustDate).ToList();
            }
            if (string.IsNullOrEmpty(searchText) && dateSearch != null)
            {
                return stockAdjusts.Where(x => x.AdjustDate == dateSearch).ToList();
            }


            return stockAdjusts.Where(x => x.AdjustDate == dateSearch &&
                                (x.InvoiceNo.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText)) ||
                                (x.Remarks.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText))
                               ).OrderByDescending(x => x.AdjustDate).ToList();
        }




        public StockAdjustModel GetStockAdjust(int id)
        {
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            string invoiceNo = string.Empty;
            if (id <= 0)
            {
                IQueryable<StockAdjust> stockAdjusts = context.StockAdjusts.Where(x => x.CompanyId == companyId);
                int count = stockAdjusts.Count();
                if (count == 0)
                {
                    return new StockAdjustModel()
                    {
                        InvoiceNo = GenerateSequenceNumber(0),
                        CompanyId = companyId,
                        AdjustDate = DateTime.Today
                    };
                }

                invoiceNo = stockAdjusts.Where(x => x.CompanyId == companyId).OrderByDescending(x => x.StockAdjustId).FirstOrDefault().InvoiceNo;
                string numberPart = invoiceNo.Substring(5, 6);
                int lastReceivedNo = Convert.ToInt32(numberPart);
                invoiceNo = GenerateSequenceNumber(lastReceivedNo);
                return new StockAdjustModel()
                {
                    InvoiceNo = invoiceNo,
                    CompanyId = companyId,
                    AdjustDate = DateTime.Today

                };

            }
            StockAdjust stockAdjust = context.StockAdjusts.Where(x => x.StockAdjustId == id).FirstOrDefault();
            if (stockAdjust == null)
            {
                throw new Exception("Date not found");
            }
            StockAdjustModel model = ObjectConverter<StockAdjust, StockAdjustModel>.Convert(stockAdjust);
            return model;

        }

        public bool SaveStockAdjust(int id, StockAdjustModel model)
        {
            int noOfRowsInsertedToProductStore = 0;
            int stockInfoId = 2;
            StockAdjust stockAdjust = ObjectConverter<StockAdjustModel, StockAdjust>.Convert(model);
            if (id > 0)
            {
                stockAdjust = context.StockAdjusts.Where(x => x.StockAdjustId == id).FirstOrDefault();
                if (stockAdjust == null)
                {
                    throw new Exception("Data not found!");
                }
                stockAdjust.ModifiedDate = DateTime.Now;
                stockAdjust.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                foreach (var StockAdjustDetail in stockAdjust.StockAdjustDetails)
                {
                    StockAdjustDetail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    StockAdjustDetail.CreatedDate = DateTime.Now;

                }
                stockAdjust.CreatedDate = DateTime.Now;
                stockAdjust.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }
            stockAdjust.InvoiceNo = model.InvoiceNo;
            stockAdjust.AdjustDate = model.AdjustDate;
            stockAdjust.Remarks = model.Remarks;
            context.StockAdjusts.Add(stockAdjust);

            context.Entry(stockAdjust).State = stockAdjust.StockAdjustId == 0 ? EntityState.Added : EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                noOfRowsInsertedToProductStore = context.Database.ExecuteSqlCommand("exec sp_Feed_StockAdjustment {0},{1}", stockAdjust.StockAdjustId, stockInfoId);
            }
            return noOfRowsInsertedToProductStore > 0;
        }


        private string GenerateSequenceNumber(int lastInvoiceNo)
        {
            lastInvoiceNo = lastInvoiceNo + 1;
            return "SA-RM" + lastInvoiceNo.ToString().PadLeft(6, '0');
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


    }
}
