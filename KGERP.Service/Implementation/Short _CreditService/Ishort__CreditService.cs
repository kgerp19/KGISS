using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Short__CreditService
{
    public interface Ishort__CreditService
    {
        Task<ShortCreditViewModel> AddPayment(ShortCreditViewModel model);
        Task<ShortCreditViewModel> GetShortCreditCollectedAmount(int companyId, long sCreditCollectionId);
        Task<long> SubmitCollectionMasters(ShortCreditViewModel model);
        VMFeedPayment GetShortList(int Vendrid);
    }
}
