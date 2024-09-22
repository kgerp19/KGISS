using KGERP.Data.Models;
using KGERP.Service.Interface;
using System;

namespace KGERP.Service.ServiceModel
{
    public class AccountSignatoryService : IAccountSignatoryService
    {
        private readonly ERPEntities context;
        public AccountSignatoryService(ERPEntities context)
        {
            this.context = context;
        }

        public bool SaveSignatory(int id, Accounting_Signatory model)
        {
            throw new NotImplementedException();
        }
    }
}
