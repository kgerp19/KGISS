
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
    public partial class DepartmentWiseReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int departmentId = 0;

                if (Request.QueryString["department"] != null)
                {
                    departmentId = Convert.ToInt32(Request.QueryString["department"].ToString());
                }

                List<EmployeeModel> eModel = null;
                using (var _context = new ERPEntities())
                {
                    dynamic result = _context.Database.SqlQuery<EmployeeModel>("exec sp_HRMS_GetDepartmentWiseReport {0} ", departmentId).ToList();
                    eModel = result;

                    DepartmentReportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/RDLC/DepartmentWiseReport.rdlc");
                    DepartmentReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("DatasetDepartmentWiseReport", result);
                    DepartmentReportViewer.LocalReport.DataSources.Add(rdc);
                    DepartmentReportViewer.LocalReport.Refresh();
                    DepartmentReportViewer.DataBind();
                }
            }
        }
    }
}