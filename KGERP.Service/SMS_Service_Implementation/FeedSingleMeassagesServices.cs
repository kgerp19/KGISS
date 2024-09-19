using KGERP.Data.Models;
using KGERP.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.SMS_Service_Implementation
{
    public class FeedSingleMeassagesServices
    {
        private ERPEntities _db = new ERPEntities();
        public async Task<int> FeedSingleMeassagesAsync(ErpSMS lstSms)
        {
            try
            {
                //var lstSms = await _db.ErpSMS.FirstOrDefaultAsync(e => (e.Status == (int)EnumSmSStatus.Pending || e.Status == (int)EnumSmSStatus.Failed)
                //    && e.TryCount < 3 && e.CompanyId == 8&&e.Id==id);
                HttpClient _client = new HttpClient();
                _client.BaseAddress = new Uri("https://portal.adnsms.com");
                if (lstSms!=null)
                {
                        var requestBody = new
                        {
                            api_key = "KEY-zfdz6nbcdsyugisj4suogf6lej6bkv6z",
                            api_secret = "@PZMc3n@D0o77y9P",
                            request_type = "SINGLE_SMS",
                            message_type = "TEXT",
                            campaign_title = "Test",
                            mobile = lstSms.PhoneNo,
                            message_body = lstSms.Message
                        };
                        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                        HttpResponseMessage resp = await _client.PostAsync("/api/v1/secure/send-sms", content);
                        if (resp.IsSuccessStatusCode)
                        {
                        lstSms.Status = 9;
                        }
                        else
                        {
                        lstSms.TryCount = 1;
                        }
                        _db.Entry(lstSms).State = EntityState.Modified;
                        _db.SaveChanges();
                    
                    SMSScheduleLog smsScheduleLog = new SMSScheduleLog();
                    var logcount = 1;
                    smsScheduleLog.Remarks = "Successfully Send Massage " + 1 + " and Failed " + logcount + " of Total " + 1 + "";
                    smsScheduleLog.LogTime = DateTime.Now;
                    smsScheduleLog.Status = "Success";
                    smsScheduleLog.CompanyId = 8;
                    _db.SMSScheduleLogs.Add(smsScheduleLog);
                    _db.SaveChanges();
                    return 1;
                }
                SMSScheduleLog smsScheduleLog2 = new SMSScheduleLog();

                smsScheduleLog2.Remarks = "No Count Meassages";
                smsScheduleLog2.LogTime = DateTime.Now;
                smsScheduleLog2.Status = "Failed";
                smsScheduleLog2.CompanyId = 8;
                _db.SMSScheduleLogs.Add(smsScheduleLog2);
                _db.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
