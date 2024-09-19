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

        public ActionResult Index(int? Page_No, string searchText)
        {
            searchText = searchText ?? "";
            List<CompanyUserMenuModel> models = companyUserMenuService.GetCompanyUserMenus(searchText);
            int Size_Of_Page = 10;
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
                TempData["message"] = "Menu has assigned successfully";
                return RedirectToAction("Index", "CompanyUserMenus");
            }
            else
            {
                TempData["message"] = message;
            }
            return RedirectToAction("Index", "CompanyUserMenus");
        }



        public ActionResult Delete(int? id)
        {
            bool result = companyUserMenuService.DeleteCompanyUserMenu(id);
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


        //HR Admin menu assign Helper
        //[HttpGet]
        //public ActionResult UserMenuAssignToHR()
        //{

        //    UserMenu userMenu = new UserMenu();
        //    userMenu.UserId = "KG0115";
        //    userMenu.MenuId = 1;
        //    userMenu.SubMenuId = 43;
        //    userMenu.CreatedBy = "KG3071";
        //    userMenu.IsActive = true;
        //    userMenu.IsView = true;
        //    userMenu.CreatedDate = DateTime.Now;
        //    db.UserMenus.Add(userMenu);
        //    db.SaveChanges();


        //    return View();
        //}

    }
}
