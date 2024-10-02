using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Utility
{
    public static class GlobalValues
    {
        private static string _userId=null;

        // Property to get or set the user ID
        public static string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
    }
    public static class Common
    {
        public static string GetGeneralRequisitionProductType { get; set; } = "G";

        //public static string GetUserId()
        //{
        //    return System.Web.HttpContext.Current.User.Identity.Name;
        //}
        public static string GetUserId()
        {
            var context = System.Web.HttpContext.Current;
            if (context != null && context.User != null)
            {
                return context.User.Identity != null ? context.User.Identity.Name : null;
            }
            return null;
        }
        public static long GetIntUserId()
        {
            return  Convert.ToInt64(System.Web.HttpContext.Current.Session["Id"]);
        }
        public static long GetManagerId()
        {
            return Convert.ToInt64(System.Web.HttpContext.Current.Session["ManagerId"]);
        }
        public static long GetHRAdminId()
        {
            return Convert.ToInt64(System.Web.HttpContext.Current.Session["HrAdminId"]);
        }
        public static int GetCompanyId()
        {
            return Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        }

        public static string GetCompanyName()
        {
            return Convert.ToString(System.Web.HttpContext.Current.Session["CompanyName"]);
        }
        public static string GetCompanyLogo()
        {
            return Convert.ToString(System.Web.HttpContext.Current.Session["CompanyLogo"]);
        }
        public static long GetGroupManagingDirectorId()
        {
            return 2;
        }
        public static string GenerateLengthWiseString(string actualValue,int demandDigit, char preFillupChar)
        {
            int actualLength = actualValue.Length;
            string newString = string.Empty;
            if (actualLength < demandDigit)
            {
                int diff = demandDigit - actualLength;
                newString = new string(preFillupChar, diff);
                return newString + actualValue;
            }
            return actualValue;
        }

    }

    public static class PageInfo
    {
        private static bool IsShoqPageInfoValue = false;

        public static string PageTitle = "PageTitle";
        public static string PageHeader = "PageHeader";
        public static string PageLink1 = "PageLink1";
        public static string PageLink2 = "PageLink2";
        public static string PageAdditionalInfo = "PageAdditionalInfo";
        public static string btnText1 = "btnText1";
        public static string btnText2 = "btnText2";
        public static string btnClass1 = "btnClass1";
        public static string btnClass2 = "btnClass2";
        public static string PageHeaderFaIcon = "PageHeaderFaIcon";
        public static bool IsShoqPageInfo
        {
            get { return IsShoqPageInfoValue; }
            set { IsShoqPageInfoValue = value; }
        }
    }
    
}
