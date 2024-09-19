using AutoMapper;
using DocumentFormat.OpenXml.ExtendedProperties;
using KGERP.Data.Models;
using KGERP.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.Recruitment;
using KGERP.Service.Implementation.Recruitment.ViewModels;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.Recruitment
{
    [CheckSession]
    public class RecruitmentController : Controller
    {
        public readonly ICompanyService _companyService;
        public readonly IRecruitmentService _recruitmentService;
        public RecruitmentController(IRecruitmentService recruitmentService, ICompanyService companyService)
        {
            _recruitmentService = recruitmentService;
            _companyService = companyService;
        }
        // GET: Recruitment
        [HttpGet]
        public ActionResult Requisition(int companyId=0, int requisitionId = 0)
        {            
            var model = new RecruitmentVM();
            // companyId = 4;
            if (requisitionId > 0)
            {
                var data = _recruitmentService.GetById(requisitionId);
                if (data != null)
                {
                    model = data;
                }               
            }
            else
            {
                var company = _companyService.GetCompany(companyId);
                if (company != null)
                {
                    model.CompanyId = company.CompanyId;
                    model.CompanyName = company.Name;
                }
                model.RequisitionDate = DateTime.Today;
                model.RequisitionNumber =_recruitmentService.GetRecruitmentNumber();
            }
            model.ActionEnum = ActionEnum.Add;           
            model.FeedbackMessage = TempData["feedbackMessage"] as string;

            ViewBag.CompanyId = companyId;
            return View(model);
        }


        [HttpGet]
        public  ActionResult Index(int companyId)
        {            
            var model = new RecruitmentVM();
            var userId = Common.GetUserId();
            var data = _recruitmentService.GetRequisitionList(null, null, null, null).ToList();
           
            
            //ModelMapper mapper = new ModelMapper();
            //var dataaa  = ObjectConverter<RecruitmentVM,RecruitmentRequisition>.Convert(data.FirstOrDefault());

            //return  RedirectToAction("Requisition", new {companyId=4});
            ViewBag.feedbackMessage =  TempData["feedbackMessage"] as string;

            ViewBag.CompanyId = companyId;
            return View(data);
        }

        [HttpPost]
        public  ActionResult AddOrUpdate(RecruitmentVM model)
        {
            int company = (int)model.CompanyId;
            if (ModelState.IsValid || true)
            {
        
                if (model.BusinessType == BusinessTypeEnum.Company)
                {
                    model.CompanyId = model.BusineesId_Fk;
                    model.DepartmentId = null;
                }
                if (model.BusinessType == BusinessTypeEnum.Division)
                {
                    model.CompanyId = null;
                    model.DepartmentId = model.BusineesId_Fk;
                }


                var result = new MethodFeedbackVM();
                if (model.ActionEnum == ActionEnum.Add && model.Id == 0)
                {
                    result =   _recruitmentService.Add(model);
                    TempData["feedbackMessage"] = $"<label style=\"color: {(result.Status == true ? ColorEnum.Green : ColorEnum.Red)};\">{result.Message}</label>";
                    if (result.Status)
                    {
                        return RedirectToAction("Requisition", new { companyId = company, requisitionId = model.Id });
                    }
                }
                if (model.ActionEnum == ActionEnum.Add && model.Id > 0 && model.RequisitionDetailId == 0)
                {
                    result =  _recruitmentService.AddRecruitmentItemDetail(model);
                    TempData["feedbackMessage"] = $"<label style=\"color: {(result.Status == true ? ColorEnum.Green : ColorEnum.Red)};\">{result.Message}</label>";
                    if (result.Status)
                    {
                        return RedirectToAction("Requisition", new { companyId = company, requisitionId = model.Id });
                    }
                }
                if (model.ActionEnum == ActionEnum.Edit && model.Id > 0 && model.RequisitionDetailId > 0)
                {
                    result = _recruitmentService.UpdateRecruitmentItemDetail(model);
                    TempData["feedbackMessage"] = $"<label style=\"color: {(result.Status == true ? ColorEnum.Green : ColorEnum.Red)};\">{result.Message}</label>";
                    if (result.Status)
                    {
                        return RedirectToAction("Requisition", new { companyId = model.CompanyId??0, requisitionId = model.Id });
                    }
                }
                model.FeedbackMessage = $"<label style=\"color: {(result.Status == true ? ColorEnum.Green : ColorEnum.Red)};\">{result.Message}</label>";
                
            }
            int? companyId = model.CompanyId;
            string errorMessage = "";
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                     errorMessage += error.ErrorMessage;
                    // Handle error message as needed
                }
            }
            model.FeedbackMessage = $"<label style=\"color: {ColorEnum.Red};\">{errorMessage}</label>";
            return RedirectToAction("Requisition", new { companyId = company, requisitionId = model.Id });
        }

        [HttpPost]
        public ActionResult Update(RecruitmentVM model)
        {
            if (model.Id > 0)
            {
                var result = new MethodFeedbackVM();
                result = _recruitmentService.Update(model);
                TempData["feedbackMessage"] = $"<label style=\"color: {(result.Status ? ColorEnum.Green : ColorEnum.Red)};\">{result.Message}</label>";
                if (result.Status)
                {
                    return RedirectToAction("Index", new { companyId = model.CompanyId??0 });
                }
            }
            return RedirectToAction("Index", new { companyId = model.CompanyId ?? 0 });
        }

        [HttpPost]
        public ActionResult Remove(int id, int companyId = 0)
        {
            RecruitmentRequisition model = new RecruitmentRequisition();
           
            if (id > 0)
            {
                var result = new MethodFeedbackVM();
                result = _recruitmentService.Remove(id,out model);
                TempData["feedbackMessage"] = $"<label style=\"color: {(result.Status? ColorEnum.Green : ColorEnum.Red)};\">{result.Message}</label>";
                if (result.Status)
                {
                    return RedirectToAction("Index", new { companyId = companyId });
                }                
            }
            return RedirectToAction("Index", new { companyId = companyId });
        }
        [HttpGet]
        public ActionResult GetRequisitionItemDetailById(int id)
        {
            var data = _recruitmentService.GetRequisitionItemDetailById(id);
            return Json(data,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveRequisitionDetials(int id,int companyId = 0)
        {
            RecruitmentRequisitionDetail model = new RecruitmentRequisitionDetail();
            if (id > 0)
            {
                var result = new MethodFeedbackVM();
                result = _recruitmentService.RemoveRequisitionDetials(id, out model);
                TempData["feedbackMessage"] = $"<label style=\"color: {(result.Status ? ColorEnum.Green : ColorEnum.Red)};\">{result.Message}</label>";
                if (result.Status)
                {
                    return RedirectToAction("Requisition", new {companyId = companyId, requisitionId = model.RecruitmentRequisitionId });
                }
            }
            return RedirectToAction("Requisition", new {companyId = companyId});
        }
        [HttpPost]
        public ActionResult UpdateStaus(int id,RecruitmentStatusEnum status)
        {

            RecruitmentRequisition model = new RecruitmentRequisition();
            if (id > 0)
            {
                var result = new MethodFeedbackVM();
                result = _recruitmentService.UpdateStatus(id,status, out model);
                TempData["feedbackMessage"] = $"<label style=\"color: {(result.Status ? ColorEnum.Green : ColorEnum.Red)};\">{result.Message}</label>";
                return RedirectToAction("Requisition", new { companyId = 0, requisitionId = model.Id });

            }
            return RedirectToAction("Requisition", new { companyId = 0, requisitionId = id });
        }


        [HttpGet]
        public ActionResult Update(int id)
        {
            if (id > 0)
            {
                var model = _recruitmentService.GetById(id);
                if (model != null)
                {
                    model.ActionEnum = ActionEnum.Edit;
                    return View("Requisition",model);
                }
                else
                {

                    TempData["feedbackMessage"] = $"<label style=\"color: {(ColorEnum.Red)};\">No Data Found</label>";
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult RecruitmentDetail(int id)
        {

            var data = _recruitmentService.GetById(id);
            return View(data);

        }

        #region Approval
        //[HttpGet]
        //public ActionResult Approval(int companyId)
        //{
        //    var data = _recruitmentService.GetApprovalList(null,null,null);
        //    ViewBag.CompanyId = companyId;
        //    return View(data);
        //}
        [HttpGet]
        public ActionResult Approval(int companyId, BindingManagerVM modelVM)
        {
           var data = _recruitmentService.GetApprovalList(modelVM.SignatoryStatus, modelVM.fromDate, modelVM.toDate);
            ViewBag.CompanyId = companyId;
            return View(data);
        }

        public ActionResult GetRecruitmentSignatoryList(int id)
        {
            var data = _recruitmentService.GetRecruitmentSignatoryList(id);
            return Json(data,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateApprovalStatus(EmployeeClearanceVM model)
        {
            var result = _recruitmentService.UpdateRecruitmentSignatoryApprovalStatus(model.Id, model.SignatoryStatus, model.Comment);
            return RedirectToAction("Approval", new { companyId = model.UserCompanyId });
        }
        #endregion
    }
}