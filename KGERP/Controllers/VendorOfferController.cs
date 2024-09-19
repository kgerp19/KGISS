using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class VendorOfferController : BaseController
    {
        private readonly IVendorOfferService vendorOfferService;
        public VendorOfferController(IVendorOfferService vendorOfferService)
        {
            this.vendorOfferService = vendorOfferService;
        }

        
        [HttpGet]
        public ActionResult Index(int vendorId, string productType, string searchText)
        {
            searchText = searchText ?? string.Empty;
            var result = vendorOfferService.GetVendorOffers(vendorId, productType, searchText).GroupBy(x => x.ProductCategory);
            return View(result);
        }

        
        [HttpGet]
        public ActionResult CreateOrEdit(int id)
        {
            VendorOfferModel model = vendorOfferService.GetVendorOffer(id);
            return View(model);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(VendorOfferModel model)
        {
            bool result = false;
            if (model.VendorOfferId <= 0)
            {
                result = vendorOfferService.SaveVendorOffer(0, model);
            }
            else
            {
                result = vendorOfferService.SaveVendorOffer(model.VendorOfferId, model);
            }
            if (result)
            {
                return RedirectToAction("Index", new { vendorId = model.VendorId, productType = model.ProductType });
            }
            return View(model);
        }


        //public ActionResult Delete(int id)
        //{
        //    DistrictModel model = districtService.GetDistrict(id);
        //    return View(model);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    bool result = districtService.DeleteDistrict(id);
        //    if (result)
        //    {
        //        return RedirectToAction("Index", new { Page_No = 1, searchText = string.Empty });
        //    }
        //    return View();
        //}

        //public ActionResult Details(int id)
        //{
        //    if (id <= 0)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ProductCategoryModel productCategory = productCategoryService.GetProductCategory(id);
        //    if (productCategory == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(productCategory);
        //}




        public ActionResult InsertCustomerOffer()
        {
            int companyId = (int)Session["CompanyId"];
            int noOfRecords = vendorOfferService.InsertCustomerOffer(companyId);
            TempData["noOfRecords"] = noOfRecords;
            return RedirectToAction("Index", new { vendorId = 0 });
        }
    }
}