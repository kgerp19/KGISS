using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.Utility.Util;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KGERP.Controllers
{
    [CheckSession]
    public class LandNLegalsController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ERPEntities db = new ERPEntities();
        //private readonly IDivisionService districtService;
        IDistrictService districtService = new DistrictService(new ERPEntities());
        IUpazilaService upazilaService = new UpazilaService(new ERPEntities());
        ILandNLegalService landNLegalService = new LandNLegalService();
        IDropDownItemService dropDownItemService = new DropDownItemService(new ERPEntities());
        ICompanyService companyService = new CompanyService(new ERPEntities());
        IBankService bankService = new BankService();

        // GET: LandNLegals
        
        [HttpGet]
        public ActionResult Index(int? Page_No, string CaseNo, string defendantName, string closedCaseStatus,
            string ResponsibleLawyer, string PlaintiffAppellant, string Company_Name, string Case_Type,
            string CaseStatus, string CourtName, string Division, string District, string ThanaUpazila)
        {
            ViewBag.Companies = companyService.GetFilterCompanySelectModels();
            ViewBag.CaseTypes = dropDownItemService.GetDropDownItemSelectModels(28);
            ViewBag.CaseStatuss = dropDownItemService.GetDropDownItemSelectModels(46);
            ViewBag.CourtNames = dropDownItemService.GetDropDownItemSelectModels(49);
            ViewBag.ResponsiblePersons = landNLegalService.GetLandNLegalEmployees();
            ViewBag.Divisions = districtService.GetDivisionSelectModels();
            ViewBag.Districts = districtService.GetDistrictSelectModels();
            ViewBag.Upzillas = upazilaService.GetUpzilaSelectModels();
            ViewBag.Banks = bankService.GetBankSelectModels();
            

            if (string.IsNullOrEmpty(closedCaseStatus))
            {
                closedCaseStatus = "";
            }
            List<LandNLegalModel> _LandNLegal = db.Database.SqlQuery<LandNLegalModel>("LnL_GetClosedCase {0}", closedCaseStatus).ToList();


            if (!string.IsNullOrWhiteSpace(ResponsibleLawyer))
                _LandNLegal = _LandNLegal.Where(m => m.ResponsibleLayer == ResponsibleLawyer).ToList();

            if (!string.IsNullOrWhiteSpace(PlaintiffAppellant))
            {
                _LandNLegal = _LandNLegal.Where(m => m.PlaintiffAppellant.ToUpper().Contains(PlaintiffAppellant.ToUpper())).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Company_Name))
            {
                _LandNLegal = _LandNLegal.Where(m => m.CompanyName == Company_Name).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Case_Type))
                _LandNLegal = _LandNLegal.Where(m => m.CaseType == Case_Type).ToList();

            if (!string.IsNullOrWhiteSpace(CaseStatus))
                _LandNLegal = _LandNLegal.Where(m => m.CaseStatus == CaseStatus).ToList();

            if (!string.IsNullOrWhiteSpace(CourtName))
                _LandNLegal = _LandNLegal.Where(m => m.CourtName == CourtName).ToList();

            if (!string.IsNullOrWhiteSpace(Division))
                _LandNLegal = _LandNLegal.Where(m => m.Division == Division).ToList();
            if (!string.IsNullOrWhiteSpace(District))
                _LandNLegal = _LandNLegal.Where(m => m.District == District).ToList();

            if (!string.IsNullOrWhiteSpace(ThanaUpazila))
                _LandNLegal = _LandNLegal.Where(m => m.ThanaUpazila == ThanaUpazila).ToList();

            if (!string.IsNullOrEmpty(CaseNo))
                _LandNLegal = db.Database.SqlQuery<LandNLegalModel>("Select * from LandNLegal where CaseStatus not In ('Judgement Completed','Withdrawn','Kharij') and CaseNo like '%" + CaseNo + "%'").ToList();
            //_LandNLegal = _LandNLegal.Where(m => m.CaseNo.Contains("" + CaseNo + "")).ToList();

            if (!string.IsNullOrWhiteSpace(defendantName))
                // _LandNLegal = _LandNLegal.Where(m => m.DefendantName.Contains(defendantName)).ToList();
                _LandNLegal = db.Database.SqlQuery<LandNLegalModel>("Select * from LandNLegal where CaseStatus not In ('Judgement Completed','Withdrawn','Kharij') and DefendantName like '%" + defendantName + "%'").ToList();
            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            return View(_LandNLegal.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: LandNLegals
        
        [HttpGet]
        public ActionResult ClosedCase(int? Page_No, string CaseNo, string closedCaseStatus, string defendantName,
            string ResponsibleLawyer, string BankName, string Company_Name, string Case_Type,
            string CaseStatus, string CourtName, string Division, string District, string ThanaUpazila)
        {
            ViewBag.Companies = companyService.GetFilterCompanySelectModels();
            ViewBag.CaseTypes = dropDownItemService.GetDropDownItemSelectModels(28);
            ViewBag.CaseStatuss = dropDownItemService.GetDropDownItemSelectModels(46);
            ViewBag.CourtNames = dropDownItemService.GetDropDownItemSelectModels(49);
            ViewBag.ResponsiblePersons = landNLegalService.GetLandNLegalEmployees();
            ViewBag.Divisions = districtService.GetDivisionSelectModels();
            ViewBag.Districts = districtService.GetDistrictSelectModels();
            ViewBag.Upzillas = upazilaService.GetUpzilaSelectModels();
            ViewBag.Banks = bankService.GetBankSelectModels();
            if (string.IsNullOrEmpty(closedCaseStatus))
            {
                closedCaseStatus = "2";
            }
            List<LandNLegalModel> _LandNLegal = db.Database.SqlQuery<LandNLegalModel>("LnL_GetClosedCase {0}", closedCaseStatus).ToList();


            if (!string.IsNullOrWhiteSpace(ResponsibleLawyer))
                _LandNLegal = _LandNLegal.Where(m => m.ResponsibleLayer == ResponsibleLawyer).ToList();

            if (!string.IsNullOrWhiteSpace(BankName))
            {
                _LandNLegal = _LandNLegal.Where(m => m.BankName == BankName).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Company_Name))
            {
                _LandNLegal = _LandNLegal.Where(m => m.CompanyName == Company_Name).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Case_Type))
                _LandNLegal = _LandNLegal.Where(m => m.CaseType == Case_Type).ToList();

            if (!string.IsNullOrWhiteSpace(CaseStatus))
                _LandNLegal = _LandNLegal.Where(m => m.CaseStatus == CaseStatus).ToList();

            if (!string.IsNullOrWhiteSpace(CourtName))
                _LandNLegal = _LandNLegal.Where(m => m.CourtName == CourtName).ToList();

            if (!string.IsNullOrWhiteSpace(Division))
                _LandNLegal = _LandNLegal.Where(m => m.Division == Division).ToList();
            if (!string.IsNullOrWhiteSpace(District))
                _LandNLegal = _LandNLegal.Where(m => m.District == District).ToList();

            if (!string.IsNullOrWhiteSpace(ThanaUpazila))
                _LandNLegal = _LandNLegal.Where(m => m.ThanaUpazila == ThanaUpazila).ToList();

            if (!string.IsNullOrEmpty(CaseNo))
                _LandNLegal = db.Database.SqlQuery<LandNLegalModel>("Select * from LandNLegal where CaseStatus not In ('Judgement Completed','Withdrawn','Kharij') and CaseNo like '%" + CaseNo + "%'").ToList();
            //_LandNLegal = _LandNLegal.Where(m => m.CaseNo.Contains("" + CaseNo + "")).ToList();

            if (!string.IsNullOrWhiteSpace(defendantName))
                // _LandNLegal = _LandNLegal.Where(m => m.DefendantName.Contains(defendantName)).ToList();
                _LandNLegal = db.Database.SqlQuery<LandNLegalModel>("Select * from LandNLegal where CaseStatus not In ('Judgement Completed','Withdrawn','Kharij') and DefendantName like '%" + defendantName + "%'").ToList();

            //_LandNLegalModel = ObjectConverter<LandNLegal, LandNLegalModel>.ConvertList(_LandNLegal.ToList()).ToList();

            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            //return View(_LandNLegalModel.Where(x => x.CaseStatus == "Judgement Completed" || x.CaseStatus == "Withdrawn").ToPagedList(No_Of_Page, Size_Of_Page));
            return View(_LandNLegal.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: LandNLegals
        
        [HttpGet]
        public ActionResult CompanyBaseCaseList(int? Page_No, string searchText, int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            int comId = (int)Session["CompanyId"];

            searchText = searchText ?? string.Empty;
            List<LandNLegalModel> legalModels = landNLegalService.GetCompanyBaseCaseList(comId);
            int Size_Of_Page = 1500;
            int No_Of_Page = (Page_No ?? 1);
            if (!string.IsNullOrEmpty(searchText))
            {
                var legaldata = legalModels.Where(
                    x => x.CaseNo.Contains(searchText)
                || x.ResponsibleLayer.Contains(searchText)
                || x.DefendantName.Contains(searchText)
                || x.CompanyName.Contains(searchText)
                || x.PlaintiffAppellant.Contains(searchText));
                return View(legaldata.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(legalModels.ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        // GET: LandNLegals
        
        [HttpGet]
        public ActionResult CompanyWiseCaseList(int? Page_No, string CaseNo, string defendantName, string closedCaseStatus,
            string ResponsibleLawyer, string BankName, string Company_Name, string Case_Type,
            string CaseStatus, string CourtName, string Division, string District, string ThanaUpazila, int? companyId)
        {
            ViewBag.Companies = companyService.GetFilterCompanySelectModels();
            ViewBag.CaseTypes = dropDownItemService.GetDropDownItemSelectModels(28);
            ViewBag.CaseStatuss = dropDownItemService.GetDropDownItemSelectModels(46);
            ViewBag.CourtNames = dropDownItemService.GetDropDownItemSelectModels(49);
            ViewBag.ResponsiblePersons = landNLegalService.GetLandNLegalEmployees();
            ViewBag.Divisions = districtService.GetDivisionSelectModels();
            ViewBag.Districts = districtService.GetDistrictSelectModels();
            ViewBag.Upzillas = upazilaService.GetUpzilaSelectModels();
            ViewBag.Banks = bankService.GetBankSelectModels();


            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }

            int comId = (int)Session["CompanyId"];

            if (string.IsNullOrEmpty(closedCaseStatus))
            {
                closedCaseStatus = "";
            }
            //List<LandNLegalModel> _LandNLegal = db.Database.SqlQuery<LandNLegalModel>("LnL_GetClosedCase {0}", closedCaseStatus).ToList();
            List<LandNLegalModel> _LandNLegal = landNLegalService.GetCompanyBaseCaseList(comId);


            if (!string.IsNullOrWhiteSpace(ResponsibleLawyer))
                _LandNLegal = _LandNLegal.Where(m => m.ResponsibleLayer == ResponsibleLawyer).ToList();

            if (!string.IsNullOrWhiteSpace(BankName))
            {
                _LandNLegal = _LandNLegal.Where(m => m.BankName == BankName).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Company_Name))
            {
                _LandNLegal = _LandNLegal.Where(m => m.CompanyName == Company_Name).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Case_Type))
                _LandNLegal = _LandNLegal.Where(m => m.CaseType == Case_Type).ToList();

            if (!string.IsNullOrWhiteSpace(CaseStatus))
                _LandNLegal = _LandNLegal.Where(m => m.CaseStatus == CaseStatus).ToList();

            if (!string.IsNullOrWhiteSpace(CourtName))
                _LandNLegal = _LandNLegal.Where(m => m.CourtName == CourtName).ToList();

            if (!string.IsNullOrWhiteSpace(Division))
                _LandNLegal = _LandNLegal.Where(m => m.Division == Division).ToList();
            if (!string.IsNullOrWhiteSpace(District))
                _LandNLegal = _LandNLegal.Where(m => m.District == District).ToList();

            if (!string.IsNullOrWhiteSpace(ThanaUpazila))
                _LandNLegal = _LandNLegal.Where(m => m.ThanaUpazila == ThanaUpazila).ToList();

            if (!string.IsNullOrEmpty(CaseNo))
                _LandNLegal = db.Database.SqlQuery<LandNLegalModel>("Select * from LandNLegal where CaseStatus not In ('Judgement Completed','Withdrawn','Kharij') and CaseNo like '%" + CaseNo + "%'").ToList();
            //_LandNLegal = _LandNLegal.Where(m => m.CaseNo.Contains("" + CaseNo + "")).ToList();

            if (!string.IsNullOrWhiteSpace(defendantName))
                // _LandNLegal = _LandNLegal.Where(m => m.DefendantName.Contains(defendantName)).ToList();
                _LandNLegal = db.Database.SqlQuery<LandNLegalModel>("Select * from LandNLegal where CaseStatus not In ('Judgement Completed','Withdrawn','Kharij') and DefendantName like '%" + defendantName + "%'").ToList();
            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            return View(_LandNLegal.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        
        [HttpGet]
        public ActionResult CompanyWiseCaseDetails(long id)
        {
            LandNLegalViewModel vm = new LandNLegalViewModel();

            vm._LandNLegal = landNLegalService.GetLandNLegal(id);
            vm.Comments = db.CaseComments.Where(x => x.CaseId == id).ToList();
            //vm.Attaches = db.CaseAttachments.Where(x => x.CaseId == id).ToList();
            vm.Histories = db.CaseHistories.Where(x => x.CaseId == id).ToList();
            vm.Companies = companyService.GetCompanySelectModels();
            vm.CaseTypes = dropDownItemService.GetDropDownItemSelectModels(28);
            vm.CaseStatuss = dropDownItemService.GetDropDownItemSelectModels(46);
            vm.CourtNames = dropDownItemService.GetDropDownItemSelectModels(49);
            //vm.AdditionalDistrictCourts = dropDownItemService.GetDropDownItemSelectModels(50);//Canceled by Zakir Sir
            vm.ResponsiblePersons = landNLegalService.GetLandNLegalEmployees();
            vm.Divisions = districtService.GetDivisionSelectModels();
            vm.Banks = bankService.GetBankSelectModels();//Added by Ashraf Ref:Zakir Sir 20200616
            if (vm._LandNLegal.OID > 0)
            {
                vm.Districts = districtService.GetDistrictByDivisionName(vm._LandNLegal.Division);
                vm.Upazilas = upazilaService.GetUpzilaByDistrictName(vm._LandNLegal.District);
            }
            else
            {
                vm.Districts = new List<SelectModel>();
                vm.Upazilas = new List<SelectModel>();
            }

            ViewBag.cHistory = GetCaseHistory(id);
            return View(vm);
        }


        
        [HttpGet]
        public ActionResult UpcomingCaseSchedule(int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            List<LandNLegalModel> legalModels = landNLegalService.GetLandNLegalEvent();
            int Size_Of_Page = 15;
            int No_Of_Page = (Page_No ?? 1);
            if (!string.IsNullOrEmpty(searchText))
            {
                var legaldata = legalModels.Where(
                    x => x.CaseNo.Contains(searchText)
                || x.ResponsibleLayer.Contains(searchText)
                || x.DefendantName.Contains(searchText)
                || x.CompanyName.Contains(searchText)
                || x.PlaintiffAppellant.Contains(searchText));
                return View(legaldata.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(legalModels.ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult Previous7DaysCaseSchedule(int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            List<LandNLegalModel> legalModels = landNLegalService.GetPrevious7DaysCaseSchedule();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            if (!string.IsNullOrEmpty(searchText))
            {
                var legaldata = legalModels.Where(
                    x => x.CaseNo.Contains(searchText)
                || x.ResponsibleLayer.Contains(searchText)
                || x.DefendantName.Contains(searchText)
                || x.CompanyName.Contains(searchText)
                || x.PlaintiffAppellant.Contains(searchText));
                return View(legaldata.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(legalModels.ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult CreateOrEdit2(long id)
        {
            LandNLegalViewModel vm = new LandNLegalViewModel();

            vm._LandNLegal = landNLegalService.GetLandNLegal(id);
            vm.Comments = db.CaseComments.Where(x => x.CaseId == id).ToList();
            //vm.Attaches = db.CaseAttachments.Where(x => x.CaseId == id).ToList();
            vm.Histories = db.CaseHistories.Where(x => x.CaseId == id).ToList();
            vm.Companies = companyService.GetCompanySelectModels();
            vm.CaseTypes = dropDownItemService.GetDropDownItemSelectModels(28);
            vm.CaseStatuss = dropDownItemService.GetDropDownItemSelectModels(46);
            vm.CourtNames = dropDownItemService.GetDropDownItemSelectModels(49);
            //vm.AdditionalDistrictCourts = dropDownItemService.GetDropDownItemSelectModels(50);//Canceled by Zakir Sir
            vm.ResponsiblePersons = landNLegalService.GetLandNLegalEmployees();
            vm.Divisions = districtService.GetDivisionSelectModels();
            vm.Banks = bankService.GetBankSelectModels();//Added by Ashraf Ref:Zakir Sir 20200616
            if (vm._LandNLegal.OID > 0)
            {
                vm.Districts = districtService.GetDistrictByDivisionName(vm._LandNLegal.Division);
                vm.Upazilas = upazilaService.GetUpzilaByDistrictName(vm._LandNLegal.District);
            }
            else
            {
                vm.Districts = new List<SelectModel>();
                vm.Upazilas = new List<SelectModel>();
            }

            ViewBag.cHistory = GetCaseHistory(id);
            return View(vm);
        }
        // POST: LandNLegals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult CreateOrEdit2(LandNLegalViewModel vm)
        {
            string redirectPage = string.Empty;
            LandNLegalModel model = vm._LandNLegal;
            //if (model._LandNLegal.OID <= 0)
            //{
            //    try
            //    {
            //        bool exist = false;
            //        exist = db.LandNLegals.Where(x => x.CaseNo == model._LandNLegal.CaseNo).Any();
            //        if (exist)
            //        {
            //            TempData["errMessage"] = "Exists";
            //            return RedirectToAction("CreateOrEdit2");
            //        }
            //        else
            //        {
            //            landNLegalService.SaveLandNLegal(0, model._LandNLegal);
            //        }
            //        redirectPage = "Index";
            //    }
            //    catch (Exception ex)
            //    {
            //        logger.Error(ex);
            //    }
            //}
            //else
            //{
            try
            {
                //LandNLegal landNLegal = db.LandNLegals.FirstOrDefault(x => x.OID == model.OID);
                //if (landNLegal == null)
                //{
                //    TempData["errMessage1"] = "Data not found!";
                //    return RedirectToAction("CreateOrEdit2");
                //}

                bool result = landNLegalService.SaveLandNLegal(model.OID, model);

                #region File Upload
                if (result == true && model.OID > 0)
                {
                    Int64 caseid = model.OID;
                    string caseId = "";
                    var caseData = db.LandNLegals.Where(x => x.OID == caseid).FirstOrDefault();
                    if (caseData != null)
                    {
                        caseId = caseData.OID.ToString();
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var file = Request.Files[i];
                            if (file != null && file.ContentLength > 0)
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                FileAttachment fileDetail = new FileAttachment()
                                {
                                    AttachFileName = caseData.OID + "_" + fileName,
                                    CompanyId = 227,
                                    CaseId = caseid,
                                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                    CreatedDate = DateTime.Now,
                                    ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name,
                                    ModifiedDate = DateTime.Now
                                };

                                string folder = Server.MapPath(string.Format("~/{0}/{1}/", "KGFiles", "KGCase"));
                                if (!Directory.Exists(folder))
                                {
                                    Directory.CreateDirectory(folder);
                                    var path1 = Path.Combine(Server.MapPath(string.Format("~/{0}/{1}/", "KGFiles", "KGCase")), caseId + "_" + fileName);
                                    file.SaveAs(path1);
                                }
                                else
                                {
                                    var path1 = Path.Combine(Server.MapPath(string.Format("~/{0}/{1}/", "KGFiles", "KGCase")), caseId + "_" + fileName);
                                    file.SaveAs(path1);
                                }

                                db.Entry(fileDetail).State = EntityState.Added;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                #endregion

                TempData["DataUpdate"] = "Data Save Successfully!";
                redirectPage = "CreateOrEdit2";
                ViewBag.cHistory = GetCaseHistory(model.OID);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            //}
            return RedirectToAction(redirectPage);
        }

        public FileResult Download(String fileName, String deedNo)
        {
             return File(Path.Combine(Server.MapPath("~/KGFiles/KGCase/" ), fileName), System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public JsonResult DeleteFile(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            try
            {
                int guid = Convert.ToInt32(id);
                var path1 = string.Empty;
                FileAttachment fileDetail = db.FileAttachments.Find(guid);
                if (fileDetail == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Result = "Error" });
                }
                else
                {
                    LandNLegal aseet = db.LandNLegals.Find(fileDetail.AttachmentId);
                    path1 = Path.Combine(Server.MapPath(string.Format("~/{0}/{1}/", "KGFiles", "KGCase")), fileDetail.AttachFileName);
                }

                //Remove from database
                db.FileAttachments.Remove(fileDetail);
                db.SaveChanges();

                //Delete file from the file system 
                if (System.IO.File.Exists(path1))
                {
                    System.IO.File.Delete(path1);
                }
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        //Fetching District Data base on Division Name-Ashraf
        [HttpPost]
        public ActionResult GetDistrictByDivisionName(string name)
        {
            List<SelectModel> Districts = districtService.GetDistrictByDivisionName(name);
            return Json(Districts, JsonRequestBehavior.AllowGet);
        }

        //Exmpale not use in this code-Ashraf
        public void BindDivision()
        {
            var division = db.Divisions.ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Select State--", Value = "0" });

            foreach (var m in division)
            {
                li.Add(new SelectListItem { Text = m.Name, Value = m.DivisionId.ToString() });
                ViewBag.division = li;

            }
        }

        //Fetching Upzilla Data base on District Name-Ashraf
        [HttpPost]
        public ActionResult GetUpzilaByDistrictName(string name)
        {
            List<SelectModel> Upazilas = upazilaService.GetUpzilaByDistrictName(name);
            return Json(Upazilas, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult LandNLegalFileUpload(LandNLegalModel file)
        {
            string errorMessage = string.Empty;
            if (file.AttachFile != null && file.AttachFile.ContentLength > 0)
            {
                bool isFileSizeValidated = true;
                isFileSizeValidated = FileUtil.ValidateCaseFileSize(file.AttachFile.ContentLength);
                if (isFileSizeValidated)
                {
                    string path = LibUtil.GetUploadPathIssueDocument(file.OID.ToString());
                    if (LibUtil.FileExists(path + file.AttachFile.FileName))
                    {
                        errorMessage = Constants.NOTIFICATION_ERROR_SAME_FILE_EXIST;
                    }
                    else
                    {
                        //LandNLegal landNLegal = db.LandNLegals.Find(id);
                        FileAttachment ca = new FileAttachment
                        {
                            CaseId = file.OID,
                            AttachFileName = file.AttachFile.FileName,
                            CreatedBy = System.Web.HttpContext.Current.User.Identity.Name
                        };
                        db.Entry(ca);
                        int result = db.SaveChanges();
                        if (result > 0)
                        {
                            try
                            {
                                if (LibUtil.CreateDirectory(path))
                                {
                                    file.AttachFile.SaveAs(path + file.AttachFile.FileName);
                                    CaseAttachmentDataBind(System.Web.HttpContext.Current.User.Identity.Name);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                                errorMessage = Constants.NOTIFICATION_ERROR_FILE_SAVING;
                            }
                        }

                        //var fileName = Path.GetFileName(file.AttachFile.FileName);
                        //var path = Path.Combine(Server.MapPath("~/FileUpload"), fileName);
                        //file.AttachFile.SaveAs(path);
                        //bool status = false;
                        //string fileName1 = Path.GetPathRoot(file.AttachFile.FileName);
                        //string extention = Path.GetExtension(file.AttachFile.FileName).ToLower();
                        //if (extention == ".txt" || extention == ".xls")
                        //{
                        //    //status = InsetData(path.ToString());
                        //}
                        //if (status == true)
                        //{
                        //    ViewBag.status = status;
                        //    if ((System.IO.File.Exists(path.ToString())))
                        //    {
                        //        System.IO.File.Delete(path.ToString());
                        //    }
                        //}
                    }
                }
                else
                {
                    //File size is to big
                }
            }
            return View();
        }

        private void CaseAttachmentDataBind(string employeeId)
        {
            if (employeeId is null)
            {
                throw new ArgumentNullException(nameof(employeeId));
            }
        }

        public string GetCaseHistory(long id)
        {
            string htmlStr = "<table width='100%' align='center' cellpadding='2' cellspacing='2' border='0' bgcolor='#F0F0F0'> ";
            DataTable dt = new DataTable();
            dt = GetCaseHistoryByCaseId(id);
            string style = "\"border-bottom:1pt solid #F3F3F3;background-color: #F5F5F5;\"";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["Name"].ToString();
                dt.Rows[i]["Date"].ToString();
                dt.Rows[i]["Remarks"].ToString();
                dt.Rows[i]["Flag"].ToString();
                string NrOfDays = "";
                DateTime d1 = DateTime.Now;
                DateTime dtstart = Convert.ToDateTime(dt.Rows[i]["Date"]);

                // DateTime dtstart = Convert.ToDateTime(dr["EndDate"]);
                //string[] strDateTime = dtstart.ToString("dd-MM-yyyy").Split('-');
                string strDateTime2 = dtstart.ToString("dd-MM-yyyy");

                // displaying values in textboxes
                //string txtDate = strDateTime[0];
                //string txtMonth = strDateTime[1];
                //string txtYear = strDateTime[2];

                DateTime d2 = Convert.ToDateTime(dt.Rows[i]["Date"].ToString());
                TimeSpan t = d1 - d2;

                if (t.Days > 0)
                {
                    int days = t.Days;
                    NrOfDays = days + " Days".ToString();
                }
                else if (t.Hours > 0)
                {
                    int Hour = t.Hours;
                    NrOfDays = Hour + " Hour".ToString();
                }
                else if (t.Minutes > 0)
                {
                    int Minute = t.Minutes;
                    NrOfDays = Minute + " Minutes".ToString();
                }
                else if (t.Seconds > 0)
                {
                    int Second = t.Seconds;
                    NrOfDays = Second + " Second".ToString();
                }
                if (dt.Rows[i]["Flag"].ToString() == "Attachment Added")
                {
                    htmlStr += "<tr  style=" + style + "><td> <span title=" + strDateTime2 + ">Changed " + NrOfDays + " ago by</span> <b> " + dt.Rows[i]["Name"].ToString() + "</b></td></tr>";
                    htmlStr += "<tr><td><b>" + dt.Rows[i]["Flag"].ToString() + "</b>: " + dt.Rows[i]["Remarks"].ToString() + " </td></tr>";
                }
                else
                {
                    htmlStr += "<tr style=" + style + "><td> <span title=" + strDateTime2 + ">Changed " + NrOfDays + " ago by</span> <b> " + dt.Rows[i]["Name"].ToString() + "</b></td></tr>";
                    htmlStr += "<tr><td><b>" + dt.Rows[i]["Flag"].ToString() + "</b>: <BR>" + dt.Rows[i]["Remarks"].ToString() + " </td></tr>";
                }
            }

            return htmlStr += "</table>";
        }

        private DataTable GetCaseHistoryByCaseId(long caseId)
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("GetCaseDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CaseId", caseId);
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            return dt;
        }
        
        [HttpGet]
        public ActionResult ExportKGCaseToExcel(string CaseNo, string closedCaseStatus, string defendantName, string ResponsibleLawyer, string BankName, string CompanyName, string CaseType,
              string CaseStatus, string CourtName, string Division, string District, string ThanaUpazila)
        {
            var gv = new GridView();
            DataTable dt = new DataTable();
            DataTable _newDataTable = new DataTable();
            if (!string.IsNullOrEmpty(CaseNo) || !string.IsNullOrEmpty(defendantName) || !string.IsNullOrEmpty(ResponsibleLawyer) || !string.IsNullOrEmpty(BankName) || !string.IsNullOrEmpty(CompanyName) || !string.IsNullOrEmpty(CaseStatus) || !string.IsNullOrEmpty(CaseType)
                || !string.IsNullOrEmpty(CaseStatus) || !string.IsNullOrEmpty(CourtName) || !string.IsNullOrEmpty(Division) || !string.IsNullOrEmpty(District) || !string.IsNullOrEmpty(ThanaUpazila))
            {
                dt = GetFilterCaseList(CaseNo, defendantName, ResponsibleLawyer, BankName, CompanyName, CaseType,
               CaseStatus, CourtName, Division, District, ThanaUpazila, closedCaseStatus);
                //dt = GetCaseList(searchText);
            }
            else
            {
                dt = GetFilterCaseList(closedCaseStatus);
            }
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    string[] selectedColumns = new[] { "CompanyName", "CaseNo", "CaseType", "CourtName", "CaseStatus", "PlaintiffAppellant", "ResponsibleLayer", "District", "BankName", "ThanaUpazila", "Amount", "FilingDate", "PreviousDate1", "NextDate" };
                    DataTable dt1 = new DataView(dt).ToTable(false, selectedColumns);

                    gv.DataSource = dt1;
                    gv.DataBind();

                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename= KG_CaseList.xls");
                    Response.ContentType = "application/ms-excel";

                    Response.Charset = "";
                    StringWriter objStringWriter = new StringWriter();
                    HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

                    gv.RenderControl(objHtmlTextWriter);
                    Response.Output.Write(objStringWriter.ToString());
                    Response.Flush();
                    Response.End();
                    return View();
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public DataTable GetFilterCaseList(string CaseNo, string defendantName, string ResponsibleLayer, string BankName, string CompanyName, string CaseType,
            string CaseStatus, string CourtName, string Division, string District, string ThanaUpazila, string closedCaseStatus)
        {
            try
            {
                DataTable dt = new DataTable();
                List<LandNLegalModel> _LandNLegal = db.Database.SqlQuery<LandNLegalModel>("LnL_GetClosedCase {0}", closedCaseStatus).ToList();

                if (!string.IsNullOrWhiteSpace(ResponsibleLayer))
                    _LandNLegal = _LandNLegal.Where(m => m.ResponsibleLayer == ResponsibleLayer).ToList();

                if (!string.IsNullOrWhiteSpace(BankName))
                {
                    _LandNLegal = _LandNLegal.Where(m => m.BankName == BankName).ToList();
                }

                if (!string.IsNullOrWhiteSpace(CompanyName))
                {
                    _LandNLegal = _LandNLegal.Where(m => m.CompanyName == CompanyName).ToList();
                }

                if (!string.IsNullOrWhiteSpace(CaseType))
                    _LandNLegal = _LandNLegal.Where(m => m.CaseType == CaseType).ToList();

                if (!string.IsNullOrWhiteSpace(CaseStatus))
                    _LandNLegal = _LandNLegal.Where(m => m.CaseStatus == CaseStatus).ToList();

                if (!string.IsNullOrWhiteSpace(CourtName))
                    _LandNLegal = _LandNLegal.Where(m => m.CourtName == CourtName).ToList();

                if (!string.IsNullOrWhiteSpace(Division))
                    _LandNLegal = _LandNLegal.Where(m => m.Division == Division).ToList();
                if (!string.IsNullOrWhiteSpace(District))
                    _LandNLegal = _LandNLegal.Where(m => m.District == District).ToList();

                if (!string.IsNullOrWhiteSpace(ThanaUpazila))
                    _LandNLegal = _LandNLegal.Where(m => m.ThanaUpazila == ThanaUpazila).ToList();

                if (!string.IsNullOrEmpty(CaseNo))
                    _LandNLegal = db.Database.SqlQuery<LandNLegalModel>("Select * from LandNLegal where CaseStatus not In ('Judgement Completed','Withdrawn','Kharij') and CaseNo like '%" + CaseNo + "%'").ToList();
                //_LandNLegal = _LandNLegal.Where(m => m.CaseNo.Contains("" + CaseNo + "")).ToList();

                if (!string.IsNullOrWhiteSpace(defendantName))
                    // _LandNLegal = _LandNLegal.Where(m => m.DefendantName.Contains(defendantName)).ToList();
                    _LandNLegal = db.Database.SqlQuery<LandNLegalModel>("Select * from LandNLegal where CaseStatus not In ('Judgement Completed','Withdrawn','Kharij') and DefendantName like '%" + defendantName + "%'").ToList();

                if (_LandNLegal != null)
                {
                    dt = CreateDataTable(_LandNLegal);
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public DataTable GetFilterCaseList(string closedCaseStatus)
        {
            try
            {
                DataTable dt = new DataTable();
                List<LandNLegalModel> closedCase = db.Database.SqlQuery<LandNLegalModel>("LnL_GetClosedCase {0}", closedCaseStatus).ToList();
                if (closedCase != null)
                {
                    dt = CreateDataTable(closedCase);
                }

                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        private DataTable CreateDataTable(IList<LandNLegalModel> item)
        {
            //.Select(p => new {p.Id, p.Title});
            //IList<LandNLegalModel> item1 = item.Select(p => new { CaseType=p.CaseType, CourtName=p.CourtName, CourtNamep.CourtName });
            Type type = typeof(LandNLegalModel);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (LandNLegalModel entity in item)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        public DataTable GetCaseList(string serachText)
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("select * from vGetCaseList where ( [CaseNo] like  '%" + serachText + "%' or  [CaseType] like  '%" + serachText + "%' or [ResponsibleLayer] like  '%" + serachText + "%' or [CaseStatus] like  '%" + serachText + "%')"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            con.Open();
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            con.Close();
                        }
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public DataTable GetCaseList()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("select * from vGetCaseList"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            con.Open();
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            con.Close();
                        }
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        
        public IList<LandNLegalModel> GetKGCaseList(string searchText)
        {
            List<LandNLegalModel> landNLegal = null;

            // KG1128	Advocate Elias Khan
            if (System.Web.HttpContext.Current.User.Identity.Name == "KG1128")
            {
                return landNLegal.Where(x => x.ResponsibleLayer == "Advocate Elias Khan").ToList();
            }//Advocate Md. Burhan Uddin
            else if (System.Web.HttpContext.Current.User.Identity.Name == "KG0864")
            {
                return landNLegal.Where(x => x.ResponsibleLayer == "Advocate Md. Burhan Uddin").ToList();
            }// KG1005 Advocate Md. Tayab Anwar and Advocate Forhad KG3307
            else if (System.Web.HttpContext.Current.User.Identity.Name == "KG1005" || System.Web.HttpContext.Current.User.Identity.Name == "KG3307")
            {
                return landNLegal.Where(x => x.ResponsibleLayer == "Advocate Md. Tayab Anwar").ToList();
            }
            else
            {
                return landNLegal.ToList();
            }
        }

        #region // Dashboard
        public class DisplayReport
        {
            public string Name
            {
                get;
                set;
            }
            public int Count
            {
                get;
                set;
            }
        }
        public ActionResult LnLDDashboard()
        {
            DataTable dt = GetClientStatus();
            if (dt.Rows.Count > 0)
            {
                ViewData.Model = dt.AsEnumerable();
            }

            DataTable dt2 = GetClientStatus();
            if (dt2.Rows.Count > 0)
            {
                TempData["data"] = dt2.AsEnumerable();
            }

            var query = db.KttlCustomers
               .GroupBy(p => p.ClientStatus)
               .Select(g => new DisplayReport { Name = g.Key, Count = g.Count() })
               .OrderByDescending(g => g.Count);
            ViewBag.ServiceYear = query;


            return View();
        }

        
        public ActionResult GetResponsiblePersonChartImage()
        {
            DataTable dt = GetResponsiblePerson();
            var key = (dynamic)null;
            if (dt.Rows.Count > 0)
            {
                List<string> gender = (from p in dt.AsEnumerable() select p.Field<string>("ResponsiblePerson")).Distinct().ToList();
                int countGender = gender.Count; //countGender

                List<string> gender2 = new List<string>();
                List<double> genderCount = new List<double>();

                foreach (DataRow row in dt.Rows)
                {
                    gender2.Add((string)Convert.ToString(row["ResponsiblePerson"]));
                }
                string[] outputgender2 = gender2.ToArray();

                foreach (DataRow row in dt.Rows)
                {
                    genderCount.Add((double)Convert.ToDouble(row["Total"]));
                }
                double[] outputgenderCount = genderCount.ToArray();
                if (gender.Count > 0)
                {

                    key = new Chart(width: 360, height: 360)
                       .AddTitle("Responsible Officer's Client")
                       .AddSeries(
                       chartType: "Bar",
                       name: "Client",
                       xValue: outputgender2,
                       yValues: outputgenderCount);
                }
            }
            return File(key.ToWebImage().GetBytes(), "image/jpeg");

        }

        
        public ActionResult GetServicesChartImage()
        {
            DataTable dt = GetServices();
            var key = (dynamic)null;
            if (dt.Rows.Count > 0)
            {
                List<string> gender = (from p in dt.AsEnumerable() select p.Field<string>("Services")).Distinct().ToList();
                int countGender = gender.Count; //countGender

                List<string> gender2 = new List<string>();
                List<double> genderCount = new List<double>();

                foreach (DataRow row in dt.Rows)
                {
                    gender2.Add((string)Convert.ToString(row["Services"]));
                }
                string[] outputgender2 = gender2.ToArray();

                foreach (DataRow row in dt.Rows)
                {
                    genderCount.Add((double)Convert.ToDouble(row["Total"]));
                }
                double[] outputgenderCount = genderCount.ToArray();
                if (gender.Count > 0)
                {
                    key = new Chart(width: 360, height: 360)
                       .AddTitle("Client Services Ratio")
                       .AddSeries(
                       chartType: "Bubble",
                       name: "Client Services",
                       xValue: outputgender2,
                       yValues: outputgenderCount);
                }
            }
            return File(key.ToWebImage().GetBytes(), "image/jpeg");
        }

        public DataTable GetResponsiblePerson()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("Select  ResponsiblePerson, Count(*) As Total from KttlCustomer Group by ResponsiblePerson"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            con.Open();
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            con.Close();
                        }
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public DataTable GetClientStatus()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("Select  ClientStatus, Count(*) As Total from KttlCustomer Group by ClientStatus"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            con.Open();
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            con.Close();
                        }
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public DataTable GetServices()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("Select  Services, Count(*) As Total from KttlCustomer Group by Services "))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            con.Open();
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            sda.Fill(dt);
                            con.Close();
                        }
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        #endregion
    }
}
