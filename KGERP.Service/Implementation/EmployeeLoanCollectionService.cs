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
    public class EmployeeLoanCollectionService : IEmployeeLoanCollectionService
    {
        private bool disposed = false;

        ERPEntities context = new ERPEntities();
        public List<EmployeeLoanCollectionModel> GetEmployeeLoanCollections(string searchText)
        {
            //IQueryable<EmployeeLoanCollection> EmployeeLoanCollections = null;
            //EmployeeLoanCollections = context.EmployeeLoanCollections.Where(x => x.EmployeeId.Contains(searchText)).OrderBy(x => x.LoanID);
            //return ObjectConverter<EmployeeLoanCollection, EmployeeLoanCollectionModel>.ConvertList(EmployeeLoanCollections.ToList()).ToList();

            dynamic result = context.Database.SqlQuery<EmployeeLoanCollectionModel>("exec sp_PFLoan_getCollectedLoan").ToList();
            return result;
        }

        public EmployeeLoanCollectionModel GetEmployeeLoanCollection(int id)
        {
            if (id == 0)
            {
                return new EmployeeLoanCollectionModel() { LoanCollectionId = id };
            }
            LoanCollection EmployeeLoanCollection = context.LoanCollections.Find(id);
            return ObjectConverter<LoanCollection, EmployeeLoanCollectionModel>.Convert(EmployeeLoanCollection);
        }

        public bool SaveEmployeeLoanCollection(int id, EmployeeLoanCollectionModel model)
        {
            LoanCollection EmployeeLoanCollection = ObjectConverter<EmployeeLoanCollectionModel, LoanCollection>.Convert(model);
            bool result = false;
            if (id > 0)
            {
                //EmployeeLoanCollection = context.EmployeeLoanCollections.FirstOrDefault(x => x.LoanID == id);

                EmployeeLoanCollection.CreatedBy = model.CreatedBy;
                EmployeeLoanCollection.CreatedDate = model.CreatedDate;
                EmployeeLoanCollection.ModifiedDate = DateTime.Now;
                EmployeeLoanCollection.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                EmployeeLoanCollection.EmployeeId = model.EmployeeId;
                EmployeeLoanCollection.LoanId = model.LoanId;
                EmployeeLoanCollection.Amount = model.Amount;
                EmployeeLoanCollection.ForMonthDate = model.ForMonthDate;
                EmployeeLoanCollection.Remarks = model.Remarks;

            }
            else
            {
                EmployeeLoanCollection.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                EmployeeLoanCollection.CreatedDate = DateTime.Now;
                EmployeeLoanCollection.EmployeeId = model.EmployeeId;
                EmployeeLoanCollection.LoanId = model.LoanId;
                EmployeeLoanCollection.Amount = model.Amount;
                EmployeeLoanCollection.ForMonthDate = model.ForMonthDate;
                EmployeeLoanCollection.Remarks = model.Remarks;
            }
            context.Entry(EmployeeLoanCollection).State = EmployeeLoanCollection.LoanCollectionId == 0 ? EntityState.Added : EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                return result = true;
            }
            else
            {
                return result;
            }
        }

        public bool DeleteEmployeeLoanCollection(int id)
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

        public List<SelectModel> GetEmployeeLoanCollectionEmployees()
        {
            return context.Employees.Where(x => x.Active == true && x.DepartmentId == 6).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.EmployeeId.ToString()
            }).ToList();
        }

        public List<EmployeeLoanCollectionModel> GetEmployeeLoanCollectionEvent()
        {
            dynamic result = context.Database.SqlQuery<EmployeeLoanCollectionModel>("exec sp_GetUpcomingCaseEvent").ToList();
            return result;
        }

        public List<EmployeeLoanCollectionModel> GetCompanyBaseCaseList(int companyId)
        {
            dynamic result = context.Database.SqlQuery<EmployeeLoanCollectionModel>("exec lnl_GetCompanyBaseCaseList {0}", companyId).ToList();
            return result;
        }

        public List<EmployeeLoanCollectionModel> GetPrevious7DaysCaseSchedule()
        {
            dynamic result = context.Database.SqlQuery<EmployeeLoanCollectionModel>("exec sp_LnL_OneWeekPreviousCaseSchedule").ToList();
            return result;
        }

    }
}
