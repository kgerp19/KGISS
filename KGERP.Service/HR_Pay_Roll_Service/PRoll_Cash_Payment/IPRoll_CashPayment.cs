using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_Cash_Payment
{
    public interface IPRoll_CashPayment
    {
        Task<long> AddCashPayment(PRoll_CashPaymentViewModel model);
        Task<PRoll_CashPaymentViewModel> CashPaymentList(int companyId);
        Task<PRoll_CashPaymentViewModel> Detalis(long id);
        Task<long> Delete(long id);
    }
}
