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
    public partial class ClientPaymentInformationReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int clientId = 0;

                if (Request.QueryString["clientId"] != null)
                {
                    clientId = Convert.ToInt32(Request.QueryString["clientId"].ToString());
                }

                List<KGREPlotBookingModel> paymentModel = null;
                using (var db = new ERPEntities())
                {
                    dynamic result = db.Database.SqlQuery<KGREPlotBookingModel>("KGRE_GetPaymentByClientId {0}", clientId).ToList();
                    //dynamic result = _context.Database.SqlQuery<EmployeeModel>("exec sp_KGRE_GetPaymentByClientId {0} ", clientId).ToList();
                    paymentModel = result;


                    //object BasicInfo = null;
                 //var   paymentModel2 = (from basic in db.KGRECustomers
                 //             join b in db.KGREPlotBookings on basic.ClientId equals b.ClientId
                 //             join p in db.KGREPlots on b.PlotId equals p.PlotId
                 //             join d in db.DropDownItems on p.PlotStatus equals d.DropDownItemId
                 //             join r in db.DropDownItems on basic.ReligionId equals r.DropDownItemId
                 //             join pid in db.KGREProjects on basic.ProjectId equals pid.ProjectId
                 //             where basic.ClientId == clientId
                 //             select new KGREPlotBookingModel()
                 //             {
                 //                 PlotId=p.PlotId,
                 //                 PlotNo = p.PlotNo,
                 //                 PlotSize = p.PlotSize,
                 //                 PlotStatus = d.Name,
                 //                 PlotFace = p.PlotFace,
                 //                 BookingId=b.BookingId,
                 //                 NoOfInstallment = b.NoOfInstallment,
                 //                 BookingDate = b.BookingDate,
                 //                 Remarks = b.Remarks,
                 //                 FileNo = b.FileNo,
                 //                 MobileNo = basic.MobileNo,
                 //                 Religion = r.Name,
                 //                 RegistrationDate = b.RegistrationDate,
                 //                 LandValue = b.LandValue,
                 //                 LandValueR = b.LandValueR,
                 //                 Additional25Percent = b.Additional25Percent,
                 //                 Additional25PercentR = b.Additional25PercentR,
                 //                 Additional10Percent = b.Additional10Percent,
                 //                 Additional10PercentR = b.Additional10PercentR,
                 //                 Additional15Percent = b.Additional15Percent,
                 //                 Additional15PercentR = b.Additional15PercentR,
                 //                 Discount = b.Discount,
                 //                 DiscountR = b.DiscountR,
                 //                 RegAmount = b.RegAmount,
                 //                 ServiceCharge4or10Per = b.ServiceCharge4or10Per,
                 //                 BookingMoney = b.BookingMoney,
                 //                 UtilityCost = b.UtilityCost,
                 //                 InstallmentAmount = b.InstallmentAmount, 
                 //                 NetReceivedR = b.NetReceivedR,
                 //                 GrandTotal = b.GrandTotal,
                 //                 GrandTotalR = b.GrandTotalR,
                 //                 Due = b.Due,
                 //                 ProjectName = pid.ProjectName,
                 //                 ClientId = basic.ClientId,
                 //                 Designation = basic.Designation,
                 //                 FullName = basic.FullName,
                 //                 DepartmentOrInstitution = basic.DepartmentOrInstitution,
                 //                 PresentAddress = basic.PresentAddress,
                 //                 PermanentAddress = basic.PermanentAddress,
                 //                 DateofBirth = basic.DateofBirth, 
                 //                 Email = basic.Email, 
                 //                 ResponsibleOfficer = basic.ResponsibleOfficer,
                 //                 CompanyId = basic.CompanyId, 
                 //             }).ToList();
                  
                    ClientPaymentReportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/RDLC/KGRE/ClientPaymentReportViewer.rdlc");
                    ClientPaymentReportViewer.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("ClientPaymentReportDataSet", paymentModel);
                    ClientPaymentReportViewer.LocalReport.DataSources.Add(rdc);
                    ClientPaymentReportViewer.LocalReport.Refresh();
                    ClientPaymentReportViewer.DataBind();
                }
            }
        }
    }
}