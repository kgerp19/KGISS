using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.ApprovalSystemService;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.Approval_Process_Model;
using KGERP.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers.ApprovalSystem
{
    [CheckSession]
    public class ApprovalProcessSystemAduitController : BaseController
    {
        private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
        public ApprovalProcessSystemAduitController(IApproval_Service Service, ICompanyService companyService)
        {
            _Service = Service;
            _companyService = companyService;
        }

		[HttpGet]
		public async Task<ActionResult> AduitApproval(int companyId = 0, int month = 0, int year = 0, int actionId = 0)
		{
			ApprovalSystemViewModel model = new ApprovalSystemViewModel();

					model.Year = year;
					model.Month = month;
					model.CompanyId = companyId;
					model.SectionEmployeeId = 0;					
					model.YearsList = _Service.YearsListLit();
					model.Companies = _companyService.GetAllCompanySelectModels2();
					model.ActionId = 0;				
				return View(model);
		}

		[HttpPost]
		public async Task<ActionResult> AduitApproval(ApprovalSystemViewModel model2)
		{
			ApprovalSystemViewModel model = new ApprovalSystemViewModel();
			
					model = await _Service.AduitApprovalformanagment(model2);
					model.Year = model2.Year;
					model.Month = model2.Month;
					model.CompanyId = model2.CompanyId;
					model.SectionEmployeeId =0;
					model.YearsList = _Service.YearsListLit();
					model.Companies = _companyService.GetAllCompanySelectModels2();
					model.ActionId = 0;				
				return View(model);
			}

	
	}
}