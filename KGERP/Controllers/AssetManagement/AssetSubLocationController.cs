using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Controllers.AssetManagement
{
    [CheckSession]
    public class AssetSubLocationController : BaseController
    {
        private readonly IAssetLocationService assetLocation;

        public AssetSubLocationController(IAssetLocationService assetLocation)
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
            var location = assetLocation.GetSubLocation().Where(x => (x.AssetLocation.Name.ToLower().Contains(searchText.ToLower()) || (x.Name.ToLower().Contains(searchText.ToLower()))));
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);


            return View(location.ToPagedList(No_Of_Page, Size_Of_Page));
        }


        
        [HttpGet]
        public ActionResult CreateOrEdit(int id)
        {
            AssetSubLocationViewModel vm = new AssetSubLocationViewModel();
            vm.SubLocation = assetLocation.GetAssetSubLocationById(id);
            vm.Location = assetLocation.GetLocationSelectModels();
            return View(vm);

        }

        
        [HttpGet]
        public JsonResult GetSerialNo(int locationId)
        {
            var serialNo = assetLocation.GetSerialNo(locationId);

            return Json(serialNo, JsonRequestBehavior.AllowGet);

        }

        
        [HttpPost]
        public ActionResult CreateOrEdit(AssetSubLocationViewModel model)
        {
            var id = model.SubLocation.SubLocationId;
            AssetSubLocationModel asset = model.SubLocation;
            var isSaved = assetLocation.SaveOrEditAssetSubLocation(id, asset);

            return RedirectToAction("Index");

        }
    }
}