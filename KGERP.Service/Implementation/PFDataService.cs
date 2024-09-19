
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
    public class PFDataService : IPFDataService
    {
        private bool disposed = false;

        ERPEntities context = new ERPEntities();
        public List<PFDataModel> GetPFDatas(string searchText)
        {
            IQueryable<KGPFData> pFDataModels = null;
            pFDataModels = context.KGPFDatas.Where(x => x.CompanyName.Contains(searchText));
            return ObjectConverter<KGPFData, PFDataModel>.ConvertList(pFDataModels.ToList()).ToList();
        }

        public PFDataModel GetPFData(int id)
        {
            if (id == 0)
            {
                return new PFDataModel() { PFDataId = id };
            }
            KGPFData pFData = context.KGPFDatas.Find(id);
            return ObjectConverter<KGPFData, PFDataModel>.Convert(pFData);
        }

        public bool SavePFData(int id, PFDataModel model)
        {
            KGPFData pFData = ObjectConverter<PFDataModel, KGPFData>.Convert(model);
            bool result = false;
            bool updateresult = false;

            string _sDate = "";
            string _eDate = "";
            if (id > 0)
            {
                pFData = context.KGPFDatas.FirstOrDefault(x => x.PFDataId == id);
                if (pFData != null)
                {
                    pFData.ModifiedDate = DateTime.Now;
                    pFData.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                    pFData.EmployeeId = model.EmployeeId;
                    pFData.SelfContribution = model.SelfContribution;
                    pFData.OfficeContribution = model.OfficeContribution;
                    pFData.SelfProfit = model.SelfProfit;
                    pFData.OfficeProfit = model.OfficeProfit;
                    pFData.PFCreatedDate = model.PFCreatedDate;
                    pFData.PfMonth = model.PfMonth;
                    updateresult = true;
                }
            }
            else
            {
                pFData.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                pFData.PFCreatedDate = DateTime.Now;
                pFData.ModifiedDate = DateTime.Now;
                pFData.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                pFData.EmployeeId = model.EmployeeId;
                pFData.SelfContribution = model.SelfContribution;
                pFData.OfficeContribution = model.OfficeContribution;
                pFData.SelfProfit = model.SelfProfit;
                pFData.OfficeProfit = model.OfficeProfit;
                pFData.PFCreatedDate = model.PFCreatedDate;
                pFData.PfMonth = model.PfMonth;
                updateresult = true;
            }
            context.Entry(pFData).State = pFData.PFDataId == 0 ? EntityState.Added : EntityState.Modified;


            return result;

        }

        public bool DeletePFData(int id)
        {
            throw new NotImplementedException();
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

        public List<SelectModel> GetPFDataModelEmployees()
        {
            return context.Employees.Where(x => x.Active == true && x.DepartmentId == 6).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.EmployeeId.ToString()
            }).ToList();
        }

        public List<PFDataModel> GetPFDataModelEvent()
        {
            dynamic result = context.Database.SqlQuery<PFDataModel>("exec sp_GetUpcomingCaseEvent").ToList();
            return result;
            //DateTime dtFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //DateTime dtTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, DateTime.Now.Day);
            //IQueryable<PFDataModel> PFDataModels = context.PFDataModels.Where(x => x.NextDate >= dtFrom && x.NextDate <= dtTo).OrderBy(x => x.NextDate);
            ////IQueryable<PFDataModel> PFDataModels = context.PFDataModels.OrderByDescending(x => x.NextDate);
            //return ObjectConverter<PFDataModel, PFDataModel>.ConvertList(PFDataModels.ToList()).ToList();
        }

        public List<PFDataModel> GetPrevious7DaysOperationSchedule()
        {
            dynamic result = context.Database.SqlQuery<PFDataModel>("exec sp_LnL_OneWeekPreviousCaseSchedule").ToList();
            return result;
        }

        public List<PFDataModel> GetKGCaseList(string searchText)
        {
            dynamic result = context.Database.SqlQuery<PFDataModel>("exec sp_6GetKGCaseList {0} ", searchText).ToList();
            return result;
        }


        public List<SelectModel> GetPFDatas()
        {
            throw new NotImplementedException();
        }


    }
}
