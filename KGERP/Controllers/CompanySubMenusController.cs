using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class CompanySubMenusController : Controller
    {
        private readonly ICompanyMenuService companyMenuService;
        private readonly ICompanyService companyService;
        private readonly ICompanySubMenuService companySubMenuService;

        public CompanySubMenusController(ICompanyMenuService companyMenuService, ICompanyService companyService,
            ICompanySubMenuService companySubMenuService)
        {
            this.companyMenuService = companyMenuService;
            this.companyService = companyService;
            this.companySubMenuService = companySubMenuService;
        }

        
        [HttpGet]
        public ActionResult Index(int? Page_No, string searchText)
        {
            searchText = searchText ?? "";
            List<CompanySubMenuModel> companySubMenus = companySubMenuService.GetCompanySubMenus(searchText);
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(companySubMenus.ToPagedList(No_Of_Page, Size_Of_Page));
        }


        [HttpGet]
        
        public ActionResult CreateOrEdit(int id)
        {
            CompanySubMenuViewModel vm = new CompanySubMenuViewModel();
            vm.CompanySubMenu = companySubMenuService.GetCompanySubMenu(id);
            vm.Companies = companyService.GetAllCompanySelectModels();

            if (vm.CompanySubMenu.CompanySubMenuId > 0)
            {
                vm.CompanyMenus = companyMenuService.GetCompanyMenuSelectModelsByCompanyId(vm.CompanySubMenu.CompanyId);
            }
            else
            {
                vm.CompanyMenus = new List<SelectModel>();
            }
            return View(vm);

        }



        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(CompanySubMenuViewModel vm)
        {
            bool result = false;
            if (vm.CompanySubMenu.CompanySubMenuId <= 0)
            {
                result = companySubMenuService.SaveCompanySubMenu(0, vm.CompanySubMenu);
            }
            else
            {
                result = companySubMenuService.SaveCompanySubMenu(vm.CompanySubMenu.CompanySubMenuId, vm.CompanySubMenu);
            }
            return RedirectToAction("Index");
        }



        [HttpPost]
        
        public JsonResult GetCompanySubMenuSelectModelsByCompanyMenu(int menuId)
        {
            List<SelectModel> companySubMenus = companySubMenuService.GetCompanySubMenuSelectModelsByCompanyMenu(menuId);
            return Json(companySubMenus, JsonRequestBehavior.AllowGet);
        }



        // GET: SubMenus/Delete/5
        public ActionResult Delete(int? id)
        {
            bool result = companySubMenuService.DeleteCompanySubMenu(id);
            if (result)
            {
                TempData["message"] = "Deleted Successfully";
            }
            else
            {
                TempData["message"] = "Sorry! Data Not Deleted";
            }
            return RedirectToAction("Index");
        }



        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
