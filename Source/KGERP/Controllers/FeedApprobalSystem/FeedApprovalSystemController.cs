using DocumentFormat.OpenXml.VariantTypes;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.OrderApproval;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace KGERP.Controllers.FeedApprobalSystem
{
    [CheckSession]

    public class FeedApprovalSystemController : Controller
    {
        private readonly IOrderApprovalService orderApprovalService;
        public FeedApprovalSystemController(IOrderApprovalService orderApprovalService)
        {
            this.orderApprovalService = orderApprovalService;
        }
        // GET: FeedApprovalSystem
        [HttpGet]
        public async Task<ActionResult> Index(int companyId, DateTime? fromDate, DateTime? toDate, SignatoryStatusEnum? SignatoryStatus)
        {
            var res = await orderApprovalService.LoadApprovalData(fromDate, toDate, SignatoryStatus);
            ViewBag.CompanyId = companyId;
            return View(res);
        }


        [HttpGet]
        public async Task<ActionResult> IndexForSeed(int companyId, DateTime? fromDate, DateTime? toDate, SignatoryStatusEnum? SignatoryStatus)
        {
            var res = await orderApprovalService.LoadApprovalDataSeed(fromDate, toDate, SignatoryStatus);
            ViewBag.CompanyId = companyId;
            return View(res);
        }

        [HttpGet]
        public async Task<ActionResult> RejectedOrdersIndex(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            var res = await orderApprovalService.LoadRejectedOrderData(fromDate, toDate);
            ViewBag.CompanyId = companyId;
            return View(res);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateSignatoryStatus(EmployeeClearanceVM model)
        {
            var result = await orderApprovalService.UpdateSignatoryApproval((int)model.Id, model.SignatoryStatus, model.Comment);
            return RedirectToAction("Index", new { companyId = model.UserCompanyId });
        }









        [HttpPost]
        public async Task<ActionResult> UpdateSignatoryStatusFromDetail(EmployeeClearanceVM model)
        {
            
            var result = await orderApprovalService.UpdateSignatoryApproval((int)model.Id, model.SignatoryStatus, model.Comment);
            return RedirectToAction("FeedProcurementSalesOrderSlave", "Procurement", new { companyId = model.UserCompanyId, productType ="F" , result });
        }

        [HttpGet]
        public async Task<ActionResult> GetApprovalList(int orderMasterId)
        {
            var approvalList = await orderApprovalService.GetAllApproval(orderMasterId);
            return Json(approvalList, JsonRequestBehavior.AllowGet);
        }
    }
}