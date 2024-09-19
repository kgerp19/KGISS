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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class BoardOfDirectorController : BaseController
    {
        private readonly IBoardOfDirectorService boardOfDirectorService;
        private readonly IDropDownItemService dropDownItemService;
        public BoardOfDirectorController(IBoardOfDirectorService boardOfDirectorService, IDropDownItemService dropDownItemService)
        {
            this.boardOfDirectorService = boardOfDirectorService;
            this.dropDownItemService = dropDownItemService;
        }

        
        public ActionResult AllBoardOfDirectorIndex(int companyId, int? Page_No, string searchText)
        {
            searchText = searchText ?? string.Empty;
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            List<BoardOfDirectorModel> models = boardOfDirectorService.GetAllBoardOfDirectors(searchText);
            List<BoardOfDirectorModel> membersWithImage = models.Select(x => new BoardOfDirectorModel
            {
                BoardOfDirectorId = x.BoardOfDirectorId,
                CompanyName = x.CompanyName,
                MemberName = x.MemberName,
                MemberOrder = x.MemberOrder,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Phone = x.Phone,
                Email = x.Email,
                IsActive = x.IsActive,
                MemberImage = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/BoardOfDirector/" + (string.IsNullOrEmpty(x.MemberImage) ? "default.png" : x.MemberImage),
            }).ToList();
            int Size_Of_Page = 10;
            int No_Of_Page = Page_No ?? 1;
            return View(membersWithImage.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        
        
        [HttpGet]
        public async Task<ActionResult> Index(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            BoardOfDirectorModel boardOfDirectorModel = new BoardOfDirectorModel();
            boardOfDirectorModel = await boardOfDirectorService.GetBoardOfDirectors(companyId);
           
            return View(boardOfDirectorModel);
        }

        public ActionResult CreateOrEdit(int id)
        {
            BoardOfDirectorViewModel vm = new BoardOfDirectorViewModel();
            vm.BoardOfDirector = boardOfDirectorService.GetBoardOfDirector(id);
            vm.Professions = dropDownItemService.GetDropDownItemSelectModels(38);
            vm.EducationQualifications = dropDownItemService.GetDropDownItemSelectModels(7);
            vm.BoardOfDirector.MemberImage = string.Format("{0}://{1}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority) + "/Images/BoardOfDirector/" + (string.IsNullOrEmpty(vm.BoardOfDirector.MemberImage) ? "default.png" : vm.BoardOfDirector.MemberImage);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(BoardOfDirectorViewModel vm)
        {
            ModelState.Clear();
            string message = string.Empty;
            bool result = false;

            if (vm.BoardOfDirector.MemberImageUpload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(vm.BoardOfDirector.MemberImageUpload.FileName);
                string extension = Path.GetExtension(vm.BoardOfDirector.MemberImageUpload.FileName);
                fileName = fileName + "_" + DateTime.Now.ToString("yymmssfff") + extension;
                vm.BoardOfDirector.MemberImage = fileName;
                vm.BoardOfDirector.MemberImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Images/BoardOfDirector/"), fileName));
            }

            if (vm.BoardOfDirector.BoardOfDirectorId <= 0)
            {
                result = boardOfDirectorService.SaveBoardOfDirector(0, vm.BoardOfDirector);
            }
            else
            {
                result = boardOfDirectorService.SaveBoardOfDirector(vm.BoardOfDirector.BoardOfDirectorId, vm.BoardOfDirector);
            }
            if (result)
            {
                TempData["message"] = "Member Saved Successfully !";
            }
            else
            {
                TempData["message"] = "Member not Saved !";
            }
            return RedirectToAction("Index", new { companyId = vm.BoardOfDirector.CompanyId });
        }


        public ActionResult Delete(int id)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            bool result = boardOfDirectorService.DeleteBoardOfDirector(id);
            if (result)
            {
                return RedirectToAction("Index", new { companyId });
            }
            return View();
        }

        
        [HttpGet]
        public ActionResult UploadBoardOfDirector(int companyId)
        {
            BoardOfDirectorModel model = new BoardOfDirectorModel { CompanyId = companyId };
            return View(model);
        }
        
        [HttpPost]
        public ActionResult UploadBoardOfDirector(BoardOfDirectorModel model)
        {
            bool result = false;
            List<BoardOfDirectorModel> boardOfDirectors = new List<BoardOfDirectorModel>();

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
                        BoardOfDirectorModel boardOfDirector = new BoardOfDirectorModel();
                        boardOfDirector.CompanyId = Convert.ToInt32(workSheet.Cells[rowIterator, 1].Value);
                        boardOfDirector.MemberName = workSheet.Cells[rowIterator, 2].Value == null ? null : workSheet.Cells[rowIterator, 2].Value.ToString();
                        boardOfDirector.MemberOrder = Convert.ToInt32(workSheet.Cells[rowIterator, 3].Value);
                        boardOfDirector.StartDate = workSheet.Cells[rowIterator, 4].Value == null ? (DateTime?)null : Convert.ToDateTime(workSheet.Cells[rowIterator, 4].Value);
                        boardOfDirector.EndDate = workSheet.Cells[rowIterator, 5].Value == null ? (DateTime?)null : Convert.ToDateTime(workSheet.Cells[rowIterator, 5].Value);
                        boardOfDirector.Remarks = string.Empty;
                        boardOfDirector.IsActive = true;
                        boardOfDirectors.Add(boardOfDirector);
                    }
                }
            }
            result = boardOfDirectorService.BulkSave(boardOfDirectors);
            if (result)
            {
                return RedirectToAction("Index", new { companyId = model.CompanyId });
            }
            return View();
        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    bool result = districtService.DeleteDistrict(id);
        //    if (result)
        //    {
        //        return RedirectToAction("Index", new { Page_No = 1, searchText = string.Empty });
        //    }
        //    return View();
        //}

        //public ActionResult Details(int id)
        //{
        //    if (id <= 0)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ProductCategoryModel productCategory = productCategoryService.GetProductCategory(id);
        //    if (productCategory == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(productCategory);
        //}
    }
}