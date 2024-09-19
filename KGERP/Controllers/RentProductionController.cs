using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class RentProductionController : Controller
    {
        private readonly IRentProductionService rentProductionService;
        private readonly IVendorService vendorService;
        private readonly IProductService productService;
        public RentProductionController(IRentProductionService rentProductionService, IVendorService vendorService, IProductService productService)
        {
            this.rentProductionService = rentProductionService;
            this.vendorService = vendorService;
            this.productService = productService;
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
            RentProductionModel model = new RentProductionModel();
            model = await rentProductionService.GetRentProductions(companyId, fromDate, toDate);
            model.StrFromDate = fromDate.Value.ToString("yyyy-MM-dd");
            model.StrToDate = toDate.Value.ToString("yyyy-MM-dd");
            return View(model);
        }
        [HttpPost]
        
        public async Task<ActionResult> Index(RentProductionModel model)
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
        public ActionResult Create(int rentProductionId)
        {
            RentProductionModel model = new RentProductionModel();
            model = rentProductionService.GetRentProduction(rentProductionId);
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RentProductionModel model)
        {
            bool status = false;
            string message;
            status = rentProductionService.SaveRentProduction(model.RentProductionId, model, out message);
            if (status)
            {
                TempData["successMessage"] = "Rent production has completed successfully.";
            }
            else
            {
                TempData["successMessage"] = message;
            }
            return RedirectToAction("Index", new { companyId = model.CompanyId});


        }
    }
}