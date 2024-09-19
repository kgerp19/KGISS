using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Service.SMS_Service_Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface ISmsServices
    {
        Task<List<vwSMSList>> GetSmsList(int status);


        Task<List<vwSMSList>> GetSmsCompanyWise(int Statusi, int companyId);
        Task<SmsListVm> GetSmsCompanyWiseList(int companyId, int Type, DateTime? fromDate, DateTime? toDate);

        //Send Sms  
        Task<bool> SendSms(ErpSMS erpSM);


        Task<bool> SendSms(List<ErpSMS> erpSM);
        Task<List<SmsType>> GetSmsTypeList();
        Task<SmsType> GetSmsTypeById(int id);
        Task<List<RealStateMessage>> getRSMSList(int companyId);
        Task<RealStateMessageVM> getRSMSListnew(int companyId);
        Task<List<MyStoredProcedureResult>> ExecuteStoredProcedure(DateTime fromDate, DateTime toDate, string userName, int CompanyId, int SelectedOption);
        Task<bool> ExecuteStoredProcedureForInsert(DateTime fromDate, DateTime toDate, string userName, int CompanyId, int SelectedOption);
        Task<long> AddRSMSListnew(RealStateMessageVM real);
        Task<RealStateMessage> Getbyrealsms(long id);
        Task<RealStateMessage> GetUpdatesms(long id);
    }
}
