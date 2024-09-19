using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ClientBookingController : Controller
    {
        private ERPEntities db = new ERPEntities();
        IKgreBookingService kgreBookingService = new KgreClientBookingService();
        // GET: ClientBooking
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Booking()
        {
            return View();
        }
        [HttpGet]
        public ActionResult LoadTableDataByid()
        {
            return View();
        }
        [HttpPost]
        public int SaveClientFinal(PlotBooking PaymentInfo, List<ClientsModel> ClientArray, ClientsInfo ClientBasicInfo, string EditClientAutoId, ClientsInfo obj)
        {
            kgreBookingService.SaveClientFinal(PaymentInfo, ClientArray, ClientBasicInfo, EditClientAutoId, obj);
            return 0;
        }
        [HttpPost]
        public ActionResult LoadTableDataByids(int id)
        {
            var lists = kgreBookingService.LoadTableDataByid(id);
            return Json(lists, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult LodeprojectByProidandCliAutoid(int id)
        {

            var list = kgreBookingService.LodeprojectByProidandCliAutoid(id);
            return Json(list, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult LodeploatbyId(int id)
        {

            var list = kgreBookingService.LodeploatbyId(id);
            return Json(list, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult LodeploatbyId1(int ProjectId, string BlockNo)
        {

            var list = kgreBookingService.LodeploatbyId1(ProjectId, BlockNo);
            return Json(list, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult LodeploatbyId2(int ProjectId, string BlockNo, string PloatNo)
        {

            var list = kgreBookingService.LodeploatbyId2(ProjectId, BlockNo, PloatNo);
            return Json(list, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult LodeploatbyId3(int ProjectId, string BlockNo, string PloatNo, string PloatSize)
        {

            var list = kgreBookingService.LodeploatbyId3(ProjectId, BlockNo, PloatNo, PloatSize);
            return Json(list, JsonRequestBehavior.AllowGet);

        }
        public object LoadCostsRate()
        {
            ERPEntities db = new ERPEntities();

            object activeInfo = null;
            activeInfo = (from roll in db.KGRECostSetups
                          select new
                          {
                              roll.NameofCost,
                              roll.Rate
                          });

            return Json(activeInfo, JsonRequestBehavior.AllowGet);
        }

    }
}