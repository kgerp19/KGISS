using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.PayRoll_Payment_Purpose_service
{
    public interface IPaymentPurposeServices
    {

        Task<PRoll_PaymentPurposeVm> GetPurpose();
        Task<long> AddPurpose(PRoll_PaymentPurposeVm model);
        Task<PRoll_PaymentPurposeVm> EditPayPurpose(int PayPurposeId);
        Task<PRoll_PaymentPurpose> UpdatePayPurpose(PRoll_PaymentPurposeVm model);
        Task<PRoll_PaymentPurpose> DeletePurpose(int PayPurposeId);

    }
}
