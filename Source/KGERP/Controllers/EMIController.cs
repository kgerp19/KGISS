using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using KGERP.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class EMIController : BaseController
    {
        private readonly IEmiService emiService;
        public EMIController(IEmiService emiService)
        {
            this.emiService = emiService;
        }

        // GET: EMI
       
        [HttpGet]
        public ActionResult Index(int? Page_No, string searchText)
        {
            searchText = searchText ?? "";
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }

            var emi = emiService.GetEmiInfoList().Where(x => x.EMINo.ToLower().Contains(searchText.ToLower()) || x.Vendor.Code.ToLower().Contains(searchText.ToLower()) || x.OrderMaster.OrderNo.ToLower().Contains(searchText.ToLower()));
            //var emi = emiService.GetEmiInfoList();

            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(emi.ToPagedList(No_Of_Page, Size_Of_Page));
        }

       
        [HttpGet]
        public ActionResult CreateOrEdit(long id)
        {

            EmiViewModel vm = new EmiViewModel();
            //vm.EMI = new EMIModel() { CompanyId = companyId };
            vm.EMI = emiService.GetEmi(id);
            if (id == 0)
            {
                vm.OrderInvoice = new List<SelectModel>();

            }
            else
            {
                vm.OrderInvoice = emiService.GetOrderinvoiceByCustomer(vm.EMI.VendorId ?? 0, vm.EMI.CompanyId ?? 10);
            }



            return View(vm);
        }

       
        [HttpPost]
        public ActionResult CreateOrEdit(EmiViewModel model)
        {
            model.EMI.EmiDetails = model.EmiDetail;


            var result = emiService.SaveOrEditEmi(model.EMI);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetOrderinvoiceByCustomer(int customerId)
        {
            int companyId = Convert.ToInt32(Session["companyId"]);
            List<SelectModel> orderInvoice = emiService.GetOrderinvoiceByCustomer(customerId, companyId);
            return Json(orderInvoice, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSalesValue(int orderId)
        {
            var salevalue = emiService.GetSalesValue(orderId);
            return Json(salevalue, JsonRequestBehavior.AllowGet);
        }



       
        [HttpGet]
        public PartialViewResult GetEmiDetails(string installmentDate, int noOfInstallment, int installmentAmount)
        {

            //EmiViewModel vm = new EmiViewModel();
            //for (int i=0;i<noOfInstallment;i++)
            //{
            //    DateTime date = installmentDate.AddMonths(i + 1);
            //    ed[i].InstallmentDate = installmentDate.AddMonths(i);
            //    ed[i].InstallmentAmount = installmentAmount;
            //    ed[i].EmiDetailId = 0;

            //}
            List<EmiDetailModel> ed = emiService.GetEmiDetails(Convert.ToDateTime(installmentDate), noOfInstallment, installmentAmount);

            return PartialView("_partialViewEmiDetail", ed);
        }


    }
}