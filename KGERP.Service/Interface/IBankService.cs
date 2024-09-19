using KGERP.Data.Models;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IBankService
    {
        List<Bank> GetBanks();
        List<SelectModel> GetBankSelectModels();
        List<SelectModel> GetBankSelectModels(int CompanyId);
    }
}
