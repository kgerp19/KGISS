using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.Hr_PRoll_FineDeducation
{
    public interface IFineDeductionServices
    {
        Task<long> AddDeduction(FineDeductionVm model);
        Task<FineDeductionVm> Detalis(long id);
        Task<long> AddFIneDetalis(FineDeductionDetailsViewModel item);
        Task<long> UpdateFineDetalis(FineDeductionDetailsViewModel model);
        Task<long> Delete(long id);
        Task<FineDeductionVm> FineList(int companyId);
    }
}
