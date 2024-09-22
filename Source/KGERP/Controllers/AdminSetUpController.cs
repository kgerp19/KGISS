using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class AdminSetUpController : Controller
    {
        IAdminSetUpService adminSetUpService = new AdminSetUpService();
        IEmployeeService employeeService = new EmployeeService(new ERPEntities());
        public ActionResult Index(int? Page_No, string searchText)
        {
            AdminSetUpModel adminSetUpModel = new AdminSetUpModel();
            adminSetUpModel = adminSetUpService.GetAdminSetUps();
           
            return View(adminSetUpModel);
        }


        //public ActionResult CreateOrEdit(long id)
        //{
        //    AdminSetUpViewModel vm = new AdminSetUpViewModel
        //    {
        //        AdminSetUp = adminSetUpService.GetAdminSetUp(id),
        //        StatusSelectModels = adminSetUpService.StatusSelectModels(),
        //        Employees = employeeService.GetEmployeeSelectModels()
        //    };
        //    return View(vm);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateOrEdit(AdminSetUpViewModel vm)
        //{
        //    if (vm.AdminSetUp.AdminId <= 0)
        //    {
        //        adminSetUpService.SaveAdminSetUp(0, vm.AdminSetUp);
        //    }

        //    else
        //    {
        //        adminSetUpService.SaveAdminSetUp(vm.AdminSetUp.AdminId, vm.AdminSetUp);
        //    }
        //    return RedirectToAction("Index");
        //}





    }
}
