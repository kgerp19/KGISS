
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
    public partial class EmployeeIDCardReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string employeeId = string.Empty;

                if (Request.QueryString["searchText"] != null)
                {
                    employeeId = Request.QueryString["searchText"].ToString();
                }


                List<EmployeeModel> eModel = null;
                using (var _context = new ERPEntities())
                {
                    // Employee _Employee = _context.Employees.Where(x => x.Active && x.EmployeeId.Contains(employeeId)).FirstOrDefault();//From Employee Page

                    dynamic result = _context.Database.SqlQuery<EmployeeModel>("exec GetEmployeeByEmployeeId {0} ", employeeId).ToList();
                    eModel = result;
                    // eModel = transactionService.GetAllTransactionByAccountCode(accountCode).ToList();

                    //transactionInfo = _context.TransactionInfoes.Where(x=>x.AccountCode== accountCode).OrderBy(a => a.TransactionDate).ToList();
                    VendorReportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/RDLC/EmployeeIDCardReport.rdlc");
                    VendorReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("DataSetEmployeeIDCardReport", result);
                    VendorReportViewer.LocalReport.DataSources.Add(rdc);
                    VendorReportViewer.LocalReport.Refresh();
                    VendorReportViewer.DataBind();
                }
            }
        }
    }
}