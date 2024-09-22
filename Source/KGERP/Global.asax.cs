using KGERP.Controllers;
using KGERP.Models;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.SMS_Service_Implementation;
using KGERP.Service.Utility;
using KGERP.Utility;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace KGERP
{


    public class CheckSessionAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
  
        if (filterContext.HttpContext.Session["Id"] == null)
        {

            filterContext.Result = new RedirectResult("~/User/Login");
        }

        base.OnActionExecuting(filterContext);
    }
}



    public static class BDCurrency
    {
        public static string SetBDCurrency(string number)
        {
            var info = new System.Globalization.CultureInfo("hi-IN");
            string valueApplied;
            decimal value = Decimal.Round(Convert.ToDecimal(number), 2);
            if (value < 0m)
            {
                string minusStr = value.ToString();
                string subtracSign = minusStr.Substring(1, minusStr.Length - 1);
                decimal minusValue = Math.Round(Convert.ToDecimal(subtracSign), 2);
                if (minusValue < 1000000000)
                {
                    valueApplied = "(" + minusValue.ToString("N2", info) + ")";
                }
                else
                {
                    decimal decimalValue = Decimal.Round(Convert.ToDecimal(minusValue), 2);
                    string strValue = decimalValue.ToString();
                    if (!strValue.Contains("."))
                    {
                        strValue = strValue + ".00";
                    }
                    string formatingPart;
                    formatingPart = strValue.Substring((strValue.Length - 1) - 10, 11);
                    string addOneformatingPart = "1" + formatingPart;
                    string firstPart;
                    #region Past Part
                    if (minusValue < 10000000000)
                    {
                        firstPart = strValue.Substring(0, 2);
                    }
                    else if (minusValue < 100000000000)
                    {
                        firstPart = strValue.Substring(0, 3);
                    }
                    else if (minusValue < 1000000000000)
                    {
                        firstPart = strValue.Substring(0, 4);
                    }
                    else if (minusValue < 10000000000000)
                    {
                        firstPart = strValue.Substring(0, 5);
                    }
                    else if (minusValue < 100000000000000)
                    {
                        firstPart = strValue.Substring(0, 6);
                    }
                    else
                    {
                        firstPart = strValue.Substring(0, 7);
                    }
                    #endregion
                    decimal formatingPartvalue = Decimal.Round(Convert.ToDecimal(addOneformatingPart), 2);
                    string formatedval = formatingPartvalue.ToString("N2", info);
                    string subtracOneformatingPart = formatedval.Substring(1, formatedval.Length - 1);
                    valueApplied = firstPart + subtracOneformatingPart;
                }
            }
            else
            {
                if (value < 1000000000)
                {
                    valueApplied = value.ToString("N2", info);
                }
                else
                {
                    decimal decimalValue = Decimal.Round(Convert.ToDecimal(value), 2);
                    string strValue = decimalValue.ToString();
                    if (!strValue.Contains("."))
                    {
                        strValue = strValue + ".00";
                    }
                    string formatingPart;
                    formatingPart = strValue.Substring((strValue.Length - 1) - 10, 11);
                    string addOneformatingPart = "1" + formatingPart;
                    string firstPart;
                    #region Past Part
                    if (value < 10000000000)
                    {
                        firstPart = strValue.Substring(0, 2);
                    }
                    else if (value < 100000000000)
                    {
                        firstPart = strValue.Substring(0, 3);
                    }
                    else if (value < 1000000000000)
                    {
                        firstPart = strValue.Substring(0, 4);
                    }
                    else if (value < 10000000000000)
                    {
                        firstPart = strValue.Substring(0, 5);
                    }
                    else if (value < 100000000000000)
                    {
                        firstPart = strValue.Substring(0, 6);
                    }
                    else
                    {
                        firstPart = strValue.Substring(0, 7);
                    }
                    #endregion
                    decimal formatingPartvalue = Decimal.Round(Convert.ToDecimal(addOneformatingPart), 2);
                    string formatedval = formatingPartvalue.ToString("N2", info);
                    string subtracOneformatingPart = formatedval.Substring(1, formatedval.Length - 1);
                    valueApplied = firstPart + subtracOneformatingPart;
                }
            }
            return valueApplied;
        }
    }


    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
            ModelMapper.SetUp();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));

            CultureInfo newCulture = new CultureInfo("en-ZA", false);
            // NOTE: change the culture name en-ZA to whatever culture suits your needs

            newCulture.DateTimeFormat.DateSeparator = "/";
            newCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            newCulture.DateTimeFormat.LongDatePattern = "dd/MM/yyyy hh:mm:ss tt";

            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;
            Application_Start2();
            AnniversaryStart();
            SetUpTimer();
            LeaveCalculationTimer();
           // Application_StartAllSMS();
            HappyAnniversaryMail();
            SeedSMSStart();
            FeedSMSStart();
        }
        private static System.Threading.Timer _timer;
        private static System.Threading.Timer _timerLeave;
        private static System.Threading.Timer _timerAnniversary;

        private void SetUpTimer()
        {
            TimeSpan timeToGo = DateTime.Now.AddDays(1).Date - DateTime.Now.Date; //timespan for 00:00 tommorrow 
            _timer = new System.Threading.Timer(x => SendEmail(), null, timeToGo, new TimeSpan(1, 0, 0, 0));
        }
        private void LeaveCalculationTimer()
        {
            TimeSpan timeToGo = (DateTime.Today.AddDays(1) - DateTime.Now); //timespan for 00:00 tommorrow 
            timeToGo = timeToGo.Add(new TimeSpan(0,30,0));
            _timerLeave = new System.Threading.Timer(x => LeaveCalculation(), null,timeToGo, new TimeSpan(1, 0, 0,0));            
        }
        private void LeaveCalculation()
        {
            //Inactive resign employee user
            Service.Utility.UserService.InactiveResignUser(DateTime.Today);
            //Update leave balance 
            Service.Utility.LeaveService.LeaveCalculation();
        }
        public void SendEmail()
        {
           //string body = "How are you?";
          // MailService.SendMail(string.Empty, string.Empty, "ashraf.erp@krishibidgroup.com", "Ashraf", string.Empty, string.Empty, "Test mail to Ashraf", body, out string sendStatus);

        }

        public bool IsReleased = true;

        protected void Application_Start2()
        {
            var res= ConfigurationManager.AppSettings["RealEstate_MessagesKey"];
            if ((Convert.ToInt32(res))== 2)
            {
                var timer = new System.Timers.Timer();
                timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                timer.Elapsed += TimerElapsed;
                timer.Start();
            }

        }
        protected void AnniversaryStart()
        {
            var res = ConfigurationManager.AppSettings["AnniversaryKey"];
            if ((Convert.ToInt32(res)) == 2)
            {
                var timer = new System.Timers.Timer();
                timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                timer.Elapsed += AnniversaryElapsed;
                timer.Start();
            }

        }

        private async void AnniversaryElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            EmployeeController ec = new EmployeeController();
            ec.WishHappyAnniversary(true);
        }

        private async void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var dataAccessLayer = new MeassagesServices();
          await dataAccessLayer.MeassagesAsync();
        }
        protected void Application_StartAllSMS()
        {
            var res = ConfigurationManager.AppSettings["AllCompany_MessagesKey"];
            if ((Convert.ToInt32(res)) == 2)
            {
                var timer = new System.Timers.Timer();
                timer.Interval = TimeSpan.FromMinutes(20).TotalMilliseconds;
                timer.Elapsed += TimerElapsedAllSMS;
                timer.Start();
            }

        }

        protected void SeedSMSStart()
        {
            var res = ConfigurationManager.AppSettings["SeedMessagesKey"];
           
            if ((Convert.ToInt32(res)) == 2)
            {
                var timer = new System.Timers.Timer();
                timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                timer.Elapsed += SeedSMSSend;
                timer.Start();
            }

        }

        protected void FeedSMSStart()
        {
            var res = ConfigurationManager.AppSettings["FeedMessagesKey"];

            if ((Convert.ToInt32(res)) == 2)
            {
                var timer = new System.Timers.Timer();
                timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                timer.Elapsed += FeedSMSSend;
                timer.Start();
            }

        }


        private async void TimerElapsedAllSMS(object sender, System.Timers.ElapsedEventArgs e)
        {
            var dataAccessLayer = new AllCompanyMeassagesServices();
            await dataAccessLayer.MeassagesAsync();
        }

        private async void SeedSMSSend(object sender, System.Timers.ElapsedEventArgs e)
        {
            var seedMeassagesServices = new SeedMeassagesServices();
            await seedMeassagesServices.SeedMeassagesAsync();
        }

        private async void FeedSMSSend(object sender, System.Timers.ElapsedEventArgs e)
        {
            var seedMeassagesServices = new FeedMeassagesServices();
            await seedMeassagesServices.FeedMeassagesAsync();
        }

        protected void HappyAnniversaryMail()
        {
            var res = ConfigurationManager.AppSettings["WishHappyAnniversary_MessagesKey"];
            if ((Convert.ToInt32(res)) == 2)
            {
                TimeSpan timeToGo = (DateTime.Today.AddDays(1) - DateTime.Now); //timespan for 00:00 tommorrow 
                timeToGo = timeToGo.Add(new TimeSpan(7, 0, 0));
                //var timer = new System.Timers.Timer();
                //timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                _timerAnniversary = new System.Threading.Timer(x => WishAnniversary(), null, timeToGo, new TimeSpan(1, 0, 0, 0));
            }

        }

        private async void WishAnniversary()
        {
            EmployeeController ec = new EmployeeController();
            ec.WishHappyAnniversary(true);
        }

    }
}
