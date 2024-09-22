using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Utility;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class BankService : IBankService
    {
        ERPEntities bankRepository = new ERPEntities();
        public List<Bank> GetBanks()
        {
            return bankRepository.Banks.ToList();
        }

        public List<SelectModel> GetBankSelectModels()
        {
            return bankRepository.Banks.ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.BankId.ToString()
            }).ToList();
        }
        public List<SelectModel> GetBankSelectModels(int CompanyId)        
        {
            return bankRepository.Banks.ToList().Where(x => x.CompanyId == CompanyId).Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.BankId.ToString()
            }).ToList();
        }
        public List<SelectModel> GetRegionSelectModels(int CompanyId)
        {
            return bankRepository.Banks.ToList().Where(x => x.CompanyId == CompanyId).Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.BankId.ToString()
            }).ToList();
        }
    }
}
