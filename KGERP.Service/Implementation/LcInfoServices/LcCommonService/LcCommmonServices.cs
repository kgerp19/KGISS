using KGERP.Data.Models;
using KGERP.Service.Implementation.CurrencyCon;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.LcInfoServices.LcCommonService
{
    public class LcCommmonServices : LcCommmonInterface
    {
        private readonly ERPEntities context;
        public LcCommmonServices(ERPEntities context)
        {
            this.context = context;
        }
        public List<SelectModelType> GetAllBanks(int companyId)
        {
            List<SelectModelType> selectModelLiat = new List<SelectModelType>();

            var result = (from t1 in context.BankBranches
                          join t2 in context.Banks on t1.BankId equals t2.BankId where t1.CompanyId == companyId
                          select new SelectModelType
                          {
                              Text = t2.Name + "-" + t1.Name + " (" + t2.ShortName + "A/C:" + t1.AccountNumber + ")",
                              Value = t1.BankBranchId
                          }).ToList();


            selectModelLiat.AddRange(result);
            return selectModelLiat;
        }

        public List<SelectModelType> GetAdvanceAndLoan(int companyId)
        {
            List<SelectModelType> selectModelLiat = new List<SelectModelType>();

            var result = (from t1 in context.HeadGLs
                          join t2 in context.Head5 on t1.ParentId equals t2.Id
                          where t1.CompanyId == companyId &&
                          (t2.AccCode == "1201001001" || t2.AccCode == "2301001002") // Investment & General Loan
                          select new SelectModelType
                          {
                              Text = "[" + t1.AccCode + "] " + t1.AccName,
                              Value = t1.Id
                          }).ToList();


            selectModelLiat.AddRange(result);
            return selectModelLiat;
        }


        public List<SelectModelType> GetAllContry()
        {
            List<SelectModelType> selectModelLiat = new List<SelectModelType>();
            var result = context.Countries.Select(x => new SelectModelType()
            {
                Text = x.CountryName,
                Value = x.CountryId
            }).ToList();
            selectModelLiat.AddRange(result);
            return selectModelLiat;
        }

        public List<SelectModelType> GetAllCurrencyList()
        {
            List<SelectModelType> selectModelLiat = new List<SelectModelType>();
            var result = context.Currencies.Where(d => d.IsActive == true).Select(x => new SelectModelType()
            {
                Text = x.Name,
                Value = x.CurrencyId
            }).ToList();
            selectModelLiat.AddRange(result);
            return selectModelLiat;
        }

        public Currency GetAllCurrencyRate(int Id)
        {

            var data = context.Currencies.Where(x => x.CurrencyId == Id).FirstOrDefault();
            return data;
        }









        public List<SelectModelType> GetAllVendorList(int companyId)
        {
            List<SelectModelType> selectModelLiat = new List<SelectModelType>();
            var result = context.Vendors.Where(v => v.CompanyId == companyId && v.IsActive == true && v.VendorTypeId == (int)Provider.Supplier).Select(x => new SelectModelType()
            {
                Text = x.Name,
                Value = x.VendorId
            }).ToList();
            selectModelLiat.AddRange(result);
            return selectModelLiat;
        }

        public List<SelectModelType> GetByCountryIdPortOfCountry(int countryId)
        {
            List<SelectModelType> selectModelLiat = new List<SelectModelType>();
            var result = context.PortOfCountries.Where(v => v.CountryId == countryId && v.IsActive == true).Select(x => new SelectModelType()
            {
                Text = x.PortName,
                Value = x.PortOfCountryId
            }).ToList();
            selectModelLiat.AddRange(result);
            return selectModelLiat;
        }

        public CompanyModel GetByCompany(int id)
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



        
    }
}
