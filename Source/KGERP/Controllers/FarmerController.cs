using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class FarmerController : BaseController
    {
        private ERPEntities db = new ERPEntities();
        private readonly IFarmerService farmerService;
        private readonly IZoneService zoneService;
        private readonly IOfficerAssignService officerAssignService;
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        public FarmerController(IFarmerService farmerService, IZoneService zoneService, IOfficerAssignService officerAssignService)
        {
            this.farmerService = farmerService;
            this.zoneService = zoneService;
            this.officerAssignService = officerAssignService;
        }

        
        public ActionResult FarmerIndex(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            return View();
        }

        
        [HttpPost]
        public ActionResult Farmers(int companyId)
        {
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];



            IQueryable<FarmerModel> farmerList = farmerService.GetFarmers(companyId, searchValue, out int count);
            int totalRows = count;

            int totalRowsAfterFiltering = farmerList.Count();


            //sorting
            farmerList = farmerList.OrderBy(sortColumnName + " " + sortDirection);
            farmerList= farmerList.OrderByDescending(x=>x.FarmerId);
            //paging
            farmerList = farmerList.Skip(start).Take(length);


            return Json(new { data = farmerList, draw = Request["draw"], recordsTotal = totalRows, recordsFiltered = totalRowsAfterFiltering }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public ActionResult CreateOrEdit(int id = 0)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            FarmerViewModel vm = new FarmerViewModel();
            vm.Farmer = farmerService.GetFarmer(id);
            vm.Zones = zoneService.GetZoneSelectModels(companyId);
            if (vm.Farmer.OfficerId == null)
            {
                vm.Officers = new List<SelectModel>();
            }
            else
            {
                vm.Officers = officerAssignService.GetOfficerSelectModelsByZone(vm.Farmer.ZoneId ?? 1);
            }
            return View(vm);
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(FarmerViewModel vm)
        {
            if (vm.Farmer.FarmerId <= 0)
            {
                farmerService.SaveFarmer(0, vm.Farmer);
                //return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                return RedirectToAction("FarmerIndex", "Farmer", new { companyId = vm.Farmer.CompanyId });
            }
            else
            {
                farmerService.SaveFarmer(vm.Farmer.FarmerId, vm.Farmer);
                //return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                return RedirectToAction("FarmerIndex", "Farmer", new { companyId = vm.Farmer.CompanyId });
            }

        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            bool result = farmerService.DeleteFarmer(id);
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);

        }

        #region Upload Farmer

        #endregion
    }
}