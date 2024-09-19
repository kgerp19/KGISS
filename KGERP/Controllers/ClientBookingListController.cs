using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Utility;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ClientBookingListController : Controller
    {
        private ERPEntities db = new ERPEntities();
        IKgreClientBookingList kgreBookingListService = new KgreClientBookingListService();
        // GET: PlotInfo
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoadBookingName()
        {
            var list = kgreBookingListService.LoadBookingListInfo();
            var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        //// GET: PlotInfo/Details/5
        //public ActionResult LoadData(int id)
        //{
        //    var list = kgrePlotInfoService.LoadData(id);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //// GET: PlotInfo/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: PlotInfo/SaveProjectInfo
        //[HttpPost]
        //public int SaveProjectInfo(PloatInfoSetup BasicInfo)
        //{
        //    kgrePlotInfoService.SaveProjectInfo(BasicInfo);
        //    return 0;

        //}

        //// GET: PlotInfo/Edit/5
        //public ActionResult LoadPlotInfo()
        //{
        //    var list = kgrePlotInfoService.LoadPlotInfo();
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //// POST: PlotInfo/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: PlotInfo/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: PlotInfo/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}