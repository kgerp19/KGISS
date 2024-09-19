using DocumentFormat.OpenXml.EMMA;
using KGERP.Service.Configuration;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using KGERP.Service.HR_Pay_Roll_Service.PRoll_AdvanceCash;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Interface;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static KGERP.Controllers.Custom_Authorization.ParentAuthorizedAttribute;

namespace KGERP.Controllers.HR_Pay_Roll
{
    [CheckSession]
    public class PRoll_Advance_CashController : Controller
    {
        private readonly IPRoll_Advance_CashService iPRoll_Advance;
        private readonly ICompanyService _companyService;
        private readonly ICostHeadsService _costHeadService;
        private readonly IBookingInstallmentService bookingInstallmentService;
        // GET: PRoll_Advance_Cash
        public PRoll_Advance_CashController(
            ICostHeadsService _costHeadService,
            IPRoll_Advance_CashService iPRoll_Advance, 
            ICompanyService _companyService,
            IBookingInstallmentService bookingInstallmentService
           )
        {
            this.iPRoll_Advance = iPRoll_Advance;
            this._companyService= _companyService;  
            this._costHeadService = _costHeadService;  
            this.bookingInstallmentService = bookingInstallmentService;  
        }
        public async Task<ActionResult> AdvanceList(int companyId)
        {
            PRoll_AdvanceViewModel obj=new PRoll_AdvanceViewModel();
            obj= await iPRoll_Advance.AdvanceList(companyId);
            obj.CompanyId = companyId;
            return View(obj);
        }
        public async Task<ActionResult> AddAdvance(int companyId)
        {
            PRoll_AdvanceViewModel model = new PRoll_AdvanceViewModel();
            model.Companies = _companyService.GetAllCompanySelectModels2();
            model.InstallmentType = await _costHeadService.GetCompanyBookingInstallmentType(0);
            model.AdVanceType = await iPRoll_Advance.GetAdvanceTypeType(companyId); 
            model.CompanyId = companyId;    
            model.AdvanceDate = DateTime.Now;
            model.AdvanceDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            model.InstallmentStartDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            return View(model);
        }

      [HttpPost]
        public async Task<ActionResult> AddAdvance(PRoll_AdvanceViewModel pRoll)
        {
            var res = await iPRoll_Advance.AddAdvance(pRoll);
            if (res > 0)
            {
                return RedirectToAction("AdvanceDatlis", new { id = res });
            }
            return View(pRoll);
        }
        public async Task<ActionResult> AdvanceDatlis(long id)
        {
            var res = await iPRoll_Advance.AdvanceDetalis(id);
            return View(res);
        }  
        public async Task<ActionResult> Delete(long id)
        {
            var res = await iPRoll_Advance.AdvanceDelete(id);
            return RedirectToAction("AdvanceDatlis", new { id = res });
        }

        [HttpPost]
        public async Task<JsonResult> InstallmentSchedule(int conmpanyId, int installmentId, int NoOfInstallment, decimal restofAmount, string BookingDate)
        {

            if (DateTime.TryParse(BookingDate, out DateTime date) == false)
            {
                date = DateTime.Now;
            }
            var res = await bookingInstallmentService.GolobalInstallmentSchedule(conmpanyId, installmentId, NoOfInstallment, restofAmount, date);
            foreach (var item in res.LstSchedules)
            {
                TextInfo info = CultureInfo.CurrentCulture.TextInfo;
                item.Title = info.ToTitleCase(item.Title);

            }

            return Json(res);
        }

        public async Task<ActionResult> AddAdvanceType(int companyId)
        {
            VMAdvanceType vmCommonUnit = new VMAdvanceType();
            vmCommonUnit = await Task.Run(() => iPRoll_Advance.GetAdvanceType(companyId));
            return View(vmCommonUnit);
        }

        [HttpPost]
        public async Task<ActionResult> AddAdvanceType(VMAdvanceType vMAdvanceType)
        {

            if (vMAdvanceType.ActionEum == ActionEnum.Add)
            {
                //Add
                await iPRoll_Advance.AdvanceTypeAdd(vMAdvanceType);
            }
            else if (vMAdvanceType.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await iPRoll_Advance.AdvanceTypeEdit(vMAdvanceType);
            }
            else if (vMAdvanceType.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await iPRoll_Advance.AdvanceTypeDelete(vMAdvanceType.AdvanceTypeId);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(AddAdvanceType), new { companyId = vMAdvanceType.CompanyId });
        }

    }
}