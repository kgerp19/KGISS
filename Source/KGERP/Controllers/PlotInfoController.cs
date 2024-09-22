using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Utility;
using System;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class PlotInfoController : Controller
    {
        private ERPEntities db = new ERPEntities();
        IKgrePlotInfoService kgrePlotInfoService = new KgrePlotService();
        // GET: PlotInfo
        public ActionResult Index(int? companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            return View();
        }

        [HttpPost]
        public ActionResult LoadProjectName()
        {
            int companyId = 0;
            if (Session["CompanyId"] != null)
            {
                companyId = Convert.ToInt32(Session["CompanyId"]);
            }
            var list = kgrePlotInfoService.LoadProjectName(companyId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadPlotStatus()
        {
            var list = kgrePlotInfoService.LoadPlotStatus();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        // GET: PlotInfo/Details/5
        public ActionResult LoadData(int id)
        {
            var list = kgrePlotInfoService.LoadData(id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        // GET: PlotInfo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlotInfo/SaveProjectInfo
        [HttpPost]
        //public int SaveProjectInfo(PloatInfoSetup BasicInfo)
        public int SaveProjectInfo(KGREPlot BasicInfo)
        {
            kgrePlotInfoService.SaveProjectInfo(BasicInfo);
            return 0;
        }

        // GET: PlotInfo/Edit/5
        public ActionResult LoadPlotInfo()
        {
            var list = kgrePlotInfoService.LoadPlotInfo();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        // POST: PlotInfo/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PlotInfo/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PlotInfo/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
