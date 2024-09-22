using Ganss.Excel;
using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using KGERP.Service.HR_Pay_Roll_Service.PRoll_Cash_Payment;
using KGERP.Service.Implementation.ApprovalSystemService;
using KGERP.Service.Interface;
using KGERP.Utility;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace KGERP.Controllers.HR_Pay_Roll
{
    [CheckSession]
    public class PRoll_MobileBillController : Controller
    {
        public readonly IMobileBillServices mobileBillServices;
        private readonly IApproval_Service _Service;
        private readonly ICompanyService _companyService;
        public PRoll_MobileBillController(IMobileBillServices mobileBillServices, IApproval_Service Service, ICompanyService companyService)
        {
            this.mobileBillServices = mobileBillServices;
            _Service = Service;
            _companyService = companyService;
        }

        public async Task<ActionResult> Add_Mobile_Bills(int companyId)
        {
            MoobileBillViewModel model = new MoobileBillViewModel();
            model.YearsList = _Service.YearsListLit();
            model.Year = DateTime.Now.Year;
            model.Month = DateTime.Now.Month - 1;
            model.CompanyId = companyId;
            //model.Companies = _companyService.GetAllCompanySelectModels2();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add_Mobile_Bills(MoobileBillViewModel model)
        {
            var result = await mobileBillServices.AddBill(model);
            if (result > 0)
            {
                return RedirectToAction("Detalis", new { id = result });
            }
            MoobileBillViewModel model2 = new MoobileBillViewModel();
            model2.YearsList = _Service.YearsListLit();
            model2.Year = DateTime.Now.Year;
            model2.Month = DateTime.Now.Month - 1;
            model2.Companies = _companyService.GetAllCompanySelectModels2();
            model2.CompanyId = model.CompanyId;
            return View(model2);

        }
        [HttpPost]
        public async Task<ActionResult> UploadMobileBill(MoobileBillViewModel model)
        {
            try
            {
                ERPEntities _context = new ERPEntities();
                var userId = Common.GetUserId();
                if (model.Month != 0 && model.Year != 0 && model.MobileBillExcel != null && model.MobileBillExcel.ContentLength > 0)
                {
                    var dataList = new ExcelMapper(model.MobileBillExcel.InputStream).Fetch<ExcelMobileBillMapperVM>().ToList();
                    var masterList = _context.PRoll_MobileBill.Where(x => x.Month == model.Month && x.Year == model.Year && x.IsActive).ToList();
                    foreach (var item in dataList)
                    {
                        var employee = (from e in _context.Employees
                                        join c in _context.Companies on e.CompanyId equals c.CompanyId
                                        where e.EmployeeId == item.EmployeeId
                                        select new
                                        {
                                            id = e.Id,
                                            employeeId = e.EmployeeId,
                                            companyId = c.CompanyId
                                        }).FirstOrDefault();

                        if (employee != null)
                        {
                            var master = masterList.FirstOrDefault(x => x.CompanyId == employee.companyId);
                            if (master != null)
                            {
                                var exist = _context.PRoll_MobileBillDetail.FirstOrDefault(x => x.EmployeeId == employee.id && x.IsActive);
                                if (exist != null)
                                {
                                    exist.Amount = item.Amount;
                                    exist.MobileBillId = master.MobileBillId;
                                    exist.ModifiedBy = userId;
                                    exist.ModifiedDate = DateTime.Now;
                                }
                                else
                                {
                                    var data = new PRoll_MobileBillDetail()
                                    {
                                        EmployeeId = employee.id,
                                        IsActive = true,
                                        Amount = item.Amount,
                                        CreatedBy = userId,
                                        CreatedDate = DateTime.Now,
                                        MobileBillId = master.MobileBillId
                                    };
                                    _context.PRoll_MobileBillDetail.Add(data);
                                }

                                _context.SaveChanges();
                            }
                            else
                            {
                                var mobileBillMaster = new PRoll_MobileBill()
                                {
                                    CompanyId = employee.companyId,
                                    IsActive = true,
                                    Month = model.Month,
                                    Year = model.Year,
                                    CreatedBy = userId,
                                    CreatedDate = DateTime.Now
                                };

                                _context.PRoll_MobileBill.Add(mobileBillMaster);
                                if (_context.SaveChanges() > 0)
                                {
                                    var data = new PRoll_MobileBillDetail()
                                    {
                                        EmployeeId = employee.id,
                                        IsActive = true,
                                        Amount = item.Amount,
                                        CreatedBy = userId,
                                        CreatedDate = DateTime.Now,
                                        MobileBillId = mobileBillMaster.MobileBillId
                                    };

                                    _context.PRoll_MobileBillDetail.Add(data);
                                    _context.SaveChanges();
                                }

                            }
                        }


                    }

                }
                return Json(new { success = true, message = "Mobile bill uploaded successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading mobile bill", error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddBillDetalis(MoobileBillViewModel model)
        {
            model.DetaliesObject.MobileBillId = model.MobileBillId;
            var result = await mobileBillServices.AddBillDetalis(model.DetaliesObject);
            if (result > 0)
            {
                return RedirectToAction("Detalis", new { id = result });
            }
            return RedirectToAction("Detalis", new { id = model.MobileBillId });

        }

        [HttpPost]
        public async Task<ActionResult> UpdateBillDetalis(MoobileBillViewModel model)
        {
            model.DetaliesObject.MobileBillId = model.MobileBillId;
            var result = await mobileBillServices.UpdateBillDetalis(model.DetaliesObject);
            if (result > 0)
            {
                return RedirectToAction("Detalis", new { id = result });
            }
            return View(model);
        }

        public async Task<ActionResult> Delete(long id)
        {
            var result = await mobileBillServices.Delete(id);
            return RedirectToAction("Detalis", new { id = result });
        }

        public async Task<ActionResult> Detalis(long id)
        {
            var result = await mobileBillServices.Detalis(id);
            return View(result);
        }
        public async Task<ActionResult> MobileBillList(int companyId)
        {
            var result = await mobileBillServices.BillList(companyId);
            return View(result);
        }

        [HttpGet]
        public ActionResult GetEmployeeinfo(int companyId, int? year, int? month)
        {
            var result =  mobileBillServices.GetMobileBills(companyId,year, month);
            return Json(result, JsonRequestBehavior.AllowGet);
        }




    }
}