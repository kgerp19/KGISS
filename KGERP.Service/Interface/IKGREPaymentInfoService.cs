using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IKGREPaymentInfoService
    {
        List<KGREPaymentModel> GetPaymentsByCustomer(int customerId);
        Task<KGREPaymentModel> GetPayment(int id);
        List<SelectModel> GetPaymentMethodSelectModels();
        List<KGREPaymentModel> GetPayments(string searchDate, string searchText, int companyId);
        bool SavePayment(int paymentId, KGREPaymentModel payment, out string message);
        bool SaveCustomer(int id, KgReCrmModel vendor, out string message);

        //PlotBooking LoadPayInfoById(string Autoid);
        //object LoadClientPayInfoById(string id);
        //void SaveOfficeInfo(PlotBooking ClientOffInfo);
        //List<DateTime> CalculateNextPayDateFrom1(string id);
        //object ClientPaymentHistory(string id);
        //void Edit(float PayDueAmount, int Id);

    }
}
