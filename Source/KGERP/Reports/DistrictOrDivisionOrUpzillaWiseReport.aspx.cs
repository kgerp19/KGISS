
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
    public partial class DistrictOrDivisionOrUpzillaWiseReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string districtName = string.Empty;

                if (Request.QueryString["District"] != null)
                {
                    districtName = Request.QueryString["District"].ToString();
                }


                List<EmployeeModel> eModel = null;
                using (var _context = new ERPEntities())
                {
                    // Employee _Employee = _context.Employees.Where(x => x.Active && x.EmployeeId.Contains(employeeId)).FirstOrDefault();//From Employee Page

                    dynamic result = _context.Database.SqlQuery<EmployeeModel>("exec sp_HRMS_GetDistrictOrDivisionOrUpzillaWiseList {0} ", districtName).ToList();
                    eModel = result;

                    DistrictOrDivisionOrUpzillaWiseReportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/RDLC/DistrictOrDivisionOrUpzillaWiseReport.rdlc");
                    DistrictOrDivisionOrUpzillaWiseReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("DataSetDistrictOrDivisionOrUpzillaWiseReport", result);
                    DistrictOrDivisionOrUpzillaWiseReportViewer.LocalReport.DataSources.Add(rdc);
                    DistrictOrDivisionOrUpzillaWiseReportViewer.LocalReport.Refresh();
                    DistrictOrDivisionOrUpzillaWiseReportViewer.DataBind();
                }
            }
        }
    }
}