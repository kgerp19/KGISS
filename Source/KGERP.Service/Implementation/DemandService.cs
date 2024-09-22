using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class DemandService : IDemandService
    {
        private bool disposed = false;
        private readonly ERPEntities context;
        public DemandService(ERPEntities context)
        {
            this.context = context;
        }

        public async Task<DemandModel> GetDemandList (int companyId, DateTime? fromDate, DateTime? toDate)
        {
            DemandModel demandModel = new DemandModel();
            demandModel.CompanyId = companyId;
            demandModel.DataList = await Task.Run(() => (from t1 in context.Demands
                                                         where t1.CompanyId == companyId
                                                         && t1.DemandDate >= fromDate
                                                         && t1.DemandDate <= toDate
                                                         select new DemandModel
                                                         {
                                                             DemandDate = t1.DemandDate,
                                                             DemandId = t1.DemandId,
                                                             DemandNo = t1.DemandNo,
                                                             Remarks = t1.Remarks
                                                         }).OrderByDescending(o=>o.DemandDate).AsEnumerable());

            return demandModel;
        }
        public List<DemandModel> GetDemands(int companyId, string searchDate, string searchText)
        {
            DateTime? dateSearch = null;
            dateSearch = !string.IsNullOrEmpty(searchDate) ? Convert.ToDateTime(searchDate) : dateSearch;

            IQueryable<Demand> demands = context.Demands.Where(x => x.CompanyId == companyId&&x.RequisitionType==2);

            if (dateSearch == null)
            {
                demands = demands.Where(x => (x.DemandNo.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                     (x.Remarks.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText))
                                    ).OrderByDescending(x => x.DemandDate);
            }
            else if (string.IsNullOrEmpty(searchText) && dateSearch != null)
            {
                demands = demands.Where(x => x.DemandDate <= dateSearch);
            }
            else
            {
                demands = demands.Where(x => x.DemandDate <= dateSearch &&
                                 (x.DemandNo.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText)) ||
                                 (x.Remarks.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText))
                               ).OrderByDescending(x => x.DemandDate);
            }

            return ObjectConverter<Demand, DemandModel>.ConvertList(demands.ToList()).ToList();
        }

        public DemandModel GetDemand(long id)
        {
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

            if (id <= 0)
            {
                return new DemandModel() { CompanyId = companyId };
            }
            Demand demand = context.Demands.Include(x => x.DemandItems).Where(x => x.DemandId == id).FirstOrDefault();
            if (demand == null)
            {
                throw new Exception("Date not found");
            }
            DemandModel model = ObjectConverter<Demand, DemandModel>.Convert(demand);
            return model;
        }

        public bool SaveDemand(long id, DemandModel model, out string message)
        {
            message = string.Empty;
            if (model == null)
            {
                throw new Exception("Store data missing!");
            }
            bool isDemandExists = context.Demands.Where(x => x.CompanyId == model.CompanyId && x.DemandNo.Equals(model.DemandNo)).Any();
            if (isDemandExists)
            {
                message = "Demand Already Exists !";
                return false;
            }
            Demand demand = ObjectConverter<DemandModel, Demand>.Convert(model);
            if (id > 0)
            {
                demand = context.Demands.Where(x => x.DemandId == id).FirstOrDefault();
                if (demand == null)
                {
                    throw new Exception("Store data not found!");
                }
                demand.ModifiedDate = DateTime.Now;
                demand.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }

            else
            {
                demand.IsActive = true;
                demand.CreatedDate = DateTime.Now;
                demand.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }
            demand.DemandNo = model.DemandNo;
            demand.DemandDate = model.DemandDate;

            context.Demands.Add(demand);


            context.Entry(demand).State = demand.DemandId == 0 ? EntityState.Added : EntityState.Modified;
            int noOfRowsAffected = context.SaveChanges();

            if (noOfRowsAffected > 0)
            {
                //---Raw Material insert as per demand into DemandItemDetail---------
                context.Database.ExecuteSqlCommand("exec sp_Feed_CalculateRMDemand {0}", demand.DemandId);
            }
            return false;
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

        public List<SoreProductQty> GetStoreProductQty()
        {
            dynamic result = context.Database.SqlQuery<SoreProductQty>("sp_GetStoreProductQuantity ").ToList();
            return result;
        }

        public List<SelectModel> GetDemandSelectModels(int companyId)
        {
            List<Demand> demands = context.Demands.Where(x => x.CompanyId == companyId && x.RequisitionType == 2).ToList();
            return demands.Select(x => new SelectModel { Text = x.DemandNo, Value = x.DemandId }).ToList();
            // List<DemandCustomModel> demands = context.Database.SqlQuery<DemandCustomModel>(@"select DemandId,DemandNo 
            //from Erp.Demand
            //where CompanyId ={0} and DemandId not in (select DemandId from Erp.PurchaseOrder)", companyId).ToList();
            // return demands.Select(x => new SelectModel { Text = x.DemandNo, Value = x.DemandId }).ToList();
        }



        public string GetNewDemandNo(string strDemandDate)
        {
            DateTime demandDate = Convert.ToDateTime(strDemandDate);
            string year = demandDate.Year.ToString();
            int monthNo = Convert.ToInt32(demandDate.Month.ToString());
            string month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(monthNo).ToUpper();
            return "D-" + month + '-' + year;
        }

        public List<DemandItemModel> GetDemandItems(long demandId)
        {
            return context.Database.SqlQuery<DemandItemModel>(@"select 
                                                                (select ProductCode from Erp.Product where ProductId=Erp.DemandItem.ProductId) as ProductCode,
                                                                (select ProductName from Erp.Product where ProductId=Erp.DemandItem.ProductId) as ProductName,
                                                                Qty 
                                                                from Erp.DemandItem where DemandId={0} 
                                                                order by (select ProductName from Erp.Product where ProductId=Erp.DemandItem.ProductId)", demandId).ToList();
        }

        public List<DemandItemDetailModel> GetDemandItemDetails(long demandId)
        {
            return context.Database.SqlQuery<DemandItemDetailModel>(@"sp_Feed_RawMaterialDemand {0}", demandId).ToList();
        }
    }
}
