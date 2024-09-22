using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ZoneController : BaseController
    {
        private readonly IZoneService zoneService;
        public ZoneController(IZoneService zoneService)
        {
            this.zoneService = zoneService;
        }
        public ActionResult Index(int companyId, int? Page_No, string searchText)
        {

            searchText = searchText ?? "";
            List<ZoneModel> models = zoneService.GetZones(companyId, searchText);
            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            return View(models.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        public ActionResult CreateOrEdit(int id)
        {
            ZoneModel model = zoneService.GetZone(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(ZoneModel model)
        {
            if (model.ZoneId <= 0)
            {
                zoneService.SaveZone(0, model);
            }
            else
            {
                zoneService.SaveZone(model.ZoneId, model);
            }
            return RedirectToAction("Index", new { companyId = model.CompanyId });
        }


        public ActionResult SubZoneIndex(int? Page_No, string searchText)
        {
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            searchText = searchText ?? "";
            List<SubZoneModel> models = zoneService.GetSubZones(companyId, searchText);
            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            return View(models.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        public ActionResult SubZoneCreateOrEdit(int id)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            SubZoneViewModel vm = new SubZoneViewModel();
            vm.SubZone = zoneService.GetSubZone(id);

            vm.Zone = zoneService.GetZoneSelectModels(companyId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubZoneCreateOrEdit(SubZoneViewModel model)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            if (model.SubZone.SubZoneId <= 0)
            {
                zoneService.SaveSubZone(0, model.SubZone);
            }
            else
            {
                zoneService.SaveSubZone(model.SubZone.SubZoneId, model.SubZone);
            }
            return RedirectToAction("SubZoneIndex", new { companyId });
        }


        public ActionResult DeleteSubZone(int id)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            zoneService.DeleteSubZone(id, companyId);
            return RedirectToAction("SubZoneIndex", new { companyId });

        }
        
        [HttpPost]
        public JsonResult GetSubZoneSelectModelsByZone(int zoneId)
        {
            var subZone = zoneService.GetSubZoneSelectModelsByZone(zoneId);
            return Json(subZone, JsonRequestBehavior.AllowGet);
        }



    }
}