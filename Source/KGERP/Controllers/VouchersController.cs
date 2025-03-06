using KGERP.Controllers.Custom_Authorization;
using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.Accounting;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Services.WareHouse;
using KGERP.Utility;
using KGERP.ViewModel;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace KGERP.Controllers
{
    [CheckSession]
    public class VouchersController : BaseController
    {
        private readonly IVoucherTypeService voucherTypeService;
        private readonly IVoucherService voucherService;
        private readonly IAccountHeadService accountHeadService;
        private readonly AccountingService _accountingService;


        public VouchersController(ERPEntities db, IAccountHeadService accountHeadService, IVoucherTypeService voucherTypeService, IVoucherService voucherService)
        {
            this.voucherTypeService = voucherTypeService;
            this.voucherService = voucherService;
            this.accountHeadService = accountHeadService;
            _accountingService = new AccountingService(db);
        }


        [HttpGet]
        
        [ParentAuthorizedAttribute]
        public async Task<ActionResult> Index(int companyId, DateTime? fromDate, DateTime? toDate, bool? vStatus, int? voucherTypeId )
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
            if (vStatus==null)
            {
                vStatus = true;
            }
                      
            VoucherModel voucherModel = new VoucherModel();
            voucherModel = await voucherService.GetVouchersList(companyId, fromDate, toDate,vStatus, voucherTypeId);
            voucherModel.VoucherTypesList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text");
            voucherModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            voucherModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            voucherModel.IsSubmit = vStatus;
            return View(voucherModel);
        }

        [HttpGet]
        public async Task<ActionResult> DraftIndex(int companyId, DateTime? fromDate, DateTime? toDate, int? voucherTypeId, bool? vStatus = false)
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
            if (vStatus == null)
            {
                vStatus = true;
            }

            VoucherModel voucherModel = new VoucherModel();
            voucherModel = await voucherService.GetDraftVouchersList(companyId, fromDate, toDate, voucherTypeId, vStatus);
            voucherModel.VoucherTypesList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text");
            voucherModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            voucherModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(voucherModel);
        }

        [HttpGet]
        
     
        public async Task<ActionResult> IndexWithDelete(int companyId, DateTime? fromDate, DateTime? toDate, bool? vStatus, int? voucherTypeId)
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
            if (vStatus == null)
            {
                vStatus = true;
            }

            VoucherModel voucherModel = new VoucherModel();
            voucherModel = await voucherService.GetVouchersListWithDelete(companyId, fromDate, toDate, vStatus, voucherTypeId);
            voucherModel.VoucherTypesList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text");
            voucherModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            voucherModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            voucherModel.IsSubmit = vStatus;
            return View(voucherModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> Index(VoucherModel voucherModel)
        {
            if (voucherModel.CompanyId > 0)
            {
                Session["CompanyId"] = voucherModel.CompanyId;
            }
                      
            voucherModel.FromDate = Convert.ToDateTime(voucherModel.StrFromDate);
            voucherModel.ToDate = Convert.ToDateTime(voucherModel.StrToDate);


            return RedirectToAction(nameof(Index), new { companyId = voucherModel.CompanyId, fromDate = voucherModel.FromDate, toDate= voucherModel.ToDate, vStatus=voucherModel.IsSubmit, voucherTypeId = voucherModel.VmVoucherTypeId??0 });
        }
        [HttpPost]

        public  ActionResult DraftIndex(VoucherModel voucherModel)
        {
            if (voucherModel.CompanyId > 0)
            {
                Session["CompanyId"] = voucherModel.CompanyId;
            }

            voucherModel.FromDate = Convert.ToDateTime(voucherModel.StrFromDate);
            voucherModel.ToDate = Convert.ToDateTime(voucherModel.StrToDate);


            return RedirectToAction(nameof(DraftIndex), new { companyId = voucherModel.CompanyId, fromDate = voucherModel.FromDate, toDate = voucherModel.ToDate, vStatus = false, voucherTypeId = voucherModel.VmVoucherTypeId ?? 0 });
        }

        [HttpPost]
        
        public async Task<ActionResult> IndexWithDelete(VoucherModel voucherModel)
        {
            if (voucherModel.CompanyId > 0)
            {
                Session["CompanyId"] = voucherModel.CompanyId;
            }

            voucherModel.FromDate = Convert.ToDateTime(voucherModel.StrFromDate);
            voucherModel.ToDate = Convert.ToDateTime(voucherModel.StrToDate);


            return RedirectToAction(nameof(IndexWithDelete), new { companyId = voucherModel.CompanyId, fromDate = voucherModel.FromDate, toDate = voucherModel.ToDate, vStatus = voucherModel.IsSubmit, voucherTypeId = voucherModel.VmVoucherTypeId ?? 0 });
        }



        [HttpGet]
        
        public async Task<ActionResult> VoucherIndex(int companyId, DateTime? fromDate, DateTime? toDate, bool? vStatus, int voucherTypeId)
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
            if (vStatus == null)
            {
                vStatus = true;
            }

            VoucherModel voucherModel = new VoucherModel();
            voucherModel = await voucherService.GetVouchersListByVoucherType(companyId, fromDate, toDate, vStatus, voucherTypeId);
           // voucherModel.VoucherTypesList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text");
            voucherModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            voucherModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            voucherModel.IsSubmit = vStatus;
            return View(voucherModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> VoucherIndex(VoucherModel voucherModel)
        {
            if (voucherModel.CompanyId > 0)
            {
                Session["CompanyId"] = voucherModel.CompanyId;
            }

            voucherModel.FromDate = Convert.ToDateTime(voucherModel.StrFromDate);
            voucherModel.ToDate = Convert.ToDateTime(voucherModel.StrToDate);


            return RedirectToAction(nameof(VoucherIndex), new { companyId = voucherModel.CompanyId, fromDate = voucherModel.FromDate, toDate = voucherModel.ToDate, vStatus = voucherModel.IsSubmit, voucherTypeId = voucherModel.VmVoucherTypeId ?? 0 });
        }

        [HttpGet]
        
        public async Task<ActionResult> StockVoucherIndex(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }          

            VoucherModel voucherModel = new VoucherModel();
            voucherModel = await voucherService.GetStockVouchersList(companyId);
            voucherModel.VoucherTypesList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text");
          
            return View(voucherModel);
        }

        [HttpGet]
        
        public ActionResult Create(int id, int companyId)
        {
            VoucherViewModel vm = new VoucherViewModel();
            Session["CompanyId"] = companyId;
            vm.Voucher = voucherService.GetVoucher(companyId, id);
            vm.Voucher.CompanyId = companyId;
            vm.VoucherTypes = voucherTypeService.GetVoucherTypeSelectModels(companyId);
            vm.CostCenters = voucherTypeService.GetAccountingCostCenter(companyId);
            return View(vm);
        }

        [HttpGet]
        
        public async Task<ActionResult> GetAllVoucher(int companyId, int? voucherTypeId, DateTime? fromDate, DateTime? toDate)
        {

            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            if (fromDate == null)
            {
                fromDate = DateTime.Now.AddDays(-2);
            }
            if (toDate == null)
            {
                toDate = DateTime.Now;
            }

            VoucherViewModel voucherModel = new VoucherViewModel();
        //    voucherModel.Voucher = await voucherService.GetAllVouchersList(companyId,voucherTypeId, fromDate, toDate);
            voucherModel.Voucher.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            voucherModel.Voucher.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            voucherModel.VoucherTypes = voucherTypeService.GetVoucherTypeSelectModels(companyId);
            voucherModel.Voucher.VoucherTypeId = voucherTypeId;
            return View(voucherModel);
        }
        [HttpPost]
        
        public async Task<ActionResult> GetAllVoucher(VoucherViewModel voucherModel)
        {
            if (voucherModel.Voucher.CompanyId > 0)
            {
                Session["CompanyId"] = voucherModel.Voucher.CompanyId;
            }
            voucherModel.Voucher.FromDate = Convert.ToDateTime(voucherModel.Voucher.StrFromDate);
            voucherModel.Voucher.ToDate = Convert.ToDateTime(voucherModel.Voucher.StrToDate);
            return RedirectToAction(nameof(Index), new { companyId = voucherModel.Voucher.CompanyId,
                voucherTypeId = voucherModel.Voucher.VoucherTypeId,
                fromDate = voucherModel.Voucher.FromDate,
                toDate = voucherModel.Voucher.ToDate});
        }
        //[HttpPost]
        //
        //[ValidateAntiForgeryToken]

        //public ActionResult CreateTempVoucher(VoucherViewModel vm)
        //{
        //    vm.Voucher = voucherService.CreateTempVoucher(vm.Voucher);

        //    return PartialView("~/Views/Vouchers/_voucherDetailGrid.cshtml", vm.Voucher);
        //}


        // POST: GLTables/Create  
        [HttpPost]
        
        public ActionResult CreateVoucher(VoucherViewModel vm)
        {
            string message = string.Empty;
            vm.Voucher.VoucherDetails = vm.VoucherDetails;
            bool result = voucherService.SaveVoucher(vm.Voucher, out message);
            if (result)
            {
                TempData["message"] = message;
                return RedirectToAction("Index", new { companyId = vm.Voucher.CompanyId });
            }
            TempData["message"] = message;
            return RedirectToAction("Create", new { id = 0, companyId = vm.Voucher.CompanyId });
        }
        //[HttpPost]
        //
        //public ActionResult RemoveVoucherItem(long id)
        //{

        //    VoucherModel Voucher = voucherService.RemoveVoucherItem(id);

        //    return PartialView("~/Views/Vouchers/_voucherDetailGrid.cshtml", Voucher);
        //}

        //
        //[HttpGet]
        public JsonResult GetVoucherNo(int voucherTypeId, int companyId,DateTime voucherDate)
        {
            ModelState.Clear();
            var voucherNo = voucherService.GetVoucherNo(voucherTypeId, companyId, voucherDate);
            return Json(voucherNo, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        
        public JsonResult VoucherNoAutoComplete(string prefix)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var vouchers = voucherService.GetVoucherNoAutoComplete(prefix, companyId);
            return Json(vouchers);
        }

     
        [HttpGet]
        public async Task<ActionResult> DraftVoucherDetailsInfo(int companyId = 0, int voucherId = 0)
        {
            VMJournalSlave vmJournalSlave = new VMJournalSlave();

            if (voucherId == 0)
            {
                vmJournalSlave = await Task.Run(() => _accountingService.GetCompaniesDetails(companyId));

            }
            else if (voucherId > 0)
            {
                vmJournalSlave = await Task.Run(() => _accountingService.GetVoucherDetails(companyId, voucherId));
            }
            vmJournalSlave.CostCenterList = new SelectList(_accountingService.CostCenterDropDownList(companyId), "Value", "Text");
            vmJournalSlave.VoucherTypesList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text");
            if (companyId == (int)CompanyName.GloriousCropCareLimited ||
                companyId == (int)CompanyName.KrishibidPrintingAndPublicationLimited ||
                companyId == (int)CompanyName.KrishibidPackagingLimited)
            {
                vmJournalSlave.BankOrCashParantList = new SelectList(_accountingService.GCCLCashAndBankDropDownList(companyId), "Value", "Text");

            }
            if (companyId == (int)CompanyName.KrishibidSeedLimited)
            {
                vmJournalSlave.BankOrCashParantList = new SelectList(_accountingService.SeedCashAndBankDropDownList(companyId), "Value", "Text");

            }

            return View(vmJournalSlave);
        }

        [HttpGet]
        public async Task<ActionResult> ManageBankOrCash(int companyId = 0, int voucherId = 0)
        {
            VMJournalSlave vmJournalSlave = new VMJournalSlave();

            if (voucherId == 0)
            {
                vmJournalSlave = await Task.Run(() => _accountingService.GetCompaniesDetails(companyId));

            }
            else if (voucherId > 0)
            {
                vmJournalSlave = await Task.Run(() => _accountingService.GetVoucherDetails(companyId, voucherId));
            }
            vmJournalSlave.CostCenterList = new SelectList(_accountingService.CostCenterDropDownList(companyId), "Value", "Text");
            vmJournalSlave.VoucherTypesList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text");

            

            return View(vmJournalSlave);
        }


        [HttpPost]
        
        public async Task<ActionResult> ManageBankOrCash(VMJournalSlave vmJournalSlave)
        {

            if (vmJournalSlave.ActionEum == ActionEnum.Add)
            {
                if (vmJournalSlave.VoucherId == 0)
                {
                    vmJournalSlave.IsStock = false;
                    // it is important / dont delete
                    var voucherNo = voucherService.GetVoucherNo(vmJournalSlave.VoucherTypeId, vmJournalSlave.CompanyFK.Value, vmJournalSlave.Date.Value);
                    vmJournalSlave.VoucherNo = voucherNo;
                    vmJournalSlave.VoucherId = await _accountingService.VoucherAdd(vmJournalSlave);

                }
                await _accountingService.VoucherDetailAdd(vmJournalSlave);
            }
            else if (vmJournalSlave.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _accountingService.VoucherDetailsEdit(vmJournalSlave);
            }
            else if (vmJournalSlave.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _accountingService.VoucherDetailsDelete(vmJournalSlave.VoucherDetailId.Value);
            }
            
            return RedirectToAction(nameof(ManageBankOrCash), new { companyId = vmJournalSlave.CompanyFK, voucherId = vmJournalSlave.VoucherId });
        }
        [HttpPost]
        
        public async Task<ActionResult> DeleteVoucher(VoucherModel voucherModel)
        {
            if (voucherModel.VoucherId > 0)
            {
                await _accountingService.VoucherDelete(voucherModel);

            }

            return RedirectToAction(nameof(Index), new { companyId = voucherModel.CompanyId});
        }

        [HttpPost]
        
        public async Task<ActionResult> DeleteVoucher2(VoucherModel voucherModel)
        {
            if (voucherModel.VoucherId > 0)
            {
                await _accountingService.VoucherDelete(voucherModel);

            }

            return RedirectToAction(nameof(IndexWithDelete), new { companyId = voucherModel.CompanyId });
        }
        [HttpPost]
        
        public async Task<ActionResult> UndoSubmitVoucher(VoucherModel voucherModel)
        {
            if (voucherModel.VoucherId > 0)
            {
                await _accountingService.VoucherUndoSubmit(voucherModel);
            }
            return RedirectToAction(nameof(Index), new { companyId = voucherModel.CompanyId });
        }

        [HttpPost]
        
        public async Task<ActionResult> ApprovalSubmittedVoucher(VoucherModel voucherModel)
        {
            if (voucherModel.VoucherId > 0)
            {
                await _accountingService.VoucherApproved(voucherModel);
            }
            return RedirectToAction(nameof(Index), new { companyId = voucherModel.CompanyId });
        }

        [HttpGet]
        public async Task<ActionResult> ManageStock(int companyId = 0, int voucherId = 0)
        {
            VMJournalSlave vmJournalSlave = new VMJournalSlave();

            if (voucherId == 0)
            {
                vmJournalSlave.CompanyFK = companyId;
            }
            else if (voucherId > 0)
            {
                vmJournalSlave = await Task.Run(() => _accountingService.GetStockVoucherDetails(companyId, voucherId));
            }
            vmJournalSlave.CostCenterList = new SelectList(_accountingService.CostCenterDropDownList(companyId), "Value", "Text");
            vmJournalSlave.VoucherTypesList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text");
            
            vmJournalSlave.BankOrCashParantList = new SelectList(_accountingService.StockDropDownList(companyId), "Value", "Text");


            return View(vmJournalSlave);
        }


        [HttpPost]
        
        public async Task<ActionResult> ManageStock(VMJournalSlave vmJournalSlave)
        {

            if (vmJournalSlave.ActionEum == ActionEnum.Add)
            {
                if (vmJournalSlave.VoucherId == 0)
                {
                    vmJournalSlave.IsStock = true;
                    vmJournalSlave.VoucherId = await _accountingService.VoucherAdd(vmJournalSlave);

                }
                await _accountingService.VoucherDetailAdd(vmJournalSlave);
            }
            else if (vmJournalSlave.ActionEum == ActionEnum.Edit)
            {
                //Delete
                await _accountingService.VoucherDetailsEdit(vmJournalSlave);
            }
            return RedirectToAction(nameof(ManageStock), new { companyId = vmJournalSlave.CompanyFK, voucherId = vmJournalSlave.VoucherId });
        }



        public async Task<ActionResult> Head5Get(int companyId, int parentId)
        {

            var headGLModel = await Task.Run(() => _accountingService.Head5Get(companyId, parentId));
            var list = headGLModel.Select(x => new { Value = x.Id, Text = x.AccName }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> HeadGLGet(int companyId, int parentId)
        {

            var headGLModel = await Task.Run(() => _accountingService.HeadGLGet(companyId, parentId));
            var list = headGLModel.Select(x => new { Value = x.Id, Text = x.AccName }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> HeadGLByHead5ParentIdGet(int companyId, int parentId)
        {
            var headGLModel = await Task.Run(() => _accountingService.HeadGLByHeadGLParentIdGet(companyId, parentId));
            var list = headGLModel.Select(x => new { Value = x.Id, Text = x.AccName }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
            //if (companyId == (int)CompanyName.KrishibidSeedLimited)
            //{
            //    var headGLModel = await Task.Run(() => _accountingService.HeadGLByHead5ParentIdGet(companyId, parentId));
            //    var list = headGLModel.Select(x => new { Value = x.Id, Text = x.AccName }).ToList();
            //    return Json(list, JsonRequestBehavior.AllowGet);

                //}
            //else if (companyId == (int)CompanyName.GloriousCropCareLimited 
            //    || companyId == (int)CompanyName.KrishibidPrintingAndPublicationLimited
            //    || companyId == (int)CompanyName.KrishibidPackagingLimited
            //    || companyId == (int)CompanyName.KrishibidFirmLimited
            //    || companyId == (int)CompanyName.KrishibidPoultryLimited
            //    || companyId == (int)CompanyName.KrishibidSeedLimited)
            //{
               
            //}

           
           
        }

        public async Task<ActionResult> StockByHead5ParentIdGet(int companyId, int parentId)
        {
            if (companyId == (int)CompanyName.NaturalFishFarmingLimited 
                || companyId == (int)CompanyName.OrganicPoultryLimited 
                || companyId == (int)CompanyName.SonaliOrganicDairyLimited
                || companyId == (int)CompanyName.KrishibidPrintingAndPublicationLimited
                || companyId == (int)CompanyName.KrishibidFoodAndBeverageLimited
                || companyId == (int)CompanyName.KrishibidPackagingLimited
                || companyId == (int)CompanyName.KrishibidBazaarLimited
                || companyId == (int)CompanyName.KrishibidPoultryLimited
                || companyId == (int)CompanyName.KrishibidFisheriesLimited
                || companyId == (int)CompanyName.KrishibidTradingLimited
                || companyId == (int)CompanyName.KrishibidSafeFood



                )
            {
                var headGLModel = await Task.Run(() => _accountingService.HeadGLGet(companyId, parentId));
                var list = headGLModel.Select(x => new { Value = x.Id, Text = x.AccName }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);

            }


            return null;
        }

        public async Task<ActionResult> AutoInsertVoucherDetails(int voucherId,int virtualHeadId, string virtualHeadParticular,int? productCategory)
        {
            long voucherDetailsId = await Task.Run(() => _accountingService.AutoInsertVoucherDetails(voucherId, virtualHeadId, virtualHeadParticular, productCategory));
            return Json(voucherDetailsId, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> AutoInsertStockVoucherDetails(int companyId,int voucherId)
        {
            long voucherDetailsId = 0;
            if (companyId == (int)CompanyName.NaturalFishFarmingLimited)
            {
                 voucherDetailsId = await Task.Run(() => _accountingService.NFFLAutoInsertStockVoucherDetails(companyId, voucherId));
            }
            if (companyId == (int)CompanyName.OrganicPoultryLimited)
            {
                voucherDetailsId = await Task.Run(() => _accountingService.OPLAutoInsertStockVoucherDetails(companyId, voucherId));
            }
            if (companyId == (int)CompanyName.SonaliOrganicDairyLimited)
            {
                voucherDetailsId = await Task.Run(() => _accountingService.SODLAutoInsertStockVoucherDetails(companyId, voucherId));
            }
            if (companyId == (int)CompanyName.KrishibidPrintingAndPublicationLimited)
            {
                voucherDetailsId = await Task.Run(() => _accountingService.PrintingAutoInsertStockVoucherDetails(companyId, voucherId));
            }
            if (companyId == (int)CompanyName.KrishibidFoodAndBeverageLimited)
            {
                voucherDetailsId = await Task.Run(() => _accountingService.FBLAutoInsertStockVoucherDetails(companyId, voucherId));
            }
            if (companyId == (int)CompanyName.KrishibidPackagingLimited)
            {
                voucherDetailsId = await Task.Run(() => _accountingService.PackagingAutoInsertStockVoucherDetails(companyId, voucherId));
            }
            if (companyId == (int)CompanyName.KrishibidBazaarLimited)
            {
                voucherDetailsId = await Task.Run(() => _accountingService.KBLAutoInsertStockVoucherDetails(companyId, voucherId));
            }
            if (companyId == (int)CompanyName.KrishibidFisheriesLimited)
            {
                voucherDetailsId = await Task.Run(() => _accountingService.FishariseAutoInsertStockVoucherDetails(companyId, voucherId));
            }
            if (companyId == (int)CompanyName.KrishibidPoultryLimited)
            {
                voucherDetailsId = await Task.Run(() => _accountingService.PoultryAutoInsertStockVoucherDetails(companyId, voucherId));
            }
            if (companyId == (int)CompanyName.KrishibidTradingLimited)
            {
                voucherDetailsId = await Task.Run(() => _accountingService.TradingAutoInsertStockVoucherDetails(companyId, voucherId));
            }  
            if (companyId == (int)CompanyName.KrishibidSafeFood)
            {
                voucherDetailsId = await Task.Run(() => _accountingService.SafeFoodAutoInsertStockVoucherDetails(companyId, voucherId));
            }
            return Json(voucherDetailsId, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> UpdateVoucherStatus(int voucherId)
        {
            long voucherDetailsId = await Task.Run(() => _accountingService.UpdateVoucherStatus(voucherId));
            return Json(voucherDetailsId, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DraftVoucherStatusSubmit(int voucherId)
        {
            long voucherDetailsId = await Task.Run(() => _accountingService.DraftVoucherStatusSubmit(voucherId));
            return Json(voucherDetailsId, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoCompleteHeadGLGet(string prefix, int companyId)
        {
            var products = _accountingService.GetAutoCompleteHeadGL(prefix, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoCompleteHeadGLForSupplierGet(string prefix, int companyId)
        {
            var products = _accountingService.GetAutoCompleteHeadGLForSupplier(prefix, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoCompleteHeadGLForCustomerGet(string prefix, int companyId)
        {
            var products = _accountingService.GetAutoCompleteHeadGLForCustomer(prefix, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoCompleteHeadGLForSeedProcessingGet(string prefix, int companyId)
        {
            var products = _accountingService.GetAutoCompleteHeadGLForSeedProcessingGet(prefix, companyId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAccountHeadNameByAccountHeadId(int headGLId)
        {
            var products = _accountingService.GetAccountHeadNameByAccountHeadId(headGLId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutoCompleteVendorHeadGet(string prefix, int companyId, int vendorTypeId)
        {
            var products = _accountingService.GetAutoCompleteVendorHeadGL(prefix, companyId, vendorTypeId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetSingleVoucherDetails(int voucherDetailId)
        {
            var model = await _accountingService.GetSingleVoucherDetails(voucherDetailId);
            return Json(model, JsonRequestBehavior.AllowGet);
        } 
        public async Task<JsonResult> GetSingleVoucher(int voucherId)
        {
            var model = await _accountingService.GetSingleVoucher(voucherId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetSinglevoucherTypes(int voucherTypesId)
        {
            var model = await _accountingService.GetSingleVoucherTypes(voucherTypesId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> FeedHeadGLByHead5ParentIdGet(int companyId, int parentId)
        {
            var headGLModel = await Task.Run(() => _accountingService.FeedHeadGLByHeadGLParentIdGet(companyId, parentId));
            var list = headGLModel.Select(x => new { Value = x.Id, Text = x.AccName }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> SupplierPaymentVoucher(int companyId = 0, int voucherId = 0)
        {
            VMJournalSlave vmJournalSlave = new VMJournalSlave();

            if (voucherId == 0)
            {
                vmJournalSlave = await Task.Run(() => _accountingService.GetCompaniesDetails(companyId));

            }
            else if (voucherId > 0)
            {
                vmJournalSlave = await Task.Run(() => _accountingService.GetVoucherDetails(companyId, voucherId));
            }
            vmJournalSlave.CostCenterList = new SelectList(_accountingService.CostCenterDropDownList(companyId), "Value", "Text");
            vmJournalSlave.VoucherTypesList = new SelectList(_accountingService.DRVVoucherTypesDownList(companyId), "Value", "Text");


            var voucherTypes = _accountingService.DRVVoucherTypesDownList(companyId);
            var selectedVoucherTypeId = voucherTypes.FirstOrDefault()?.GetType().GetProperty("Value")?.GetValue(voucherTypes.FirstOrDefault()) ?? 0;
            vmJournalSlave.VoucherTypesList = new SelectList(voucherTypes, "Value", "Text", selectedVoucherTypeId);
            vmJournalSlave.VoucherTypeId = (int)selectedVoucherTypeId;

            return View(vmJournalSlave);
        }


        [HttpPost]

        public async Task<ActionResult> SupplierPaymentVoucher(VMJournalSlave vmJournalSlave)
        {

            if (vmJournalSlave.ActionEum == ActionEnum.Add)
            {
                if (vmJournalSlave.VoucherId == 0)
                {
                    vmJournalSlave.IsStock = false;
                    // it is important / dont delete
                    var voucherNo = voucherService.GetVoucherNo(vmJournalSlave.VoucherTypeId, vmJournalSlave.CompanyFK.Value, vmJournalSlave.Date.Value);
                    vmJournalSlave.VoucherNo = voucherNo;
                    vmJournalSlave.VoucherId = await _accountingService.VoucherAdd(vmJournalSlave);

                }
                await _accountingService.VoucherDetailAdd(vmJournalSlave);
            }
            else if (vmJournalSlave.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _accountingService.VoucherDetailsEdit(vmJournalSlave);
            }
            else if (vmJournalSlave.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _accountingService.VoucherDetailsDelete(vmJournalSlave.VoucherDetailId.Value);
            }

            return RedirectToAction(nameof(SupplierPaymentVoucher), new { companyId = vmJournalSlave.CompanyFK, voucherId = vmJournalSlave.VoucherId });
        }



        [HttpGet]
        public async Task<ActionResult> CustomerCollectionVoucher(int companyId = 0, int voucherId = 0)
        {
            VMJournalSlave vmJournalSlave = new VMJournalSlave();

            if (voucherId == 0)
            {
                vmJournalSlave = await Task.Run(() => _accountingService.GetCompaniesDetails(companyId));

            }
            else if (voucherId > 0)
            {
                vmJournalSlave = await Task.Run(() => _accountingService.GetVoucherDetails(companyId, voucherId));
            }
            vmJournalSlave.CostCenterList = new SelectList(_accountingService.CostCenterDropDownList(companyId), "Value", "Text");
            vmJournalSlave.ProductCategoryList = new SelectList(_accountingService.ProductCategoryDropDownList(companyId), "Value", "Text");
            vmJournalSlave.VoucherTypesList = new SelectList(_accountingService.CRVVoucherTypesDownList(companyId), "Value", "Text");

            var voucherTypes = _accountingService.CRVVoucherTypesDownList(companyId);
            var selectedVoucherTypeId = voucherTypes.FirstOrDefault()?.GetType().GetProperty("Value")?.GetValue(voucherTypes.FirstOrDefault()) ?? 0;
            vmJournalSlave.VoucherTypesList = new SelectList(voucherTypes, "Value", "Text", selectedVoucherTypeId);
            vmJournalSlave.VoucherTypeId = (int)selectedVoucherTypeId;

            return View(vmJournalSlave);
        }


        [HttpPost]

        public async Task<ActionResult> CustomerCollectionVoucher(VMJournalSlave vmJournalSlave)
        {

            if (vmJournalSlave.ActionEum == ActionEnum.Add)
            {
                if (vmJournalSlave.VoucherId == 0)
                {
                    vmJournalSlave.IsStock = false;
                    // it is important / dont delete
                    var voucherNo = voucherService.GetVoucherNo(vmJournalSlave.VoucherTypeId, vmJournalSlave.CompanyFK.Value, vmJournalSlave.Date.Value);
                    vmJournalSlave.VoucherNo = voucherNo;
                    vmJournalSlave.VoucherId = await _accountingService.VoucherAdd(vmJournalSlave);

                }
                await _accountingService.VoucherDetailAdd(vmJournalSlave);
            }
            else if (vmJournalSlave.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _accountingService.VoucherDetailsEdit(vmJournalSlave);
            }
            else if (vmJournalSlave.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _accountingService.VoucherDetailsDelete(vmJournalSlave.VoucherDetailId.Value);
            }

            return RedirectToAction(nameof(CustomerCollectionVoucher), new { companyId = vmJournalSlave.CompanyFK, voucherId = vmJournalSlave.VoucherId });
        }

        [HttpGet]
        public async Task<ActionResult> CollectionVoucherList(int companyId, DateTime? fromDate, DateTime? toDate, bool? vStatus, int? voucherTypeId)
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
            if (vStatus == null)
            {
                vStatus = true;
            }

            VoucherModel voucherModel = new VoucherModel();

            var voucherTypes = _accountingService.CRVVoucherTypesDownList(companyId);
            var selectedVoucherTypeId = voucherTypes.FirstOrDefault()?.GetType().GetProperty("Value")?.GetValue(voucherTypes.FirstOrDefault()) ?? 0;
            voucherModel = await _accountingService.GetVouchersList(companyId, fromDate, toDate, vStatus, (int)selectedVoucherTypeId);
            voucherModel.VoucherTypesList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text");
            voucherModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            voucherModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            voucherModel.IsSubmit = vStatus;

            voucherModel.VoucherTypesList = new SelectList(voucherTypes, "Value", "Text", selectedVoucherTypeId);
            voucherModel.VoucherTypeId = (int)selectedVoucherTypeId;

            return View(voucherModel);
        }

        [HttpPost]
        public async Task<ActionResult> CollectionVoucherList(VoucherModel voucherModel)
        {
            if (voucherModel.CompanyId > 0)
            {
                Session["CompanyId"] = voucherModel.CompanyId;
            }

            voucherModel.FromDate = Convert.ToDateTime(voucherModel.StrFromDate);
            voucherModel.ToDate = Convert.ToDateTime(voucherModel.StrToDate);

            return RedirectToAction(nameof(CollectionVoucherList), new { companyId = voucherModel.CompanyId, fromDate = voucherModel.FromDate, toDate = voucherModel.ToDate, vStatus = voucherModel.IsSubmit, voucherTypeId = voucherModel.VmVoucherTypeId ?? 0 });
        }
        [HttpGet]
        public async Task<ActionResult> PaymentVoucherList(int companyId, DateTime? fromDate, DateTime? toDate, bool? vStatus, int? voucherTypeId)
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
            if (vStatus == null)
            {
                vStatus = true;
            }

            VoucherModel voucherModel = new VoucherModel();

            var voucherTypes = _accountingService.DRVVoucherTypesDownList(companyId);
            var selectedVoucherTypeId = voucherTypes.FirstOrDefault()?.GetType().GetProperty("Value")?.GetValue(voucherTypes.FirstOrDefault()) ?? 0;
            voucherModel = await _accountingService.GetVouchersList(companyId, fromDate, toDate, vStatus, (int)selectedVoucherTypeId);
            voucherModel.VoucherTypesList = new SelectList(_accountingService.VoucherTypesDownList(companyId), "Value", "Text");
            voucherModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            voucherModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            voucherModel.IsSubmit = vStatus;

            voucherModel.VoucherTypesList = new SelectList(voucherTypes, "Value", "Text", selectedVoucherTypeId);
            voucherModel.VoucherTypeId = (int)selectedVoucherTypeId;

            return View(voucherModel);
        }

        [HttpPost]
        public async Task<ActionResult> PaymentVoucherList(VoucherModel voucherModel)
        {
            if (voucherModel.CompanyId > 0)
            {
                Session["CompanyId"] = voucherModel.CompanyId;
            }

            voucherModel.FromDate = Convert.ToDateTime(voucherModel.StrFromDate);
            voucherModel.ToDate = Convert.ToDateTime(voucherModel.StrToDate);

            return RedirectToAction(nameof(PaymentVoucherList), new { companyId = voucherModel.CompanyId, fromDate = voucherModel.FromDate, toDate = voucherModel.ToDate, vStatus = voucherModel.IsSubmit, voucherTypeId = voucherModel.VmVoucherTypeId ?? 0 });
        }

        [HttpPost]
        public async Task<ActionResult> UndoSubmitCollectionVoucher(VoucherModel voucherModel)
        {
            if (voucherModel.VoucherId > 0)
            {
                await _accountingService.VoucherUndoSubmit(voucherModel);
            }
            return RedirectToAction(nameof(CollectionVoucherList), new { companyId = voucherModel.CompanyId });
        }
        [HttpPost]
        public async Task<ActionResult> UndoSubmitPaymentVoucher(VoucherModel voucherModel)
        {
            if (voucherModel.VoucherId > 0)
            {
                await _accountingService.VoucherUndoSubmit(voucherModel);
            }
            return RedirectToAction(nameof(PaymentVoucherList), new { companyId = voucherModel.CompanyId });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCollectionVoucher(VoucherModel voucherModel)
        {
            if (voucherModel.VoucherId > 0)
            {
                await _accountingService.VoucherDelete(voucherModel);

            }
            return RedirectToAction(nameof(CollectionVoucherList), new { companyId = voucherModel.CompanyId });
        }
        [HttpPost]
        public async Task<ActionResult> DeletePaymentVoucher(VoucherModel voucherModel)
        {
            if (voucherModel.VoucherId > 0)
            {
                await _accountingService.VoucherDelete(voucherModel);

            }
            return RedirectToAction(nameof(PaymentVoucherList), new { companyId = voucherModel.CompanyId });
        }
    }
}
