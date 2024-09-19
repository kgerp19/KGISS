using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class CompanyController : Controller
    {
        private readonly ICompanyService companyService;
        public CompanyController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }

        
        [HttpGet]
        public ActionResult Index(string searchText)
        {
            List<CompanyModel> companies = companyService.GetCompanies(searchText);
            List<CompanyModel> companiesWithImage = companies.Select(x => new CompanyModel
            {
                CompanyId = x.CompanyId,
                Name = x.Name,
                ShortName = x.ShortName,
                OrderNo = x.OrderNo,
                MushokNo = x.MushokNo,
                IsCompany = x.IsCompany,
                CompanyLogo = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/Logo/" + (string.IsNullOrEmpty(x.CompanyLogo) ? "logo.png" : x.CompanyLogo),
            }).ToList();
            return View(companiesWithImage);
        }

        [HttpGet]
        
        public ActionResult CreateOrEdit(int id)
        {
            CompanyModel company = companyService.GetCompany(id);
            company.CompanyLogo = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/Logo/" + (string.IsNullOrEmpty(company.CompanyLogo) ? "logo.png" : company.CompanyLogo);
            return View(company);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(CompanyModel model)
        {
            ModelState.Clear();
            string message = string.Empty;
            bool result = false;

            if (model.CompanyLogoUpload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(model.CompanyLogoUpload.FileName);
                string extension = Path.GetExtension(model.CompanyLogoUpload.FileName);
                fileName = fileName + "_" + DateTime.Now.ToString("yymmssfff") + extension;
                model.CompanyLogo = fileName;
                model.CompanyLogoUpload.SaveAs(Path.Combine(Server.MapPath("~/Images/Logo/"), fileName));
            }

            if (model.CompanyId <= 0)
            {
                result = companyService.SaveCompany(0, model);
            }
            else
            {
                result = companyService.SaveCompany(model.CompanyId, model);
            }
            if (result)
            {
                TempData["message"] = "Company Saved Successfully !";
            }
            else
            {
                TempData["message"] = "Company not Saved !";
            }
            return RedirectToAction("Index");
        }


        #region BusinessHead
        [HttpGet]
        public ActionResult BusinessHead()
        {
            BusinessHeadModel model = new BusinessHeadModel();
            var data = companyService.GetAllBusinessHead();
            if (data != null)
            {
                model.DataList = data;
            }
            model.ActionType = ActionEnum.Add;
            model.ToDate = null;
            model.FromDate = DateTime.Today;
            return View(model) ;
        }

        [HttpPost]
        public ActionResult BusinessHead(BusinessHeadModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ActionType == ActionEnum.Add)
                {
                    var result = companyService.SaveBusinessHead(model);
                }
                if (model.ActionType == ActionEnum.Edit)
                {
                    var result = companyService.UpdateBusinessHead(model);
                }
            }
            return RedirectToAction("BusinessHead");
        }

        [HttpGet]
        public ActionResult GetBusinessHeadById(int id)
        {
            var data = companyService.GetBusinessHead(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion BusinessHead

    }

}