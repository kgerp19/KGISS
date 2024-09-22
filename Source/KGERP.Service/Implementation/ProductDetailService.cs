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
    public class ProductDetailService : IProductDetailService
    {
        private bool disposed = false;

        private readonly ERPEntities context;
        public ProductDetailService(ERPEntities context)
        {
            this.context = context;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public List<ProductDetailModel> GetProductDetail()
        {
            var product = context.ProductDetails.Include(x => x.Product).ToList();
            return ObjectConverter<ProductDetail, ProductDetailModel>.ConvertList(product).ToList();
        }

        public bool SaveOrEdit(List<ProductDetailModel> model)
        {
            var noOfItem = model.Count;
            var count = 0;
            var data = ObjectConverter<ProductDetailModel, ProductDetail>.ConvertList(model);
            foreach (var item in data)
            {
                long storeDetailId = context.StoreDetails.Where(x => x.Store.LcNo == item.LcNo && x.ProductId == item.ProductId).Select(x => x.StoreDetailId).FirstOrDefault();
                item.StoreDetailsId = storeDetailId;
                item.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                item.CreatedDate = DateTime.Now;
                context.ProductDetails.Add(item);
                context.SaveChanges();
                count++;
            }
            if (noOfItem == count)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public List<SelectModel> ProductDetail(int id)
        {

            return context.Products.Where(x => x.ProductId == id).ToList().Select(x => new SelectModel()
            {
                Text = x.ProductName,
                Value = x.ProductId
            }).ToList();
        }
        public List<IntSelectModel> ProductDetailByLcNo(string lcNo)
        {
            List<IntSelectModel> product = context.Database.SqlQuery<IntSelectModel>("sp_GetProductNameByLcNo {0}", lcNo).ToList();
            return product;

        }

        public ProductDetailModel GetProductDetailById(int id)
        {
            var product = context.ProductDetails.Include(x => x.Product).Where(x => x.ProductDetailsId == id).FirstOrDefault();
            product = product ?? new ProductDetail();
            return ObjectConverter<ProductDetail, ProductDetailModel>.Convert(product);
        }

        public bool Update(ProductDetailModel model)
        {
            var product = context.ProductDetails.Where(x => x.ProductDetailsId == model.ProductDetailsId).FirstOrDefault();
            if (product != null)
            {
                product.EngineNo = model.EngineNo;
                product.ChassissNO = model.ChassissNO;
                product.BetteryNo = model.BetteryNo;
                product.FuelPumpSlNo = model.FuelPumpSlNo;
                product.RearTyreLH = model.RearTyreLH;
                product.RearTyreRH = model.RearTyreRH;
                product.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                product.ModifiedDate = DateTime.Now;
            }
            return context.SaveChanges() > 0;
        }
    }
}
