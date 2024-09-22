using DocumentFormat.OpenXml.EMMA;
using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Service.Implementation.ApprovalSystemService;
using KGERP.Service.Implementation.ComparativeStatementService.Comparative_Statement;
using KGERP.Service.Interface;
using KGERP.Services.Procurement;
using KGERP.Utility;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static KGERP.Controllers.Custom_Authorization.ParentAuthorizedAttribute;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ComparativeStatementController : Controller
    {
        public readonly IComparativeStatementService _comparativeStatement;
        private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
        public ComparativeStatementController(IComparativeStatementService _comparativeStatement, IApproval_Service Service, ICompanyService companyService)
        {
            this._comparativeStatement = _comparativeStatement;
            _Service = Service;
            _companyService = companyService;
        }
        public ActionResult Index(int CompanyId )
        {
            ComparativeStatementVm vm= new ComparativeStatementVm();
            vm = _comparativeStatement.GetCS(CompanyId);
            return View(vm);
        }


        [HttpGet]
        public ActionResult CreateStatement(int CompanyId,long? CsId)
        {
            ComparativeStatementVm comparativeStatement = new ComparativeStatementVm();
            if (CsId == null)
            {
                comparativeStatement.CSNO = _comparativeStatement.GetCSNO(CompanyId);
                comparativeStatement.CSDate = DateTime.Now;
                comparativeStatement.DeliveryDate = DateTime.Now;
                comparativeStatement.CompanyId = CompanyId;
            }
            else
            {
                comparativeStatement = _comparativeStatement.GetComparativeStatement(CsId);
                comparativeStatement.CompanyId = CompanyId;
            }
            return View(comparativeStatement);

        }

        [HttpPost]
        public ActionResult CreateStatement(ComparativeStatementVm Model)
        {
            ComparativeStatementVm vm = new ComparativeStatementVm();
            vm.CSID=_comparativeStatement.SaveComparativeStateMent(Model);
            return RedirectToAction("CreateStatement", new { CompanyId = Model.CompanyId, CsId=vm.CSID });


        }
        public JsonResult GetProducts(string prefix, int companyId,string productType)
        {
            var obj = _comparativeStatement.GetProductsByCompany(companyId, prefix, productType);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CreateStatementDetails(ComparativeStatementVm Model)
        {
            ComparativeStatementVm vm = new ComparativeStatementVm();
         var obj = _comparativeStatement.SaveComparativeStateMentDetails(Model);
            return RedirectToAction("CreateStatement", new { CompanyId = Model.CompanyId, CsId = Model.CSID });
        }

        [HttpPost]
        public ActionResult EditStatementDetails(ComparativeStatementDetailVm Model)
        {
            var obj = _comparativeStatement.EditComparativeStateMentDetails(Model);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSupplier(int companyId,string prefix)
        {
            var obj = _comparativeStatement.GetSupplier(companyId, prefix);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeletetatementDetails(long csDetailID)
        {
            var obj = _comparativeStatement.DeleteComparativeStateMentDetails(csDetailID);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult RecomendationCheck(long csDetailID)
        {
            var obj = _comparativeStatement.MakeRecomended(csDetailID);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitCS(ComparativeStatementVm Model)
        {
            var result=_comparativeStatement.Submitstatus(Model);
            return RedirectToAction("CreateStatement", new { CompanyId = Model.CompanyId, CsId = Model.CSID });
        }


     
        public ActionResult EditCStament(long? CsId)
        {
            ComparativeStatementVm comparativeStatement = new ComparativeStatementVm();
            comparativeStatement = _comparativeStatement.GetForEdit(CsId);
           return View(comparativeStatement);

        }

        public ActionResult DelteCStament(long? CsId,int CompanyId)
        {
            ComparativeStatementVm comparativeStatement = new ComparativeStatementVm();
            var obj = _comparativeStatement.Deletecs(CsId);
            return RedirectToAction("Index", new { CompanyId = CompanyId});

        }
        [HttpGet]
        public ActionResult ComparativeApproval(int companyId, DateTime? fromDate, DateTime? toDate, SignatoryStatusEnum? Status)
        {
            var userId = Common.GetIntUserId();
            ViewBag.CompanyId = companyId;
            //userId = 1028;
            if (fromDate == null) fromDate = DateTime.Today.AddMonths(-2);
            if (toDate == null) toDate = DateTime.Today;
            var obj = _comparativeStatement.GetSignatureList(companyId, fromDate, toDate, userId, Status);
            return View(obj);

        }
        [HttpGet]
        public async Task<ActionResult> GetApprovalList(int CSID)
        {
            var approvalList = _comparativeStatement.GetAllApproval(CSID);
            return Json(approvalList.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> UpdateApprovalStatus(int companyId, int CSID, SignatoryStatusEnum status, string comment)
        {
            var result =  _comparativeStatement.UpdateCSSignatoryApprovalStatus(CSID, (int)status, comment);
            return RedirectToAction(nameof(ComparativeApproval), new { companyId = companyId });

        }

    }
}