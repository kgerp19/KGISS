using KGERP.Utility;
using System;
using System.Web.UI.WebControls;

namespace KGERP.Reports
{
    public partial class ReportTemplate : System.Web.UI.Page
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    rvSiteMapping.ServerReport.ReportServerCredentials = new ReportCredentials("Administrator", "Sysroot@123", "Domain");

                    string reportFolder = System.Configuration.ConfigurationManager.AppSettings["SSRSReportsFolder"].ToString();

                    rvSiteMapping.Height = Unit.Pixel(Convert.ToInt32(Request["Height"]) - 58);
                    rvSiteMapping.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

                    rvSiteMapping.ServerReport.ReportServerUrl = new Uri("http://192.168.0.13/ReportServer/?%2fErpReport/"); // Add the Reporting Server URL
                    rvSiteMapping.ServerReport.ReportPath = String.Format("/{0}/{1}", reportFolder, Request["ReportName"].ToString());

                    rvSiteMapping.ServerReport.Refresh();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
        
        }
    }
}