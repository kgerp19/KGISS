using KGERP.Service.HR_Pay_Roll_Service.PRoll_Cash_Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_SalaryConfiguration_Services
{
   public interface IPRoll_SalaryConfiguration
    {
        Task<long> AddSalaryConfiguration(PRoll_SalaryConfigurationViewModel model);
        Task<long> UpdateSalaryConfiguration(PRoll_SalaryConfigurationViewModel model);
        Task<int> DeleteSalaryConfiguration(long id);
        Task<PRoll_SalaryConfigurationViewModel> SalaryConfigurationList(int companyId);
        Task<PRoll_SalaryConfigurationViewModel> SalaryConfigurationDetalis(long id);
    }
}
