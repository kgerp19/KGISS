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
    public class DemandController : BaseController
    {

        private readonly IDemandService demandService;
        private readonly IVendorService vendorService;
        private readonly IProductCategoryService productCategoryService;
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DemandController(IDemandService demandService,
            IVendorService vendorService,
            IProductCategoryService productCategoryService)
        {
            this.demandService = demandService;
            this.vendorService = vendorService;
            this.productCategoryService = productCategoryService;
        }

        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId,  DateTime? fromDate, DateTime? toDate)
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
            DemandModel demandModel = new DemandModel();

            demandModel = await demandService.GetDemandList(companyId, fromDate, toDate);

            demandModel.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            demandModel.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
           
            return View(demandModel);
        }

        [HttpPost]
        
        public async Task<ActionResult> Index(DemandModel model)
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
        public ActionResult CreateOrEdit(long id)
        {
            DemandModel demand = demandService.GetDemand(id);
            return View(demand);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(DemandModel model)
        {
            bool result = false;
            string message = string.Empty;
            if (model.DemandId <= 0)
            {
                model.RequisitionType = 2;
                result = demandService.SaveDemand(0, model, out message);
            }
            else
            {
                result = demandService.SaveDemand(model.DemandId, model, out message);

            }
            TempData["successMessage"] = message;
            if (result)
            {
                TempData["successMessage"] = "Demand Created Successfully !";
                return RedirectToAction("Index", new { companyId = model.CompanyId });
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult GetNewDemandNo(string strDemandDate)
        {
            string demandNo = demandService.GetNewDemandNo(strDemandDate);
            return Json(demandNo, JsonRequestBehavior.AllowGet);
        }
        //
        //[HttpGet]
        //public ActionResult GetStoreProduct(int? Page_No, string searchText)
        //{
        //    searchText = searchText == null ? "" : searchText;
        //    List<SoreProductQty> result = storeService.GetStoreProductQty().Where(x => x.ProductName.ToLower().Contains(searchText.ToLower()) || x.StoreName.ToLower().Contains(searchText.ToLower())).ToList();
        //    //return View(result);
        //    int Size_Of_Page = 10;
        //    int No_Of_Page = (Page_No ?? 1);
        //    return View(result.ToPagedList(No_Of_Page, Size_Of_Page));
        //}


        
        [HttpGet]
        public ActionResult DemandItemIndex(long demandId)
        {
            DemandViewModel vm = new DemandViewModel();
            try
            {
                vm.Demand = demandService.GetDemand(demandId);
                vm.DemandItems = demandService.GetDemandItems(demandId);
                vm.DemandItemDetails = demandService.GetDemandItemDetails(demandId);
                return View(vm);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return View(vm);
            }
        }
    }
}