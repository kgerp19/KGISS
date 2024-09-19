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
    public class SeedMeassagesServices
    {
        private ERPEntities _db = new ERPEntities();

        public async Task<int> SeedMeassagesAsync()
        {
            try
            {
                var lstSms = await _db.ErpSMS.Where(e => (e.Status == (int)EnumSmSStatus.Pending || e.Status == (int)EnumSmSStatus.Failed)
                    && e.TryCount < 3 && e.Date <= DateTime.Now && e.CompanyId == 21).Take(20).ToListAsync();

                HttpClient _client = new HttpClient();
                _client.BaseAddress = new Uri("https://portal.adnsms.com");
                if (lstSms.Count() > 0)
                {
                    int countsms = 0;
                    foreach (var sms in lstSms)
                    {

                        var requestBody = new
                        {
                            api_key = "KEY-6lzz74ag2mvluy02iaugrj5ht6ffngvd",
                            api_secret = "1NsH1erN5Hz$2RRe",
                            request_type = "SINGLE_SMS",
                            message_type = "TEXT",
                            campaign_title = "Test",
                            mobile = sms.PhoneNo,
                            message_body = sms.Message
                        };
                        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                        HttpResponseMessage resp = await _client.PostAsync("/api/v1/secure/send-sms", content);
                        if (resp.IsSuccessStatusCode)
                        {
                            sms.Status = 9;
                            countsms = countsms + 1;
                        }
                        else
                        {
                            sms.TryCount = sms.TryCount + 1;
                        }

                        if (sms.TryCount == 3)
                        {
                            sms.Status = 3;
                        }
                        _db.Entry(sms).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    SMSScheduleLog smsScheduleLog = new SMSScheduleLog();
                    var logcount = lstSms.Count() - countsms;
                    smsScheduleLog.Remarks = "Successfully Send Massage " + countsms + " and Failed " + logcount + " of Total " + lstSms.Count() + "";
                    smsScheduleLog.LogTime = DateTime.Now;
                    smsScheduleLog.Status = "Success";
                    smsScheduleLog.CompanyId = 21;
                    _db.SMSScheduleLogs.Add(smsScheduleLog);
                    _db.SaveChanges();
                    return 1;
                }
                SMSScheduleLog smsScheduleLog2 = new SMSScheduleLog();

                smsScheduleLog2.Remarks = "No Count Meassages";
                smsScheduleLog2.LogTime = DateTime.Now;
                smsScheduleLog2.Status = "Failed";
                smsScheduleLog2.CompanyId = 21;
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


    public partial class VmErpSMS
    {
        public long Id { get; set; }
        public System.DateTime Date { get; set; }
        public string Subject { get; set; }
        public string PhoneNo { get; set; }
        public string Message { get; set; }
        public int CompanyId { get; set; }
        public Nullable<int> Status { get; set; }
        public int TryCount { get; set; }
        public System.DateTime RowTime { get; set; }
        public string Remarks { get; set; }
        public int SmsType { get; set; }

        public virtual SmsType SmsType1 { get; set; }
        public virtual Company Company { get; set; }
    }




}
