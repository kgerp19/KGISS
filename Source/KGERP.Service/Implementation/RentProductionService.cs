using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class RentProductionService : IRentProductionService
    {
        private readonly ERPEntities context;
        public RentProductionService(ERPEntities context)
        {
            this.context = context;
        }
        public async Task<RentProductionModel> GetRentProductions(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            RentProductionModel model = new RentProductionModel();
            model.CompanyId = companyId;
            model.DataList = await Task.Run(() => (from t1 in context.RentProductions
                                                                join t2 in context.Vendors on t1.RentCompanyId equals t2.VendorId         
                                                                where t1.CompanyId == companyId
                                                                && t1.ProductionDate >= fromDate
                                                                && t1.ProductionDate <= toDate
                                                                select new RentProductionModel
                                                                {
                                                                    ProductionDate= t1.ProductionDate,
                                                                    RentProductionNo = t1.RentProductionNo,
                                                                    RentCompanyName = t2.Name,
                                                                    Remarks = t1.Remarks
                                                                }).OrderByDescending(o => o.ProductionDate).AsEnumerable());

            return model;

        }
        public List<RentProductionModel> GetRentProductions(string searchDate, string searchText, int companyId)
        {

            DateTime? dateSearch = null;
            dateSearch = !string.IsNullOrEmpty(searchDate) ? Convert.ToDateTime(searchDate) : dateSearch;

            List<RentProductionModel> models = context.Database.SqlQuery<RentProductionModel>("exec GetRentProductions {0}", companyId).ToList();

            if (dateSearch == null)
            {
                return models.Where(x => (x.RentProductionNo.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                    (x.RentCompanyName.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
                                     (x.RentCompanyOwner.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText))
                                    ).OrderByDescending(x => x.ProductionDate).ToList();
            }
            if (string.IsNullOrEmpty(searchText) && dateSearch != null)
            {
                return models.Where(x => x.ProductionDate.Value.Date == dateSearch.Value.Date).OrderByDescending(x => x.ProductionDate).ToList();
            }


            return models.Where(x => x.ProductionDate.Value.Date == dateSearch.Value.Date &&
                                (x.RentProductionNo.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText)) ||
                                (x.RentCompanyName.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText)) ||
                                   (x.RentCompanyOwner.ToLower().Contains(searchText) || String.IsNullOrEmpty(searchText))
                               ).OrderByDescending(x => x.ProductionDate).ToList();
        }
        public RentProductionModel GetRentProduction(int rentProductionId)
        {
            if (rentProductionId <= 0)
            {
                return new RentProductionModel()
                {
                    CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]),
                    RentProductionNo = "To be shown..."
                };
            }
            RentProduction rentProduction = context.RentProductions.Where(x => x.RentProductionId == rentProductionId).FirstOrDefault();
            if (rentProduction == null)
            {
                throw new Exception("Rent Production data not found");
            }
            RentProductionModel model = ObjectConverter<RentProduction, RentProductionModel>.Convert(rentProduction);
            return model;
        }

        public bool SaveRentProduction(int rentProductionId, RentProductionModel model, out string message)
        {
            message = string.Empty;
            RentProduction rentProduction = ObjectConverter<RentProductionModel, RentProduction>.Convert(model);


            if (rentProductionId > 0)
            {
                rentProduction = context.RentProductions.FirstOrDefault(x => x.RentProductionId == rentProductionId);
                if (rentProduction == null)
                {
                    throw new Exception("Rent Production data not found!");
                }
                rentProduction.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                rentProduction.ModifiedDate = DateTime.Now;
            }
            else
            {
                //foreach (var rentProductionDetail in rentProduction.RentProductionDetails)
                //{
                //    orderDetail.CustomerId = model.CustomerId;
                //    orderDetail.OrderDate = model.OrderDate;
                //    orderDetail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                //    orderDetail.CreateDate = DateTime.Now;
                //    orderDetail.IsActive = true;
                //}
                rentProduction.IsActive = true;
                rentProduction.CreateDate = DateTime.Now;
                rentProduction.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }
            if (rentProduction.RentProductionId > 0)
            {
                rentProduction.RentProductionNo = model.RentProductionNo;
            }
            else
            {
                RentProduction lastRentProdution = context.RentProductions.Where(x => x.CompanyId == model.CompanyId).OrderByDescending(x => x.RentProductionId).FirstOrDefault();
                if (lastRentProdution == null)
                {
                    rentProduction.RentProductionNo = GenerateSequenceNumber(0);
                }
                else
                {
                    string numberPart = lastRentProdution.RentProductionNo.Substring(5, 6);
                    int lastRentProductionNo = Convert.ToInt32(numberPart);
                    rentProduction.RentProductionNo = GenerateSequenceNumber(lastRentProductionNo);
                }
            }
            rentProduction.CompanyId = model.CompanyId;
            rentProduction.ProductionDate = model.ProductionDate.Value;
            rentProduction.Remarks = model.Remarks;

            context.Entry(rentProduction).State = rentProduction.RentProductionId == 0 ? EntityState.Added : EntityState.Modified;

            return context.SaveChanges() > 0;

        }

        private string GenerateSequenceNumber(int lastRentProductionNo)
        {
            lastRentProductionNo = lastRentProductionNo + 1;
            return "RENT-" + lastRentProductionNo.ToString().PadLeft(6, '0');
        }
    }
}
