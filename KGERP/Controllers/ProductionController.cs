using KGERP;
using KGERP.Service.Implementation.Production;
using KGERP.Services.Procurement;
using KGERP.Services.Production;
using KGERP.Utility;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KG.App.Controllers
{
    [CheckSession]
    public class ProductionController : Controller
    {
        private HttpContext httpContext;
        private readonly ProductionService _service;


        public ProductionController(ProductionService productionService)
        {
            _service = productionService;
        }
        public JsonResult GetAutoCompleteSupplierGet(string prefix, int companyId)
        {
            var products = _service.GetAutoCompleteSupplier(prefix, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        #region Production Process
        [HttpGet]
        public async Task<ActionResult> ProdReferenceSlaveReport(int companyId = 0, int prodReferenceId = 0)
        {
            VMProdReferenceSlave vmProdReferenceSlave = new VMProdReferenceSlave();
            if (prodReferenceId > 0)
            {
                vmProdReferenceSlave = await Task.Run(() => _service.ProdReferenceSlaveGet(companyId, prodReferenceId));

            }
            return View(vmProdReferenceSlave);
        }
        [HttpGet]
        public async Task<ActionResult> ProdReferenceSlave(int companyId = 0, int prodReferenceId = 0)
        {
            VMProdReferenceSlave vmPurchaseOrderSlave = new VMProdReferenceSlave();

            if (prodReferenceId == 0)
            {
                vmPurchaseOrderSlave.CompanyFK = companyId;

            }
            else if (prodReferenceId > 0)
            {
                vmPurchaseOrderSlave = await Task.Run(() => _service.ProdReferenceSlaveGet(companyId, prodReferenceId));

            }

            return View(vmPurchaseOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> ProdReferenceSlave(VMProdReferenceSlave vmProdReferenceSlave)
        {

            if (vmProdReferenceSlave.ActionEum == ActionEnum.Add)
            {
                if (vmProdReferenceSlave.ProdReferenceId == 0)
                {
                    vmProdReferenceSlave.ProdReferenceId = await _service.Prod_ReferenceAdd(vmProdReferenceSlave);
                }
                await _service.ProdReferenceSlaveAdd(vmProdReferenceSlave);
            }
            //else if (vmProdReferenceSlave.ActionEum == ActionEnum.Edit)
            //{
            //    //Delete
            //    await _service.ProcurementPurchaseOrderSlaveEdit(vmProdReferenceSlave);
            //}
            return RedirectToAction(nameof(ProdReferenceSlave), new { companyId = vmProdReferenceSlave.CompanyFK, prodReferenceId = vmProdReferenceSlave.ProdReferenceId });
        }
        public JsonResult AutoCompleteMaterialReceivesGet(int companyId, string prefix)
        {
            var products = _service.GetAutoCompleteMaterialReceives(companyId, prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> ProdReferenceSlaveByChallan(int companyId = 0, int prodReferenceId = 0)
        {
            VMProdReferenceSlave vmPurchaseOrderSlave = new VMProdReferenceSlave();

            if (prodReferenceId == 0)
            {
                vmPurchaseOrderSlave.CompanyFK = companyId;

            }
            else if (prodReferenceId > 0)
            {
                vmPurchaseOrderSlave = await Task.Run(() => _service.ProdReferenceSlaveGet(companyId, prodReferenceId));

            }

            return View(vmPurchaseOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> ProdReferenceSlaveByChallan(VMProdReferenceSlave vmProdReferenceSlave)
        {

            if (vmProdReferenceSlave.ActionEum == ActionEnum.Add)
            {
                if (vmProdReferenceSlave.ProdReferenceId == 0)
                {
                    vmProdReferenceSlave.ProdReferenceId = await _service.Prod_ReferenceAdd(vmProdReferenceSlave);
                }
                await _service.ProdReferenceSlaveByChallanAdd(vmProdReferenceSlave);
            }
            else if (vmProdReferenceSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _service.ProdReferenceSlaveEdit(vmProdReferenceSlave);
            }
            return RedirectToAction(nameof(ProdReferenceSlaveByChallan), new { companyId = vmProdReferenceSlave.CompanyFK, prodReferenceId = vmProdReferenceSlave.ProdReferenceId });
        }

        [HttpPost]
        public async Task<ActionResult> ReferenceSlaveDelete(VMProdReferenceSlave vmProdReferenceSlave)
        {
            if (vmProdReferenceSlave.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmProdReferenceSlave.ProdReferenceId = await _service.Prod_ReferenceSlaveDelete(vmProdReferenceSlave.ProdReferenceSlaveID);
            }
            return RedirectToAction(nameof(ProdReferenceSlave), new { companyId = vmProdReferenceSlave.CompanyFK, prodReferenceId = vmProdReferenceSlave.ProdReferenceId });
        }

        
        [HttpGet]
        public async Task<ActionResult> ProdReferenceList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (companyId > 0)
            {  Session["CompanyId"] = companyId; }

            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);


            VMProdReference vmPaymentMaster = new VMProdReference();
            vmPaymentMaster = await Task.Run(() => _service.ProdReferenceListGet(companyId, fromDate, toDate));
            vmPaymentMaster.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmPaymentMaster.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(vmPaymentMaster);
        }

        [HttpPost]
        
        public async Task<ActionResult> ProdReferenceList(VMProdReference model)
        {
            if (model.CompanyFK > 0)
            {
                Session["CompanyId"] = model.CompanyFK;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(ProdReferenceList), new { companyId = model.CompanyFK, fromDate = model.FromDate, toDate = model.ToDate });

        }


        
        //[HttpGet]
        //public async Task<ActionResult> ProdReferenceList(int companyId)
        //{
        //    VMProdReference vmProdReference = new VMProdReference();
        //    vmProdReference = await _service.ProdReferenceListGet(companyId);

        //    return View(vmProdReference);
        //}
        [HttpPost]
        public async Task<ActionResult> ProdReferenceListList(VMProdReference vmProdReference)
        {
            if (vmProdReference.ActionEum == ActionEnum.Edit)
            {
                await _service.Prod_ReferenceEdit(vmProdReference);
            }
            return RedirectToAction(nameof(ProdReferenceListList), new { companyId = vmProdReference.CompanyFK });
        }

        public async Task<JsonResult> SingleProdReferenceSlave(int id)
        {
            var model = await _service.GetSingleProdReferenceSlave(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SingleProdReferenceSlaveConsumption(int id)
        {
            var model = await _service.GetSingleProd_ReferenceSlaveConsumption(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SingleProdReferenceSlaveExpensessConsumption(int id)
        {
            var model = await _service.GetSingleProdReferenceSlaveExpansessConsumption(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SingleProdReference(int id)
        {
            VMProdReference model = new VMProdReference();
            model = await _service.GetSingleProdReference(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SubmitProdReference(VMProdReference vmProdReference)
        {
            vmProdReference.ProdReferenceId = await _service.ProdReferenceSubmit(vmProdReference.ProdReferenceId);
            return RedirectToAction(nameof(ProdReferenceListList), new { companyId = vmProdReference.CompanyFK });
        }
        [HttpPost]
        public async Task<ActionResult> SubmitProdReferenceFromSlave(VMProdReferenceSlave vmProdReferenceSlave)
        {
            vmProdReferenceSlave.ProdReferenceId = await _service.ProdReferenceSubmit(vmProdReferenceSlave.ProdReferenceId);
            return RedirectToAction(nameof(ProdReferenceSlave), "Procurement", new { companyId = vmProdReferenceSlave.CompanyFK, prodReferenceId = vmProdReferenceSlave.ProdReferenceId });
        }


        [HttpPost]
        public async Task<ActionResult> DeleteProdReference(VMProdReference vmProdReference)
        {
            if (vmProdReference.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmProdReference.ProdReferenceId = await _service.ProdReferenceDelete(vmProdReference.ProdReferenceId);
            }
            return RedirectToAction(nameof(ProdReferenceListList), new { companyId = vmProdReference.CompanyFK });
        }

        #endregion

        public ActionResult GetMaterialReceiveDetails(int materialReceiveId)
        {
            var model = new VMProdReferenceSlave();
            if (materialReceiveId > 0)
            {
                model.DataToList = _service.GetMaterialReceiveDetailsData(materialReceiveId);
            }
            return PartialView("_MaterialReceiveDetails", model);
        }



        public async Task<ActionResult> GCCLProdReferenceSlave(int companyId = 0, int prodReferenceId = 0)
        {
            VMProdReferenceSlave vmProdReferenceSlave = new VMProdReferenceSlave();

            if (prodReferenceId == 0)
            {
                vmProdReferenceSlave.CompanyFK = companyId;

            }
            else if (prodReferenceId > 0)
            {
                vmProdReferenceSlave = await Task.Run(() => _service.GCCLProdReferenceSlaveGet(companyId, prodReferenceId));

            }
            if (companyId == (int)CompanyName.GloriousCropCareLimited)
            {

                vmProdReferenceSlave.FactoryExpensesList = new SelectList(_service.GCCLLCFactoryExpanceHeadGLList(companyId), "Value", "Text");
                vmProdReferenceSlave.AdvanceHeadList = new SelectList(_service.GCCLAdvanceHeadGLList(companyId), "Value", "Text");
                //vmProdReferenceSlave.sto = new SelectList(_service.GCCLAdvanceHeadGLList(companyId), "Value", "Text");

            }
            return View(vmProdReferenceSlave);
        }
        [HttpPost]
        public async Task<ActionResult> GCCLProdReferenceSlave(VMProdReferenceSlave vmProdReferenceSlave)
        {

            if (vmProdReferenceSlave.ActionEum == ActionEnum.Add)
            {
                if (vmProdReferenceSlave.ProdReferenceId == 0)
                {
                    vmProdReferenceSlave.ProdReferenceId = await _service.Prod_ReferenceAdd(vmProdReferenceSlave);
                }
                if (vmProdReferenceSlave.RProductId > 0)
                {
                    await _service.GCCLProd_ReferenceSlaveConsumptionAdd(vmProdReferenceSlave);
                }
                if (vmProdReferenceSlave.FactoryExpensesHeadGLId > 0)
                {
                    await _service.GCCLProdReferenceFactoryExpensesAdd(vmProdReferenceSlave);
                }
                if (vmProdReferenceSlave.FProductId > 0)
                {
                    await _service.GCCLProdReferenceSlaveAdd(vmProdReferenceSlave);
                }
            }
            else if (vmProdReferenceSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                if (vmProdReferenceSlave.RProductId > 0 && vmProdReferenceSlave.ID > 0)
                {
                    await _service.ProdReferenceSlaveRawConsumptionEdit(vmProdReferenceSlave);
                }
                if (vmProdReferenceSlave.FactoryExpensesHeadGLId > 0 && vmProdReferenceSlave.ID > 0)
                {
                    await _service.ProdReferenceSlaveFactoryConsumptionEdit(vmProdReferenceSlave);
                }
                if (vmProdReferenceSlave.FProductId > 0 && vmProdReferenceSlave.ProdReferenceSlaveID > 0)
                {
                    await _service.ProdReferenceSlaveEdit(vmProdReferenceSlave);
                }                
            }
            else if (vmProdReferenceSlave.ActionEum == ActionEnum.Delete)
            {
                //Delete
                if (vmProdReferenceSlave.ID > 0)
                {
                    await _service.DeleteProdReferenceSlaveConsumption(vmProdReferenceSlave);
                }
                if (vmProdReferenceSlave.ProdReferenceSlaveID > 0)
                {
                    await _service.DeleteProdReferenceSlave(vmProdReferenceSlave);
                }

            }
            else if (vmProdReferenceSlave.ActionEum == ActionEnum.Finalize)
            {
                await _service.SubmitProdReference(vmProdReferenceSlave);


            }
            return RedirectToAction(nameof(GCCLProdReferenceSlave), new { companyId = vmProdReferenceSlave.CompanyFK, prodReferenceId = vmProdReferenceSlave.ProdReferenceId });
        }

        public async Task<ActionResult> KPLProdReference(int companyId = 0, int prodReferenceId = 0,int resultFlg=0)
        {
            VMProdReferenceSlave vmPurchaseOrderSlave = new VMProdReferenceSlave();
            if (prodReferenceId == 0)
            {
                vmPurchaseOrderSlave.CompanyFK = companyId;
                vmPurchaseOrderSlave.ReferenceDate = DateTime.Now.Date;

            }
            else if (prodReferenceId > 0)
            {
                vmPurchaseOrderSlave = await Task.Run(() => _service.KPLProdReferenceSlaveGet(companyId, prodReferenceId));
                ViewBag.resultFlg = resultFlg;
            }

            return View(vmPurchaseOrderSlave);
        }
        [HttpPost]
        public async Task<ActionResult> KPLProdReference(VMProdReferenceSlave vmProdReferenceSlave)
        {

            if (vmProdReferenceSlave.ActionEum == ActionEnum.Add)
            {
                if (vmProdReferenceSlave.ProdReferenceId == 0)
                {
                    vmProdReferenceSlave.ProdReferenceId = await _service.KplProd_ReferenceAdd(vmProdReferenceSlave);
                }
            }
            return RedirectToAction(nameof(KPLProdReference), new { companyId = vmProdReferenceSlave.CompanyFK, prodReferenceId = vmProdReferenceSlave.ProdReferenceId });
        }


        public async Task<ActionResult> KPLProdReferenceAuthDetails(int companyId = 0, int prodReferenceId = 0, int resultFlg = 0)
        {
            VMProdReferenceSlave vmPurchaseOrderSlave = new VMProdReferenceSlave();
            if (prodReferenceId == 0)
            {
                vmPurchaseOrderSlave.CompanyFK = companyId;
                vmPurchaseOrderSlave.ReferenceDate = DateTime.Now.Date;

            }
            else if (prodReferenceId > 0)
            {
                vmPurchaseOrderSlave = await Task.Run(() => _service.KPLProdReferenceSlaveGet(companyId, prodReferenceId));
                ViewBag.resultFlg = resultFlg;
            }

            return View(vmPurchaseOrderSlave);
        }



        [HttpGet]
        public ActionResult ProductOrderQuantityByOrderList(long OrderMasterId,int prodReferenceSlaveID=0)
        {
            var model = new VMProdReferenceSlave();

            model.DataToList = _service.ProductOrderQuantityByOrderList(OrderMasterId, prodReferenceSlaveID);
            return PartialView("_ProductOrderByOrderListPartial", model);

        }

        [HttpGet]
        public ActionResult ProductOrderQuantityByOrderDetailsIdList(long OrderDetailsId, int prodReferenceSlaveID = 0)
        {
            var model = new VMProdReferenceSlave();

            model.DataToList = _service.ProductOrderQuantityByOrderDetailsIdList(OrderDetailsId, prodReferenceSlaveID);
            return PartialView("_ProductOrderByOrderListPartial", model);

        }

        [HttpGet]
        public async Task<ActionResult> KpLProdReferenceSlaveReport(int companyId = 0, int prodReferenceId = 0)
        {
            VMProdReferenceSlave vmProdReferenceSlave = new VMProdReferenceSlave();
            if (prodReferenceId > 0)
            {
                vmProdReferenceSlave = await Task.Run(() => _service.ProdReferenceSlaveGet(companyId, prodReferenceId));

            }
            return View(vmProdReferenceSlave);
        }


        [HttpPost]
        public async Task<ActionResult> KpLProdReferenceSlaveDelete(int id,int companyid)
        {
            var ProdReferenceId = 0;
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                   ProdReferenceId=  await _service.KpLProdReferenceSlaveDelete(id);
                }
            }

            return RedirectToAction(nameof(KPLProdReference), new { companyId = companyid, prodReferenceId= ProdReferenceId });
        }

        [HttpGet]
        public async Task<ActionResult> KPLProdReferenceList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (companyId > 0)
            { Session["CompanyId"] = companyId; }


            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);
             

            VMProdReference vmPaymentMaster = new VMProdReference();
            vmPaymentMaster = await Task.Run(() => _service.KPLProdReferenceListGet(companyId, fromDate, toDate));
            vmPaymentMaster.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmPaymentMaster.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(vmPaymentMaster);
        }

        [HttpGet]
        public async Task<ActionResult> KPLProdAuthPendingReferenceList(int companyId, DateTime? fromDate, DateTime? toDate)
        {
            if (companyId > 0)
            { Session["CompanyId"] = companyId; }

            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (!fromDate.HasValue) fromDate = firstDayOfMonth;
            if (!toDate.HasValue) toDate = firstDayOfMonth.AddMonths(1).AddDays(-1);

            

            VMProdReference vmPaymentMaster = new VMProdReference();
            vmPaymentMaster = await Task.Run(() => _service.KPLProdAuthPendingReferenceListGet(companyId, fromDate, toDate));
            vmPaymentMaster.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            vmPaymentMaster.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(vmPaymentMaster);
        }

        [HttpPost]

        public ActionResult KPLProdAuthPendingReferenceList(VMProdReference model)
        {
            if (model.CompanyFK > 0)
            {
                Session["CompanyId"] = model.CompanyFK;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(KPLProdAuthPendingReferenceList), new { companyId = model.CompanyFK, fromDate = model.FromDate, toDate = model.ToDate });

        }



        [HttpPost]

        public ActionResult KPLProdReferenceList(VMProdReference model)
        {
            if (model.CompanyFK > 0)
            {
                Session["CompanyId"] = model.CompanyFK;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            return RedirectToAction(nameof(KPLProdReferenceList), new { companyId = model.CompanyFK, fromDate = model.FromDate, toDate = model.ToDate });

        }

        [HttpPost]
        public async Task<ActionResult> DeleteProdReferenceForKpl(VMProdReferenceSlave vmProdReferenceSlave)
        {
            if (vmProdReferenceSlave.ActionEum == ActionEnum.Delete)
            {
                vmProdReferenceSlave.ModifiedBy = Common.GetUserId();
                vmProdReferenceSlave.ProdReferenceId = await Task.Run(()=> _service.DeleteProdReferenceforKpl(vmProdReferenceSlave.ProdReferenceId, vmProdReferenceSlave.ModifiedBy));
            }
            return RedirectToAction(nameof(KPLProdReferenceList), new { companyId = vmProdReferenceSlave.CompanyFK});
        }

        [HttpPost]
        public async Task<ActionResult> AuthProdReference(VMProdReferenceSlave vmProdReferenceSlave)
        {
            if (vmProdReferenceSlave.ActionEum == ActionEnum.Edit)
            {
                vmProdReferenceSlave.ModifiedBy = Common.GetUserId();
                vmProdReferenceSlave.ProdReferenceId = await Task.Run(() => _service.AuthSubmitProdReferenceForKpl(vmProdReferenceSlave.ProdReferenceId, vmProdReferenceSlave.ModifiedBy));
            }
            return RedirectToAction(nameof(KPLProdReferenceAuthDetails), new { companyId = vmProdReferenceSlave.CompanyFK });
        }

        [HttpPost]
        public async Task<ActionResult> AuthSubmitProdReferenceForKpl(VMProdReferenceSlave vmProdReferenceSlave)
        {
            if (vmProdReferenceSlave.ActionEum == ActionEnum.Edit)
            {
                vmProdReferenceSlave.ModifiedBy = Common.GetUserId();
                vmProdReferenceSlave.ProdReferenceId = await Task.Run(() => _service.AuthSubmitProdReferenceForKpl(vmProdReferenceSlave.ProdReferenceId, vmProdReferenceSlave.ModifiedBy));
            }
            return RedirectToAction(nameof(KPLProdAuthPendingReferenceList), new { companyId = vmProdReferenceSlave.CompanyFK });
        }

        [HttpPost]
        public async Task<ActionResult> KpLProdReferenceSubmit(int hdnprodReferenceId,int companyid)
        {
            var prodReferenceId = 0; 
            if (hdnprodReferenceId > 0)
            {
                 prodReferenceId = await _service.SubmitProdReferenceforKpl(hdnprodReferenceId);

                // await _service.SubmitMultiReferenceforKpl();


            }            
            return RedirectToAction(nameof(KPLProdReference), new { companyId = companyid, prodReferenceId = prodReferenceId });
        }

        [HttpPost]
        public async Task<ActionResult> KpLProdReferenceUpdate(VMProdReference vMProdReference)
        {
            int proReferenceId = 0;
            if (vMProdReference.ProdReferenceId > 0)
            {
               proReferenceId=  await _service.KpLProdReferenceUpdate(vMProdReference.ProdReferenceId, vMProdReference.ReferenceDate);
            }

            return RedirectToAction(nameof(KPLProdReferenceList), new { companyId = vMProdReference.CompanyFK, prodReferenceId= proReferenceId });
        }

        [HttpPost]
        public async Task<ActionResult> GetCustomerForOrderData(int prodReferenceId)
        {
           var resultData = await _service.GetCustomerForOrderData(prodReferenceId);
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> KpLProdReferenceSlaveUpdate(VMProdReferenceSlave vMProdReferenceSlave)
        {
            int proReferenceId = 0;
            if (vMProdReferenceSlave.ProdReferenceSlaveID > 0)
            {
                proReferenceId = await _service.KpLProdReferenceSlaveUpdate(vMProdReferenceSlave);
            }

            return RedirectToAction(nameof(KPLProdReference), new { companyId = vMProdReferenceSlave.CompanyFK, prodReferenceId = vMProdReferenceSlave.ProdReferenceId });
        }

    }
}