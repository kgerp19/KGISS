using ClosedXML.Excel;
using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using iText = iTextSharp.text;
using iTextPdf = iTextSharp.text.pdf;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ApplicationManageController : Controller
    {
        private readonly IApplicationManageService _applicationManageService;
        private readonly ERPEntities db = new ERPEntities();
        private readonly ConfigurationService _configrationService;

        // Normally injected, assuming default constructor for MVC 5 without DI container configured specifically
        public ApplicationManageController(ConfigurationService configurationService)
        {
            _applicationManageService = new ApplicationManageService(db);
            _configrationService = configurationService;
        }

        private long GetEmployeeId()
        {
            try
            {
                return Convert.ToInt64(Session["Id"]);
            }
            catch
            {
                return 0; // Or handle appropriately
            }
        }

        private string GetUsername()
        {
            try
            {
                return User.Identity.Name;
            }
            catch
            {
                return "Unknown";
            }
        }

        private void PopulateDropdowns(ApplicationManageModel model)
        {
            var currentUserId = Common.GetIntUserId();
            var employees = db.Employees.Where(e => e.Active && e.CompanyId == model.CompanyId && e.Id== currentUserId).Select(e => new { e.Id, Name = e.EmployeeId + " - " + e.Name }).ToList();
            model.Managers = new SelectList(employees, "Id", "Name", model.ManagerId);

            //var vendors = db.Vendors.Where(v => v.IsActive && v.CompanyId == model.CompanyId).Select(v => new { v.VendorId, Name = v.Code + " - " + v.Name }).ToList();
            //model.Applicants = new SelectList(vendors, "VendorId", "Name", model.ApplicantId);
            
            var vendors = (from v in db.Vendors
                           join sz in db.SubZones on v.SubZoneId equals sz.SubZoneId
                           where v.IsActive && v.CompanyId == model.CompanyId && sz.SalesOfficerId== currentUserId
                           select new
                           {
                               v.VendorId,
                               Name = v.Code + " - " + v.Name
                           }).ToList();

            model.Applicants = new SelectList(vendors, "VendorId", "Name", model.ApplicantId);

        }

        [HttpGet]
        public async Task<ActionResult> OrderCreditLimitEntry(int companyId, long applicationId = 0)
        {
            var model = await _applicationManageService.GetOrderCreditLimitApplications(companyId, applicationId);
            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now;
            PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> OrderCreditLimitEntry(ApplicationManageModel model)
        {
            long employeeId = GetEmployeeId();
            if (ModelState.IsValid)
            {
                bool hasSignatory = db.RequisitionSignatories.Any(x => x.IntegrateWith == "OrderCreditLimit" && x.IsActive);
                if (!hasSignatory)
                {
                    TempData["ErrorMessage"] = "Signaotry can't found Please signatory set first please";
                    return RedirectToAction("OrderCreditLimitEntry", new { companyId = model.CompanyId });
                }
                else
                {
                    try
                    {
                        long newAppId = await _applicationManageService.SaveOrderCreditLimitApplication(model, GetUsername(), employeeId);
                        if (newAppId==0)
                        {
                            TempData["Message"] = "Data can't save!";
                        }
                        else
                        {
                            TempData["Message"] = "Application Submitted Successfully";
                        }
                        
                        return RedirectToAction("OrderCreditLimitEntry", new { companyId = model.CompanyId, applicationId = newAppId });
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Failed to submit application. " + ex.Message;
                    }
                }
            }

            // Repopulate if failed
            var listModel = await _applicationManageService.GetOrderCreditLimitApplications(model.CompanyId, 0);
            model.ApplicationList = listModel.ApplicationList;
            PopulateDropdowns(model);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> OrderCreditLimitApprovalList(int companyId)
        {
            long employeeId = GetEmployeeId();
            var model = await _applicationManageService.GetPendingApprovals(companyId, employeeId);
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> OrderCreditLimitManageList(
    int companyId, int? searchStatus, string strFromDate, string strToDate, int? subZoneId)
        {
            DateTime? fromDate = null;
            DateTime? toDate = null;

            if (string.IsNullOrEmpty(strFromDate) && string.IsNullOrEmpty(strToDate))
            {
                fromDate = DateTime.Now.AddMonths(-1);
                toDate = DateTime.Now;
                strFromDate = fromDate.Value.ToString("yyyy-MM-dd");
                strToDate = toDate.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                if (DateTime.TryParse(strFromDate, out var f)) fromDate = f;
                if (DateTime.TryParse(strToDate, out var t)) toDate = t;
            }

            int zoneId = subZoneId ?? 0;

            var model = await _applicationManageService
                .GetOrderCreditLimitManageList(companyId, searchStatus, fromDate, toDate, zoneId);

            model.TerritoryList = new SelectList(
                _configrationService.CommonSubZonesDropDownList(companyId), "Value", "Text", zoneId);
            model.StrFromDate = strFromDate;
            model.StrToDate = strToDate;
            model.SubZoneId = zoneId;   // dropdown এ selected value ধরে রাখার জন্য

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateApprovalStatus(long approvalId, int status, string comment)
        {
            try
            {
                bool result = await _applicationManageService.UpdateApprovalStatus(approvalId, status, comment, GetUsername());
                return Json(new { success = result, message = result ? "Status updated successfully" : "Failed to update status." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult> QuickChangeCreditLimit(int companyId = 0)
        {
            if (companyId <= 0 && Session["CompanyId"] != null)
            {
                int.TryParse(Session["CompanyId"].ToString(), out companyId);
            }

            var model = await _applicationManageService.GetQuickChangeCreditLimitList(companyId);
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCreditLimit(int vendorId, decimal creditLimit)
        {
            try
            {
                if (vendorId <= 0)
                {
                    return Json(new { success = false, message = "Invalid customer selected." });
                }

                if (creditLimit < 0)
                {
                    return Json(new { success = false, message = "Credit limit cannot be negative." });
                }

                bool result = await _applicationManageService.UpdateVendorCreditLimit(vendorId, creditLimit, GetUsername());
                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Credit limit updated successfully.",
                        vendorId = vendorId,
                        newCreditLimit = creditLimit
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Customer not found or update failed." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating credit limit: " + ex.Message });
            }
        }

        #region Export (Excel / PDF)

        // তারিখ ঠিক করার common helper (ManageList এর মতোই default: গত ১ মাস)
        private static void ResolveDates(string strFromDate, string strToDate,
            out DateTime? fromDate, out DateTime? toDate)
        {
            fromDate = null;
            toDate = null;

            if (string.IsNullOrEmpty(strFromDate) && string.IsNullOrEmpty(strToDate))
            {
                fromDate = DateTime.Now.AddMonths(-1);
                toDate = DateTime.Now;
                return;
            }

            DateTime f, t;
            if (DateTime.TryParse(strFromDate, out f)) fromDate = f;
            if (DateTime.TryParse(strToDate, out t)) toDate = t;
        }

        private static string StatusText(int status)
        {
            if (status == 1) return "Approved";
            if (status == 2) return "Rejected";
            return "Pending";
        }

        // ---------------- EXCEL ----------------
        [HttpGet]
        public async Task<ActionResult> ExportOrderCreditLimitExcel(
            int companyId, int? searchStatus, string strFromDate, string strToDate, int? subZoneId)
        {
            DateTime? fromDate, toDate;
            ResolveDates(strFromDate, strToDate, out fromDate, out toDate);

            var model = await _applicationManageService.GetOrderCreditLimitManageList(
                companyId, searchStatus, fromDate, toDate, subZoneId ?? 0);

            var list = model.ApplicationList != null ? model.ApplicationList.ToList() : null;
            if (list == null || !list.Any())
            {
                return RedirectToAction("OrderCreditLimitManageList",
                    new { companyId, searchStatus, strFromDate, strToDate, subZoneId });
            }

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Credit Limit");

                // Title
                ws.Cell(1, 1).Value = "Order Credit Limit Applications";
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Font.FontSize = 14;

                ws.Cell(2, 1).Value = "Period: "
                    + (fromDate.HasValue ? fromDate.Value.ToString("dd MMM yyyy") : "-")
                    + " to "
                    + (toDate.HasValue ? toDate.Value.ToString("dd MMM yyyy") : "-")
                    + "   |   Generated: " + DateTime.Now.ToString("dd MMM yyyy HH:mm");

                // Header
                string[] headers = {
            "ID", "Apply Date", "Credit Limit Amount", "Validity From", "Validity To",
            "Days", "Territory", "Customer Code", "Applicant", "Manager",
            "Reason", "Signatory Flow", "Status"
        };
                int headerRow = 4;
                for (int i = 0; i < headers.Length; i++)
                    ws.Cell(headerRow, i + 1).Value = headers[i];

                var hr = ws.Range(headerRow, 1, headerRow, headers.Length);
                hr.Style.Font.Bold = true;
                hr.Style.Font.FontColor = XLColor.White;
                hr.Style.Fill.BackgroundColor = XLColor.FromHtml("#4361EE");
                hr.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Data
                int r = headerRow + 1;
                foreach (var item in list)
                {
                    string flow = (item.SignatoryApprovals != null && item.SignatoryApprovals.Any())
                        ? string.Join(" > ", item.SignatoryApprovals.Select(s =>
                            s.EmployeeName + " (" + s.DesignationName + ") - " + s.StatusName))
                        : "";

                    ws.Cell(r, 1).SetValue(item.ApplicationId);

                    ws.Cell(r, 2).SetValue(item.ApplicationDate);
                    ws.Cell(r, 2).Style.DateFormat.Format = "dd-MMM-yyyy";

                    ws.Cell(r, 3).SetValue(item.CreditLimitAmount);
                    ws.Cell(r, 3).Style.NumberFormat.Format = "#,##0.00";

                    ws.Cell(r, 4).SetValue(item.StartDate);
                    ws.Cell(r, 4).Style.DateFormat.Format = "dd-MMM-yyyy";

                    ws.Cell(r, 5).SetValue(item.EndDate);
                    ws.Cell(r, 5).Style.DateFormat.Format = "dd-MMM-yyyy";

                    ws.Cell(r, 6).SetValue(item.DayCounts);
                    ws.Cell(r, 7).SetValue(item.SubZoneName ?? "");
                    ws.Cell(r, 8).SetValue(item.CustomerCode ?? "");
                    ws.Cell(r, 9).SetValue(item.ApplicantName ?? "");
                    ws.Cell(r, 10).SetValue(item.ManagerName ?? "");
                    ws.Cell(r, 11).SetValue(item.Reason ?? "");
                    ws.Cell(r, 12).SetValue(flow);
                    ws.Cell(r, 13).SetValue(StatusText(item.Status));
                    r++;
                }

                // Border, filter, freeze
                var all = ws.Range(headerRow, 1, r - 1, headers.Length);
                all.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                all.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                all.SetAutoFilter();
                ws.SheetView.FreezeRows(headerRow);

                // কলামের width নিজে সেট করা (AdjustToContents ব্যবহার না করে)
                double[] colWidths = { 8, 14, 18, 14, 14, 8, 22, 18, 22, 22, 40, 55, 12 };
                for (int i = 0; i < colWidths.Length; i++)
                    ws.Column(i + 1).Width = colWidths[i];

                ws.Column(11).Style.Alignment.WrapText = true;
                ws.Column(12).Style.Alignment.WrapText = true;
                ws.Range(headerRow + 1, 1, r - 1, headers.Length)
                  .Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    string fileName = "OrderCreditLimit_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                    return File(ms.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // ---------------- PDF ----------------
        [HttpGet]
        public async Task<ActionResult> ExportOrderCreditLimitPdf(
            int companyId, int? searchStatus, string strFromDate, string strToDate, int? subZoneId)
        {
            DateTime? fromDate, toDate;
            ResolveDates(strFromDate, strToDate, out fromDate, out toDate);

            var model = await _applicationManageService.GetOrderCreditLimitManageList(
                companyId, searchStatus, fromDate, toDate, subZoneId ?? 0);

            var list = model.ApplicationList != null ? model.ApplicationList.ToList() : null;
            if (list == null || !list.Any())
            {
                return RedirectToAction("OrderCreditLimitManageList",
                    new { companyId, searchStatus, strFromDate, strToDate, subZoneId });
            }

            var titleFont = iText.FontFactory.GetFont(iText.FontFactory.HELVETICA_BOLD, 15, new iText.BaseColor(31, 41, 55));
            var subFont = iText.FontFactory.GetFont(iText.FontFactory.HELVETICA, 9, new iText.BaseColor(107, 114, 128));
            var headFont = iText.FontFactory.GetFont(iText.FontFactory.HELVETICA_BOLD, 8, iText.BaseColor.WHITE);
            var bodyFont = iText.FontFactory.GetFont(iText.FontFactory.HELVETICA, 8, new iText.BaseColor(55, 65, 81));

            var doc = new iText.Document(iText.PageSize.A4.Rotate(), 20, 20, 25, 25);

            using (var ms = new MemoryStream())
            {
                iTextPdf.PdfWriter.GetInstance(doc, ms);
                doc.Open();

                doc.Add(new iText.Paragraph("Order Credit Limit Applications", titleFont));
                doc.Add(new iText.Paragraph(
                    "Period: "
                    + (fromDate.HasValue ? fromDate.Value.ToString("dd MMM yyyy") : "-")
                    + " to "
                    + (toDate.HasValue ? toDate.Value.ToString("dd MMM yyyy") : "-")
                    + "   |   Total: " + list.Count
                    + "   |   Generated: " + DateTime.Now.ToString("dd MMM yyyy HH:mm"),
                    subFont)
                { SpacingAfter = 10f });

                string[] headers = {
            "ID", "Apply Date", "Credit Limit", "Validity Period", "Territory",
            "Customer Code", "Applicant", "Manager", "Reason", "Status"
        };

                var table = new iTextPdf.PdfPTable(headers.Length) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 5, 10, 10, 16, 11, 10, 11, 11, 16, 8 });
                table.HeaderRows = 1;

                foreach (var h in headers)
                {
                    table.AddCell(new iTextPdf.PdfPCell(new iText.Phrase(h, headFont))
                    {
                        BackgroundColor = new iText.BaseColor(67, 97, 238),
                        HorizontalAlignment = iText.Element.ALIGN_CENTER,
                        VerticalAlignment = iText.Element.ALIGN_MIDDLE,
                        Padding = 5,
                        BorderColor = iText.BaseColor.WHITE
                    });
                }

                bool alt = false;
                foreach (var item in list)
                {
                    var bg = alt ? new iText.BaseColor(249, 250, 251) : iText.BaseColor.WHITE;
                    alt = !alt;

                    string[] cells = {
                item.ApplicationId.ToString(),
                item.ApplicationDate.ToString("dd MMM yyyy"),
                Convert.ToString(item.CreditLimitAmount),
                item.StartDate.ToString("dd MMM yyyy") + " - " + item.EndDate.ToString("dd MMM yyyy")
                    + " (" + item.DayCounts + " days)",
                item.SubZoneName ?? "",
                item.CustomerCode ?? "",
                item.ApplicantName ?? "",
                item.ManagerName ?? "",
                item.Reason ?? "",
                StatusText(item.Status)
            };

                    for (int i = 0; i < cells.Length; i++)
                    {
                        table.AddCell(new iTextPdf.PdfPCell(new iText.Phrase(cells[i], bodyFont))
                        {
                            BackgroundColor = bg,
                            Padding = 4,
                            VerticalAlignment = iText.Element.ALIGN_MIDDLE,
                            HorizontalAlignment = (i == 0 || i == 9) ? iText.Element.ALIGN_CENTER : iText.Element.ALIGN_LEFT,
                            BorderColor = new iText.BaseColor(229, 231, 235)
                        });
                    }
                }

                doc.Add(table);
                doc.Close();

                string fileName = "OrderCreditLimit_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                return File(ms.ToArray(), "application/pdf", fileName);
            }
        }

        #endregion
    }
}
