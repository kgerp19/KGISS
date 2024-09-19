using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Math;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using KGERP.Service.HR_Pay_Roll_Service.ParollServices;
using KGERP.Service.HR_Pay_Roll_Service.PRoll_Cash_Payment;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.ApprovalSystemService;
using KGERP.Service.Interface;
using KGERP.Utility;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers.HR_Pay_Roll
{
    [CheckSession]
    public class PayrollController : Controller
    {
        private readonly IApproval_Service _approvalService;

        public readonly IPayrollServices parollServices;
        private readonly ICompanyService _companyService;
        IBankBranchService _bankBranchService = new BankBranchService();
        public PayrollController(IPayrollServices parollServices, IApproval_Service Service, ICompanyService companyService, IApproval_Service approval_Service)
        {
            this.parollServices = parollServices;
            _companyService = companyService;
            _approvalService = approval_Service;
        }


        public async Task<ActionResult> PayRollByCompanyId(int companyId)
        {
            var result = await parollServices.PayRollByCompanyId(companyId);
            return View(result);
        }


     



        public async Task<ActionResult> AddPayRoll(int companyId)
        {
            VmPRollPayRollDetail model = new VmPRollPayRollDetail();
            model.YearsList = _approvalService.YearsListLit();
            model.Year = DateTime.Now.Year;
            model.Month = DateTime.Now.Month - 1;
            model.FestivalDateStr = DateTime.Now.ToString("dd-MM-yyyy");
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddPayRoll(VmPRollPayRollDetail model)
        {
            var result = await parollServices.AddPayRoll(model);
            if (result > 0)
            {
                return RedirectToAction("PayRollByCompanyId", new { companyId = model.CompanyId });
            }
            return View(model);

        }

        public ActionResult PayrollProcess(int CompanyId, long PayRollId, int Month, int Year)
        {
            int data = parollServices.ProcessPayroll(CompanyId, PayRollId, Month, Year);
            return RedirectToAction(nameof(PayRollByCompanyId), new { companyId = CompanyId });
        }

        public async Task<ActionResult> PayRollDetails(int companyId,long PayRollId)
        {
            var result = await parollServices.CompanyWiseSalaryDetails(companyId, PayRollId);            
            ViewBag.BranchList = _bankBranchService.GetBankBranchByCompanyId(companyId);
            return View(result);
        }



        public async Task<ActionResult> PayRollBonusDetails(int companyId, long PayRollId)
        {
            var result = await parollServices.CompanyWiseBonusDetails(companyId, PayRollId);
            ViewBag.BranchList = _bankBranchService.GetBankBranchByCompanyId(companyId);
            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> PayRollDetails(VmSalaryDetails model)
        {
            if (model.SalaryReportType == Utility.SalaryReportTypeEnum.BankAdviceSheet)
            {
                var employeeIdList = model.SalaryList.Where(x => x.BankSheetFlag).Select(x => x.Id).ToList();
                string employeeIdCSV = "";
                if (employeeIdList != null && employeeIdList.Count() > 0)
                {

                    employeeIdCSV = string.Join(",", employeeIdList);
                }
                return RedirectToAction("BankAdviceSheetReport", "Report", new
                {
                    companyId = model.CompanyId,
                    payRollId = model.PayRollId,
                    reportType = model.ReportTypeId == 1 ?  ReportType.EXCEL: ReportType.PDF,
                    employeeIds = employeeIdCSV,
                    bankBranchId = model.BankBranchId,
                    salaryDate = model.SalaryDisbursementDate
                }) ;
            }
            if (model.SalaryReportType == SalaryReportTypeEnum.PaymentDone)
            {

            }
            if (model.SalaryReportType == SalaryReportTypeEnum.BankAdviceAndPaymentDone)
            {
                var employeeIdList = model.SalaryList.Where(x => x.BankSheetFlag).Select(x => x.Id).ToList();
                string employeeIdCSV = "";
                if (employeeIdList != null && employeeIdList.Count() > 0)
                {

                    employeeIdCSV = string.Join(",", employeeIdList);
                }
                return RedirectToAction("BankAdviceSheetReport", "Report", new
                {
                    companyId = model.CompanyId,
                    payRollId = model.PayRollId,
                    reportType = ReportType.PDF,
                    employeeIds = employeeIdCSV,
                    bankBranchId = model.BankBranchId
                });
            }
            //var result = await parollServices.ProcessBankSheetAndPayment(model);


            return RedirectToAction(nameof(PayRollDetails), new { companyId = model.CompanyId, PayRollId = model.PayRollId });

        }
        [HttpPost]


        public async Task<ActionResult> PayRollBonusDetails(VmSalaryDetails model)
        {
            if (model.SalaryReportType == Utility.SalaryReportTypeEnum.BankAdviceSheet)
            {
                var employeeIdList = model.SalaryList.Where(x => x.BankSheetFlag).Select(x => x.Id).ToList();
                string employeeIdCSV = "";
                if (employeeIdList != null && employeeIdList.Count() > 0)
                {

                    employeeIdCSV = string.Join(",", employeeIdList);
                }
                return RedirectToAction("BonusAdviceSheetReport", "Report", new
                {
                    companyId = model.CompanyId,
                    payRollId = model.PayRollId,
                    reportType = model.ReportTypeId == 1 ? ReportType.EXCEL : ReportType.PDF,
                    employeeIds = employeeIdCSV,
                    bankBranchId = model.BankBranchId,
                    salaryDate = model.SalaryDisbursementDate
                });
            }
            if (model.SalaryReportType == SalaryReportTypeEnum.PaymentDone)
            {

            }
            if (model.SalaryReportType == SalaryReportTypeEnum.BankAdviceAndPaymentDone)
            {
                var employeeIdList = model.SalaryList.Where(x => x.BankSheetFlag).Select(x => x.Id).ToList();
                string employeeIdCSV = "";
                if (employeeIdList != null && employeeIdList.Count() > 0)
                {

                    employeeIdCSV = string.Join(",", employeeIdList);
                }
                return RedirectToAction("BonusAdviceSheetReport", "Report", new
                {
                    companyId = model.CompanyId,
                    payRollId = model.PayRollId,
                    reportType = ReportType.PDF,
                    employeeIds = employeeIdCSV,
                    bankBranchId = model.BankBranchId
                });
            }
            //var result = await parollServices.ProcessBankSheetAndPayment(model);


            return RedirectToAction(nameof(PayRollBonusDetails), new { companyId = model.CompanyId, PayRollId = model.PayRollId });

        }
        public ActionResult Test()
        {
            return Content("OK");
        }



        public async Task<ActionResult> PayRollBnusCompanyId(int companyId)
        {
            var result = await parollServices.PayRollBonus(companyId);
            return View(result);
        }

        public async Task<ActionResult> AddPayRollBonus(int companyId)
        {
            PRoll_BonusVm model = new PRoll_BonusVm();
            model.YearsList = _approvalService.YearsListLit();
            model.Year = DateTime.Now.Year;
            model.Month = DateTime.Now.Month - 1;

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> AddPayRollBonus(PRoll_BonusVm model)
        {
            var result = await parollServices.AddPayRollBonus(model);
            if (result > 0)
            {
                return RedirectToAction("PayRollBnusCompanyId", new { companyId = model.CompanyId });
            }
            return View(model);

        }



   
        public async Task<ActionResult> DeletePayRollBonus(int CompanyId,long BonusId)
        {
            var result = await parollServices.DeleteBonus(BonusId);
                return RedirectToAction("PayRollBnusCompanyId", new { companyId = CompanyId });
     

        }
        public async Task<ActionResult> EditPayRollBonus(long bonusId)

        {
            PRoll_BonusVm model = new PRoll_BonusVm();
            model = await parollServices.EditPayRollBonus(bonusId);
            model.YearsList = _approvalService.YearsListLit();
           
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> EditPayRollBonus(PRoll_BonusVm model)

        {
            var obj= await parollServices.UpdatePayRollBonus(model);

            return RedirectToAction("PayRollBnusCompanyId", new { companyId = model.CompanyId });
        }


        [HttpGet]
        public async Task<ActionResult> AdvancetypeList(int CompanyId)
        {
            PRoll_AdvanceTypeVm vm = new PRoll_AdvanceTypeVm();
            vm= await parollServices.PayRollAdvanceType(CompanyId);
            return View(vm);
        }


        [HttpGet]
        public async Task<ActionResult> Create(int CompanyId,int? AdvanceTypeId)

        {
            PRoll_AdvanceTypeVm vm = new PRoll_AdvanceTypeVm();
            if (AdvanceTypeId > 0)
            {
                vm= await parollServices.Edit(AdvanceTypeId);
            }
            vm.CompanyId = CompanyId;
            return View(vm);
        }
        [HttpPost]
        public async Task<ActionResult> Create(PRoll_AdvanceTypeVm model)

        {
          
            var obj = await parollServices.AddPayRollAdvanceType(model);
            return RedirectToAction("AdvancetypeList", new { companyId = model.CompanyId });
        }


        [HttpGet]
        public async Task<ActionResult> Delete(int CompanyId, int AdvanceTypeId)

        {
            PRoll_AdvanceTypeVm vm = new PRoll_AdvanceTypeVm();
            var obj = await parollServices.DeleteAdvanceType(AdvanceTypeId);
            return RedirectToAction("AdvancetypeList", new { companyId = CompanyId });
        }

    }
}