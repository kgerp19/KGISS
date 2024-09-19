using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class KGRECustomerMapHeadGlController : Controller
    {
        private ERPEntities db = new ERPEntities();
        IKgReCrmService kgReCrmService = new KgReCrmService();
        private readonly ICompanyService _companyService;
        // GET: KGRECustomerMapHeadGl
        public KGRECustomerMapHeadGlController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        public async Task<ActionResult> Index(int companyId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer.CList =kgReCrmService.GetMappingCustomer(companyId);
            vmCommonCustomer.HeadList =kgReCrmService.IncomeHeadGLList(companyId);
            vmCommonCustomer.CompanyFK = companyId;
            var company = _companyService.GetCompany(companyId);
            vmCommonCustomer.CompanyName = company.Name;
            return View(vmCommonCustomer);
        }

        [HttpPost]
        public async Task<ActionResult> Index(VMCommonSupplier vM)
        {
            var res = kgReCrmService.CustomerHeadUpdate(vM);
            return RedirectToAction("Index", new { companyId = res});
        }

    }
}