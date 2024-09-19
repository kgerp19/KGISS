using Microsoft.Reporting.WebForms;

namespace KGERP.Utility
{
    public class ReportCredentials : IReportServerCredentials
    {

        public ReportCredentials(string userName, string password, string domain)
        {
            UserName = userName;
            Password = password;
            Domain = domain;
        }

        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }
        public System.Net.ICredentials NetworkCredentials
        {
            get
            {
                return new System.Net.NetworkCredential(UserName, Password, Domain);
            }
        }

        private string UserName { get; set; }
        private string Password { get; set; }
        private string Domain { get; set; }

        public bool GetFormsCredentials(out System.Net.Cookie authCookie, out string userName, out string password, out string authority)
        {
            authCookie = null;
            userName = password = authority = null;
            return false;
        }
    }
}