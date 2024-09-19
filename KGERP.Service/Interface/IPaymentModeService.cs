using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IPaymentModeService
    {
        //List<PaymentMode> GetPaymentModes();
        //List<SelectModel> GetPaymentSelectModels();
        List<SelectModel> GetPaymentModeSelectModels();
        List<SelectModel> GetPaymentReceiveSelectModels();
        List<SelectModel> PaymentModes();
    }
}
