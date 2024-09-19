using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.SpecialAddition
{
    public interface ISpecialAdditionServices
    {
        Task<SpecialAdditionViewModel> SpecialAddtnList(int CompanyId);
        Task<SpecialAdditionViewModel> SpAddDetalis(long id);
        Task<long> AddSpecialAddition(SpecialAdditionDetailsViewModel item);
        Task<long> Delete(long id);
        Task<long> UpdateBillDetalis(SpecialAdditionDetailsViewModel model);
        Task<long> AddSpecialAddition(SpecialAdditionViewModel model);
        List<EmployeeInfoVM> GetSpecialAdd(int companyId, int? year, int? month);
    }
}
