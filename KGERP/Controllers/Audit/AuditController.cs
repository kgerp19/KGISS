using DocumentFormat.OpenXml.EMMA;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation.Audit;
using KGERP.Service.Implementation.Audit.ViewModels;
using KGERP.Service.Implementation.FTP;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.FTP_Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace KGERP.Controllers.Audit
{
    [CheckSession]
    public class AuditController : Controller
    {
        
        private IAuditService _auditService;
        private IFTPService _ftpservice;
        private readonly IGLDLCustomerService gLDLCustomerService;
        public AuditController(IAuditService auditService, IFTPService ftpservice, IGLDLCustomerService gLDLCustomerService)
        {
            _auditService = auditService;
            this._ftpservice = ftpservice;
            this.gLDLCustomerService = gLDLCustomerService;
        }
        // GET: Audit
        [HttpGet]
        public ActionResult Index(int? companyId, int? year,int? month,int? type, int companyId2 = 0)
        {
            PreservingAuditDocumentVM model = new PreservingAuditDocumentVM();
            var data = _auditService.GetAllAuditDocument(companyId, year, month, type, companyId2);

            if (data != null)
            {
                model.DataList = data.ToList();
            }
            return View(model);
        }



        [HttpGet]
        public ActionResult ListIndex2(int? companyId, int? year, int? month, int? type, int companyId2 = 0)
        {
            PreservingAuditDocumentVM model = new PreservingAuditDocumentVM();
            var data = _auditService.GetAllAuditDocument(companyId, year, month, type, companyId2);

            if (data != null)
            {
                model.DataList = data.ToList();
            }
            return View(model);
        }

        [HttpPost]
        
        public async Task<ActionResult> Index(PreservingAuditDocumentVM model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }

            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);


            return RedirectToAction(nameof(Index), new { companyId = 0, Year = model.Year, Month = model.Month, Type =(int)model.Type, companyId2 = model.CompanyId });

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> FileUpload(int Id)
        {
            PreservingAuditDocumentVM model = new PreservingAuditDocumentVM();
            model = await Task.Run(() => _auditService.GetAuditDocumentFileAsync(Id));
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> FileUpload2(int Id)
        {
            PreservingAuditDocumentVM model = new PreservingAuditDocumentVM();
            model = await Task.Run(() => _auditService.GetAuditDocumentFileAsync(Id));
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> CommonAuditFileuplode(PreservingAuditDocumentVM model)
        {

            if (model.Attachments == null)
            {
                return RedirectToAction("FileUpload", new { Id = model.Id });
            }
            List<FileItem> itemlist = new List<FileItem>();
            for (int i = 0; i < model.Attachments.Count(); i++)
            {
                if (model.Attachments[i].DocId == 0)
                {
                    itemlist.Add(new FileItem
                    {
                        file = model.Attachments[i].File,
                        docdesc = model.Attachments[i].Title,
                        docid = 0,
                        FileCatagoryId = 7,
                        fileext = Path.GetExtension(model.Attachments[i].File.FileName),
                        docfilename = Guid.NewGuid().ToString() + Path.GetExtension(model.Attachments[i].File.FileName),
                        isactive = true,
                        RecDate = DateTime.Now,
                        SortOrder = i,
                        userid = Convert.ToInt32(Session["Id"])
                    });
                }
            }
            itemlist = await _ftpservice.UploadFileBulk(itemlist, model.Id.ToString());
            long CGId = Convert.ToInt64(model.Id);
            var result = await gLDLCustomerService.FileMapping(itemlist, CGId);
            return RedirectToAction("FileUpload", new { Id = model.Id });
        }



        //[HttpGet]
        //public ActionResult LoadAuditDocument(DateTime? fromDate, DateTime? toDate,int? type,int CompanyId)
        //{
        //    PreservingAuditDocumentVM model = new PreservingAuditDocumentVM();
        //    var data = _auditService.GetAllAuditDocument(fromDate,toDate,type);
        //    if (data != null && data.Count() > 0)
        //    {
        //        model.DataList = data.ToList();
        //    }
        //    return View("Index", model);
        //}


   




        [HttpGet]
        public ActionResult AddOrUpdateAuditDocument(int? id)
        {

            if (id != null)
            {
                var modelVM = _auditService.GetAuditDocumentById(id.Value);
                if (modelVM != null)
                {
                    return View(modelVM);
                }
            }
            PreservingAuditDocumentVM model = new PreservingAuditDocumentVM();
            var data = _auditService.GetAllAuditDocument(null,null,null,null);
            if (data != null)
            {
                model.DataList = data.ToList();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddOrUpdateAuditDocument(PreservingAuditDocumentVM modelVM)
        {
            if (ModelState.IsValid)
            {
                if (modelVM.ActionEnum == Utility.ActionEnum.Add)
                {

                    var result = _auditService.AddAuditDocument(modelVM);
                }
                if (modelVM.ActionEnum == Utility.ActionEnum.Edit)
                {

                    var result = _auditService.UpdateAuditDocument(modelVM);
                }
                if (modelVM.ActionEnum == Utility.ActionEnum.Delete)
                {
                    var result = _auditService.DeleteAuditDocument(modelVM.Id);
                }
            }
            return RedirectToAction("FileUpload", new { Id = modelVM.Id });
        }

        [HttpPost]
        public ActionResult RemoveAuditDocument(long id)
        {
            var result = _auditService.DeleteAuditDocument(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GetAuditDocumentById(long id)
        {
            var data = _auditService.GetAuditDocumentById(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}