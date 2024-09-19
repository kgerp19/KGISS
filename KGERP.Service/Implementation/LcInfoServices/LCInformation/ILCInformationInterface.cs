using KGERP.Data.Models;
using KGERP.Services.Procurement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.LcInfoServices.LCInformation
{
    public interface ILCInformationInterface
    {
        long AddLC(LcInfoViewModel model);
        LcInfoViewModel LCDetails(long id);
        LcInfoViewModel LCList(int companyId);
        int LCDelete(LcInfoViewModel model);
        Task<int> LCSubmit(LcInfoViewModel model);
        Task<int> LCAmendmentSubmit(LcInfoViewModel model);
        Task<int> LCValueAmendmentSubmit(LcInfoViewModel model);

        VMLCAmendment AmendmentDetails(long LCAmendmentId);
        long UpdateLC(LcInfoViewModel model);
        Task<long> LCAmendmentSave(LcInfoViewModel model);
        Task<long> LCValueAmendmentSave(LcInfoViewModel model);
        LcValueAmmendmentvm GetLcAmmendmentValue(long amendmentId);
        bool DeleteAmmendMendValue(int ammendmendid);
        LCAmendment GetLCAmendment(long amendmentId);
        bool DeleteLCAmendment(int amendmentID);
    }
}
