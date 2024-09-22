using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class StockInfoService : IStockInfoService
    {
        private bool disposed = false;


        private readonly ERPEntities context;
        public StockInfoService(ERPEntities context)
        {
            this.context = context;
        }
        public async Task<StockInfoModel> GetStockInfos(int companyId)
        {
            StockInfoModel model = new StockInfoModel();
            model.CompanyId= companyId;
            model.DataList = await Task.Run(() => (from t1 in context.StockInfoes
                                                   join t2 in context.Employees on t1.DepotInchargeEmpId equals t2.Id
                                                   into x
                                                   from t2 in x.DefaultIfEmpty()
                                                   where t1.IsActive && t1.CompanyId == companyId
                                                   select new StockInfoModel
                                                   {
                                                       EmployeeId = t2.EmployeeId,
                                                       Name = t1.Name,
                                                       Code=t1.Code,
                                                       StockInfoId = t1.StockInfoId,
                                                       CompanyId = t1.CompanyId,
                                                       StockType = t1.StockType,
                                                       DepotInchargeName=t2 !=null? t2.Name:"",
                                                       DepotInchargeEmpId = t1.DepotInchargeEmpId,
                                                   }
                                                 ).OrderBy(o => o.Name)
                                                 .AsEnumerable()); ;

            //IQueryable<StockInfo> queryable = context.StockInfoes.OrderByDescending(x => x.StockInfoId);
            return model;
        }

        public string StocName(int stockId)
        {
            string stockName = context.StockInfoes.Where(x => x.StockInfoId == stockId).Select(x => x.Name).FirstOrDefault();
            return stockName;
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


        public List<SelectModel> GetStockInfoSelectModels(int companyId)
        {
            return context.StockInfoes.Where(x => x.CompanyId == companyId && x.IsActive).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.StockInfoId

            }).ToList();
        }

        public List<SelectModel> GetFactorySelectModels(int companyId)
        {
            if (companyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                return context.StockInfoes.Where(x => x.CompanyId == companyId && x.IsActive).ToList().Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.StockInfoId
                }).ToList();
            }
            else
            {
                return context.StockInfoes.Where(x => x.StockType.Equals("F") && x.CompanyId == companyId && x.IsActive).ToList().Select(x => new SelectModel()
                {
                    Text = x.Name,
                    Value = x.StockInfoId
                }).ToList();

            }

        }

        public List<SelectModel> GetDepoSelectModels(int companyId)
        {
            return context.StockInfoes.Where(x => !x.StockType.Equals("F") && x.CompanyId == companyId && x.IsActive).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.StockInfoId
            }).OrderBy(x => x.Text).ToList();
        }

        public List<SelectModel> GetKFMALDepoSelectModels(int companyId)
        {
            return context.StockInfoes.Where(x => !x.StockType.Equals("F") && x.CompanyId == companyId && x.IsActive).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.StockInfoId
            }).OrderBy(x => x.Text).ToList();
        }

        public List<SelectModel> GetStoreSelectModels(int companyId)
        {
            return context.StockInfoes.Where(x => x.CompanyId == companyId && x.IsActive).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.StockInfoId
            }).OrderBy(x => x.Text).ToList();
        }

        public List<SelectModel> GetALLStoreSelectModels(int companyId)
        {
            List<SelectModel> stocks = context.StockInfoes.Where(x => x.CompanyId == companyId && x.IsActive).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.StockInfoId
            }).OrderBy(x => x.Text).ToList();
            stocks.Add(new SelectModel { Text = "All", Value = "0" });
            return stocks.OrderBy(x => Convert.ToInt32(x.Value)).ToList();
        }


        public List<SelectModel> GetALLZoneSelectModels(int companyId)
        {
            List<SelectModel> stocks = context.Zones.Where(x => x.CompanyId == companyId && x.IsActive).ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.ZoneId
            }).OrderBy(x => x.Text).ToList();
            stocks.Add(new SelectModel { Text = "All", Value = "0" });
            return stocks.OrderBy(x => Convert.ToInt32(x.Value)).ToList();
        }

        public async Task<int> StockInfoAdd(StockInfoModel model)
        {
            var result = -1;
            StockInfo obj = new StockInfo
            {
                Name = model.Name,
                CompanyId = model.CompanyId,
                Code= model.Code,
               // CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
              //  CreatedDate = DateTime.Now,
                IsActive = true,
                DepotInchargeEmpId= model.DepotInchargeEmpId,


            };
            context.StockInfoes.Add(obj);
            if (await context.SaveChangesAsync() > 0)
            {
                result = obj.StockInfoId;
            }
            return result;
        }

        public async Task<int> StockInfoEdit(StockInfoModel model)
        {
            var result = -1;
            StockInfo obj = context.StockInfoes.Find(model.StockInfoId);
            obj.Name = model.Name;
            obj.Code = model.Code;
            obj.DepotInchargeEmpId = model.DepotInchargeEmpId;


           // obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            //obj.ModifiedDate = DateTime.Now;

            if (await context.SaveChangesAsync() > 0)
            {
                result = obj.StockInfoId;
            }
            return result;
        }

        public async Task<int> StockInfoDelete(int id)
        {
            int result = -1;
            if (id != 0)
            {
                StockInfo obj = context.StockInfoes.Find(id);
                obj.IsActive = false;

                if (await context.SaveChangesAsync() > 0)
                {
                    result = obj.StockInfoId;
                }
            }
            return result;
        }


        //public List<SelectModel> GetSuppliers()
        //{
        //    return supplierRepository.Suppliers.ToList().Select(x => new SelectModel()
        //    {
        //        Text = x.Name.ToString(),
        //        Value = x.SupplierId.ToString()

        //    }).ToList();
        //}

        //public List<SelectModel> GetStokNames()
        //{
        //    return stockRepository.StockInfoes.ToList().Select(x => new SelectModel()
        //    {
        //        Text = x.Name.ToString(),
        //        Value = x.StockInfoId.ToString()

        //    }).ToList();
        //}
    }
}
