using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;



namespace KGERP.Controllers
{
    [CheckSession]
    public class StockAdjustController : BaseController
    {

        private readonly IStockAdjustService stockAdjustService;
        private readonly WareHouseService _service;

        public StockAdjustController(IStockAdjustService stockAdjustService, WareHouseService wareHouseService)
        {
            _service = wareHouseService;
            this.stockAdjustService = stockAdjustService;
        }
        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddMonths(-2);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }
            StockAdjustModel stockTransferModel = new StockAdjustModel();

            stockTransferModel = await stockAdjustService.GetStockAdjusts(companyId, fromDate, toDate);

            stockTransferModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            stockTransferModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(stockTransferModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> Index(StockAdjustModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(Index), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }
        
        [HttpGet]
        public async Task<ActionResult> IssueIndex(int companyId, DateTime? fromDate, DateTime? toDate, int status = 0)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddMonths(-2);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }
            IssueVm model = new IssueVm();

            model = await stockAdjustService.GetStockIssues(companyId, fromDate, toDate, status);

            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(model);
        }
        [HttpPost]
        
        public async Task<ActionResult> IssueIndex(IssueVm model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
             
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(IssueIndex), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate, status = model.Status });
        }

        [HttpGet]
        public async Task<ActionResult> IssueSlave(int companyId = 0, int issueId = 0)
        {
            IssueVm model = new IssueVm();

            if (issueId == 0)
            {
                model.CompanyId = companyId;
                model.Status = (int)EnumIssueStatus.Draft;
            }
            else if (issueId > 0)
            {
                model = await Task.Run(() => stockAdjustService.IssueSlaveGet(companyId, issueId));

            }
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> IssueSlave(IssueVm model)
        {

            if (model.ActionEum == ActionEnum.Add)
            {
                if (model.IssueId == 0)
                {
                    if(model.IssueTo > 0)
                    {
                        model.IssueId = await stockAdjustService.IssueAdd(model);
                    }
                    else
                    {
                        return RedirectToAction(nameof(IssueSlave), new { companyId = model.CompanyId, issueId = model.IssueId });
                    }
                }
                await stockAdjustService.IssueSlaveAdd(model);
            }
            else if (model.ActionEum == ActionEnum.Edit)
            {
                await stockAdjustService.IssueSlaveEdit(model);
            }
            return RedirectToAction(nameof(IssueSlave), new { companyId = model.CompanyId, issueId = model.IssueId });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteIssueSlave(IssueVm model)
        {
            if (model.ActionEum == ActionEnum.Delete)
            {
                model.IssueDetailId = await stockAdjustService.IssueSlaveDelete(model.IssueDetailId);
            }
            return RedirectToAction(nameof(IssueSlave), new { companyId = model.CompanyId, issueId = model.IssueId });
        }

        public async Task<JsonResult> SingleIssueSlave(int id)
        {
            var model = await stockAdjustService.GetSingleIssueSlave(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SingleIssue(int id)
        {
            IssueVm model = new IssueVm();
            model = await stockAdjustService.GetSingleIssue(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> SubmitIssueSlave(IssueVm model)
        {
            model.IssueId = await stockAdjustService.IssueSubmit(model.IssueId);
            return RedirectToAction(nameof(IssueSlave), new { companyId = model.CompanyId, issueId = model.IssueId });
        }
        public JsonResult GetAutoCompleteEmployeeGet( string prefix)
        {
            var products = stockAdjustService.GetAutoCompleteEmployee( prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public ActionResult CreateOrEdit(int id)
        {
            StockAdjustModel model = stockAdjustService.GetStockAdjust(id);
            return View(model);

        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(StockAdjustModel model)
        {
            if (model.StockAdjustId <= 0)
            {
                stockAdjustService.SaveStockAdjust(0, model);
            }
            else
            {
                stockAdjustService.SaveStockAdjust(model.StockAdjustId, model);
            }

            return RedirectToAction("Index", new { companyId = model.CompanyId });
        }

        [HttpGet]
        public async Task<ActionResult> ItemAdjustment(int companyId=8, int stockAdjustId = 0)
        {
            VMStockAdjustDetail vmStockAdjustDetail = new VMStockAdjustDetail();
            if (stockAdjustId == 0)
            {
                vmStockAdjustDetail = new VMStockAdjustDetail()
                {
                    CompanyFK = companyId
                };
            }
            else if (stockAdjustId > 0)
            {
                vmStockAdjustDetail = await _service.ISSRMAdjustmentDetailGet(companyId, stockAdjustId);
            }
            return View(vmStockAdjustDetail);
        }

        [HttpPost]
        public async Task<ActionResult> ItemAdjustment(VMStockAdjustDetail vmStockAdjustDetail)
        {
            if (vmStockAdjustDetail.ActionEum == ActionEnum.Add)
            {
                if (vmStockAdjustDetail.StockAdjustId == 0)
                {
                    vmStockAdjustDetail.StockAdjustId = await _service.FeedStockAdjustAdd(vmStockAdjustDetail);
                }
                await _service.FeedRMAdjustDetailAdd(vmStockAdjustDetail);
            }
            else if (vmStockAdjustDetail.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitRMAdjusts(vmStockAdjustDetail);//need to add code for feed inside this function

            }
            return RedirectToAction(nameof(ItemAdjustment), new { companyId = vmStockAdjustDetail.CompanyFK, stockAdjustId = vmStockAdjustDetail.StockAdjustId });
        }
    }
}