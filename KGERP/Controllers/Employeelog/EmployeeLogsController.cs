using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.EmployeeLogService;
using KGERP.Service.Implementation.FTP;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.FTP_Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.Employeelog
{
    [CheckSession]
    public class EmployeeLogsController : Controller
    {
        private readonly EmployeeLogServices logServices;
        private readonly IGLDLCustomerService gLDLCustomerService;
        private IFTPService _ftpservice;
        IEmployeeService employeeService = new EmployeeService(new ERPEntities());
        public EmployeeLogsController(EmployeeLogServices logServices, IFTPService ftpservice,IGLDLCustomerService gLDLCustomerService)
        {
            this.logServices = logServices;
            this._ftpservice = ftpservice;
            this.gLDLCustomerService = gLDLCustomerService;
        }
        // GET: EmployeeLogs
        public async  Task<ActionResult> Index()
        {
            EmployeeViewModel model = new EmployeeViewModel();
        
            model.logdate = DateTime.Now;
            model.EmployeeLogTypes = new SelectList(await logServices.EmployeeLogType(), "Value", "Text");
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Index(EmployeeViewModel model)
        {

            var result= await logServices.Addlog(model);
            if (result.Id > 0)
            {
                if (model.Attachments == null)
                {
                    return RedirectToAction("Index");
                }
                List<FileItem> itemlist = new List<FileItem>();

                if (model.Attachments[0]!=null)
                {
                    for (int i = 0; i < model.Attachments.Count(); i++)
                    {

                        itemlist.Add(new FileItem
                        {
                            file = model.Attachments[i],
                            docdesc = model.Attachments[i].FileName,
                            docid = 0,
                            FileCatagoryId = 9,
                            fileext = Path.GetExtension(model.Attachments[i].FileName),
                            docfilename = Guid.NewGuid().ToString() + Path.GetExtension(model.Attachments[i].FileName),
                            isactive = true,
                            RecDate = DateTime.Now,
                            SortOrder = i,
                            userid = Convert.ToInt32(Session["Id"])
                        });

                    }
                    itemlist = await _ftpservice.UploadFileBulk(itemlist, result.Id.ToString());
                    long CGId = Convert.ToInt64(model.Id);
                    var result1 = await gLDLCustomerService.FileMapping(itemlist, CGId);
                    return RedirectToAction("Details", "Employee", new { employeeId = model.EmpId });

                }

                return RedirectToAction("Details", "Employee", new { employeeId = model.EmpId });

            }
            return View(model);
        }
        

        public async Task<JsonResult> getloglistByEmp(long EmpId)
        {
            var result = await logServices.logList(EmpId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> DeleteEmployeeLogsById(long EmpLogsId)
        {
            var result = await logServices.DeleteEmpLogsById(EmpLogsId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> getAttatchment(long id)
        {
            var result = await logServices.logAttactmentbyId(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public ActionResult Details(long id)
        {

            EmployeeModel model = employeeService.GetEmployee(id);



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
    }
}