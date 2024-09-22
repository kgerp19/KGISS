using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Utility;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class BankBranchController : Controller
    {

        IBankBranchService bankBranchService = new BankBranchService();


        public ActionResult Index()
        {
            List<BankBranch> bankBranches = bankBranchService.GetBankBranches();
            return View(bankBranches);
        }

        [HttpPost]
        public ActionResult GetBranchByBank(int bankId)
        {
            List<SelectModel> branches = bankBranchService.GetBankBranchByBank(bankId);
            return Json(branches, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetBranchByCompanyId(int companyId)
        {
            List<SelectModel> branches = bankBranchService.GetBankBranchByCompanyId(companyId);
            return Json(branches, JsonRequestBehavior.AllowGet);
        }


    }
}