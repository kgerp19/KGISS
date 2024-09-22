using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Controllers.AssetManagement
{
    [CheckSession]
    public class AssetAssignController : BaseController
    {
        private IAssignAssetService assetService;
        public AssetAssignController(IAssignAssetService assetService)
        {
            this.assetService = assetService;
        }
        // GET: AssetAssign
        
        [HttpGet]
        public ActionResult Index(int? Page_No, string type, string searchText)
        {
            searchText = searchText ?? "";
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }

            List<AssetAssignModel> assignedAssetModel = null;
            assignedAssetModel = assetService.Index();
            var assetList = assetService.Index().Where(x => (x.Employee.Name.ToLower().Contains(searchText.ToLower())) || (x.AssetType.Name.ToLower().Contains(searchText.ToLower())));
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(assignedAssetModel.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        public ActionResult CreateOrEdit(int id)
        {
            AssetAssignViewModel vm = new AssetAssignViewModel();
            vm.AssetAssign = assetService.GetAssetAssignById(id);
            vm.Company = assetService.Company();
            vm.AssetLocation = assetService.AssetLocation();
            vm.AssetSubLocation = assetService.AssetSubLocation(vm.AssetAssign.AssetLocationId);
            vm.AssetCategory = assetService.AssetCategory();
            vm.AssetType = assetService.AssetType(vm.AssetAssign.AssetCategoryId);
            vm.Asset = assetService.SerialNo(vm.AssetAssign.AssetTypeId, vm.AssetAssign.CompanyId);
            return View(vm);
        }

        
        [HttpPost]
        public ActionResult CreateOrEdit(AssetAssignViewModel vm)
        {
            AssetAssignModel model = vm.AssetAssign;
            assetService.SaveOrEdit(model);
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public JsonResult GetAssetTypeByAssetCategory(int categoryId)
        {
            var asset = assetService.AssetType(categoryId);
            return Json(asset, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public JsonResult GetSubLocationByLocationId(int locationId)
        {
            var locatin = assetService.AssetSubLocation(locationId);
            return Json(locatin, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public JsonResult GetAssetSerialNo(int assetTypeId, int companyId)
        {
            var locatin = assetService.SerialNo(assetTypeId, companyId);
            return Json(locatin, JsonRequestBehavior.AllowGet);
        }
    }
}