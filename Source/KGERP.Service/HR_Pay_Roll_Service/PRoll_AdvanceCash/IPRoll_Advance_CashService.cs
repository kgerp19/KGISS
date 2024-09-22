using KGERP.Service.Configuration;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_AdvanceCash
{
    public interface IPRoll_Advance_CashService
    {
        Task<long> AddAdvance(PRoll_AdvanceViewModel model);
        Task<long> UpdatPRoll_AdvanceViewModeleAdvance(PRoll_AdvanceViewModel model);
        Task<PRoll_AdvanceViewModel> AdvanceDetalis(long id);
        Task<PRoll_AdvanceViewModel> AdvanceList(int companyId);
        Task<int> AdvanceDelete(long id);
        Task<List<SelectModelAdvanceType>> GetAdvanceTypeType(int companyId);

        Task<VMAdvanceType> GetAdvanceType(int companyId);
        Task<int> AdvanceTypeAdd(VMAdvanceType vMAdvanceType);
        Task<int> AdvanceTypeEdit(VMAdvanceType vMAdvanceType);
        Task<int> AdvanceTypeDelete(int id);
    }
}
