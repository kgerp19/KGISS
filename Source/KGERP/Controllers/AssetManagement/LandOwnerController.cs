using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Controllers.AssetManagement
{
    [CheckSession]
    public class LandOwnerController : Controller
    {
        private readonly ILandOwnerService landService;
        public LandOwnerController(ILandOwnerService landService)
        {
            this.landService = landService;
        }

        
        [HttpGet]
        public ActionResult IndexReceiver(string searchText)
        {
            List<LandReceiverModel> receiver = landService.GetLandReceiver(searchText);
            return View(receiver);
        }

        [HttpGet]
        
        public ActionResult CreateOrEditReceiver(int id)
        {
            LandReceiverModel owner = landService.GetLandReceiver(id);
            return View(owner);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEditReceiver(LandReceiverModel model)
        {
            ModelState.Clear();
            string message = string.Empty;
            bool result = false;



            if (model.LandReceiverId <= 0)
            {
                result = landService.SaveLandReceiver(0, model);
            }
            else
            {
                result = landService.SaveLandReceiver(model.LandReceiverId, model);
            }
            if (result)
            {
                TempData["message"] = "Saved Successfully !";
            }
            else
            {
                TempData["message"] = "Information not Saved !";
            }
            return RedirectToAction("IndexReceiver");
        }




        
        [HttpGet]
        public ActionResult IndexUser(string searchText)
        {
            List<LandUserModel> owner = landService.GetLandUser(searchText);
            return View(owner);
        }

        [HttpGet]
        
        public ActionResult CreateOrEditUser(int id)
        {
            LandUserModel owner = landService.GetLandUser(id);
            return View(owner);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEditUser(LandUserModel model)
        {
            ModelState.Clear();
            string message = string.Empty;
            bool result = false;



            if (model.LandUserId <= 0)
            {
                result = landService.SaveLandUser(0, model);
            }
            else
            {
                result = landService.SaveLandUser(model.LandUserId, model);
            }
            if (result)
            {
                TempData["message"] = "Saved Successfully !";
            }
            else
            {
                TempData["message"] = "Information not Saved !";
            }
            return RedirectToAction("IndexUser");
        }

    }
}