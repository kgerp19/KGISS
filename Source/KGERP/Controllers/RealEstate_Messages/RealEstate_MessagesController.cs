using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using KGERP.Controllers.MoneyReceiptProcess;
using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using KGERP.Service.Implementation.SMSService;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.SMS_Service_Implementation;
using KGERP.Utility;
using KGERP.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace KGERP.Controllers.RealEstateSMS
{
    [CheckSession]

    public class RealEstate_MessagesController : Controller
    {
        private ERPEntities db = new ERPEntities();
        private readonly ISmsServices _ISmsServices;
        public static HttpClient _client = new HttpClient();
        string baseUrl = "http://portal.adnsms.com";
        string api_key = "KEY-embfynx49pqdvmb3uz1gjlhmy1o91bgw";
        string api_secret = "P3tge3BToEf7l5xm";
        string postUrl = "/api/v1/secure/send-sms";
        // GET: RealEstate_SMS
        public RealEstate_MessagesController(ISmsServices ISmsServices)
        {
            _ISmsServices = ISmsServices;
        }
        public async Task<ActionResult> Index(long Id)
        {
            var res = db.RealStateMessages.FirstOrDefault(f=>f.ID== Id);
            _client.BaseAddress = new Uri("https://smsplus.sslwireless.com");
            var requestBody = new
            {
                api_token = "iknr8aty-ucxrhwao-vf1ioequ-txzzubdj-nterupkg",
                sid = "KGREMASK",
                msisdn = res.Address,
                sms = res.Message,
                csms_id = DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + "kgre" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second


                //api_key = "KEY-niok0cupstnofmg288ile4zksa5m2vka",
                //api_secret = "oL4OKONfhyoRkIs@",
                //request_type = "SINGLE_SMS",
                //message_type = "TEXT",
                //campaign_title = "Test",
                //mobile = res.Address,
                //message_body =res.Message
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            HttpResponseMessage resp = await _client.PostAsync("/api/v3/send-sms", content);
            if (resp.IsSuccessStatusCode)
            {
                res.Status = 100;
                res.DeliveryDate = DateTime.Now;
                db.Entry(res).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction(nameof(GetByMessageList), new { companyId = res.CompanyId});
            }
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> GetByMessageList(int companyId)
        {
            var res = await _ISmsServices.getRSMSListnew(companyId);
            return View(res);
        }
        [HttpGet]
        public async Task<ActionResult> GetByid(long id)
        {
            var res = await _ISmsServices.Getbyrealsms(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }        
        [HttpPost]
        public async Task<ActionResult> NewMessage(RealStateMessageVM message)
        {
            message.Tag = 1;
            var res = await _ISmsServices.AddRSMSListnew(message);
            if (res>0)
            {
                return RedirectToAction(nameof(Index), new { id = res });
            }
            return RedirectToAction(nameof(GetByMessageList), new { companyId = message.CompanyId });
        }
        [HttpGet]
        public async Task<ActionResult> SingleMessage(int companyId)
        {
            RealStateMessageVM vM=new RealStateMessageVM();
            vM.CompanyId = companyId;
            if (companyId==7)
            {
                vM.Message = "Dear Client Name,\r\n\r\nThank you for paying the ....Purpose.... TK 0000 on 00-00-2023 against your Plot ..... at Project Name.\r\n\r\nRegards CC, GLDL";
            }
            else
            {
                vM.Message = "Dear Client Name,\r\n\r\nThank you for paying the ....Purpose..... TK 000000 on 00-00-2023 against your Flat .... at Project Name.\r\n\r\nRegards CC, KPL";
            }
            return View(vM);
        }
        [HttpPost]
        public async Task<ActionResult> SingleMessage(RealStateMessageVM real)
        {
            real.Tag = 2;
            var res = await _ISmsServices.AddRSMSListnew(real);
            if (res > 0)
            {
                return RedirectToAction(nameof(Index), new { id = res });
            }
            return RedirectToAction(nameof(GetByMessageList), new { companyId = real.CompanyId });
        }


      
        [HttpGet]
        public async Task<ActionResult> MessageList(int companyId)
        {
            MessageViewModel messageViewModel = new MessageViewModel();
            DateTime currentDate = DateTime.Now;

            // Get the first date of the current month
            messageViewModel.FromDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime firstDateOfNextMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(1);
            // Get the last date of the current month by subtracting one day from the first date of the next month      

            messageViewModel.ToDate = firstDateOfNextMonth.AddDays(-1);
            messageViewModel.CompanyId = companyId;
            messageViewModel.CompanyName = companyId == 7 ? "GLDL" : "KPL";
            return View(messageViewModel);
        }

        //[HttpPost]
        public async Task<ActionResult> MessageList(MessageViewModel messageViewModel)
        {
            string userName = (string)Session["UserName"];
            var result = await _ISmsServices.ExecuteStoredProcedure(messageViewModel.FromDate, messageViewModel.ToDate,userName,messageViewModel.CompanyId, messageViewModel.SelectedOption);
            messageViewModel.Result = result;
            messageViewModel.CompanyName = messageViewModel.CompanyId == 7 ? "GLDL" : "KPL";

            return View(messageViewModel);
        }
       
        
        public async Task<ActionResult> InsertMessage(string FromDate, string ToDate, int CompanyId, int SelectedOption) // 1 first Sms 2 scond sms
        {
            var fDate = Convert.ToDateTime(FromDate);
            var tDate = Convert.ToDateTime(ToDate);
            string userName = (string)Session["UserName"];
            var result = await _ISmsServices.ExecuteStoredProcedureForInsert(fDate, tDate, userName, CompanyId, SelectedOption);
            var message = " ";
           
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }



   
    public class SingleSMSResp
    {
        public string request_type { get; set; }
        public string campaign_uid { get; set; }
        public string sms_uid { get; set; }
        public List<string> invalid_numbers { get; set; }
        public int api_response_code { get; set; }
        public string api_response_message { get; set; }
    }
}