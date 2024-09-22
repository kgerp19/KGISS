using KGERP.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static KGERP.Service.Implementation.HrDesignationService;

namespace KGERP.Controllers.HRConfiguration
{
    [CheckSession]
    public class DesignationController : Controller
    {
        private readonly HrDesignationService hrDesignationService;
        public DesignationController(HrDesignationService hrDesignationService)
        {
            this.hrDesignationService = hrDesignationService;
        }
        // GET: Designation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add_Designation()
        {
            DesignationViewModel model = new DesignationViewModel();
            model = hrDesignationService.desilist(model);
            return View(model);
        }

        [HttpPost]
         public ActionResult Add_Designation(DesignationViewModel model)
        {
            ModelState.Clear();
            var res = hrDesignationService.checckname(model);
            if (res)
            {
                var result =hrDesignationService.AddDes(model);
                return RedirectToAction(nameof(Add_Designation));
            }
            ModelState.AddModelError("Name", "Already exists");
            model = hrDesignationService.desilist(model);
            return View(model);
        }

    }
}