using KGERP.Data.Models;
using KGERP.Service.ServiceModel.FTP_Models;
using KGERP.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate
{
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
    public class MeassagesServices
    {
        private ERPEntities _db = new ERPEntities();
        //string api_key = "KEY-embfynx49pqdvmb3uz1gjlhmy1o91bgw";
        //string api_secret = "P3tge3BToEf7l5xm";
        //string postUrl = "/api/v1/secure/send-sms";
        //GET: RealEstate_SMS


        public async Task<int> MeassagesAsync()
        {
            List<RealStateMessage> realStates = _db.RealStateMessages.Where(d => d.Status != 100 && d.MediaType == 1 && d.Status != 99 && d.TryCount < 3).Take(500).ToList();
            HttpClient _client = new HttpClient();
            _client.BaseAddress = new Uri("https://smsplus.sslwireless.com");
            if (realStates.Count() > 0)
            {
                foreach (var item in realStates)
                {
                    var requestBody = new
                    {
                        api_token = "iknr8aty-ucxrhwao-vf1ioequ-txzzubdj-nterupkg",
                        sid = "KGREMASK",
                        msisdn = item.Address,
                        sms = item.Message,
                        csms_id = DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + "kgre" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second


                        //api_key = "KEY-niok0cupstnofmg288ile4zksa5m2vka",
                        //api_secret = "oL4OKONfhyoRkIs@",
                        //request_type = "SINGLE_SMS",
                        //message_type = "TEXT",
                        //campaign_title = "Test",
                        //mobile = item.Address,
                        //message_body = item.Message
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                    HttpResponseMessage resp = await _client.PostAsync("/api/v3/send-sms", content);
                    
                    if (resp.IsSuccessStatusCode)
                    {
                        item.Status = 100;
                        item.SMSLog = item.SMSLog + ",Date:" + DateTime.Now.ToLongDateString() + "-" + resp.StatusCode.ToString();
                        item.DeliveryDate = DateTime.Now;
                    }
                    else
                    {
                        item.SMSLog = item.SMSLog + ",Date:" + DateTime.Now.ToLongDateString() + "-" + resp.StatusCode.ToString();
                    }
                    item.TryCount = item.TryCount + 1;
                    if (item.TryCount == 3)
                    {
                        item.Status = 99;
                    }
                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                return 1;
            }
            return 0;
        }



        //public async Task<int> MeassagesAsync()
        //{
        //    List<RealStateMessage> realStates = _db.RealStateMessages.Where(d => d.Status != 100 && d.MediaType == 1 && d.Status != 99 && d.TryCount < 3).Take(20).ToList();
        //    HttpClient _client = new HttpClient();
        //    _client.BaseAddress = new Uri("https://smsplus.sslwireless.com");
        //    if (realStates.Count() > 0)
        //    {
        //        var requestBody = new
        //        {

        //            api_token = "iknr8aty-ucxrhwao-vf1ioequ-txzzubdj-nterupkg",
        //            sid = "KGREMASK",
        //            msisdn = "01986249236",
        //            sms = "Message Body from KG ERP",
        //            csms_id = DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + "kgre" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second

        //            //api_key = "KEY-niok0cupstnofmg288ile4zksa5m2vka",
        //            //api_secret = "oL4OKONfhyoRkIs@",
        //            //request_type = "SINGLE_SMS",
        //            //message_type = "TEXT",
        //            //campaign_title = "Test",
        //            //mobile = item.Address,
        //            //message_body = item.Message
        //        };
        //        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        //        HttpResponseMessage resp = await _client.PostAsync("/api/v3/send-sms", content);
        //        //HttpResponseMessage resp = await _client.PostAsync("/api/v1/secure/send-sms", content);
        //        if (resp.IsSuccessStatusCode)
        //        {
        //            //item.Status = 100;
        //            //item.SMSLog = item.SMSLog + ",Date:" + DateTime.Now.ToLongDateString() + "-" + resp.StatusCode.ToString();
        //            //item.DeliveryDate = DateTime.Now;
        //        }
        //        //else
        //        //{
        //        //    item.SMSLog = item.SMSLog + ",Date:" + DateTime.Now.ToLongDateString() + "-" + resp.StatusCode.ToString();
        //        //}
        //        //item.TryCount = item.TryCount + 1;
        //        //if (item.TryCount == 3)
        //        //{
        //        //    item.Status = 99;
        //        //}
        //        //_db.Entry(item).State = EntityState.Modified;
        //        //_db.SaveChanges();




        //        //foreach (var item in realStates)
        //        //{

        //        //}
        //        return 1;
        //    }
        //    return 0;
        //}

    }
}
