using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class CompanyUserMenusController : Controller
    {
        private readonly ICompanyMenuService companyMenuService;
        private readonly ICompanySubMenuService companySubMenuService;
        private readonly ICompanyService companyService;
        private readonly ICompanyUserMenuService companyUserMenuService;
        public CompanyUserMenusController(ICompanyMenuService companyMenuService, ICompanySubMenuService companySubMenuService,
            ICompanyService companyService,
            ICompanyUserMenuService companyUserMenuService)
        {
            this.companyMenuService = companyMenuService;
            this.companySubMenuService = companySubMenuService;
            this.companyUserMenuService = companyUserMenuService;
            this.companyService = companyService;
        }

        // Index with filter support
        public ActionResult Index(int? Page_No, string searchText,
            int? companyId, int? menuId, int? subMenuId)
        {
            searchText = searchText ?? "";

            // Load company list for filter dropdown
            ViewBag.Companies = companyService.GetAllCompanySelectModels();
            ViewBag.SearchText = searchText;
            ViewBag.CompanyId = companyId;
            ViewBag.MenuId = menuId;
            ViewBag.SubMenuId = subMenuId;

            List<CompanyUserMenuModel> models;

            // Use filtered search if any filter is applied
            if (!string.IsNullOrEmpty(searchText) || companyId > 0 || menuId > 0 || subMenuId > 0)
            {
                models = companyUserMenuService.GetCompanyUserMenus(searchText, companyId, menuId, subMenuId);
            }
            else
            {
                models = new List<CompanyUserMenuModel>();
            }

            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            return View(models.ToPagedList(No_Of_Page, Size_Of_Page));
        }


        [HttpGet]
        public ActionResult CreateOrEdit(int id)
        {
            CompanyUserMenuViewModel vm = new CompanyUserMenuViewModel();
            vm.CompanyUserMenu = companyUserMenuService.GetCompanyUserMenu(id);
            vm.Companies = companyService.GetAllCompanySelectModels();

            if (vm.CompanyUserMenu.CompanyUserMenuId > 0)
            {
                vm.CompanyMenus = companyMenuService.GetCompanyMenuSelectModelsByCompanyId(vm.CompanyUserMenu.CompanyId);
                vm.CompanySubMenus = companySubMenuService.GetCompanySubMenuSelectModelsByCompanyMenu(vm.CompanyUserMenu.CompanyMenuId);
            }
            else
            {
                vm.CompanyMenus = new List<SelectModel>();
                vm.CompanySubMenus = new List<SelectModel>();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(CompanyUserMenuViewModel vm)
        {
            bool result = false;
            string message = string.Empty;
            if (vm.CompanyUserMenu.CompanyUserMenuId <= 0)
            {
                result = companyUserMenuService.SaveCompanyUserMenu(0, vm.CompanyUserMenu, out message);
            }
            else
            {
                result = companyUserMenuService.SaveCompanyUserMenu(vm.CompanyUserMenu.CompanyUserMenuId, vm.CompanyUserMenu, out message);
            }
            if (result)
            {
                TempData["message"] = "Permission saved successfully";
                TempData["messageType"] = "success";
                // Redirect back to Index with userId pre-filled so results auto-show
                return RedirectToAction("Index", "CompanyUserMenus",
                    new { searchText = vm.CompanyUserMenu.UserId });
            }
            else
            {
                TempData["message"] = message;
                TempData["messageType"] = "error";
            }
            return RedirectToAction("Index", "CompanyUserMenus",
                new { searchText = vm.CompanyUserMenu.UserId });
        }


        public ActionResult Delete(int? id)
        {
            // Get the userId before deleting, so we can redirect back with it
            string userId = "";
            if (id.HasValue)
            {
                var existing = companyUserMenuService.GetCompanyUserMenu(id.Value);
                if (existing != null) userId = existing.UserId;
            }

            bool result = companyUserMenuService.DeleteCompanyUserMenu(id);
            if (result)
            {
                TempData["message"] = "Deleted Successfully";
                TempData["messageType"] = "success";
            }
            else
            {
                TempData["message"] = "Sorry! Data Not Deleted";
                TempData["messageType"] = "error";
            }
            return RedirectToAction("Index", new { searchText = userId });
        }

        // JSON: Check if permission record already exists for Company+Menu+SubMenu+User
        [HttpPost]
        public JsonResult GetExistingPermission(int companyId, int menuId, int subMenuId, string userId)
        {
            if (companyId <= 0 || menuId <= 0 || subMenuId <= 0 || string.IsNullOrEmpty(userId))
            {
                return Json(new { found = false });
            }

            // Search for existing record
            var results = companyUserMenuService.GetCompanyUserMenus(userId, companyId, menuId, subMenuId);
            var match = results.FirstOrDefault(x =>
                x.CompanyId == companyId &&
                x.CompanyMenuId == menuId &&
                x.CompanySubMenuId == subMenuId &&
                x.UserId.Equals(userId, System.StringComparison.OrdinalIgnoreCase));

            if (match != null)
            {
                return Json(new
                {
                    found = true,
                    companyUserMenuId = match.CompanyUserMenuId,
                    isView = match.IsView,
                    isAdd = match.IsAdd,
                    isUpdate = match.IsUpdate,
                    isDelete = match.IsDelete,
                    isActive = match.IsActive
                });
            }

            return Json(new { found = false });
        }

    }
}
