using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class UpazilaController : BaseController
    {
        private readonly IDistrictService districtService;
        private readonly IUpazilaService upazilaService;

        public UpazilaController(IDistrictService districtService,
            IUpazilaService upazilaService)
        {
            this.districtService = districtService;
            this.upazilaService = upazilaService;
        }
        
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            UpazilaModel upazilaModel = new UpazilaModel();
            upazilaModel = await upazilaService.GetUpazilas();
            return View(upazilaModel);
        }   
        public ActionResult Details(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UpazilaModel upazila = upazilaService.GetUpazila(id);
            if (upazila == null)
            {
                return HttpNotFound();
            }
            return View(upazila);
        }


        public ActionResult CreateOrEdit(int id)
        {
            UpazilaViewModel vm = new UpazilaViewModel();
            vm.Upazila = upazilaService.GetUpazila(id);
            vm.Districts = districtService.GetDistrictSelectModels();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(UpazilaViewModel vm)
        {

            if (vm.Upazila.UpazilaId <= 0)
            {
                upazilaService.SaveUpazila(0, vm.Upazila);
            }
            else
            {
                upazilaService.SaveUpazila(vm.Upazila.UpazilaId, vm.Upazila);
            }
            return RedirectToAction("Index");
        }



        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UpazilaModel upazila = upazilaService.GetUpazila(id);
            if (upazila == null)
            {
                return HttpNotFound();
            }
            return View(upazila);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            bool result = upazilaService.DeleteUpazila(id);
            if (result)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetUpazilaSelectModelsByDistrict(int districtId)
        {
            List<SelectModel> upazilas = upazilaService.GetUpazilaSelectModelsByDistrict(districtId);
            return Json(upazilas, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetNewUpazilaCode(int districtId)
        {
            string newCode = upazilaService.GetUpazilaCodeByDistrict(districtId);
            return Json(newCode, JsonRequestBehavior.AllowGet);
        }

    }
}
