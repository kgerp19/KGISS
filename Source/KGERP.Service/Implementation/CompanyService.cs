using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation
{
    public class CompanyService : ICompanyService
    {
        private readonly ERPEntities context;
        public CompanyService(ERPEntities context)
        {
            this.context = context;
        }
        public List<CompanyModel> GetCompanies(string searchText)
        {
            IQueryable<Company> companies = context.Companies.Where(x => (x.Name.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText)) || (x.ShortName.ToLower().Contains(searchText.ToLower())
            || string.IsNullOrEmpty(searchText))).OrderBy(x => x.OrderNo);
            List<CompanyModel> models = ObjectConverter<Company, CompanyModel>.ConvertList(companies.ToList()).ToList();
            return models;
        }

        public CompanyModel GetCompany(int id)
        {
            if (id <= 0)
            {
                return new CompanyModel()
                {
                    IsCompany = true,
                    IsActive = true,
                    CompanyLogo = "~/Images/Logo/logo.png"
                };
            }
            Company company = context.Companies.FirstOrDefault(x => x.CompanyId == id);
            return ObjectConverter<Company, CompanyModel>.Convert(company);
        }

        public List<SelectModel> GetCompanySelectModels()
        {
            return context.Companies.Where(x => x.IsCompany).OrderBy(x => x.Name).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.CompanyId
            }).ToList();
        }
        public List<SelectModel> GetCompanySelectModelsISS(int companyId)
        {
            return context.Companies.Where(x => x.IsCompany && x.CompanyId == companyId && x.IsActive).OrderBy(x => x.Name).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.CompanyId
            }).ToList();
        }

        public List<SelectModel> GetCompanySelectModelsForKSSl(int CompanyId)
        {
            return context.Companies.Where(x => x.IsCompany && x.CompanyId==CompanyId).OrderBy(x => x.Name).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.CompanyId
            }).ToList();
        }



        public List<SelectModel> GetAllCompanySelectModels()
        {
            return context.Companies.OrderBy(x => x.Name).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.CompanyId
            }).ToList();
        }

        public List<SelectModel> GetAllCompanySelectModels2()
        {
            return context.Companies.Where(f => f.IsDepartment == true).OrderBy(x => x.Name).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.CompanyId
            }).ToList();
        }

        public List<SelectModel> GetKGRECompnay()
        {

            return context.Database.SqlQuery<Company>("Select * from Company  where CompanyId  in (7,9) order by OrderNo").ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.CompanyId
            }).ToList();
        }

        public List<SelectModel> GetFilterCompanySelectModels()
        {
            return context.Database.SqlQuery<Company>("Select * from Company  where CompanyId not in (1,26,27,30,31,227) order by OrderNo").ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.CompanyId
            }).ToList();
        }

        public bool SaveCompany(int id, CompanyModel model)
        {
            if (model == null)
            {
                throw new Exception("Company data missing!");
            }

            Company company = ObjectConverter<CompanyModel, Company>.Convert(model);
            if (id > 0)
            {
                company = context.Companies.FirstOrDefault(x => x.CompanyId == id);
                if (company == null)
                {
                    throw new Exception("Company Menu not found!");
                }
                company.ModifiedDate = DateTime.Now;
                company.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;

                company.CompanyId = model.CompanyId;
                company.Name = model.Name;
                company.ShortName = model.ShortName;
                company.OrderNo = model.OrderNo;
                company.MushokNo = model.MushokNo;
                company.Address = model.Address;
                company.Phone = model.Phone;
                company.Fax = model.Fax;
                company.Email = model.Email;

                if (!string.IsNullOrEmpty(model.CompanyLogo))
                {
                    company.CompanyLogo = model.CompanyLogo;
                }
                company.IsCompany = model.IsCompany;
                company.IsActive = model.IsActive;
                return context.SaveChanges() > 0;
            }

            else
            {
                var maxSerialNo = context.Companies.Max(x => x.SerialNo)+1;

                company.CompanyId = context.Database.SqlQuery<int>("exec spGetNewCompanyId").FirstOrDefault();
                company.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                company.CreatedDate = DateTime.Now;

                company.Name = model.Name;
                company.ShortName = model.ShortName;
                company.OrderNo = model.OrderNo;
                company.SerialNo = maxSerialNo;
                company.MushokNo = model.MushokNo;
                company.Address = model.Address;
                company.Phone = model.Phone;
                company.Fax = model.Fax;
                company.Email = model.Email;
                if (!string.IsNullOrEmpty(model.CompanyLogo))
                {
                    company.CompanyLogo = model.CompanyLogo;
                }
                company.IsCompany = model.IsCompany;
                company.IsActive = model.IsActive;

                context.Companies.Add(company);
                return context.SaveChanges() > 0;
            }
        }

        public List<SelectModel> GetCompanySelectModelById(int companyId)
        {
            return context.Companies.Where(x => x.CompanyId == companyId).ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.CompanyId
            }).ToList();
        }

        public List<SelectModel> GetSaleYearSelectModel()
        {
            List<SelectModel> years = new List<SelectModel>();
            int beginYear = Convert.ToInt32(ConfigurationManager.AppSettings["SalesBeginYear"]);
            int endYear = DateTime.Today.Year;
            for (int i = beginYear; i <= endYear; i++)
            {
                years.Add(new SelectModel { Text = i.ToString(), Value = i.ToString() });
            }
            return years;
        }


        #region BusinessHead
        public bool SaveBusinessHead(BusinessHeadModel model)
        {
            var existHead = context.BusinessHeads.FirstOrDefault(x => x.BusinessType == (int)model.BusinessType && x.BusineesId_Fk == model.BusineesId_Fk && x.IsActive);
            var fromDate = model.FromDate;
            var toDate = model.ToDate;
            if (existHead != null)
            {
                if (existHead.FromDate > model.FromDate)
                {
                    return false;
                }
                if (existHead.ToDate == null && existHead.FromDate < model.FromDate)
                {
                    existHead.ToDate = model.FromDate.AddDays(-1);
                    existHead.ModifiedBy = Common.GetUserId();
                    existHead.ModifiedDate = DateTime.Now;
                    existHead.IsActive = false;
                }
            }
            BusinessHead businessHead = new BusinessHead()
            {
                BusineesId_Fk = model.BusineesId_Fk,
                IsActive = true,
                BusinessType = (int)model.BusinessType,
                FromDate = fromDate,
                ToDate = toDate,
                CreatedBy = Common.GetUserId(),
                CreatedDate = DateTime.Now,
                EmployeeId  = model.EmployeeId,
            };
            context.BusinessHeads.Add(businessHead);
            if (context.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }
        public bool UpdateBusinessHead(BusinessHeadModel model) 
        {
            var existHead = context.BusinessHeads.FirstOrDefault(x => x.BusinessType == (int)model.BusinessType && x.BusineesId_Fk == model.BusineesId_Fk && x.IsActive);

            if (existHead != null && existHead.FromDate != model.FromDate)
            {
                var previousExist = context.BusinessHeads.FirstOrDefault(x => x.BusinessType == existHead.BusinessType && x.BusineesId_Fk == existHead.BusineesId_Fk && !x.IsActive 
                && x.ToDate > model.FromDate);

                if (previousExist != null)
                {
                    if(previousExist.FromDate > model.FromDate) {
                        previousExist.FromDate = model.FromDate.AddDays(-1);
                    }
                    if (previousExist.ToDate > model.FromDate)
                    {
                        previousExist.ToDate = model.FromDate.AddDays(-1);
                    }
                }
            }

            if (existHead != null)
            {
                existHead.ToDate = model.FromDate.AddDays(-1);
                existHead.ModifiedBy = Common.GetUserId();
                existHead.ModifiedDate = DateTime.Now;
                existHead.FromDate = model.FromDate;
                existHead.EmployeeId = model.EmployeeId;
                existHead.BusinessType = (int)model.BusinessType;
                existHead.IsActive = true;
            }
            if (context.SaveChanges() > 0)
            {
                return true;
            }
            return false;

        }
        public bool RemoveBusinessHead(int id) 
        {
            var existHead = context.BusinessHeads.FirstOrDefault(x => x.Id == id && x.IsActive);
            if (existHead != null)
            {
                existHead.IsActive = false;
                existHead.ModifiedBy = Common.GetUserId();
                existHead.ModifiedDate = DateTime.Now;
            }
            if (context.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }
        public List<BusinessHeadModel> GetAllBusinessHead() 
        {
            var list = (from h in context.BusinessHeads
                        join e in context.Employees on h.EmployeeId equals e.Id
                       join c in context.Companies on h.BusineesId_Fk equals c.CompanyId into newCompany
                       from c in newCompany.DefaultIfEmpty()
                       join d in context.Departments on h.BusineesId_Fk equals d.DepartmentId into newDepartment
                       from d in newDepartment.DefaultIfEmpty()
                       where h.IsActive
                       select new BusinessHeadModel()
                       {
                           Id = h.Id,
                           EmployeeId = h.EmployeeId,
                           EmployeeName = e.Name,
                           FromDate = h.FromDate,
                           ToDate = h.ToDate,
                           BusineesId_Fk = h.BusineesId_Fk,
                           BusinessType = (BusinessTypeEnum)h.BusinessType,
                           IsActive = h.IsActive,
                           BusinessName = h.BusinessType == (int)BusinessTypeEnum.Company ? c.ShortName : d.Name
                       }).ToList();
            return list;
        }
        public BusinessHeadModel GetBusinessHead(int id) 
        {
            var data = (from h in context.BusinessHeads
                        join e in context.Employees on h.EmployeeId equals e.Id
                        join c in context.Companies on h.BusineesId_Fk equals c.CompanyId into newCompany
                        from c in newCompany.DefaultIfEmpty()
                        join d in context.Departments on h.BusineesId_Fk equals d.DepartmentId into newDepartment
                        from d in newDepartment.DefaultIfEmpty()
                        where h.IsActive && h.Id == id
                        select new BusinessHeadModel()
                        {
                            Id= h.Id,
                            EmployeeId = h.EmployeeId,
                            EmployeeName = e.Name,
                            FromDate = h.FromDate,
                            ToDate = h.ToDate,
                            BusineesId_Fk = h.BusineesId_Fk,
                            BusinessType = (BusinessTypeEnum)h.BusinessType,
                            BusinessTypeInt = h.BusinessType,
                            IsActive = h.IsActive,
                            BusinessName = h.BusinessType == (int)BusinessTypeEnum.Company ? c.ShortName : d.Name
                        }).FirstOrDefault();
            return data;
        }

        public async Task<List<SelectListItem>> GetDDLPayrollShit(int ComId,CancellationToken cancellationToken)
        {
            var result = context.PRoll_Sheet.Where(x => x.IsActive && x.CompanyId==ComId).Select(x => new SelectListItem()
            {
                Text=x.Name,
                Value=x.PayRollSheetId.ToString()
            });

            return await result.ToListAsync(cancellationToken);
        }

        public async Task<long?> GetPayrollSheetIdByEmpId(long EmpId, CancellationToken cancellationToken = default)
        {
            var employee = await context.Employees.FirstOrDefaultAsync(x => x.Id == EmpId);
            if (employee != null)
            {
                return employee.PRollSheetId;
            }
            return null; // or any default value you prefer if employee is not found
        }


        #endregion BusinessHead
    }
}

