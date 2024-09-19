using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ShareHolderController : BaseController
    {
        private readonly IShareHolderService shareHolderService;
        private readonly IDropDownItemService dropDownItemService;
        public ShareHolderController(IShareHolderService shareHolderService, IDropDownItemService dropDownItemService)
        {
            this.shareHolderService = shareHolderService;
            this.dropDownItemService = dropDownItemService;
        }


        
        [HttpGet]
        public ActionResult AllShareHolderIndex()
        {
            return View();
        }


        
        [HttpPost]
        public ActionResult AllShareHolder()
        {
            //Server Side Parameter
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"] ?? string.Empty;
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            IQueryable<ShareHolderModel> shareHolderList = shareHolderService.GetAllShareHolders(searchValue, out int count);
            int totalRows = count;
            int totalRowsAfterFiltering = shareHolderList.Count();
            //sorting
            shareHolderList = shareHolderList.OrderBy(sortColumnName + " " + sortDirection);

            //paging
            shareHolderList = shareHolderList.Skip(start).Take(length);


            return Json(new { data = shareHolderList, draw = Request["draw"], recordsTotal = totalRows, recordsFiltered = totalRowsAfterFiltering }, JsonRequestBehavior.AllowGet);
        }



        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            ShareHolderModel shareHolders =await shareHolderService.GetShareHolders(companyId);
           
            return View(shareHolders);
        }


        
        [HttpGet]
        public ActionResult CreateOrEdit(int id)
        {
            ModelState.Clear();
            ShareHolderViewModel vm = new ShareHolderViewModel();
            vm.ShareHolder = shareHolderService.GetShareHolder(id);
            vm.ShareHolderTypes = dropDownItemService.GetDropDownItemSelectModels(39);
            vm.Professions = dropDownItemService.GetDropDownItemSelectModels(38);
            vm.EducationQualifications = dropDownItemService.GetDropDownItemSelectModels(7);
            vm.Gender = dropDownItemService.GetDropDownItemSelectModels(3);
            vm.ShareHolder.ShareHolderImage = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/ShareHolder/" + vm.ShareHolder.ShareHolderImage;
            vm.ShareHolder.NIDImage = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/ShareHolderNID/" + vm.ShareHolder.NIDImage;
            
            return View(vm);
        }


        
        [HttpPost]
        public ActionResult CreateOrEdit(ShareHolderViewModel vm)
        {
            ModelState.Clear();
            string message = string.Empty;
            bool result = false;

            if (vm.ShareHolderImageUpload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(vm.ShareHolderImageUpload.FileName);
                string extension = Path.GetExtension(vm.ShareHolderImageUpload.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                vm.ShareHolder.ShareHolderImage = fileName;
                vm.ShareHolderImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Images/ShareHolder/"), fileName));
            }

            if (vm.NIDImageUpload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(vm.NIDImageUpload.FileName);
                string extension = Path.GetExtension(vm.NIDImageUpload.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                vm.ShareHolder.NIDImage = fileName;
                vm.NIDImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Images/ShareHolderNID/"), fileName));
            }

            if (vm.ShareHolder.ShareHolderId <= 0)
            {
                result = shareHolderService.SaveShareHolder(0, vm.ShareHolder, out message);
            }

            else
            {
                result = shareHolderService.SaveShareHolder(vm.ShareHolder.ShareHolderId, vm.ShareHolder, out message);
            }
            if (!result)
            {

            }

            

            return RedirectToAction("CreateOrEdit", new { vm.ShareHolder.ShareHolderId });
        }


        
        [HttpGet]
        public ActionResult UploadShareHolder(int companyId)
        {
            ShareHolderModel model = new ShareHolderModel { CompanyId = companyId };
            return View(model);
        }
        
        [HttpPost]
        public ActionResult UploadShareHolder(ShareHolderModel model)
        {
            bool result = false;
            List<ShareHolderModel> shareHolders = new List<ShareHolderModel>();

            HttpPostedFileBase file = model.UploadedFile;
            if (file.ContentLength >= 0)
            {
                string fileName = file.FileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];
                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                using (var package = new ExcelPackage(file.InputStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        ShareHolderModel shareHolder = new ShareHolderModel();

                        shareHolder.CompanyId = Convert.ToInt32(workSheet.Cells[rowIterator, 2].Value);
                        shareHolder.Name = workSheet.Cells[rowIterator, 3].Value == null ? null : workSheet.Cells[rowIterator, 3].Value.ToString();
                        shareHolder.GenderId = workSheet.Cells[rowIterator, 4].Value == null ? (int?)null : Convert.ToInt32(workSheet.Cells[rowIterator, 4].Value);
                        shareHolder.NID = workSheet.Cells[rowIterator, 5].Value == null ? null : workSheet.Cells[rowIterator, 5].Value.ToString();
                        shareHolder.DateOfBirth = workSheet.Cells[rowIterator, 6].Value == null ? (DateTime?)null : Convert.ToDateTime(workSheet.Cells[rowIterator, 6].Value);
                        shareHolder.StartDate = workSheet.Cells[rowIterator, 7].Value == null ? (DateTime?)null : Convert.ToDateTime(workSheet.Cells[rowIterator, 7].Value);
                        shareHolder.Phone = workSheet.Cells[rowIterator, 8].Value == null ? null : workSheet.Cells[rowIterator, 8].Value.ToString();
                        shareHolder.Email = workSheet.Cells[rowIterator, 9].Value == null ? null : workSheet.Cells[rowIterator, 9].Value.ToString();
                        shareHolder.PresentAddress = workSheet.Cells[rowIterator, 10].Value == null ? null : workSheet.Cells[rowIterator, 10].Value.ToString();
                        shareHolder.PresentAddress = workSheet.Cells[rowIterator, 11].Value == null ? null : workSheet.Cells[rowIterator, 11].Value.ToString();
                        shareHolder.FatherName = workSheet.Cells[rowIterator, 12].Value == null ? null : workSheet.Cells[rowIterator, 12].Value.ToString();
                        shareHolder.MotherName = workSheet.Cells[rowIterator, 13].Value == null ? null : workSheet.Cells[rowIterator, 13].Value.ToString();
                        shareHolder.Spouse = workSheet.Cells[rowIterator, 14].Value == null ? null : workSheet.Cells[rowIterator, 14].Value.ToString();
                        shareHolder.HomePhone = workSheet.Cells[rowIterator, 15].Value == null ? null : workSheet.Cells[rowIterator, 15].Value.ToString();
                        shareHolder.OfficePhone = workSheet.Cells[rowIterator, 16].Value == null ? null : workSheet.Cells[rowIterator, 16].Value.ToString();
                        shareHolder.Fax = workSheet.Cells[rowIterator, 17].Value == null ? null : workSheet.Cells[rowIterator, 17].Value.ToString();
                        shareHolder.EducationQualificationId = workSheet.Cells[rowIterator, 18].Value == null ? (int?)null : Convert.ToInt32(workSheet.Cells[rowIterator, 18].Value);
                        shareHolder.ProfessionId = workSheet.Cells[rowIterator, 19].Value == null ? (int?)null : Convert.ToInt32(workSheet.Cells[rowIterator, 19].Value);
                        shareHolder.Organization = workSheet.Cells[rowIterator, 20].Value == null ? null : workSheet.Cells[rowIterator, 20].Value.ToString();
                        shareHolder.Designation = workSheet.Cells[rowIterator, 21].Value == null ? null : workSheet.Cells[rowIterator, 21].Value.ToString();
                        shareHolder.NoOfShare = workSheet.Cells[rowIterator, 22].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 22].Value);
                        shareHolder.Amount = workSheet.Cells[rowIterator, 23].Value == null ? 0 : Convert.ToInt32(workSheet.Cells[rowIterator, 23].Value);
                        shareHolder.Nominee = workSheet.Cells[rowIterator, 24].Value == null ? null : workSheet.Cells[rowIterator, 24].Value.ToString();
                        shareHolder.TIN = workSheet.Cells[rowIterator, 25].Value == null ? null : workSheet.Cells[rowIterator, 25].Value.ToString();
                        shareHolder.MemberId = workSheet.Cells[rowIterator, 26].Value == null ? null : workSheet.Cells[rowIterator, 26].Value.ToString();
                        shareHolder.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        shareHolder.CreatedDate = DateTime.Today;
                        shareHolder.IsActive = true;
                        shareHolders.Add(shareHolder);
                    }
                }
            }
            result = shareHolderService.BulkSave(shareHolders);
            if (result)
            {
                return RedirectToAction("Index", new { companyId = model.CompanyId });
            }
            return View();
        }
    }
}