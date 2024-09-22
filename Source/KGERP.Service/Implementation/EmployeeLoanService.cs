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
    public class EmployeeLoanService : IEmployeeLoanService
    {
        private bool disposed = false;

        ERPEntities context = new ERPEntities();
        public List<EmployeeLoanModel> GetEmployeeLoans(string searchText)
        {
            //IQueryable<EmployeeLoan> EmployeeLoans = null;
            //EmployeeLoans = context.EmployeeLoans.Where(x => x.EmployeeId.Contains(searchText)).OrderBy(x => x.LoanID);
            //return ObjectConverter<EmployeeLoan, EmployeeLoanModel>.ConvertList(EmployeeLoans.ToList()).ToList();

            dynamic result = context.Database.SqlQuery<EmployeeLoanModel>("exec sp_PFLoan_getAllLoan").ToList();
            return result;
        }

        public EmployeeLoanModel GetEmployeeLoan(int id)
        {
            if (id == 0)
            {
                return new EmployeeLoanModel() { LoanID = id };
            }
            EmployeeLoan EmployeeLoan = context.EmployeeLoans.Find(id);
            return ObjectConverter<EmployeeLoan, EmployeeLoanModel>.Convert(EmployeeLoan);
        }

        public bool SaveEmployeeLoan(int id, EmployeeLoanModel model)
        {
            EmployeeLoan EmployeeLoan = ObjectConverter<EmployeeLoanModel, EmployeeLoan>.Convert(model);
            bool result = false;
            if (id > 0)
            {
                //EmployeeLoan = context.EmployeeLoans.FirstOrDefault(x => x.LoanID == id);

                EmployeeLoan.CreatedBy = model.CreatedBy;
                EmployeeLoan.CreatedDate = model.CreatedDate;
                EmployeeLoan.ModifiedDate = DateTime.Now;
                EmployeeLoan.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                EmployeeLoan.EmployeeId = model.EmployeeId;
                EmployeeLoan.LoanType = model.LoanType;
                EmployeeLoan.LoanPurpose = model.LoanPurpose;
                EmployeeLoan.Amount = model.Amount;
                EmployeeLoan.NoOfInstallment = model.NoOfInstallment;
                EmployeeLoan.LoanApplyDate = model.LoanApplyDate;
                EmployeeLoan.InstallmentAmount = model.InstallmentAmount;
                EmployeeLoan.InstallStartDate = model.InstallStartDate;
                EmployeeLoan.LoanEndDate = model.LoanEndDate;
                EmployeeLoan.Remarks = model.Remarks;

            }
            else
            {
                EmployeeLoan.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                EmployeeLoan.CreatedDate = DateTime.Now;
                EmployeeLoan.EmployeeId = model.EmployeeId;
                EmployeeLoan.LoanType = model.LoanType;
                EmployeeLoan.LoanPurpose = model.LoanPurpose;
                EmployeeLoan.Amount = model.Amount;
                EmployeeLoan.NoOfInstallment = model.NoOfInstallment;
                EmployeeLoan.LoanApplyDate = model.LoanApplyDate;
                EmployeeLoan.InstallmentAmount = model.InstallmentAmount;
                EmployeeLoan.InstallStartDate = model.InstallStartDate;
                EmployeeLoan.LoanEndDate = model.LoanEndDate;
                EmployeeLoan.Remarks = model.Remarks;
            }
            context.Entry(EmployeeLoan).State = EmployeeLoan.LoanID == 0 ? EntityState.Added : EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                return result = true;
            }
            else
            {
                return result;
            }
        }

        public bool DeleteEmployeeLoan(int id)
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

        public List<SelectModel> GetEmployeeLoanEmployees()
        {
            return context.Employees.Where(x => x.Active == true && x.DepartmentId == 6).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.EmployeeId.ToString()
            }).ToList();
        }

        public List<EmployeeLoanModel> GetEmployeeLoanEvent()
        {
            dynamic result = context.Database.SqlQuery<EmployeeLoanModel>("exec sp_GetUpcomingCaseEvent").ToList();
            return result;
        }

        public List<EmployeeLoanModel> GetCompanyBaseCaseList(int companyId)
        {
            dynamic result = context.Database.SqlQuery<EmployeeLoanModel>("exec lnl_GetCompanyBaseCaseList {0}", companyId).ToList();
            return result;
        }

        public List<EmployeeLoanModel> GetPrevious7DaysCaseSchedule()
        {
            dynamic result = context.Database.SqlQuery<EmployeeLoanModel>("exec sp_LnL_OneWeekPreviousCaseSchedule").ToList();
            return result;
        }

    }
}
