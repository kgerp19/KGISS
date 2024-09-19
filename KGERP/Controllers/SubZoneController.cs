using KGERP.Service.Interface;
using KGERP.Utility;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class SubZoneController : BaseController
    {
        private readonly ISubZoneService subZoneService;
        public SubZoneController(ISubZoneService subZoneService)
        {
            this.subZoneService = subZoneService;
        }


        [HttpPost]
        public ActionResult GetSubZoneSelectModelsByZone(int zoneId)
        {
            List<SelectModel> subZones = subZoneService.GetSubZoneSelectModelsByZone(zoneId);
            return Json(subZones, JsonRequestBehavior.AllowGet);
        }

    }
}