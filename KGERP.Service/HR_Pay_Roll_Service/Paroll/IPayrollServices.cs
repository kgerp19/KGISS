using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.ParollServices
{
    public interface IPayrollServices
    {
       
        Task<VmPRollPayRoll> PayRollByCompanyId(int companyId);
        Task<long> AddPayRoll(VmPRollPayRollDetail model);
        Task<PRoll_BonusVm> PayRollBonus(int companyId);
        Task<long> AddPayRollBonus(PRoll_BonusVm model);
        int ProcessPayroll(int CompanyId, long PayRollId, int Month, int Year);
        Task<PRoll_Bonus> DeleteBonus(long bonusid);
        Task<PRoll_BonusVm> EditPayRollBonus(long bonusid);
        Task<PRoll_Bonus> UpdatePayRollBonus(PRoll_BonusVm model);
        Task<VmSalaryDetails> CompanyWiseSalaryDetails(int companyId, long PayRollId);
        Task<VmSalaryDetails> CompanyWiseBonusDetails(int companyId, long PayRollId);
        Task<long> ProcessBankSheetAndPayment(VmSalaryDetails model);
        Task<PRoll_AdvanceTypeVm> PayRollAdvanceType(int companyId);
        Task<int> AddPayRollAdvanceType(PRoll_AdvanceTypeVm model);
        Task<PRoll_AdvanceTypeVm> Edit(int? Advanceid);
        Task<bool> DeleteAdvanceType(int AdvanceTypeId);
    }
}
