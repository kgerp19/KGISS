using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.SupplierProducts
{
    public class SupplierProductService
    {
        private readonly ERPEntities context;
        public SupplierProductService(ERPEntities context)
        {
            this.context = context;
        }

        public async Task<SupplierProductViewModel> SaveSupplierProduct(SupplierProductViewModel model)
        {
            try
            {
                SupplierProduct product = new SupplierProduct();
                product.ProductId = model.ProductId;
                product.VendorId = model.VendorId;
                product.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                product.CreatedDate = DateTime.Now;
                product.IsActive = true;
                context.SupplierProduct.Add(product);
                var res = await context.SaveChangesAsync();
                if (res != 0)
                { model.Id = product.Id; return model;}
                return model;
            }
            catch (Exception)
            { return model;}
        }

        public async Task<SupplierProductViewModel> UpdateSupplierProduct(SupplierProductViewModel model)
        {
            try
            {
                SupplierProduct product = await context.SupplierProduct.FirstOrDefaultAsync(f => f.Id == model.Id);
                product.ProductId = model.ProductId;
                product.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                product.ModifiedDate = DateTime.Now;
                context.SupplierProduct.Add(product);
                if (await context.SaveChangesAsync() > 0)
                { return model;}
                model.VendorId = 0;
                return model;
            }
            catch (Exception)
            { return model;}
        }

        public async Task<SupplierProductViewModel> DeleteSupplierProduct(SupplierProductViewModel model)
        {
            try
            {
                var product = await context.SupplierProduct.FirstOrDefaultAsync(f => f.Id == model.Id);
                product.IsActive = false;
                product.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                product.ModifiedDate = DateTime.Now;
                if (await context.SaveChangesAsync() > 0)
                { return model;  }
                return model;
            }
            catch (Exception)
            { return model; }

        }


        public async Task<SupplierProductViewModel> SupplierWaisProduct(int supplier)
        {
            SupplierProductViewModel model = new SupplierProductViewModel();
            model = await Task.Run(() => (from t1 in context.Vendors.Where(x => x.VendorId == supplier)
                                              //join zon in context.Zones on t1.ZoneId equals zon.ZoneId
                                          select new SupplierProductViewModel
                                          {

                                              VendorId = t1.VendorId,
                                              VendorName = t1.Name,
                                              Phone = t1.Phone,
                                              Code = t1.Code,
                                              Address = t1.Address,

                                              //Zone=zon.Name
                                          }).FirstOrDefault());

            model.DataList = await Task.Run(() => (from t1 in context.SupplierProduct.Where(x => x.IsActive == true && x.VendorId == supplier)
                                                   join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                                   join t3 in context.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                   join t4 in context.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                   join t5 in context.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId

                                                   select new SupplierProductViewModel
                                                   {
                                                       Id = t1.Id,
                                                       VendorId = t1.VendorId,
                                                       ProductId = t1.ProductId,
                                                       ProductName = t5.Name + "-" + t4.Name + "-" + t3.ProductName,
                                                       VendorName = t2.Name,
                                                       CreatedBy = t1.CreatedBy,
                                                       CreatedDate = t1.CreatedDate
                                                   }).OrderByDescending(x => x.Id).AsEnumerable());

                            return model;
                        }

        public async Task<SupplierProductViewModel> SupplierProductList()
        {
            SupplierProductViewModel model = new SupplierProductViewModel();
            model.DataList = await Task.Run(() => (from t1 in context.SupplierProduct.Where(x => x.IsActive)
                                                   join t2 in context.Vendors on t1.VendorId equals t2.VendorId
                                                   join t3 in context.Products.Where(x => x.IsActive) on t1.ProductId equals t3.ProductId
                                                   join t4 in context.ProductSubCategories.Where(x => x.IsActive) on t3.ProductSubCategoryId equals t4.ProductSubCategoryId
                                                   join t5 in context.ProductCategories.Where(x => x.IsActive) on t4.ProductCategoryId equals t5.ProductCategoryId
                                                   join t6 in context.Units.Where(x => x.IsActive) on t3.UnitId equals t6.UnitId
                                                   group new { t1, t2, t3 } by new { t1.VendorId, t2.Phone, t2.Name, t2.Address } into gruopAC
                                                   let countProduct = gruopAC.Count()
                                                   orderby countProduct
                                                   select new SupplierProductViewModel
                                                   {
                                                       VendorId = gruopAC.Key.VendorId,
                                                       VendorName = gruopAC.Key.Name,
                                                       Address = gruopAC.Key.Address,
                                                       Phone = gruopAC.Key.Phone,
                                                       Count = countProduct

                                                   }).AsEnumerable());
                               return model;

                    }



    }
}
