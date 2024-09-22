
using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KGERP.Reports
{
    public partial class ReligionWiseReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int ReligionId = 0;

                if (Request.QueryString["religion"] != null)
                {
                    ReligionId = Convert.ToInt32(Request.QueryString["religion"].ToString());
                }

                List<EmployeeModel> eModel = null;
                using (var _context = new ERPEntities())
                {
                    dynamic result = _context.Database.SqlQuery<EmployeeModel>("exec sp_HRMS_GetReligionWiseReport {0} ", ReligionId).ToList();
                    eModel = result;

                    ReligionWiseReportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/RDLC/ReligionWiseReport.rdlc");
                    ReligionWiseReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("DatasetReligionWiseReport", result);
                    ReligionWiseReportViewer.LocalReport.DataSources.Add(rdc);
                    ReligionWiseReportViewer.LocalReport.Refresh();
                    ReligionWiseReportViewer.DataBind();
                }
            }
        }
    }
}