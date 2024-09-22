using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IPaymentService
    {
        List<PaymentModel> GetPaymentsByVendor(int vendorId);
        Task<PaymentModel> GetPayment(int id);
        List<SelectModel> GetPaymentMethodSelectModels();
        Task<PaymentModel> GetPayments(int companyId, DateTime? fromDate, DateTime? toDate);
      
        bool SavePayment(int paymentId, PaymentModel payment, out string message);
        bool SaveVendor(int id, VendorModel vendor, out string message);
    }
}
