using KGERP.Data.CustomModel;
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
    public class ConvertedProductService : IConvertedProductService
    {
        private readonly ERPEntities context;
        private readonly AccountingService _accountingService;

        public ConvertedProductService(ERPEntities context, AccountingService accountingService)
        {
            this.context = context;
            _accountingService = accountingService;
        }

        public async Task<ConvertedProductModel> GetConvertedProducts(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            ConvertedProductModel convertedProductModel = new ConvertedProductModel();
            convertedProductModel.CompanyId = companyId;
            convertedProductModel.DataList = await Task.Run(() => (from t1 in context.ConvertedProducts
                                                                   join t2 in context.Products on t1.ConvertFrom equals t2.ProductId
                                                                   join t3 in context.Products on t1.ConvertTo equals t3.ProductId
                                                                   where t1.CompanyId == companyId
                                                                   && t1.ConvertedDate >= fromDate
                                                                   && t1.ConvertedDate <= toDate
                                                                   select new ConvertedProductModel
                                                                   {
                                                                       InvoiceNo = t1.InvoiceNo,
                                                                       ConvertedDate = t1.ConvertedDate.Value,
                                                                       ConvertStatus = t1.ConvertStatus,
                                                                       ConvertedQty = t1.ConvertedQty,
                                                                       ToItem = t3.ProductName,
                                                                       FromItem = t2.ProductName,
                                                                       ConvertedProductId = t1.ConvertedProductId,
                                                                       CompanyId = t1.CompanyId,
                                                                       ConvertedUnitPrice = t1.ConvertedUnitPrice,
                                                                       ConvertFromUnitPrice = t1.ConvertFromUnitPrice,
                                                                       CreatedBy = t1.CreatedBy,
                                                                       IntegratedFrom = "ConvertedProducts"

                                                                   }).OrderByDescending(o => o.ConvertedDate).AsEnumerable());

            return convertedProductModel;

        }

        public List<ConvertedProductModel> GetConvertedProducts(DateTime? searchDate, string searchText, int companyId)
        {
            return context.Database.SqlQuery<ConvertedProductModel>("exec spGetConvertedProducts {0},{1},{2}", companyId, searchText, searchDate).OrderByDescending(x => x.ConvertedDate).ToList();
        }
        public async Task<ConvertedProductModel> GetConvertedProductById(int companyId, int convertedProductId)
        {
            ConvertedProductModel convertedProductModel = new ConvertedProductModel();
            if (convertedProductId > 0)
            {
                convertedProductModel = (from t1 in context.ConvertedProducts
                                         join t2 in context.Products on t1.ConvertFrom equals t2.ProductId
                                         join t3 in context.ProductSubCategories on t2.ProductSubCategoryId equals t3.ProductSubCategoryId
                                         join t4 in context.ProductCategories on t3.ProductCategoryId equals t4.ProductCategoryId

                                         join t5 in context.Products on t1.ConvertTo equals t5.ProductId
                                         join t6 in context.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId
                                         join t7 in context.ProductCategories on t6.ProductCategoryId equals t7.ProductCategoryId
                                         where t1.CompanyId == companyId && t1.ConvertedProductId == convertedProductId

                                         select new ConvertedProductModel
                                         {
                                             IntegratedFrom = "ConvertedProduct",
                                             ConvertedDate = t1.ConvertedDate.Value,
                                             ConvertFrom = t1.ConvertFrom,
                                             ConvertTo = t1.ConvertTo,
                                             CompanyId = t1.CompanyId,
                                             ProductFromName = t4.Name + " " + t3.Name + " " + t2.ProductName,
                                             ProductToName = t7.Name + " " + t6.Name + " " + t5.ProductName,
                                             ConvertFromUnitPrice = t1.ConvertFromUnitPrice,
                                             ConvertedUnitPrice = t1.ConvertedUnitPrice,
                                             ConvertedQty = t1.ConvertedQty,
                                             ConvertFromAccountHeadId = t2.AccountingHeadId,
                                             ConvertToAccountHeadId = t5.AccountingHeadId,
                                             InvoiceNo = t1.InvoiceNo
                                         }).FirstOrDefault();
            }
            return convertedProductModel;
        }
        public async Task<ConvertedProductModel> GetConvertedProduct(int companyId, int convertedId)
        {
            string invoiceNo = string.Empty;
            if (convertedId <= 0)
            {
                IQueryable<ConvertedProduct> convertedProducts = context.ConvertedProducts
                    .Where(x => x.CompanyId == companyId);
                int count = convertedProducts.Count();
                if (count == 0)
                {
                    return new ConvertedProductModel()
                    {
                        InvoiceNo = GenerateSequenceNumber(0),
                        CompanyId = companyId,
                        ConvertedDate = DateTime.Now
                    };
                }

                convertedProducts = convertedProducts.Where(x => x.CompanyId == companyId)
                    .OrderByDescending(x => x.ConvertedProductId).Take(1);
                invoiceNo = convertedProducts.ToList().FirstOrDefault().InvoiceNo;
                string numberPart = invoiceNo.Substring(2, 6);
                int lastNumberPart = Convert.ToInt32(numberPart);
                invoiceNo = GenerateSequenceNumber(lastNumberPart);
                return new ConvertedProductModel()
                {
                    InvoiceNo = invoiceNo,
                    CompanyId = companyId,
                    ConvertedDate = DateTime.Now
                };

            }
            ConvertedProductModel model = new ConvertedProductModel();

            model = await Task.Run(() => (from t1 in context.ConvertedProducts
                                          join t2 in context.Products on t1.ConvertFrom equals t2.ProductId
                                          join t3 in context.ProductSubCategories on t2.ProductSubCategoryId equals t3.ProductSubCategoryId
                                          join t4 in context.ProductCategories on t3.ProductCategoryId equals t4.ProductCategoryId
                                          join t5 in context.Products on t1.ConvertTo equals t5.ProductId
                                          join t6 in context.ProductSubCategories on t5.ProductSubCategoryId equals t6.ProductSubCategoryId
                                          join t7 in context.ProductCategories on t6.ProductCategoryId equals t7.ProductCategoryId

                                          where t1.ConvertedProductId == convertedId
                                          && t1.CompanyId == companyId
                                          select new ConvertedProductModel
                                          {
                                              ProductFromName = t4.Name + " " + t3.Name + " " + t2.ProductName,
                                              ProductToName = t7.Name + " " + t6.Name + " " + t5.ProductName,
                                              CompanyId = t1.CompanyId,
                                              InvoiceNo = t1.InvoiceNo,
                                              ConvertedDate = t1.ConvertedDate.Value,
                                              ConvertedProductId = t1.ConvertedProductId,
                                              ConvertedQty = t1.ConvertedQty,
                                              ConvertFrom = t1.ConvertFrom,
                                              ConvertTo = t1.ConvertTo,
                                              ConvertStatus = t1.ConvertStatus,
                                              ConvertedUnitPrice = t1.ConvertedUnitPrice
                                          }

                ).FirstOrDefaultAsync());

            return model;
        }

        private string GenerateSequenceNumber(int lastReceivedNo)
        {
            int num = ++lastReceivedNo;
            return "CV" + num.ToString().PadLeft(6, '0');
        }

        public async Task<int> SaveConvertedProduct(ConvertedProductModel model)
        {
            ConvertedProduct objectToSave = await context.ConvertedProducts
                .SingleOrDefaultAsync(s => s.ConvertedProductId == model.ConvertedProductId);

            if (objectToSave == null)
            {
                objectToSave = new ConvertedProduct()
                {
                    CompanyId = model.CompanyId,
                    ConvertedDate = model.ConvertedDate,
                    ConvertFrom = model.ConvertFrom,
                    ConvertTo = model.ConvertTo,
                    InvoiceNo = model.InvoiceNo,
                    ConvertedQty = model.ConvertedQty,
                    ConvertedUnitPrice = model.ConvertedUnitPrice,
                    ConvertFromUnitPrice = model.ConvertFromUnitPrice,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    ConvertStatus = "P"
                };

                context.ConvertedProducts.Add(objectToSave);
                await context.SaveChangesAsync();
            }

            return objectToSave.ConvertedProductId;

        }
        public decimal GetStockckAvailableQuantity(int companyId, int productId, int stockFrom, string selectedDate)
        {
            //var sql = $"exec sp_FinishGoodStockByProductDepoWise '{selectedDate}',{companyId},{stockFrom},{productId}";
            //ProductCurrentStockModel data = context.Database.SqlQuery<ProductCurrentStockModel>(sql).FirstOrDefault();
            DateTime date = Convert.ToDateTime(selectedDate);
            var result = context.Database.SqlQuery<FeedFinishedProductStock>("Finished_Product_Wise_StockReport {0}, {1}, {2}", date, companyId, productId).FirstOrDefault();     
            return result.ClosingQty;
        }

        public async Task<bool> ChangeConvertStatus(int convertedProductId, int companyId, string convertStatus)

        {


            ConvertedProduct convertedProduct = context.ConvertedProducts.Where(x => x.ConvertedProductId == convertedProductId).FirstOrDefault();
            if (convertedProduct == null)
            {
                throw new Exception("Data not found!");
            }
            //var productPriceConvertFrom = context.ProductPrices
            //    .Where(q => q.PriceType == "TP"
            //    && q.ProductId == convertedProduct.ConvertFrom
            //    && q.CompanyId == companyId
            //    && q.PriceUpdatedDate <= convertedProduct.ConvertedDate)
            //    .OrderByDescending(o => o.PriceUpdatedDate)
            //    .FirstOrDefault();

            //decimal? ConvertFromPrice = productPriceConvertFrom.UnitPrice;

            //var productPriceConvertTo = context.ProductPrices
            //   .Where(q => q.PriceType == "TP"
            //   && q.ProductId == convertedProduct.ConvertTo
            //   && q.CompanyId == companyId
            //   && q.PriceUpdatedDate <= convertedProduct.ConvertedDate)
            //   .OrderByDescending(o => o.PriceUpdatedDate)
            //   .FirstOrDefault();
            //decimal? ConvertToPrice = productPriceConvertTo.UnitPrice;

            // decimal? convertedUnitPrice = context.Database.SqlQuery<decimal?>("exec spGetConvertedUnitPrice {0}", convertedProduct.ConvertFrom).FirstOrDefault();

            convertedProduct.ConvertStatus = convertStatus;
            //convertedProduct.ConvertFromUnitPrice = ConvertFromPrice ?? 0;
            //convertedProduct.ConvertedUnitPrice = ConvertToPrice ?? 0;
            convertedProduct.ApprovalDate = DateTime.Now;

            if (context.SaveChanges() > 0)
            {
                if (convertStatus.Equals("A"))
                {
                    //int noOfRowsAfftected = context.Database.ExecuteSqlCommand("exec spConvertProduct {0}", convertedProduct.ConvertedProductId);

                    var AccData = await GetConvertedProductById(companyId, convertedProduct.ConvertedProductId);
                    //ChangeFinishedItemCostingPrice(convertedProduct.ConvertedProductId, companyId, AccData.ConvertedDate);
                   await _accountingService.AccountingProductConvertPushFeed(companyId, AccData, (int)FeedJournalEnum.ProductConvertVoucher);
                    return true;
                }
            }
            return false;
        }

        private void ChangeFinishedItemCostingPrice(int convertedProductId, int companyId, DateTime date)
        {
            var convertedProducts = context.ConvertedProducts.Where(x => x.ConvertedProductId == convertedProductId).ToList();
            foreach (var item in convertedProducts)
            {
                var result = context.Database.SqlQuery<FeedFinishedProductStock>("Finished_Product_Wise_StockReport {0}, {1}, {2}", item.ConvertedDate, companyId, item.ConvertTo).FirstOrDefault();
                var product = context.Products.Find(result.ProductId);
                product.CostingPrice = result.ClosingRate;
                context.SaveChanges();
            }
        }


    }
}
