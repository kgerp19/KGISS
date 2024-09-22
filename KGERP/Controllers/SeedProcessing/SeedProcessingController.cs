using KGERP.Service.Interface;
using KGERP.Service.ServiceModel.SeedProcessingModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.SeedProcessing
{
    public class SeedProcessingController : BaseController
    {
        private readonly ISeedProcessingService _seedProcessingService;
        private readonly IMaterialReceiveService _materialReceiveService;

        public SeedProcessingController(ISeedProcessingService seedProcessingService, IMaterialReceiveService materialReceiveService)
        {
            _seedProcessingService = seedProcessingService;
            _materialReceiveService = materialReceiveService;
        }
        #region SeedProcessing
        [HttpGet]
        public async Task<ActionResult> SeedProcessingCreate(int companyId, long SeedProcessingId = 0)
        {
            SeedProcessingDetailsVM model = new SeedProcessingDetailsVM();
            var hi = Common.GetUserId();
            if (SeedProcessingId > 0)
            {
                model = await _seedProcessingService.GetDataSeedProcessingAndDetailsBySeedProcessingId(SeedProcessingId);
            }
            model.CompanyFK = companyId;
            model.SeedProcessDate = DateTime.Today;
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> SeedProcessingList(int companyId)
        {
            SeedProcessingDetailsVM mvModel = new SeedProcessingDetailsVM();
            if (companyId > 0)
            {
                mvModel = await _seedProcessingService.GetDataSeedProcessingList(companyId);
            }
            mvModel.CompanyFK = companyId;

            return View(mvModel);
        }


        [HttpPost]
        public async Task<ActionResult> SeedProcessingCreate(SeedProcessingDetailsVM seedProcessingDetails, MaterialReceiveDetailsWithProductVM materialReceiveDetailsWithProductVM)
        {
            if (seedProcessingDetails.SeedProcessingId == 0)
            {
                seedProcessingDetails.SeedProcessingId = await _seedProcessingService.CreateSeepProcessing(seedProcessingDetails, materialReceiveDetailsWithProductVM);
            }

            return RedirectToAction(nameof(SeedProcessingCreate), new { companyId = seedProcessingDetails.CompanyFK, seedProcessingId = seedProcessingDetails.SeedProcessingId });
        }

        [HttpGet]
        public async Task<JsonResult> GetMetrialReciveDetailsList(string prefix, int CompanyId = 0)
        {
            var MaterialReciveDetaislDDL = await _materialReceiveService.CompanyWiseMaterialReceiveId(prefix, CompanyId);
            return Json(MaterialReciveDetaislDDL, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> GetMetrialReciveDetailsDataList(long materialReceiveId)
        {
            var model = new MaterialReceiveDetailsWithProductVM();
            model.DataListPro = await _materialReceiveService.MaterialReceiveWiseMaterialReceiveDetailsDataList(materialReceiveId);
            return PartialView("_MaterialReceiveDetailListPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> SeedProcessingDetailsDelete(SeedProcessingDetailsVM processingDetailsVM)
        {
            var model = new SeedProcessingDetailsVM();
            model.SeedProcessingId = await _seedProcessingService.DeleteSeedProcessingDetails(processingDetailsVM.SeedProcessingDetailsId);
            return RedirectToAction(nameof(SeedProcessingCreate), new { companyId = processingDetailsVM.CompanyFK, seedProcessingId = model.SeedProcessingId });
        }

        [HttpPost]
        public async Task<ActionResult> SeedProcessingDelete(SeedProcessingDetailsVM seedProcessing)
        {
            long resultSeedProcessingId = await _seedProcessingService.DeleteSeedProcessing(seedProcessing.SeedProcessingId);
            return RedirectToAction(nameof(SeedProcessingList), new { companyId = seedProcessing.CompanyFK });
        }

        [HttpPost]
        public async Task<ActionResult> SeedProcessingDetailsUpdate(SeedProcessingDetailsVM processingDetailsVM)
        {
            var model = new SeedProcessingDetailsVM();
            model.SeedProcessingId = await _seedProcessingService.SeedProcessingDetailsUpdate(processingDetailsVM);
            return RedirectToAction(nameof(SeedProcessingCreate), new { companyId = processingDetailsVM.CompanyFK, seedProcessingId = model.SeedProcessingId });
        }

        [HttpPost]
        public async Task<ActionResult> SeedProcessingUpdate(SeedProcessingDetailsVM processingDetailsVM)
        {
            var model = new SeedProcessingDetailsVM();
            model.SeedProcessingId = await _seedProcessingService.SeedProcessingUpdate(processingDetailsVM);
            return RedirectToAction(nameof(SeedProcessingList), new { companyId = processingDetailsVM.CompanyFK });
        }

        [HttpPost]
        public async Task<ActionResult> SeedProcessingSubmitted(SeedProcessingDetailsVM processingDetailsVM)
        {
            var model = new SeedProcessingDetailsVM();
            model.SeedProcessingId = await _seedProcessingService.SeedProcessingSubmitted(processingDetailsVM);
            return RedirectToAction(nameof(SeedProcessingCreate), new { companyId = processingDetailsVM.CompanyFK, seedProcessingId = model.SeedProcessingId });
        }
        #endregion
    }
}