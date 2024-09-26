using DocumentFormat.OpenXml.EMMA;
using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.Utility;
using KGERP.Utility;
using KGERP.Utility.Util;
using KGERP.ViewModel;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Windows.Media.Media3D;
using static KGERP.Controllers.Custom_Authorization.ParentAuthorizedAttribute;

namespace KGERP.Controllers
{
    [CheckSession]
    public class EmployeeController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IEmployeeService employeeService = new EmployeeService(new ERPEntities());
        ICompanyService companyService = new CompanyService(new ERPEntities());
        IDropDownItemService dropDownItemService = new DropDownItemService(new ERPEntities());
        IDistrictService districtService = new DistrictService(new ERPEntities());
        IUpazilaService upazilaService = new UpazilaService(new ERPEntities());
        IDepartmentService departmentService = new DepartmentService();
        IDesignationService designationService = new DesignationService();
        IBankService bankService = new BankService();
        IBankBranchService bankBranchService = new BankBranchService();
        IShiftService shiftService = new ShiftService();
        IGradeService gradeService = new GradeService();
        private ERPEntities db = new ERPEntities();
        //private readonly IDistrictService districtService;
        //private readonly IWebHostEnvironment _hostingEnvironment;

        //public EmployeeController(IWebHostEnvironment hostingEnvironment)
        //{
        //    _hostingEnvironment = hostingEnvironment;
        //}

        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId)
        {
            EmployeeVm model = new EmployeeVm();
            model = await employeeService.GetEmployees(companyId);
            return View(model);
        }
        
        [HttpGet]
        public async Task<ActionResult> KSSLIndex(int CompanyId)
        {
            EmployeeVm model = new EmployeeVm();
            model = await employeeService.GetKSSLEmployees(CompanyId);
            return View(model);
        }

        
        [HttpGet]
        public async Task<ActionResult> MySalaery()
        {
            int myCompanyId  = Convert.ToInt32(Session["CompanyId"].ToString()); // My CompanyId
            VmPayRoll model = new VmPayRoll();
            model = await employeeService.GetMySalaery(myCompanyId);
            return View(model);
        }

        
        [HttpGet]
        public async Task<ActionResult> MySalaeryByPayrollId(long payRollId)
        {
            long myEmployeeId = Convert.ToInt64(Session["Id"].ToString());
            VmPayRollDetail model = new VmPayRollDetail();
            model = await employeeService.GetMySalaeryByPayrollId(payRollId, myEmployeeId);
            return View(model);
        }

        
        [HttpGet]
        public ActionResult ProbitionList(int? Page_No, string searchText)
        {
            List<EmployeeModel> employees = employeeService.GetProbitionPreiodEmployeeList();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(employees.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        
        [HttpGet]
        public ActionResult EmployeeSearchIndex()
        {
            return View();
        }

        
        [HttpGet]
        public async Task<ActionResult> EmployeeSearchIndexByCompanyId(int companyId)
        {
            EmployeeVm model = new EmployeeVm();
            model = await employeeService.GetEmployees(companyId);
            return View(model);
        }

        
        [HttpPost]
        public ActionResult EmployeeSearchByCompany()
        {
            var userId = HttpContext.Session["Id"];
            var userIdd = Convert.ToInt64(HttpContext.Session["Id"]);
        

            //Server Side Parameter
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];
            int? companyid = db.Employees.Where(x => x.Id == userIdd).Select(c=>c.CompanyId).FirstOrDefault();


            List<EmployeeModel> empList = employeeService.EmployeeSearchByCoId(companyid);
            int totalRows = empList.Count;
            if (!string.IsNullOrEmpty(searchValue)) //filter
            {
                empList = empList = empList.Where(x => x.EmployeeId.ToLower().Contains(searchValue.ToLower())
                                          || x.Name.ToLower().Contains(searchValue.ToLower())
                                          || x.DepartmentName.ToLower().Contains(searchValue.ToLower())
                                          || x.DesignationName.ToLower().Contains(searchValue.ToLower())
                                          || x.StrJoiningDate.ToLower().Contains(searchValue.ToLower())
                                          || x.OfficeEmail.ToLower().Contains(searchValue.ToLower())
                                          || x.PABX.ToLower().Contains(searchValue.ToLower())
                                          || x.MobileNo.ToLower().Contains(searchValue.ToLower())
                                          || x.BloodGroupName.ToLower().Contains(searchValue.ToLower())
                                          || x.Remarks.ToLower().Contains(searchValue.ToLower())).ToList<EmployeeModel>();

            }
            int totalRowsAfterFiltering = empList.Count;


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
 
            empList = empList.OrderBy(sortColumnName + " " + sortDirection).ToList<EmployeeModel>();

            //paging
            empList = empList.Skip(start).Take(length).ToList<EmployeeModel>();


            return Json(new { data = empList, draw = Request["draw"], recordsTotal = totalRows, recordsFiltered = totalRowsAfterFiltering }, JsonRequestBehavior.AllowGet);

        }



        
        [HttpPost]
        public ActionResult EmployeeSearch()
        {
            //Server Side Parameter
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];



            List<EmployeeModel> empList = employeeService.EmployeeSearch();
            int totalRows = empList.Count;
            if (!string.IsNullOrEmpty(searchValue)) //filter
            {
                empList = empList.Where(x => x.EmployeeId.ToLower().Contains(searchValue.ToLower())
                                          || x.Name.ToLower().Contains(searchValue.ToLower())
                                          || x.DepartmentName.ToLower().Contains(searchValue.ToLower())
                                          || x.DesignationName.ToLower().Contains(searchValue.ToLower())
                                          || x.StrJoiningDate.ToLower().Contains(searchValue.ToLower())
                                          || x.OfficeEmail.ToLower().Contains(searchValue.ToLower())
                                          || x.PABX.ToLower().Contains(searchValue.ToLower())
                                          || x.MobileNo.ToLower().Contains(searchValue.ToLower())
                                          || x.BloodGroupName.ToLower().Contains(searchValue.ToLower())
                                          || x.Remarks.ToLower().Contains(searchValue.ToLower())).ToList<EmployeeModel>();
            }
            int totalRowsAfterFiltering = empList.Count;


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
            empList = empList.OrderBy(sortColumnName + " " + sortDirection).ToList<EmployeeModel>();

            //paging
            empList = empList.Skip(start).Take(length).ToList<EmployeeModel>();


            return Json(new { data = empList, draw = Request["draw"], recordsTotal = totalRows, recordsFiltered = totalRowsAfterFiltering }, JsonRequestBehavior.AllowGet);

        }

        
        [HttpGet]
        public async Task<ActionResult> PreviousEmployees(int? Page_No, bool employeeType, string searchText)
        {
            searchText = searchText ?? string.Empty;
            List<EmployeeModel> employees = await employeeService.GetEmployeesAsync(employeeType, searchText);
            int Size_Of_Page = 10000;
            int No_Of_Page = (Page_No ?? 1);
            return View(employees.ToPagedList(No_Of_Page, Size_Of_Page));

        }

        
        [HttpGet]
        public ActionResult Details(long employeeId = 0)
        {
            long empId = 0;
            int companyId = Common.GetCompanyId();
            if (employeeId > 0)
            {
                empId = employeeId;
            }
            else
            {
                empId = Convert.ToInt64(Session["Id"].ToString());
            }

            EmployeeModel model = employeeService.GetEmployee(empId, companyId);
            if (model == null)
            {
                return HttpNotFound();
            }
            if (model.ImageFileName == null)
            {
                model.ImageFileName = "default.png";
            }
            model.ImagePath = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/Picture/" + model.ImageFileName;

            return View(model);
        }

        

        
        [HttpGet]
        public ActionResult CreateOrEdit(int companyId, long id)
        {
           
            EmployeeViewModel vm = new EmployeeViewModel();
            vm.Employee = employeeService.GetEmployee(id, companyId);


            var companies = companyService.GetCompanySelectModelsISS(companyId);
            var selectedCompaniesID = companies.FirstOrDefault()?.GetType().GetProperty("Value")?.GetValue(companies.FirstOrDefault()) ?? 0;

            var request = HttpContext.Request;
            var baseUrl = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
            if (string.IsNullOrEmpty(vm.Employee.ImageFileName))
            {
                vm.Employee.ImageFileName = "default.png";
            }
            var imageUrl = baseUrl + "/Images/Picture/" + vm.Employee.ImageFileName;
            vm.Employee.ImagePath = imageUrl;


            if (string.IsNullOrEmpty(vm.Employee.SignatureFileName))
            {
                vm.Employee.SignatureFileName = "signaturePen.png";
            }
            var signatureUrl = baseUrl + "/Images/Signature/" + vm.Employee.SignatureFileName;
            vm.Employee.SignaturePath = signatureUrl;

            vm.Managers = employeeService.GetEmployeeSelectModelsISS(companyId);
            vm.Companies = companyService.GetCompanySelectModelsISS(companyId);
            vm.Religions = dropDownItemService.GetDropDownItemSelectModels(9);
            vm.BloodGroups = dropDownItemService.GetDropDownItemSelectModels(5);
            vm.Countries = dropDownItemService.GetDropDownItemSelectModels(14);
            //vm.Districts = districtService.GetDistrictSelectModels();
            vm.Divisions = districtService.GetDivisionSelectModels();
            vm.MaritalTypes = dropDownItemService.GetDropDownItemSelectModels(2);
            vm.Genders = dropDownItemService.GetDropDownItemSelectModels(3);
            vm.EmployeeCategories = dropDownItemService.GetDropDownItemSelectModels(8);
            vm.Departments = departmentService.GetDepartmentSelectModels();
            vm.Designations = designationService.GetDesignationSelectModels();
            vm.OfficeTypes = dropDownItemService.GetDropDownItemSelectModels(12);
            vm.DisverseMethods = dropDownItemService.GetDropDownItemSelectModels(13);
            vm.JobStatus = dropDownItemService.GetDropDownItemSelectModels(15);
            vm.JobTypes = dropDownItemService.GetDropDownItemSelectModels(10);
            vm.Banks = bankService.GetBankSelectModels();
            vm.BankBranches = new List<SelectModel>();
            vm.CompanyList = new SelectList(companies, "Value", "Text", selectedCompaniesID);
            vm.Employee.CompanyId = (int)selectedCompaniesID;
            if (vm.Employee.BankId > 0)
            {
                vm.BankBranches = bankBranchService.GetBankBranchByBank(vm.Employee.BankId ?? 0);
            }

            if (vm.Employee.DivisionId > 0)
            {
                vm.Districts = districtService.GetDistrictByDivisionId(vm.Employee.DivisionId ?? 0);
                if (vm.Employee.DistrictId > 0)
                {
                    vm.Upazilas = upazilaService.GetUpzilaByDistrictId(vm.Employee.DistrictId ?? 0);
                }
            }
            else
            {
                vm.Upazilas = new List<SelectModel>();
                vm.Districts = new List<SelectModel>();
            }

            vm.Shifts = shiftService.GetShiftSelectModels();
            vm.SalaryGrades = gradeService.GetGradeSelectModels();
            return View(vm);
        }
       
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(EmployeeViewModel vm, HttpPostedFileBase image, HttpPostedFileBase signature)
        {
            ModelState.Clear();
            bool result = false;
            string picture = string.Empty;
            string sig = string.Empty;

            if (image != null)
            {
                var supportedTypes = new[] { "jpg", "jpeg", "png", "bmp", "JPG", "JPEG", "PNG", "BMP" };
                var fileExt = System.IO.Path.GetExtension(image.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    string ErrorMessage = Constants.FileType;
                    throw new Exception(ErrorMessage);
                }
                int count = 1;
                string fileExtension = Path.GetExtension(image.FileName);
                picture = vm.Employee.EmployeeId + fileExtension;

                string fullPath = Path.Combine(Server.MapPath("~/Images/Picture"), picture);


                string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
                string extension = Path.GetExtension(fullPath);
                string path = Path.GetDirectoryName(fullPath);
                string newFullPath = fullPath;

                while (System.IO.File.Exists(newFullPath))
                {
                    picture = string.Format("{0}({1})", fileNameOnly, count++);
                    newFullPath = Path.Combine(path, picture + extension);
                    picture = picture + extension;
                }
                image.SaveAs(Path.Combine(path, picture));
                vm.Employee.ImageFileName = picture;
            }

            if (signature != null)
            {
                var supportedTypes = new[] { "jpg", "jpeg", "png", "bmp", "JPG", "JPEG", "PNG", "BMP" };
                var fileExt = System.IO.Path.GetExtension(signature.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    string ErrorMessage = Constants.FileType;
                    throw new Exception(ErrorMessage);
                }
                int count = 1;
                string fileExtension = Path.GetExtension(signature.FileName);
                sig = vm.Employee.EmployeeId + fileExtension;

                string fullPath = Path.Combine(Server.MapPath("~/Images/Signature"), sig);


                string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
                string extension = Path.GetExtension(fullPath);
                string path = Path.GetDirectoryName(fullPath);
                string newFullPath = fullPath;

                while (System.IO.File.Exists(newFullPath))
                {
                    picture = string.Format("{0}({1})", fileNameOnly, count++);
                    newFullPath = Path.Combine(path, sig + extension);
                    sig = sig + extension;
                }
                signature.SaveAs(Path.Combine(path, sig));
                vm.Employee.SignatureFileName = sig;
            }

            if (vm.Employee.Id <= 0)
            {
                result = employeeService.SaveEmployee(0, vm.Employee);
                if (result)
                {
                    return RedirectToAction("CreateOrEdit", new { companyId = vm.Employee.CompanyId,  id = 0 });
                }
            }
            else
            {
                result = employeeService.SaveEmployee(vm.Employee.Id, vm.Employee);
                if (result)
                {
                    return RedirectToAction("CreateOrEdit", new { companyId = vm.Employee.CompanyId, id = vm.Employee.Id });
                }

            }
            //if (result)
            //{
            //    return RedirectToAction("CreateOrEdit", new { id = 0 });
            //}
            return RedirectToAction("CreateOrEdit", new { companyId = vm.Employee.CompanyId, id = vm.Employee.Id });
        }







        //<ani>
        
        [HttpGet]
        public ActionResult CreateOrEditKSSL(long id, int CompanyId)
        {
            EmployeeViewModel vm = new EmployeeViewModel();
            vm.Employee = employeeService.GetEmployeeForKSSL(id, CompanyId);

            var request = HttpContext.Request;
            var baseUrl = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);
            if (string.IsNullOrEmpty(vm.Employee.ImageFileName))
            {
                vm.Employee.ImageFileName = "default.png";
            }
            var imageUrl = baseUrl + "/Images/Picture/" + vm.Employee.ImageFileName;
            vm.Employee.ImagePath = imageUrl;


            if (string.IsNullOrEmpty(vm.Employee.SignatureFileName))
            {
                vm.Employee.SignatureFileName = "signaturePen.png";
            }
            var signatureUrl = baseUrl + "/Images/Signature/" + vm.Employee.SignatureFileName;
            vm.Employee.SignaturePath = signatureUrl;

            vm.Managers = employeeService.GetEmployeeSelectModels();
            vm.Companies = companyService.GetCompanySelectModelsForKSSl(CompanyId);
            vm.Religions = dropDownItemService.GetDropDownItemSelectModels(9);
            vm.BloodGroups = dropDownItemService.GetDropDownItemSelectModels(5);
            vm.Countries = dropDownItemService.GetDropDownItemSelectModels(14);
            //vm.Districts = districtService.GetDistrictSelectModels();
            vm.Divisions = districtService.GetDivisionSelectModels();
            vm.MaritalTypes = dropDownItemService.GetDropDownItemSelectModels(2);
            vm.Genders = dropDownItemService.GetDropDownItemSelectModels(3);
            vm.EmployeeCategories = dropDownItemService.GetDropDownItemSelectModels(8);
            vm.Departments = departmentService.GetDepartmentSelectModels();
            vm.Designations = designationService.GetDesignationSelectModels();
            vm.OfficeTypes = dropDownItemService.GetDropDownItemSelectModels(12);
            vm.DisverseMethods = dropDownItemService.GetDropDownItemSelectModels(13);
            vm.JobStatus = dropDownItemService.GetDropDownItemSelectModels(15);
            vm.JobTypes = dropDownItemService.GetDropDownItemSelectModels(10);
            vm.RegionDistricts = dropDownItemService.GetRegionSelectModels();
            vm.Banks = bankService.GetBankSelectModels(CompanyId);
            vm.Employee.DDLServiceCompany = dropDownItemService.GetDDLVendors(CompanyId);

            vm.BankBranches = new List<SelectModel>();
            if (vm.Employee.BankId > 0)
            {
                vm.BankBranches = bankBranchService.GetBankBranchByBank(vm.Employee.BankId ?? 0);
            }

            if (vm.Employee.DivisionId > 0)
            {
                vm.Districts = districtService.GetDistrictByDivisionId(vm.Employee.DivisionId ?? 0);
                if (vm.Employee.DistrictId > 0)
                {
                    vm.Upazilas = upazilaService.GetUpzilaByDistrictId(vm.Employee.DistrictId ?? 0);
                }
            }
            else
            {
                vm.Upazilas = new List<SelectModel>();
                vm.Districts = new List<SelectModel>();
            }

            vm.Shifts = shiftService.GetShiftSelectModels();
            vm.SalaryGrades = gradeService.GetGradeSelectModels();
            vm.Employee.CompanyId = CompanyId;
            return View(vm);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEditKSSL(EmployeeViewModel vm, HttpPostedFileBase image, HttpPostedFileBase signature)
        {
            ModelState.Clear();
            bool result = false;
            string picture = string.Empty;
            string sig = string.Empty;

            if (image != null)
            {
                var supportedTypes = new[] { "jpg", "jpeg", "png", "bmp", "JPG", "JPEG", "PNG", "BMP" };
                var fileExt = System.IO.Path.GetExtension(image.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    string ErrorMessage = Constants.FileType;
                    throw new Exception(ErrorMessage);
                }
                int count = 1;
                string fileExtension = Path.GetExtension(image.FileName);
                picture = vm.Employee.EmployeeId + fileExtension;

                string fullPath = Path.Combine(Server.MapPath("~/Images/Picture"), picture);


                string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
                string extension = Path.GetExtension(fullPath);
                string path = Path.GetDirectoryName(fullPath);
                string newFullPath = fullPath;

                while (System.IO.File.Exists(newFullPath))
                {
                    picture = string.Format("{0}({1})", fileNameOnly, count++);
                    newFullPath = Path.Combine(path, picture + extension);
                    picture = picture + extension;
                }
                image.SaveAs(Path.Combine(path, picture));
                vm.Employee.ImageFileName = picture;
            }

            if (signature != null)
            {
                var supportedTypes = new[] { "jpg", "jpeg", "png", "bmp", "JPG", "JPEG", "PNG", "BMP" };
                var fileExt = System.IO.Path.GetExtension(signature.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    string ErrorMessage = Constants.FileType;
                    throw new Exception(ErrorMessage);
                }
                int count = 1;
                string fileExtension = Path.GetExtension(signature.FileName);
                sig = vm.Employee.EmployeeId + fileExtension;

                string fullPath = Path.Combine(Server.MapPath("~/Images/Signature"), sig);


                string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
                string extension = Path.GetExtension(fullPath);
                string path = Path.GetDirectoryName(fullPath);
                string newFullPath = fullPath;

                while (System.IO.File.Exists(newFullPath))
                {
                    picture = string.Format("{0}({1})", fileNameOnly, count++);
                    newFullPath = Path.Combine(path, sig + extension);
                    sig = sig + extension;
                }
                signature.SaveAs(Path.Combine(path, sig));
                vm.Employee.SignatureFileName = sig;
            }

            if (vm.Employee.Id <= 0)
            {
                result = employeeService.SaveEmployee(0, vm.Employee);
                if (result)
                {
                    return RedirectToAction("CreateOrEditKSSL", new { id = 0, CompanyId=vm.Employee.CompanyId });
                }
            }
            else
            {
                result = employeeService.SaveEmployee(vm.Employee.Id, vm.Employee);
                if (result)
                {
                    return RedirectToAction("CreateOrEditKSSL", new { id = vm.Employee.Id, CompanyId = vm.Employee.CompanyId });
                }

            }
            //if (result)
            //{
            //    return RedirectToAction("CreateOrEdit", new { id = 0 });
            //}
            return RedirectToAction("CreateOrEditKSSL", new { id = vm.Employee.Id, CompanyId = vm.Employee.CompanyId });
        }














        public FileResult Download(String fileName, String employeeId)
        {
            return File(Path.Combine(Server.MapPath("~/KGFiles/HRMS/" + employeeId), fileName), System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
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
                    Employee employee = db.Employees.Find(fileDetail.EmployeeId);
                    path1 = Path.Combine(Server.MapPath(string.Format("~/{0}/{1}/{2}/", "KGFiles", "HRMS", employee.EmployeeId)), fileDetail.AttachFileName);
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


        
        [HttpGet]
        public ActionResult Delete(long id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int companyId = Common.GetCompanyId();
            EmployeeModel employee = employeeService.GetEmployee(id, companyId);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            bool result = employeeService.DeleteEmployee(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult EmployeeAutoComplete(string prefix)
        {
            var employee = employeeService.GetEmployeeAutoComplete(prefix);
            return Json(employee);
        }

        [HttpPost]
        public JsonResult EmployeeAutoCompleteByCompany(string prefix,int CompanyId)
        {
            var employee = employeeService.GetEmployeeAutoCompleteByCompany(prefix, CompanyId);
            return Json(employee);
        }
        [HttpPost]
        public JsonResult EmployeeAutoCompleteOfficerJoin(string prefix,int CompanyId)
        {
            var employee = employeeService.GetEmployeeAutoCompleteOfficer(prefix,CompanyId);
            return Json(employee);
        }




        [HttpGet]
        public JsonResult GetEmployeeInformation(long id)
        {
            int companyId = Common.GetCompanyId();
            EmployeeModel employee = employeeService.GetEmployee(id, companyId);

            var result = JsonConvert.SerializeObject(employee, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EmployeeDesignationAutoComplete(string prefix)
        {
            var employee = employeeService.GetEmployeeDesignationAutoComplete(prefix);
            return Json(employee);
        }

        #region  Employee Anniversary Event 20-02-2020

        
        [HttpGet]
        public ActionResult GetBirthday(int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            List<EmployeeModel> employees = employeeService.GetEmployeeEvent();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            if (!string.IsNullOrEmpty(searchText))
            {
                var employees1 = employees.Where(
                    x => x.Anniversary.Contains(searchText)
                || x.Name.Contains(searchText)
                || x.EDepartment.Contains(searchText)
                || x.EDesignation.Contains(searchText));
                return View(employees1.ToPagedList(No_Of_Page, Size_Of_Page));
            }
            else
            {
                return View(employees.ToPagedList(No_Of_Page, Size_Of_Page));
            }
        }

        
        [HttpGet]
        public ActionResult GetTodayAnniversary(int? Page_No, string searchText, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.message = message;
            }
            searchText = searchText ?? string.Empty;
            List<EmployeeModel> employees = employeeService.GetEmployeeTodayEvent();
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(employees.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        public ActionResult WishHappyAnniversary(bool IsGloval = false)
        {
            DataTable dt = new DataTable();
            dt = GetHappyAnniversaryData();
            //creating a image object   
            // Delete all files in a directory  
            int existingFiles = 0;
            int count = 0;
            if (existingFiles == 0)
            {
                string[] files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/FileUpload/Aniversary"));
                //string[] files = Directory.GetFiles(Server.MapPath("~/FileUpload/Aniversary"));
                if (files != null || files.Length != 0)
                {
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Email"].ToString() != null)
                    {
                        dt.Rows[i]["Anniversary"].ToString();
                        dt.Rows[i]["Name"].ToString();
                        dt.Rows[i]["EmployeeId"].ToString();
                        dt.Rows[i]["EventDate"].ToString();
                        dt.Rows[i]["EDepartment"].ToString();
                        dt.Rows[i]["EDesignation"].ToString();
                        dt.Rows[i]["Email"].ToString();
                        HappyAnniversaryCard(dt.Rows[i]["Anniversary"].ToString(),
                        dt.Rows[i]["Name"].ToString(),
                        dt.Rows[i]["EmployeeId"].ToString(),
                        dt.Rows[i]["EventDate"].ToString(),
                        dt.Rows[i]["EDepartment"].ToString(),
                        dt.Rows[i]["EDesignation"].ToString(),
                        dt.Rows[i]["Email"].ToString());
                        count += 1;
                    }
                    
                }
                existingFiles += 1;

            }
            if (IsGloval == true)
            {
                return Content("");
            }

            return RedirectToAction("GetTodayAnniversary", "Employee", new { message = count + " Anniversary mail sent" });
        }

        private DataTable GetHappyAnniversaryData()
        {
            string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Employee_TodayAniversaryEvent", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public void HappyAnniversaryCard(string anniversary, string name, string employeeId, string eventDate, string department, string designation, string email)
        {
            //if (email == "" || email == "kgerp19@gmail.com")
            //    return;
            string htmlMessage = @"<!DOCTYPE html><html><head>
        <meta name=" + "viewport" + " content=" + "width = device - width, initial - scale = 1><style>" +
        ".container { position: relative;text - align: center;color: white;}" +
        ".centered {position: absolute;top: 40 %;left: 53 %;transform: translate(-50 %, -50 %);"
        + "color: red;border: 1px dotted blue;display: inline - block;max - width: 250px;padding: 10px;" +
          "word -break: break-all;}</style></head><body>" +
        "<div class=" + "container" + "> <img src = " + "cid:EmbeddedContent_AnniversaryCard" + " alt=" + "Annivarsary" + "/> </div></body></html> ";

            string receiverEmail = "rafiqulislam@krishibidgroup.com";// email;
           
            string senderEmail = "kgerp@krishibidgroup.com";
            string senderName = "Krishibid Group";
            var password = "kgerp@2023";
            var fromEmail = new MailAddress(senderEmail, senderName);            
            string[] bccEmail = db.DropDownItems.Where(x => x.DropDownTypeId == 70 && x.IsActive == true)
                       .Select(x => x.Name).ToArray();
            string[] ccEmail = db.DropDownItems.Where(x => x.DropDownTypeId == 69 && x.IsActive == true)
                       .Select(x => x.Name).ToArray();
            
           
            var client = new SmtpClient
            {
                Host = "mail.krishibidgroup.com",
                Port = 25,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, password)
            };


            if (!string.IsNullOrEmpty(anniversary) && anniversary == "Work Anniversary")
            {
                
                string path2 = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/Work Anniversary.png");
                Bitmap bitmap = (Bitmap)Image.FromFile(path2);//load the image file
                var fileName = Path.GetFileName(path2);
                var path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/FileUpload/Aniversary"), name + "_" + fileName);//Save in temporary file

                //draw the image object using a Graphics object    
                Graphics graphicsImage = Graphics.FromImage(bitmap);

                //Set the alignment based on the coordinates       
                StringFormat stringformat = new StringFormat();
                stringformat.Alignment = StringAlignment.Center;
                stringformat.LineAlignment = StringAlignment.Center;

                StringFormat stringformat2 = new StringFormat();
                stringformat2.Alignment = StringAlignment.Center;
                stringformat2.LineAlignment = StringAlignment.Center;

                //Set the font color/format/size etc..      
                System.Drawing.Color StringColor = ColorTranslator.FromHtml("#933eea");//direct color adding  
                System.Drawing.Color StringColor3 = ColorTranslator.FromHtml("#154a2a");//direct color adding   
                System.Drawing.Color StringColor2 = ColorTranslator.FromHtml("#e80c88");//customise color adding 
                System.Drawing.Color smsColor = ColorTranslator.FromHtml("#581845");//customise color adding   

                string Str_TextOnImage2 = name + "\n" + designation + "\n" + department;//Your Text On Image    
                string anniversaryday = "On your Joining (Date: " + eventDate + ")";//Your Text On Image    
                string anniversarySms = "You have been an essential part of our organization's journey \nand success. We are very much grateful for the contribution and \npassion you have shown. Thank you for being with us.";

                graphicsImage.DrawString(Str_TextOnImage2, new Font("Helvetica", 55,
                FontStyle.Bold), new SolidBrush(StringColor2), new Point(1140, 700),
                stringformat2); 
                //Response.ContentType = "image/jpeg";

                graphicsImage.DrawString(anniversaryday, new Font("arial", 38,
                FontStyle.Bold), new SolidBrush(smsColor), new Point(1140, 890), stringformat2);
               // Response.ContentType = "image/jpeg";

                graphicsImage.DrawString(anniversarySms, new Font("arial", 38,
                FontStyle.Bold), new SolidBrush(StringColor3), new Point(1140, 1040), stringformat2);
                //Response.ContentType = "image/jpeg";

                bitmap.Save(path, ImageFormat.Png);
                bitmap.Dispose(); //bitmap.Save(Response.OutputStream, ImageFormat.Jpeg);//Display image in Browser

                MailMessage msg = new MailMessage(senderEmail, receiverEmail);
                // Create the HTML view
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlMessage, Encoding.UTF8, MediaTypeNames.Text.Html);
                // Create a plain text message for client that don't support HTML
                string textBody = "You must use an e-mail client that supports HTML messages";
                AlternateView plainView = AlternateView.CreateAlternateViewFromString(Regex.Replace(textBody, "<[^>]+?>", string.Empty), Encoding.UTF8, MediaTypeNames.Text.Plain);
                string mediaType = MediaTypeNames.Image.Jpeg;
                LinkedResource img = new LinkedResource(path, mediaType);
                img.ContentId = "EmbeddedContent_AnniversaryCard";

                img.ContentType.MediaType = mediaType;
                img.TransferEncoding = TransferEncoding.Base64;
                img.ContentType.Name = img.ContentId;
                img.ContentLink = new Uri("cid:" + img.ContentId);
                htmlView.LinkedResources.Add(img);
                msg.AlternateViews.Add(plainView);
                msg.AlternateViews.Add(htmlView);

                //foreach (string cc in ccEmail)
                //{
                //    msg.CC.Add(cc);
                //}
                //foreach (string bcc in bccEmail)
                //{
                //    msg.Bcc.Add(bcc);
                //}
               
                msg.IsBodyHtml = true;
                msg.Subject = "Happy " + anniversary;
                client.Send(msg);
            }
            else
            {
                string path2 = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/Happy_Birthday_Card.jpg");
                Bitmap bitmap = (Bitmap)Image.FromFile(path2);//load the image file
                var fileName = Path.GetFileName(path2);
                var path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/FileUpload/Aniversary"), name + "_" + fileName);//Save in temporary file

                //draw the image object using a Graphics object    
                Graphics graphicsImage = Graphics.FromImage(bitmap);

                //Set the alignment based on the coordinates       
                StringFormat stringformat = new StringFormat();
                stringformat.Alignment = StringAlignment.Center;
                stringformat.LineAlignment = StringAlignment.Center;

                StringFormat stringformat2 = new StringFormat();
                stringformat2.Alignment = StringAlignment.Center;
                stringformat2.LineAlignment = StringAlignment.Center;

                //Set the font color/format/size etc..      
                System.Drawing.Color StringColor = ColorTranslator.FromHtml("#933eea");//direct color adding    
                System.Drawing.Color StringColor2 = ColorTranslator.FromHtml("#e80c88");//customise color adding 
                System.Drawing.Color smsColor = ColorTranslator.FromHtml("#581845");//customise color adding   
                string Str_TextOnImage = "Happy \nBirth Anniversary \nto ";//Your Text On Image    

                string Str_TextOnImage2 = name + "\n" + designation + "\n" + department;//Your Text On Image    
                string birthdaySms = "On your birthday (Date: " + eventDate + ")\nWe wish you good luck. We hope \nthis wonderfull day fill up your \nheart with joy and blessings.";//Your Text On Image    

                graphicsImage.DrawString(Str_TextOnImage, new Font("arial", 16,
                FontStyle.Regular), new SolidBrush(StringColor), new Point(1125, 380),
                stringformat); 
                //Response.ContentType = "image/jpeg";

                graphicsImage.DrawString(Str_TextOnImage2, new Font("Helvetica", 12,
                FontStyle.Bold), new SolidBrush(StringColor2), new Point(1125, 600),
                stringformat2);
                //Response.ContentType = "image/jpeg";

                graphicsImage.DrawString(birthdaySms, new Font("arial", 10,
                FontStyle.Bold), new SolidBrush(smsColor), new Point(1125, 799),
                stringformat2); 
                //Response.ContentType = "image/jpeg";
                bitmap.Save(path, ImageFormat.Png);
                bitmap.Dispose(); //bitmap.Save(Response.OutputStream, ImageFormat.Jpeg);//Display image in Browser

                MailMessage msg = new MailMessage(senderEmail, receiverEmail);
                // Create the HTML view
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlMessage, Encoding.UTF8, MediaTypeNames.Text.Html);
                // Create a plain text message for client that don't support HTML
                string textBody = "You must use an e-mail client that supports HTML messages";
                AlternateView plainView = AlternateView.CreateAlternateViewFromString(Regex.Replace(textBody, "<[^>]+?>", string.Empty), Encoding.UTF8, MediaTypeNames.Text.Plain);
                string mediaType = MediaTypeNames.Image.Jpeg;
                LinkedResource img = new LinkedResource(path, mediaType);
                img.ContentId = "EmbeddedContent_AnniversaryCard";

                img.ContentType.MediaType = mediaType;
                img.TransferEncoding = TransferEncoding.Base64;
                img.ContentType.Name = img.ContentId;
                img.ContentLink = new Uri("cid:" + img.ContentId);
                htmlView.LinkedResources.Add(img);
                msg.AlternateViews.Add(plainView);
                msg.AlternateViews.Add(htmlView);

                //foreach (string cc in ccEmail)
                //{
                //    msg.CC.Add(cc);
                //}
                //foreach (string bcc in bccEmail)
                //{
                //    msg.Bcc.Add(bcc);
                //}
                msg.IsBodyHtml = true;
                msg.Subject = "Happy " + anniversary;
                client.Send(msg);
            }
        }

        #endregion

        
        [HttpGet]
        public ActionResult TeamMemberIndex(int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            List<EmployeeModel> employees = employeeService.GetTeamMembers(searchText);
            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(employees.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        
        [HttpGet]
        public ActionResult TeamMemberEdit(long id)
        {
            EmployeeModel member = employeeService.GetTeamMember(id);
            return View(member);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeamMemberEdit(EmployeeModel model)
        {
            bool result = false;
            result = employeeService.UpdateTeamMember(model);
            if (result)
            {
                TempData["successMessage"] = "Operation Successful !";
                return RedirectToAction("TeamMemberIndex");
            }
            return View(model);
        }

        
        [HttpGet]
        public ActionResult EmployeeAdvanceSearch(int? departmentId, int? designationId, string searchText)
        {
            departmentId = departmentId ?? 0;
            designationId = designationId ?? 0;
            ViewBag.departments = departmentService.GetDepartmentSelectListModels();
            ViewBag.designations = designationService.GetDesignationSelectListModels();
            List<EmployeeModel> employees = employeeService.GetEmployeeAdvanceSearch(departmentId, designationId, searchText);
            return View(employees);
        }

        //----ani----

        
        [HttpGet]
        public async Task<ActionResult> SignatureList(int CompanyId)
        {
            EmployeeVm model = new EmployeeVm();
            model = await employeeService.GetEmployeesCompanyWise(CompanyId);
            return View(model);
        }

        
        [HttpGet]
        public async Task<ActionResult> EmployeeListForJobDescription(int CompanyId)
        {
            EmployeeVm model = new EmployeeVm();
            model = await employeeService.GetAllEmployeesWise();
            return View(model);
        }
        
        [HttpGet]
        public async Task<ActionResult> BankAccList(int CompanyId)
        {
            EmployeeVm model = new EmployeeVm();
            model = await employeeService.GetEmployeesCompanyWise(CompanyId);
            return View(model);
        }
        
        [HttpGet]
        public ActionResult GetBankAcc(long EmpID)
        {
            EmployeeModel model = new EmployeeModel();
            model = employeeService.GetEmployeeForAcc(EmpID);
            return View(model);
        }

        
        [HttpGet]
        public  ActionResult GetSignature(long EmpID)
        {
            int companyId = Common.GetCompanyId();
            EmployeeModel model = new EmployeeModel();
            model =  employeeService.GetEmployee(EmpID, companyId);
            
            return View(model);
        }
        
        [HttpPost]
        public ActionResult SignatureAdd(EmployeeModel vm, HttpPostedFileBase image)
        {
            try
            {
                ModelState.Clear();
                bool result = false;
                string picture = string.Empty;

                if (image != null)
                {
                    var supportedTypes = new[] { "jpg", "jpeg", "png", "bmp" };
                    var fileExt = Path.GetExtension(image.FileName).Substring(1);

                    if (!supportedTypes.Contains(fileExt, StringComparer.OrdinalIgnoreCase))
                    {
                        string errorMessage = "Unsupported file type.";
                        throw new Exception(errorMessage);
                    }

                
                    string employeeName = vm.Name.Replace(" ", "_"); 
                    string extension = Path.GetExtension(image.FileName);

                   
                    picture = $"{employeeName}_{vm.EmployeeId}.{extension}";
                    string fullPath = Path.Combine(Server.MapPath("~/Images/Picture"), picture);

                    int count = 1;
                    while (System.IO.File.Exists(fullPath))
                    {
                      
                        picture = $"{employeeName}_{vm.EmployeeId}({count++}){extension}";
                        fullPath = Path.Combine(Server.MapPath("~/Images/Picture"), picture);
                    }

                 
                    image.SaveAs(fullPath);

                    
                    vm.SignatureFileName = picture;
                }

              
                result = employeeService.savesignature(vm);



                return RedirectToAction("SignatureList", new { CompanyId = vm.CompanyId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return RedirectToAction("SignatureList", new { CompanyId = vm.CompanyId });

            }
        }
        
        [HttpPost]
        public ActionResult AccoountNumAdd(EmployeeModel vm)
        {
            try { 
                bool result = false;
                result = employeeService.saveAccountNumber(vm);
                return RedirectToAction("BankAccList", new { CompanyId = vm.CompanyId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return RedirectToAction("BankAccList", new { CompanyId = vm.CompanyId });

            }
        }


        [HttpGet]
        
        public ActionResult EmployeeIDCard(string searchText, string ReportName, string ReportDescription)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                var rptInfo = new ReportInfo
                {
                    ReportName = "",
                    ReportDescription = "description",
                    ReportURL = String.Format("../../Reports/EmployeeIDCardReport.aspx?searchText={0}&Height={1}", searchText, 800),
                    Width = 700,
                    Height = 950
                };

                return View(rptInfo);
            }
            else
            {
                ReportInfo rptInfo = new ReportInfo();
                return View(rptInfo);
            }
        }
        #region Exit InterView
        [HttpGet]
        public ActionResult ExitInterview()
        {
            var userId = Common.GetUserId();

            var data = employeeService.GetAllExitInterView(userId);

            return View(data);

        }

        [HttpGet]
        public ActionResult ClearanceEntry()
        {
            var userId = Common.GetUserId();
            var data = employeeService.GetAllExitInterView("").ToList();
            return View(data);

        }


        [HttpGet]
        public ActionResult ExitInterviewDetail(int id)
        {           

            ExitInterviewVM exitInterviewVM = employeeService.GetExitInterView(id);
            
            return View(exitInterviewVM);

        }
        [HttpGet]
        public ActionResult CreateExitInterview(int? companyId, int id=0, ActionEnum actionType = ActionEnum.Add)
        {
            var exitInterviewVM = employeeService.GetExitInterView(id);
            exitInterviewVM.ActionType = actionType;
            return View(exitInterviewVM);

        }
        [HttpPost]
        public ActionResult CreateExitInterview(ExitInterviewVM modelVM)
        {
            // ExitInterviewModel model = new ExitInterviewModel();
            if (modelVM.ActionType == ActionEnum.Add)
            {
                var result =  employeeService.SaveExitInterView(modelVM);
               
            }
            if (modelVM.ActionType == ActionEnum.Edit)
            {
                employeeService.UpdateExitInterView(modelVM);
            }
            if (modelVM.ActionType == ActionEnum.Delete)
            {
                employeeService.DeleteExitInterView((int)modelVM.Id);
            }
            return RedirectToAction("ExitInterview");
        }

        [HttpGet]
        public ActionResult ExitInterviewClearanceMappingAdd(int id, string employeeId,DateTime? resignDate, bool isWithoutExitInterview=false)
        {
            // ExitInterviewModel model = new ExitInterviewModel();
            var userId = Common.GetUserId();
            if (isWithoutExitInterview && !string.IsNullOrEmpty(employeeId))
            {

                var isExist = db.ExitInterviews.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);
                if (isExist != null)
                {
                    TempData["error"] = "Sorry! Already exist.";
                    return RedirectToAction("ExitInterview");
                }
                ExitInterview model = new ExitInterview()
                {
                    EmployeeId = employeeId,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now,
                    ExpectedResignDate = resignDate,
                    ReasonForLeaving = "Entry without feedback session.",
                    MotivationForJobChanges = "",
                    JobSearchStartFrom = "",
                    AttractiveFactors = "",
                    CareerGoalDiscussion = "",

                    JobEnjoyment = "",
                    JobChallenges = "",
                    PositiveAspectsOfWorkPlace = "",
                    NegativeAspectsOfWorkPlace = "",
                    WorkplaceImprovementSuggestions = "",
                    CareerGoalsElseWhere = "",
                    Status =(int) ApprovalStatusEnum.Submitted

                };
                var result = db.ExitInterviews.Add(model);
                try
                {
                    if (db.SaveChanges() > 0)
                    {
                        TempData["success"] = "Successully saved.";
                        var saveMapping = employeeService.SaveClearanceSignatoryMapping(model.Id);
                    }

                }
                catch (Exception e)
                {
                    TempData["error"] = "Sorry! Something went wrong";
                    return RedirectToAction("ExitInterview");
                }
              
                var mappingResult = employeeService.SaveClearanceSignatoryMapping(id);
                return RedirectToAction("ExitInterview");
            }
            if (id > 0)
            {
                var exist = employeeService.GetExitInterView(id);
                if (exist != null && exist.Status == ApprovalStatusEnum.Draft)
                {

                    var mappingResult = employeeService.SaveClearanceSignatoryMapping(id);
                }

            }
            return RedirectToAction("ExitInterview");
        }


        [HttpGet]
        public ActionResult DeleteExitInterview(int id)
        {
            var result = employeeService.DeleteExitInterView(id);
            return RedirectToAction("ExitInterview");

        }
        [HttpGet]
        public ActionResult ClearanceSignatory()
        {
            BusinessHeadModel model = new BusinessHeadModel();
            var data = employeeService.GetClearanceSignatory(null,null);
            if (data != null)
            {
                model.DataList = data.ToList();
            }
            model.ActionType = ActionEnum.Add;
            model.ToDate = null;
            model.FromDate = DateTime.Today;
            return View(model);
        }
        [HttpPost]
        public ActionResult ClearanceSignatory(BusinessHeadModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.ActionType == ActionEnum.Add)
                {
                    var result = employeeService.SaveClearanceSignatory(model);
                }
                if (model.ActionType == ActionEnum.Edit)
                {
                    var result = employeeService.UpdateClearanceSignatory(model);
                }
            }
            return RedirectToAction("ClearanceSignatory");
        }

        [HttpGet]
        public ActionResult GetExitInterviewApprovalList(int id)
        {            
            var approvalList = employeeService.GetAllExitInterviewApprovalList(id);
            return Json(approvalList.ToList(), JsonRequestBehavior.AllowGet);
        }       

        [HttpGet]
        public ActionResult RemoveClearanceSignatory(int id)
        {
            if (id > 0)
            {
                var result = employeeService.RemoveClearanceSignatory(id);
            }
            return RedirectToAction("ClearanceSignatory");
        }


        [HttpGet]
        public ActionResult GetClearanceSignatoryById(int id)
        {
            var data = employeeService.GetClearanceSignatoryById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ClearanceSignatoryApproval(int companyId, SignatoryStatusEnum? SignatoryStatus, DateTime? fromDate, DateTime? toDate)
        {
            
            var data = employeeService.GetClearanceApprovalData(fromDate, toDate, SignatoryStatus);
            return View(data);
        }
        [HttpPost]
        public ActionResult UpdateClearanceSignatoryStatus(EmployeeClearanceVM model)
        {
            var result = employeeService.UpdateClearanceSignatoryApprovalStatus(model.Id, model.SignatoryStatus, model.Comment);
            return RedirectToAction("ClearanceSignatoryApproval", new { companyId = model.UserCompanyId });
        }

        #endregion Exit InterVIew

        #region HRMS Report 20210316
        //vm.Managers = employeeService.GetEmployeeSelectModels();
        //vm.Companies = companyService.GetCompanySelectModels();
        //vm.Religions = dropDownItemService.GetDropDownItemSelectModels(9);
        //vm.BloodGroups = dropDownItemService.GetDropDownItemSelectModels(5);  
        //vm.MaritalTypes = dropDownItemService.GetDropDownItemSelectModels(2);
        //vm.Genders = dropDownItemService.GetDropDownItemSelectModels(3);
        //vm.EmployeeCategories = dropDownItemService.GetDropDownItemSelectModels(8);
        //vm.Departments = departmentService.GetDepartmentSelectModels();
        //vm.Designations = designationService.GetDesignationSelectModels();
        //vm.OfficeTypes = dropDownItemService.GetDropDownItemSelectModels(12); 
        //vm.JobStatus = dropDownItemService.GetDropDownItemSelectModels(15);
        //vm.JobTypes = dropDownItemService.GetDropDownItemSelectModels(10);

        #region 5. Gender Wise Report 20210402
        [HttpGet]
        
        public ActionResult GenderWiseReport(string gender)
        {
            ViewBag.Genders = dropDownItemService.GetDropDownItemSelectModels(3);

            if (!string.IsNullOrEmpty(gender))
            {
                var rptInfo = new ReportInfo
                {
                    ReportName = "",
                    ReportDescription = "description",
                    ReportURL = String.Format("../../Reports/GenderWiseReport.aspx?gender={0}&Height={1}", gender, 800),
                    Width = 700,
                    Height = 950
                };
                return View(rptInfo);
            }
            else
            {
                //ViewBag.NoSelect = "Please select District or Upzilla !";
                ReportInfo rptInfo = new ReportInfo();
                return View(rptInfo);
            }
        }
        #endregion

        #region 4. Religion Wise Report 20210402
        [HttpGet]
        
        public ActionResult ReligionWiseReport(string religion)
        {
            ViewBag.Religions = dropDownItemService.GetDropDownItemSelectModels(9);

            if (!string.IsNullOrEmpty(religion))
            {
                var rptInfo = new ReportInfo
                {
                    ReportName = "",
                    ReportDescription = "description",
                    ReportURL = String.Format("../../Reports/ReligionWiseReport.aspx?religion={0}&Height={1}", religion, 800),
                    Width = 700,
                    Height = 950
                };
                return View(rptInfo);
            }
            else
            {
                //ViewBag.NoSelect = "Please select District or Upzilla !";
                ReportInfo rptInfo = new ReportInfo();
                return View(rptInfo);
            }
        }
        #endregion

        #region 4. Office Type Wise Report 20210402
        [HttpGet]
        
        public ActionResult OfficeTypeWiseReport(string officeType)
        {
            ViewBag.OfficeTypes = dropDownItemService.GetDropDownItemSelectModels(12);

            if (!string.IsNullOrEmpty(officeType))
            {
                var rptInfo = new ReportInfo
                {
                    ReportName = "",
                    ReportDescription = "description",
                    ReportURL = String.Format("../../Reports/OfficeTypeWiseReport.aspx?officeType={0}&Height={1}", officeType, 800),
                    Width = 700,
                    Height = 950
                };
                return View(rptInfo);
            }
            else
            {
                //ViewBag.NoSelect = "Please select District or Upzilla !";
                ReportInfo rptInfo = new ReportInfo();
                return View(rptInfo);
            }
        }
        #endregion

        #region 3. Department/ Division Wise Report
        [HttpGet]
        
        public ActionResult DepartmentWiseReport(string department)
        {
            ViewBag.Departments = departmentService.GetDepartmentSelectModels();

            if (!string.IsNullOrEmpty(department))
            {
                var rptInfo = new ReportInfo
                {
                    ReportName = "",
                    ReportDescription = "description",
                    ReportURL = String.Format("../../Reports/DepartmentWiseReport.aspx?department={0}&Height={1}", department, 800),
                    Width = 700,
                    Height = 950
                };
                return View(rptInfo);
            }
            else
            {
                //ViewBag.NoSelect = "Please select District or Upzilla !";
                ReportInfo rptInfo = new ReportInfo();
                return View(rptInfo);
            }
        }
        #endregion

        #region 2. Blood Group Wise Report
        [HttpGet]
        
        public ActionResult BloodGroupWiseReport(string bloodGroup)
        {
            ViewBag.BloodGroups = dropDownItemService.GetDropDownItemSelectModels(5);

            if (!string.IsNullOrEmpty(bloodGroup))
            {
                var rptInfo = new ReportInfo
                {
                    ReportName = "",
                    ReportDescription = "description",
                    ReportURL = String.Format("../../Reports/BloodGroupWiseReport.aspx?bloodGroup={0}&Height={1}", bloodGroup, 800),
                    Width = 700,
                    Height = 950
                };
                return View(rptInfo);
            }
            else
            {
                //ViewBag.NoSelect = "Please select District or Upzilla !";
                ReportInfo rptInfo = new ReportInfo();
                return View(rptInfo);
            }
        }
        #endregion

        #region 1. District Or Division Or Upzilla WiseReport
        [HttpGet]
        
        public ActionResult DistrictOrDivisionOrUpzillaWiseReport(string Division, string District, string Upzilla, bool? active)
        {
            ViewBag.Divisions = districtService.GetDivisionSelectModels();

            if (!string.IsNullOrEmpty(District) && !string.IsNullOrEmpty(Division))
            {
                ViewBag.Districts = districtService.GetDistrictByDivisionName(Division);
                ViewBag.Upzillas = upazilaService.GetUpzilaByDistrictName(District);
            }
            else
            {
                ViewBag.Districts = new List<SelectModel>();
                ViewBag.Upzillas = new List<SelectModel>();
            }

            if (!string.IsNullOrEmpty(District))
            {
                var rptInfo = new ReportInfo
                {
                    ReportName = "",
                    ReportDescription = "description",
                    ReportURL = String.Format("../../Reports/DistrictOrDivisionOrUpzillaWiseReport.aspx?District={0}&Height={1}", District, 800),
                    Width = 700,
                    Height = 950
                };

                return View(rptInfo);
            }
            else if (string.IsNullOrEmpty(District))
            {
                ReportInfo rptInfo = new ReportInfo();
                return View(rptInfo);

            }
            else
            {
                ViewBag.NoSelect = "Please select District or Upzilla !";
                ReportInfo rptInfo = new ReportInfo();
                return View(rptInfo);
            }
        }

        //Fetching District Data base on Division Name-Ashraf
        [HttpPost]
        public ActionResult GetDistrictByDivisionName(string name)
        {
            List<SelectModel> Districts = districtService.GetDistrictByDivisionName(name);
            return Json(Districts, JsonRequestBehavior.AllowGet);
        }


        //Fetching Upzilla Data base on District Name-Ashraf
        [HttpPost]
        public ActionResult GetUpzilaByDistrictName(string name)
        {
            List<SelectModel> Upazilas = upazilaService.GetUpzilaByDistrictName(name);
            return Json(Upazilas, JsonRequestBehavior.AllowGet);
        }


        //Fetching District Data base on Division Name-Ashraf
        [HttpPost]
        public ActionResult GetDistrictByDivisionId(int? divisionId)
        {
            List<SelectModel> Districts = districtService.GetDistrictByDivisionId(divisionId);
            return Json(Districts, JsonRequestBehavior.AllowGet);
        }


        //Fetching Upzilla Data base on District Name-Ashraf
        [HttpPost]
        public ActionResult GetUpzilaByDistrictId(int? districtId)
        {
            List<SelectModel> Upazilas = upazilaService.GetUpzilaByDistrictId(districtId);
            return Json(Upazilas, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion
    }
}
