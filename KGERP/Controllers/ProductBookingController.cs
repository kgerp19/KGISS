using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.VariantTypes;
using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.FTP;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Implementation.Realestate.BookingAprovalList;
using KGERP.Service.Implementation.Realestate.BookingCollaction;
using KGERP.Service.Implementation.Realestate.CustomersBooking;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.FTP_Models;
using KGERP.Services.Procurement;
using KGERP.Utility;
using KGERP.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Services.Description;
using static KGERP.Controllers.Custom_Authorization.ParentAuthorizedAttribute;

namespace KGERP.Controllers
{
    [CheckSession]
    public class ProductBookingController : Controller
    {

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
        string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        public ProductBookingController(IBookingInstallmentService bookingInstallmentService,
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
        public async Task<ActionResult> KPLCustomer(int companyId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => kgReCrmService.GetCustomer(companyId));
           

            return View(vmCommonCustomer);
        }


        // GET: ProductBooking
        #region //GLDLNEW   

       
        [HttpPost]
        public JsonResult GetByProduct(int productid)
        {
            var result = kgReCrmService.Getbyproduct(productid);
            return Json(result);
        }


        [HttpGet]
        public JsonResult GetByProductstastus(int productid)
        {
            GLDLBookingViewModel vm = new GLDLBookingViewModel();
            var result = kgReCrmService.Getbyproduct(productid);
            vm.ProductId = result.ProductId;
            if (result.CompanyId==7)
            {
                vm.PStatus = ((ProductStatusEnumGLDL)result.Status).ToString();
            }
            else
            {
                vm.PStatus = ((ProductStatusEnumKPL)result.Status).ToString();
            }
            
            return Json(vm,JsonRequestBehavior.AllowGet);
        }
       
        [HttpPost]
        public async Task<JsonResult> GetProductCostHeadsbyId(int productid, int companyId)
        {
            BookingViewModel model = new BookingViewModel();
            model.product = kgReCrmService.GetbyproductForGldl(productid);
            return Json(model);
        }
           
       
        [HttpGet]
        public JsonResult GetfileCheck(int companyId, string fileNo)
        {
            GLDLBookingViewModel vm = new GLDLBookingViewModel();
            var res = gLDLCustomerService.bookingFilecheck(companyId, fileNo);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> CustomerBooking(int companyId, int vandorId = 0)
        {
            GLDLBookingViewModel vm = new GLDLBookingViewModel() { CompanyId = companyId };
            vm.ApplicationDate = DateTime.Now;
            vm.ApplicationDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            vm.DownPaymentDateNew = DateTime.Now.ToString("dd-MMM-yyyy");
            vm.BookingMoneyPaymentDateNew = DateTime.Now.ToString("dd-MMM-yyyy");
            if (vandorId > 0)
            {
                var crm = kgReCrmService.GetVendorById(vandorId);
               // vm.CustomerGroupName = crm.Name + " & Associated";
                vm.CustomerGroupName = crm.Name;
                vm.PrimaryContactAddr = crm.Address;
                vm.PrimaryContactNo = crm.Phone;
                vm.PrimaryEmail = crm.Email;
            }          
            vm.ProductCategoryList = voucherTypeService.GetProductCategoryGldl(companyId);
            vm.LstPurchaseCostHeads = await _costHeadService.GetCostHeadsByCompanyId(companyId);
            vm.BookingInstallmentType = await _costHeadService.GetCompanyBookingInstallmentType(companyId);
            vm.Employee = new SelectList(_teamService.GetTeamListByCompanyId(companyId), "Value", "Text");            
            return View(vm);
        }
        [HttpPost]
        public async Task<ActionResult> CustomerBooking(GLDLBookingViewModel model)
        {
            model.EntryBy = Convert.ToInt32(Session["Id"]);
            model.ApplicationDate = Convert.ToDateTime(model.ApplicationDateString);
            model.BookingDate = Convert.ToDateTime(model.BookingDateString);
            model.DownPaymentDate = Convert.ToDateTime(model.DownPaymentDateNew);
            model.BookingMoneyPaymentDate = Convert.ToDateTime(model.BookingMoneyPaymentDateNew);
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
            var result = await gLDLCustomerService.CustomerBokking(model);
            if (result.CGId != 0)
            {
                return RedirectToAction("CustomerBookingInformation", new { companyId = model.CompanyId, CGId = model.CGId });
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CustomerBookingPassUrl(GLDLBookingViewModel model)
        {
            return RedirectToAction("CustomerBooking", new { clientId = model.ClientId, companyId = model.CompanyId });
        }

        

        [HttpGet]
        public async Task<ActionResult> CustomerBookingInformation(int companyId, long CGId)
        {
            var res = await customerBookingService.CustomerBookingView(companyId, CGId);
            res.pRM_Relations = await customerBookingService.PRMRelation();
            res.BookingInstallmentType = await _costHeadService.GetCompanyBookingInstallmentType(companyId);
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
        [HttpGet]
        public async Task<ActionResult> UpdateBookingInformation(int companyId, long CGId)
        {
            var res = await customerBookingService.CustomerBookingView(companyId, CGId);
            res.ApplicationDateString = res.ApplicationDate?.ToString("dd-MMM-yyyy");
            res.BookingDateString = res.BookingDate?.ToString("dd-MMM-yyyy");
            res.Employee = new SelectList(_teamService.GetTeamListByCompanyId(companyId), "Value", "Text");
            return View(res);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateBookingInformation(GLDLBookingViewModel model)
        {
            model.ApplicationDate = Convert.ToDateTime(model.ApplicationDateString);
            model.BookingDate = Convert.ToDateTime(model.BookingDateString);
            var res= await gLDLCustomerService.UpdateCustomerBokking(model);
            return RedirectToAction("CustomerBookingInformation", new { companyId = model.CompanyId, CGId = model.CGId });
        }
        [HttpPost]
        public async Task<ActionResult> CustomerNominee(CustomerNominee nominee)
        {
            var res = await gLDLCustomerService.AddCustomerNominee(nominee);
            return RedirectToAction("CustomerBookingInformation", new { companyId = nominee.CompanyId, CGId = nominee.CGId });
        }


        [HttpPost]
        public async Task<ActionResult> UpdateNominee(CustomerNominee nominee)
        {
            var res = await gLDLCustomerService.UpdateNominee(nominee);
            return RedirectToAction("CustomerBookingInformation", new { companyId = nominee.CompanyId, CGId = nominee.CGId });
        }


        [HttpPost]
        public async Task<ActionResult> DeleteNominee(CustomerNominee nominee)
        {
            var res = await gLDLCustomerService.DeleteNominee(nominee);
            return RedirectToAction("CustomerBookingInformation", new { companyId = nominee.CompanyId, CGId = nominee.CGId });
        }
        [HttpPost]
        public async Task<JsonResult> GetNominee(long id)
        {
            var res = await gLDLCustomerService.GetByNominee(id);
            return Json(res);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateNomineefile(NomineeFile file)
        {
            if (file.NIDFile == null && file.ImageFile == null)
            {
                return RedirectToAction("CustomerBookingInformation", new { companyId = file.CompanyId, CGId = file.CGId });
            }
            List<FileItem> itemlist = new List<FileItem>();
            if (file.ImageFile != null)
            {
                itemlist.Add(new FileItem
                {
                    file = file.ImageFile,
                    docdesc = "Nominee Photo",
                    docfilename = Guid.NewGuid().ToString() + Path.GetExtension(file.ImageFile.FileName),
                    docid = 0,
                    FileCatagoryId = 2,
                    fileext = Path.GetExtension(file.ImageFile.FileName),
                    isactive = true,
                    RecDate = DateTime.Now,
                    SortOrder = 1,
                    userid = Convert.ToInt32(Session["Id"])
                });
            }
            if (file.NIDFile != null)
            {
                itemlist.Add(new FileItem
                {
                    file = file.NIDFile,
                    docdesc = "Nominee NID",
                    docfilename = Guid.NewGuid().ToString() + Path.GetExtension(file.NIDFile.FileName),
                    docid = 0,
                    FileCatagoryId = 2,
                    fileext = Path.GetExtension(file.NIDFile.FileName),
                    isactive = true,
                    RecDate = DateTime.Now,
                    SortOrder = 2,
                    userid = Convert.ToInt32(Session["Id"])
                });
            }
            itemlist = await _ftpservice.UploadFileBulk(itemlist, file.CGId.ToString());
            long ImageDocId = 0;
            long NIDDocId = 0;
            if (file.ImageFile != null)
            {
                ImageDocId = itemlist.FirstOrDefault(f => f.SortOrder == 1).docid;
            }
            if (file.NIDFile != null)
            {
                NIDDocId = itemlist.FirstOrDefault(f => f.SortOrder == 2).docid;
            }

            var res = await gLDLCustomerService.FileUpdateNominee(file, ImageDocId, NIDDocId);
            return RedirectToAction("CustomerBookingInformation", new { companyId = file.CompanyId, CGId = file.CGId });
        }


        [HttpPost]
        public async Task<JsonResult> DeleteNomineeImageFile(long docId, long nomineeId, long CGId, long companyId)
        {
            var result = await _ftpservice.DeleteFile(docId);
            if (result)
            {
                var res = await gLDLCustomerService.UpdateNomineeImageDociId(nomineeId, docId);
                return Json(res);
            }
            return Json(0);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteNomineeNidFile(long docId, long nomineeId, long CGId, long companyId)
        {
            var result = await _ftpservice.DeleteFile(docId);
            if (result)
            {
                var res = await gLDLCustomerService.UpdateNomineeNIDDociId(nomineeId, docId);
                return Json(res);
            }
            return Json(0);
        }

        [HttpPost]
        public async Task<JsonResult> CGFileDelete(long docId, long CGId)
        {
            var result = await _ftpservice.DeleteFile(docId);
            if (result)
            {
                var res = await gLDLCustomerService.DeleteCGFile(docId, CGId);
                return Json(res);
            }
            return Json(false);
        }

        [HttpPost]
        public async Task<ActionResult> CGUloadFile(GLDLBookingViewModel model)
        {
            if (model.Attachments == null)
            {
                return RedirectToAction("CustomerBookingInformation", new { companyId = model.CompanyId, CGId = model.CGId });
            }
            List<FileItem> itemlist = new List<FileItem>();
            for (int i = 0; i < model.Attachments.Count(); i++)
            {
                if (model.Attachments[i].DocId == 0)
                {
                    itemlist.Add(new FileItem
                    {
                        file = model.Attachments[i].File,
                        docdesc = model.Attachments[i].Title,
                        docid = 0,
                        FileCatagoryId = 2,
                        fileext = Path.GetExtension(model.Attachments[i].File.FileName),
                        docfilename = Guid.NewGuid().ToString() + Path.GetExtension(model.Attachments[i].File.FileName),
                        isactive = true,
                        RecDate = DateTime.Now,
                        SortOrder = i,
                        userid = Convert.ToInt32(Session["Id"])
                    });
                }
            }
            itemlist = await _ftpservice.UploadFileBulk(itemlist, model.CGId.ToString());
            var result = await gLDLCustomerService.FileMapping(itemlist, model.CGId);
            return RedirectToAction("CustomerBookingInformation", new { companyId = model.CompanyId, CGId = model.CGId });
        }

        [HttpPost]
        public async Task<JsonResult> InstallmentSchedule(int conmpanyId, int installmentId, int NoOfInstallment, decimal restofAmount, string BookingDate)
        {

            if (DateTime.TryParse(BookingDate, out DateTime date) == false)
            {
                date = DateTime.Now;
            }
            var res = await bookingInstallmentService.GenerateInstallmentSchedule(conmpanyId, installmentId, NoOfInstallment, restofAmount, date);
            foreach (var item in res.LstSchedules)
            {
                TextInfo info = CultureInfo.CurrentCulture.TextInfo;
                item.Title = info.ToTitleCase(item.Title);
                
            }
          
            return Json(res);
        }

        [HttpGet]
        public async Task<ActionResult> CustomerBookingList(int companyId, DateTime? fromDate, DateTime? toDate,int? ProductCategoryId)
        {
            if (fromDate == null)
            {
                DateTime date = DateTime.Now;   
                fromDate =new DateTime(date.Year,date.Month,1);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }
             GLDLBookingViewModel model = new GLDLBookingViewModel();

            model = await customerBookingService.CustomerBookingList(companyId, fromDate, toDate,ProductCategoryId);
            model.ProductCategoryList = voucherTypeService.GetProductCategoryGldl(companyId);
            model.StrFromDate = fromDate.Value.ToString("dd-MM-yyyy");
            model.StrToDate = toDate.Value.ToString("dd-MM-yyyy");
            model.SecondProductCateId = ProductCategoryId;
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> SPCustomerBookingList(int companyId, DateTime? fromDate, DateTime? toDate, int? ProductCategoryId)
        {
            if (fromDate == null)
            {
                DateTime date = DateTime.Now;
                fromDate = new DateTime(date.Year, date.Month, 1);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }
            GLDLBookingViewModel model = new GLDLBookingViewModel();

            model = await customerBookingService.SPCustomerBookingList(companyId, fromDate, toDate, ProductCategoryId);
            model.ProductCategoryList = voucherTypeService.GetProductSeaPalaceGldl(companyId);
            model.StrFromDate = fromDate.Value.ToString("dd-MM-yyyy");
            model.StrToDate = toDate.Value.ToString("dd-MM-yyyy");
            model.SecondProductCateId = ProductCategoryId;
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> CustomerBookingListWithDelete(int companyId, DateTime? fromDate, DateTime? toDate, int? ProductCategoryId)
        {
            if (fromDate == null)
            {
                DateTime date = DateTime.Now;
                fromDate = new DateTime(date.Year, date.Month, 1);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }
            GLDLBookingViewModel model = new GLDLBookingViewModel();

            model = await customerBookingService.CustomerBookingList(companyId, fromDate, toDate, ProductCategoryId);
            model.ProductCategoryList = voucherTypeService.GetProductCategoryGldl(companyId);
            model.StrFromDate = fromDate.Value.ToString("dd-MM-yyyy");
            model.StrToDate = toDate.Value.ToString("dd-MM-yyyy");
            model.SecondProductCateId = ProductCategoryId;
            return View(model);
        }


        [HttpGet]
        public async Task<ActionResult> CustomerBookingListInactive(int companyId, DateTime? fromDate, DateTime? toDate, int? ProductCategoryId)
        {
            if (fromDate == null)
            {
                DateTime date = DateTime.Now;
                fromDate = new DateTime(date.Year, date.Month, 1);
            }

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }
            GLDLBookingViewModel model = new GLDLBookingViewModel();

            model = await customerBookingService.CustomerBookingListInactive(companyId, fromDate, toDate, ProductCategoryId);
            model.ProductCategoryList = voucherTypeService.GetProductCategoryGldl(companyId);
            model.StrFromDate = fromDate.Value.ToString("dd-MM-yyyy");
            model.StrToDate = toDate.Value.ToString("dd-MM-yyyy");
            model.SecondProductCateId = ProductCategoryId;
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> ReSaleBookingList(int companyId)
        {

            GLDLBookingViewModel model = new GLDLBookingViewModel();
            model = await customerBookingService.ReSaleBookingList(companyId);
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> FilterBookingList(GLDLBookingViewModel model )
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);
            
            return RedirectToAction(nameof(CustomerBookingList), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate,ProductCategoryId=model.SecondProductCateId });
        }
        [HttpPost]
        public async Task<ActionResult> SPFilterBookingList(GLDLBookingViewModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);

            return RedirectToAction(nameof(SPCustomerBookingList), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate, ProductCategoryId = model.SecondProductCateId });
        }
        [HttpPost]
        public async Task<ActionResult> FilterBookingListDelete(GLDLBookingViewModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);

            return RedirectToAction(nameof(CustomerBookingListWithDelete), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate, ProductCategoryId = model.SecondProductCateId });
        }
        [HttpPost]
        public async Task<ActionResult> FilterBookingListInactive(GLDLBookingViewModel model)
        {
            if (model.CompanyId > 0)
            {
                Session["CompanyId"] = model.CompanyId;
            }
            model.FromDate = Convert.ToDateTime(model.StrFromDate);
            model.ToDate = Convert.ToDateTime(model.StrToDate);

            return RedirectToAction(nameof(CustomerBookingListInactive), new { companyId = model.CompanyId, fromDate = model.FromDate, toDate = model.ToDate, ProductCategoryId = model.SecondProductCateId });
        }
        [HttpGet]
        public async Task<ActionResult> CustomerList(int companyId)
        {
            var res = kgReCrmService.LoadBookingListInfo();
            return View(res);
        }

        [HttpPost]
        public async Task<JsonResult> CustomerAutoComplete(string prefix, int companyId)
        {
            var res = gLDLCustomerService.GetCustomerAutoComplete(prefix, companyId);
            return Json(res);
        }

        //individual datalist start    [HttpGet]
        [HttpGet]
        public async Task<ActionResult> MD_Booking_List(int companyId)
        {
            var res = await bookingAprovalStatus.MdAprovalApprove(4, companyId);
            return View(res);
        }

        public async Task<ActionResult> DMD_Booking_List(int companyId)
        {
            var res = await bookingAprovalStatus.DMdAprovalApprove(3, companyId);
            return View(res);
        }


        [HttpGet]
        public async Task<ActionResult> Booking_for_TeamLead(int companyId)
        {
            var id = Convert.ToUInt64(Session["Id"]);
            var res = await bookingAprovalStatus.BookingforTeamLead((long)id, companyId);
            return View(res);
        }

        [HttpGet]
        public async Task<ActionResult> Booking_for_Dealing_Officer(int companyId)
        {
            var id = Convert.ToUInt64(Session["Id"]);
            var res = await bookingAprovalStatus.BookingforDealingOfficer((long)id, companyId);
            return View(res);
        }

   


        //individual datalist End


        [HttpGet]
        public async Task<ActionResult> BookingDraftList(int companyId)
        {

                DateTime fromDate = DateTime.Now.AddMonths(-1);          
                DateTime toDate = DateTime.Now;
            var productcategory = 0;
            
            /// var res =await bookingAprovalStatus.BookingAprovalDraft(1, companyId);
            var res = await customerBookingService.CustomerBookingList(companyId, fromDate, toDate, productcategory);
            return View(res);
        }

        //[HttpGet]
        //public async Task<ActionResult> BookingRecheckList(int companyId)
        //{
        //    //  var res = await bookingAprovalStatus.BookingAprovalRecheck(2, companyId);
        //    var res = await customerBookingService.CustomerBookingList(companyId);
        //    return View(res);

        //}

        //[HttpGet]
        //public async Task<ActionResult> BookingApproveList(int companyId)
        //{
        //    //var res = await bookingAprovalStatus.BookingAprovalApprove(3, companyId);
        //    var res = await customerBookingService.CustomerBookingList(companyId);
        //    return View(res);
        //}

        [HttpPost]
        public async Task<ActionResult> BookingStatus(GLDLBookingViewModel model)
        {
            model.EntryBy = Convert.ToInt32(Session["Id"]);
            var res = await bookingAprovalStatus.BookingStatusChange(model);
            return RedirectToAction("CustomerBookingList", new { companyId = model.CompanyId });
        }


        [HttpPost]
        public async Task<ActionResult> DerftBookingStatus(GLDLBookingViewModel model)
        {
            model.EntryBy = Convert.ToInt32(Session["Id"]);
            var res = await bookingAprovalStatus.BookingStatusChange(model);
            return RedirectToAction("Booking_for_Dealing_Officer", new { companyId = model.CompanyId });
        }

        [HttpPost]
        public async Task<ActionResult> RechecekBookingStatus(GLDLBookingViewModel model)
        {
            model.EntryBy = Convert.ToInt32(Session["Id"]);
            var res = await bookingAprovalStatus.BookingStatusChange(model);
            return RedirectToAction("Booking_for_TeamLead", new { companyId = model.CompanyId });
        }

        [HttpPost]
        public async Task<ActionResult> ApproveBookingStatus(GLDLBookingViewModel model)
        {
            model.EntryBy = Convert.ToInt32(Session["Id"]);
            var res = await bookingAprovalStatus.BookingStatusChange(model);
            return RedirectToAction("BookingApproveList", new { companyId = model.CompanyId });
        }
             
        [HttpPost]
        public async Task<ActionResult> MDBookingStatus(GLDLBookingViewModel model)
        {
            model.EntryBy = Convert.ToInt32(Session["Id"]);
            var res = await bookingAprovalStatus.BookingStatusChange(model);
            return RedirectToAction("MD_Booking_List", new { companyId = model.CompanyId });
        }
             
        [HttpPost]
        public async Task<ActionResult> DMDBookingStatus(GLDLBookingViewModel model)
        {
            model.EntryBy = Convert.ToInt32(Session["Id"]);
            var res = await bookingAprovalStatus.BookingStatusChange(model);
            return RedirectToAction("DMD_Booking_List", new { companyId = model.CompanyId });
        }



        [HttpPost]
        public async Task<ActionResult> finalsubmit(GLDLBookingViewModel bookingViewModel)
        {
            DateTime stringdate =Convert.ToDateTime("2022-11-01");
            
            var product = kgReCrmService.Getbyproduct(bookingViewModel.ProductId.Value);
            if (bookingViewModel.CompanyId == (int)CompanyName.GloriousLandsAndDevelopmentsLimited)
            {
                if (product.Status == (int)ProductStatusEnumGLDL.UnSold&& bookingViewModel.BookingDate> stringdate)
                {
                    var v = await customerBookingService.SubmitBooking(bookingViewModel);
                }
                else
                {
                    var v = await customerBookingService.SubmitStatusChange(bookingViewModel);
                }
            }
            if (bookingViewModel.CompanyId == (int)CompanyName.KrishibidPropertiesLimited)
            {
                if (product.Status == (int)ProductStatusEnumKPL.VacantFlat && bookingViewModel.BookingDate > stringdate)
                {
                    var v = await customerBookingService.SubmitBooking(bookingViewModel);
                }
                else
                {
                    var v = await customerBookingService.SubmitStatusChange(bookingViewModel);
                }
            }  
            return RedirectToAction("CustomerBookingInformation", new { companyId = bookingViewModel.CompanyId.Value, CGId = bookingViewModel.CGId });
        }

        [HttpPost]
        public async Task<ActionResult> SignalCollaction(GLDLBookingViewModel collaction)
        {
            CollactionBillViewModel model = new CollactionBillViewModel();
            model.CompanyId = collaction.CompanyId;
            model.CGId = collaction.CGId;
            model.BookingId = collaction.BookingId;
            var res = await customerBookingService.InstallmentScheduleById(collaction.InstallmentId);
            model.ReceivableAmount = res.Amount;
            model.InAmount = res.Amount;
            model.InstallmentId = res.InstallmentId;
            model.TransactionDate = DateTime.Now;
            model.BookingNo = collaction.BookingNo;
            model.BankOrCashParantList = new SelectList(_accountingService.GCCLCashAndBankDropDownList((int)collaction.CompanyId), "Value", "Text");
            var company = _companyService.GetCompany((int)model.CompanyId);
            model.CompanyName = company.Name;
            model.TransactionDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> SignalConfirmCollaction(CollactionBillViewModel collaction)
        {
            collaction.TransactionDate = Convert.ToDateTime(collaction.TransactionDateString);
            return View(collaction);
        }

        [HttpGet]
        public async Task<ActionResult> MultiCollaction(int companyId, long CGId, int collactionType)
        {
            CollactionBillViewModel collaction = new CollactionBillViewModel();
            collaction = await customerBookingService.CustomerInstallmentSchedule(companyId, CGId);
            collaction.TransactionDate = Convert.ToDateTime(collaction.TransactionDateString);
            var company = _companyService.GetCompany(companyId);
            collaction.CompanyName = company.Name;
            collaction.CollactionType = collactionType;
            collaction.TransactionDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            collaction.BankOrCashParantList = new SelectList(_accountingService.GCCLCashAndBankDropDownList((int)collaction.CompanyId), "Value", "Text");
            return View(collaction);
        }
        [HttpPost]
        public async Task<ActionResult> MultiCollaction(CollactionBillViewModel collaction)
        {
            var res = await _bookingCollaction.CustomerBookingView(collaction);

            if (res.PaymentMasterId == 0)
            {
                return RedirectToAction("MultiCollaction", new { companyId = collaction.CompanyId.Value, CGId = collaction.CGId, collactionType = collaction.CollactionType });
            }
            else
            {
                return RedirectToAction("BookingCollactionDetalis", new { companyId = collaction.CompanyId.Value, CGId = collaction.CGId, paymentMasterId = collaction.PaymentMasterId });
            }
        }

        [HttpGet]
        public async Task<ActionResult> BookingCollactionDetalis(int companyId, long CGId, int paymentMasterId)
        {
            CollactionBillViewModel model = new CollactionBillViewModel();
            model = await _bookingCollaction.CollactionBookingView(companyId, paymentMasterId, CGId);
            model.Schedule = customerBookingService.GetByInstallmentSchedule(companyId, CGId);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCollaction(CollactionBillViewModel model)
        {
            var res = await _bookingCollaction.CollactionDelete(model);
            return RedirectToAction("BookingCollactionList", new { companyId = model.CompanyId });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteItemCollaction(CollactionBillViewModel model)
        {
            var res = await _bookingCollaction.CollactionItemDelete(model);
            return RedirectToAction("BookingCollactionList", new { companyId = model.CompanyId });
        }

        [HttpGet]
        public async Task<ActionResult> BookingCollactionList(int companyId)
        {
            var res = await _bookingCollaction.CollactionList(companyId);
            var company = _companyService.GetCompany(companyId);
            res.CompanyName = company.Name;
            res.CompanyId = companyId;
            res.BankOrCashParantList = new SelectList(_accountingService.GCCLCashAndBankDropDownList(companyId), "Value", "Text");
            return View(res);
        }

        [HttpPost]
        public async Task<ActionResult> PaymentConfirmation(CollactionBillViewModel model)
        {
            model = await _bookingCollaction.CollactionBookingView(model.CompanyId.Value, model.PaymentMasterId, model.CGId.Value);
            var bookingCollection = await _accountingService.InstallmentCollectionPushGLDL(model.CompanyId.Value, model, 90);
            if (bookingCollection > 0)
            {
                var res = await _bookingCollaction.CollactionStatusUpdate(model.PaymentMasterId);
                //return RedirectToAction("ManageBankOrCash", "Vouchers", new { companyId = model.CompanyId.Value, voucherId = bookingCollection });
                return RedirectToAction("BookingCollactionDetalis", new { companyId = model.CompanyId.Value, CGId = model.CGId, paymentMasterId = model.PaymentMasterId });
            }
            return RedirectToAction("BookingCollactionDetalis", new { companyId = model.CompanyId.Value, CGId = model.CGId, paymentMasterId = model.PaymentMasterId });
        }

        [HttpPost]
        public async Task<ActionResult> UpdateCollaction(CollactionBillViewModel model)
        {
            var res = await _bookingCollaction.CollactionUpdate(model);
            return RedirectToAction("BookingCollactionList", new { companyId = model.CompanyId });
        }

        [HttpPost]
        public async Task<ActionResult> AddnewCollaction(CollactionBillViewModel model)
        {
            var res = await _bookingCollaction.NewCollaction(model);
            return RedirectToAction("BookingCollactionDetalis", new { companyId = model.CompanyId.Value, CGId = model.CGId, paymentMasterId = model.PaymentMasterId });
        }
        [HttpPost]
        public JsonResult GetBookingList(int companyId, string prefix)
        {
            var res = _bookingCollaction.GetBookingList(companyId, prefix);
            return Json(res);
        }
       
        [HttpGet]
        public JsonResult GetVendorInformation(int companyId, int vendorId)
        {
            VendorModel vendor = _vendorService.GetSupplier(vendorId);
            return Json(vendor, JsonRequestBehavior.AllowGet);
        }

       
        [HttpPost]
        public async Task<JsonResult> Getinstallmentclient(long id)
        {
            var obj =await customerBookingService.getInstallmetClient(id);
            return Json(obj,JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> DeleteBookingInfo(long CGID,int CompanyId)
        {
            var res = await customerBookingService.DeleteProductBookingInfo(CGID);
            return RedirectToAction("CustomerBookingListWithDelete", new { companyId = CompanyId });
        }
        [HttpGet]
        public async Task<ActionResult> UndoBookingInfo(long CGID, int CompanyId)
        {
            var res = await customerBookingService.UndoDeleteProductBookingInfo(CGID);
            return RedirectToAction("CustomerBookingListInactive", new { companyId = CompanyId });
        }
        //ani
       
        [HttpPost]
        public async Task<JsonResult> UpdateInstallMent(InstallmentScheduleShortModel model)
        {
            var obj = await customerBookingService.UpdateInsatllment(model);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> AvailableProductForSales(int companyId, int categoryId = 0, int subCategoryId = 0, string productType = "")
        {
            VMrealStateProductsForList vm = new VMrealStateProductsForList();    
            vm = await _coservice.GetPlotOrFlatforsalepersone(companyId, categoryId, subCategoryId);
            vm.CompanyId=companyId;
            vm.ProductCategoryList = voucherTypeService.GetProductCategoryGldl(companyId);
            vm.ProductType = productType;
            vm.ProductCategory = categoryId;
            vm.SubProductCategory = subCategoryId;
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> AddnewInstallMent(GLDLBookingViewModel model)
        {
            var res = await customerBookingService.SaveNewInstallMent(model);
            return RedirectToAction("CustomerBookingInformation", new { companyId = model.CompanyId, CGId = model.CGId });
        }


      
        [HttpPost]
        public async Task<JsonResult> DeleteAction(int installmentId)
        {
            var obj = await customerBookingService.deleteInstallment(installmentId);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }



        #endregion
        #region Sea Palace customer booking
        [HttpGet]
        public async Task<ActionResult> SPCustomerBooking(int companyId, int vendorId = 0)
        {
            GLDLBookingViewModel vm = new GLDLBookingViewModel() { CompanyId = companyId };
            vm.ApplicationDate = DateTime.Now;
            vm.ApplicationDateString = DateTime.Now.ToString("dd-MMM-yyyy");
            vm.DownPaymentDateNew = DateTime.Now.ToString("dd-MMM-yyyy");
            vm.BookingMoneyPaymentDateNew = DateTime.Now.ToString("dd-MMM-yyyy");

            if (vendorId > 0)
            {
                var crm = kgReCrmService.GetVendorById(vendorId);
                vm.CustomerGroupName = crm.Name;
                vm.PrimaryContactAddr = crm.Address;
                vm.PrimaryContactNo = crm.Phone;
                vm.PrimaryEmail = crm.Email;
            }

            vm.ProductCategoryList = voucherTypeService.GetProductSeaPalaceGldl(companyId);
            vm.ProductCategoryId = (int?)(vm.ProductCategoryList.FirstOrDefault().Value);

            vm.ProductSubCategoryListt = voucherTypeService.GetProductSubCategorySeaPalaceGldl(vm.ProductCategoryId);
            vm.ProductSubCategoryId = (int?)(vm.ProductSubCategoryListt.FirstOrDefault()?.Value);

            vm.ProductListt = voucherTypeService.GetShareSeaPalaceGldl(vm.ProductSubCategoryId);
            vm.ProductId = (int?)(vm.ProductListt.FirstOrDefault()?.Value);

            vm.LstPurchaseCostHeads = await _costHeadService.GetCostHeadsByCompanyId(companyId);
            vm.BookingInstallmentType = await _costHeadService.GetCompanyBookingInstallmentType(companyId);
            vm.Employee = new SelectList(_teamService.GetTeamListByCompanyId(companyId), "Value", "Text");

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> SPCustomerBooking(GLDLBookingViewModel model)
        {
            model.EntryBy = Convert.ToInt32(Session["Id"]);
            model.ApplicationDate = Convert.ToDateTime(model.ApplicationDateString);
            model.BookingDate = Convert.ToDateTime(model.BookingDateString);
            model.DownPaymentDate = Convert.ToDateTime(model.DownPaymentDateNew);
            model.BookingMoneyPaymentDate = Convert.ToDateTime(model.BookingMoneyPaymentDateNew);
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
            var result = await gLDLCustomerService.SPCustomerBokking(model);
            if (result.CGId != 0)
            {
                return RedirectToAction("CustomerBookingInformation", new { companyId = model.CompanyId, CGId = model.CGId });
            }
            return View(model);
        }
        
        #endregion
    }
}