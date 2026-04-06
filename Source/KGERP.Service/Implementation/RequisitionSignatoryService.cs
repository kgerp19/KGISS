using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class RequisitionSignatoryService : IRequisitionSignatoryService
    {
        private readonly ERPEntities context;

        public RequisitionSignatoryService(ERPEntities context)
        {
            this.context = context;
        }

        public async Task<RequisitionSignatoryViewModel> GetAllSignatories(int companyId, string integrateWith = null)
        {
            var model = new RequisitionSignatoryViewModel();

            var query = from rs in context.RequisitionSignatories
                        join emp in context.Employees on rs.EmployeeId equals emp.Id into empJoin
                        from emp in empJoin.DefaultIfEmpty()
                        join sig in context.Employees on rs.SignatoryEmpId equals sig.Id into sigJoin
                        from sig in sigJoin.DefaultIfEmpty()
                        join comp in context.Companies on rs.CompanyId equals comp.CompanyId into compJoin
                        from comp in compJoin.DefaultIfEmpty()
                        where rs.IsActive && rs.CompanyId==companyId
                        select new { rs, emp, sig, comp };

            if (companyId > 0)
            {
                query = query.Where(x => x.rs.CompanyId == companyId);
            }

            if (!string.IsNullOrEmpty(integrateWith))
            {
                query = query.Where(x => x.rs.IntegrateWith == integrateWith);
            }

            var data = await query.OrderBy(x => x.rs.CompanyId)
                .ThenBy(x => x.rs.EmployeeId)
                .ThenBy(x => x.rs.OrderBy)
                .ToListAsync();

            model.DataList = data.Select(x => new RequisitionSignatoryViewModel
            {
                RequisitionSignatoryId = x.rs.RequisitionSignatoryId,
                EmployeeId = x.rs.EmployeeId,
                EmployeeName = x.emp != null ? x.emp.Name : "",
                EmployeeCode = x.emp != null ? x.emp.EmployeeId : "",
                SignatoryEmpId = x.rs.SignatoryEmpId,
                SignatoryName = x.sig != null ? x.sig.Name : "",
                SignatoryCode = x.sig != null ? x.sig.EmployeeId : "",
                DesignationName = x.rs.DesignationName,
                OrderBy = x.rs.OrderBy,
                IsHRAdmin = x.rs.IsHRAdmin,
                IntegrateWith = x.rs.IntegrateWith,
                Limitation = x.rs.Limitation,
                CompanyId = x.rs.CompanyId,
                CompanyName = x.comp != null ? x.comp.Name : "",
                CategoryId = x.rs.CategoryId,
                CreatedBy = x.rs.CreatedBy,
                CreatedDate = x.rs.CreatedDate,
                ModifiedBy = x.rs.ModifiedBy,
                ModifiedDate = x.rs.ModifiedDate,
                IsActive = x.rs.IsActive,
                IsSupremeApproved = x.rs.IsSupremeApproved
            }).ToList();

            // Populate dropdowns
            model.ModelNames = GetModelNames(companyId);
            model.Companies = GetCompaniesByUser(companyId);
            model.Categories = GetCategories();

            return model;
        }

        public async Task<RequisitionSignatoryViewModel> GetSignatoryById(long id, int companyId)
        {
            var result = await (from rs in context.RequisitionSignatories
                                join emp in context.Employees on rs.EmployeeId equals emp.Id into empJoin
                                from emp in empJoin.DefaultIfEmpty()
                                join sig in context.Employees on rs.SignatoryEmpId equals sig.Id into sigJoin
                                from sig in sigJoin.DefaultIfEmpty()
                                join comp in context.Companies on rs.CompanyId equals comp.CompanyId into compJoin
                                from comp in compJoin.DefaultIfEmpty()
                                where rs.RequisitionSignatoryId == id
                                select new { rs, emp, sig, comp }).FirstOrDefaultAsync();



            if (result == null)
                return null;

            string empDesignationName = await context.Designations.Where(x => x.DesignationId == result.emp.DesignationId).Select(x => x.Name).FirstOrDefaultAsync();

            var model = new RequisitionSignatoryViewModel
            {
                RequisitionSignatoryId = result.rs.RequisitionSignatoryId,
                EmployeeId = result.rs.EmployeeId,
                EmployeeName = result.emp != null ? result.emp.Name : "",
                EmployeeCode = result.emp != null ? result.emp.EmployeeId : "",
                SignatoryEmpId = result.rs.SignatoryEmpId,
                SignatoryName = result.sig != null ? result.sig.Name : "",
                SignatoryCode = result.sig != null ? result.sig.EmployeeId : "",
                DesignationNameSig = result.rs.DesignationName,
                DesignationName = empDesignationName,
                OrderBy = result.rs.OrderBy,
                IsHRAdmin = result.rs.IsHRAdmin,
                IntegrateWith = result.rs.IntegrateWith,
                Limitation = result.rs.Limitation,
                CompanyId = result.rs.CompanyId,
                CompanyName = result.comp != null ? result.comp.Name : "",
                CategoryId = result.rs.CategoryId,
                CreatedBy = result.rs.CreatedBy,
                CreatedDate = result.rs.CreatedDate,
                ModifiedBy = result.rs.ModifiedBy,
                ModifiedDate = result.rs.ModifiedDate,
                IsActive = result.rs.IsActive,
                IsSupremeApproved = result.rs.IsSupremeApproved
            };

            // Populate dropdowns
            model.ModelNames = GetModelNames(companyId);
            model.Companies = GetCompaniesByUser(companyId);
            model.Categories = GetCategories();

            return model;
        }

        public async Task<bool> SaveSignatory(RequisitionSignatoryViewModel model)
        {
            try
            {
                RequisitionSignatory entity;
                int EmployeeCompanyId = await context.Employees.Where(x => x.Id == model.EmployeeId).Select(x => x.CompanyId.Value).FirstAsync();
                if (EmployeeCompanyId <= 0)
                {
                    throw new Exception("Employee Company Can't Found!");
                }
                if (model.RequisitionSignatoryId > 0)
                {
                    // Update existing
                    entity = await context.RequisitionSignatories
                        .FirstOrDefaultAsync(x => x.RequisitionSignatoryId == model.RequisitionSignatoryId);

                    if (entity == null)
                        return false;

                    entity.ModifiedBy = Common.GetUserId();
                    entity.ModifiedDate = DateTime.Now;
                }
                else
                {
                    // Create new
                    entity = new RequisitionSignatory
                    {
                        CreatedBy = Common.GetUserId(),
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };

                    // Validate 4 signatories rule before adding
                    var existingCount = await GetSignatoryCount(model.EmployeeId, model.IntegrateWith);
                    if (existingCount >= 4)
                    {
                        throw new Exception("Employee already has 4 signatories. Cannot add more.");
                    }

                    context.RequisitionSignatories.Add(entity);
                }

                // Update properties
                entity.EmployeeId = model.EmployeeId;
                entity.SignatoryEmpId = model.SignatoryEmpId;
                entity.DesignationName = model.DesignationNameSig;
                entity.OrderBy = model.OrderBy;
                entity.IsHRAdmin = model.IsHRAdmin;
                entity.IntegrateWith = model.IntegrateWith;
                entity.Limitation = model.Limitation;
                entity.CompanyId = model.CompanyId;
                entity.CategoryId = model.CategoryId;
                entity.IsSupremeApproved = model.IsHRAdmin;
                entity.CompanyId = EmployeeCompanyId;


                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving signatory: " + ex.Message);
            }
        }

        public async Task<bool> DeleteSignatory(long id)
        {
            try
            {
                var entity = await context.RequisitionSignatories
                    .FirstOrDefaultAsync(x => x.RequisitionSignatoryId == id);

                if (entity == null)
                    return false;

                // Soft delete
                entity.IsActive = false;
                entity.ModifiedBy = Common.GetUserId();
                entity.ModifiedDate = DateTime.Now;

                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting signatory: " + ex.Message);
            }
        }

        public async Task<bool> ReplaceApprover(ReplaceApproverViewModel model)
        {
            try
            {
                var query = context.RequisitionSignatories
                    .Where(x => x.SignatoryEmpId == model.OldSignatoryEmpId && x.IsActive);

                if (model.CompanyId.HasValue && model.CompanyId.Value > 0)
                {
                    query = query.Where(x => x.CompanyId == model.CompanyId.Value);
                }

                if (!string.IsNullOrEmpty(model.IntegrateWith))
                {
                    query = query.Where(x => x.IntegrateWith == model.IntegrateWith);
                }

                var entitiesToUpdate = await query.ToListAsync();

                if (!entitiesToUpdate.Any())
                    return false;

                // Get new signatory designation
                var newSignatory = await context.Employees
                    .Include(x => x.Designation)
                    .FirstOrDefaultAsync(x => x.Id == model.NewSignatoryEmpId);

                string newDesignation = newSignatory?.Designation?.Name ?? "";

                // Update all records
                foreach (var entity in entitiesToUpdate)
                {
                    entity.SignatoryEmpId = model.NewSignatoryEmpId;
                    entity.DesignationName = newDesignation;
                    entity.ModifiedBy = Common.GetUserId();
                    entity.ModifiedDate = DateTime.Now;
                }

                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error replacing approver: " + ex.Message);
            }
        }

        public async Task<bool> ValidateFourSignatories(long employeeId, string integrateWith)
        {
            var count = await GetSignatoryCount(employeeId, integrateWith);
            return count == 4;
        }

        public async Task<int> GetSignatoryCount(long employeeId, string integrateWith)
        {
            return await context.RequisitionSignatories
                .Where(x => x.EmployeeId == employeeId
                    && x.IntegrateWith == integrateWith
                    && x.IsActive)
                .CountAsync();
        }

        public async Task<string> getSignatoryDesignation(long signatoryId)
        {
            return await context.Employees
                   .Where(x => x.Id == signatoryId)
                   .Join(context.Designations,
                       employee => employee.DesignationId,
                       designation => designation.DesignationId,
                       (employee, designation) => designation.Name)
                   .FirstOrDefaultAsync();
        }

        public List<SelectModel> GetModelNames(int companyId)
        {
            return context.DropDownItems
                .Where(x => x.IsActive && x.DropDownTypeId == 76 && x.CompanyId== companyId && x.IsActive)
                .Select(x => x.Name)
                .Distinct()
                .OrderBy(x => x)
                .Select(x => new SelectModel
                {
                    Text = x,
                    Value = x
                })
                .ToList();
        }

        public List<SelectModel> GetCompaniesByUser(int companyId)
        {
            return context.Companies
                .Where(x => x.IsActive && x.CompanyId== companyId)
                .OrderBy(x => x.Name)
                .Select(x => new SelectModel
                {
                    Text = x.Name,
                    Value = x.CompanyId.ToString()
                })
                .ToList();
        }

        public List<SelectModel> GetDivisionsByCompany(int companyId)
        {
            return context.Divisions
                .Select(x => x.Name)
                .Distinct()
                .OrderBy(x => x)
                .Select(x => new SelectModel
                {
                    Text = x,
                    Value = x
                })
                .ToList();
        }

        public List<SelectModel> GetDepartmentsByDivision(int divisionId)
        {
            // Note: This is a simplified implementation
            // You may need to adjust based on your actual Division table structure
            return context.Departments
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new SelectModel
                {
                    Text = x.Name,
                    Value = x.DepartmentId.ToString()
                })
                .ToList();
        }

        public List<SelectModel> GetSectionsByDepartment(int departmentId)
        {
            // Implement based on your Section table structure
            // Placeholder implementation
            return new List<SelectModel>();
        }

        public List<SelectModel> GetDesignationsByDepartment(int departmentId)
        {
            return context.Designations
                .OrderBy(x => x.Name)
                .Select(x => new SelectModel
                {
                    Text = x.Name,
                    Value = x.DesignationId.ToString()
                })
                .ToList();
        }

        public List<SelectModel> GetEmployeesByFilters(int? companyId = null, int? divisionId = null, int? departmentId = null, int? designationId = null)
        {
            var query = context.Employees.Where(x => x.Active);

            if (companyId.HasValue && companyId.Value > 0)
            {
                query = query.Where(x => x.CompanyId == companyId.Value);
            }

            if (departmentId.HasValue && departmentId.Value > 0)
            {
                query = query.Where(x => x.DepartmentId == departmentId.Value);
            }

            if (designationId.HasValue && designationId.Value > 0)
            {
                query = query.Where(x => x.DesignationId == designationId.Value);
            }

            return query
                .OrderBy(x => x.Name)
                .Select(x => new SelectModel
                {
                    Text = x.EmployeeId + " - " + x.Name,
                    Value = x.Id.ToString()
                })
                .ToList();
        }

        public async Task<ReplaceApproverViewModel> GetEmployeeDetailsForReplace(long employeeId)
        {
            var employee = await context.Employees
                .Include(x => x.Company)
                .Include(x => x.Department)
                .Include(x => x.Designation)
                .FirstOrDefaultAsync(x => x.Id == employeeId);

            if (employee == null)
                return null;

            return new ReplaceApproverViewModel
            {
                NewSignatoryEmpId = employee.Id,
                NewSignatoryName = employee.Name,
                NewSignatoryCode = employee.EmployeeId,
                NewCompanyName = employee.Company?.Name ?? "",
                NewDepartmentName = employee.Department?.Name ?? "",
                NewDesignationName = employee.Designation?.Name ?? "",
                NewAppointmentDate = employee.JoiningDate,
                NewEmployeeStatus = employee.Active ? "Active" : "Inactive"
            };
        }

        public List<SelectModel> GetCategories()
        {
            return context.ProductCategories
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new SelectModel
                {
                    Text = x.Name,
                    Value = x.ProductCategoryId.ToString()
                })
                .ToList();
        }

        public async Task<object> GetSignatoriesForDataTable(int start, int length, string searchValue, string sortColumn, string sortDirection, long? employeeId, int? companyId, int? divisionId, int? departmentId, int? designationId, string integrateWith)
        {
            var query = from rs in context.RequisitionSignatories
                        join emp in context.Employees on rs.EmployeeId equals emp.Id into empJoin
                        from emp in empJoin.DefaultIfEmpty()
                        join sig in context.Employees on rs.SignatoryEmpId equals sig.Id into sigJoin
                        from sig in sigJoin.DefaultIfEmpty()
                        join comp in context.Companies on rs.CompanyId equals comp.CompanyId into compJoin
                        from comp in compJoin.DefaultIfEmpty()
                        join dept in context.Departments on emp.DepartmentId equals dept.DepartmentId into deptJoin
                        from dept in deptJoin.DefaultIfEmpty()
                        where rs.IsActive
                        select new { rs, emp, sig, comp, dept };

            // Apply filters
            if (companyId.HasValue && companyId.Value > 0)
            {
                query = query.Where(x => x.rs.CompanyId == companyId.Value);
            }

            if (!string.IsNullOrEmpty(integrateWith))
            {
                query = query.Where(x => x.rs.IntegrateWith == integrateWith);
            }

            if (departmentId.HasValue && departmentId.Value > 0)
            {
                query = query.Where(x => x.emp.DepartmentId == departmentId.Value);
            }

            if (designationId.HasValue && designationId.Value > 0)
            {
                query = query.Where(x => x.emp.DesignationId == designationId.Value);
            }

            if (employeeId.HasValue && employeeId.Value > 0)
            {
                query = query.Where(x => x.emp.Id == employeeId.Value);
            }

            // Apply search
            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(x =>
                    x.emp.Name.Contains(searchValue) ||
                    x.sig.Name.Contains(searchValue) ||
                    x.comp.Name.Contains(searchValue) ||
                    x.rs.DesignationName.Contains(searchValue)
                );
            }

            int totalRecords = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(sortColumn))
            {
                if (sortDirection == "asc")
                {
                    query = query.OrderBy(x => x.rs.CompanyId);
                }
                else
                {
                    query = query.OrderByDescending(x => x.rs.CompanyId);
                }
            }
            else
            {
                query = query.OrderBy(x => x.rs.CompanyId).ThenBy(x => x.rs.OrderBy);
            }

            // Apply pagination
            var data = await query
                .Skip(start)
                .Take(length)
                .Select(x => new
                {
                    requisitionSignatoryId = x.rs.RequisitionSignatoryId,
                    companyName = x.comp != null ? x.comp.Name : "",
                    departmentName = x.dept != null ? x.dept.Name ?? "" : "",
                    designationName = x.rs.DesignationName,
                    approverName = x.sig != null ? x.sig.Name : "",
                    approverCode = x.sig != null ? x.sig.EmployeeId : "",
                    level = x.rs.OrderBy,
                    limitation = x.rs.Limitation,
                    employeeName = x.emp.Name
                })
                .ToListAsync();

            return new
            {
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            };
        }

        public List<SelectModel> GetDepartmentsByCompany(int companyId)
        {
            return context.Departments
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new SelectModel
                {
                    Text = x.Name,
                    Value = x.DepartmentId.ToString()
                })
                .ToList();
        }


    }
}
