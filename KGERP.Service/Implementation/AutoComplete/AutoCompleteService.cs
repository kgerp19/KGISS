using KGERP.Data.Models;
using KGERP.Service.Implementation.AutoComplete.ViewModels;
using KGERP.Service.Implementation.General_Requisition;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.AutoComplete
{
    public class AutoCompleteService : IAutoCompleteService
    {
        private readonly ERPEntities _context;
        public AutoCompleteService(ERPEntities eRPEntities)
        {
            _context = eRPEntities;
        }
        public async Task<IEnumerable<AutoCompleteVM>> GetAllEmployeeAutoComplete(string prefix = null)
        {
            var companyList = (from c in _context.Employees
                               join ds in _context.Designations on c.DesignationId equals ds.DesignationId
                               where c.Active && (string.IsNullOrEmpty(prefix) || c.Name.Contains(prefix) || c.EmployeeId.Contains(prefix))
                               select new AutoCompleteVM { Id = (int)c.Id,EmployeeId = c.EmployeeId, Name = c.Name+" - "+c.EmployeeId+" ("+ds.Name+")" }).AsEnumerable();
            return companyList;
        }
        public async Task<IEnumerable<AutoCompleteVM>> TaskGetAllEmployeeAutoComplete(string prefix = null,int workspaceid = 0)
        {
            var employeelist = (from c in _context.WorkSpacesMembers
                                join emp in _context.Employees on c.EmployeeId equals emp.Id
                               join ds in _context.Designations on emp.DesignationId equals ds.DesignationId
                               where c.WorkSpacesId==workspaceid && c.IsActive && (string.IsNullOrEmpty(prefix) || emp.Name.Contains(prefix) || emp.EmployeeId.Contains(prefix)) 
                               select new AutoCompleteVM { Id = (int)emp.Id, EmployeeId = emp.EmployeeId, Name = emp.Name + " - " + c.EmployeeId + " (" + ds.Name + ")" }).AsEnumerable();
            return employeelist;
        }
        public async Task<IEnumerable<AutoCompleteVM>> GetAllCompanyAutoCompleteAsync(string prefix = null)
        {
            var companyList = (from c in _context.Companies
                               where c.IsActive && (string.IsNullOrEmpty(prefix) || c.Name.Contains(prefix))
                               select new AutoCompleteVM { Id = c.CompanyId, Name = c.Name }).AsEnumerable();
            return  companyList;
        }
        public async Task<IEnumerable<AutoCompleteVM>> GetAllProjectList(int? companyId)
        {
            var projectList = (from c in _context.Accounting_CostCenter
                               where c.IsActive && (companyId == null || c.CompanyId == companyId)
                               select new AutoCompleteVM { Id = c.CostCenterId, Name = c.Name }).AsEnumerable();
            return projectList;
        }
        public async Task<IEnumerable<AutoCompleteVM>>  GetAllDepartmentAutoCompleteAsync(string prefix = null)
        {
            var companyList = (from c in _context.Departments
                               where (string.IsNullOrEmpty(prefix) || c.Name.Contains(prefix))
                               select new AutoCompleteVM { Id = c.DepartmentId, Name = c.Name }).AsEnumerable();
            return companyList;
        }


        public async Task<IEnumerable<AutoCompleteVM>> GetAllGRQSProductCategoryAutoCompleteAsync(string prefix = null)
        {
            var categoryList = (from c in _context.GeneralRequisitionProductCategories
                               where c.IsActive && (string.IsNullOrEmpty(prefix) || c.CategoryName.Contains(prefix))
                               select new AutoCompleteVM { Id = c.Id, Name = c.CategoryName }).AsEnumerable();
            return categoryList;
        }

        public async Task<EmployeeVm> GetEmployee(int employeeId)
        {
            var employee = (from e in _context.Employees
                                join c in _context.Companies on e.CompanyId equals c.CompanyId
                                join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                                join ds in _context.Designations on e.DesignationId equals ds.DesignationId

                                where e.Active && e.Id == employeeId
                                select new EmployeeVm
                                {
                                    Id = e.Id,
                                    EmployeeName = e.Name,
                                    DepartmentName = d.Name,
                                    DesignationName = ds.Name,
                                    CompanyName = c.Name


                                }).FirstOrDefault();
            return employee;
        }
    }
}
