using KGERP.Service.Implementation.FTP;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.FTP_Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Controllers
{


    [CheckSession]
    public class UploadController : Controller
    {
        private IFTPService _service;
        private readonly ICompanyService _companyService;
        public UploadController(IFTPService service, ICompanyService companyService)
        {
            this._service = service;
            _companyService = companyService;
        }

        
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<ActionResult> Index(FileViewModel data)
        {
            List<FileItem> itemlist = new List<FileItem>();
            for (int i = 0; i < data.file.Count; i++)
            {
                itemlist.Add(new FileItem
                {
                    file = data.file[i],
                    docdesc = data.fileTitle,
                    docfilename = data.file[i].FileName,
                    docid = 0,
                    FileCatagoryId = 1,
                    fileext = Path.GetExtension(data.file[i].FileName),
                    isactive = true,
                    RecDate = DateTime.Now,
                    SortOrder = i,
                    userid = 12
                });
            }
            var x = await _service.UploadFileBulk(itemlist);
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> List(int id = 0, bool Bin = false)
        {
            var list = await _service.GetAllFilesByCatagory(id, Bin);
            return View(list);
        }
        
        [HttpGet]
        public async Task<ActionResult> RealStateFileList(int companyId)
        {
            FileArchiveViewModel file = new FileArchiveViewModel();
    
            if (companyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited || companyId == (int)CompanyName.KrishibidPropertiesLimited)
            {
                file = await _service.GetAllFilesByCompany(2, false, companyId);
                var company = _companyService.GetCompany(companyId);
                file.CompanyName = company.Name;
                file.CompanyId = companyId;
                return View(file);
            }

            return View(file);
        }


        public async Task<ActionResult> DeleteFTP(int docid) // its working and called
        {
            try
            {
                await _service.DeleteFile(docid);
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                return Json("No Such File", JsonRequestBehavior.AllowGet);
            }
        }  
        
        public async Task<ActionResult> RealStateDeleteFTP(int docid, int companyId) // its working and called
        {
            try
            {
                await _service.DeleteFile(docid);
                return RedirectToAction("RealStateFileList", new { companyId = companyId});
            }
            catch (Exception ex)
            {
                return Json("No Such File", JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> RestoreFTP(int docid) // its working and called
        {
            try
            {
                await _service.RestoreFileFromBin(docid);
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                return Json("No Such File", JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> PermanentDeleteFTP(int docid) // its working and called
        {
            try
            {
                await _service.DeletePermanently(docid);
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                return Json("No Such File", JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<ActionResult> DownloadFTP(int docid) // its working and called
        {
            try
            {
                var Stream = await _service.GetFileById(docid);
                if (String.IsNullOrWhiteSpace(Stream.ErrorMessage))
                {
                    return File(Stream.Data, Stream.mimeType, Stream.FileName);
                }
                else
                {
                    return Json(Stream.ErrorMessage, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("No Such File", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> EmptyBin() // its working and called
        {
            try
            {
                var IsEmptyed = await _service.EmptyRecycleBin();
                return Json(IsEmptyed, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }



    }
}
