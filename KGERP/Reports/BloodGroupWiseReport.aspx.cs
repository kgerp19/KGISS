
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
    public partial class BloodGroupWiseReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int BloodId = 0;

                if (Request.QueryString["bloodGroup"] != null)
                {
                    BloodId = Convert.ToInt32(Request.QueryString["bloodGroup"].ToString());
                }

                List<EmployeeModel> eModel = null;
                using (var _context = new ERPEntities())
                {
                    dynamic result = _context.Database.SqlQuery<EmployeeModel>("exec sp_HRMS_GetBloodGroupWiseReport {0} ", BloodId).ToList();
                    eModel = result;

                    BloodReportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/RDLC/BloodGroupWiseReport.rdlc");
                    BloodReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("DatasetBloodGroupWiseReport", result);
                    BloodReportViewer.LocalReport.DataSources.Add(rdc);
                    BloodReportViewer.LocalReport.Refresh();
                    BloodReportViewer.DataBind();
                }
            }
        }
    }
}