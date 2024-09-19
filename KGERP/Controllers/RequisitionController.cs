using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class RequisitionController : BaseController
    {
        private readonly IRequisitionService requisitionService;
        private readonly IStoreService storeService;
        private readonly IStockInfoService stockInfoService;
        private readonly IVendorService vendorService;
        public RequisitionController(IRequisitionService requisitionService, IStoreService storeService, IStockInfoService stockInfoService,
            IVendorService vendorService)
        {
            this.requisitionService = requisitionService;
            this.storeService = storeService;
            this.stockInfoService = stockInfoService;
            this.vendorService = vendorService;
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
            RequisitionModel storeModel = new RequisitionModel();
            storeModel = await requisitionService.RequisitionList(companyId, fromDate, toDate);
            storeModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            storeModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(storeModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> Index(RequisitionModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(Index), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }



        
        public async Task<ActionResult> CreateOrEdit(int companyId = 0, int requisitionId = 0)
        {
            RequisitionModel model = new RequisitionModel();
            DateTime date = DateTime.Now.AddDays(-3);
            model.CurrentDate = date.ToString("yyyy-MM-dd");
            if (requisitionId > 0)
            {
                model = await Task.Run(() => requisitionService.GetRequisition(companyId, requisitionId));

            }
            else
            {
                model.CompanyId = companyId;
                model.Status = (int)EnumPOStatus.Draft;
            }
            return View(model);
        }

        
        [HttpPost]
        public async Task<ActionResult> CreateOrEdit(RequisitionModel model)
        {
            if (model.ActionEum == ActionEnum.Add)
            {
                if (model.RequisitionId == 0)
                {
                    model.RequisitionId = await requisitionService.CreateProductionRequisition(model);

                }
                await requisitionService.CreateProductionRequisitionItem(model);
            }
            else if (model.ActionEum == ActionEnum.Edit)
            {
               await requisitionService.EditProductionReqisitionDetail(model);
            }
            return RedirectToAction(nameof(CreateOrEdit), new { companyId = model.CompanyId, requisitionId = model.RequisitionId });
        
    }
        
        [HttpPost]
        public ActionResult SubmitRequisition(RequisitionModel model)
        {
            //need to implements 

            return View(model);
        }
        
        [HttpGet]
        public JsonResult GetProcessLossAmount(int productId)
        {
            var data = requisitionService.GetProcessLossAmount(productId);
            return new JsonResult { Data = data.ProcessLoss, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        
        public ActionResult DeleteRequisiton(int requisitionId)
        {
            requisitionService.DeleteRequisition(requisitionId);
            return RedirectToAction("Index", new { companyId = Session["CompanyId"] });
        }


        //// GET: RequisitionDeliver
        //
        //[HttpGet]
        //public ActionResult RequisitionDeliverIndex(int companyId)
        //{
        //    RequisitionModel requisitionModel1 = new RequisitionModel();
        //    if (companyId > 0)
        //    {
        //        requisitionModel1 =  requisitionService.RequisitionDeliveryPendingList(companyId);
        //    }
        //    return View(requisitionModel1);
        //}

        
        [HttpGet]
        public async Task<ActionResult> RequisitionDeliverIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            RequisitionModel storeModel = new RequisitionModel();

            storeModel = await requisitionService.RequisitionDeliveryPendingList(companyId, fromDate, toDate);
            storeModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            storeModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(storeModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> RequisitionDeliverIndex(RequisitionModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return  RedirectToAction(nameof(RequisitionDeliverIndex), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }


        
        [HttpGet]
        public ActionResult RequisitionDeliverEdit(int companyId, int requisitionId)
        {
            RequisitionDeliverViewModel vm = new RequisitionDeliverViewModel();
            vm.Requisition = requisitionService.GetRequisitionById(requisitionId);
             vm.RequisitionItems = requisitionService.GetRequisitionItems(requisitionId);
            var strDeliveryDate = vm.Requisition.DeliveredDate.ToString();
            if (vm.Requisition.RequisitionStatus == "N" )
            {
                var delveriNo = vm.Requisition.RequisitionNo;
                var no = delveriNo.Substring(1);
                vm.Requisition.DeliveryNo = "D" + no;
            }
            else if(vm.Requisition.RequisitionStatus == "D" || vm.Requisition.RequisitionStatus == "I")
            {
                vm.Requisition = storeService.FeedRequisitionPushGet(companyId, requisitionId);
            }
            else
            {
                GetRequisitionDeliverRawMaterials(requisitionId, strDeliveryDate);
            }

            //vm.RequisitonItemDetails = requisitionService.GetRequisitionItemDetails(requisitionId);
            //vm.Requisition.FormulaMessage = requisitionService.GetFormulaMessage(requisitionId);
            //vm.RequisitionItems = requisitionService.GetRequisitionItems(requisitionId);

            //foreach (RequisitionItemDetailModel item in vm.RequisitonItemDetails)
            //{
            //    if (item.BalanceQty <= 0)
            //    {
            //        count = count + 1;
            //    }
            //}
            //ViewBag.Count = count;
           
            return View(vm);
        }

        
        [HttpGet]
        public PartialViewResult GetRequisitionDeliverRawMaterials(int requisitionId, string strDeliveryDate)
        {
            int count = 0;
            DateTime deliveryDate = strDeliveryDate == null ? DateTime.Now : Convert.ToDateTime(strDeliveryDate);
            List<RequisitionItemDetailModel> requisitonItemDetails = requisitionService.GetRequisitionItemDetails(requisitionId, deliveryDate);
            ViewBag.FormulaMessage = requisitionService.GetFormulaMessage(requisitionId);
            foreach (RequisitionItemDetailModel item in requisitonItemDetails)
            {
                if (item.BalanceQty <= 0)
                {
                    count = count + 1;
                }
            }
            ViewBag.Count = count;
            return PartialView("~/Views/Requisition/_requisitionDeliverRawMaterials.cshtml", requisitonItemDetails);
        }

        
        [HttpPost]
        public ActionResult RequisitionDeliverEdit(RequisitionDeliverViewModel model)
        {
            long result = -1;
            result = requisitionService.CreateOrEdit(model.Requisition);
            if (result>0)
            {
                TempData["message"] = "Raw Materials Delivered Successfully";
            }
            return RedirectToAction("RequisitionDeliverEdit", new { companyId = model.Requisition.CompanyId, requisitionId = result });
        }


        

        
        [HttpGet]
        public async Task<ActionResult> RequisitionIssueIndex(int companyId, DateTime? fromDate, DateTime? toDate)
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
            RequisitionModel storeModel = new RequisitionModel();
            storeModel = await requisitionService.RequisitionIssuePendingList(companyId, fromDate, toDate);
            storeModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            storeModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(storeModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> RequisitionIssueIndex(RequisitionModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(RequisitionIssueIndex), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate });
        }


        
        [HttpGet]
        public async Task<ActionResult> RequisitionIssueEdit(int companyId, int requisitionId)
        {
            StoreViewModel vm = new StoreViewModel();
            vm.Requisition = new RequisitionModel();
            vm.Requisition = requisitionService.GetRequisitionById(requisitionId);
            if (vm.Requisition.RequisitionStatus == "I")
            {
                vm.Requisition = storeService.FeedRequisitionPushGet(companyId, requisitionId);
                return View(vm);
            }
            else
            {
                string productType = "F";
                vm.StockInfos = stockInfoService.GetFactorySelectModels(companyId);
                vm.Requisition =await requisitionService.GetRequisition(companyId, requisitionId);
                vm.RequistionItems = requisitionService.GetRequisitionItems(requisitionId);
                vm.Store = storeService.GetRequisitionItemStore(0, productType);
                vm.Store.RequisitionId = vm.Requisition.RequisitionId;
                vm.Store.RequisitionNo = vm.Requisition.RequisitionNo;

                vm.Store.CompanyId = companyId;
                vm.Requisition.CompanyId = companyId;
                return View(vm);
            }
            
        }

        //
        //[HttpGet]
        //public PartialViewResult GetRequitionItemIssueStatus(int requisitionId, int storeId)
        //{

        //    List<RequisitionItemModel> requisitionItems = requisitionService.GetRequisitionItemIssueStatus(requisitionId);

        //    return PartialView("~/Views/RequisitionIssue/_productionIssue.cshtml", requisitionItems);
        //}

        //
        //[HttpGet]
        //public PartialViewResult ChangeRequitionItemIssueStatus(int requisitionId, int storeId, int productId, string reciveCode, string receivedDate, decimal outputQty, decimal overHead, decimal processLoss)
        //{
        //    DateTime receiveDate = Convert.ToDateTime(receivedDate);
        //    StoreModel store = new StoreModel()
        //    {
        //        RequisitionId = requisitionId,
        //        StoreId = storeId,
        //        ProductId = productId,
        //        ReceivedCode = reciveCode,
        //        ReceivedDate = receiveDate
        //    };
        //    var data = requisitionService.FinshProductStore(store, outputQty, overHead, processLoss);
        //    List<RequisitionItemModel> requisitionItems = requisitionService.GetRequisitionItemIssueStatus(requisitionId);
        //    return PartialView("~/Views/RequisitionIssue/_productionIssue.cshtml", requisitionItems);


        
        [HttpGet]
        public PartialViewResult GetProductionItems(int companyId ,int requisitionId, string strIssueDate)
        {
            //int companyId = Convert.ToInt32(Session["CompanyId"]);
            DateTime issueDate = strIssueDate == null ? DateTime.Now : Convert.ToDateTime(strIssueDate);
            List<RequisitionItemModel> model = requisitionService.GetProductionItems(companyId, requisitionId, issueDate);
            return PartialView("~/Views/Requisition/_requisitionItemIssue.cshtml", model);
        }

    }

}