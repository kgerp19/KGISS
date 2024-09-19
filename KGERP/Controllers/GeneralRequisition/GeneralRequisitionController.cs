using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.VariantTypes;
using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.FTP;
using KGERP.Service.Implementation.General_Requisition;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.FTP_Models;
using KGERP.Services.Procurement;
using KGERP.Utility;
using KGERP.Utility.Extensions;
using KGERP.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace KGERP.Controllers.GeneralRequisition
{
    [CheckSession]
    public class GeneralRequisitionController : Controller
    {
        //test
        private readonly ERPEntities _db;
        private readonly IGeneralRequisitionService _generalRequistionService;
        private IFTPService _ftpservice;
        private readonly ConfigurationService _service;
        public GeneralRequisitionController(ERPEntities db, IGeneralRequisitionService generalRequisitionService,IFTPService fTPService, ConfigurationService configurationService)
        {
            _db = db;
            _generalRequistionService = generalRequisitionService;
            _ftpservice = fTPService;
            _service = configurationService;
        }
        [HttpGet]
        public async Task<ActionResult> Index(int companyId, DateTime? fromDate, DateTime? toDate,GeneralRequisitionStatusEnum? Status)
        {           
            if (companyId > 0)
            {
                ViewBag.CompanyId = companyId;
            }
            else
            {
                ViewBag.CompanyId = 0;
            }
            if (fromDate == null) fromDate = DateTime.Today.AddMonths(-2);
            if (toDate == null) toDate = DateTime.Today;
            var generalRequisitionList = await  _generalRequistionService.GetAllAsync(companyId, fromDate, toDate,Status);
            return View(generalRequisitionList);
        }
        [HttpGet]
        public async Task<ActionResult> RequistionList(int companyId, DateTime? fromDate, DateTime? toDate, int? Status)
        {
            if (companyId > 0)
            {
                ViewBag.CompanyId = companyId;
            }
            else
            {
                ViewBag.CompanyId = 0;
            }
            if (fromDate == null)
            {
                fromDate = DateTime.Today.AddMonths(-2);
            }
            else
            {
                fromDate = fromDate;
            }
            if (toDate == null)
            { 
                toDate = DateTime.Today;
            }
            else
            {
                toDate = toDate;
            }
            var generalRequisitionList = await _generalRequistionService.GetAllRequisitionAsync(companyId, fromDate, toDate , Status);
            return View(generalRequisitionList);
        }
        [HttpGet]
        public async Task<ActionResult> GetAllApprovedList(int companyId)
        {
            ViewBag.CompanyId = companyId;
            var fromDate = DateTime.Today.AddMonths(-2);
            var  toDate = DateTime.Today;
            var userId = Common.GetIntUserId();
            ERPEntities _context = new ERPEntities();
            int? cId = companyId;
            var financeDiractor = _context.Employees.FirstOrDefault(x => x.DesignationId == 177 && x.Active).Id;
            if (financeDiractor == userId)
            {
                cId = null;
            }
            var generalRequisitionList = await _generalRequistionService.GetAllAsync(cId, fromDate, toDate, GeneralRequisitionStatusEnum.Approved);
            return View(generalRequisitionList);
        }
        [HttpPost]
        public async Task<ActionResult> GetAllApprovedList(int companyId, DateTime? fromDate, DateTime? toDate, GeneralRequisitionStatusEnum? Status)
        {
            ViewBag.CompanyId = companyId;
            if (fromDate == null) fromDate = DateTime.Today.AddMonths(-2);
            if (toDate == null) toDate = DateTime.Today;
            var userId = Common.GetIntUserId();
            ERPEntities _context = new ERPEntities();
            int? cId = companyId;
            var financeDiractor = _context.Employees.FirstOrDefault(x => x.DesignationId == 177 && x.Active).Id;
            if (financeDiractor == userId)
            {
                cId = null;
            }
            var generalRequisitionList = await _generalRequistionService.GetAllAsync(cId, fromDate, toDate, GeneralRequisitionStatusEnum.Approved);
            return View("GetAllApprovedList",generalRequisitionList);
        }

        [HttpGet]
        public async Task<ActionResult> Approval(int companyId,DateTime? fromDate , DateTime? toDate,SignatoryStatusEnum? RequisitionSignatoryStatus)
        {
           var userId =  Common.GetIntUserId();
            ViewBag.CompanyId = companyId;
             //userId = 1028;
            if (fromDate == null) fromDate = DateTime.Today.AddMonths(-2);
            if (toDate == null) toDate = DateTime.Today;
            var requisitionList = _generalRequistionService.GetAllRequisitionApprovalList(companyId,fromDate,toDate,userId,RequisitionSignatoryStatus);
            return View(requisitionList);

        }
        [HttpGet]
        public async Task<ActionResult> UpdateApprovalStatus(int companyId, int requisitionSignatoryId, SignatoryStatusEnum status,string comment)
        {
            var result = await _generalRequistionService.UpdateRequisitionSignatoryApprovalStatus(requisitionSignatoryId, (int)status,comment);
            return RedirectToAction(nameof(Approval), new { companyId = companyId });

        }
        [HttpGet]
        public async Task<ActionResult> GetApprovalList(int requisitionId)
        {
            var approvalList = _generalRequistionService.GetAllApproval(requisitionId);
            return Json(approvalList.ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> GetEDocumentApprovalList(int requisitionId)
        {
            var approvalList = _generalRequistionService.GetAllEDocumentApprovalList(requisitionId);
            return Json(approvalList.ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> AddOrUpdate(int companyId = 0, int requisitionId = 0)
        {
            
            GeneralRequisitionMasterVM requisition = new GeneralRequisitionMasterVM();
           
            if (requisitionId == 0)
            {
                requisition.UnitList = (await _service.GetUnit(companyId)).DataList;
                requisition.Status = (int)EnumPOStatus.Draft;
            }
            else if (requisitionId > 0)
            {
                requisition = await Task.Run(() => _generalRequistionService.Get(requisitionId));
                requisition.UnitList = (await _service.GetUnit(companyId)).DataList;
                if (requisition != null)
                {
                    if (requisition.Status != GeneralRequisitionStatusEnum.Draft)
                    {
                        requisition.RequisitionApprovalLIst = _generalRequistionService.GetAllApproval(requisitionId).ToList();
                    }
                }
            }
            requisition.UserCompanyId = companyId;
            return View(requisition);
        }
        [HttpPost]
        public async Task<ActionResult> AddOrUpdate(GeneralRequisitionMasterVM requisition)
        {
            if (ModelState.IsValid)
            {
                var userId = Common.GetUserId();                
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction(nameof(AddOrUpdate), new { companyId = requisition.UserCompanyId, requisitionId = requisition.Id });
                }
                //return RedirectToAction("AddOrUpdate");
                if (requisition.Id == 0)
                {
                    var emp = _db.Employees.FirstOrDefault(x => x.EmployeeId == userId);
                    requisition.CreatedBy = userId;
                    requisition.CreatedDate = DateTime.Now;
                    requisition.EemployeeId = emp.Id;
                    requisition.RequisitionNumber = await _generalRequistionService.GetGeneralRequisitionName((int)requisition.RequisitionToCompanyId, BusinessTypeEnum.Company);
                    requisition.RequisitionDate = DateTime.Today;
                    //if (requisition.GeneralRequisitionType == CompanyTypeEnum.Company)
                    //{

                    //    requisition.RequisitionToCompanyId = requisition.CommonId;
                    //    requisition.DivisionId = null;
                    //}
                    //if (requisition.GeneralRequisitionType == CompanyTypeEnum.Division)
                    //{
                    //    requisition.RequisitionToCompanyId = null;
                    //    requisition.DivisionId = requisition.CommonId;
                    //    requisition.ProjectId = null;
                    //}
                    requisition.GeneralRequisitionType =BusinessTypeEnum.Company;
                    if (requisition.RequisitionAssetType == RequisitionAssetTypeEnum.Asset)
                    {
                        requisition.IsAsset = true;
                    }

                    await _generalRequistionService.Add(requisition);
                    return RedirectToAction(nameof(AddOrUpdate), new { companyId = requisition.UserCompanyId, requisitionId = requisition.Id });
                }
                if (requisition.Id > 0 && requisition.RequisitionItemId == 0)
                {
                    if (requisition.Quantity > 0 && requisition.UnitPrice > 0)
                    {

                        requisition.CreatedBy = userId;
                        requisition.CreatedDate = DateTime.Now;
                        await _generalRequistionService.AddRequisitionItemDetail(requisition);
                    }
                }
                if (requisition.Id > 0 && requisition.RequisitionItemId > 0)
                {
                    requisition.ModifiedBy = userId;
                    requisition.ModifiedDate = DateTime.Now;
                    await _generalRequistionService.UpdateRequisitionDetail(requisition);
                }
            }

            return RedirectToAction(nameof(AddOrUpdate), new { companyId = requisition.UserCompanyId, requisitionId = requisition.Id });
        }
        [HttpPost]
        public async Task<ActionResult> Update(GeneralRequisitionMasterVM requisition)
        {
            int companyId = requisition.UserCompanyId;
            if (ModelState.IsValid && requisition.RequisitionToCompanyId > 0)
            {
                requisition.ModifiedBy = Common.GetUserId();
                if (requisition.ProjectId != null && requisition.ProjectId <= 0)
                {
                    requisition.ProjectId = null;
                }
                requisition.GeneralRequisitionType = BusinessTypeEnum.Company;
                if (requisition.RequisitionAssetType == RequisitionAssetTypeEnum.Asset)
                {
                    requisition.IsAsset = true;
                }
                requisition.ModifiedDate = DateTime.Now;
                var status =await _generalRequistionService.Update(requisition);
            }

            return RedirectToAction(nameof(Index), new { companyId = companyId });
        }

        [HttpPost]
        public async Task<ActionResult> UpdateStatus(GeneralRequisitionMasterVM requisition)
        {
            var result = await _generalRequistionService.UpdateStatus(requisition.Id, (int)requisition.Status);
            return RedirectToAction(nameof(AddOrUpdate), new { companyId = requisition.UserCompanyId, requisitionId = requisition.Id });

        }

        [HttpPost]
        public async Task<ActionResult> DeleteGeneralRequisition(int id = 0, int companyId = 0)
        {
            if (ModelState.IsValid)
            {
                var userId = Common.GetUserId();
                if (id > 0)
                {
                    await _generalRequistionService.Remove(id);
                }
            }

            return RedirectToAction(nameof(Index), new { companyId = companyId });
        }


        [HttpPost]
        public async Task<ActionResult> DeleteGeneralRequisitionItem(GeneralRequisitionMasterVM requisition)
        {
            if (ModelState.IsValid)
            {
                var userId = Common.GetUserId();
                if (requisition.RequisitionItemId > 0)
                {
                    requisition.ModifiedBy = userId;
                    requisition.ModifiedDate = DateTime.Now;
                    await _generalRequistionService.RemoveRequisitionDetail(requisition);
                }
            }

            return RedirectToAction(nameof(AddOrUpdate), new { companyId = requisition.UserCompanyId, requisitionId = requisition.Id });
        }

        public JsonResult GetGenralRequisitionProductListAutoCompleteJson(int companyId, string prefix)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                prefix = prefix.ToLower();
            }
            companyId = 26;
            string genralRequisitionProductType = Common.GetGeneralRequisitionProductType;
            var v = (from t1 in _db.Products
                     join t2 in _db.ProductSubCategories on t1.ProductSubCategoryId equals t2.ProductSubCategoryId
                     join t3 in _db.ProductCategories on t2.ProductCategoryId equals t3.ProductCategoryId

                     where t1.CompanyId == companyId && t3.ProductType == genralRequisitionProductType && t1.IsActive && t2.IsActive && t3.IsActive &&
                     ((t1.ProductName.ToLower().StartsWith(prefix)) || (t2.Name.ToLower().StartsWith(prefix)) || (t3.Name.ToLower().StartsWith(prefix)) || (t1.ShortName.ToLower().StartsWith(prefix)))
                     select new
                     {
                         label = t3.Name + " " + t2.Name + " " + t1.ProductName,
                         val = t1.ProductId,
                         price = t1.UnitPrice
                     }).OrderBy(x => x.label).Take(50).ToList();

            return Json(v, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> RequisitionCategory(int companyId = 0)
        {
            GeneralRequisitionProductCategoryVM categoryVM = new GeneralRequisitionProductCategoryVM();
            var data = await _generalRequistionService.GetAllRequisitionCategoryAsync();
            if (data != null)
            {
                categoryVM.DataList = data;
            }
            return View(categoryVM);
        }

        [HttpPost]
        public async Task<ActionResult> RequisitionCategory(GeneralRequisitionProductCategoryVM categoryVM)
        {
            //if (!ModelState.IsValid || categoryVM == null || !Enum.IsDefined(typeof(ActionEnum),categoryVM.ActionId))
            //{
            //    return RedirectToAction(nameof(RequisitionCategory));
            //}
            if (categoryVM.CategoryId == 0 && categoryVM.ActionId == ActionEnum.Add)
            {
                categoryVM.CreatedDate = DateTime.Now;
                categoryVM.CreatedBy = Common.GetUserId();
                var result = await _generalRequistionService.AddRequisitionCategory(categoryVM);
                return RedirectToAction(nameof(RequisitionCategory));
            }
            if (categoryVM.CategoryId > 0 && categoryVM.ActionId == ActionEnum.Edit)
            {
                categoryVM.ModifiedDate = DateTime.Now;
                categoryVM.ModifiedBy = Common.GetUserId();
                var result = await _generalRequistionService.UpdateRequisitionCategory(categoryVM);
                return RedirectToAction(nameof(RequisitionCategory));
            }
            if (categoryVM.CategoryId > 0 && categoryVM.ActionId == ActionEnum.Delete)
            {
                categoryVM.ModifiedDate = DateTime.Now;
                categoryVM.ModifiedBy = Common.GetUserId();
                var result = await _generalRequistionService.RemoveGeneralRequisitionCategory(categoryVM.CategoryId);
                return RedirectToAction(nameof(RequisitionCategory));
            }
            return RedirectToAction(nameof(RequisitionCategory));
        }
        [HttpPost]
        public async Task<ActionResult> DeleteRequisitionCategory(int id)
        {
            if (id > 0)
            {
                var result = _generalRequistionService.RemoveGeneralRequisitionCategory(id);
            }
            return RedirectToAction(nameof(RequisitionCategory));
        }
        #region Signatory
        [HttpGet]
        public async Task<ActionResult> RequisitionSignatory(int companyId = 0, string integrateWith = "")
        {
            var data =await _generalRequistionService.GetAllRequisitionSignatoryByCompany(companyId, integrateWith);
            var model = new RequisitionSignatoryVM();
            model.UserCompanyId = companyId;
            model.ActionId = ActionEnum.Add;
            model.DataList = data.ToList();
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> LoadRequisitionSignatoryList(int companyId, string integrateWith = "")
        {            
            var data = await _generalRequistionService.GetAllRequisitionSignatoryByCompany(companyId, integrateWith);
            return PartialView("_LoadRequisitionSignatoryList",data);
        }
        [HttpGet]
        public async Task<ActionResult> LoadCategorizedRequisitionSignatoryList(int categoryId)
        {
            var data = await _generalRequistionService.GetAllRequisitionSignatoryByCategory(categoryId);
            return PartialView("_LoadRequisitionSignatoryList", data);
        }
        [HttpGet]
        public async Task<ActionResult> LoadDivisionwisedRequisitionSignatoryList(int divisionId)
        {
            var data = await _generalRequistionService.GetAllRequisitionSignatoryByCategory(divisionId);
            return PartialView("_LoadRequisitionSignatoryList", data);
        }
        [HttpGet]
        public async Task<ActionResult> AdminRequisitionSignatory(int companyId = 0)
        {
            var data = await _generalRequistionService.GetAllRequisitionSignatory();
            var model = new RequisitionSignatoryVM();
            model.UserCompanyId = companyId;
            model.ActionId = ActionEnum.Add;
            model.DataList = data.ToList();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> RequisitionSignatory(RequisitionSignatoryVM model)
        {
            if (ModelState.IsValid)
            {
                bool result = false;
                var useerId = Common.GetUserId();
                if (model.ActionId == ActionEnum.Add)
                {
                    model.CreatedBy = useerId;
                    model.CreatedDate = DateTime.Now;
                    model.CompanyId = model.UserCompanyId;
                    result =await _generalRequistionService.AddRequisitionSignatory(model);
                }
                if (model.ActionId == ActionEnum.Edit)
                {
                    model.ModifiedBy = useerId;
                    model.ModifiedDate = DateTime.Now;
                    result = await _generalRequistionService.UpdateRequisitionSignatory(model);
                }
                if (model.ActionId == ActionEnum.Delete)
                {
                    model.ModifiedBy = useerId;
                    model.ModifiedDate = DateTime.Now;
                    result = await _generalRequistionService.DeleteRequisitionSignatory(model);
                }
            }
            return RedirectToAction(nameof(RequisitionSignatory), new { companyId = model.UserCompanyId, integrateWith  = model.IntegrateWith});
        }

        [HttpPost]
        public async Task<ActionResult> AdminRequisitionSignatory(RequisitionSignatoryVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.CompanyId == 0 && model.CategoryId.IsNullOrZero())
                {
                    return RedirectToAction(nameof(AdminRequisitionSignatory));
                }
                if (!model.CategoryId.IsNullOrZero())
                {
                    model.CompanyId = 0;
                    model.IsCategorizedSignatory = true;
                }

                bool result = false;
                var useerId = Common.GetUserId();
                if (model.ActionId == ActionEnum.Add)
                {
                    model.CreatedBy = useerId;
                    model.CreatedDate = DateTime.Now;
                    //model.CompanyId = model.UserCompanyId;
                    result = await _generalRequistionService.AddRequisitionSignatory(model);
                }
                if (model.ActionId == ActionEnum.Edit)
                {
                    model.ModifiedBy = useerId;
                    model.ModifiedDate = DateTime.Now;
                    result = await _generalRequistionService.UpdateRequisitionSignatory(model);
                }
                if (model.ActionId == ActionEnum.Delete)
                {
                    model.ModifiedBy = useerId;
                    model.ModifiedDate = DateTime.Now;
                    result = await _generalRequistionService.DeleteRequisitionSignatory(model);
                }
            }
            return RedirectToAction(nameof(AdminRequisitionSignatory), new { companyId = model.UserCompanyId });
        }
        #endregion Signatory



        #region ERequisition
        //[HttpGet]
        //public async Task<ActionResult> AddOrUpdate(int companyId = 0, int requisitionId = 0)
        //{

        //    GeneralRequisitionMasterVM requisition = new GeneralRequisitionMasterVM();
        //    if (requisitionId == 0)
        //    {
        //        requisition.Status = (int)EnumPOStatus.Draft;
        //    }
        //    else if (requisitionId > 0)
        //    {
        //        requisition = await Task.Run(() => _generalRequistionService.Get(requisitionId));
        //        if (requisition != null)
        //        {
        //            if (requisition.Status != GeneralRequisitionStatusEnum.Draft)
        //            {
        //                requisition.RequisitionApprovalLIst = _generalRequistionService.GetAllApproval(requisitionId).ToList();
        //            }
        //        }
        //    }
        //    requisition.UserCompanyId = companyId;
        //    return View(requisition);
        //}

        public ActionResult ERequisition()
        {
            return View(new EmployeeViewModel());
        }
        [HttpGet]
        public async Task<ActionResult> ERequisitionAdd(int companyId = 0, int requisitionId = 0)
        {
            ERequisitionVM requisition = new ERequisitionVM();
            requisition.RequisitionDate = DateTime.Today;
            if (requisitionId == 0)
            {
                requisition.ERequisitionStatus = (int)GeneralRequisitionStatusEnum.Draft;
            }
            else if (requisitionId > 0)
            {
                requisition = await Task.Run(() => _generalRequistionService.GetERequistionById(requisitionId));
                if (requisition == null)
                {
                    requisition = new ERequisitionVM();
                }
            }
            requisition.UserCompanyId = companyId;
            if (requisition.Attachments == null)
            {
                requisition.Attachments = new List<AttachmentVM>();
            } 
            return View(requisition);
        }
        [HttpPost]
        public async Task<ActionResult> ERequisitionAdd(ERequisitionVM requisition)
        {
            var userId = Common.GetUserId();
            //insert new requisition
            if (requisition.Id == 0 && !string.IsNullOrEmpty(requisition.ActionButton) && requisition.ActionButton.ToLower() == "create")
            {
               
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(requisition.Remarks))
                {
                    return RedirectToAction(nameof(ERequisitionAdd), new { companyId = requisition.UserCompanyId, requisitionId = requisition.Id });
                }
                //var emp = _db.Employees.FirstOrDefault(x => x.EmployeeId == userId);
                requisition.CreatedBy = userId;
                requisition.CreatedDate = DateTime.Now;
                //requisition.RequisitionNumber = await _generalRequistionService.GetGeneralRequisitionName((int)requisition.RequisitionToCompanyId, BusinessTypeEnum.Company);
                requisition.RequisitionNumber =await _generalRequistionService.GetEFileName();
                requisition.RequisitionDate = requisition.RequisitionDate;
                var result =  await _generalRequistionService.AddERequisition(requisition);
                if (requisition.Attachments != null &&  requisition.Attachments.Count() > 0)
                {
                    requisition.Attachments = requisition.Attachments.Where(x=> x.File != null).ToList();
                }

                if (result && requisition.Attachments != null && requisition.Attachments.Count() > 0)
                {
                    foreach (var item in requisition.Attachments)
                    {
                        string fileName = item.Title;
                        if (string.IsNullOrEmpty(item.Title))
                        {
                            fileName = Path.GetFileNameWithoutExtension(item.File.FileName);
                        }
                        FileItem file = new FileItem
                        {
                            file = item.File,
                            //docdesc = Path.GetFileNameWithoutExtension(item.File.FileName),
                            docdesc = fileName,
                            docid = 0,
                            FileCatagoryId = 8,
                            fileext = Path.GetExtension(item.File.FileName),
                            docfilename = Guid.NewGuid().ToString() + Path.GetExtension(item.File.FileName),
                            isactive = true,
                            RecDate = DateTime.Now,
                            SortOrder = 1,
                            userid = (int)Common.GetIntUserId()
                        };
                        var isUpload = await _ftpservice.UploadFile(file, requisition.Id.ToString(), false);
                        var mapping = await _ftpservice.FileMapping(new List<FileItem>() { file }, requisition.Id,false);
                    }
                    //var files = await _ftpservice.UploadFileBulk(requisition.Attachment, model.Id.ToString());
                    //long CGId = Convert.ToInt64(model.Id);
                    //var result = await gLDLCustomerService.FileMapping(itemlist, CGId);
                }
                //ERequisitionVM model = new ERequisitionVM();
                requisition.RequisitionDate = DateTime.Today;
                return RedirectToAction(nameof(ERequisitionAdd), new { companyId = requisition.UserCompanyId, requisitionId = requisition.Id });

                
            }
            //add requisition detail
            //if (requisition.Id > 0 && requisition.EmployeeId == 0)
            //{
            //    bool isValid = string.IsNullOrEmpty(requisition.Description)?false:true;

            //   //add details
            //    if(isValid && requisition.ERequisitionDetailId == 0)
            //    {
            //        requisition.CreatedBy = userId;
            //        requisition.CreatedDate = DateTime.Now;
            //        await _generalRequistionService.AddERequisitionItemDetail(requisition);
            //    }
            //    //update requisition detail
            //    if (requisition.ERequisitionDetailId > 0 && isValid)
            //    {
            //        await _generalRequistionService.UpdateERequisitionItemDetail(requisition);

            //    }
            //}
            //forwarding section
            if (requisition.EmployeeId > 0 && requisition.Id > 0 &&  !string.IsNullOrEmpty(requisition.ActionButton) && requisition.ActionButton.ToLower() == "forward")
            {
                requisition.CreatedBy = userId;
                requisition.CreatedDate = DateTime.Now;
                await _generalRequistionService.ForwardRequisition(requisition);
            }

            //upload new file
            if (!string.IsNullOrEmpty(requisition.ActionButton) && requisition.ActionButton.ToLower() == "update" && requisition.Attachments != null && requisition.Attachments.Count() > 0)
            {
                foreach (var item in requisition.Attachments.Where(x => x.DocId == 0).ToList())
                {
                    string fileName = item.Title;
                    if (string.IsNullOrEmpty(item.Title))
                    {
                        fileName = Path.GetFileNameWithoutExtension(item.File.FileName);
                    }
                    FileItem file = new FileItem
                    {
                        file = item.File,
                        //docdesc = Path.GetFileNameWithoutExtension(item.File.FileName),
                        docdesc = fileName,
                        docid = 0,
                        FileCatagoryId = 8,
                        fileext = Path.GetExtension(item.File.FileName),
                        docfilename = Guid.NewGuid().ToString() + Path.GetExtension(item.File.FileName),
                        isactive = true,
                        RecDate = DateTime.Now,
                        SortOrder = 1,
                        userid = (int)Common.GetIntUserId()
                    };
                    var isUpload = await _ftpservice.UploadFile(file, requisition.Id.ToString(), false);
                    var mapping = await _ftpservice.FileMapping(new List<FileItem>() { file }, requisition.Id, false);
                }
            }
                return RedirectToAction(nameof(ERequisitionAdd), new { companyId = requisition.UserCompanyId, requisitionId = requisition.Id });
        }
        public async Task<ActionResult> ERequisitionUpdate(ERequisitionVM model)
        {
            var result = await _generalRequistionService.UpdateERequisition(model);
            if (result && model.Attachment != null)
            {
                FileItem file = new FileItem
                {
                    file = model.Attachment,
                    docdesc = Path.GetFileNameWithoutExtension(model.Attachment.FileName),
                    docid = 0,
                    FileCatagoryId = 8,
                    fileext = Path.GetExtension(model.Attachment.FileName),
                    docfilename = Guid.NewGuid().ToString() + Path.GetExtension(model.Attachment.FileName),
                    isactive = true,
                    RecDate = DateTime.Now,
                    SortOrder = 1,
                    userid = (int)Common.GetIntUserId()
                };
                var isUpload = await _ftpservice.UploadFile(file, model.Id.ToString(), false);
                var mapping = await _ftpservice.FileMapping(new List<FileItem>() { file }, model.Id,false);
                //itemlist = await _ftpservice.UploadFileBulk(itemlist, model.Id.ToString());
                //long CGId = Convert.ToInt64(model.Id);
                //var result = await gLDLCustomerService.FileMapping(itemlist, CGId);
            }
            return RedirectToAction(nameof(ERequisitionTracking), new { companyId = model.UserCompanyId });
        }
      
        [HttpPost]
        public async Task<ActionResult> DeleteEDocument(int id = 0, int companyId = 0)
        {
            if (ModelState.IsValid)
            {
                var userId = Common.GetUserId();
                if (id > 0)
                {
                   var result =  await _generalRequistionService.Remove(id);
                }
            }

            return RedirectToAction(nameof(ERequisitionTracking), new { companyId = companyId });
        }
        [HttpGet]
        public ActionResult DeleteERequisitionItemDetail(int? companyId, int? requisitionId, int id)
        {
            _generalRequistionService.DeleteERequisitionItemDetail(id);
            return RedirectToAction(nameof(ERequisitionAdd), new { companyId = companyId, requisitionId = requisitionId });
        }
    
        [HttpGet]
        public ActionResult DeleteERequisitionSignatory(int? companyId, int? requisitionId, int? signatoryId)
        {
            _generalRequistionService.DeleteRequisitionSignatory((int)signatoryId);
            return RedirectToAction(nameof(ERequisitionAdd), new { companyId = companyId, requisitionId = requisitionId });
        }


        [HttpGet]
        public async Task<ActionResult> ERequisitionTracking(int companyId, DateTime? fromDate, DateTime? toDate, GeneralRequisitionStatusEnum? ERequisitionStatus)
        {
            List<ERequisitionVM> requisition = new List<ERequisitionVM>();
            var ddrequisition =await _generalRequistionService.GetAllEDocumentList(companyId,fromDate,toDate, ERequisitionStatus);
            ViewBag.CompanyId = companyId;
            return View(ddrequisition);
        }

        [HttpGet]
        public async Task<ActionResult> LoadERequisitionList(int companyId = 0, int requisitionId = 0)
        {
            ERequisitionVM requisition = new ERequisitionVM();
            requisition.RequisitionDate = DateTime.Today;
            if (requisitionId == 0)
            {
                requisition.ERequisitionStatus = (int)GeneralRequisitionStatusEnum.Draft;
            }
            else if (requisitionId > 0)
            {
                requisition = await Task.Run(() => _generalRequistionService.GetERequistionById(requisitionId));
                if (requisition != null)
                {
                    //if (requisition.Status != GeneralRequisitionStatusEnum.Draft)
                    //{
                    //    requisition.RequisitionApprovalLIst = _generalRequistionService.GetAllApproval(requisitionId).ToList();
                    //}
                }
            }
            requisition.UserCompanyId = companyId;
            return View(requisition);
        }

        [HttpGet]
        public async Task<ActionResult> EDocumentsApproval(int companyId, DateTime? fromDate, DateTime? toDate, EFileSignatoryStatusEnum? SignatoryStatus)
        {
            //List<ERequisitionVM> requisition = new List<ERequisitionVM>();
            var userId = Common.GetIntUserId();
            //userId = 1304;
            //userId = 42218;
            //userId = 42056;
            //userId = 21747;
            var requisition = await _generalRequistionService.GetAllEDocumentApprovalList(companyId,fromDate,toDate,userId, SignatoryStatus);
            if (requisition == null)
            {
                requisition = new List<ERequisitionVM>();
            }
            return View(requisition);
        }


        [HttpGet]
        public async Task<ActionResult> LoadEDocumentApprovalList(int companyId, DateTime? fromDate, DateTime? toDate, SignatoryStatusEnum? RequisitionSignatoryStatus)
        {
            var userId = Common.GetIntUserId();
            ViewBag.CompanyId = companyId;
            //userId = 1028;
            if (fromDate == null) fromDate = DateTime.Today.AddMonths(-2);
            if (toDate == null) toDate = DateTime.Today;
            var requisitionList = _generalRequistionService.GetAllRequisitionApprovalList(companyId, fromDate, toDate, userId, RequisitionSignatoryStatus);
            return View(requisitionList);

        }
        [HttpPost]
        public async Task<ActionResult> UpdateEDocumentApprovalStatus(EFileApprovalVM model)
        {
            int companyId = model.companyId;
            HttpPostedFileBase Attachment = model.Attachment;
            int requisitionSignatoryId = model.requisitionSignatoryId;
             EFileSignatoryStatusEnum status = model.status;

            EFileSignatoryStatusEnum signatoryStatus = (EFileSignatoryStatusEnum)model.SignatoryAction;
            if (status == EFileSignatoryStatusEnum.Rejected)
            {
                signatoryStatus = EFileSignatoryStatusEnum.Rejected;
            }


            string comment = model.comment;
            int? forwardTo = model.forwardTo;
            var result = await _generalRequistionService.UpdateEDocumentSignatoryApprovalStatus(requisitionSignatoryId, signatoryStatus, comment);



            if (result && forwardTo != null && forwardTo > 0)
            {
                ERPEntities _context = new ERPEntities();
                var requisitionApproval = _context.RequisitionSignatoryApprovals.FirstOrDefault(x => x.RequisitionSignatoryApprovalId == requisitionSignatoryId);
                if (requisitionApproval != null)
                {
                    var totalCount = _context.RequisitionSignatoryApprovals.Count(x => x.RequisitionId == requisitionApproval.RequisitionId);
                    RequisitionSignatoryApproval forward = new RequisitionSignatoryApproval()
                    {
                        RequisitionId = requisitionApproval.RequisitionId,
                        EmployeeId = forwardTo.Value,
                        OrderBy = ++totalCount,
                        IsActive = true,
                        CreatedBy = Common.GetUserId(),
                        CreatedDate = DateTime.Now,
                        Status = (int)EFileSignatoryStatusEnum.Pending
                    };
                    _context.RequisitionSignatoryApprovals.Add(forward);
                    await _context.SaveChangesAsync();
                }
                
            }
            if (result && Attachment != null)
            {
                ERPEntities _context = new ERPEntities();
                var requisitionApproval = _context.RequisitionSignatoryApprovals.FirstOrDefault(x => x.RequisitionSignatoryApprovalId == requisitionSignatoryId);
                if (requisitionApproval != null)
                {
                    FileItem file = new FileItem
                    {
                        file = Attachment,
                        docdesc = Path.GetFileNameWithoutExtension(Attachment.FileName),
                        docid = 0,
                        FileCatagoryId = 8,
                        fileext = Path.GetExtension(Attachment.FileName),
                        docfilename = Guid.NewGuid().ToString() + Path.GetExtension(Attachment.FileName),
                        isactive = true,
                        RecDate = DateTime.Now,
                        SortOrder = 1,
                        userid = (int)Common.GetIntUserId()
                    };
                    var isUpload = await _ftpservice.UploadFile(file,requisitionApproval.RequisitionId.ToString(), false);
                    var mapping = await _ftpservice.FileMapping(new List<FileItem>() { file }, requisitionApproval.RequisitionId,false);
                }
                //itemlist = await _ftpservice.UploadFileBulk(itemlist, model.Id.ToString());
                //long CGId = Convert.ToInt64(model.Id);
                //var result = await gLDLCustomerService.FileMapping(itemlist, CGId);
            }
            return RedirectToAction(nameof(EDocumentsApproval), new { companyId = companyId });

        }

        #endregion Erequisition
    }
}