using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class BagController : BaseController
    {
        private readonly IBagService bagService;
        public BagController(IBagService bagService)
        {
            this.bagService = bagService;
        }

        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            BagModel bagModel = new BagModel();
            bagModel = await bagService.GetBags(companyId);
            return View(bagModel);
        }
        
        [HttpGet]
        public JsonResult GetBagInfo(int bagId)
        {

            BagModel model = bagService.GetBag(bagId);
            var result = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBagWeightByBagId(int bagId)
        {
            decimal bagValue = bagService.GetBagWeightByBagId(bagId);
            return Json(bagValue, JsonRequestBehavior.AllowGet);
        }
    }
}