using KGERP.Data.Models;
using KGERP.Service;
using KGERP.Service.Implementation.SalesManagement;
using KGERP.Service.Implementation.TaskManagment;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace KGERP.Controllers.SalesManagement
{
    [CheckSession]
    public class SalesManagementController : Controller
    {
        public readonly ISalesManagementService _salesManagementService;
        public SalesManagementController(ISalesManagementService salesManagementService)
        {
            _salesManagementService = salesManagementService;
        }

        #region KG Sales Acheivement Page
        [HttpGet]
        public async Task<ActionResult> SalesAchievement(int companyid, DateTime? fromDate, DateTime? toDate)
        {
            int year = DateTime.Now.Year;
            if (DateTime.Now.Month < 7)
            {
                year -= 1;
            }
            if (fromDate == null)
            {
                fromDate = new DateTime(year, 7, 1);
            }
            if (toDate == null)
            {
                toDate = fromDate.Value.AddMonths(12);
            }
            SalesManagementVM model = new SalesManagementVM();
            model.StrFromDate = fromDate.Value.ToString("dd/MM/yyyy");
            model.StrToDate = toDate.Value.ToString("dd/MM/yyyy");
            model.CompanyId = companyid;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SalesAchievement(SalesManagementVM model)
        {
            model.FromDate = DateTime.ParseExact(model.StrFromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            model.ToDate = DateTime.ParseExact(model.StrToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (ModelState.IsValid)
            {
                long result;
                if (model.AchievementId <= 0)
                {
                    result = await _salesManagementService.SaveSalesAchievementAsync(model);
                }
                else
                {
                    result = await _salesManagementService.UpdateAchievementAsync(model);
                }

                if (result > 0)
                {
                    return RedirectToAction(nameof(SalesAchievement), new { companyid = model.CompanyId});
                }
                else
                {
                    TempData["ErrorMessage"] = "A Sales achievement already exists in the specified date range.";
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAchievements()
        {
            var achievements = await _salesManagementService.GetAllAchievementsAsync();
            foreach (SalesManagementVM item in achievements)
            {
                item.TotalTergetedAmountStr = BDCurrency.SetBDCurrency(item.TotalTergetedAmount.ToString());
            }

            return Json(achievements, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteAchievement(long achievementId)
        {
            try
            {
                _salesManagementService.DeleteAchievementAsync(achievementId);
                return Json(new { success = true, message = "Data removed successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<ActionResult> GetAchievement(long achievementId)
        {
            var achievement = await _salesManagementService.GetAchievementAsync(achievementId);
            return Json(new { achievement }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region KG Company Sales Target
        [HttpGet]
        public async Task<ActionResult> KGCompanySalesTarget(int companyId,long? salesId)
        {
            SalesManagementVM model = new SalesManagementVM();
            model.SalesAcheivements = _salesManagementService.GetDDLSalesAchievements();
            model.Companies = _salesManagementService.GetDDLCompany(companyId);
            model.AchievementId = salesId ?? 0;

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> KGCompanySalesTargetSave(int KGcomTargetId, int AchievementId, int CompanyId, decimal TargetAmount)
        {
            try
            {
                long isExist = 0;
                var model = new SalesManagementVM
                {
                    KGcomTargetId = KGcomTargetId,
                    AchievementId = AchievementId,
                    CompanyId = CompanyId,
                    TargetAmount = TargetAmount
                };

                if (KGcomTargetId <= 0)
                {
                    isExist = await _salesManagementService.SaveKGCompanySaleTarget(model);
                }
                else if (KGcomTargetId > 0)
                {
                    isExist = await _salesManagementService.UpdateCompanySalesTargetAsync(model);
                }
                if (isExist > 0)
                {
                    return Json(new { success = true, message = "Data saved successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Sales target for this company already exists!." });
                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public async Task<ActionResult> GetSaleswiseTarget(int achievementId)
        {
            var salesInfo = await _salesManagementService.GetSaleswiseTarget(achievementId);
            foreach (SalesManagementVM item in salesInfo.DataList)
            {
                item.TargetAmountStr = BDCurrency.SetBDCurrency(item.TargetAmount.ToString());
            }
            return Json(salesInfo, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> CompanyWiseSalesAchievementList(int companyId)
        {
            SalesManagementVM model = await _salesManagementService.GetCompanyWiseSalesAchievements(companyId);
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> GetSalesTarget(long salesTargetId)
        {
            var salesTarget = await _salesManagementService.GetSalesTargetAsync(salesTargetId);
            return Json(salesTarget, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteSalesTarget(long salesTargetId)
        {
            try
            {
                _salesManagementService.DeleteSalesTargetAsync(salesTargetId);
                return Json(new { success = true, message = "Data removed successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult> AssignSalestarget(int KGCompanySaleTergetId)

        {


            SalesManagementVM model = new SalesManagementVM();

            model = await _salesManagementService.GetMonthlyTarget(KGCompanySaleTergetId);



            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> SaveTargetAmounts(List<TargetAmountViewModel> targetAmounts)
        {
            KGCompanyMonthlySaleTergetVM kGCompanyMonthlySaleTergetVM = new KGCompanyMonthlySaleTergetVM();
            var res = await _salesManagementService.SaveMonthlyTarget(targetAmounts);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTarget(int id, decimal saleAmount, decimal saleQuantity, decimal collectionAmount)
        {

            var res = await _salesManagementService.EditData(id, saleAmount, saleQuantity, collectionAmount);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> SalesPeronAssign(int KGCompanyMonthlySaleTergetId, int CompanyId)
        {
            KGSalesAchivementDetailVm model = await _salesManagementService.GetOfficerAssign(KGCompanyMonthlySaleTergetId, CompanyId);
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> SaveAchievementDetails(SalesAchievementDetailViewModel Achivement)
        {
            var res = await _salesManagementService.SaveAchievementDetail(Achivement);

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> SaleTagetFix(int KGCompanyMonthlySaleTergetId, int CompanyId)
        {
            KGSalesAchivementDetailVm model = await _salesManagementService.fixTarget(KGCompanyMonthlySaleTergetId, CompanyId);
            model.KGCompanyMonthlySaleTergetId = KGCompanyMonthlySaleTergetId;
            return View(model);

        }

        [HttpPost]
        public async Task<ActionResult> SaveEmployeeTargets(List<EmployeeTargetViewModel> empTargets)
        {
            var res = await _salesManagementService.SaveEmTarget(empTargets);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        #endregion
        [HttpGet]
        public async Task<ActionResult> CompanyWiseSalesAchievementListPerson(int CompanyId)
        {
            SalesManagementVM model = await _salesManagementService.GetCompanyWiseSalesAchievementsPerSon(CompanyId);
            model.CompanyId = CompanyId;

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> SalesPeronAssignPersnWise(int KGSalesTargeID, int CompanyId)
        {
            KGSalesAchivementDetailVm model = await _salesManagementService.GetOfficerAssignPersonWise(KGSalesTargeID, CompanyId);
            return View(model);
        }

        [HttpPost]

        public async Task<ActionResult> SavePersonAchivmentPersonWise(KGSalesCollectedAchivementVm Model)
        {
            var res = await _salesManagementService.SavePersonWeeklyAchivement(Model);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]

        //public async Task<ActionResult> UpdateAchievement(KGSalesCollectedAchivementVm Model)
        //{
        //    var res = await _salesManagementService.UpdateAchivement(Model);
        //    return Json(res, JsonRequestBehavior.AllowGet);
        //}
        [HttpGet]

        public async Task<ActionResult> GetPersonAchivmentPersonWise(long currentKgId)
        {
            var res = await _salesManagementService.GetWeeklyAchivement(currentKgId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> DeletePersonAchivmentPersonWise(long currentKgId, long achievementId)
        {
            var res = await _salesManagementService.DeleteWeeklyAchivement(currentKgId, achievementId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
    }
   