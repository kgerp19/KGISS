using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.Vendor;
using KGERP.Utility;
using KGERP.ViewModel;
using Newtonsoft.Json;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class VendorController : BaseController
    {
        private readonly IDistrictService districtService;
        private readonly IUpazilaService upazilaService;
        private readonly IVendorService vendorService;
        private readonly IPaymentService paymentService;
        private readonly IZoneService zoneService;
        private readonly ISubZoneService subZoneService;
        private readonly IHeadGLService headGLService;
        public VendorController(IHeadGLService headGLService, IDistrictService districtService,
            IUpazilaService upazilaService,
            ISubZoneService subZoneService, IVendorService vendorService,
            IPaymentService paymentService, IZoneService zoneService)
        {
            this.vendorService = vendorService;
            this.paymentService = paymentService;
            this.zoneService = zoneService;
            this.subZoneService = subZoneService;
            this.districtService = districtService;
            this.upazilaService = upazilaService;
            this.headGLService = headGLService;
        }

        
        [HttpGet]
        public ActionResult AllCustomerIndex(int companyId)
        {
            if (companyId > 0)
            {
                Session["CompanyId"] = companyId;
            }
            return View();
        }


        
        [HttpPost]
        public ActionResult AllCustomer(int customerType)
        {
            //Server Side Parameter
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];



            List<VendorModel> customerList = vendorService.GetAllCustomers(customerType);
            int totalRows = customerList.Count;
            if (!string.IsNullOrEmpty(searchValue))//filter
            {
                customerList = customerList.Where(x => x.CompanyName.ToLower().Contains(searchValue.ToLower())
                                          || x.CustomerType.ToLower().Contains(searchValue.ToLower())
                                          || x.Name.ToLower().Contains(searchValue.ToLower())
                                          || x.Phone.ToLower().Contains(searchValue.ToLower())
                                          || x.Email.ToLower().Contains(searchValue.ToLower())
                                          || x.Email.ToLower().Contains(searchValue.ToLower())
                                          || x.Address.ToLower().Contains(searchValue.ToLower())
                                          ).ToList<VendorModel>();
            }
            int totalRowsAfterFiltering = customerList.Count;


            //sorting
            customerList = customerList.OrderBy(sortColumnName + " " + sortDirection).ToList<VendorModel>();

            //paging
            customerList = customerList.Skip(start).Take(length).ToList<VendorModel>();


            return Json(new { data = customerList, draw = Request["draw"], recordsTotal = totalRows, recordsFiltered = totalRowsAfterFiltering }, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public async Task< ActionResult> Index(int companyId,int  vendorTypeId)
        {
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }
           
            var vendors = await vendorService.GetVendors(companyId, vendorTypeId);
            //if (vendorTypeId == 1)
            //{
            //    return View("SupplierIndex", vendors.ToPagedList(No_Of_Page, Size_Of_Page));
            //}
            return View(vendors);
        }


        
        [HttpGet]
        public async Task<ActionResult> CreateOrEdit(int id, int vendorTypeId)
        {
            ModelState.Clear();
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            VendorViewModel vm = new VendorViewModel();
            vm.Vendor = vendorService.GetVendorByType(id, vendorTypeId);
            vm.Months = vendorService.GetMonthSelectModes();
            vm.Zones = zoneService.GetZoneSelectModels(companyId);
            vm.Vendor.CompanyId = companyId;
            if (id > 0)
            {
                vm.CustomerCodes = headGLService.GetUpdateCustomerCode(vendorTypeId, companyId);
            }
            else
            {
                vm.CustomerCodes = headGLService.GetInsertCustomerCode(vendorTypeId, companyId);
            }
            vm.Countries = districtService.GetCountriesSelectModels();
            vm.Districts = districtService.GetDistrictSelectModels();

            if (vm.Vendor.VendorId > 0)
            {
                vm.Upazilas = upazilaService.GetUpazilaSelectModelsByDistrict(vm.Vendor.DistrictId ?? 0);
                vm.SubZones = zoneService.GetSubZoneSelectModelsByZone(vm.Vendor.ZoneId ?? 0);
            }
            else
            {
                vm.Upazilas = new List<SelectModel>();
                vm.SubZones = new List<SelectModel>();
            }
            if (companyId == (int)CompanyName.KrishibidFarmMachineryAndAutomobilesLimited)
            {
                return View("KFMALCreateOrEdit", vm);
            }
            else
            {
                return View(vm);
            }

        }


        
        [HttpPost]
        public async Task<ActionResult> CreateOrEdit(VendorViewModel vm)
        {
            ModelState.Clear();
            string message = string.Empty;
            bool result = false;

            if (vm.VendorImageUpload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(vm.VendorImageUpload.FileName);
                string extension = Path.GetExtension(vm.VendorImageUpload.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                vm.Vendor.ImageUrl = "~/Images/VendorImage/" + fileName;
                vm.VendorImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Images/VendorImage/"), fileName));
            }
            if (vm.NomineeImageUpload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(vm.NomineeImageUpload.FileName);
                string extension = Path.GetExtension(vm.VendorImageUpload.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                vm.Vendor.NomineeImageUrl = "~/Images/VendorImage/" + fileName;
                vm.NomineeImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Images/VendorImage/"), fileName));
            }

            if (vm.Vendor.VendorId <= 0)
            {
                result =await vendorService.SaveVendor(0, vm.Vendor, message);
                if (result)
                {
                    message = "Data saved successfully";
                }
            }

            else
            {
                result =await vendorService.SaveVendor(vm.Vendor.VendorId, vm.Vendor, message);
                if (result)
                {
                    message = "Data updated successfully";
                }
            }
            TempData["message"] = message;
            return RedirectToAction("Index", new { companyId = vm.Vendor.CompanyId, vendorTypeId = vm.Vendor.VendorTypeId, isActive = vm.Vendor.IsActive });


        }
        
        [HttpGet]
        public async Task<ActionResult> PaymentIndex(int companyId)
        {
            if (GetCompanyId() > 0)
            {
                Session["CompanyId"] = GetCompanyId();
            }

            VendorModel vendorModel = new VendorModel();
            vendorModel = await vendorService.GetCustomerPayments(companyId);
            return View(vendorModel);
        }
        //
        //[HttpGet]
        //public ActionResult PaymentIndex(int? Page_No, string searchText)
        //{
        //    if (GetCompanyId() > 0)
        //    {
        //        Session["CompanyId"] = GetCompanyId();
        //    }
        //    searchText = searchText ?? "";
        //    List<VendorModel> vendors = vendorService.GetCustomerPayments(searchText, Convert.ToInt32(Session["CompanyId"]), (int)Provider.Customer);
        //    int Size_Of_Page = 10;
        //    int No_Of_Page = Page_No ?? 1;
        //    return View(vendors.ToPagedList(No_Of_Page, Size_Of_Page));
        //}
        
        [HttpGet]
        public JsonResult GetCustomerPaymentInformation(int customerId)
        {
            VendorModel vendor = vendorService.GetVendorPaymentStatus(customerId);

            var result = JsonConvert.SerializeObject(vendor, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public JsonResult GetCustomerInformation(int customerId)
        {
            VendorModel vendor = vendorService.GetVendor(customerId);

            var result = JsonConvert.SerializeObject(vendor, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public JsonResult GetSupplierInformation(int supplierId)
        {
            VendorModel vendor = vendorService.GetSupplier(supplierId);

            var result = JsonConvert.SerializeObject(vendor, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public ActionResult CustomerReceivable(int id)
        {
            VendorViewModel vm = new VendorViewModel();
            vm.Vendor = vendorService.GetVendor(id);
            vm.CustomerReceivables = vendorService.GetCustomerReceivables(id);
            return View(vm);
        }

        
        [HttpGet]
        public ActionResult CustomerPayment(int id)
        {
            VendorViewModel vm = new VendorViewModel();
            vm.Vendor = vendorService.GetVendor(id);
            vm.Payments = paymentService.GetPaymentsByVendor(id);
            return View(vm);
        }


        
        [HttpGet]
        public ActionResult CustomerLedger(int id)
        {
            List<CustomerLedgerCustomModel> customerLedgers = vendorService.GetCustomerLedger(id);
            return View(customerLedgers);
        }

        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var customers = vendorService.GetCustomerAutoComplete(prefix, companyId);

            return Json(customers);
        }

     
        public JsonResult GetCustomerByMarketingOfficer(string prefix, long? salePersonId)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var customers = vendorService.GetCustomerByMarketingOfficerId(prefix, companyId, salePersonId);

            return Json(customers, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult AutoCompleteCustomer(string prefix, int companyId)
        {
          
            var customers = vendorService.GetCustomerAutoComplete(prefix, companyId);

            return Json(customers,  JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AutoCompleteByCompanyId(int companyId, string prefix)
        {
            var customers = vendorService.GetCustomerAutoComplete(prefix, companyId);
            return Json(customers);
        }
        [HttpPost]
        public JsonResult CustomerAssociatesCustomerId(int customerId)
        {
            var customers = vendorService.CustomerAssociatesCustomerId(customerId);
            return Json(customers);
        }
        [HttpPost]
        public JsonResult ClientAutoComplete(int companyId, string prefix)
        {
            var customers = vendorService.GetClientAutoComplete(prefix, companyId);
            return Json(customers);
        }

        [HttpPost]
        public JsonResult SupplierAutoComplete(int companyId, string prefix)
        {
            //int companyId = Convert.ToInt32(Session["CompanyId"]);
            var customers = vendorService.GetSupplierAutoComplete(prefix, companyId);

            return Json(customers);
        }
        [HttpGet]
        public JsonResult GetSupplierAutoComplete(int companyId, string prefix)
        {
            //int companyId = Convert.ToInt32(Session["CompanyId"]);
            var customers = vendorService.GetSupplierAutoComplete(prefix, companyId);

            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RentCompanyAutoComplete(string prefix)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var customers = vendorService.GetRentCompanyAutoComplete(prefix, companyId);

            return Json(customers);
        }



        [HttpPost]
        public JsonResult GetAutoGeneratedVendorCode(int upazilaId, int vendorTypeId)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            string customerCode = vendorService.GetAutoGeneratedVendorCode(companyId, upazilaId, vendorTypeId);
            return Json(customerCode);
        }


        
        [HttpGet]
        public ActionResult UploadVendor(int companyId, int vendorTypeId)
        {
            VendorModel model = new VendorModel
            {
                CompanyId = companyId,
                VendorTypeId = vendorTypeId
            };
            return View(model);
        }
        
        [HttpPost]
        public ActionResult UploadVendor(VendorModel model)
        {
            bool result = false;
            List<VendorModel> vendors = new List<VendorModel>();

            HttpPostedFileBase file = model.UploadedFile;
            if (file.ContentLength >= 0)
            {
                string fileName = file.FileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];
                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                using (var package = new ExcelPackage(file.InputStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        VendorModel vendor = new VendorModel();

                        vendor.CompanyId = Convert.ToInt32(workSheet.Cells[rowIterator, 1].Value);
                        vendor.VendorTypeId = Convert.ToInt32(workSheet.Cells[rowIterator, 2].Value);
                        vendor.Name = workSheet.Cells[rowIterator, 3].Value == null ? null : workSheet.Cells[rowIterator, 3].Value.ToString();
                        vendor.ContactName = workSheet.Cells[rowIterator, 4].Value == null ? null : workSheet.Cells[rowIterator, 4].Value.ToString();
                        vendor.Address = workSheet.Cells[rowIterator, 5].Value == null ? null : workSheet.Cells[rowIterator, 5].Value.ToString();
                        vendor.DistrictName = workSheet.Cells[rowIterator, 6].Value == null ? null : workSheet.Cells[rowIterator, 6].Value.ToString();
                        vendor.UpazilaName = workSheet.Cells[rowIterator, 7].Value == null ? null : workSheet.Cells[rowIterator, 7].Value.ToString();
                        vendor.ThanaName = workSheet.Cells[rowIterator, 8].Value == null ? null : workSheet.Cells[rowIterator, 8].Value.ToString();
                        vendor.Phone = workSheet.Cells[rowIterator, 9].Value == null ? null : workSheet.Cells[rowIterator, 9].Value.ToString();
                        vendor.BIN = workSheet.Cells[rowIterator, 10].Value == null ? null : workSheet.Cells[rowIterator, 10].Value.ToString();
                        vendor.PaymentDue = workSheet.Cells[rowIterator, 11].Value == null ? (decimal?)null : Convert.ToDecimal(workSheet.Cells[rowIterator, 11].Value);
                        vendor.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        vendor.CreatedDate = DateTime.Today;
                        vendor.IsActive = true;
                        vendors.Add(vendor);
                    }
                }
            }
            result = vendorService.BulkSave(vendors);
            if (result)
            {
                return RedirectToAction("Index", new { companyId = model.CompanyId, vendorTypeId = model.VendorTypeId });
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetCustomerSelectModelsByCompany(int companyId)
        {
            const int customer = 2;
            List<SelectModel> customers = vendorService.GetCustomerSelectModelsByCompany(companyId, customer);
            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public ActionResult Delete(int id, int vendorTypeId, bool isActive)
        {

            bool result = vendorService.DeleteVendor(id);
            if (result)
            {
                TempData["message"] = "Data deleted successfully";
            }
            else
            {
                TempData["message"] = "Data can not be deleted";
            }

            return RedirectToAction("Index", new { companyId = Session["CompanyId"], vendorTypeId, isActive });
        }

        
        [HttpGet]
        public async Task<ActionResult> VendorDeedIndex(int companyId, int customerId = 0)
        {
            var model = await vendorService.GetAllVendorDeed(companyId, customerId);

            return View(model);
        }

        
        [HttpGet]
        public async Task<ActionResult> CreateVendorDeed(int companyId, int vendorDeedId =0,int customerId = 0)
        {
            var model = new VendorDeedVm();

            model.CompanyId = companyId;
            if (vendorDeedId > 0)
            {
                model = await vendorService.GetVendorDeedById(vendorDeedId, customerId);
            }
            else
            {
                var vendor = vendorService.GetVendor(customerId);
                model.VendorId = vendor.VendorId;
                model.VendorName = vendor.Name;
            }

            return View(model);
        }

        
        [HttpPost]
        public async Task<ActionResult> CreateVendorDeed(VendorDeedVm model)
        {
            var vendorDeedId = await vendorService.SaveVendorDeed(model);


            return RedirectToAction("CreateVendorDeed", "Vendor", new
            {
                companyId = model.CompanyId,
                vendorDeedId = vendorDeedId,
                customerId = model.VendorId
            });
        }

        
        [HttpGet]
        public async Task<ActionResult> RemoveVendorDeed(int companyId,int vendorDeedId)
        {
            if (vendorDeedId > 0)
            {
                await vendorService.RemoveVendorDeed(vendorDeedId);
            }

            return RedirectToAction("VendorDeedIndex", "Vendor", new
            {
                companyId = companyId
            });
        }

        #region VendorAdjustment GCCL

        [HttpGet]
        public ActionResult CustomerAdjustment(int companyId, int adjustmentId=0)
        {            
            var model = new VendorAdjustmentVM();
            model.ActionId = (int)ActionEnum.Add;
            if (adjustmentId > 0)
            {
                model = vendorService.GetVendorAdjustmentById(adjustmentId);
                if (model == null)
                {
                    return RedirectToAction("CustomerAdjustmentList", new { companyId = model.UserCompnayId });
                }
                model.ActionId = (int)ActionEnum.Edit;
            }
            model.UserCompnayId = companyId;
            model.ZoneList = zoneService.GetZoneSelectModels(companyId);
            model.SubZoneList = new List<SelectModel>();
            if (adjustmentId > 0)
            {
                model.SubZoneList = zoneService.GetSubZoneSelectModelsByZone(model.ZoneId);
                //model.VendorList = vendorService.GetZonewiseCustomerSelectModels(model.ZoneId, model.SubZoneId);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult CustomerAdjustment(VendorAdjustmentVM model)
        {
            var result = new MethodFeedbackVM();
            if (model.ActionEum == ActionEnum.Add)
            {
                result  = vendorService.InsertVendorAdjustment(model);
            }
            if (model.ActionEum == ActionEnum.Edit)
            {
                result = vendorService.UpdateVendorAdjustment(model);
            }
            if (result.Status)
            {
                return RedirectToAction("CustomerAdjustmentList", new { companyId = model.UserCompnayId });
            }
            return View(result);
        }
        [HttpPost]
        public ActionResult UpdateCustomerAdjustmentStatus(VendorAdjustmentVM model)
        {
            var result = vendorService.UpdateVendorAdjustmentStatus(model);   
            return   RedirectToAction("CustomerAdjustmentList", new { companyId = model.UserCompnayId });
        }
            [HttpGet]
        public ActionResult CustomerAdjustmentList(int companyId)
        {
            var model = new VendorAdjustmentVM();
            model.UserCompnayId = companyId;

            model = vendorService.GetVendorAdjustmentList(companyId);
            return View(model);
        }

        [HttpGet]
        public ActionResult GetZonewiseCustomerSelectModels(int? zoneId, int? subZoneId)
        {
            var model = vendorService.GetZonewiseCustomerSelectModels(zoneId, subZoneId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion VendorAdjustment

    }
}