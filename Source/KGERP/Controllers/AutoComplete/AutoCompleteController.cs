using KGERP.Data.Models;
using KGERP.Service.Implementation.AutoComplete;
using KGERP.Utility;
using Ninject.Infrastructure.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.AutoComplete
{
    [CheckSession]
    public class AutoCompleteController : Controller
    {
        // GET: AutoComplete
        private readonly IAutoCompleteService _autocompleteService;
        public AutoCompleteController(IAutoCompleteService autoCompleteService)
        {
            _autocompleteService = autoCompleteService;
        }
        public ActionResult GetAllCompanyAutoComplete(string prefix=null)
        {
            var companyList =  _autocompleteService.GetAllCompanyAutoCompleteAsync(prefix).Result.ToEnumerable();
            return Json(companyList,JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllProjectList(int? companyId)
        {
            var companyList = _autocompleteService.GetAllProjectList(companyId).Result.ToEnumerable();
            return Json(companyList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllEmployeeAutoComplete(string prefix = null)
        {
            var companyList = _autocompleteService.GetAllEmployeeAutoComplete(prefix).Result.ToEnumerable();
            return Json(companyList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllEmployeeAutoCompleteTaskManagement(string prefix = null,int workspaceid = 0)
        {
            var companyList = _autocompleteService.TaskGetAllEmployeeAutoComplete(prefix, workspaceid).Result.ToEnumerable();
            return Json(companyList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllDepartmentAutoComplete(string prefix=null)
        {
            var companyList = _autocompleteService.GetAllDepartmentAutoCompleteAsync(prefix).Result.ToEnumerable();
            return Json(companyList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllGRQSProductCategoryAutoComplete(string prefix = null)
        {           
            var companyList = _autocompleteService.GetAllGRQSProductCategoryAutoCompleteAsync(prefix).Result.ToEnumerable();
            return Json(companyList, JsonRequestBehavior.AllowGet);
        }
        public  ActionResult GetEmployee(int employeeId)
        {
            var employee = _autocompleteService.GetEmployee(employeeId).Result;
            return Json(employee, JsonRequestBehavior.AllowGet);
        }
    }
}