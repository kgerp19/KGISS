using KGERP.Service.Interface;
using KGERP.Utility;

namespace KGERP.Controllers
{
    [CheckSession]
    public class AccountHeadsController : BaseController
    {
        private readonly IAccountHeadService accountHeadService;

        public AccountHeadsController(IAccountHeadService accountHeadService)
        {
            this.accountHeadService = accountHeadService;
        }


        // GET: AccountHeads
        //public ActionResult Index(int? Page_No, string searchText)
        //{
        //    if (GetCompanyId() > 0)
        //    {
        //        Session["CompanyId"] = GetCompanyId();
        //    }
        //    searchText = searchText ?? "";
        //    List<AccountHeadModel> accountHeads = accountHeadService.GetAccountHeads(Convert.ToInt32(Session["CompanyId"]), searchText);
        //    int Size_Of_Page = 10;
        //    int No_Of_Page = (Page_No ?? 1);
        //    return View(accountHeads.ToPagedList(No_Of_Page, Size_Of_Page));
        //}


        // GET: AccountHeads/Create
        //[HttpGet]
        //[CheckSession]
        //public ActionResult CreateOrEdit(long id)
        //{
        //    string c = Request.QueryString["companyId"];
        //    int companyId = Convert.ToInt32(Session["CompanyId"]);
        //    AccountHeadViewModel vm = new AccountHeadViewModel();
        //    vm.AccountHead = accountHeadService.GetAccountHead(id);
        //    vm.AccountHead.CompanyId = companyId;
        //    vm.ParentHeads = accountHeadService.GetParentAccountHeadSelectModelByCompany(companyId);
        //    return View(vm);
        //}

        // POST: AccountHeads/Create 
        //[HttpPost]
        //[ValidateAntiForgeryToken]

        //public ActionResult CreateOrEdit(AccountHeadViewModel vm)
        //{
        //    bool result = false;
        //    if (vm.AccountHead.AccountHeadId <= 0)
        //    {
        //        result = accountHeadService.SaveAccountHead(0, vm.AccountHead);
        //    }
        //    else
        //    {
        //        result = accountHeadService.SaveAccountHead(vm.AccountHead.AccountHeadId, vm.AccountHead);
        //    }
        //    if (result)
        //    {
        //        TempData["successMessage"] = "Operation Successful !";
        //    }
        //    return RedirectToAction("Index", new { companyId = vm.AccountHead.CompanyId });
        //}

        // GET: AccountHeads/Delete/5
        //public ActionResult Delete(long id)
        //{
        //    AccountHeadModel accountHead = accountHeadService.GetAccountHead(id);
        //    if (accountHead == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(accountHead);
        //}

        // POST: AccountHeads/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    bool result = accountHeadService.DeleteAccountHead(id);
        //    if (result)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        //[HttpPost]
        //public JsonResult AutoComplete(string prefix)
        //{
        //    int companyId = Convert.ToInt32(Session["CompanyId"]);
        //    //int companyId = Convert.ToInt32(Request.QueryString["companyId"]);
        //    var accountHeads = accountHeadService.GetAccountHeadAutoComplete(prefix, companyId);
        //    return Json(accountHeads);
        //}

        //[CheckSession]
        //[HttpGet]
        //public JsonResult GetNextAccountHead(int parentId)
        //{
        //    string newAccountHead = accountHeadService.GenerateNewAccountHead(parentId);

        //    return Json(newAccountHead, JsonRequestBehavior.AllowGet);
        //}


        ////[CheckSession]
        ////[HttpGet]
        ////public ActionResult TreeView()
        ////{
        ////    return View();
        ////}

        //[CheckSession]
        //[HttpGet]
        //public JsonResult TreeViewItems()
        //{
        //    var accountHeadTreeViews = accountHeadService.GetAccountHeadTreeView();

        //    return Json(accountHeadTreeViews, JsonRequestBehavior.AllowGet);
        //}

        ////-----------------------------Tree view Menu-----------------------
        //public ActionResult OnDemand()
        //{
        //    List<AccountHead> all = new List<AccountHead>();
        //    using (ERPEntities dc = new ERPEntities())
        //    {
        //        all = dc.AccountHeads.Where(a => a.ParentId == null && a.CompanyId == 28).ToList();
        //    }
        //    return View(all);
        //}

        //public JsonResult GetSubMenu(string pid)
        //{
        //    // this action for Get Sub Menus from database and return as json data
        //    //System.Threading.Thread.Sleep(5000);
        //    List<AccountHead> subMenus = new List<AccountHead>();
        //    int pID = 0;
        //    int.TryParse(pid, out pID);
        //    using (ERPEntities dc = new ERPEntities())
        //    {
        //        subMenus = dc.AccountHeads.Where(a => a.ParentId == pID && a.CompanyId == 28).ToList();
        //    }

        //    return new JsonResult { Data = subMenus, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}


        ////--------------------------------Final----------------------------------------
        ////public ActionResult TreeView(int companyId)
        ////{
        ////    //int companyId = Convert.ToInt32(Session["CompanyId"]);
        ////    List<AccountHeadModel> accountHeads = accountHeadService.GetAccountHeadsByCompany(companyId);
        ////    return View(accountHeads);
        ////}

        //[HttpGet]
        //[CheckSession]
        //public ActionResult TestTreeView(int companyId)
        //{


        //    companyId = 28;
        //    List<AccountHeadProcessModel> accountHeads = accountHeadService.GetAccountHeadsTreeViewByCompany(companyId);
        //    return View(accountHeads);
        //}

    }
}
