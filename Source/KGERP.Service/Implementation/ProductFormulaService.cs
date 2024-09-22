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
    public class ProductFormulaService : IProductFormulaService
    {
        private readonly ERPEntities context;
        public ProductFormulaService(ERPEntities context)
        {
            this.context = context;
        }


        public ProductFormulaModel GetProductFormula(int id)
        {

            ProductFormula productFormula = context.ProductFormulas.Include(x => x.Product).Where(x => x.ProductFormulaId == id).FirstOrDefault();
            if (productFormula == null)
            {
                return new ProductFormulaModel() { FQty = 1000, IsActive = true };
            }
            return ObjectConverter<ProductFormula, ProductFormulaModel>.Convert(productFormula);
        }

        public ProductFormulaModel GetRawMaterial(int id, int productId)
        {
            if (id == 0)
            {
                return new ProductFormulaModel { FProductId = productId };
            }
            ProductFormula productFormula = context.ProductFormulas.Find(id);
            return ObjectConverter<ProductFormula, ProductFormulaModel>.Convert(productFormula);
        }

        public List<ProductFormulaModel> GetRawMaterials(int productId)
        {
            IQueryable<ProductFormula> queryable = context.ProductFormulas.Include("Product").Include("RawMaterial").Where(x => x.FProductId == productId).OrderBy(x => x.Product.ProductName);
            return ObjectConverter<ProductFormula, ProductFormulaModel>.ConvertList(queryable.ToList()).ToList();
        }

        public bool SaveProductFormula(int id, ProductFormulaModel model, out string message)
        {
            message = string.Empty;
            int noOfRowsAffected = 0;
            if (model == null)
            {
                throw new Exception("Product Formula data missing!");
            }

            ProductFormula productFormula = ObjectConverter<ProductFormulaModel, ProductFormula>.Convert(model);

            bool isFormulaAlreadyExist = context.ProductFormulas.Where(x => x.FProductId == model.FProductId && x.ProductFormulaId != model.ProductFormulaId).Any();

            if (isFormulaAlreadyExist)
            {
                message = "The formula for this product already exist";
                return false;
            }
            if (id > 0)
            {
                productFormula = context.ProductFormulas.FirstOrDefault(x => x.ProductFormulaId == id);
                if (productFormula == null)
                {
                    throw new Exception("Data not found!");
                }
                productFormula.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                productFormula.ModifiedDate = DateTime.Now;
            }
            else
            {
                productFormula.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                productFormula.CreatedDate = DateTime.Now;
                productFormula.IsActive = true;
            }


            productFormula.FProductId = model.FProductId;
            productFormula.FQty = model.FQty;
            productFormula.FormulaDate = model.FormulaDate;
            productFormula.Product = null;
            context.Entry(productFormula).State = productFormula.ProductFormulaId == 0 ? EntityState.Added : EntityState.Modified;
            noOfRowsAffected = context.SaveChanges();
            if (noOfRowsAffected > 0)
            {
                message = "Formula for this product saved successfully";
            }
            return noOfRowsAffected > 0;
        }

        public bool DeleteProductFormula(int id)
        {
            ProductFormula productFormula = context.ProductFormulas.Find(id);
            if (productFormula == null)
            {
                throw new Exception("Formula not found");
            }
            context.ProductFormulas.Remove(productFormula);
            return context.SaveChanges() > 0;
        }
       
        public async Task<ProductFormulaModel> GetProductFormulas(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            ProductFormulaModel productFormulaModel = new ProductFormulaModel();
            productFormulaModel.CompanyId = companyId;
            productFormulaModel.DataList = await Task.Run(() => (from t1 in context.ProductFormulas
                                                                join t2 in context.Products on t1.FProductId equals t2.ProductId
                                                                
                                                                where t1.CompanyId == companyId
                                                              
                                                                && t1.IsActive == true
                                                                select new ProductFormulaModel
                                                                {
                                                                    ProductName = t1.Product.ProductName,
                                                                    FormulaDate=t1.FormulaDate,
                                                                    FQty= t1.FQty,
                                                                    IsActive=t1.IsActive,
                                                                    ProductFormulaId=t1.ProductFormulaId
                                                                }).OrderByDescending(o => o.FormulaDate).AsEnumerable());

            return productFormulaModel;      
        }

        public bool SavePFormulaDetail(int id, PFormulaDetailModel model)
        {
            PFormulaDetail pFormulaDetail = ObjectConverter<PFormulaDetailModel, PFormulaDetail>.Convert(model);
            if (id > 0)
            {
                pFormulaDetail = context.PFormulaDetails.FirstOrDefault(x => x.PFormulaDetailId == id);
                if (pFormulaDetail == null)
                {
                    throw new Exception("Data not found!");
                }
                pFormulaDetail.ModifiedDate = DateTime.Now;
                pFormulaDetail.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                pFormulaDetail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                pFormulaDetail.CreatedDate = DateTime.Now;
            }

            pFormulaDetail.ProductFormulaId = model.ProductFormulaId;
            pFormulaDetail.RProductId = model.RProductId;
            pFormulaDetail.RQty = model.RQty;
            pFormulaDetail.RProcessLoss = model.RProcessLoss;
            pFormulaDetail.Remarks = model.Remarks;

            context.Entry(pFormulaDetail).State = pFormulaDetail.PFormulaDetailId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public PFormulaDetailModel GetPFormulaDetail(int pFormulaDetailId)
        {
            if (pFormulaDetailId == 0)
            {
                return new PFormulaDetailModel();
            }
            PFormulaDetail pFormulaDetail = context.PFormulaDetails.Find(pFormulaDetailId);
            return ObjectConverter<PFormulaDetail, PFormulaDetailModel>.Convert(pFormulaDetail);
        }

        public bool DeletePFormulaDetail(int pFormulaDetailId)
        {
            PFormulaDetail formulaDetail = context.PFormulaDetails.Find(pFormulaDetailId);
            if (formulaDetail == null)
            {
                throw new Exception("Data not found");
            }
            context.PFormulaDetails.Remove(formulaDetail);
            return context.SaveChanges() > 0;
        }

        public ProductFormulaModel GetProductFormulaUsingProductId(int productId)
        {
            Product product = context.Products.Where(x => x.ProductId == productId).FirstOrDefault();
            ProductModel productModel = ObjectConverter<Product, ProductModel>.Convert(product);

            return new ProductFormulaModel()
            {
                ProductFormulaId = 0,
                FProductId = product.ProductId,
                Product = productModel,
                FQty = 1000,
                FormulaDate = DateTime.Today,
                IsActive = true
            };

        }
    }
}
