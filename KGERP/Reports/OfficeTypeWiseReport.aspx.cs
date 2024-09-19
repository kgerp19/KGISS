
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
    public partial class OfficeTypeWiseReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int OfficeTypeId = 0;

                if (Request.QueryString["officeType"] != null)
                {
                    OfficeTypeId = Convert.ToInt32(Request.QueryString["officeType"].ToString());
                }

                List<EmployeeModel> eModel = null;
                using (var _context = new ERPEntities())
                {
                    dynamic result = _context.Database.SqlQuery<EmployeeModel>("exec sp_HRMS_GetOfficeTypeWiseReport {0} ", OfficeTypeId).ToList();
                    eModel = result;

                    OfficeTypeWiseReportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/RDLC/OfficeTypeWiseReport.rdlc");
                    OfficeTypeWiseReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("DatasetOfficeTypeWiseReport", result);
                    OfficeTypeWiseReportViewer.LocalReport.DataSources.Add(rdc);
                    OfficeTypeWiseReportViewer.LocalReport.Refresh();
                    OfficeTypeWiseReportViewer.DataBind();
                }
            }
        }
    }
}