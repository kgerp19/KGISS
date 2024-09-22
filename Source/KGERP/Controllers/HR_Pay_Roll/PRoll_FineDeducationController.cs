using KGERP.Service.HR_Pay_Roll_Service.Hr_PRoll_FineDeducation;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
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
    public class PRoll_FineDeducationController : Controller
    {
        private readonly IFineDeductionServices _finedeductionService;
        private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
      
        public PRoll_FineDeducationController(IFineDeductionServices FinedeductionService,IApproval_Service Service, ICompanyService companyService)
        {
            _finedeductionService = FinedeductionService;
              _Service = Service;
            _companyService = companyService;
        }
        public async Task<ActionResult> Add_FineDeduction(int companyId)
        {
            FineDeductionVm model = new FineDeductionVm();
            model.CompanyId = companyId;
            model.YearsList = _Service.YearsListLit();
            model.Year = DateTime.Now.Year;
            model.Month = DateTime.Now.Month - 1;
            //model.Companies = _companyService.GetAllCompanySelectModels2();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Add_FineDeduction(FineDeductionVm model)
        {
            var result = await _finedeductionService.AddDeduction(model);
            if (result > 0)
            {
                return RedirectToAction("Detalis", new { id = result });
            }
            FineDeductionVm model2 = new FineDeductionVm();
            model2.YearsList = _Service.YearsListLit();
            model2.Year = DateTime.Now.Year;
            model2.Month = DateTime.Now.Month - 1;
            model2.Companies = _companyService.GetAllCompanySelectModels2();
            model2.CompanyId = model.CompanyId;
            return View(model2);
          

        }

         [HttpPost]
        public async Task<ActionResult> AddFinedeDetalis(FineDeductionVm model)
        {
            model.DetaliesObject.FineDeducationId = model.FineDeducationId;
            var result = await _finedeductionService.AddFIneDetalis(model.DetaliesObject);
            if (result > 0)
            {
                return RedirectToAction("Detalis", new { id = result });
            }
            return RedirectToAction("Detalis", new { id = model.FineDeducationId });

        }
        public async Task<ActionResult> Detalis(long id)
        {
            var result = await _finedeductionService.Detalis(id);
            return View(result);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateFineDetalis(FineDeductionVm model)
        {
            model.DetaliesObject.FineDeducationId = model.FineDeducationId;
            var result = await _finedeductionService.UpdateFineDetalis(model.DetaliesObject);
            if (result > 0)
            {
                return RedirectToAction("Detalis", new { id = result });
            }
            return View(model);
        }

        public async Task<ActionResult> Delete(long id)
        {
            var result = await _finedeductionService.Delete(id);
            return RedirectToAction("Detalis", new { id = result });
        }
        public async Task<ActionResult> FineDeductionList(int companyId)
        {
            FineDeductionVm model=new FineDeductionVm();
         model = await _finedeductionService.FineList(companyId);
            model.CompanyId = companyId;
            return View(model);
        }
    }
}