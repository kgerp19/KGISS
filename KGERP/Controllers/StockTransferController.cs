using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class StockTransferController : BaseController
    {
        private IStockTransferService stockTransferService;
        private readonly IStockInfoService stockInfoService;
        public StockTransferController(IStockTransferService stockTransferService, IStockInfoService stockInfoService)
        {
            this.stockTransferService = stockTransferService;
            this.stockInfoService = stockInfoService;
        }

        
        [HttpGet]
        public async Task<ActionResult> GcclRmTransferIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            StockTransferModel stockTransferModel = new StockTransferModel();


            stockTransferModel = await stockTransferService.GetGcclRmTransfer(companyId, fromDate, toDate);

            stockTransferModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            stockTransferModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(stockTransferModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> GcclRmTransferIndex(StockTransferModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);


            return RedirectToAction(nameof(GcclRmTransferIndex), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }




       //ani

        
        [HttpGet]
        public async Task<ActionResult> KFMALRmTransferIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            StockTransferModel stockTransferModel = new StockTransferModel();


            stockTransferModel = await stockTransferService.GetGcclRmTransfer(companyId, fromDate, toDate);

            stockTransferModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            stockTransferModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(stockTransferModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> KFMALRmTransferIndex(StockTransferModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);


            return RedirectToAction(nameof(GcclRmTransferIndex), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }


        [HttpGet]
        public async Task<ActionResult> KFMALRmTransferSlave(int companyId = 0, int stockTransferId = 0)
        {
            StockTransferModel model = new StockTransferModel();

            if (stockTransferId == 0)
            {
                model.CompanyId = companyId;
                model.Status = (int)EnumIssueStatus.Draft;
                model.StockToList = stockInfoService.GetStockInfoSelectModels(companyId);
                model.StockFromList = stockInfoService.GetStockInfoSelectModels(companyId);
                model.TransferDate = DateTime.Now;
            }
            else if (stockTransferId > 0)
            {
                model = await Task.Run(() => stockTransferService.GetStockTransferSlave(companyId, stockTransferId));

            }
            return View(model);
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
            StockTransferModel stockTransferModel = new StockTransferModel();


            stockTransferModel = await stockTransferService.GetStockTransfer(companyId, fromDate, toDate);

            stockTransferModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            stockTransferModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(stockTransferModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> Index(StockTransferModel model)
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
        public ActionResult Create()
        {
            var companyId = Convert.ToInt32(Session["CompanyId"]);
            StockTransferViewModel vm = new StockTransferViewModel
            {
              //  StockTransfer = stockTransferService.GetStockTransfer(0),
                StockFrom = stockInfoService.GetStockInfoSelectModels(companyId),
                StockTo = stockInfoService.GetStockInfoSelectModels(companyId)
            };
            return View(vm);

        }


        [HttpGet]
        public async Task<ActionResult> GcclRmTransferSlave(int companyId = 0, int stockTransferId = 0)
        {
            StockTransferModel model = new StockTransferModel();

            if (stockTransferId == 0)
            {
                model.CompanyId = companyId;
                model.Status = (int)EnumIssueStatus.Draft;
                model.StockToList = stockInfoService.GetStockInfoSelectModels(companyId);
                model.StockFromList = stockInfoService.GetStockInfoSelectModels(companyId);
                model.TransferDate = DateTime.Now;
            }
            else if (stockTransferId > 0)
            {
                model = await Task.Run(() => stockTransferService.GetStockTransferSlave(companyId, stockTransferId));

            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> StockTransferSlave(int companyId = 0, int stockTransferId = 0)
        {
            StockTransferModel model = new StockTransferModel();

            if (stockTransferId == 0)
            {
                model.CompanyId = companyId;
                model.Status = (int)EnumIssueStatus.Draft;
                model.StockToList = stockInfoService.GetStockInfoSelectModels(companyId);
                model.StockFromList = stockInfoService.GetStockInfoSelectModels(companyId);
                model.TransferDate = DateTime.Now;
            }
            else if (stockTransferId > 0)
            {
                model = await Task.Run(() => stockTransferService.GetStockTransferSlave(companyId, stockTransferId));

            }
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> StockTransferSlave(StockTransferModel model)
        {

            if (model.ActionEum == ActionEnum.Add)
            {
                if (model.StockTransferId == 0)
                {
                    model.StockTransferId = await stockTransferService.StockTransferAdd(model);
                }
                await stockTransferService.StockTransferSlaveAdd(model);
            }
            else if (model.ActionEum == ActionEnum.Edit)
            {
                await stockTransferService.StockTransferSlaveEdit(model);
            }
            return RedirectToAction(nameof(StockTransferSlave), new { companyId = model.CompanyId, stockTransferId = model.StockTransferId });
        }
        [HttpPost]
        public async Task<ActionResult> StockTransferSubmit(StockTransferModel model)
        {

            if (model.Status == (int)EnumStockTransferStatus.Draft)
            {
                if (model.StockTransferId >0)
                {
                    model.StockTransferId = await stockTransferService.StockTransferSubmit(model.StockTransferId);
                }
            }
            return RedirectToAction(nameof(StockTransferSlave), new { companyId = model.CompanyId, stockTransferId = model.StockTransferId });
        }
        [HttpPost]
        public async Task<ActionResult> DeleteStockTransferSlave(StockTransferModel model)
        {
            if (model.ActionEum == ActionEnum.Delete)
            {
                model.StockTransferDetailId = await stockTransferService.StockTransferSlaveDelete(model.StockTransferDetailId);
            }
            return RedirectToAction(nameof(StockTransferSlave), new { companyId = model.CompanyId,stockTransferId = model.StockTransferId });
        }
        public async Task<JsonResult> SingleStockTransferSlave(int id)
        {
            var model = await stockTransferService.GetSingleStockTransferSlave(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public JsonResult AutoCompleteProduct(int companyId, string prefix, string productType)
        {
            var products = stockTransferService.GetProductAutoComplete(companyId,prefix, productType);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetStockAvailableQuantity(int companyId, int productId, int stockFrom, string selectedDate)
        {
            // string  date = selectedDate.ToString("dd/MM/yyyy");
             
            ProductCurrentStockModel stockckAvailableQuantity = stockTransferService.GetStockckAvailableQuantity(companyId,productId, stockFrom, selectedDate);
            return Json(stockckAvailableQuantity, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public async Task<ActionResult> StockReceiveIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            StockTransferModel stockTransferModel = new StockTransferModel();

            stockTransferModel = await stockTransferService.GetStockTransfer(companyId, fromDate, toDate);
            stockTransferModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            stockTransferModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(stockTransferModel);
        }

        [HttpPost]
        
        public async Task<ActionResult> StockReceiveIndex(StockTransferModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);

            return RedirectToAction(nameof(StockReceiveIndex), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }

        
        [HttpGet]
        public async Task<ActionResult> StockReceiveUpdate(int companyId, int stockTransferId=0)
        {
            StockTransferModel vm = new StockTransferModel();
            vm = await stockTransferService.GetStockTransferById(companyId, stockTransferId);
            return View(vm);
        }

        
        [HttpPost]
        public async Task<ActionResult> StockReceiveUpdate(StockTransferModel vm)
        {
            await stockTransferService.StockReceivedUpdate(vm);
            return RedirectToAction(nameof(StockReceiveUpdate), new { companyId= vm.CompanyId, stockTransferId = vm.StockTransferId });
        }
        //[HttpPost]
        //public async Task<ActionResult> StockReceivedSubmit(StockTransferModel model)
        //{

        //    if (model.Status == (int)EnumStockTransferStatus.Submitted)
        //    {
        //        if (model.StockTransferId > 0)
        //        {
        //            model.StockTransferId = await stockTransferService.StockReceivedUpdate(model);
        //            model.StockTransferId = await stockTransferService.StockReceivedSubmit(model.StockTransferId);
        //        }
        //    }
        //    return RedirectToAction(nameof(StockTransferSlave), new { companyId = model.CompanyId, stockTransferId = model.StockTransferId });
        //}

        
        [HttpGet]
        public ActionResult StockReceiveConfirmStatus(int stockTransferDetailId, decimal receiveQty, int stockTransferId, string receiveDate, int productId)
        {
            var companyId = Convert.ToInt32(Session["CompanyId"]);
            stockTransferService.ConfirmStockReceive(stockTransferDetailId, receiveQty, stockTransferId, Convert.ToDateTime(receiveDate), productId, companyId);
            return Json(new { stockTransferId }, JsonRequestBehavior.AllowGet);

        }

      
    }
}