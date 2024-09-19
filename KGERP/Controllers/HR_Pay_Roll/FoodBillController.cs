using DocumentFormat.OpenXml.EMMA;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Service.Implementation.ApprovalSystemService;
using KGERP.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static KGERP.Utility.Util.PermissionCollection;

namespace KGERP.Controllers.HR_Pay_Roll
{
    [CheckSession]
    public class FoodBillController : Controller
    {
        public readonly IFoodBillServices foodBillServices;
        private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
        public FoodBillController(IFoodBillServices foodBillServices,IApproval_Service Service, ICompanyService companyService)
        {
            this.foodBillServices = foodBillServices;
            _Service = Service;
            _companyService = companyService;
        }
        // GET: FoodBill
        public async Task<ActionResult> Add_Food_Bills(int companyId)
        {
            FoodBillViewModel model = new FoodBillViewModel();  
            model.YearsList = _Service.YearsListLit();
            model.Year=DateTime.Now.Year;
            model.Month=DateTime.Now.Month-1;
            model.CompanyId = companyId;
           // model.Companies = _companyService.GetAllCompanySelectModels2();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add_Food_Bills(FoodBillViewModel model)
        {
            var result=await foodBillServices.AddBill(model);
            if (result>0)
            {
                return RedirectToAction("Detalis", new { id = result });
            }
            FoodBillViewModel model2 = new FoodBillViewModel();
            model2.YearsList = _Service.YearsListLit();
            model2.Year = DateTime.Now.Year;
            model2.Month = DateTime.Now.Month - 1;
            model2.Companies = _companyService.GetAllCompanySelectModels2();
            model2.CompanyId=model.CompanyId;
            return View(model2);

        }

        [HttpPost]
        public async Task<ActionResult> AddBillDetalis(FoodBillViewModel model)
        {
            model.DetaliesObject.FoodBillId = model.FoodBillId;
                        var result=await foodBillServices.AddBillDetalis(model.DetaliesObject);
            if (result>0)
            {
                return RedirectToAction("Detalis", new { id = result });
            }
            return View(model); 
            
        }  
        
        [HttpPost]
        public async Task<ActionResult> UpdateBillDetalis(FoodBillViewModel model)
        {
            model.DetaliesObject.FoodBillId = model.FoodBillId;
            var result = await foodBillServices.UpdateBillDetalis(model.DetaliesObject);
            if (result > 0)
            {
                return RedirectToAction("Detalis", new { id = result });
            }
            return View(model); 
            
        }

        public async Task<ActionResult> Delete(long id)
        {
            var result = await foodBillServices.Delete(id);
            return RedirectToAction("Detalis", new { id = result });
        }

        public async Task<ActionResult> Detalis(long id)
        {
            var result = await foodBillServices.Detalis(id);
            return View(result);
        }
        public async Task<ActionResult> FoodBillList(int companyId)
        {
            var result = await foodBillServices.BillList(companyId);
            return View(result);
        }

        public async Task<JsonResult> GetEmployeeinfo(int companyId)
        {
        var result=await foodBillServices.employees(companyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAllEmployeeinfoforConfiguration(int companyId)
        {
            var result = await foodBillServices.AllEmployeesfoConfiguration(companyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetAllEmployeeinfo(int companyId)
        {
            var result = await foodBillServices.AllEmployees(companyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> GetAllEmployeeinfoForFine(int companyId)
        {
            var result = await foodBillServices.AllEmployeesForFine(companyId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}