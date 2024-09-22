using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.SMS_Service_Implementation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KGERP.Service.Implementation.SMSService
{
   public class SMSServices :ISmsServices
    {
    
        //string Apikey = "KEY-czvwjcex2sdevdgf390x7k20vs7eeo4t";
        //string ApiSecret = "YnRUb0JH38cKmI@n";
        //string BaseUrl = "https://portal.adnsms.com";

        private readonly ERPEntities _db;
        public SMSServices( ERPEntities entities)
        {
            this._db = entities;
        }



        //faild

        public async Task<List<RealStateMessage>> getRSMSList(int companyId)
        {
            List<RealStateMessage> messages = await _db.RealStateMessages.Where(f => f.IsActive == true && f.CompanyId == companyId).ToListAsync();
            return messages;
        }
        public async Task<RealStateMessage> Getbyrealsms(long id)
        {
            RealStateMessage res = _db.RealStateMessages.FirstOrDefault(f => f.ID == id);
          return res;
        } 
        public async Task<RealStateMessage> GetUpdatesms(long id)
        {
            RealStateMessage res = _db.RealStateMessages.FirstOrDefault(f => f.ID == id);
            res.Status = 100;
            res.DeliveryDate= DateTime.Now;
            _db.Entry(res).State = EntityState.Modified;
            _db.SaveChanges();
            return res;
        }
        public async Task<List<vwSMSList>> GetSmsList(int status)
        {
            if (status==99)
            {
                var res= await _db.vwSMSLists.ToListAsync();
                return res;
            }
            else
            {
                var result = await _db.vwSMSLists.Where(s=>s.Status==status).ToListAsync();
                return result;
            }
        }
        public async Task<List<vwSMSList>> GetSmsCompanyWise(int Status,int companyId)
        {
            return await _db.vwSMSLists.Where(e => e.Status == Status&&e.CompanyId==companyId).ToListAsync();
        }


        public async Task<SmsListVm> GetSmsCompanyWiseList(int companyId ,int Type, DateTime? fromDate, DateTime? toDate)
        {
            SmsListVm vm = new SmsListVm();
            vm.CompanyId = companyId;
            vm.type = Type;
            vm.FromDate = fromDate.Value;
            vm.ToDate = toDate.Value;
            vm.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vm.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            vm.SmsTypeList =await GetSmsTypeList();
            vm.DataList= await _db.vwSMSLists
                .Where(q=>q.CompanyId == companyId
                && q.Date>fromDate && q.Date < toDate
                &&(Type==0?q.SmsType>0:q.SmsType==Type)
                )
                .Select(s=> new SmsVm
                {
                    CompanyId = companyId,
                    Status = s.Status,
                    CompanyName=s.CompanyName,
                    Date=s.Date,
                    Subject=s.Subject,
                    PhoneNo=s.PhoneNo,
                    Message=s.Message,
                    TryCount=s.TryCount,
                    RowTime =s.RowTime,
                    Remarks=s.Remarks,
                    SmsType=s.SmsType,
                    SMSTypeName=s.SMSTypeName
                }).OrderByDescending(o=>o.Date)
                .ToListAsync();

                return vm;
        }


        //Send Sms  
        public async Task<bool> SendSms(ErpSMS erpSM)
        {
            int r = -1;
            //ErpSM sM = MapModel(erpSM);
            _db.ErpSMS.Add(erpSM);

            try
            {
                r = await _db.SaveChangesAsync();
                return r > 0;
            }
            catch (Exception)
            {

                return false;
            }
         
        }

        public async Task<List<SmsType>> GetSmsTypeList()
        {
            return await _db.SmsTypes.Where(e => e.IsActive == true).ToListAsync();

        }
        public async Task<SmsType> GetSmsTypeById(int id)
        {
            return await _db.SmsTypes.Where(e => e.IsActive == true && e.Id == id).SingleOrDefaultAsync();

        }
        public async Task<bool>SendSms(List<ErpSMS> erpSM)
        {
            int r = -1;
            _db.ErpSMS.AddRange(erpSM);
            try
            {
                r = await _db.SaveChangesAsync();
                return r > 0;
            }
            catch (Exception ex)
            {

                return false;
            }
        }


        private ErpSMS MapModel(ErpSMS erpSM)
        {
            ErpSMS model = new ErpSMS();
            model.Message = erpSM.Message;
            model.PhoneNo = erpSM.PhoneNo;
            model.Status = erpSM.Status;
            model.SmsType = erpSM.SmsType;
            model.Date = erpSM.Date;
            model.Remarks = erpSM.Remarks;
            model.Subject = erpSM.Subject;
            return model;
        }

        public async Task<RealStateMessageVM> getRSMSListnew(int companyId)
        {
            RealStateMessageVM vM = new RealStateMessageVM();   
            List<RealStateMessage> messages = await _db.RealStateMessages.Where(f => f.IsActive == true && f.CompanyId == companyId).OrderByDescending(f=>f.CreateDate).ToListAsync();
         vM.CompanyId = companyId;
            vM.reals = messages;    
            return vM;
        }

        public async Task<long> AddRSMSListnew(RealStateMessageVM real)
        {
            RealStateMessage message = new RealStateMessage();
            if (real.Tag==1)
            {
                message.Address = real.vM.Address;
                message.Message = real.vM.Message;
            }
            else
            {
                message.Address = real.Address;
                message.Message = real.Message;
            }
            message.CompanyId =real.CompanyId;
            message.Agenda = 2;

            message.MediaType = 1;
            message.CreateDate = DateTime.Now;
            message.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            message.Status = 1;
            message.IsActive = true;
            _db.RealStateMessages.Add(message);
           await _db.SaveChangesAsync();
            return message.ID;
        }



        public async Task<List<MyStoredProcedureResult>> ExecuteStoredProcedure(DateTime fromDate, DateTime toDate,string userName,int CompanyId, int SelectedOption)
        {

           

            // Call the stored procedure with the provided parameters
            var result = await _db.Database.SqlQuery<MyStoredProcedureResult>(
                "EXEC [dbo].[GetPeriodicInstallment] @StrFromDate, @StrToDate, @CompanyId, @EmployeeId, @SelectedOption",
                new SqlParameter("@StrFromDate", fromDate),
                new SqlParameter("@StrToDate", toDate),
                new SqlParameter("@CompanyId", CompanyId),
                new SqlParameter("@EmployeeId", userName),
                new SqlParameter("@SelectedOption", SelectedOption)
            ).ToListAsync();

            return result;
        }

        public async Task<bool> ExecuteStoredProcedureForInsert(DateTime fromDate, DateTime toDate, string userName, int CompanyId, int SelectedOption)
        {
            try
            {
                await _db.Database.ExecuteSqlCommandAsync(
                    "EXEC [dbo].[InsertPeriodicInstallmentToRealStateMessage] @StrFromDate, @StrToDate, @CompanyId, @EmployeeId, @SelectedOption",
                    new SqlParameter("@StrFromDate", fromDate),
                    new SqlParameter("@StrToDate", toDate),
                    new SqlParameter("@CompanyId", CompanyId),
                    new SqlParameter("@EmployeeId", userName),
                    new SqlParameter("@SelectedOption", SelectedOption)
                );

                // If execution reaches here, it means the stored procedure executed successfully
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                Console.WriteLine(ex.Message);

                // If an exception occurs during execution, return false
                return false;
            }
        }


    }
}
