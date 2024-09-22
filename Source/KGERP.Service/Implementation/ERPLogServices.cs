using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
  public  class ERPLogServices
    {
        private ERPEntities context;
        public ERPLogServices(ERPEntities db)
        {
            context = db;
        }

        public async Task<int> AddNewLog(ErpLogViewModel model)
        {
            try
            {
                ErpLogInfo erpLogInfo = new ErpLogInfo();
                // var mac = GetMAC();
                // string ip = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[1].ToString();          
                // string os = Environment.OSVersion.ToString();
                // string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                // erpLogInfo.MacAddress = mac;
                // erpLogInfo.UserIpAddress = model.IPAddress; 
                //erpLogInfo.UserName = "Operating System : " + os + "Device User" + userName;
                erpLogInfo.CompanyId = model.CompanyId;
                erpLogInfo.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                erpLogInfo.CreatedDate = DateTime.Now;
                erpLogInfo.UserIpAddress = model.IPAddress;
                erpLogInfo.MacAddress = model.MacAddress;
                erpLogInfo.UserName=model.UserName;
                erpLogInfo.Note = model.Note;
                erpLogInfo.IntegretFrom=model.IntegretFrom;
                context.ErpLogInfoes.Add(erpLogInfo);
                await context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private string GetMAC()
        {
            string macAddresses = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return macAddresses;
        }

    }
}
