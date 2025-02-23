using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Implementation.Accounting;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly ERPEntities context;
        private readonly WareHouseService _service;
        public ProductService(ERPEntities context, WareHouseService service)
        {
            this.context = context;
            _service = service;
        }
        public async Task<ProductPriceModel> GetProductTpPrice(int companyId, int productId, string priceType)
        {
            ProductPriceModel model = new ProductPriceModel();

            model = await Task.Run(() => (from t1 in context.Products
                                                   join t2 in context.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                                   join t3 in context.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                                   where t1.CompanyId == companyId && t1.ProductId == productId
                                                   select new ProductPriceModel
                                                   {
                                                       ProductName = t3.Name +" "+ t2.Name +" "+ t1.ProductName,
                                                       ProductCode =t1.ProductCode,
                                                       CompanyId= t1.CompanyId,
                                                       PriceType= priceType,
                                                       ProductId= t1.ProductId,

                                                   }
                                                  ).FirstOrDefaultAsync());
            model.DataList = await Task.Run(() => (from t1 in context.ProductPrices
                                                   where t1.CompanyId == companyId
                                                   && t1.ProductId == productId
                                                   && t1.PriceType == priceType
                                                   select new ProductPriceModel
                                                   {
                                                       ProductId= t1.ProductId,
                                                       PriceType= t1.PriceType,
                                                       TPPrice= t1.UnitPrice ??0,
                                                       PriceUpdatedDate= t1.PriceUpdatedDate
                                                   }
                                                   ).OrderByDescending(o => o.PriceUpdatedDate).AsEnumerable());

            return model;
        }
        public async Task<ProductPriceModel> GetProductSalePrice(int companyId, int productId, string priceType)
        {
            ProductPriceModel model = new ProductPriceModel();

            model = await Task.Run(() => (from t1 in context.Products
                                          join t2 in context.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                          join t3 in context.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                          where t1.CompanyId == companyId && t1.ProductId == productId
                                          select new ProductPriceModel
                                          {
                                              ProductName = t3.Name + " " + t2.Name + " " + t1.ProductName,
                                              ProductCode = t1.ProductCode,
                                              CompanyId = t1.CompanyId,
                                              PriceType = priceType,
                                              ProductId = t1.ProductId,

                                          }
                                                  ).FirstOrDefaultAsync());
            model.DataList = await Task.Run(() => (from t1 in context.ProductPrices
                                                   where t1.CompanyId == companyId
                                                   && t1.ProductId == productId
                                                   && t1.PriceType == priceType
                                                   select new ProductPriceModel
                                                   {
                                                       ProductId = t1.ProductId,
                                                       PriceType = t1.PriceType,
                                                       TPPrice = t1.UnitPrice ?? 0,
                                                       UnitPrice = t1.UnitPrice,
                                                       PriceUpdatedDate = t1.PriceUpdatedDate
                                                   }
                                                   ).OrderByDescending(o => o.PriceUpdatedDate).AsEnumerable());;

            return model;
        }
        public ProductModel GetProduct(int id, string productType)
        {
            string productCode = string.Empty;
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            if (id <= 0)
            {
                IQueryable<Product> products = context.Products.Include(x => x.ProductCategory).Where(x => x.ProductCategory.CompanyId == companyId && x.ProductCategory.ProductType.Equals(productType) && x.IsActive);
                int count = products.Count();
                if (count == 0)
                {
                    return new ProductModel()
                    {
                        CompanyId = companyId,
                        ProductType = productType,
                        ProductCode = GenerateProductCode(productType, "0"),
                        IsActive = true
                    };
                }

                products = products.Where(x => x.ProductCategory.CompanyId == companyId).OrderByDescending(x => x.ProductCode).Take(1);
                productCode = products.ToList().FirstOrDefault().ProductCode;
                productCode = GenerateProductCode(productType, productCode);
                return new ProductModel()
                {
                    CompanyId = companyId,
                    ProductType = productType,
                    ProductCode = productCode,
                    IsActive = true
                };

            }
            Product Product = context.Products.Find(id);
            return ObjectConverter<Product, ProductModel>.Convert(Product);
        }
        private string GenerateProductCode(string type, string lastProductCode)
        {
            string productCodeNumber = lastProductCode.Length < 4 ? "0" : lastProductCode.Substring(1, 4);
            int productNumber = Convert.ToInt32(productCodeNumber);
            ++productNumber;
            return type + productNumber.ToString().PadLeft(4, '0');
        }

        public bool EditProduct(int id, ProductModel model, out string message)
        {
            var result = -1;
            int noOfRowsAfftected = 0;
            message = string.Empty;
            if (model == null)
            {
                throw new Exception("Product data missing!");
            }

            bool exist = context.Products.Where(x => x.CompanyId == model.CompanyId && x.ProductCategoryId == model.ProductCategoryId && x.ProductSubCategoryId == model.ProductSubCategoryId && x.ProductName.ToLower().Equals(model.ProductName.ToLower()) && x.ProductId != id).Any();
            if (exist)
            {
                message = "This Product Already Exists !";
                return false;
            }
            Product product = ObjectConverter<ProductModel, Product>.Convert(model);
            if (id > 0)
            {
                product = context.Products.FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {
                    throw new Exception("Product Category not found!");
                }
                product.ModifiedDate = DateTime.Now;
                product.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                product.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                product.CreatedDate = DateTime.Now;
            }

            product.ProductType = model.ProductType;
            product.CompanyId = model.CompanyId;
            product.ProductCategoryId = model.ProductCategoryId;
            product.ProductCode = model.ProductCode;
            product.ProductSubCategoryId = model.ProductSubCategoryId;
            product.ProductName = model.ProductName;
            product.ShortName = model.ShortName;
            product.UnitId = model.UnitId;
            product.DieSize = model.DieSize;
            product.PackSize = model.PackSize;
            product.TPPrice = model.TPPrice != null ? model.TPPrice.Value : 0;
            product.UnitPrice = model.UnitPrice;
            product.Remarks = model.Remarks;
            product.OrderNo = model.OrderNo;
            product.PackId = model.PackId;
            product.IsActive = model.IsActive;
            product.ProcessLoss = model.ProcessLoss;
            context.Entry(product).State = product.ProductId == 0 ? EntityState.Added : EntityState.Modified;
            noOfRowsAfftected = context.SaveChanges();

            result = product.ProductId;

            return noOfRowsAfftected > 0;
        }

        public bool SaveProduct(int id, ProductModel model, out string message)
        {
            var result = -1;
            int noOfRowsAfftected = 0;
            message = string.Empty;
            if (model == null)
            {
                throw new Exception("Product data missing!");
            }

            bool exist = context.Products.Where(x => x.CompanyId == model.CompanyId && x.ProductCategoryId == model.ProductCategoryId && x.ProductSubCategoryId == model.ProductSubCategoryId && x.ProductName.ToLower().Equals(model.ProductName.ToLower()) && x.ProductId != id).Any();
            if (exist)
            {
                message = "This Product Already Exists !";
                return false;
            }
            Product product = ObjectConverter<ProductModel, Product>.Convert(model);
            if (id > 0)
            {
                product = context.Products.FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {
                    throw new Exception("Product Category not found!");
                }
                product.ModifiedDate = DateTime.Now;
                product.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                product.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                product.CreatedDate = DateTime.Now;
            }

            product.ProductType = model.ProductType;
            product.CompanyId = model.CompanyId;
            product.ProductCategoryId = model.ProductCategoryId;
            product.ProductCode = model.ProductCode;
            product.ProductSubCategoryId = model.ProductSubCategoryId;
            product.ProductName = model.ProductName;
            product.ShortName = model.ShortName;
            product.UnitId = model.UnitId;
            product.DieSize = model.DieSize;
            product.PackSize = model.PackSize;
            product.TPPrice = model.TPPrice != null ? model.TPPrice.Value : 0;
            product.UnitPrice = model.UnitPrice;
            product.Remarks = model.Remarks;
            product.OrderNo = model.OrderNo;
            product.PackId = model.PackId;
            product.IsActive = model.IsActive;
            product.ProcessLoss = model.ProcessLoss;
            context.Entry(product).State = product.ProductId == 0 ? EntityState.Added : EntityState.Modified;
            noOfRowsAfftected = context.SaveChanges();

           
                if (product.CompanyId == (int)CompanyName.KrishibidFeedLimited)
                {
                    result = product.ProductId;

                    Product product1 = context.Products.Find(product.ProductId);

                    VMHeadIntegration integration = new VMHeadIntegration
                    {
                        AccName = product1.ProductName,
                        LayerNo = 6,
                        Remarks = "6 Layer",
                        IsIncomeHead = false,
                        ProductType = product1.ProductType,
                        CompanyFK = product1.CompanyId,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        CreatedDate = DateTime.Now
                    };


                    int headGl = ProductHeadGlPush(integration, product);

                    //if (headGlId != null)
                    //{
                    //    await GLDLBlockCodeAndHeadGLIdEdit(commonProductSubCategory.ProductSubCategoryId, headGlId, head5Id);
                    //}
                }


                result = product.ProductId;
            


            if (model.ProductId == 0)
            {
                context.Database.ExecuteSqlCommand(@"exec sp_FeedInsertIntoVendorOfferIfNewProduct {0},{1},{2}", product.ProductId, product.CompanyId, product.CreatedBy);
            }
            return noOfRowsAfftected > 0;
        }

        private int ProductHeadGlPush(VMHeadIntegration vmModel, Product product)
        {
            int result = -1;
            ProductCategory productCategory = context.ProductCategories.Find(product.ProductCategoryId);
            //List<Head5> head5s = new List<Head5>();
            HeadGL headGl_1 = new HeadGL
            {
                Id = context.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(productCategory.AccountingHeadId.Value),

                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };


            context.HeadGLs.Add(headGl_1);
            var catagoryForAssets = context.Products.SingleOrDefault(x => x.ProductId == product.ProductId);
            catagoryForAssets.AccountingHeadId = headGl_1.Id;
            context.SaveChanges();

            HeadGL headGl_2 = new HeadGL
            {
                Id = context.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(productCategory.AccountingIncomeHeadId.Value), // 
                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingIncomeHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,

                OrderNo = 0,
                Remarks = "GL Layer"
            };
            context.HeadGLs.Add(headGl_2);
            var catagoryForIncome = context.Products.SingleOrDefault(x => x.ProductId == product.ProductId);
            catagoryForIncome.AccountingIncomeHeadId = headGl_2.Id;
            context.SaveChanges();


            HeadGL headGl_3 = new HeadGL
            {
                Id = context.Database.SqlQuery<int>("spGetNewId").FirstOrDefault(),
                AccCode = GenerateHeadGlAccCode(productCategory.AccountingExpenseHeadId.Value), // 
                AccName = vmModel.AccName,
                ParentId = productCategory.AccountingExpenseHeadId.Value,// Propertise Accounts Receivable Head4 Id
                CompanyId = vmModel.CompanyFK,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                LayerNo = 6,
                CreateDate = DateTime.Now,
                IsActive = true,
                OrderNo = 0,
                Remarks = "GL Layer"
            };


            context.HeadGLs.Add(headGl_3);
            var catagoryForExpanse = context.Products.SingleOrDefault(x => x.ProductId == product.ProductId);
            catagoryForExpanse.AccountingExpenseHeadId = headGl_3.Id;
            context.SaveChanges();


            return result;

        }




        private string GenerateHeadGlAccCode(int Head5Id)
        {
            var Head5 = context.Head5.Where(x => x.Id == Head5Id).FirstOrDefault();


            var HeadGlDataList = context.HeadGLs.Where(x => x.ParentId == Head5Id).AsEnumerable();

            string newAccountCode = "";
            if (HeadGlDataList.Any())
            {
                string lastAccCode = HeadGlDataList.OrderByDescending(x => x.AccCode).FirstOrDefault().AccCode;
                string parentPart = lastAccCode.Substring(0, 10);
                string childPart = lastAccCode.Substring(10, 3);
                newAccountCode = parentPart + (Convert.ToInt32(childPart) + 1).ToString().PadLeft(3, '0');

            }
            else
            {
                newAccountCode = Head5.AccCode + "001";

            }
            return newAccountCode;
        }

















        public bool DeleteProduct(int id)
        {
            Product product = context.Products.Find(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            List<ProductPrice> productPrices = context.ProductPrices.Where(x => x.ProductId == id).ToList();
            if (productPrices.Any())
            {
                foreach (var item in productPrices)
                {
                    context.ProductPrices.Remove(item);
                    context.SaveChanges();
                }
            }
            context.Products.Remove(product);
            return context.SaveChanges() > 0;
        }
        public async Task<ProductModel> GetProducts(int companyId, string type)
        {
            ProductModel productModel = new ProductModel();
            productModel.CompanyId = companyId;
            productModel.DataList = await Task.Run(() => (from t1 in context.Products
                                                          join t2 in context.ProductCategories on t1.ProductCategoryId equals t2.ProductCategoryId
                                                          join t3 in context.ProductSubCategories on t1.ProductSubCategoryId equals t3.ProductSubCategoryId
                                                          join t4 in context.Units on t1.UnitId equals t4.UnitId
                                                          join t5 in context.Products on t1.PackId equals t5.ProductId
                                                          into pack
                                                          from t5 in pack.DefaultIfEmpty()

                                                          where t1.CompanyId == companyId
                                                            && t1.ProductType == type
                                                              select new ProductModel
                                                              {
                                                                  ProductName = t1.ProductName,
                                                                  Remarks = t1.Remarks,
                                                                  CategoryName = t2.Name,
                                                                  SubCategoryName = t3.Name,
                                                                  UnitName= t4.Name,
                                                                  ProductCode= t1.ProductCode,
                                                                  UnitPrice= t1.UnitPrice,
                                                                  TPPrice= t1.TPPrice,
                                                                  DieSize= t1.DieSize,
                                                                  PackSize=t1.PackSize,
                                                                  ProcessLoss= t1.ProcessLoss,
                                                                  PackName= t5 != null ? t5.ProductName : "",
                                                                  PackId = t1.PackId,
                                                                  ProductId= t1.ProductId,
                                                                  ProductType= t1.ProductType
                                                              }).OrderBy(o => o.ProductName)
                                                                .AsEnumerable());

            return productModel;
        }
        public async Task<ProductPriceModel> GetLastUpdatedProductTpPrice(int companyId, string priceType)
        {
            ProductPriceModel productModel = new ProductPriceModel();
            productModel.CompanyId = companyId;

            productModel.DataList = await Task.Run(() => (from t1 in context.Products
                                                          join t2 in context.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId 
                                                          join t3 in context.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId 
                                                          where t1.CompanyId == companyId
                                                          && t1.ProductType == "F"
                                                          select new ProductPriceModel
                                                          {
                                                              ProductName = t3.Name + " " + t2.Name + " " + t1.ProductName,
                                                              ProductId = t1.ProductId,
                                                              CompanyId = t1.CompanyId,
                                                              TPPrice = t1.TPPrice,
                                                              ProductCode = t1.ProductCode,
                                                              PriceUpdatedDate = ( context.ProductPrices.Where(x => x.ProductId == t1.ProductId && x.PriceType == priceType).OrderByDescending(x => x.Id).FirstOrDefault().PriceUpdatedDate),
                                                              PriceType = priceType
                                                          }).OrderBy(o => o.ProductName).AsEnumerable());
           
            return productModel;
        }
        public async Task<ProductPriceModel> GetLastUpdatedProductSalePrice(int companyId, string priceType)
        {
            ProductPriceModel productModel = new ProductPriceModel();
            productModel.CompanyId = companyId;

            productModel.DataList = await Task.Run(() => (from t1 in context.Products
                                                          join t2 in context.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                                                          join t3 in context.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                                                          where t1.CompanyId == companyId && t1.IsActive && t2.IsActive && t3.IsActive
                                                          && t1.ProductType == "F"
                                                          select new ProductPriceModel
                                                          {
                                                              ProductName = t3.Name + " " + t2.Name + " " + t1.ProductName,
                                                              ProductId = t1.ProductId,
                                                              CompanyId = t1.CompanyId,
                                                              UnitPrice = t1.UnitPrice ?? 0,
                                                              ProductCode = t1.ProductCode,
                                                              PriceUpdatedDate = (context.ProductPrices.Where(x => x.ProductId == t1.ProductId && x.PriceType==priceType).OrderByDescending(x => x.Id).FirstOrDefault().PriceUpdatedDate),
                                                              PriceType = priceType,
                                                              SaleCommissionRate= (context.ProductPrices.Where(x => x.ProductId == t1.ProductId && x.PriceType== priceType).OrderByDescending(x => x.Id).FirstOrDefault().SaleCommissionRate),

                                                          }).OrderBy(o => o.ProductName).AsEnumerable());
            return productModel;
        }

        public List<ProductModel> GetProducts(int companyId, string type, string searchText)
        {
            IQueryable<Product> queryable = context.Products
                .Include(x => x.ProductCategory)
                .Include(x => x.ProductSubCategory).Include(x => x.Unit)
                .Where(x => x.ProductCategory.CompanyId == companyId && x.IsActive && x.ProductCategory.ProductType.Equals(type) && (x.ProductCategory.Name.Contains(searchText) || x.ProductSubCategory.Name.Contains(searchText) || x.ProductName.Contains(searchText))).OrderBy(x => x.ProductCategoryId);
            return ObjectConverter<Product, ProductModel>.ConvertList(queryable.ToList()).ToList();
        }

        public List<SelectModel> GetProductSelectModelsByProductSubCategory(int productSubCategoryId)
        {
            return context.Products.ToList().Where(x => x.ProductSubCategoryId == productSubCategoryId).Select(x => new SelectModel()
            {
                Text = x.ProductName,
                Value = x.ProductId
            }).OrderBy(x => x.Text).ToList();
        }

        public object GetProductAutoComplete(string prefix, int companyId, string productType)
        {
            return (from t1 in context.Products
                     join t2 in context.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                     join t3 in context.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId
                     where t3.CompanyId == companyId && t1.IsActive && t3.ProductType == productType
                     && (t1.ProductName.ToLower().StartsWith(prefix) || t2.Name.ToLower().StartsWith(prefix) || t3.Name.ToLower().StartsWith(prefix))
                     select new 
                     {
                         label =t3.Name +" - " + t2.Name +" - "+ t1.ProductName ,
                         val = t1.ProductId,
                         unitprice= t1.UnitPrice,
                         TPPrice = t1.TPPrice,
                         CostingPrice = t1.CostingPrice

                     }).Take(100).ToList();
            //return context.Products.Include(x => x.ProductSubCategory.ProductCategory).Where(x => x.ProductSubCategory.ProductCategory.CompanyId == companyId && x.IsActive && x.ProductSubCategory.ProductCategory.ProductType == productType && (x.ProductName.ToLower().StartsWith(prefix))).Select(x => new
            //{
            //    label = x.ProductName,
            //    val = x.ProductId
            //}).Take(20).ToList();
        }

        //public List<ProductPriceModel> GetLastUpdatedProductPrice(int companyId, string priceType)
        //{
        //    List<ProductPriceModel> model = context.Database.SqlQuery<ProductPriceModel>("exec sp_lastUpdatedProductPrice {0},{1}", companyId, priceType).ToList();
        //    return model;
        //}

      
        public bool SaveProductPrice(ProductPriceModel model)
        {
            Product product = context.Products.Where(x => x.ProductId == model.ProductId).FirstOrDefault();
            if (product != null && model.PriceType.Equals("Purchase"))
            {
                product.PurchaseRate = model.PurchaseRate;
                product.PurchaseCommissionRate = model.PurchaseCommissionRate;
                product.SaleCommissionRate = model.SaleCommissionRate;
                context.SaveChanges();
            }
            if (product != null && model.PriceType.Equals("Sale"))
            {
                product.UnitPrice = model.UnitPrice;
                product.CreditSalePrice = model.CreditSellPrice;
                product.SaleCommissionRate = model.SaleCommissionRate;
                context.SaveChanges();
            }
            ProductPrice prPrice = ObjectConverter<ProductPriceModel, ProductPrice>.Convert(model);
            model.CreatedDate = DateTime.Now;
            model.CreatedBy = HttpContext.Current.User.Identity.Name;
            context.ProductPrices.Add(prPrice);
            return context.SaveChanges() > 0;
        }

        public bool SaveConvertedProduct(ConvertedProductModel model)
        {
            model.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            model.CreatedDate = DateTime.Now;
            var convert = ObjectConverter<ConvertedProductModel, ConvertedProduct>.Convert(model);
            ProductStore product = new ProductStore();
            //product.ConvertedQty = model.ConvertedQty;
            product.ProductId = model.ConvertFrom;
            product.TransactionDate = model.ConvertedDate;
            context.ProductStores.Add(product);
            context.ConvertedProducts.Add(convert);
            return context.SaveChanges() > 0;

        }

        public decimal GetBaseCommissionRate(int? productId)
        {
            return context.Database.SqlQuery<decimal>(@"select CashCustomerRate from  Erp.ProductCategory where ProductCategoryId = (select ProductCategoryId from Erp.Product where ProductId ={0})", productId).FirstOrDefault();
        }

        public List<ProductModel> GetFinishProducts(int companyId, string searchText)
        {
            IQueryable<Product> queryable = context.Products.Include(x => x.ProductCategory).Include(x => x.ProductSubCategory).Where(x => x.ProductCategory.CompanyId == companyId && x.ProductCategory.ProductType.Equals("F") && (x.ProductCategory.Name.Contains(searchText) || x.ProductSubCategory.Name.Contains(searchText) || x.ProductName.Contains(searchText))).OrderBy(x => x.ProductCategory.Name).ThenBy(x => x.ProductSubCategory.Name).ThenBy(x => x.ProductName);
            return ObjectConverter<Product, ProductModel>.ConvertList(queryable.ToList()).ToList();
        }

        public object GetProductAutoComplete(string prefix, int companyId)
        {
            return context.Products.Include(x => x.ProductSubCategory.ProductCategory).Where(x => x.ProductSubCategory.ProductCategory.CompanyId == companyId && x.ProductSubCategory.ProductCategory.ProductType == "R" && (x.ProductName.StartsWith(prefix))).Select(x => new
            {
                label = x.ProductName,
                val = x.ProductId
            }).ToList();
        }

        public ProductModel GetProductInformation(int productId)
        {
            if (productId == 0)
            {
                return new ProductModel();
            }
            Product Product = context.Products.Include(x => x.Unit).Where(x => x.ProductId == productId).FirstOrDefault();
            return ObjectConverter<Product, ProductModel>.Convert(Product);
        }

        public BusinessCostCustomModel GetCustomerBusinessCost(int customerId, int productId, int stockInfoId)
        {
            return context.Database.SqlQuery<BusinessCostCustomModel>("exec spGetCustomerBusinessCost {0},{1},{2}", customerId, productId, stockInfoId).FirstOrDefault();
        }



        public List<SelectModel> GetRawMterialsSelectModel(int companyId)
        {
            List<Product> products = context.Products.Include(x => x.ProductCategory).Where(x => x.ProductCategory.CompanyId == companyId && x.ProductCategory.ProductType == "R").OrderBy(x => x.ProductName).ToList();

            return products.Select(x => new SelectModel { Text = x.ProductName, Value = x.ProductId }).ToList();
        }

        public object GetFinishProductAutoComplete(string prefix, int companyId)
        {
            return context.Products.Include(x => x.ProductSubCategory.ProductCategory).Where(x => x.ProductSubCategory.ProductCategory.CompanyId == companyId && x.ProductSubCategory.ProductCategory.ProductType == "F" && (x.ProductName.StartsWith(prefix))).Select(x => new
            {
                label = x.ProductName,
                val = x.ProductId
            }).ToList();
        }

        public object GetProductAutoCompleteCatagoryWise(string prefix, int companyId, string productType)
        {
            return context.Products.Include(x => x.ProductSubCategory.ProductCategory).Where(x => x.ProductCategory.ProductCategoryId == 6 && x.ProductSubCategory.ProductCategory.CompanyId == companyId && x.ProductSubCategory.ProductCategory.ProductType == productType && (x.ProductName.StartsWith(prefix))).Select(x => new
            {
                label = x.ProductName,
                val = x.ProductId
            }).Take(20).ToList();
        }


        public string ProductPackName(int id)
        {
            Product product = context.Products.Where(x => x.ProductId == id).FirstOrDefault();
            var packId = product.PackId;
            var packName = context.Products.Where(x => x.ProductId == packId).Select(x => x.ProductName).FirstOrDefault();
            return packName;
        }

        public List<Product> GetProductInfo()
        {
            string query = @"select top(10) * from Erp.Product";
            return context.Database.SqlQuery<Product>(query).ToList();

        }

        public ProductModel GetProductUnitPrice(int pId)
        {
            var custInfo = context.Products.FirstOrDefault(x => x.ProductId == pId);
            return ObjectConverter<Product, ProductModel>.Convert(custInfo);
        }

        public decimal GetStockckAvailableQuantity(int productId, int stockFrom)
        {
            if (stockFrom == 0)
            {
                int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
                int factory = context.StockInfoes.Where(x => x.CompanyId == companyId && x.StockType.Equals("F")).FirstOrDefault().StockInfoId;
                return context.Database.SqlQuery<decimal>(@"select cast( isnull(sum(isnull(InQty,0))-sum(isnull(OutQty,0)),0) as decimal) as StockckAvailableQuantity from Erp.ProductStore where ProductId={0} and StockInfoId={1}", productId, factory).FirstOrDefault();
            }
            else
            {
                return context.Database.SqlQuery<decimal>(@"select cast( isnull(sum(isnull(InQty,0))-sum(isnull(OutQty,0)),0) as decimal) as StockckAvailableQuantity from Erp.ProductStore where ProductId={0} and StockInfoId={1}", productId, stockFrom).FirstOrDefault();
            }
        }

        public bool SaveProductList(List<Product> newProductList)
        {
            context.Products.AddRange(newProductList);
            return context.SaveChanges() > 0;
        }

        public decimal GetRawMaterialStockUnitPrice(int id, DateTime date, int companyId)
        {
            return context.Database.SqlQuery<decimal>(@"SELECT dbo.fn_GetProductCumulativePrice({0},{1})", id, date).FirstOrDefault();
        }

        public decimal GetProductPoressLoss(int productId)
        {
            return context.Products.Where(x => x.ProductId == productId).First().ProcessLoss;
        }

        public List<SelectModel> GetRawmaterialByDemand(int companyId,int demandId)
        {
            //int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            IQueryable<Product> rawMaterials = context.Products.Where(x => x.CompanyId == companyId && x.ProductType.Equals("R")).OrderBy(x => x.ProductCategoryId).ThenBy(x => x.ProductName);
            return rawMaterials.ToList().Select(x => new SelectModel { Text = x.ProductName, Value = x.ProductId }).ToList();
        }

        public List<SelectModel> GetProductbyProductSubCategorySelectModels(int productSubCategoryId)
        {
            IQueryable<Product> products = context.Products.Where(x => x.ProductSubCategoryId == productSubCategoryId);
            return products.Select(x => new SelectModel { Text = x.ProductName, Value = x.ProductId }).ToList();
        }

        public decimal GetProductCogsPrice(int? companyId, int? productId)
        {
            decimal cogsPrice = context.Database.SqlQuery<decimal>(@"select ISNULL((Sum(UnitPrice * InQty)/ NULLIF(sum(InQty),0)),0) 
                                                                     from Erp.ProductStore where CompanyId={0} and ProductId={1}", companyId, productId).FirstOrDefault();
            return cogsPrice;
        }

        public List<SelectModel> GetProductSelectModelsByCompanyAndProductType(int companyId, string productType)
        {

            IQueryable<Product> products = context.Products.Where(x => x.CompanyId == companyId && x.ProductType.ToLower().Equals(productType.ToLower())).OrderBy(x => x.ProductName);
            return products.ToList().Select(x => new SelectModel { Text = x.ProductName, Value = x.ProductId }).ToList();
        }

        public decimal GetProductPrice(int productId)
        {
            Product product = context.Products.Where(x => x.ProductId == productId).FirstOrDefault();
            if (product == null)
            {
                return 0;
            }
            return product.UnitPrice ?? 0;
        }

        public async Task<int> SaveProductTpPrice(ProductPriceModel model)
        {
            long previousPriceId = 0;
            var productPrice = await context.ProductPrices
                .SingleOrDefaultAsync(s => s.UnitPrice == model.TPPrice
                && s.PriceUpdatedDate == model.PriceUpdatedDate
                && s.CompanyId == model.CompanyId
                && s.ProductId == model.ProductId
                && s.PriceType == model.PriceType);

            var previousProductPrice = context.ProductPrices
                .Where(s => s.ProductId == model.ProductId
                && s.PriceType == model.PriceType)
                .OrderByDescending(o => o.Id)
                .ToList().Take(1);
            if (previousProductPrice.Count() > 0)
            {
                previousPriceId = previousProductPrice.FirstOrDefault().Id;
            }
            else
            {
                previousPriceId = 0;
            }

            if (productPrice== null)
            {
                productPrice = new ProductPrice();
                productPrice.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                productPrice.CreatedDate = DateTime.Now;
                productPrice.CompanyId = model.CompanyId;
                productPrice.PriceType = model.PriceType;
                productPrice.ProductId = model.ProductId;
                productPrice.PriceUpdatedDate = model.PriceUpdatedDate;

                productPrice.UnitPrice = model.TPPrice;
                productPrice.SaleCommissionRate = model.SaleCommissionRate;
                productPrice.PurchaseRate = model.PurchaseRate;
                productPrice.PurchaseCommissionRate = model.PurchaseCommissionRate;
                productPrice.CreditSellPrice = model.CreditSellPrice;

                context.ProductPrices.Add(productPrice);

                var product = context.Products.SingleOrDefault(s => s.ProductId == model.ProductId && s.CompanyId == model.CompanyId);
                product.TPPrice = model.TPPrice;



            }
            try
            {
                 await context.SaveChangesAsync();
                //if (previousPriceId != 0) {
                //    await _service.FeedProductTpPriceSubmit(productPrice.Id,previousPriceId);
                //}
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return productPrice.ProductId ?? 0;
        }
        public async Task<int> SaveProductSalePrice(ProductPriceModel model)
        {
            if(model.PriceUpdatedDate==null || model.UnitPrice <=0)
            {
                return model.ProductId??0;
            }
            var productPrice = await context.ProductPrices
                .SingleOrDefaultAsync(s => s.UnitPrice == model.TPPrice
                && s.PriceUpdatedDate == model.PriceUpdatedDate
                && s.CompanyId == model.CompanyId
                && s.ProductId == model.ProductId
                && s.PriceType == model.PriceType);

            if (productPrice == null)
            {
                productPrice = new ProductPrice();
                productPrice.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                productPrice.CreatedDate = DateTime.Now;
                productPrice.CompanyId = model.CompanyId;
                productPrice.PriceType = model.PriceType;
                productPrice.ProductId = model.ProductId;
                productPrice.PriceUpdatedDate = model.PriceUpdatedDate;

                productPrice.UnitPrice = model.UnitPrice;
                productPrice.SaleCommissionRate = model.SaleCommissionRate;
                productPrice.PurchaseRate = model.PurchaseRate;
                productPrice.PurchaseCommissionRate = model.PurchaseCommissionRate;
                productPrice.CreditSellPrice = model.CreditSellPrice;
                context.ProductPrices.Add(productPrice);

                var product = context.Products.SingleOrDefault(s => s.ProductId == model.ProductId && s.CompanyId == model.CompanyId);
                product.UnitPrice = model.UnitPrice;
            }
            try
            {
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return productPrice.ProductId ?? 0;
        }

        public List<SelectModel> GetProductTypeSelectModels()
        {
            return new List<SelectModel> {
                new SelectModel { Text = "Finish Goods", Value = "F" },
                new SelectModel { Text = "Raw Materials", Value = "R" }
            };
        }

        public List<SelectListItem> GetDDLStockInfoDepartment(int? companyId)
        {
            var query = context.StockInfoes.Where(x => x.CompanyId == companyId && x.StockType == "D").Select(x=> new SelectListItem
            {
                Text=x.Name,
                Value=x.StockInfoId.ToString()
            });
            return  query.ToList();
        }


    }
}
