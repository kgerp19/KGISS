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
    public class InstallmentTypeController : Controller
    {
        private readonly IInstallmentTypeService _Service;
        public InstallmentTypeController(IInstallmentTypeService Service)
        {
            this._Service = Service;
        }

        
        [HttpGet]
        public async  Task<ActionResult> Index(int CompanyId)
        {
            InstallmentTypeViewModel model = new InstallmentTypeViewModel() { CompanyId=CompanyId};
            model.List  =await _Service.GetAllInstallmentTypesByCompanyId(CompanyId);
            return View(model);
        }

        [HttpGet]
        
        public ActionResult Create(int CompanyId=7)
        {
            InstallmentTypeInsertModel model = new InstallmentTypeInsertModel() {
                CompanyId = CompanyId
            };
            return View(model);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InstallmentTypeInsertModel model)
        {
            ModelState.Clear();
            
            bool result = false;
            model.CreatedBy = User.Identity.Name;
            model.CreatedDate=DateTime.Now;
            result = await _Service.AddInstallmentType(model);
           
            if (result)
            {
                TempData["message"] = " Saved Successfully !";
            }
            else
            {
                TempData["message"] = "Not Saved !";
            }
            return RedirectToAction("Index",new { CompanyId=model.CompanyId });
        }

        [HttpGet]
        
        public async Task< ActionResult> Edit(int id)
        {
            InstallmentTypeEditModel model = new InstallmentTypeEditModel();
            var data= await _Service.GetInstallmentTypeById(id); 
            if (data == null)
            {
                TempData["message"] = "Not found";
                return RedirectToAction(nameof(Index));
            }
            model.ModifiedDate = data.ModifiedDate;
            model.ModifiedBy = data.ModifiedBy;
            model.InstallmentTypeId = data.InstallmentTypeId;
            model.IsOneTime = data.IsOneTime;
            model.Name = data.Name;
            model.CompanyId = data.CompanyId;
            model.IntervalMonths = data.IntervalMonths;
            return View(model);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(InstallmentTypeEditModel model)
        {
            ModelState.Clear();

            bool result = false;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedDate = DateTime.Now;
            result = await _Service.UpdateInstallmentType(model);

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
        
       
        public async Task<ActionResult> Delete(int id)
        {
            ModelState.Clear();

            var data =await _Service.GetInstallmentTypeById(id);
            bool result = false;
            result = await _Service.DeleteInstallmentType(id);

            if (result)
            {
                TempData["message"] = " Deleted Successfully !";
            }
            else
            {
                TempData["message"] = "Failed to delete !";
            }
            return RedirectToAction("Index",new { CompanyId=data.CompanyId});
        }


    }

}