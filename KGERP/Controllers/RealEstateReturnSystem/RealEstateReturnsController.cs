using DocumentFormat.OpenXml.EMMA;
using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Implementation.FTP;
using KGERP.Service.Implementation.RealEstateReturnSystemService;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.FTP_Models;
using KGERP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.RealEstateReturnSystem
{
    [CheckSession]
    public class RealEstateReturnsController : Controller
    {
        private readonly MoneyReceiptService _moneyReceiptService;
        private readonly IRealEstateReturnSystem estateReturnSystem;
        private IFTPService _ftpservice;
        public RealEstateReturnsController(MoneyReceiptService moneyReceiptService, IRealEstateReturnSystem estateReturnSystem, IFTPService ftpservice)
        {
            _moneyReceiptService = moneyReceiptService;
            this.estateReturnSystem = estateReturnSystem;
            _ftpservice= ftpservice;    
        }
        // GET: RealEstateReturns
        public async Task<ActionResult> Index(int companyid)
        {
            RealEstateReturnSystemVM cm = new RealEstateReturnSystemVM()
            {
                CompanyId = companyid,
            };
            cm.ProjectList = await _moneyReceiptService.ProjectList(companyid);
            return View(cm);
        }
        [HttpGet]
        public async Task<ActionResult> FileInformation(int companyid, long CGId, int StatusId)
        {
            RealEstateReturnSystemVM model = new RealEstateReturnSystemVM();
            model = await estateReturnSystem.GetFileInformation(CGId);
            model.CompanyId = companyid;
            model.StatusId = StatusId;
            model.CGId = CGId;
            model.strCancelDate = DateTime.Now.ToString("dd/MM/yyyy");
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmReturn(RealEstateReturnSystemVM vM)
        {
            List<FileItem> itemlist = new List<FileItem>();
            if (vM.UploadedFile != null)
            {
                itemlist.Add(new FileItem
                {
                    file = vM.UploadedFile,
                    docdesc = vM.BookingViewModel.ClientName + "(" + vM.BookingViewModel.FileNo + ")",
                    docid = 0,
                    FileCatagoryId = 6,
                    fileext = vM.UploadedFile.FileName,
                    docfilename = Guid.NewGuid().ToString() + vM.UploadedFile.FileName,
                    isactive = true,
                    RecDate = DateTime.Now,
                    SortOrder = 0,
                    userid = Convert.ToInt32(Session["Id"])
                });
                itemlist = await _ftpservice.UploadFileBulk(itemlist, vM.CGId.ToString());
            }
            vM.DocId = itemlist.FirstOrDefault(d => d.SortOrder == 0).docid;
            var res = await estateReturnSystem.AddReturn(vM);
            return RedirectToAction(nameof(FileInformation), new { vM.CompanyId, vM.CGId, vM.StatusId });
        }
         
        [HttpGet]
        public async Task<ActionResult> ReturnList(int companyid,string message=null)
        {
           
            var res = await estateReturnSystem.ReturnListn(companyid);
            if (message != null)
            {
                res.message = message;
            }
            return View(res);
        }


        [HttpGet]
        public async Task<ActionResult> KGEREReturnList(int companyid, string message = null)
        {

            var res = await estateReturnSystem.KGEREReturnListn(companyid);
            if (message != null)
            {
                res.message = message;
            }
            return View(res);
        }


        [HttpGet]
        public async Task<ActionResult> Remove(long id,int companyid)
        {

            var res = await estateReturnSystem.RemoveReturn(id);
            return RedirectToAction("KGEREReturnListn",new { companyId = companyid});
        }

        [HttpPost]
        public async Task<ActionResult> AccConfirmReturn(RealEstateReturnListVM returnListVM)
        {
            var resvm = await estateReturnSystem.GetFileInformation(returnListVM.CGId);
            if (returnListVM.StatusId == 2)
            {
                var res = await estateReturnSystem.AccConfirmReturn(returnListVM, resvm);
                if (res != 0)
                {
                    return RedirectToAction("ManageBankOrCash", "Vouchers", new { companyId = returnListVM.CompanyId, voucherId = res });
                }

                return RedirectToAction(nameof(ReturnList), new { returnListVM.CompanyId, message = "Return process unsuccessful" });
            }
            if (returnListVM.StatusId == 3)
            {
                var res = await estateReturnSystem.AccTransfarFileConfirmReturn(returnListVM, resvm);
                if (res != 0)
                {
                    return RedirectToAction("ManageBankOrCash", "Vouchers", new { companyId = returnListVM.CompanyId, voucherId = res });
                }

                return RedirectToAction(nameof(ReturnList), new { returnListVM.CompanyId, message = "Return process unsuccessful" });
            }
            return RedirectToAction(nameof(ReturnList), new { returnListVM.CompanyId});
        }

    }
}