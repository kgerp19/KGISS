using KGERP.Data.Models;
using KGERP.Service.CommonModels.Model;
using KGERP.Service.Implementation.QualityExceptionComplaints;
using KGERP.Service.Interface;
using KGERP.Utility;
using KGERP.Utility.Interface;
using NPOI.SS.Formula.Functions;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers.QualityExceptionComplaintCon
{
    public class QualityExceptionComplaintController : BaseController
    {
        private readonly IDropDownItemService _dropDownItemService;
        private readonly IDropdownService _dropdownService;
        private readonly IQualityExceptionComplaintService _qualityExceptionComplaintService;

        public QualityExceptionComplaintController(IDropDownItemService dropDownItemService, IDropdownService dropdownService, IQualityExceptionComplaintService qualityExceptionComplaintService)
        {
            _dropDownItemService = dropDownItemService;
            _dropdownService = dropdownService;
            _qualityExceptionComplaintService = qualityExceptionComplaintService;
        }

        [HttpGet]
        public async Task<ActionResult> QualityExceptionComplaint(int companyId=0, long QualityExceptionComplaintId = 0, long QualityExceptionComplaintDetailId = 0)
        {
            QualityExceptionComplaintDetailVM dataModel = new QualityExceptionComplaintDetailVM();
            dataModel = await _qualityExceptionComplaintService.GetAllDataOfQualityExceptionComplaint(QualityExceptionComplaintId, QualityExceptionComplaintDetailId);
            dataModel.CompanyId = companyId <= 0 ? Common.GetCompanyId() : companyId;
            dataModel.DDLCustomer = await _dropDownItemService.GetDDLCustomerByCompany(dataModel.CompanyId);
            dataModel.DDLOrder = _dropdownService.DefaultDDL();
            dataModel.DDLOrderDelivery = _dropdownService.DefaultDDL();
            dataModel.DDLOrderDeliveryDetail = _dropdownService.DefaultDDL();
            dataModel.DDLEmployee = _dropdownService.RenderDDL(await _dropDownItemService.GetDDLAllEmployeeByCompanyId(), true);
            return View(dataModel);
        }
        [HttpPost]
        public async Task<ActionResult> QualityExceptionComplaint(QualityExceptionComplaintDetailVM detailVM)
        {
            var result = await _qualityExceptionComplaintService.SaveAndUpdateDataQualityExceptionMasterChildAndMapTable(detailVM);
            return RedirectToAction(nameof(QualityExceptionComplaint), new {QualityExceptionComplaintId = result.QualityExceptionComplaintId, QualityExceptionComplaintDetailId = result.QualityExceptionComplaintDetailId });
        }

        [HttpPost]
        public async Task<JsonResult> AddQualityExceptionItem(int DropDownTypeId, string QualityException)
        {
            int CompanyId =Common.GetCompanyId();
            var result = await _qualityExceptionComplaintService.AddQualityExceptionItem(DropDownTypeId, QualityException, CompanyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetDDLOrderByCustomer(long VendorId)
        {
            var result = _dropdownService.RenderDDL(await _dropDownItemService.GetDDLOrderByCustomer(VendorId), true);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetDDLOrderDeliveryByOrderMaster(long OrderMasterId)
        {
            var result = _dropdownService.RenderDDL(await _dropDownItemService.GetDDLOrderDeliveryByOrderMaster(OrderMasterId), true);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetDDLOrderDeliveryDetailsByOrderDelivery(long OrderDeliveryId)
        {
            var result = _dropdownService.RenderDDL(await _dropDownItemService.GetDDLOrderDeliveryDetailsByOrderDelivery(OrderDeliveryId), true);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetDataOfOrderDeliveryDetailsByOrderDeliveryDetailId(long OrderDeliveryDetaiId)
        {
            var result = await _qualityExceptionComplaintService.GetDataOfOrderDeliveryDetailsByOrderDeliveryDetailId(OrderDeliveryDetaiId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> QualityExceptionComplaintDetailsSubmit(QualityExceptionComplaintDetailVM detailVM)
        {

            var result = await _qualityExceptionComplaintService.QualityExceptionComplaintDetailsSubmit(detailVM);
            return RedirectToAction(nameof(QualityExceptionComplaint), new { QualityExceptionComplaintId = result.QualityExceptionComplaintId, QualityExceptionComplaintDetailId = result.QualityExceptionComplaintDetailId });
        }

        [HttpPost]
        public async Task<ActionResult> ProductionInChargeSubmition(QualityExceptionComplaintDetailVM detailVM)
        {

            var result = await _qualityExceptionComplaintService.ProductionInChargeSubmition(detailVM);

            return RedirectToAction(nameof(QualityExceptionComplaint), new { QualityExceptionComplaintId = result.QualityExceptionComplaintId, QualityExceptionComplaintDetailId = result.QualityExceptionComplaintDetailId });
        }

        [HttpGet]
        public async Task<ActionResult> QualityExceptionComplaintList(DateTime? formDate, DateTime? toDate)
        {
            QualityExceptionComplaintDetailVM detailVM = new QualityExceptionComplaintDetailVM();
            if (formDate == null || toDate == null)
            {
                formDate = DateTime.Now.AddMonths(-1);
                toDate = DateTime.Now.AddMonths(1);
            }
            detailVM = await Task.Run(() => _qualityExceptionComplaintService.GetQualityExceptionComplaintList(formDate, toDate));
            detailVM.FormDate = formDate.Value.ToShortDateString();
            detailVM.ToDate = toDate.Value.ToShortDateString();
            detailVM.CompanyId = Common.GetCompanyId();
            return View(detailVM);
        }

        [HttpPost]
        public  ActionResult QualityExceptionComplaintListFilter(DateTime? formDate, DateTime? toDate)
        {
            return RedirectToAction(nameof(QualityExceptionComplaintList), new { formDate, toDate });
        }

        [HttpGet]
        public async Task<JsonResult> QualityExceptionComplaintDelete(long QualityExceptionComplaintId)
        {
            RResult rResult = new RResult();
            rResult = await _qualityExceptionComplaintService.QualityExceptionComplaintDelete(QualityExceptionComplaintId);
            return Json(rResult,JsonRequestBehavior.AllowGet);
        }
    }
}