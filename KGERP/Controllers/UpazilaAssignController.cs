using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class UpazilaAssignController : Controller
    {
        private readonly IUpazilaAssignService upazilaAssignService;
        private readonly IDistrictService districtService;
        public UpazilaAssignController(IUpazilaAssignService upazilaAssignService, IDistrictService districtService)
        {
            this.upazilaAssignService = upazilaAssignService;
            this.districtService = districtService;
        }
        [HttpGet]
        
        public ActionResult Index()
        {
            UpazilaAssignViewModel vm = new UpazilaAssignViewModel();
            vm.Districts = districtService.GetDistrictSelectModels();
            return View(vm);
        }

        [HttpPost]
        
        public ActionResult CreateOrEdit(List<UpazilaAssignModel> upazilaAssigns)
        {
            bool result = upazilaAssignService.SaveUpazilaAssign(upazilaAssigns);
            if (result)
            {
                TempData["message"] = "Operation Successful !";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult GetUpazilaListByDistrict(int districtId, long employeeId)
        {
            List<UpazilaAssignModel> upazilaAssigns = upazilaAssignService.GetUpazilaListByDistrictAndEmployee(districtId, employeeId);
            return PartialView("~/Views/UpazilaAssign/_upazilaList.cshtml", upazilaAssigns);
        }

    }
}