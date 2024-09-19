using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class VoucherTypeService : IVoucherTypeService
    {
        private readonly ERPEntities context;
        public VoucherTypeService(ERPEntities context)
        {
            this.context = context;
        }
        public List<VoucherTypeModel> GetVoucherTypes()
        {
            IQueryable<VoucherType> vendors = context.VoucherTypes.AsQueryable();
            return ObjectConverter<VoucherType, VoucherTypeModel>.ConvertList(vendors.ToList()).ToList();

        }

        public List<SelectModel> GetVoucherTypeSelectModels(int companyId = 0)
        {
            if (companyId == (int)CompanyName.GloriousCropCareLimited || companyId == (int)CompanyName.KrishibidSeedLimited)
            {
                return context.VoucherTypes.Where(x => x.CompanyId == companyId).ToList().Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.VoucherTypeId
                }).ToList();
            }
            else if (companyId > 0)
            {
                return context.VoucherTypes.Where(x => x.CompanyId == companyId).ToList().Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.VoucherTypeId
                }).ToList();
            }
            else
            {
                return context.VoucherTypes.Where(x => x.CompanyId == null).ToList().Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.VoucherTypeId
                }).ToList();
            }

        }

        public List<SelectModel> GetAccountingCostCenter(int companyId)
        {
            List<SelectModel> selectModelLiat = new List<SelectModel>();
            SelectModel selectModel = new SelectModel
            {
                Text = "== Select Cost Center ==",
                Value = 0,
            };
            selectModelLiat.Add(selectModel);

            var v = context.Accounting_CostCenter.Where(x => x.CompanyId == companyId).ToList()
               .Select(x => new SelectModel()
                {
                   Text = x.Name,
                   Value = x.CostCenterId
               }).ToList();
            selectModelLiat.AddRange(v);

            //if (companyId == 7 || companyId == 9)
            //{
            //    var v = context.KGREProjects.Where(x => x.CompanyId == companyId && x.IsAccounting==1).ToList()
            //    .Select(x => new SelectModel()
            //    {
            //        Text = x.ProjectName,
            //        Value = x.ProjectId
            //    }).ToList();
            //    selectModelLiat.AddRange(v);
            //}
            //else
            //{

            //}

            return selectModelLiat;
        }

        public List<SelectModel> GetProductCategory(int companyId)
        {
            List<SelectModel> selectModelLiat = new List<SelectModel>();
            SelectModel selectModel = new SelectModel
            {
                Text = "==Select Product Category==",
                Value = 0,
            };
            selectModelLiat.Add(selectModel);

            var v = context.ProductCategories.Where(x => x.CompanyId == companyId).ToList()
                .Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.ProductCategoryId
                }).ToList();

            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }


        public List<SelectModel> GetProductCategoryKPL(int companyId)
        {
            List<SelectModel> selectModelLiat = new List<SelectModel>();
            SelectModel selectModel = new SelectModel
            {
                Text = "==Select Product Category==",
                Value = 0,
            };
            selectModelLiat.Add(selectModel);

            var v = context.ProductCategories.Where(x => x.CompanyId == companyId&&x.IsActive==true).ToList()
                .Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.ProductCategoryId
                }).ToList();

            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }


        public List<SelectModel> GetProductCategoryGldl(int companyId)
        {
            List<SelectModel> selectModelLiat = new List<SelectModel>();
            var v = context.ProductCategories.Where(x => x.CompanyId == companyId&&x.IsActive==true).ToList()
                .Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.ProductCategoryId
                }).ToList();

            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }
        public List<SelectModel> GetProductSeaPalaceGldl(int companyId)
        {
            List<SelectModel> selectModelLiat = new List<SelectModel>();
            var v = context.ProductCategories.Where(x=> x.IsActive == true && x.ProductCategoryId == 124).ToList()
                .Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.ProductCategoryId
                }).ToList();

            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }
        public List<SelectModel> GetProductSubCategorySeaPalaceGldl(int? categoryId)
        {
            List<SelectModel> selectModelLiat = new List<SelectModel>();
            var v = context.ProductSubCategories.Where(x => x.IsActive == true && x.ProductCategoryId == categoryId).ToList()
                .Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.ProductSubCategoryId
                }).ToList();

            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }
        public List<SelectModel> GetShareSeaPalaceGldl(int? subcategoryId)
        {
            List<SelectModel> selectModelLiat = new List<SelectModel>();
            var v = context.Products.Where(x => x.IsActive == true && x.ProductSubCategoryId == subcategoryId).ToList()
                .Select(x => new SelectModel()
                {
                    Text = x.ProductName,
                    Value = x.ProductId
                }).ToList();

            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }


        public List<SelectModel> GetProductCategoryGcclAndKFmal(int companyId)
        {
            List<SelectModel> selectModelLiat = new List<SelectModel>();
            var v = context.ProductCategories.Where(x => x.CompanyId == companyId &&x.ProductCategoryId==28&& x.IsActive == true).ToList()
                .Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.ProductCategoryId
                }).ToList();

            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }

        public List<SelectModel> EnumerableYearRange()
        {
           
            List<SelectModel> selectModelLiat = new List<SelectModel>();
            var v = Enumerable.Range(2018, 100).ToList()
                .Select(x => new SelectModel()
                {
                    Text = x,
                    Value = x
                }).ToList();

            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }
    }
}
