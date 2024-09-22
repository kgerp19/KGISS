using KGERP.Service.Interface;
using KGERP.Utility;
using KGERP.ViewModel;
using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ComplainManagementController : BaseController
    {
        public IComplainManagementService complainService;

        public ComplainManagementController(IComplainManagementService complainService)
        {
            this.complainService = complainService;

        }
        // GET: ComplainManagement
        public ActionResult Index()
        {
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }

            return View();
        }


        public ActionResult ManagerActionIndex()
        {
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }

            return View();
        }

        public ActionResult EmployeeActionIndex()
        {
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }

            return View();
        }



        
        [HttpPost]
        public JsonResult GetComplainListForAction()
        {
            var companyId = Convert.ToInt32(Session["companyId"]);

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];



            var data = complainService.GetAllComplain(start, length, searchValue, sortColumnName, sortDirection, companyId);
            var complain = data.complain;
            int totalRows = complain.Count;
            complain = complain.Where(x => x.IsActionTaked == 1).ToList();
            if (!string.IsNullOrEmpty(searchValue))//filter
            {
                complain = complain.Where(x => x.CustomerName.ToLower().Contains(searchValue.ToLower())
                                          || x.MobileNo.Contains(searchValue)
                                          || x.ComplainTypeName.ToLower().Contains(searchValue.ToLower())
                                          || x.ComplainDescription.ToLower().Contains(searchValue.ToLower())
                                          || x.InvoiceNo.ToLower().Contains(searchValue.ToLower())
                                          || x.CreatedBy.ToLower().Contains(searchValue.ToLower())).ToList();
            }
            int totalRowsAfterFiltering = complain.Count;
            //sorting
            complain = complain.OrderBy(sortColumnName + " " + sortDirection).ToList();

            //paging
            complain = complain.Skip(start).Take(length).ToList();


            //var json = JsonConvert.SerializeObject(complain, Formatting.None,
            //           new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { data = complain, draw = Request["draw"], recordsTotal = totalRows, recordsFiltered = totalRowsAfterFiltering }, JsonRequestBehavior.AllowGet);

        }


        
        [HttpPost]
        public JsonResult GetComplainList()
        {
            var companyId = Convert.ToInt32(Session["companyId"]);

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];



            var data = complainService.GetAllComplain(start, length, searchValue, sortColumnName, sortDirection, companyId);
            var complain = data.complain;
            int totalRows = complain.Count;
            if (!string.IsNullOrEmpty(searchValue))//filter
            {
                complain = complain.Where(x => x.CustomerName.ToLower().Contains(searchValue.ToLower())
                                          || x.MobileNo.Contains(searchValue)
                                          || x.ComplainTypeName.ToLower().Contains(searchValue.ToLower())
                                          || x.ComplainDescription.ToLower().Contains(searchValue.ToLower())
                                          || x.InvoiceNo.ToLower().Contains(searchValue.ToLower())
                                          || x.ComplainStatus.ToLower().Contains(searchValue.ToLower())
                                          || x.CreatedBy.ToLower().Contains(searchValue.ToLower())).ToList();
            }
            int totalRowsAfterFiltering = complain.Count;
            //sorting
            //complain = complain.OrderBy(sortColumnName + " " + sortDirection).ToList();


            //paging
            complain = complain.Skip(start).Take(length).ToList();


            //var json = JsonConvert.SerializeObject(complain, Formatting.None,
            //           new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Json(new { data = complain, draw = Request["draw"], recordsTotal = totalRows, recordsFiltered = totalRowsAfterFiltering }, JsonRequestBehavior.AllowGet);

        }

        
        [HttpGet]
        public ActionResult CreateOrEdit(int id = 0, string status = null)
        {

            string layout = Request.QueryString["Layout"];
            //string isNew = status;
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            ViewBag.Layout = layout;
            ViewBag.Status = status;
            ComplainViewModel vm = new ComplainViewModel();
            vm.Complain = complainService.GetComplain(id);
            vm.ComplainType = complainService.GetComplainType(companyId);
            if (layout != null)
            {
                return View("CreateOrEditWithLayout", vm);
            }
            else
            {
                return View(vm);
            }


        }
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEditWithLayout(ComplainViewModel vm)
        {

            complainService.SaveOrEdit(vm.Complain);
            return RedirectToAction("Index");

        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrEdit(ComplainViewModel vm)
        {


            if (vm.Complain.ComplainId <= 0)
            {
                complainService.SaveOrEdit(vm.Complain);
                return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                complainService.SaveOrEdit(vm.Complain);

                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            bool result = complainService.DeleteComplain(id);
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);

        }
    }
}