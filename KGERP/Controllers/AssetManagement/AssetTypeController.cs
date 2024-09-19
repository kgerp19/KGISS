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
    public class AssetTypeController : BaseController
    {
        private readonly IAssetCategoryService assetLocation;

        public AssetTypeController(IAssetCategoryService assetLocation)
        {
            this.assetLocation = assetLocation;
        }

        
        [HttpGet]
        public ActionResult Index(int? Page_No, string type, string searchText)
        {
            searchText = searchText ?? "";
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }
            List<AssetTypeModel> assetTypeModel = null;
            if (!string.IsNullOrEmpty(searchText))
            {
                assetTypeModel = assetLocation.GetAssetType().Where(x => x.Name.Contains(searchText) || x.Category.Contains(searchText)).ToList();
            }
            else
            {
                assetTypeModel = assetLocation.GetAssetType().ToList();
            }
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);


            return View(assetTypeModel.ToPagedList(No_Of_Page, Size_Of_Page));
        }


        
        [HttpGet]
        public ActionResult CreateOrEdit(int id)
        {
            AssetTypeViewModel vm = new AssetTypeViewModel();
            vm.AssetType = assetLocation.GetAssetTypeById(id);
            vm.Category = assetLocation.GetAssetCategorySelectModels();
            return View(vm);

        }

        
        [HttpGet]
        public JsonResult GetSerialNo(int catId)
        {
            var serialNo = assetLocation.GetSerialNo(catId);

            return Json(serialNo, JsonRequestBehavior.AllowGet);

        }

        
        [HttpPost]
        public ActionResult CreateOrEdit(AssetTypeViewModel model)
        {
            var id = model.AssetType.AssetTypeId;
            AssetTypeModel asset = model.AssetType;
            var isSaved = assetLocation.SaveOrEditAssetSubType(id, asset);

            return RedirectToAction("Index");

        }
    }
}