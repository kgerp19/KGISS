using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.Utility;
using KGERP.Utility;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class LeaveCategoryController : Controller
    {
        private readonly ILeaveCategoryService leaveCategoryService;
        public LeaveCategoryController(ILeaveCategoryService leaveCategoryService)
        {
            this.leaveCategoryService = leaveCategoryService;
        }

        public ActionResult Index()
        {
            var restlt = LeaveService.LeaveCalculation();
            List<LeaveCategoryModel> leaveCategories = leaveCategoryService.GetLeaveCategories();
            return View(leaveCategories);
        }


        public ActionResult CreateOrEdit(int id)
        {
            LeaveCategoryModel model = leaveCategoryService.GetLeaveCategory(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(LeaveCategoryModel model)
        {
            if (model.LeaveCategoryId <= 0)
            {
                leaveCategoryService.SaveLeaveCategory(0, model);
            }
            else
            {
                leaveCategoryService.SaveLeaveCategory(model.LeaveCategoryId, model);
            }

            return RedirectToAction("Index");
        }
    }
}
