using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class DropDownItemController : Controller
    {
        private readonly IDropDownItemService dropDownItemService;
        private readonly IDropDownTypeService dropDownTypeService;
        public DropDownItemController(IDropDownItemService dropDownItemService, IDropDownTypeService dropDownTypeService)
        {
            this.dropDownItemService = dropDownItemService;
            this.dropDownTypeService = dropDownTypeService;
        }
        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            DropDownItemModel dropDownItemModel = new DropDownItemModel();
            dropDownItemModel = await dropDownItemService.GetDropDownItems(companyId);
            return View(dropDownItemModel);
        }
        

        public ActionResult CreateOrEdit(int id)
        {
            int companyId = (int)Session["CompanyId"];
            DropDownItemViewModel vm = new DropDownItemViewModel();
            vm.DropDownItem = dropDownItemService.GetDropDownItem(id);
            vm.DropDownTypes = dropDownTypeService.GetDropDownTypeSelectModelsByCompany(companyId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(DropDownItemViewModel vm)
        {
            string message = string.Empty;
            if (vm.DropDownItem.DropDownItemId <= 0)
            {
                dropDownItemService.SaveDropDownItem(0, vm.DropDownItem, out message);
            }
            else
            {
                dropDownItemService.SaveDropDownItem(vm.DropDownItem.DropDownItemId, vm.DropDownItem, out message);
            }
            return RedirectToAction("Index", new { companyId = vm.DropDownItem.CompanyId });
        }

        [HttpGet]
        
        public ActionResult DeleteDropDownItem(int id)
        {
            int companyId = (int)Session["CompanyId"];
            bool result = dropDownItemService.DeleteDropDownItem(id);
            if (result)
            {
                return RedirectToAction("Index", new { companyId });
            }
            return View();
        }


        #region Settings for Case management
        
        public ActionResult IndexLnL(int? Page_No, string searchText)
        {
            searchText = searchText ?? "";
            List<DropDownItemModel> models = dropDownItemService.GetDropDownItems(searchText);
            int Size_Of_Page = 10;
            int No_Of_Page = Page_No ?? 1;
            return View(models.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        public ActionResult CreateOrEditLnL(int id)
        {
            DropDownItemViewModel vm = new DropDownItemViewModel();
            vm.DropDownItem = dropDownItemService.GetDropDownItem(id);
            vm.DropDownTypes = dropDownTypeService.GetDropDownTypeSelectModels();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEditLnL(DropDownItemViewModel vm)
        {

            string message = string.Empty;
            if (vm.DropDownItem.DropDownItemId <= 0)
            {
                dropDownItemService.SaveDropDownItem(0, vm.DropDownItem, out message);
            }
            else
            {
                dropDownItemService.SaveDropDownItem(vm.DropDownItem.DropDownItemId, vm.DropDownItem, out message);
            }
            return RedirectToAction("IndexLnL");
        }

        #endregion
    }
}