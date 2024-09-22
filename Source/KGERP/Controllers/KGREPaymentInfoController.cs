using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Utility;
using System;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class KGREPaymentInfoController : Controller
    {
        //private ERPEntities db = new ERPEntities();
        //IKGREPaymentInfoService kgrePaymentInfoService = new KGREPaymentInfoService();
        //// GET: KGREPaymentInfo
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //// GET: KGREPaymentInfo/Details/5
        //public ActionResult LoadClientPayInfoById(string id)
        //{
        //    var list = kgrePaymentInfoService.LoadClientPayInfoById(id);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult LoadPayInfoById(string Autoid)
        //{
        //    var list = kgrePaymentInfoService.LoadPayInfoById(Autoid);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //// GET: KGREPaymentInfo/Create
        //public ActionResult CalculateNextPayDateFrom1(string id)
        //{
        //    var list = kgrePaymentInfoService.CalculateNextPayDateFrom1(id);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult ClientPaymentHistory(string id)
        //{
        //    var list = kgrePaymentInfoService.ClientPaymentHistory(id);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
        ////public void EditData(float PayDueAmount, int Id)
        ////{
        ////    kgrePaymentInfoService.Edit(PayDueAmount , Id);
        ////}
        //[HttpPost]
        //public int PaymentInfos(PlotBooking PaymentInfo, string date)
        //{
        //    DateTime dates = DateTime.ParseExact(date, "yyyy-dd-MM", null);
        //    ERPEntities db = new ERPEntities();
        //    PlotBooking clientInfoTwoModel = new PlotBooking();
        //    clientInfoTwoModel.ClientAutoId = PaymentInfo.ClientAutoId;
        //    clientInfoTwoModel.RestOfAmount = PaymentInfo.RestOfAmount;
        //    clientInfoTwoModel.PayType = PaymentInfo.PayType;
        //    clientInfoTwoModel.BankName = PaymentInfo.BankName;
        //    clientInfoTwoModel.ChaqueNo = PaymentInfo.ChaqueNo;
        //    clientInfoTwoModel.BokkingMoney = PaymentInfo.BokkingMoney;

        //    clientInfoTwoModel.Booking_Date = dates;
        //    clientInfoTwoModel.InstallMentAmount = PaymentInfo.InstallMentAmount;
        //    clientInfoTwoModel.NoOfInstallment = PaymentInfo.NoOfInstallment;
        //    db.PlotBookings.Add(clientInfoTwoModel);
        //    db.SaveChanges();

        //    return 0;
        //}
        //// POST: KGREPaymentInfo/Create
        //[HttpPost]
        //public int Edits(float PayDueAmount, int Id)
        //{
        //    //KGREPaymentInfoController ss = new KGREPaymentInfoController();
        //    //ss.EditData(PayDueAmount, Id);
        //    kgrePaymentInfoService.Edit(PayDueAmount, Id);
        //    return 0;

        //}

        //// GET: KGREPaymentInfo/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: KGREPaymentInfo/Edit/5
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

        //// GET: KGREPaymentInfo/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: KGREPaymentInfo/Delete/5
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
