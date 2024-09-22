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
    public class CompanyMenusController : Controller
    {
        private readonly ICompanyMenuService companyMenuService;
        private readonly ICompanyService companyService;

        public CompanyMenusController(ICompanyMenuService companyMenuService, ICompanyService companyService)
        {
            this.companyMenuService = companyMenuService;
            this.companyService = companyService;
        }


        // GET: Menus
        public ActionResult Index(int? Page_No, string searchText)
        {
            List<CompanyMenuModel> companyMenus = companyMenuService.GetCompanyMenus(searchText);
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(companyMenus.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        [HttpGet]
        
        public ActionResult CreateOrEdit(int id)
        {
            CompanyMenuViewModel vm = new CompanyMenuViewModel();
            vm.CompanyMenu = companyMenuService.GetCompanyMenu(id);
            vm.Companies = companyService.GetAllCompanySelectModels();
            return View(vm);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(CompanyMenuViewModel vm)
        {
            if (vm.CompanyMenu.CompanyMenuId <= 0)
            {
                companyMenuService.SaveCompanyMenu(0, vm.CompanyMenu);
            }
            else
            {
                companyMenuService.SaveCompanyMenu(vm.CompanyMenu.CompanyMenuId, vm.CompanyMenu);
            }
            return RedirectToAction("Index", "CompanyMenus", new { companyId = vm.CompanyMenu.CompanyId });
        }


        [HttpPost]
        
        public JsonResult GetCompanyMenuSelectModelsByCompany(int companyId)
        {
            List<SelectModel> companyMenus = companyMenuService.GetCompanyMenuSelectModelsByCompany(companyId);
            return Json(companyMenus, JsonRequestBehavior.AllowGet);
        }


        //// GET: Menus/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //   Menu Menu = db.Menus.Find(id);
        //    if (Menu == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(Menu);
        //}

        //// GET: Menus/Create
        //public ActionResult Create()
        //{
        //    ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name");
        //    return View();
        //}

        //// POST: Menus/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(Menu menu)
        //{
        //    if (menu != null)
        //    {
        //        menu.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
        //        menu.CreatedDate = DateTime.Now;
        //        menu.IsActive = menu.IsActive;
        //        db.Menus.Add(menu);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", menu.CompanyId);
        //    return View(menu);
        //}

        //// GET: Menus/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Menu Menu = db.Menus.Find(id);
        //    if (Menu == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", Menu.CompanyId);
        //    return View(Menu);
        //}

        //// POST: Menus/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Menu Menu)
        //{
        //    if (Menu!=null)
        //    {
        //        Menu oldMenu = db.Menus.FirstOrDefault(x => x.MenuId == Menu.MenuId);
        //        if (oldMenu==null)
        //        {
        //            throw new Exception("Menu not found");
        //        }
        //        oldMenu.OrderNo = Menu.OrderNo;
        //        oldMenu.CompanyId = Menu.CompanyId;
        //        oldMenu.Name = Menu.Name;
        //        oldMenu.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
        //        oldMenu.ModifiedDate = DateTime.Now;
        //        oldMenu.IsActive = Menu.IsActive;
        //        db.Entry(oldMenu).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", Menu.CompanyId);
        //    return View(Menu);
        //}

        //// GET: Menus/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Menu Menu = db.Menus.Find(id);
        //    if (Menu == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(Menu);
        //}

        //// POST: Menus/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Menu Menu = db.Menus.Find(id);
        //    db.Menus.Remove(Menu);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


    }
}
