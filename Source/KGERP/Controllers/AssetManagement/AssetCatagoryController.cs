using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using PagedList;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Controllers.AssetManagement
{
    [CheckSession]
    public class AssetCatagoryController : BaseController
    {
        private readonly IAssetCategoryService assetCategory;

        public AssetCatagoryController(IAssetCategoryService assetCategory)
        {
            this.assetCategory = assetCategory;
        }

        
        [HttpGet]
        public ActionResult Index(int? Page_No, string type, string searchText)
        {
            searchText = searchText ?? "";
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }
            var location = assetCategory.GetAssetCategory().Where(x => x.Name.ToLower().Contains(searchText.ToLower()));
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(location.ToPagedList(No_Of_Page, Size_Of_Page));

        }

        
        [HttpGet]
        public ActionResult CreateOrEdit(int id)
        {
            AssetCategoryModel location = assetCategory.GetAssetCategoryById(id);
            return View(location);

        }


        
        [HttpPost]
        public ActionResult CreateOrEdit(AssetCategoryModel asset)
        {
            var id = asset.AssetCategoryId;
            var isSaved = assetCategory.SaveOrEdit(id, asset);

            return RedirectToAction("Index");

        }


    }
}