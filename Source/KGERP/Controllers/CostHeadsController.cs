using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class CostHeadsController : Controller
    {
        private readonly ICostHeadsService _Service;
        public CostHeadsController(ICostHeadsService Service)
        {
            this._Service = Service;
        }

        
        [HttpGet]
        public async  Task<ActionResult> Index(int CompanyId)
        {
            BookingCostHeadListViewModel model= new BookingCostHeadListViewModel() { CompanyId=CompanyId};
            model.List =await _Service.GetCostHeadsByCompanyId(CompanyId);
            return View(model);
        }

        [HttpGet]
        
        public ActionResult Create(int CompanyId)
        {
            BookingHeadInsertModel model = new BookingHeadInsertModel() { CompanyId=CompanyId};
            return View(model);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BookingHeadInsertModel model)
        {
            ModelState.Clear();

            bool result = false;
            model.CreatedBy = User.Identity.Name;
            model.CreatedDate = DateTime.Now;
            result = await _Service.AddCostHeads(model);

            if (result)
            {
                TempData["message"] = " Saved Successfully !";
            }
            else
            {
                TempData["message"] = "Not Saved !";
            }
            return RedirectToAction("Index", new {CompanyId=model.CompanyId });
        }

        [HttpGet]
        
        public async Task<ActionResult> Edit(int id,int companyId)
        {
            BookingHeadEditModel model = new BookingHeadEditModel();
            var data = await _Service.GetCostHeadsById(id);
            if (data == null)
            {
                TempData["message"] = "Not found";
                return RedirectToAction("Index", new { CompanyId = companyId });
            }
            model.CompanyId = data.CompanyId;
            model.CostId = data.CostId;
            model.CostName = data.CostName;
            return View(model);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BookingHeadEditModel model)
        {
            ModelState.Clear();

            bool result = false;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            result = await _Service.UpdateCostHeads(model);

            if (result)
            {
                TempData["message"] = " Updated Successfully !";
            }
            else
            {
                TempData["message"] = "Couldn't Update !";
            }
            return RedirectToAction("Index", new { CompanyId = model.CompanyId });
        }

        [HttpGet]
        

        public async Task<ActionResult> Delete(int id,int companyId)
        {
            ModelState.Clear();

            bool result = false;
            result = await _Service.DeleteCostHeads(id);

            if (result)
            {
                TempData["message"] = " Deleted Successfully !";
            }
            else
            {
                TempData["message"] = "Failed to delete !";
            }
            return RedirectToAction("Index", new { CompanyId = companyId });
        }


    }

}