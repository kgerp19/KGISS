using KGERP.Data.Models;
using KGERP.Service.CommonModels.Model;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_SalaryConfiguration_Services
{
    public class PRoll_SalaryConfigurationServices : IPRoll_SalaryConfiguration
    {
        ERPEntities context = new ERPEntities();
        public async Task<long> AddSalaryConfiguration(PRoll_SalaryConfigurationViewModel model)
        {
            long result = -1;
            var dbResult = await context.PRoll_SalaryConfiguration.FirstOrDefaultAsync(d => d.EmployeeId == model.EmployeeId && d.IsActive == true);

            if (dbResult == null)
            {
                PRoll_SalaryConfiguration pRoll = new PRoll_SalaryConfiguration();
                pRoll.EmployeeId = model.EmployeeId;
                pRoll.IsActive = true;
                pRoll.CompanyId = model.CompanyId;
                pRoll.Basic = model.Basic;
                pRoll.HouseRent = model.HouseRent;
                pRoll.Transportation = model.Transportation;
                pRoll.Medical = model.Medical;
                pRoll.MobileBill = model.MobileBill;
                pRoll.Welfare_D = model.Welfare_D;
                pRoll.ProvidentFund_D = 10;
                pRoll.Tax_D = model.Tax_D;
                pRoll.Rent_D = model.Rent_D;
                pRoll.IsBonusAllowed = model.IsBonusAllowed;
                pRoll.TransportationAddition = model.TransportationAddition;
                pRoll.Gross = model.Gross;
                pRoll.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                pRoll.CreatedDate = DateTime.Now;
                context.PRoll_SalaryConfiguration.Add(pRoll);


                if (await context.SaveChangesAsync() > 0)
                {

                    var dbEmployeeData = await context.Employees.FirstOrDefaultAsync(x => x.Id == model.EmployeeId);
                    dbEmployeeData.PRollSheetId = model.RollSheetId;
                    context.Entry(dbEmployeeData).State = EntityState.Modified;
                    if (await context.SaveChangesAsync() > 0)
                    {
                        result = pRoll.SalaryConfigurationId;
                    }

                }
            }
            return result;
        }

        public async Task<int> DeleteSalaryConfiguration(long id)
        {
            var pRoll = await context.PRoll_SalaryConfiguration.FirstOrDefaultAsync(d => d.SalaryConfigurationId == id && d.IsActive == true);
            pRoll.IsActive = false;
            pRoll.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            pRoll.ModifiedDate = DateTime.Now;
            context.Entry(pRoll).State = EntityState.Modified;
            context.SaveChanges();
            return pRoll.CompanyId;
        }

        public async Task<PRoll_SalaryConfigurationViewModel> SalaryConfigurationDetalis(long id)
        {
            PRoll_SalaryConfigurationViewModel mobile = new PRoll_SalaryConfigurationViewModel();
            mobile = await Task.Run(() => (from t1 in context.PRoll_SalaryConfiguration
                                           join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                           join t3 in context.Employees on t1.EmployeeId equals t3.Id
                                           where t1.IsActive == true && t1.SalaryConfigurationId == id

                                           select new PRoll_SalaryConfigurationViewModel
                                           {
                                               RollSheetId = t3.PRollSheetId,
                                               IsBonusAllowed = t1.IsBonusAllowed,
                                               SalaryConfigurationId = t1.SalaryConfigurationId,
                                               Gross = t1.Gross,
                                               Basic = t1.Basic,
                                               Medical = t1.Medical,
                                               HouseRent = t1.HouseRent,
                                               Transportation = t1.Transportation,
                                               MobileBill = t1.MobileBill,
                                               Welfare_D = t1.Welfare_D,
                                               Tax_D = t1.Tax_D,
                                               ProvidentFund_D = t1.ProvidentFund_D,
                                               EmployeeId = t1.EmployeeId,
                                               EmployeeName = t3.Name,
                                               CompanyId = t2.CompanyId,
                                               CompanyName = t2.Name,
                                               TransportationAddition = t1.TransportationAddition,
                                               Rent_D = t1.Rent_D,

                                               CreatedDate = t1.CreatedDate,
                                               CreatedBy = t1.CreatedBy
                                           }).FirstOrDefaultAsync());
            return mobile;
        }

        public async Task<PRoll_SalaryConfigurationViewModel> SalaryConfigurationList(int companyId)
        {
            string CompanyName = "";
            var company = context.Companies.Where(x => x.CompanyId == companyId).FirstOrDefault();
            CompanyName = company != null ? company.Name : "All Companies";
            PRoll_SalaryConfigurationViewModel mobile = new PRoll_SalaryConfigurationViewModel();
            mobile.CompanyName = CompanyName;
            mobile.CompanyId = companyId;
            mobile.SalaryConfigurationList = await Task.Run(() => (from t1 in context.PRoll_SalaryConfiguration
                                                                   join t2 in context.Companies on t1.CompanyId equals t2.CompanyId
                                                                   join t3 in context.Employees on t1.EmployeeId equals t3.Id
                                                                   join t4 in context.Designations on t3.DesignationId equals t4.DesignationId
                                                                   where t1.IsActive == true && t1.CompanyId == companyId
                                                                   select new PRoll_SalaryConfigurationViewModel
                                                                   {
                                                                       SalaryConfigurationId = t1.SalaryConfigurationId,
                                                                       Gross = t1.Gross,
                                                                       Basic = t1.Basic,
                                                                       Medical = t1.Medical,
                                                                       HouseRent = t1.HouseRent,
                                                                       Transportation = t1.Transportation,
                                                                       MobileBill = t1.MobileBill,
                                                                       Welfare_D = t1.Welfare_D,
                                                                       Tax_D = t1.Tax_D,
                                                                       EmployeeId = t1.EmployeeId,
                                                                       EmployeeName = t3.EmployeeId + " - " + t3.Name + " - " + t4.Name,
                                                                       CompanyId = t2.CompanyId,
                                                                       TransportationAddition = t1.TransportationAddition,
                                                                       Rent_D = t1.Rent_D,
                                                                       CompanyName = t2.Name,
                                                                       CreatedDate = t1.CreatedDate,
                                                                       CreatedBy = t1.CreatedBy,
                                                                       ProvidentFund_D = t1.ProvidentFund_D,
                                                                       IsBonusAllowed = t1.IsBonusAllowed
                                                                   }).OrderByDescending(f => f.EmployeeId).AsQueryable());
            return mobile;
        }

        public async Task<long> UpdateSalaryConfiguration(PRoll_SalaryConfigurationViewModel model)
        {
            long result = -1;
            PRoll_SalaryConfiguration pRoll = await context.PRoll_SalaryConfiguration.FirstOrDefaultAsync(d => d.SalaryConfigurationId == model.SalaryConfigurationId && d.IsActive == true);
            if (pRoll != null)
            {
                pRoll.EmployeeId = model.EmployeeId;
                pRoll.CompanyId = model.CompanyId;
                pRoll.Basic = model.Basic;
                pRoll.HouseRent = model.HouseRent;
                pRoll.Transportation = model.Transportation;
                pRoll.Medical = model.Medical;
                pRoll.MobileBill = model.MobileBill;
                pRoll.Welfare_D = model.Welfare_D;
                pRoll.Tax_D = model.Tax_D;
                pRoll.IsBonusAllowed = model.IsBonusAllowed;
                pRoll.Gross = model.Gross;
                pRoll.ProvidentFund_D = model.ProvidentFund_D;
                pRoll.TransportationAddition = model.TransportationAddition;
                pRoll.Rent_D = model.Rent_D;
                pRoll.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                pRoll.ModifiedDate = DateTime.Now;
                context.Entry(pRoll).State = EntityState.Modified;
                if (await context.SaveChangesAsync() > 0)
                {
                    var dbEmployeeData = await context.Employees.FirstOrDefaultAsync(x => x.Id == model.EmployeeId);
                    dbEmployeeData.PRollSheetId = model.RollSheetId;
                    context.Entry(dbEmployeeData).State = EntityState.Modified;
                    if (await context.SaveChangesAsync() > 0)
                    {
                        result = pRoll.SalaryConfigurationId;
                    }
                }
            }
            return result;
        }
    }
}
