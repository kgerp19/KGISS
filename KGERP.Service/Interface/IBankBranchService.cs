using KGERP.Data.Models;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IBankBranchService
    {
        List<BankBranch> GetBankBranches();
        List<SelectModel> GetBankBranchSelectModels();
        List<SelectModel> GetBankBranchByBank(int bankId);
        List<SelectModel> GetBankBranchByCompanyId(int bankId);
    }
}
