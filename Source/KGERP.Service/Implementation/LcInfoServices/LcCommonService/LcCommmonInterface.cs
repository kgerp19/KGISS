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
    public interface LcCommmonInterface
    {
        List<SelectModelType> GetAllBanks(int companyId);
        List<SelectModelType> GetAdvanceAndLoan(int companyId);
        List<SelectModelType> GetAllVendorList(int companyId);
        List<SelectModelType> GetAllContry();
        List<SelectModelType> GetByCountryIdPortOfCountry(int countryId);
        List<SelectModelType> GetAllCurrencyList();
        CompanyModel GetByCompany(int id);
        Currency GetAllCurrencyRate(int Id);

    }
}
