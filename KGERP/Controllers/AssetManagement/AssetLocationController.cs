using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using PagedList;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Controllers.AssetManagement
{
    [CheckSession]
    public class AssetLocationController : BaseController
    {
        private readonly IAssetLocationService assetLocation;

        public AssetLocationController(IAssetLocationService assetLocation)
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
            var location = assetLocation.GetLocation().Where(x => x.Name.ToLower().Contains(searchText.ToLower()));
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);


            return View(location.ToPagedList(No_Of_Page, Size_Of_Page));

        }
        
        [HttpGet]
        public ActionResult CreateOrEdit(int id)
        {
            AssetLocationModel location = assetLocation.GetAssetLocationById(id);
            return View(location);

        }

        
        [HttpPost]
        public ActionResult CreateOrEdit(AssetLocationModel asset)
        {
            var id = asset.LocationId;
            var isSaved = assetLocation.SaveOrEdit(id, asset);

            return RedirectToAction("Index");

        }


    }
}