using ClosedXML.Excel;
using KGERP.Data.CustomModel;
using KGERP.Service.Interface;
using KGERP.Utility.Util;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace KGERP.Controllers.Crm
{
    //[CheckSession]
    public class CrmsController : BaseController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ICrmService _service;
        private readonly IPermissionHandler _permissionService;

        public CrmsController(ICrmService service, IPermissionHandler permissionService)
        {
            _service = service;
            _permissionService = permissionService;
        }


        [HttpGet]
        public async Task<ActionResult> Index(int companyId)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            var model = new CrmViewModel();
            model = await _service.GetIndex(companyId, uId);
            return View(model);
        }



        public PartialViewResult NavCrmPartial()
        {
            var menuVm = new MainMenuListVm();
            string companyIdParam = Request.Params["companyId"];
            int companyId;

            if (string.IsNullOrEmpty(companyIdParam))
            {
                throw new ArgumentException("The companyId parameter is missing.");
            }
            else
            {
                string firstValue = companyIdParam.Split(',')[0].Trim();

                if (!int.TryParse(firstValue, out companyId))
                {
                    throw new ArgumentException("The first value of the companyId parameter is not a valid integer.");
                }
                else
                {
                    // companyId is successfully parsed
                }
            }


            //int companyId = Convert.ToInt32(Request.Params["companyId"]);
            menuVm = _service.GetAllMenu(companyId);
            if (System.Web.HttpContext.Current.User.Identity.Name == null)
            {
                return PartialView(RedirectToAction("Login"));
            }
            return PartialView("_NavCrmPartial", menuVm);
        }


        [HttpPost]
        public async Task<JsonResult> MakeSchedule(CrmScheduleVm model)
        {
            model = await _service.MakeSchedule(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> SwitchResponsibleOffice(int clientId, int companyId)
        {

            var model = await _service.SwitchResponsibleOffice(clientId, companyId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> GetClientDetailsById(int clientId, int companyId)
        {

            var model = new CrmScheduleListVm();
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            if (!_permissionService.HasPermission(uId, PermissionCollection.Crms.Client.CanClientDetailsView, companyId))
            {
                model.HasMessage = true;
                model.MessageList.Add("You have no permission to details view");
                return View(model);
            }

            if (_service.IsLeader(uId, companyId))
            {
                uId = 0;
            }

            model = await _service.GetScheduleByClientId(clientId, uId, companyId);

            return View(model);

        }
        [HttpPost]
        public async Task<JsonResult> GetAllClient(int companyId)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            companyId = companyId == 0 ? Convert.ToInt32(Request.Params["companyId"]) : companyId;

            //var draw = Convert.ToInt32(Request.Form["draw"].FirstOrDefault());
            //var start = Request.Form["start"].FirstOrDefault();
            //var length = Request.Form["length"].FirstOrDefault();
            //var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault().ToString(); 
            //var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault().ToString();
            //var searchValue = Request.Form["search[value]"].FirstOrDefault().ToString();
            //int pageSize = length != null ? Convert.ToInt32(length) : 10;
            //int skip = start != null ? Convert.ToInt32(start) : 0;


            var data = await _service.GetAllClient(companyId, uId);
            // var data = await _service.GetAllClient(7);
            var result = data.DataList.ToList();

            //  int filteredResultsCount = result.Count > 0 ? result[0].TotalRows : 0;


            // int totalResultsCount = result.Count > 0 ? result[0].TotalRows : 0;
            int totalRows = result.Count;

            if (!string.IsNullOrEmpty(searchValue))
            {
                result = result.Where(x => x.Name.Contains(searchValue)
                || x.MobileNo.StartsWith(searchValue)
                                          ).ToList();
            }

            int totalRowsAfterFiltering = result.Count;


            //sorting
            //if (!string.IsNullOrEmpty(sortColumnName) && !string.IsNullOrEmpty(sortDirection))
            //{
            //    if (sortDirection.Trim().ToLower() == "asc")
            //    {
            //        empList = SortHelper.OrderBy<EmployeeModel>(empList, sortColumnName);
            //    }
            //    else
            //    {
            //        empList = SortHelper.OrderByDescending<EmployeeModel>(empList, sortColumnName);
            //    }
            //}
            // result = result.OrderBy(sortColumnName + " " + sortDirection).ToList<CrmVm>();

            //paging
            result = result.Skip(start).Take(length).ToList();
            return Json(new { data = result, draw = Request["draw"], recordsTotal = totalRows, recordsFiltered = totalRowsAfterFiltering }, JsonRequestBehavior.AllowGet);


            //  return Json(new { draw = draw, recordsTotal = totalResultsCount, recordsFiltered = filteredResultsCount, data = result }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> AllClientList(int companyId)
        {
            var model = new CrmListVm();
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);

            if (!_permissionService.HasPermission(uId, PermissionCollection.Crms.Client.CanViewAllClient, companyId))
            {
                model.HasMessage = true;
                model.MessageList.Add("You have no permission");

                return View(model);
            }

            if (!_service.IsLeader(uId, companyId))
            {
                return RedirectToAction(nameof(UserClientList), "Crms", new { companyId = companyId });
            }

            model.CompanyId = companyId;

            return View(model);


        }

        [HttpGet]
        public async Task<ActionResult> UserClientList(int companyId, int pageNumber = 0, int pagesize = 100, string searchText = "")
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            var model = new CrmListVm();
            model = await _service.GetUserClient(companyId, uId, pageNumber, pagesize, searchText);

            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> ClientUpload(int companyId)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            var model = new CrmUploadListVm();
            // model = await _service.GetAllCrmUpload(companyId);
            model.GenderList = await _service.GetDropdownGender();
            model.ReligionList = await _service.GetDropdownReligion();
            model.DealingOfficerList = await _service.GetDropdownDealingOfficerForLead(companyId, uId);
            model.ProjectList = await _service.GetDropdownProject(companyId);
            model.ServiceStatusList = await _service.GetDropdownServiceStatus(companyId);
            model.TypeofInterestList = await _service.GetDropdownTypeofInterest();
            model.SourceofMediaList = await _service.GetDropdownSourceofMedia(companyId);
            model.PromotionalOfferList = await _service.GetDropdownPromotionalOffer(companyId);
            model.ChoiceofAreaList = await _service.GetDropdownChoiceofArea(companyId);
            model.UploadDatetimeList = await _service.GetUploaddate(companyId);
            if (model.UploadDatetimeList.Count > 0)
            {
                model.lastUploadSerialNo = model.UploadDatetimeList.FirstOrDefault().Id;
            }

            model.CompanyId = companyId;
            model.IsLeader = _service.IsLeader(uId, companyId);
            model.ManagerId = _service.Manager(uId, companyId);


            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> ClientUpload(CrmUploadListVm model, int pageNumber = 0, int pagesize = 100, string searchText = "")
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            int uuid = uId;
            //if(model.CompanyId == 0)
            //{
            //    var CompanyId = HttpContext.Session["CompanyId"];
            //    model.CompanyId = CompanyId;
            //}
            //if (_service.IsLeader(uId, model.CompanyId)) uId = 0;

            var obj = await _service.FilteringClientlist(model, uuid, pageNumber, pagesize, searchText);
            obj.GenderList = await _service.GetDropdownGender();
            obj.ReligionList = await _service.GetDropdownReligion();
            obj.DealingOfficerList = await _service.GetDropdownDealingOfficerForLead(model.CompanyId, uuid);
            obj.ProjectList = await _service.GetDropdownProject(model.CompanyId);
            obj.ServiceStatusList = await _service.GetDropdownServiceStatus(model.CompanyId);
            obj.TypeofInterestList = await _service.GetDropdownTypeofInterest();
            obj.SourceofMediaList = await _service.GetDropdownSourceofMedia(model.CompanyId);
            obj.PromotionalOfferList = await _service.GetDropdownPromotionalOffer(model.CompanyId);
            obj.ChoiceofAreaList = await _service.GetDropdownChoiceofArea(model.CompanyId);

            obj.CompanyId = model.CompanyId;
            //obj.ResponsibleOfficerId = uId;
            obj.IsLeader = _service.IsLeader(uuid, model.CompanyId);
            obj.ManagerId = _service.Manager(uuid, model.CompanyId);

            return View(obj);
        }
        [HttpGet]
        public async Task<ActionResult> AddNewClient(int companyId, bool? hasError, List<string> messageList)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            var model = new CrmVm();

            if (!_permissionService.HasPermission(uId, PermissionCollection.Crms.Client.CanAddNewClient, companyId))
            {
                model.HasMessage = true;
                model.MessageList.Add("You have no permission to add new Client");
                return View(model);
            }

            model.GenderList = await _service.GetDropdownGender();
            model.ReligionList = await _service.GetDropdownReligion();
            model.DealingOfficerList = await _service.GetDropdownDealingOfficer(companyId);
            model.ProjectList = await _service.GetDropdownProject(companyId);
            model.ServiceStatusList = await _service.GetDropdownServiceStatus(companyId);
            model.TypeofInterestList = await _service.GetDropdownTypeofInterest();
            model.SourceofMediaList = await _service.GetDropdownSourceofMedia(companyId);
            model.PromotionalOfferList = await _service.GetDropdownPromotionalOffer(companyId);
            model.ChoiceofAreaList = await _service.GetDropdownChoiceofArea(companyId);
            model.CompanyId = companyId;
            model.IsLeader = _service.IsLeader(uId, companyId);
            model.isManagwe = _service.Manager(uId, companyId);


            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> AddNewClient(CrmVm model)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            model.IsLeader = _service.IsLeader(uId, model.CompanyId);
            model.isManagwe = _service.Manager(uId, model.CompanyId);
            model.UserId = uId;
            if (!_permissionService.HasPermission(uId, PermissionCollection.Crms.Client.CanAddNewClient, model.CompanyId))
            {
                model.HasMessage = true;
                model.MessageList.Add("You have no permission to add new Client");
                return View(model);
            }
            if (model.HasMessage)
            {

            }
            model = await _service.SaveClient(model);
            if (model == null)
            {
                return RedirectToAction(nameof(AddNewClient), "Crms", new { companyId = model.CompanyId });
            }



            return RedirectToAction(nameof(GetClientDetailsById), "Crms", new { clientId = model.ClientId, companyId = model.CompanyId });
        }


        [HttpGet]
        public async Task<ActionResult> UploadClientBatchHistory(int companyId)
        {
            var model = new ClientBatchUplodListVm();
            model = await _service.GetCrmClientBatchUpload(companyId);
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> UploadClientBatch(int companyId)
        {
            var model = new CrmUploadVm();
            model.CompanyId = companyId;

            return View(model);
        }

        //[HttpPost]
        //public async Task<ActionResult> UploadClientBatch(CrmUploadVm model)
        //{
        //    var vm = new CrmUploadVm();
        //    vm.CompanyId = model.CompanyId;
        //    int companyId2 = model.CompanyId;

        //    if (model.ExcelFile != null && model.ExcelFile.ContentLength > 0)
        //    {
        //        OleDbConnection conn = new OleDbConnection();
        //        OleDbCommand cmd = new OleDbCommand();
        //        OleDbDataAdapter da = new OleDbDataAdapter();
        //        DataSet ds = new DataSet();
        //        string connString = "";
        //        string strFileName = DateTime.Now.ToString("ddMMyyyy_HHmmss");
        //        string strFileType = Path.GetExtension(model.ExcelFile.FileName).ToString().ToLower();
        //        var fileName = Path.GetFileName(model.ExcelFile.FileName);
        //        string uploadsFolder = Server.MapPath("~/App_Data/uploads");
        //        if (!Directory.Exists(uploadsFolder))
        //        {
        //            Directory.CreateDirectory(uploadsFolder);
        //        }
        //        var path = Path.Combine(Path.GetTempPath(), fileName);
        //        //var path = Path.Combine(uploadsFolder, fileName);
        //        if (strFileType == ".xls" || strFileType == ".xlsx")
        //        {
        //            try
        //            {
        //                model.ExcelFile.SaveAs(path);
        //            }
        //            catch (Exception ex)
        //            {
        //                // vm.Status = "Failed!!! Cann't read file";
        //                return View(vm);
        //            }
        //        }
        //        else
        //        {
        //            vm.Status = "Failed!!! Invalid File Type";
        //            return View(vm);
        //        }
        //        if (strFileType.Trim() == ".xls")
        //        {

        //            connString = string.Format(ConfigurationManager.ConnectionStrings["Excel07ConString"].ToString(), path);//"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strNewPath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
        //        }
        //        else if (strFileType.Trim() == ".xlsx")
        //        {
        //            connString = string.Format(ConfigurationManager.ConnectionStrings["Excel07ConString"].ToString(), path);//"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strNewPath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //        }
        //        else
        //        {
        //            vm.Status = "Failed!!! Invalid File Type";

        //            return View(vm);
        //        }
        //        try
        //        {
        //            connString = string.Format(connString, path);
        //            OleDbConnection connExcel = new OleDbConnection(connString);
        //            OleDbCommand cmdExcel = new OleDbCommand();
        //            OleDbDataAdapter oda = new OleDbDataAdapter();
        //            DataTable dt = new DataTable();
        //            cmdExcel.Connection = connExcel;
        //            connExcel.Open();
        //            DataTable dtExcelSchema;
        //            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //            string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
        //            cmdExcel.CommandText = "SELECT * From [" + SheetName + "]";
        //            oda.SelectCommand = cmdExcel;
        //            oda.Fill(ds);
        //            dt = ds.Tables[0];
        //            DateTime defaultDate = Convert.ToDateTime("1900-01-01");
        //            List<CrmVm> CustomerList = new List<CrmVm>();

        //            var responsibleOfficerList = await _service.GetDropdownDealingOfficer(companyId2);
        //            var typeOfInterestList = await _service.GetDropdownTypeofInterest();
        //            var sourceOfMediaList = await _service.GetDropdownSourceofMedia(companyId2);
        //            var promotionalOfferList = await _service.GetDropdownPromotionalOffer(companyId2);
        //            var statusList = await _service.GetDropdownServiceStatus(companyId2);
        //            var choiceOfAreaList = await _service.GetDropdownChoiceofArea(companyId2);
        //            var genderList = await _service.GetDropdownGender();
        //            var religonList = await _service.GetDropdownReligion();
        //            var projectList = await _service.GetDropdownProject(companyId2);


        //            foreach (DataRow dr in dt.Rows)
        //            {
        //                if (dr["Full Name"].ToString().Length > 3)
        //                {
        //                    var obj = new CrmVm();
        //                    obj.Name = dr["Full Name"].ToString();
        //                    obj.CompanyId = model.CompanyId;
        //                    obj.DateofBirth = !string.IsNullOrEmpty(dr["Date of Birth"].ToString()) ? Convert.ToDateTime(dr["Date of Birth"].ToString()) : defaultDate;

        //                    obj.DateOfContact = DateTime.Now;

        //                    obj.CampaignText = dr["Campaign Name"].ToString();
        //                    obj.ResponsibleOfficeName = dr["Dealing Officer"].ToString();

        //                    var res = responsibleOfficerList.FirstOrDefault(q => q.Name.Trim() == obj.ResponsibleOfficeName.Trim());
        //                    obj.ResponsibleOfficerId = res == null ? 0 : res.Id;

        //                    obj.PresentAddress = dr["Present Address"].ToString();
        //                    obj.PermanentAddress = dr["Permanent address"].ToString();


        //                    obj.JobTitle = dr["Job Title"].ToString();
        //                    obj.OrganizationText = dr["Organization"].ToString();


        //                    obj.MobileNo = "0" + dr["Mobile"].ToString();
        //                    obj.MobileNo2 = !string.IsNullOrEmpty(dr["Mobile 2"].ToString()) ? "0" + dr["Mobile 2"].ToString() : "";


        //                    obj.Email = dr["Email"].ToString();
        //                    obj.Email2 = dr["Email 2"].ToString();
        //                    obj.SourceofMediaText = dr["Source of Media"].ToString();

        //                    var som = sourceOfMediaList.FirstOrDefault(q => q.Name.ToUpper() ==
        //                    dr["Source of Media"].ToString().ToUpper());
        //                    obj.SourceofMediaId = som == null ? 0 : som.Id;

        //                    //obj.ServicesDescription = dr["Service Detail"].ToString();

        //                    obj.OfferText = dr["Promotional Offer"].ToString();
        //                    var offer = promotionalOfferList
        //                        .FirstOrDefault(q => q.Name.ToUpper() ==
        //                        dr["Promotional Offer"].ToString().ToUpper());
        //                    obj.OfferId = offer == null ? 0 : offer.Id;

        //                    var genderObj = genderList
        //                        .FirstOrDefault(q => q.Name.ToUpper() == dr["Gender"].ToString().ToUpper());
        //                    obj.GenderId = genderObj == null ? 1 : genderObj.Id;

        //                    string gender = dr["Gender"].ToString();
        //                    if (!string.IsNullOrEmpty(gender))
        //                    {
        //                        obj.GenderText = char.ToUpper(gender[0]) + gender.Substring(1);
        //                    }
        //                    obj.GenderText = dr["Gender"].ToString();

        //                    obj.TypeofInterestText = dr["Interested In"].ToString();
        //                    var typeofInterest = typeOfInterestList
        //                        .FirstOrDefault(q => q.Name == dr["Interested In"].ToString());

        //                    obj.TypeofInterestId = typeofInterest == null ? 0 : typeofInterest.Id;

        //                    obj.StatusText = !string.IsNullOrEmpty(dr["Service Status"].ToString()) ? dr["Service Status"].ToString() : "New";
        //                    var stastus = statusList
        //                        .FirstOrDefault(q => q.Name.ToUpper() == dr["Service Status"].ToString().ToUpper());

        //                    obj.StatusId = stastus == null ? 0 : stastus.Id;

        //                    obj.ProjectText = dr["Project Name"].ToString().Trim();

        //                    var project = projectList.FirstOrDefault(q => q.Name.ToUpper() ==
        //                    dr["Project Name"].ToString().Trim().ToUpper());

        //                    obj.ProjectId = project == null ? 0 : project.Id;

        //                    //obj.ChoiceAreaText = dr["Choice of Area"].ToString();

        //                    //var choiceofArea = choiceOfAreaList.FirstOrDefault(q => q.Name.ToUpper() ==
        //                    //dr["Choice of Area"].ToString().Trim().ToUpper());

        //                    //obj.ChoiceAreaId = choiceofArea == null ? 0 : choiceofArea.Id;

        //                    obj.Remarks = dr["Remarks"].ToString();
        //                    obj.ReferredBy = dr["Referred By"].ToString();

        //                    obj.CompanyId = companyId2;
        //                    CustomerList.Add(obj);
        //                }
        //                else if (string.IsNullOrWhiteSpace(dr["Full Name"].ToString()))
        //                {
        //                    break;
        //                }

        //            }
        //            vm.ResponseList = await _service.UploadClientBatch(CustomerList, companyId2, fileName);
        //            return View(vm);

        //        }
        //        catch (Exception ex)
        //        {
        //            vm.Status = $"Failed!!! {ex.Message}";
        //            return View(vm);
        //        }
        //    }
        //    else
        //    {
        //        vm.Status = "Failed!!! File not found";
        //        return View(vm);
        //    }

        //}

        [HttpPost]
        public async Task<ActionResult> UploadClientBatch(CrmUploadVm model)
        {
            var vm = new CrmUploadVm();
            vm.CompanyId = model.CompanyId;

            if (model.ExcelFile != null && model.ExcelFile.ContentLength > 0)
            {
                try
                {
                    using (var package = new ExcelPackage(model.ExcelFile.InputStream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        vm.ResponseList = await _service.UploadClientBatchh(worksheet, vm.CompanyId, model.ExcelFile.FileName);
                        TempData["Message"] = "Excel data uploaded successfully.";
                        return View(vm);
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "An error occurred while uploading Excel data: " + ex.Message;
                    return View(vm);
                }
            }
            else
            {
                TempData["Error"] = "Please select a valid Excel file.";
                return View(vm);
            }
        }


















        [HttpPost]
        public JsonResult SyncClientBatch(CrmUploadVm model)
        {

            var vm = _service.SyncClientBatch(model);
            return Json(vm, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public async Task<JsonResult> GetClientStatusHistories(int clientId)
        {
            long clientID = Convert.ToInt64(clientId);
            var model = await _service.GetServiceStatusHistories(clientID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<JsonResult> GetClientById(int clientId)
        {
            var model = await _service.GetClientById(clientId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<JsonResult> GetAllCompany()
        {
            var model = new CompanyListVm();
            model = await _service.GetAllCompany();
            return Json(model, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public async Task<ActionResult> TeamList(int companyId)
        {
            var model = new TeamListVm();
            model = await _service.GetTeamList(companyId);
            return View(model);
        }


        [HttpGet]
        public async Task<ActionResult> SaveServiceStatus(int companyId)
        {
            var model = new ServiceStatusListVm();
            model = await _service.GetAllServiceStatus(companyId);
            return View(model);
        }


        [HttpPost]
        public async Task<JsonResult> SaveServiceStatus(ServiceStatusVm model)
        {
            model = await _service.SaveServiceStatus(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public async Task<ActionResult> ChoiceAreaIndex(int companyId)
        {
            var model = new ChoiceAreaListVm();
            model = await _service.GetAllChoiceArea(companyId);

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> ChoiceArea(ChoiceAreaVm model)
        {
            model = await _service.SaveChoiceArea(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> PromootionalOffer(int companyId)
        {
            var model = new PromotionalOfferListVm();
            model = await _service.GetAllPromotionalOffer(companyId);
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> PromootionalOffer(PromotionalOfferVm model)
        {
            model = await _service.SavePromotionalOffer(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllResponsibleOfficer(int companyId)
        {
            companyId = companyId == 0 ? Convert.ToInt32(Request.Params["companyId"]) : companyId;
            var model = new ResponsibleOfficerListVm();
            model = await _service.GetAllResponsibleOfficer(companyId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllproject(int companyId)
        {
            var model = new ProjectListVm();
            model = await _service.GetAllproject(companyId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }





        [HttpGet]
        public async Task<JsonResult> GetAllServiceStatus(int companyId)
        {
            var model = new ServiceStatusListVm();
            model = await _service.GetAllServiceStatus(companyId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<JsonResult> GetAllProjectList(int companyId)
        {
            var model = new ProjectListVm();

            model = await _service.GetAllprojectForcrm(companyId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }







        [HttpGet]
        public async Task<JsonResult> GetServiceHistoryById(int KgreHistoryId)
        {
            var model = new ServiceStatusHistVm();
            model = await _service.GetServiceHistoryById(KgreHistoryId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetClientScheduleById(int scheduleId)
        {
            var model = new CrmScheduleVm();
            model = await _service.GetClientScheduleById(scheduleId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> RemoveServiceStatusById(ServiceStatusHistVm model)
        {
            model = await _service.RemoveServiceStatusNote(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<JsonResult> RemoveScheduleById(CrmScheduleVm model)
        {
            model = await _service.RemoveClientSchedule(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> EditStatusNote(ServiceStatusHistVm model)
        {
            model = await _service.UpdateStatusNote(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<JsonResult> EditScheduleNote(CrmScheduleVm model)
        {
            model = await _service.UpdateScheduleNote(model);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetAutoCompleteClientName(string prefix)
        {

            var client = await _service.GetAutoCompleteClientName(prefix);
            return Json(client, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetAutoCompleteClientMobile(string prefix)
        {
            var client = await _service.GetAutoCompleteClientMobile(prefix);
            return Json(client, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public async Task<JsonResult> DeleteServiceStatus(int id)
        {

            var obj = await _service.DeleteServiceStatus(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetServiceStatusById(int id)
        {
            var obj = await _service.GetServicestatusById(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }





        [HttpPost]
        public async Task<JsonResult> UpdateResponsibleOfficer(SelectModelVm Model)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            var obj = await _service.UpdateResofcrID(Model, uId);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<JsonResult> UpdateServiceStatus(SelectModelVm Model)
        {
            var obj = await _service.UpdateServstsId(Model);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCompany(SelectModelVm Model)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            var obj = await _service.UpdateCompany(Model, uId);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> SwitchServiceStatus(SelectModelVm Model)
        {
            var obj = await _service.SwitchServiceStatus(Model);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetPromotionalOfferById(int id)
        {
            var obj = await _service.GetPromotionalOfferById(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> GetAllPremission(int? companyId, int? userId)
        {

            var model = new PermissionModelListVm();
            if (userId != null && userId > 0 && companyId != null && companyId > 0)
            {
                model = _service.GetPermissionHandle(userId ?? 0, companyId ?? 0);
            }

            model.CompanyList = await _service.GetDropdownCompany();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> GetAllPremission(PermissionModelListVm model)
        {

            if (model == null)
            {
                return RedirectToAction("GetAllPremission", "Crms", new
                {
                    companyId = 0,
                    userId = 0
                });
            }
            return RedirectToAction("GetAllPremission", "Crms", new
            {
                companyId = model.CompanyId,
                userId = model.UserId
            });
        }

        [HttpPost]
        public async Task<JsonResult> SavePermission(int id, bool status, int companyId, int userId)
        {

            var model = await _service.SavePermission(id, status, companyId, userId);

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<JsonResult> GetAutoCompleteEmployee(string prefix)
        {
            var client = await _service.GetAutoCompleteEmployee(prefix);
            return Json(client, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<ActionResult> UpdateClientById(int clientId, int companyId)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            var model = new CrmScheduleListVm();
            if (!_permissionService.HasPermission(uId, PermissionCollection.Crms.Client.CanEditClient, companyId))
            {
                model.HasMessage = true;
                model.MessageList.Add("You Have No Permission To Update Client Information");
                return View(model);
            }
            if (_service.IsLeader(uId, companyId)) uId = 0;

            model = await _service.GetScheduleByClientId(clientId, uId, companyId);
            return View(model);
        }


        [HttpGet]
        public async Task<JsonResult> GetServiceByClientList(int StatusId, int companyId)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);


            var Model = new ClientStatusListVm();

            Model = await _service.GetClentStatus(StatusId, uId, companyId);

            return Json(Model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> ExportClientExcel(int? GenderId, int? ReligionId,
           int? ResponsibleOfficerId, int? ProjectId,
           int? TypeofInterestId, int? StatusId, int? SourceofMediaId, int? OfferId, int? ChoiceAreaId, int? lastUploadSerialNo, int companyId)
        {
            var model = new CrmUploadListVm();
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            int oni = Convert.ToInt32(userId);
            if (!_permissionService.HasPermission(uId, PermissionCollection.Crms.Client.CanExportClientExcel, companyId))
            {
                model.HasMessage = true;
                model.MessageList.Add("You Have No Permission To Update Client Information");
                return View(model);
            }

            model.CompanyId = companyId;
            model.GenderId = GenderId ?? 0;
            model.ReligionId = ReligionId ?? 0;
            model.ResponsibleOfficerId = ResponsibleOfficerId ?? 0;
            model.ProjectId = ProjectId ?? 0;
            model.TypeofInterestId = TypeofInterestId ?? 0;
            model.StatusId = StatusId ?? 0;
            model.SourceofMediaId = SourceofMediaId ?? 0;
            model.OfferId = OfferId ?? 0;
            model.ChoiceAreaId = ChoiceAreaId ?? 0;
            model.lastUploadSerialNo = lastUploadSerialNo ?? 0;


            if (_service.IsLeader(uId, model.CompanyId)) uId = 0;

            var dataList = await _service.FilteringClientlist(model, oni);

            var gv = new GridView();

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[20] { new DataColumn("Name"),
                                            new DataColumn("Mobile No"),
                                            new DataColumn("MobileNo 2"),
                                            new DataColumn("Email"),
                                            new DataColumn("Email2"),
                                             new DataColumn("DateofBirth"),
                                            new DataColumn("Gender"),
                                            new DataColumn("Campaign Name"),
                                            new DataColumn("Responsible Officer"),
                                            new DataColumn("Permanent Address"),
                                            new DataColumn("Present Address"),
                                            new DataColumn("Job Title"),
                                            new DataColumn("Organization"),
                                            new DataColumn("SourceofMedia"),
                                            new DataColumn("Promotional Offer"),
                                            new DataColumn("Project Name"),
                                            new DataColumn("Type of Interest"),
                                            new DataColumn("Status Level"),
                                            new DataColumn("Referred By"),
                                            new DataColumn("Remarks")
                                           });

            foreach (var m in dataList.DataList)
            {
                dt.Rows.Add(m.Name, m.MobileNo, m.MobileNo2, m.Email1, m.Email2,
                    m.DateofBirth, m.GenderName, m.CampaignText, m.ResponsibleOfficeName,
                    m.PermanentAddress, m.PresentAddress, m.JobTitle, m.OrganizationText,
                    m.SourceofMediaText, m.PromotionalOfferText, m.ProjectText, m.TypeofInterestText,
                    m.StatusText, m.ReferredBy, m.Remarks);
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }

        }


        [HttpGet]
        public async Task<ActionResult> ExportClientUploadBatchHistoryExcel(int companyId, int uploadSerialNo, DateTime uploadDateTime)
        {
            var model = new ClientBatchUplodListVm();
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            if (!_permissionService.HasPermission(uId, PermissionCollection.Crms.Client.CanExportClientUploadBatchExcel, companyId))
            {
                model.HasMessage = true;
                model.MessageList.Add("You Have No Permission To Expot Client Information");
                return View(model);
            }

            model.CompanyId = companyId;

            if (_service.IsLeader(uId, model.CompanyId)) uId = 0;

            var dataList = await _service.FilteringClientUploadBatchList(companyId, uploadSerialNo, uploadDateTime, uId);

            var gv = new GridView();
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[20] {
                                            new DataColumn("Name"),
                                            new DataColumn("Mobile No"),
                                            new DataColumn("MobileNo 2"),
                                            new DataColumn("Email"),
                                            new DataColumn("Email2"),
                                            new DataColumn("DateofBirth"),
                                            new DataColumn("Gender"),
                                            new DataColumn("Campaign Name"),
                                            new DataColumn("Responsible Officer"),
                                            new DataColumn("Permanent Address"),
                                            new DataColumn("Present Address"),
                                            new DataColumn("Job Title"),
                                            new DataColumn("Organization"),
                                            new DataColumn("SourceofMedia"),
                                            new DataColumn("Promotional Offer"),
                                            new DataColumn("Project Name"),
                                            new DataColumn("Type of Interest"),
                                            new DataColumn("Status Level"),
                                            new DataColumn("Referred By"),
                                            new DataColumn("Remarks")
            });


            foreach (var m in dataList.DataList)
            {
                dt.Rows.Add(
                    m.Name,
                    m.MobileNo,
                    m.MobileNo2,
                    m.Email,
                    m.Email2,
                    m.DateofBirth,
                    m.GenderText,
                    m.CampaignText,
                    m.ResponsibleOfficeName,
                    m.PermanentAddress,
                    m.PresentAddress,
                    m.JobTitle,
                    m.OrganizationText,
                    m.SourceofMediaText,
                    m.OfferText,
                    m.ProjectText,
                    m.TypeofInterestText,
                    m.StatusText,
                    m.ReferredBy,
                    m.Remarks
                    );
            }


            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }

        }



        [HttpGet]
        public async Task<JsonResult> GetClientCopyById(int clientId)
        {
            var model = await _service.GetClientCopyById(clientId);
            model.CompanyList = await _service.GetDropdownCompany();
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<JsonResult> CopyClientSave(int ClientId, int SelectedCompanyId)
        {
            var model = await _service.CopyClientSave(ClientId, SelectedCompanyId);

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<JsonResult> SaveTask(int scheduleId, int SelectedCompanyId)
        {
            var model = await _service.SaveTask(scheduleId, SelectedCompanyId);

            return Json(model, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public async Task<ActionResult> UpdateSectionClient(CrmVm model)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            if (_service.IsLeader(uId, model.CompanyId)) uId = 0;

            model.UserId = uId;


            model = await _service.UpdateSectionClient(model);

            return RedirectToAction(nameof(GetClientDetailsById), "Crms", new { clientId = model.ClientId, companyId = model.CompanyId });
        }


        [HttpGet]
        public async Task<ActionResult> PendingScheduleList(int companyId)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            if (_service.IsLeader(uId, companyId)) uId = 0;

            var model = new CrmScheduleListVm();

            model.UserId = uId;

            model = await _service.GetPendingScheduleList(companyId, uId);


            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> CompletedScheduleList(int companyId)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            if (_service.IsLeader(uId, companyId)) uId = 0;

            var model = new CrmScheduleListVm();

            model.UserId = uId;

            model = await _service.GetCompletedScheduleList(companyId, uId);


            return View(model);
        }

        [HttpPost]

        public async Task<ActionResult> RemarksView(int ClientId)
        {
            var obj = await _service.GetNoteList(ClientId);


            return Json(obj);


        }


        [HttpPost]
        public async Task<ActionResult> NoteBox(CrmVm Model)
        {
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            if (_service.IsLeader(uId, Model.CompanyId)) uId = 0;

            Model = await _service.noteview(Model);

            return Json(Model);



        }



        [HttpGet]
        public async Task<ActionResult> CountingCategory(int Companyid)
        {
            CrmVm vm = new CrmVm();
            vm.DealingOfficerList = await _service.AllResponsibleoffficerrr(Companyid);
            return View(vm);
        }



        [HttpGet]
        public async Task<ActionResult> Modifyhistory(int Companyid)
        {
            CrmVm vm = new CrmVm();
            vm.DealingOfficerList = await _service.AllResponsibleoffficerrr(Companyid);
            vm.ServiceStatusList = await _service.GetService(Companyid);
            vm.CompanyId = Companyid;
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> GetModifyhistory(CrmVm model)
        {
            var obj = await _service.ModifyUpload(model);

            return Json(obj);

        }


        [HttpPost]
        public async Task<ActionResult> GetCountingResponsible(CrmViewModel model)
        {
            var obj = await _service.Countingupdate(model);

            return Json(obj);

        }


        [HttpPost]
        public async Task<ActionResult> GetExistClient(string MobileNumber, int CompanyId)
        {
            var obj = await _service.GetExistClientByID(MobileNumber, CompanyId);
            return Json(obj);

        }

        [HttpPost]
        public async Task<ActionResult> Reassign(CrmVm model)
        {

            var obj = await _service.ReassignClient(model);
            return Json(obj);

        }

        [HttpGet]
        public async Task<ActionResult> CarRequisition(int CompanyId, int? id)
        {
            VehicleVm vm = new VehicleVm();
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);


            if (id > 0)
            {
                vm = await _service.GetDetailsForUpdate(id);
                vm.ProjectList = await _service.GetDropdownProject(CompanyId);
                vm.Strdate = vm.Date.ToString("dd/MM/yyyy");
            }
            else
            {
                vm = await _service.GetOfficerName(CompanyId, uId);
                vm.ProjectList = await _service.GetDropdownProject(CompanyId);
                vm.Date = DateTime.Now;
                vm.Strdate = vm.Date.ToString("dd/MM/yyyy");
            }




            return View(vm);

        }

        [HttpPost]
        public async Task<ActionResult> CarRequisition(VehicleVm model)
        {
            VehicleVm vm = new VehicleVm();
            vm = await _service.CraRequisitinAdd(model);
            return RedirectToAction(nameof(CarRequisitionListIdividual), "Crms", new { companyId = model.CompanyId });
        }


        [HttpGet]
        public async Task<ActionResult> CarRequisitionList(int CompanyId, DateTime? Dateee)
        {
            VehicleVm vm = new VehicleVm();
            if (Dateee == null)
            {
                Dateee = DateTime.Now;
            }

            vm = await _service.GetCarRquisitinList(CompanyId, Dateee);
            vm.Strdate = Dateee.Value.ToString("dd/MM/yyyy");
            return View(vm);

        }


        [HttpPost]

        public async Task<ActionResult> CarRequisitionList(VehicleVm vm)
        {
            vm.Date = Convert.ToDateTime(vm.Strdate);
            return RedirectToAction(nameof(CarRequisitionList), new { companyId = vm.CompanyId, Dateee = vm.Date });
        }

        [HttpGet]
        public async Task<ActionResult> CarRequisitionListIdividual(int CompanyId)
        {
            VehicleVm vm = new VehicleVm();
            var userId = HttpContext.Session["Id"];
            int uId = Convert.ToInt32(userId);
            vm = await _service.GetCarRquisitinListIndividual(CompanyId, uId);

            return View(vm);

        }



        [HttpPost]
        public async Task<JsonResult> DeleteCarRequision(int id)
        {

            var obj = await _service.DeleteCarReq(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public async Task<ActionResult> SaveDriverName(VehicleVm model)
        {

            var obj = await _service.Savedrivernamsper(model);
            return Json(obj);

        }


    }
}