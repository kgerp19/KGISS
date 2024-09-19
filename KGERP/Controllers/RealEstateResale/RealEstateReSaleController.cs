using KGERP.Data.Models;
using KGERP.Service.Implementation.RealStateMoneyReceipt;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Services.Procurement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KGERP.Service.Implementation.FTP;
using KGERP.Service.Implementation.Realestate.BookingAprovalList;
using KGERP.Service.Implementation.Realestate.BookingCollaction;
using KGERP.Service.Implementation.Realestate.CustomersBooking;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.ServiceModel;
using System.Threading.Tasks;
using KGERP.Utility;

namespace KGERP.Controllers.RealEstateResale
{
    [CheckSession]
    public class RealEstateReSaleController : Controller
    {
        private readonly IOrderMasterService orderMasterService;
        private ERPEntities db = new ERPEntities();
        private readonly ConfigurationService _coservice;
        IKgReCrmService kgReCrmService = new KgReCrmService();
        private readonly IVoucherTypeService voucherTypeService;
        private readonly ICostHeadsService _costHeadService;
        private IFTPService _ftpservice;
        private readonly IVendorService _vendorService;
        private readonly IInstallmentTypeService _Service;
        private readonly IGLDLCustomerService gLDLCustomerService;
        private readonly ICustomerBookingService customerBookingService;
        private readonly IBookingInstallmentService bookingInstallmentService;
        private readonly IBookingAprovalStatus bookingAprovalStatus;
        private readonly ITeamService _teamService;
        private readonly ICompanyService _companyService;
        private readonly IBookingCollaction _bookingCollaction;
        private readonly AccountingService _accountingService;
        public RealEstateReSaleController(IBookingInstallmentService bookingInstallmentService,
            ICustomerBookingService customerBookingService,
            IGLDLCustomerService gLDLCustomerService,
            IVoucherTypeService voucherTypeService,
            ICostHeadsService costHeadService,
            IFTPService ftpservice,
            IBookingAprovalStatus bookingAprovalStatus,
            ITeamService teamService,
            AccountingService accountingService,
            IInstallmentTypeService _Service,
            ICompanyService _companyService,
            IBookingCollaction bookingCollaction,
            IVendorService vendorService,
            ConfigurationService coservice)
        {
            this.voucherTypeService = voucherTypeService;
            this._ftpservice = ftpservice;
            this._Service = _Service;
            this._companyService = _companyService;
            this.bookingAprovalStatus = bookingAprovalStatus;
            this.customerBookingService = customerBookingService;
            this.gLDLCustomerService = gLDLCustomerService;
            this.bookingInstallmentService = bookingInstallmentService;
            _costHeadService = costHeadService;
            _teamService = teamService;
            _accountingService = accountingService;
            _bookingCollaction = bookingCollaction;
            _vendorService = vendorService;
            this._coservice = coservice;
        }
        [HttpGet]
        public async Task<ActionResult>ReSaleBooking(int companyId, long CGId)
        {
            GLDLBookingViewModel vm = new GLDLBookingViewModel() { CompanyId = companyId };
            var res = await customerBookingService.CustomerBookingView(companyId, CGId);
            vm.ApplicationDate = DateTime.Now;
            vm.CGId= CGId;
            vm.ProductCategoryId=res.ProductCategoryId;
            vm.ProductSubCategoryId=res.ProductSubCategoryId;
            vm.ProductId = res.ProductId;
            vm.PlotName = res.PlotName;
            vm.PlotSize = res.PlotSize;
            vm.BlockName = res.BlockName;
            vm.ProjectName=res.ProjectName;
            vm.RatePerKatha =res.RatePerKatha;
            vm.LandValue =res.LandValue;
            vm.TotalAmount =res.LandValue;
            vm.RestofAmount =res.LandValue;
            vm.InstallmentAmount =res.LandValue;
            vm.GrandTotalAmount =res.LandValue;
            vm.ApplicationDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            vm.ProductCategoryList = voucherTypeService.GetProductCategoryGldl(companyId);
            vm.LstPurchaseCostHeads = await _costHeadService.GetCostHeadsByCompanyId(companyId);
            vm.BookingInstallmentType = await _costHeadService.GetCompanyBookingInstallmentType(companyId);
            vm.Employee = new SelectList(_teamService.GetTeamListByCompanyId(companyId), "Value", "Text");
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> ReSaleBooking(GLDLBookingViewModel model)
        {
            model.EntryBy = Convert.ToInt32(Session["Id"]);
            model.ApplicationDate = Convert.ToDateTime(model.ApplicationDateString);
            model.BookingDate = Convert.ToDateTime(model.BookingDateString);
            int count = await gLDLCustomerService.GetByclient(model.ClientId);
            model.ClientName = model.CustomerGroupName;
            if (count == 0)
            {
                model.CustomerGroupName = model.CustomerGroupName + "(" + "#001" + ")";
            }
            else
            {
                model.CustomerGroupName = model.CustomerGroupName + "(" + "#00" + (count + 1) + ")";
            }

            var obj = await gLDLCustomerService.PductChangingStatus(model);
            var result = await gLDLCustomerService.CustomerBokking(model);
            if (result.CGId != 0)
            {
                return RedirectToAction("ResaleBookingInformation", new { companyId = model.CompanyId, CGId = model.CGId });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ResaleBookingInformation(int companyId, long CGId)
        {
            var res = await customerBookingService.CustomerBookingView(companyId, CGId);
            res.pRM_Relations = await customerBookingService.PRMRelation();
            var MapEmployeeId = Convert.ToInt32(Session["Id"]);
            if (res.EmployeeId == MapEmployeeId && res.approval.EntryBy > 0 && res.approval.CheckedBy > 0 && res.approval.ApprovedBy > 0)
            {
                res.IsShow = true;
            }
            else
            {
                res.IsShow = false;
            }
            return View(res);
        }

    }
}