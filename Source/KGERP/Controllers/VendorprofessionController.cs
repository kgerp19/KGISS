using KGERP.Service.Implementation.FTP;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KGERP.Service.Implementation.Configuration;
using KGERP.Service.Implementation.VendorProfessions;
using KGERP.Data.Models;
using KGERP.Utility;

namespace KGERP.Controllers
{
    [CheckSession]
    public class VendorprofessionController : Controller
    {
        private readonly IVendorProfession _vendorService;
        public VendorprofessionController( IVendorProfession vendorService)
        {
            _vendorService = vendorService;
        }
        // GET: Vendorprofession
        public ActionResult Index()
        {
            VendorProfessionVm vm= new VendorProfessionVm();
            vm = _vendorService.GetVendorProfessionVm();
            return View(vm);
        }
        [HttpPost]
        public bool SaveVendorProfession(VendorProfession model)
        {           
            var res =_vendorService.AddNewName(model);
            return res;
        }


        [HttpGet]
        public ActionResult Foredit(int id)
        {
            var res = _vendorService.GetNameById(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public bool EditSavevendor(VendorProfession model)
        {
            var res=_vendorService.Editsave(model);
            return res;
        }

        [HttpPost]
        public bool Deletevendor(int id)
        {
            var res = _vendorService.DeletVenodorByiD(id);
            return res;
        }
        


    }
}