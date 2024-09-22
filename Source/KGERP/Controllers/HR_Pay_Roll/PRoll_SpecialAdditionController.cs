using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using KGERP.Service.HR_Pay_Roll_Service.SpecialAddition;
using KGERP.Service.Implementation.ApprovalSystemService;
using KGERP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.HR_Pay_Roll
{
    [CheckSession]
    public class PRoll_SpecialAdditionController : Controller
    {
        public readonly ISpecialAdditionServices specialAdditionServices;
        private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
        public PRoll_SpecialAdditionController(ISpecialAdditionServices specialddditionServices, IApproval_Service Service, ICompanyService companyService)
        {
            this.specialAdditionServices = specialddditionServices;
            _Service = Service;
            _companyService = companyService;
        }

        public async Task<ActionResult> Index(int CompanyId)
        {
            SpecialAdditionViewModel obj =new SpecialAdditionViewModel();
          
            obj = await specialAdditionServices.SpecialAddtnList(CompanyId);
          
            return View(obj);
            
        }

        public async Task<ActionResult> Detalis(long id)
        {
            var result = await specialAdditionServices.SpAddDetalis(id);
            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> AddSpecialAdditionDetalis(SpecialAdditionViewModel model)
        
        {
            model.DetaliesObject.SpecialAdditionId = model.SpecialAdditionId;
            var result = await specialAdditionServices.AddSpecialAddition(model.DetaliesObject);
            if (result > 0)
            {
                return RedirectToAction("Detalis", new { id = result });
            }
            return RedirectToAction("Detalis", new { id = model.SpecialAdditionId });

        }

        [HttpPost]
        public async Task<ActionResult> UpdateBillDetalis(SpecialAdditionViewModel model)
        {
            model.DetaliesObject.SpecialAdditionId = model.SpecialAdditionId;
            var result = await specialAdditionServices.UpdateBillDetalis(model.DetaliesObject);
            if (result > 0)
            {
                return RedirectToAction("Detalis", new { id = result });
            }
            return View(model);
        }
        public async Task<ActionResult> Delete(long id)
        {
            var result = await specialAdditionServices.Delete(id);
            return RedirectToAction("Detalis", new { id = result });
        }

        public async Task<ActionResult> Add_SpecialAddition(int CompanyId)
        {
            SpecialAdditionViewModel model = new SpecialAdditionViewModel();
            model.YearsList = _Service.YearsListLit();
            model.Year = DateTime.Now.Year;
            model.Month = DateTime.Now.Month - 1;
            model.CompanyId= CompanyId;
            //model.Companies = _companyService.GetAllCompanySelectModels2();
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> Add_SpecialAddition(SpecialAdditionViewModel model)
        {
            var result = await specialAdditionServices.AddSpecialAddition(model);
            if (result > 0)
            {
                return RedirectToAction("Detalis", new { id = result });
            }
            SpecialAdditionViewModel model2 = new SpecialAdditionViewModel();
            model2.YearsList = _Service.YearsListLit();
            model2.Year = DateTime.Now.Year;
            model2.Month = DateTime.Now.Month - 1;
            model2.Companies = _companyService.GetAllCompanySelectModels2();
            model2.CompanyId = model.CompanyId;
            return View(model2);

        }
        [HttpGet]
        public ActionResult GetEmployeeinfo(int companyId, int? year, int? month)
        {
            var result = specialAdditionServices.GetSpecialAdd(companyId, year, month);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}