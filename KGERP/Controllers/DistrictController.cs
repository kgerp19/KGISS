using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using PagedList;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class DistrictController : BaseController
    {
        private readonly IDistrictService districtService;
        public DistrictController(IDistrictService districtService)
        {
            this.districtService = districtService;
        }

        
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            DistrictModel districtModel = new DistrictModel();
            districtModel = await districtService.GetDistricts();
            return View(districtModel);
        }
        
        [HttpGet]
        public ActionResult CreateOrEdit(int id)
        {
            DistrictModel model = districtService.GetDistrict(id);
            return View(model);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(DistrictModel model)
        {
            if (model.DistrictId <= 0)
            {
                districtService.SaveDistrict(0, model);
            }
            else
            {
                districtService.SaveDistrict(model.DistrictId, model);
            }
            return RedirectToAction("Index");
        }


        public ActionResult Delete(int id)
        {
            DistrictModel model = districtService.GetDistrict(id);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            bool result = districtService.DeleteDistrict(id);
            if (result)
            {
                return RedirectToAction("Index", new { Page_No = 1, searchText = string.Empty });
            }
            return View();
        }

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
    }
}