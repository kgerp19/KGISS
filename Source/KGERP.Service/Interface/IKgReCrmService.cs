using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IKgReCrmService : IDisposable
    {
        Task<KgReCrmModel> GetCommonClient(int companyId);
        Task<bool> IsExistingMobileNo(string mobileNo, string mobileNo2,int compantId);
        Task<KgReCrmModel> CreateNewClient(KgReCrmModel model);

        List<SelectModel> GetKGREClient();

        List<KgReCrmModel> GetKGREExistingLeadList(string searchText);
        List<KgReCrmModel> GetKGREClient(string searchText);
        List<KgReCrmModel> GetKGREClientList();
        KgReCrmModel GetFileNo(int ProjectId);
        Task<List<SelectModel>> GetKGREClient( int companyId);
        List<SelectModel> GetProjects(int? companyId);
        KgReCrmModel GetKGRClientById(int? id);
        VendorModel GetVendorById(int? id);
        List<KgReCrmModel> GetKGREClientEvent();
        List<KgReCrmModel> GetKGREClientFollowup();
        List<KgReCrmModel> GetKGRELeadList(string searchText);
        List<KgReCrmModel> GetKGRENewLeadList(string searchText);
        List<KgReCrmModel> GetPrevious7DaysClientSchedule();
        bool SaveKGREClient(int id, KgReCrmModel model);
        bool SaveKGREClientBooking(int id, KgReCrmModel model);
        bool DeleteKGREClient(int id);
        //List<KgReCrmModel> DailyDealingOfficerActivity( DateTime fromDate, DateTime toDate, string emoloyeeId);
        List<KgReCrmModel> DailyDealingOfficerActivity(string fromDate, string toDate, string emoloyeeId);
        object LoadBookingListInfo();
        Product Getbyproduct(int productId);
        VMCommonSupplier GetCustomer(int companyId);
        int  CustomerHeadUpdate(VMCommonSupplier vM);
        List<SelectModel> GetMappingCustomer(int companyId);
 
        object LoadBookingListPaymentInfo();
        object GetCustomerAutoComplete(string prefix, int companyId);
        object AutoCompleteByFileNo(string prefix, int companyId);
        KGREPlotBookingModel GetClientPaymentStatus(int customerId);
        ProductInfoVm GetbyproductForGldl(int productId);
       List<object> IncomeHeadGLList(int companyId);
    }
}
