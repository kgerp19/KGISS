using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using KGERP;
using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.CurrencyCon;
using KGERP.Service.Implementation.FTP;
using KGERP.Service.Implementation.PortCountry;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Implementation.VendorProfessions;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel.FTP_Models;
using KGERP.Services.Procurement;
using KGERP.Utility;
using KGERP.Utility.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Pos.App.Controllers
{
    [CheckSession]
    public class ConfigurationController : Controller
    {

        private HttpContext httpContext;
        private readonly ConfigurationService _service;
        private readonly ICompanyService _companyService;
        private readonly IGLDLCustomerService gLDLCustomerService;
        private IFTPService _ftpservice;
        private readonly IVendorService _vendorService;
        private readonly IVoucherTypeService voucherTypeService;
        private readonly IVendorProfession _pvendorService;
        private readonly IDropDownItemService _dropDownItemService;
        private readonly IDropdownService _dropdownService;

        public ConfigurationController(IFTPService ftpservice,
            ICompanyService companyService,
            IVoucherTypeService voucherTypeService,
            ConfigurationService configurationService,
            IGLDLCustomerService gLDLCustomerService,
            IVendorService vendorService,
            IVendorProfession pvendorService,
            IDropDownItemService dropDownItemService,
            IDropdownService dropdownService)
        {
            _service = configurationService;
            _companyService = companyService;
            this._ftpservice = ftpservice;
            this.gLDLCustomerService = gLDLCustomerService;
            this.voucherTypeService = voucherTypeService;
            _vendorService = vendorService;
            _pvendorService = pvendorService;
            _dropDownItemService = dropDownItemService;
            _dropdownService = dropdownService;
        }
        #region URLInfo
        public ActionResult Index(int? id)
        {
            var companyId = Common.GetCompanyId();
            UrlInfo urlInfo;
            if (id.HasValue)
            {
                urlInfo = _service.GetUrlById(id.Value);
            }
            else
            {
                urlInfo = new UrlInfo();  // New URL entry
            }

            var model = new UrlViewModel
            {
                DataList = _service.GetAllUrls(),
                UrlInfo = urlInfo,
                CompanyList = new SelectList(_service.CompaniesDropDownList(companyId), "Value", "Text")
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult SaveUrl(UrlInfo urlInfo)
        {
            if (ModelState.IsValid)
            {
                _service.SaveUrl(urlInfo);
                return RedirectToAction("Index");
            }
            return View("Index", new UrlViewModel { DataList = _service.GetAllUrls(), UrlInfo = urlInfo });
        }

        public ActionResult DeleteUrl(int id)
        {
            _service.DeleteUrl(id);
            return RedirectToAction("Index");
        }
        #endregion

        #region User Role Menuitem
        public async Task<ActionResult> UserMenuAssignment(int companyId)
        {
            VMUserMenuAssignment vmUserMenuAssignment = new VMUserMenuAssignment();
            vmUserMenuAssignment.CompanyList = new SelectList(_service.CompaniesDropDownListISS(companyId), "Value", "Text");

            return View(vmUserMenuAssignment);
        }
        [HttpPost]
        public async Task<ActionResult> UserMenuAssignment(VMUserMenuAssignment model)
        {
            VMUserMenuAssignment vmUserMenuAssignment = new VMUserMenuAssignment();
            vmUserMenuAssignment = await _service.UserMenuAssignmentGet(model);
            return View(vmUserMenuAssignment);
        }

        public JsonResult CompanyUserMenuEdit(int id, bool isActive)
        {
            VMUserMenuAssignment model = new VMUserMenuAssignment
            {
                IsActive = isActive,
                CompanyUserMenusId = id
            };
            CompanyUserMenu companyUserMenu = _service.CompanyUserMenuEdit(model);
            return Json(new { menuid = companyUserMenu.CompanyUserMenuId, updatedstatus = companyUserMenu.IsActive });
        }

        #endregion
        public async Task<ActionResult> AccountingCostCenter(int companyId)
        {
            VMUserMenu vmUserMenu;
            vmUserMenu = await Task.Run(() => _service.AccountingCostCenterGet(companyId));

            return View(vmUserMenu);
        }
        [HttpPost]
        public async Task<ActionResult> AccountingCostCenter(VMUserMenu model)
        {

            if (model.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.AccountingCostCenterAdd(model);
            }
            else if (model.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.AccountingCostCenterEdit(model);
            }
            else if (model.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.AccountingCostCenterDelete(model.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(AccountingCostCenter), new { companyId = model.CompanyFK });
        }

        #region User Menu
        public async Task<ActionResult> UserMenu(int companyId)
        {
            VMUserMenu vmUserMenu;
            vmUserMenu = await Task.Run(() => _service.UserMenuGetISS(companyId));
            vmUserMenu.CompanyList = new SelectList(_service.CompaniesDropDownListISS(companyId), "Value", "Text");

            return View(vmUserMenu);
        }
        [HttpPost]
        public async Task<ActionResult> UserMenu(VMUserMenu model)
        {

            if (model.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.UserMenuAdd(model);
            }
            else if (model.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.UserMenuEdit(model);
            }
            else if (model.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.UserMenuDelete(model.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(UserMenu), new { companyId = model.CompanyFK });
        }


        #endregion

        #region User Submenu
        public async Task<ActionResult> UserSubMenu(int companyId)
        {
            VMUserSubMenu vmUserSubMenu;
            vmUserSubMenu = await Task.Run(() => _service.UserSubMenuGetISS(companyId));
            vmUserSubMenu.UserMenuList = new SelectList(_service.CompanyMenusDropDownListISS(companyId), "Value", "Text");

            var companies = _service.CompaniesDropDownListISS(companyId);
            var selectedCompaniesID = companies.FirstOrDefault()?.GetType().GetProperty("Value")?.GetValue(companies.FirstOrDefault()) ?? 0;
            vmUserSubMenu.CompanyList = new SelectList(companies, "Value", "Text");
            vmUserSubMenu.CompanyFK = (int)selectedCompaniesID;

            return View(vmUserSubMenu);
        }
        [HttpPost]
        public async Task<ActionResult> UserSubMenu(VMUserSubMenu model)
        {
            if (model.ActionEum == ActionEnum.Add)
            {
                await _service.UserSubMenuAdd(model);
            }
            else if (model.ActionEum == ActionEnum.Edit)
            {
                await _service.UserSubMenuEdit(model);
            }
            else if (model.ActionEum == ActionEnum.Delete)
            {
                await _service.UserSubMenuDelete(model.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(UserSubMenu), new { companyId = model.CompanyFK });
        }
        #endregion

        #region Report Signatory
        public async Task<ActionResult> CommonReportSignatory(int companyId)
        {

            CommonReportSignatoryVM commonReportSignatory = new CommonReportSignatoryVM();
            commonReportSignatory = await Task.Run(() => _service.GetReportSignatory(companyId));
            return View(commonReportSignatory);
        }

        [HttpPost]
        public async Task<ActionResult> CommonReportSignatory(CommonReportSignatoryVM commonReportSignatory)
        {

            if (commonReportSignatory.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.ReportSignatoryAdd(commonReportSignatory);
            }
            else if (commonReportSignatory.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ReportSignatoryEdit(commonReportSignatory);
            }
            else if (commonReportSignatory.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ReportSignatoryDelete(commonReportSignatory.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonReportSignatory), new { companyId = commonReportSignatory.CompanyFK });
        }
        #endregion
        #region Unit
        public async Task<JsonResult> SingleCommonUnit(int id)
        {

            VMCommonUnit model = new VMCommonUnit();
            model = await _service.GetSingleCommonUnit(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> CommonUnit(int companyId)
        {

            VMCommonUnit vmCommonUnit = new VMCommonUnit();
            vmCommonUnit = await Task.Run(() => _service.GetUnit(companyId));
            return View(vmCommonUnit);
        }

        [HttpPost]
        public async Task<ActionResult> CommonUnit(VMCommonUnit vmCommonUnit)
        {

            if (vmCommonUnit.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.UnitAdd(vmCommonUnit);
            }
            else if (vmCommonUnit.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.UnitEdit(vmCommonUnit);
            }
            else if (vmCommonUnit.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.UnitDelete(vmCommonUnit.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonUnit), new { companyId = vmCommonUnit.CompanyFK });
        }
        [HttpPost]
        public async Task<ActionResult> CommonUnitDelete(VMCommonUnit vmCommonUnit)
        {

            if (vmCommonUnit.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.UnitDelete(vmCommonUnit.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonUnit));
        }
        [HttpPost]

        #endregion
        public async Task<JsonResult> SingleProductCategory(int id)
        {
            var model = await _service.GetSingleProductCategory(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SingleCommonProductSubcategory(int id)
        {
            var model = await _service.GetSingleProductSubCategory(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CommonProductSubCategoryGet(int companyId, int categoryId)
        {

            var vmCommonProductSubCategory = await Task.Run(() => _service.CommonProductSubCategoryGet(companyId, categoryId));
            var list = vmCommonProductSubCategory.Select(x => new { Value = x.ID, Text = x.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> CommonUnitGet(int companyId)
        {

            var vmCommonProductSubCategory = await Task.Run(() => _service.CompanyMenusGet(companyId));
            var list = vmCommonProductSubCategory.Select(x => new { Value = x.ID, Text = x.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> CommonProductGet(int companyId, int productSubCategoryId)
        {

            var vmCommonProductSubCategory = await Task.Run(() => _service.CommonProductGet(companyId, productSubCategoryId));
            var list = vmCommonProductSubCategory.Select(x => new { Value = x.ID, Text = x.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region Common Zone
        public async Task<ActionResult> CommonZone(int companyId)
        {
            VMCommonZone vmCommonZone = new VMCommonZone();
            vmCommonZone = await Task.Run(() => _service.GetZones(companyId));
            return View(vmCommonZone);
        }

        [HttpPost]
        public async Task<ActionResult> CommonZone(VMCommonZone vmCommonZone)
        {

            if (vmCommonZone.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.ZoneAdd(vmCommonZone);
            }
            else if (vmCommonZone.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ZonesEdit(vmCommonZone);
            }
            else if (vmCommonZone.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ZonesDelete(vmCommonZone.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonZone), new { companyId = vmCommonZone.CompanyFK });
        }


        #endregion

        #region Common Subzone

        [HttpGet]
        public async Task<ActionResult> GetSubZoneList(int companyId, int zoneId = 0)
        {
            var model = await Task.Run(() => _service.GetSubZoneList(companyId, zoneId));
            var list = model.Select(x => new { Value = x.Value, Text = x.Text }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CommonSubZone(int companyId, int zoneId = 0)
        {

            VMCommonSubZone vmCommonSubZone = new VMCommonSubZone();
            vmCommonSubZone = await Task.Run(() => _service.GetSubZones(companyId, zoneId));
            vmCommonSubZone.ZoneList = new SelectList(_service.CommonZonesDropDownList(companyId), "Value", "Text");

            return View(vmCommonSubZone);
        }
        [HttpPost]
        public async Task<ActionResult> CommonSubZone(VMCommonSubZone vmCommonSubZone)
        {

            if (vmCommonSubZone.ActionEum == ActionEnum.Add)
            {
                //Add 

                await _service.SubZoneAdd(vmCommonSubZone);
            }
            else if (vmCommonSubZone.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.SubZonesEdit(vmCommonSubZone);
            }
            else if (vmCommonSubZone.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.SubZonesDelete(vmCommonSubZone.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonSubZone), new { companyId = vmCommonSubZone.CompanyFK, zoneId = vmCommonSubZone.ZoneId });
        }


        #endregion

        #region Common Finish Product Category

        public async Task<ActionResult> CommonFinishProductCategory(int companyId)
        {

            VMCommonProductCategory vmCommonProductCategory = new VMCommonProductCategory();
            vmCommonProductCategory = await Task.Run(() => _service.GetFinishProductCategory(companyId, "F"));
            return View(vmCommonProductCategory);
        }
        [HttpPost]
        public async Task<ActionResult> CommonFinishProductCategory(VMCommonProductCategory vmCommonProductCategory)
        {

            if (vmCommonProductCategory.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProductCategory.ProductType = "F";
                await _service.ProductFinishCategoryAdd(vmCommonProductCategory);
            }
            else if (vmCommonProductCategory.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductFinishCategoryEdit(vmCommonProductCategory);
            }
            else if (vmCommonProductCategory.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductFinishCategoryDelete(vmCommonProductCategory.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonFinishProductCategory), new { companyId = vmCommonProductCategory.CompanyFK });
        }



        #endregion

        #region Common Finish Product SubCategory


        public async Task<ActionResult> CommonFinishProductSubCategory(int companyId, int categoryId = 0)
        {

            VMCommonProductSubCategory vmCommonProductSubCategory = new VMCommonProductSubCategory();
      
            vmCommonProductSubCategory = await Task.Run(() => _service.GetProductSubCategory(companyId, categoryId, "F"));
            //vmCommonProductSubCategory.BrandList = _service.GetBrandList(companyId);
            return View(vmCommonProductSubCategory);
        }
        [HttpPost]
        public async Task<ActionResult> CommonFinishProductSubCategory(VMCommonProductSubCategory vmCommonProductSubCategory)
        {

            if (vmCommonProductSubCategory.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProductSubCategory.ProductType = "F";

                await _service.ProductSubCategoryAdd(vmCommonProductSubCategory);
            }
            else if (vmCommonProductSubCategory.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductSubCategoryEdit(vmCommonProductSubCategory);
            }
            else if (vmCommonProductSubCategory.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductSubCategoryDelete(vmCommonProductSubCategory.ID);
            }
            
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonFinishProductSubCategory), new { companyId = vmCommonProductSubCategory.CompanyFK, categoryId = vmCommonProductSubCategory.Common_ProductCategoryFk });
        }


        #endregion

        #region Common Finish Product
        public async Task<ActionResult> FinishedProductList(int companyId)
        {

            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            vmCommonProduct = await Task.Run(() => _service.GetProduct(companyId, 0, 0, "F"));
             
            return View(vmCommonProduct);
        }

        public async Task<ActionResult> CommonFinishProduct(int companyId, int categoryId = 0, int subCategoryId = 0)
        {

            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            vmCommonProduct = await Task.Run(() => _service.GetProduct(companyId, categoryId, subCategoryId, "F"));

            vmCommonProduct.UnitList = new SelectList(_service.UnitDropDownList(companyId), "Value", "Text");


            return View(vmCommonProduct);
        }
        public async Task<ActionResult> KFMALCommonFinishProduct(int companyId, int categoryId = 0, int subCategoryId = 0)
        {

            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            vmCommonProduct = await Task.Run(() => _service.kfmalGetProduct(companyId, categoryId, subCategoryId, "F"));
            vmCommonProduct.UnitList = new SelectList(_service.UnitDropDownList(companyId), "Value", "Text");
            return View(vmCommonProduct);
        }
        public async Task<ActionResult> FeedCommonFinishProduct(int companyId, int categoryId = 0, int subCategoryId = 0)
        {

            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            vmCommonProduct = await Task.Run(() => _service.GetProduct(companyId, categoryId, subCategoryId, "F"));

            vmCommonProduct.UnitList = new SelectList(_service.UnitDropDownList(companyId), "Value", "Text");


            return View(vmCommonProduct);
        }
        public async Task<ActionResult> GCCLCommonFinishProduct(int companyId, int categoryId = 0, int subCategoryId = 0)
        {

            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            vmCommonProduct = await Task.Run(() => _service.GetProduct(companyId, categoryId, subCategoryId, "F"));

            vmCommonProduct.UnitList = new SelectList(_service.UnitDropDownList(companyId), "Value", "Text");


            return View(vmCommonProduct);
        }
        [HttpPost]
        public async Task<ActionResult> CommonFinishProduct(VMCommonProduct vmCommonProduct)
        {

            //if (vmCommonProduct.ImageFile != null)
            //{
            //    vmCommonProduct.Image = _service.UploadFile(vmCommonProduct.ImageFile, "Product", _webHostEnvironment.WebRootPath);
            //}
            if (vmCommonProduct.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProduct.ProductType = "F";

                await _service.ProductAdd(vmCommonProduct);
            }
            else if (vmCommonProduct.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductEdit(vmCommonProduct);
            }
            else if (vmCommonProduct.ActionEum == ActionEnum.Delete)
            {
                //Delete
               var result= await _service.ProductDelete(vmCommonProduct.ID);
                vmCommonProduct.Common_ProductSubCategoryFk = result.Common_ProductSubCategoryFk;
                vmCommonProduct.Common_ProductFk = result.Common_ProductFk;
            }
            else
            {
                return RedirectToAction("Error");
            }

            //if (vmCommonProduct.CompanyFK == (int)CompanyName.GloriousCropCareLimited)
            //{
            //    return RedirectToAction(nameof(GCCLCommonFinishProduct), new
            //    {
            //        companyId = vmCommonProduct.CompanyFK,
            //        categoryId = 0,//vmCommonProduct.Common_ProductCategoryFk, 
            //        subCategoryId = 0// vmCommonProduct.Common_ProductSubCategoryFk
            //    });
            //}


            return RedirectToAction(nameof(CommonFinishProduct), new { companyId = vmCommonProduct.CompanyFK, categoryId = vmCommonProduct.Common_ProductCategoryFk, subCategoryId = vmCommonProduct.Common_ProductSubCategoryFk });
        }

        [HttpPost]
        public async Task<ActionResult> kfmaLCommonFinishProduct(VMCommonProduct vmCommonProduct)
        {

            //if (vmCommonProduct.ImageFile != null)
            //{
            //    vmCommonProduct.Image = _service.UploadFile(vmCommonProduct.ImageFile, "Product", _webHostEnvironment.WebRootPath);
            //}
            if (vmCommonProduct.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProduct.ProductType = "F";

                await _service.kfmalProductAdd(vmCommonProduct);
            }
            else if (vmCommonProduct.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.kmaLProductEdit(vmCommonProduct);
            }
            else if (vmCommonProduct.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.kfmalProductDelete(vmCommonProduct.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }

            //if (vmCommonProduct.CompanyFK == (int)CompanyName.GloriousCropCareLimited)
            //{
            //    return RedirectToAction(nameof(GCCLCommonFinishProduct), new
            //    {
            //        companyId = vmCommonProduct.CompanyFK,
            //        categoryId = 0,//vmCommonProduct.Common_ProductCategoryFk, 
            //        subCategoryId = 0// vmCommonProduct.Common_ProductSubCategoryFk
            //    });
            //}


            return RedirectToAction(nameof(kfmaLCommonFinishProduct), new { companyId = vmCommonProduct.CompanyFK, categoryId = vmCommonProduct.Common_ProductCategoryFk, subCategoryId = vmCommonProduct.Common_ProductSubCategoryFk });
        }

        public async Task<ActionResult> CommonFinishProductBOM(int companyId, int productId = 0)
        {

            var vmFinishProductBOM = await Task.Run(() => _service.GetCommonProductByID(companyId, productId));

            return View(vmFinishProductBOM);
        }
        [HttpPost]
        public async Task<ActionResult> CommonFinishProductBOM(VMFinishProductBOM vmFinishProductBOM)
        {

            if (vmFinishProductBOM.ActionEum == ActionEnum.Add)
            {
                //Add 

                await _service.FinishProductBOMAdd(vmFinishProductBOM);
            }
            else if (vmFinishProductBOM.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.FinishProductBOMEdit(vmFinishProductBOM);
            }
            else if (vmFinishProductBOM.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.FinishProductBOMDelete(vmFinishProductBOM.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonFinishProductBOM), new { companyId = vmFinishProductBOM.CompanyFK, productId = vmFinishProductBOM.FProductFK });
        }


        #endregion


        #region Common Raw Product Category

        public async Task<ActionResult> CommonRawProductCategory(int companyId)
        {

            VMCommonProductCategory vmCommonProductCategory = new VMCommonProductCategory();
            vmCommonProductCategory = await Task.Run(() => _service.GetFinishProductCategory(companyId, "R"));
            return View(vmCommonProductCategory);
        }
        [HttpPost]
        public async Task<ActionResult> CommonRawProductCategory(VMCommonProductCategory vmCommonProductCategory)
        {

            if (vmCommonProductCategory.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProductCategory.ProductType = "R";

                await _service.ProductRawCategoryAdd(vmCommonProductCategory);
            }
            else if (vmCommonProductCategory.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductFinishCategoryEdit(vmCommonProductCategory);
            }
            else if (vmCommonProductCategory.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductFinishCategoryDelete(vmCommonProductCategory.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonRawProductCategory), new { companyId = vmCommonProductCategory.CompanyFK });
        }


        #endregion

        #region Common Raw Product SubCategory


        public async Task<ActionResult> CommonRawProductSubCategory(int companyId, int categoryId = 0)
        {

            VMCommonProductSubCategory vmCommonProductSubCategory = new VMCommonProductSubCategory();
            vmCommonProductSubCategory = await Task.Run(() => _service.GetProductSubCategory(companyId, categoryId, "R"));
            return View(vmCommonProductSubCategory);
        }
        [HttpPost]
        public async Task<ActionResult> CommonRawProductSubCategory(VMCommonProductSubCategory vmCommonProductSubCategory)
        {

            if (vmCommonProductSubCategory.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProductSubCategory.ProductType = "R";

                await _service.ProductSubCategoryAdd(vmCommonProductSubCategory);
            }
            else if (vmCommonProductSubCategory.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductSubCategoryEdit(vmCommonProductSubCategory);
            }
            else if (vmCommonProductSubCategory.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductSubCategoryDelete(vmCommonProductSubCategory.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonRawProductSubCategory), new { companyId = vmCommonProductSubCategory.CompanyFK, categoryId = vmCommonProductSubCategory.Common_ProductCategoryFk });
        }


        #endregion

        #region Common Raw Product
        public async Task<ActionResult> CommonRawProduct(int companyId, int categoryId = 0, int subCategoryId = 0)
        {

            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            vmCommonProduct = await _service.GetProduct(companyId, categoryId, subCategoryId, "R");

            vmCommonProduct.UnitList = new SelectList(_service.UnitDropDownList(companyId), "Value", "Text");
            return View(vmCommonProduct);
        }
        public async Task<ActionResult> CommonRawProductR(int companyId, int categoryId = 0, int subCategoryId = 0)
        {
            // Fetch the products based on the parameters
            var products = await _service.SerCommonRawProductR(companyId, categoryId, subCategoryId);

            // Return the products as JSON (note that Product objects must have ProductId and ProductName to work)
            return Json(products.Select(p => new { ProductId = p.ProductId, ProductName = p.ProductName }), JsonRequestBehavior.AllowGet);
        }







        [HttpPost]
        public async Task<ActionResult> CommonRawProduct(VMCommonProduct vmCommonProduct)
        {

            //if (vmCommonProduct.ImageFile != null)
            //{
            //    vmCommonProduct.Image = _service.UploadFile(vmCommonProduct.ImageFile, "Product", _webHostEnvironment.WebRootPath);
            //}
            if (vmCommonProduct.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProduct.ProductType = "R";

                vmCommonProduct = await _service.ProductAdd(vmCommonProduct);
            }
            else if (vmCommonProduct.ActionEum == ActionEnum.Edit)
            {
                //Edit
                vmCommonProduct = await _service.ProductEdit(vmCommonProduct);
            }
            else if (vmCommonProduct.ActionEum == ActionEnum.Delete)
            {
                //Delete
                vmCommonProduct = await _service.ProductDelete(vmCommonProduct.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonRawProduct), new { companyId = vmCommonProduct.CompanyFK, categoryId = vmCommonProduct.Common_ProductCategoryFk, subCategoryId = vmCommonProduct.Common_ProductSubCategoryFk });
        }




        #endregion

        #region Packing Category

        public async Task<ActionResult> CommonPackingCategory(int companyId)
        {

            VMCommonProductCategory vmCommonProductCategory = new VMCommonProductCategory();
            vmCommonProductCategory = await Task.Run(() => _service.GetFinishProductCategory(companyId, "P"));
            return View(vmCommonProductCategory);
        }
        [HttpPost]
        public async Task<ActionResult> CommonPackingCategory(VMCommonProductCategory vmCommonProductCategory)
        {

            if (vmCommonProductCategory.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProductCategory.ProductType = "P";

                await _service.ProductFinishCategoryAdd(vmCommonProductCategory);
            }
            else if (vmCommonProductCategory.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductFinishCategoryEdit(vmCommonProductCategory);
            }
            else if (vmCommonProductCategory.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductFinishCategoryDelete(vmCommonProductCategory.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonPackingCategory), new { companyId = vmCommonProductCategory.CompanyFK });
        }


        #endregion

        #region Common Raw Product SubCategory


        public async Task<ActionResult> CommonPackingSubCategory(int companyId, int categoryId = 0)
        {

            VMCommonProductSubCategory vmCommonProductSubCategory = new VMCommonProductSubCategory();
            vmCommonProductSubCategory = await Task.Run(() => _service.GetProductSubCategory(companyId, categoryId, "P"));
            return View(vmCommonProductSubCategory);
        }
        [HttpPost]
        public async Task<ActionResult> CommonPackingSubCategory(VMCommonProductSubCategory vmCommonProductSubCategory)
        {

            if (vmCommonProductSubCategory.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProductSubCategory.ProductType = "P";

                await _service.ProductSubCategoryAdd(vmCommonProductSubCategory);
            }
            else if (vmCommonProductSubCategory.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductSubCategoryEdit(vmCommonProductSubCategory);
            }
            else if (vmCommonProductSubCategory.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductSubCategoryDelete(vmCommonProductSubCategory.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonPackingSubCategory), new { companyId = vmCommonProductSubCategory.CompanyFK, categoryId = vmCommonProductSubCategory.Common_ProductCategoryFk });
        }


        #endregion

        #region Common Raw Product
        public async Task<ActionResult> CommonPackingMaterials(int companyId, int categoryId = 0, int subCategoryId = 0)
        {

            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            vmCommonProduct = await Task.Run(() => _service.GetProduct(companyId, categoryId, subCategoryId, "P"));

            vmCommonProduct.UnitList = new SelectList(_service.UnitDropDownList(companyId), "Value", "Text");


            return View(vmCommonProduct);
        }
        [HttpPost]
        public async Task<ActionResult> CommonPackingMaterials(VMCommonProduct vmCommonProduct)
        {

            //if (vmCommonProduct.ImageFile != null)
            //{
            //    vmCommonProduct.Image = _service.UploadFile(vmCommonProduct.ImageFile, "Product", _webHostEnvironment.WebRootPath);
            //}
            if (vmCommonProduct.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProduct.ProductType = "P";

                await _service.ProductAdd(vmCommonProduct);
            }
            else if (vmCommonProduct.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductEdit(vmCommonProduct);
            }
            else if (vmCommonProduct.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductDelete(vmCommonProduct.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonPackingMaterials), new { companyId = vmCommonProduct.CompanyFK, categoryId = vmCommonProduct.Common_ProductCategoryFk, subCategoryId = vmCommonProduct.Common_ProductSubCategoryFk });
        }




        #endregion





        #region Common Zone

        public async Task<ActionResult> POTremsAndConditions(int companyId)
        {
            VMPOTremsAndConditions vmTremsAndConditions = new VMPOTremsAndConditions();
            vmTremsAndConditions = await Task.Run(() => _service.GetPOTremsAndConditions(companyId));
            return View(vmTremsAndConditions);
        }
        [HttpPost]

        public async Task<ActionResult> POTremsAndConditions(VMPOTremsAndConditions vmpoTremsAndConditions)
        {

            if (vmpoTremsAndConditions.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.POTremsAndConditionAdd(vmpoTremsAndConditions);
            }
            else if (vmpoTremsAndConditions.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.POTremsAndConditionEdit(vmpoTremsAndConditions);
            }
            else if (vmpoTremsAndConditions.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.POTremsAndConditionDelete(vmpoTremsAndConditions.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(POTremsAndConditions), new { companyId = vmpoTremsAndConditions.CompanyFK });
        }


        #endregion
        public JsonResult RMUnitAndClosingRateByProductId(int productId)
        {
            var model = _service.GetRMUnitAndClosingRateByProductId(productId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RMUnitAndClosingRateByProductIdByLot(int companyId,int productId,string lotnumber)
        {
            var model = _service.GetRMUnitAndClosingRateByProductIdByLot(companyId,productId,lotnumber);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CommonProductByIDGet(int id)
        {
            var model = _service.GetCommonProductByID(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FinishProductBOMsByIDGet(int id)
        {
            var model = _service.GetFinishProductBOMsByID(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ChangeProductMRPPrice(int id = 0)
        {

            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            if (id > 0)
            {
                vmCommonProduct = _service.GetCommonProductByID(id);
            }


            return View(vmCommonProduct);
        }
        [HttpPost]
        public async Task<ActionResult> ChangeProductMRPPrice(VMCommonProduct vmCommonProduct)
        {

            if (vmCommonProduct.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductMRPPriceEdit(vmCommonProduct);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(ChangeProductMRPPrice), new { id = vmCommonProduct.ID });
        }


        //public async Task<ActionResult> MakeCommonProductGRN(int id = 0)
        //{
        //    if (_vmLogin == null)
        //    {
        //        return RedirectToAction("Login", "Auth");
        //    }
        //    VMCommonProduct vmCommonProduct = new VMCommonProduct();
        //    if (id > 0)
        //    {
        //        vmCommonProduct = _service.GetCommonProductByID(id);
        //    }

        //    return View(vmCommonProduct);
        //}


        //[HttpPost]
        //public async Task<ActionResult> MakeCommonProductGRN(VMCommonProduct vmCommonProduct)
        //{

        //    if (vmCommonProduct.ActionEum == ActionEnum.Edit)
        //    {
        //        //Edit
        //        await _service.ProductGRNEdit(vmCommonProduct);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Error");
        //    }
        //    return RedirectToAction(nameof(MakeCommonProductGRN), new { id = vmCommonProduct.ID });
        //}


        public JsonResult CommonProductByID(int id)
        {

            var model = _service.GetCommonProductByID(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCommonProductByProducId(int id, string LotNo,int CompanyId=0)
        {

            var model = _service.GetCommonProductByProducId(id,LotNo,CompanyId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutoCompleteSupplier(int companyId, string prefix)
        {
            var products = _service.GetAutoCompleteSupplier(companyId, prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoCompleteSupplierGet(string id)
        {
            var products = _service.GetAutoCompleteSupplier(id);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoCompleteProductCategoryGet(int companyId, string prefix, string productType)
        {
            var products = _service.GetAutoCompleteProductCategory(companyId, prefix, productType);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoCompleteProductGet(int companyId, string prefix, string productType = "")
        {
            var products = _service.GetAutoCompleteProduct(companyId, prefix, productType);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult AutoCompleteRawPackingMaterialsGet(int companyId, string prefix)
        {
            var products = _service.GetAutoCompleteRawPackingMaterials(companyId, prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutoCompleteFinishedGoodsGet(int companyId, string prefix)
        {

            var products = _service.GetAutoCompleteFinishedGoods(companyId, prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLotNymber(int companyId, int ProductId)
        {

            var products = _service.GetAutoCompleteLot(companyId, ProductId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLotNymberRaw(int ProductId)
        {

            var products = _service.GetAutoCompleteLotRaw(ProductId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAutoCompleteRawGoods(int companyId, string prefix)
        {
            var products = _service.GetAutoCompleteRawGoods(companyId, prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAutoCompleteSalesPerson( string prefix, int companyId)
        {
            var products = _service.GetAutoCompleteSaleperson(companyId, prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }





        public JsonResult getallEmployee(string prefix)
        {
            var products = _service.AllEmployee(prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getallEmployeeId(string prefix)
        {
            var products = _service.AllEmployeebyid(prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmployeeAutoComplete(string prefix)
        {
            var employees = _service.AllEmployee(prefix);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeIdByCompanyAutoComplete(string prefix, int companyId)
        {
            var employees = _service.AllEmployeeIdByCompanyId(prefix, companyId);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeByCompanyAutoComplete(string prefix, int companyId)
        {
            var employees = _service.AllEmployeeByCompanyId(prefix, companyId);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getallEmployeeforMenu(string prefix)
        {
            var products = _service.GetEmployeeforIncentive(prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AllEmployeeforIncentive(string prefix)
        {
            var products = _service.GetEmployeeforIncentive(prefix);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        #region Common Supplier


        public JsonResult CommonSupplierByIDGet(int id)
        {
            var model = _service.GetCommonSupplierByID(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }



        //public JsonResult CommonSupplierByIDGet(int id)
        //{
        //    var model = _service.GetCommonSupplierByID(id);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        public async Task<ActionResult> CommonSupplier(int companyId)
        {

            VMCommonSupplier vmCommonSupplier = new VMCommonSupplier();
            vmCommonSupplier = await Task.Run(() => _service.GetSupplier(companyId));
            vmCommonSupplier.CountryList = new SelectList(_service.CommonCountriesDropDownList(), "Value", "Text");
            return View(vmCommonSupplier);
        }
        [HttpPost]
        public async Task<ActionResult> CommonSupplier(VMCommonSupplier vmCommonSupplier)
        {

            if (vmCommonSupplier.ActionEum == ActionEnum.Add)
            {
                //Add 
               
                    await _service.SEEDSupplierAdd(vmCommonSupplier);

                
               
            }
            else if (vmCommonSupplier.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.SupplierEdit(vmCommonSupplier);
            }
            else if (vmCommonSupplier.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.SupplierDelete(vmCommonSupplier.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonSupplier), new { companyId = vmCommonSupplier.CompanyFK });
        }

        #endregion

        #region Common Customer
        public JsonResult CommonCustomerByIDGet(int id)
        {
            var model = _service.GetCommonCustomerByID(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCommonCustomerByIDForKSSL(int id)
        {
            var model = _service.GetCommonCustomerByIDForKSSL(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RSCustomerByIDGet(int id)
        {
            var model = _service.GetRSCustomerByID(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FeedCommonCustomerByIDGet(int id)
        {
            var model = _service.GetCommonCustomerByIDFeed(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> RSCommonCustomer(int companyId)
        {

            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => _service.RSGetCustomer(companyId));
            vmCommonCustomer.DivisionList = new SelectList(_service.CommonDivisionsDropDownList(), "Value", "Text");
            vmCommonCustomer.DistrictList = new SelectList(_service.CommonDistrictsDropDownList(), "Value", "Text");
            vmCommonCustomer.UpazilasList = new SelectList(_service.CommonUpazilasDropDownList(), "Value", "Text");
            vmCommonCustomer.PaymentTypeList = new SelectList(_service.CommonCustomerPaymentType(), "Value", "Text");
            vmCommonCustomer.vendorProfession = new SelectList(_pvendorService.getfordropdown(), "Value", "Text");
            return View(vmCommonCustomer);
        }

        public async Task<ActionResult> RSCommonCustomerBooking(int companyId, int vendorId=0, int ProductSubCategoryId=0)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            if (vendorId > 0)
            {
                vmCommonCustomer = await Task.Run(() => _service.RSGetCustomerBooking(companyId, vendorId));
            }
            else
            {
                vmCommonCustomer = await Task.Run(() => _service.RSGetCustomerBookingProductCategories(companyId, ProductSubCategoryId));

            }
            return View(vmCommonCustomer);
        }
        public async Task<ActionResult> RSCommonCustomerGroup(int companyId, int vendorId)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => _service.RSGetCustomerGroup(companyId, vendorId));
            vmCommonCustomer.DivisionList = new SelectList(_service.CommonDivisionsDropDownList(), "Value", "Text");
            vmCommonCustomer.DistrictList = new SelectList(_service.CommonDistrictsDropDownList(), "Value", "Text");
            vmCommonCustomer.UpazilasList = new SelectList(_service.CommonUpazilasDropDownList(), "Value", "Text");
            vmCommonCustomer.PaymentTypeList = new SelectList(_service.CommonCustomerPaymentType(), "Value", "Text");

            return View(vmCommonCustomer);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteVendorImageFile(long docId)
        {
            var result = await _ftpservice.DeletePermanentlyVendor(docId);
            return Json(result);
        }


        [HttpPost]
        public async Task<ActionResult> RSCommonCustomerGroup(VMCommonSupplier vmCommonCustomer, HttpPostedFileBase file)
        {

            List<FileItem> itemlist = new List<FileItem>();
            if (file != null)
            {
                FileItem item = new FileItem();
                item.file = file;
                item.docdesc = "Vendor Photo";
                item.docfilename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                item.docid = 0;
                item.FileCatagoryId = 4;
                item.fileext = Path.GetExtension(file.FileName);
                item.isactive = true;
                item.RecDate = DateTime.Now;
                item.SortOrder = 1;
                item.userid = Convert.ToInt32(Session["Id"]);
                itemlist.Add(item);
                itemlist = await _ftpservice.UploadFileBulk(itemlist, "VendorImage");
                if (file != null)
                {
                    var ress = itemlist.FirstOrDefault(f => f.SortOrder == 1);
                    vmCommonCustomer.ImageDocId = ress.docid;
                }
            }

            if (vmCommonCustomer.ActionEum == ActionEnum.Add)
            {
                //Add 
                var res = await _service.RSCustomerGroupAdd(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.RSCustomerGroupEdit(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.CustomerDelete(vmCommonCustomer.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(RSCommonCustomerGroup), new { companyId = vmCommonCustomer.CompanyFK, vendorId = vmCommonCustomer.VendorReferenceId });
        }

        public async Task<ActionResult> PackagingCustomer(int companyId, int zoneId = 0, int subZoneId = 0)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => _service.GetPackagingCustomer(companyId, zoneId, subZoneId));
            vmCommonCustomer.DivisionList = new SelectList(_service.CommonDivisionsDropDownList(), "Value", "Text");
            vmCommonCustomer.DistrictList = new SelectList(_service.CommonDistrictsDropDownList(), "Value", "Text");
            vmCommonCustomer.UpazilasList = new SelectList(_service.CommonUpazilasDropDownList(), "Value", "Text");
            vmCommonCustomer.PaymentTypeList = new SelectList(_service.CommonCustomerPaymentType(), "Value", "Text");
             return View(vmCommonCustomer);
        }


        [HttpPost]
        public async Task<ActionResult> PackagingCustomer(VMCommonSupplier vmCommonCustomer, HttpPostedFileBase file)
        {
            List<FileItem> itemlist = new List<FileItem>();
            if (file != null)
            {
                FileItem item = new FileItem();
                item.file = file;
                item.docdesc = "Packaging Vendor Photo";
                item.docfilename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                item.docid = 0;
                item.FileCatagoryId = 5;
                item.fileext = Path.GetExtension(file.FileName);
                item.isactive = true;
                item.RecDate = DateTime.Now;
                item.SortOrder = 1;
                item.userid = Convert.ToInt32(Session["Id"]);
                itemlist.Add(item);
                itemlist = await _ftpservice.UploadFileBulk(itemlist, "PackagingVendorImage");
                if (file != null)
                {
                    var ress = itemlist.FirstOrDefault(f => f.SortOrder == 1);
                    vmCommonCustomer.ImageDocId = ress.docid;
                }
            }
            if (vmCommonCustomer.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.PackagingCustomerAdd(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.CustomerEdit(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.CustomerDelete(vmCommonCustomer.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
          
            return RedirectToAction(nameof(PackagingCustomer), new { companyId = vmCommonCustomer.CompanyFK });
        }





        public async Task<ActionResult> CommonCustomer(int companyId, int zoneId = 0, int subZoneId = 0)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => _service.GetCustomer(companyId, zoneId, subZoneId));
            vmCommonCustomer.DivisionList = new SelectList(_service.CommonDivisionsDropDownList(), "Value", "Text");
            vmCommonCustomer.DistrictList = new SelectList(_service.CommonDistrictsDropDownList(), "Value", "Text");
            vmCommonCustomer.UpazilasList = new SelectList(_service.CommonUpazilasDropDownList(), "Value", "Text");
            vmCommonCustomer.PaymentTypeList = new SelectList(_service.CommonCustomerPaymentType(), "Value", "Text");
            vmCommonCustomer.ZoneListList = new SelectList(_service.CommonZonesDropDownList(companyId), "Value", "Text");
            vmCommonCustomer.TerritoryList = new SelectList(_service.CommonSubZonesDropDownList(companyId), "Value", "Text");
            vmCommonCustomer.Checkdetail = new SelectList(_service.ChekdetailList(), "Value", "Text");
            vmCommonCustomer.CheckType = new SelectList(_service.ChekdTypeList(), "Value", "Text");
            return View(vmCommonCustomer);
        }

        public async Task<ActionResult> CommonCustomerForKSSL(int companyId, int zoneId = 0, int subZoneId = 0)
        {
            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await _service.GetCustomerForKSSL(companyId, zoneId, subZoneId);
            vmCommonCustomer.DivisionList = new SelectList(_service.CommonDivisionsDropDownList(), "Value", "Text");
            vmCommonCustomer.DistrictList = new SelectList(_service.CommonDistrictsDropDownList(), "Value", "Text");
            vmCommonCustomer.UpazilasList = new SelectList(_service.CommonUpazilasDropDownList(), "Value", "Text");
            vmCommonCustomer.PaymentTypeList = new SelectList(_service.CommonCustomerPaymentType(), "Value", "Text");
            vmCommonCustomer.ZoneListList = new SelectList(_service.CommonZonesDropDownList(companyId), "Value", "Text");
            return View(vmCommonCustomer);
        }


        public async Task<ActionResult> CommonFeedCustomer(int companyId, int zoneId = 0, int subZoneId = 0)
        {

            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => _service.GetFeedCustomer(companyId, zoneId));
            vmCommonCustomer.DivisionList = new SelectList(_service.CommonDivisionsDropDownList(), "Value", "Text");
            vmCommonCustomer.DistrictList = new SelectList(_service.CommonDistrictsDropDownList(), "Value", "Text");
            vmCommonCustomer.UpazilasList = new SelectList(_service.CommonUpazilasDropDownList(), "Value", "Text");
            vmCommonCustomer.PaymentTypeList = new SelectList(_service.CommonCustomerPaymentType(), "Value", "Text");
            vmCommonCustomer.NomineeRelationList = new SelectList(_service.CommonRelationList(), "Value", "Text");
            vmCommonCustomer.ZoneListList = new SelectList(_service.CommonZonesDropDownList(companyId), "Value", "Text");
            vmCommonCustomer.TerritoryList = new SelectList(_service.CommonSubZonesDropDownList(companyId), "Value", "Text");
            vmCommonCustomer.EmployeeList = new SelectList(_service.EmployeesDropDownList(companyId), "Value", "Text");

            vmCommonCustomer.Months = _vendorService.GetMonthSelectModes();
            vmCommonCustomer.VendorTypeId =(int)Provider.Customer;
           
            vmCommonCustomer.Condition = "Condition : If customer fails to 100% closing, any incentive, carrying and any other adjustment will not be adjusted.";


            return View(vmCommonCustomer);
        }

        public async Task<ActionResult> KFMALCustomer(int companyId, int zoneId = 0, int subZoneId = 0)
        {

            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => _service.GetKfmalCustomer(companyId, zoneId));
            vmCommonCustomer.DivisionList = new SelectList(_service.CommonDivisionsDropDownList(), "Value", "Text");
            vmCommonCustomer.DistrictList = new SelectList(_service.CommonDistrictsDropDownList(), "Value", "Text");
            vmCommonCustomer.UpazilasList = new SelectList(_service.CommonUpazilasDropDownList(), "Value", "Text");
            vmCommonCustomer.PaymentTypeList = new SelectList(_service.CommonCustomerPaymentType(), "Value", "Text");
            vmCommonCustomer.NomineeRelationList = new SelectList(_service.CommonRelationList(), "Value", "Text");
            vmCommonCustomer.ZoneListList = new SelectList(_service.CommonZonesDropDownList(companyId), "Value", "Text");
            vmCommonCustomer.TerritoryList = new SelectList(_service.CommonSubZonesDropDownList(companyId), "Value", "Text");
         
            vmCommonCustomer.VendorTypeId = (int)Provider.Customer;
            

            return View(vmCommonCustomer);
        }

        [HttpPost]
        public async Task<ActionResult> RSCommonCustomer(VMCommonSupplier vmCommonCustomer, HttpPostedFileBase file)
        {

            List<FileItem> itemlist = new List<FileItem>();
            if (file != null)
            {
                FileItem item = new FileItem();
                item.file = file;
                item.docdesc = "Vendor Photo";
                item.docfilename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                item.docid = 0;
                item.FileCatagoryId = 4;
                item.fileext = Path.GetExtension(file.FileName);
                item.isactive = true;
                item.RecDate = DateTime.Now;
                item.SortOrder = 1;
                item.userid = Convert.ToInt32(Session["Id"]);
                itemlist.Add(item);
                itemlist = await _ftpservice.UploadFileBulk(itemlist, "VendorImage");
                if (file != null)
                {
                    var ress = itemlist.FirstOrDefault(f => f.SortOrder == 1);
                    vmCommonCustomer.ImageDocId = ress.docid;
                }
            }
            if (vmCommonCustomer.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.RSCustomerAdd(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.RSCustomerEdit(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.CustomerDelete(vmCommonCustomer.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(RSCommonCustomer), new { companyId = vmCommonCustomer.CompanyFK });
        }
        [HttpPost]
        public async Task<JsonResult> CGFileDelete(long docId, long CGId)
        {
           
            return Json(false);
        }

        [HttpGet]
        public async Task<ActionResult> CustomerUplode(int customerId)
        {
            VMCommonSupplier vmCommonCustomer=new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => _service.GetCustomerBuID2(customerId));
            return View(vmCommonCustomer);  
         }
        [HttpPost]
        public async Task<ActionResult> CommonCustomeruplode(VMCommonSupplier model)
        {

            if (model.Attachments == null)
            {
                return RedirectToAction("CustomerUplode", new { customerId = model.ID });
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
                        FileCatagoryId = 5,
                        fileext = Path.GetExtension(model.Attachments[i].File.FileName),
                        docfilename = Guid.NewGuid().ToString() + Path.GetExtension(model.Attachments[i].File.FileName),
                        isactive = true,
                        RecDate = DateTime.Now,
                        SortOrder = i,
                        userid = Convert.ToInt32(Session["Id"])
                    });
                }
            }
            itemlist = await _ftpservice.UploadFileBulk(itemlist, model.ID.ToString());
            long CGId = Convert.ToInt64(model.ID);
            var result = await gLDLCustomerService.FileMapping(itemlist, CGId);
            return RedirectToAction("CustomerUplode", new { customerId = model.ID});
        }

        [HttpPost]
        public async Task<ActionResult> CommonCustomerForKSSL(VMCommonSupplier vmCommonCustomer, HttpPostedFileBase file)
        {
            List<FileItem> itemlist = new List<FileItem>();
            if (file != null)
            {
                FileItem item = new FileItem();
                item.file = file;
                item.docdesc = "Feed Vendor Photo";
                item.docfilename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                item.docid = 0;
                item.FileCatagoryId = 5;
                item.fileext = Path.GetExtension(file.FileName);
                item.isactive = true;
                item.RecDate = DateTime.Now;
                item.SortOrder = 1;
                item.userid = Convert.ToInt32(Session["Id"]);
                itemlist.Add(item);
                itemlist = await _ftpservice.UploadFileBulk(itemlist, "FeedVendorImage");
                if (file != null)
                {
                    var ress = itemlist.FirstOrDefault(f => f.SortOrder == 1);
                    vmCommonCustomer.ImageDocId = ress.docid;
                }
            }

            if (vmCommonCustomer.ActionEum == ActionEnum.Add)
            {

                //Add 
                await _service.CustomerKSSLAdd(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.CustomerEdit(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.CustomerDelete(vmCommonCustomer.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonCustomerForKSSL), new { companyId = vmCommonCustomer.CompanyFK });

        }


        [HttpPost]
        public async Task<ActionResult> CommonCustomer(VMCommonSupplier vmCommonCustomer,HttpPostedFileBase file2)
        {
            if (Request.Files.Count > 0)
            {
                foreach (string fileKey in Request.Files)
                {
                    var file = Request.Files[fileKey];
                    if (file != null && file.ContentLength > 0)
                    {
                        // Define the path where you want to save the uploaded files
                        var uploadPath = Path.Combine(Server.MapPath("~/FileUpload"), Path.GetFileName(file.FileName));

                        // Save the file
                        file.SaveAs(uploadPath);

                        // Generate the file URL
                        var fileUrl = Url.Content("~/FileUpload/" + Path.GetFileName(file.FileName));

                        // Match the file input field name and save the URL to the corresponding model property
                        switch (fileKey)
                        {
                            case "Image":
                                vmCommonCustomer.Imageurl = fileUrl;
                                break;
                            case "NidImage":
                                vmCommonCustomer.NidImage = fileUrl;
                                break;
                            case "TradeLicenceUrl":
                                vmCommonCustomer.TradeLicenceUrl = fileUrl;
                                break;
                            case "BSAMemUrl":
                                vmCommonCustomer.BSAMemUrl = fileUrl;
                                break;
                            case "SaleLiUrl":
                                vmCommonCustomer.SaleLiUrl = fileUrl;
                                break;
                            case "DelerLiUrl":
                                vmCommonCustomer.DelerLiUrl = fileUrl;
                                break;
                            case "TinUrl":
                                vmCommonCustomer.TinUrl = fileUrl;
                                break;
                            case "BankChkUrl":
                                vmCommonCustomer.BankChkUrl = fileUrl;
                                break;
                            default:
                                // Handle other cases if needed
                                break;
                        }
                    }
                }
            }

            if (vmCommonCustomer.ActionEum == ActionEnum.Add)
            {

                //Add 
                await _service.CustomerAdd(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.CustomerEdit(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.CustomerDelete(vmCommonCustomer.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            //if (vmCommonCustomer.CompanyFK == (int)CompanyName.GloriousLandsAndDevelopmentsLimited || vmCommonCustomer.CompanyFK == (int)CompanyName.KrishibidPropertiesLimited)
            //{
            //    return RedirectToAction(nameof(RSCommonCustomer), new { companyId = vmCommonCustomer.CompanyFK });
            //}
            //if (vmCommonCustomer.CompanyFK == (int)CompanyName.KrishibidFeedLimited)
            //{
            //    return RedirectToAction(nameof(CommonFeedCustomer), new { companyId = vmCommonCustomer.CompanyFK });
            //}
            return RedirectToAction(nameof(CommonCustomer), new { companyId = vmCommonCustomer.CompanyFK });
        }


        [HttpPost]
        public async Task<ActionResult> KFMALCustomer(VMCommonSupplier vmCommonCustomer, HttpPostedFileBase file)
        {
            List<FileItem> itemlist = new List<FileItem>();
            #region File
            if (file != null)
            {
                FileItem item = new FileItem();
                item.file = file;
                item.docdesc = "Feed Vendor Photo";
                item.docfilename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                item.docid = 0;
                item.FileCatagoryId = 5;
                item.fileext = Path.GetExtension(file.FileName);
                item.isactive = true;
                item.RecDate = DateTime.Now;
                item.SortOrder = 1;
                item.userid = Convert.ToInt32(Session["Id"]);
                itemlist.Add(item);
                itemlist = await _ftpservice.UploadFileBulk(itemlist, "FeedVendorImage");
                if (file != null)
                {
                    var ress = itemlist.FirstOrDefault(f => f.SortOrder == 1);
                    vmCommonCustomer.ImageDocId = ress.docid;
                }
            }
            #endregion

            if (vmCommonCustomer.ActionEum == ActionEnum.Add)
            {

                //Add 
                await _service.KfmalCustomerAdd(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.KfmalCustomerEdit(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.CustomerDelete(vmCommonCustomer.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            if (vmCommonCustomer.CompanyFK == (int)CompanyName.GloriousLandsAndDevelopmentsLimited || vmCommonCustomer.CompanyFK == (int)CompanyName.KrishibidPropertiesLimited)
            {
                return RedirectToAction(nameof(RSCommonCustomer), new { companyId = vmCommonCustomer.CompanyFK });
            }
            if (vmCommonCustomer.CompanyFK == (int)CompanyName.KrishibidFeedLimited)
            {
                return RedirectToAction(nameof(CommonFeedCustomer), new { companyId = vmCommonCustomer.CompanyFK });
            }
            return RedirectToAction(nameof(CommonCustomer), new { companyId = vmCommonCustomer.CompanyFK });
        }
        [HttpPost]
        public async Task<ActionResult> CommonCustomerDelete(VMCommonCustomer vmCommonCustomer)
        {

            if (vmCommonCustomer.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.CustomerDelete(vmCommonCustomer.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonCustomer));
        }
        public async Task<ActionResult> CommonCustomerByID(int customerId)
        {

            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => _service.GetCustomerBuID(customerId));



            return View(vmCommonCustomer);
        }

        public async Task<ActionResult> CommonCustomerByIDForKSSL(int customerId)
        {

            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => _service.GetCustomerByID(customerId));



            return View(vmCommonCustomer);
        }
        #endregion


        #region Geolocation


        public async Task<ActionResult> CommonDivisions()
        {
            VMCommonDivisions vmCommonDivisions = new VMCommonDivisions();
            vmCommonDivisions = await Task.Run(() => _service.GetDivisions());
            return View(vmCommonDivisions);
        }

        public async Task<ActionResult> CommonDistricts(int divisionsId = 0)
        {

            VMCommonDistricts vmCommonDistricts = new VMCommonDistricts();
            vmCommonDistricts = await Task.Run(() => _service.GetDistricts(divisionsId));
            vmCommonDistricts.DivisionList = new SelectList(_service.CommonDivisionsDropDownList(), "Value", "Text");
            return View(vmCommonDistricts);
        }

        public async Task<ActionResult> CommonUpazilas(int divisionsId = 0, int districtsId = 0)
        {


            VMCommonThana vmCommonThana = new VMCommonThana();
            vmCommonThana = await Task.Run(() => _service.GetUpazilas(divisionsId, districtsId));
            vmCommonThana.DistrictList = await Task.Run(() => _service.GetDistrictsDropDownList());
            return View(vmCommonThana);
        }
        [HttpGet]
        public async Task<ActionResult> CommonDistrictsGet(int id)
        {

            var vmCDistricts = await Task.Run(() => _service.CommonDistrictsGet(id));
            var list = vmCDistricts.Select(x => new { Value = x.ID, Text = x.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CommonSubZonesGet(int id)
        {

            var vmCDistricts = await Task.Run(() => _service.CommonSubZonesGet(id));
            var list = vmCDistricts.Select(x => new { Value = x.ID, Text = x.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> CommonUpazilasGet(int id)
        {

            var vmPoliceStations = await Task.Run(() => _service.DDLUpazilasListByDistrictID(id));
            return Json(vmPoliceStations, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpazilaGetById(int id)
        {

            var vmPoliceStations = await Task.Run(() => _service.UpazilasById(id));
            return Json(vmPoliceStations, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> EmployeeGet(int zoneId)
        {

            var vmPoliceStations = await Task.Run(() => _service.EmployeesByZoneId(zoneId));
            return Json(vmPoliceStations, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public async Task<ActionResult> AccountingSignatory(int companyId)
        {

            VMAccountingSignatory vmAccountingSignatory = new VMAccountingSignatory();
            vmAccountingSignatory = await Task.Run(() => _service.GetAccountingSignatory(companyId));
            vmAccountingSignatory.CompanyList = new SelectList(_service.CompaniesDropDownList(companyId), "Value", "Text");
            vmAccountingSignatory.DDLEmployee = _dropdownService.RenderDDL(await _dropDownItemService.GetDDLAllEmployeeByCompanyId(companyId), true);
            return View(vmAccountingSignatory);
        }
        [HttpPost]
        public async Task<ActionResult> AccountingSignatory(VMAccountingSignatory vmAccountingSignatory)
        {

            if (vmAccountingSignatory.ActionEum == ActionEnum.Add)
            {


                await _service.AccountingSignatoryAdd(vmAccountingSignatory);
            }
            else if (vmAccountingSignatory.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.AccountingSignatoryEdit(vmAccountingSignatory);
            }
            else if (vmAccountingSignatory.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.AccountingSignatoryDelete(vmAccountingSignatory.SignatoryId);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(AccountingSignatory), new { companyId = vmAccountingSignatory.CompanyFK });
        }


        public async Task<ActionResult> Company()
        {

            VMCompany VMCompany = new VMCompany();
            VMCompany = await Task.Run(() => _service.GetCompany());
            //VMCompany.CompanyList = new SelectList(_service.CompaniesDropDownList(), "Value", "Text");

            return View(VMCompany);
        }
        [HttpPost]
        public async Task<ActionResult> Company(VMCompany VMCompany)
        {

            if (VMCompany.ActionEum == ActionEnum.Add)
            {


                await _service.CompanyAdd(VMCompany);
            }
            //else if (VMCompany.ActionEum == ActionEnum.Edit)
            //{
            //    //Edit
            //    await _service.AccountingSignatoryEdit(VMCompany);
            //}
            //else if (VMCompany.ActionEum == ActionEnum.Delete)
            //{
            //    //Delete
            //    await _service.AccountingSignatoryDelete(VMCompany.SignatoryId);
            //}
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(Company));
        }


        #region Common Bank





        public async Task<ActionResult> CommonBank(int companyId)
        {
            VMCommonBank vmCommonBank = new VMCommonBank();
            vmCommonBank = await Task.Run(() => _service.GetBanks(companyId));
            return View(vmCommonBank);
        }

        
        [HttpPost]
        public async Task<ActionResult> CommonBank(VMCommonBank vMCommonBank)
        {

            if (vMCommonBank.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.BankAdd(vMCommonBank);
            }
            else if (vMCommonBank.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.BankEdit(vMCommonBank);
            }
            else if (vMCommonBank.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.BankDelete(vMCommonBank.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonBank), new { companyId = vMCommonBank.CompanyFK });
        }


        #endregion
        public async Task<ActionResult> CommonShift(int companyId)
        {
            VMCommonShift vMCommonShift = new VMCommonShift();
            vMCommonShift = await Task.Run(() => _service.GetShift(companyId));
            return View(vMCommonShift);
        }
        [HttpPost]
        public async Task<ActionResult> CommonShift(VMCommonShift vMCommonShift)
        {

            if (vMCommonShift.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.ShiftAdd(vMCommonShift);
            }
            else if (vMCommonShift.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ShiftEdit(vMCommonShift);
            }
            else if (vMCommonShift.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ShiftDelete(vMCommonShift.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonShift), new { companyId = vMCommonShift.CompanyFK });
        }


        public async Task<ActionResult> CommonDesignation(int companyId)
        {
            VMCommonDesignation commonDesignation = new VMCommonDesignation();
            commonDesignation = await Task.Run(() => _service.GetDesignation(companyId));
            return View(commonDesignation);
        }
        [HttpPost]
        public async Task<ActionResult> CommonDesignation(VMCommonDesignation vMCommonDesignation)
        {

            if (vMCommonDesignation.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.DesignationAdd(vMCommonDesignation);
            }
            else if (vMCommonDesignation.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.DesignationEdit(vMCommonDesignation);
            }
            else if (vMCommonDesignation.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.DesignationDelete(vMCommonDesignation.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonDesignation), new { companyId = vMCommonDesignation.CompanyFK });
        }

        #region Bank Branch
        public async Task<ActionResult> CommonBankBranchGet(int companyId, int bankId)
        {

            var vmCommonProductSubCategory = await Task.Run(() => _service.CommonBankGet(companyId, bankId));
            var list = vmCommonProductSubCategory.Select(x => new { Value = x.ID, Text = x.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> CommonBankBranch(int companyId, int bankId = 0)
        {

            VMCommonBankBranch vMCommonBankBranch = new VMCommonBankBranch();
            vMCommonBankBranch = await Task.Run(() => _service.GetBankBranchs(companyId, bankId));
            vMCommonBankBranch.BankList = new SelectList(_service.CommonBanksDropDownList(companyId), "Value", "Text");

            return View(vMCommonBankBranch);
        }
        [HttpPost]
        public async Task<ActionResult> CommonBankBranch(VMCommonBankBranch vMCommonBankBranch)
        {

            if (vMCommonBankBranch.ActionEum == ActionEnum.Add)
            {
                //Add 

                await _service.BankBranchAdd(vMCommonBankBranch);
            }
            else if (vMCommonBankBranch.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.BankBranchEdit(vMCommonBankBranch);
            }
            else if (vMCommonBankBranch.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.BankBranchDelete(vMCommonBankBranch.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonBankBranch), new { companyId = vMCommonBankBranch.CompanyFK, bankId = vMCommonBankBranch.BankId });
        }
        #endregion

        #region Account Cheque Info

        public JsonResult CommonActChequeInfoByIDGet(int id)
        {
            var model = _service.GetActChequeInfoByID(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CommonActChequeInfo(int companyId)
        {

            VMCommonActChequeInfo vMCommonActChequeInfo = new VMCommonActChequeInfo();
            vMCommonActChequeInfo = await Task.Run(() => _service.GetActChequeInfos(companyId));
            vMCommonActChequeInfo.BankList =
                new SelectList(_service.CommonBanksDropDownList(companyId), "Value", "Text");

            vMCommonActChequeInfo.BankBranchList =
               new SelectList(_service.CommonBankBranchsDropDownList(companyId), "Value", "Text");


            vMCommonActChequeInfo.SignatoryList =
              new SelectList(_service.CommonActSignatorysDropDownList(companyId), "Value", "Text");

            return View(vMCommonActChequeInfo);
        }
        [HttpPost]
        public async Task<ActionResult> CommonActChequeInfo(VMCommonActChequeInfo vMCommonActChequeInfo)
        {

            if (vMCommonActChequeInfo.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.ActChequeInfoAdd(vMCommonActChequeInfo);
            }
            else if (vMCommonActChequeInfo.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ActChequeInfoEdit(vMCommonActChequeInfo);
            }
            else if (vMCommonActChequeInfo.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ActChequeInfoDelete(vMCommonActChequeInfo.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonActChequeInfo), new { companyId = vMCommonActChequeInfo.CompanyFK });
        }
        #endregion




        #region Common Raw Product Category

        public async Task<ActionResult> ProductCategoryProject(int companyId, string productType)
        {

            VMCommonProductCategory vmCommonProductCategory = new VMCommonProductCategory();
            vmCommonProductCategory = await Task.Run(() => _service.GetProductCategoryProject(companyId, productType));
            return View(vmCommonProductCategory);
        }
        [HttpPost]
        public async Task<ActionResult> ProductCategoryProject(VMCommonProductCategory vmProductCategoryProject)
        {

            if (vmProductCategoryProject.ActionEum == ActionEnum.Add)
            {
                //Add                

                await _service.ProductCategoryProjectAdd(vmProductCategoryProject);
            }
            else if (vmProductCategoryProject.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductCategoryProjectEdit(vmProductCategoryProject);
            }
            else if (vmProductCategoryProject.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductCategoryProjectDelete(vmProductCategoryProject.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(ProductCategoryProject), new { companyId = vmProductCategoryProject.CompanyFK, productType = vmProductCategoryProject.ProductType });
        }
        [HttpGet]
        public async Task<JsonResult> GetProjectDetailsById(int id)
        {
            var obj = await _service.GetProjectDetailsById(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Common Raw Product SubCategory


        public async Task<ActionResult> ProductSubCategoryBlock(int companyId, int categoryId = 0, string productType = "")
        {

            VMCommonProductSubCategory vmCommonProductSubCategory = new VMCommonProductSubCategory();

            vmCommonProductSubCategory = await Task.Run(() => _service.GetProductSubCategory(companyId, categoryId, productType));
            vmCommonProductSubCategory.ProductCategoryList = await _service.GetProductCategory(companyId, productType);
            return View(vmCommonProductSubCategory);
        }
        [HttpPost]
        public async Task<ActionResult> ProductSubCategoryBlock(VMCommonProductSubCategory vmCommonProductSubCategory)
        {

            if (vmCommonProductSubCategory.ActionEum == ActionEnum.Add)
            {
                //Add               

                await _service.ProductSubCategoryAdd(vmCommonProductSubCategory);
            }
            else if (vmCommonProductSubCategory.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductSubCategoryEdit(vmCommonProductSubCategory);
            }
            else if (vmCommonProductSubCategory.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductSubCategoryDelete(vmCommonProductSubCategory.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(ProductSubCategoryBlock), new { companyId = vmCommonProductSubCategory.CompanyFK, categoryId = vmCommonProductSubCategory.Common_ProductCategoryFk, productType = vmCommonProductSubCategory.ProductType });
        }


        #endregion

        #region Common Raw Product
        [HttpGet]
        public async Task<ActionResult> ProductPlotOrFlat(int companyId, int categoryId = 0, int subCategoryId = 0, string productType = "")
        {
            VMrealStateProductsForList vm = new VMrealStateProductsForList();
            vm = await _service.GetPlotOrFlat(companyId, categoryId, subCategoryId);
            vm.ProductCategoryList = voucherTypeService.GetProductCategoryGldl(companyId);
            vm.ProductType = productType;
            return View(vm);
        }

        [HttpGet]
        public  JsonResult ProductPlotOrFlatName(int companyId, int categoryId = 0, int subCategoryId = 0, string productName = "")
        {
            realStateProducts vm = new realStateProducts();
            vm =  _service.checkProduct(companyId, categoryId, subCategoryId,productName);
            if (vm==null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> ProductPlotOrFlat(VMRealStateProduct vmCommonProduct)
        {
            if (vmCommonProduct.ActionEum == ActionEnum.Add)
            {
                //Add                

                await _service.RealStateProductAdd(vmCommonProduct);
            }
            else if (vmCommonProduct.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.RealStateProductEdit(vmCommonProduct);
            }
            else if (vmCommonProduct.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductDelete(vmCommonProduct.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(ProductPlotOrFlat), new { companyId = vmCommonProduct.CompanyFK, productType = vmCommonProduct.ProductType });
        }

        public async Task<ActionResult> ProductPlotOrFlatEdit(int ProductId, string productType = "")
        {
            VMRealStateProduct model = new VMRealStateProduct();
            model = await _service.GetRealStateProductForEdit(ProductId);
            model.StatusList = new SelectList(_service.PlotOrPlatStatusList(model.CompanyFK.Value), "Value", "Text");

            var company = _companyService.GetCompany((int)model.CompanyFK);
            model.ActionId = (int)ActionEnum.Edit;
            model.CompanyName = company.Name;
            model.ProductType = productType;
            return View(model);
        }
        public async Task<ActionResult> ProductPlotOrFlatView(int companyId, int ProductId)
        {
            VMrealStateProductsForList model = new VMrealStateProductsForList();
            model = await _service.GetPlotOrFlatView(companyId, ProductId);
            if (companyId == 9)
            {
                var lid = model.realStateProducts.FlatProp.LandFacing;
                model.realStateProducts.LandFacingTitle = await _service.FacingName(lid);
            }

            var company = _companyService.GetCompany((int)model.CompanyId);
            model.ActionId = (int)ActionEnum.Edit;
            model.CompanyName = company.Name;

            return View(model);
        }
        //public async Task<ActionResult> ProductPlotOrFlatEdit(int companyId , int id)
        //{
        //    VMRealStateProduct vmCommonProduct = new VMRealStateProduct();
        //    vmCommonProduct =  _service.GetCommonProductByID(id);
        //    vmCommonProduct.UnitList = new SelectList(_service.UnitDropDownList(companyId), "Value", "Text");
        //    vmCommonProduct.FacingDropDown = await _service.GetFacingDropDown();
        //    return View(vmCommonProduct);
        //}
        public async Task<ActionResult> ProductPlotOrFlatCreate(int companyId, string productType = "")
        {
            VMRealStateProduct vmCommonProduct = new VMRealStateProduct();
            var company = _companyService.GetCompany(companyId);
            vmCommonProduct.CompanyFK = companyId;
            vmCommonProduct.CompanyName = company.Name;
            vmCommonProduct.ProductType = productType;
            vmCommonProduct.StatusList = new SelectList(_service.PlotOrPlatStatusList(companyId), "Value", "Text");
            vmCommonProduct.UnitList = new SelectList(_service.UnitDropDownList(companyId), "Value", "Text");
            vmCommonProduct.FacingDropDown = await _service.GetFacingDropDown();
            vmCommonProduct.GetProductCategoryList = await _service.GetProductCategory(companyId, productType);
            return View(vmCommonProduct);
        }


        #endregion



        public async Task<ActionResult> CommonClient(int companyId)
        {

            VMCommonSupplier vmCommonCustomer = new VMCommonSupplier();
            vmCommonCustomer = await Task.Run(() => _service.GetClient(companyId));
            vmCommonCustomer.DivisionList = new SelectList(_service.CommonDivisionsDropDownList(), "Value", "Text");
            vmCommonCustomer.DistrictList = new SelectList(_service.CommonDistrictsDropDownList(), "Value", "Text");
            vmCommonCustomer.UpazilasList = new SelectList(_service.CommonUpazilasDropDownList(), "Value", "Text");
            vmCommonCustomer.PaymentTypeList = new SelectList(_service.CommonCustomerPaymentType(), "Value", "Text");
            return View(vmCommonCustomer);
        }


        [HttpPost]
        public async Task<ActionResult> CommonClient(VMCommonSupplier vmCommonCustomer, HttpPostedFileBase file)
        {

            if (vmCommonCustomer.ActionEum == ActionEnum.Add)
            {
                //Add 
                await _service.CustomerAdd(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.CustomerEdit(vmCommonCustomer);
            }
            else if (vmCommonCustomer.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.CustomerDelete(vmCommonCustomer.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            if (vmCommonCustomer.CompanyFK == (int)CompanyName.KrishibidFeedLimited)
            {
                return RedirectToAction(nameof(CommonFeedCustomer), new { companyId = vmCommonCustomer.CompanyFK });
            }
            return RedirectToAction(nameof(CommonCustomer), new { companyId = vmCommonCustomer.CompanyFK });
        }
        [HttpPost]
        public async Task<ActionResult> SaveDivision(Division Model)
        { 
            var vmPoliceStations = await Task.Run(() => _service.SaveDivision(Model));
            return Json(vmPoliceStations, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public async Task<JsonResult> GetDivisionById(int id)
        {
            var obj = await _service.GetDivisionById(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> SaveUpazila(Upazila Model)
        {
            var vmPoliceStations = await Task.Run(() => _service.SaveUpazila(Model));
            return Json(vmPoliceStations, JsonRequestBehavior.AllowGet);
        }


        
        [HttpGet]
        public async Task<JsonResult> GetDistrictById(int id)
        {
            var obj = await _service.GetDistrictById(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> SaveDistrict(District Model)
        {
            var vmPoliceStations = await Task.Run(() => _service.SaveDistrict(Model));
            return Json(vmPoliceStations, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public async Task<JsonResult> DeleteDistrict(int id)
        {
            var obj = await _service.DeleteDistrict(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public async Task<ActionResult> CurrencyIndex()
        {
            CurrencyVm model = new CurrencyVm();
            model =await _service.GetCurrency();
            return View(model);
        }

     

        
        [HttpGet]
        public async Task<ActionResult> AddOrEditCurrency(int? Id)
        {
            CurrencyVm model = new CurrencyVm();
            if(Id != null)
            {
                model =await _service.GetCurencyForUpdate(Id);
                return View(model);
            }
            return View(model);
        }

        
        [HttpPost]
        public async Task<ActionResult> AddOrEditCurrency(CurrencyVm model)
        {
            var obj =await _service.SaveOrUpdateCurrency(model);

            return RedirectToAction("CurrencyIndex");
        }
        
        [HttpPost]
        public async Task<ActionResult> AddOrEditPortofCountry(PortOfCountryVm model)
        {
            var obj = await _service.SaveOrUpdatePortofCountry(model);

            return RedirectToAction("PortCountryIndex");
        }

        
        [HttpGet]
        public ActionResult DeletePortOfCountryItem(int Id)
        {
            var obj = _service.DeletePortOfCountry(Id);
            return RedirectToAction("PortCountryIndex");
        }



        
        [HttpGet]
        public ActionResult DeleteCurrencyItem(int Id)
        {
            var obj = _service.DeleteCurrency(Id);
            return RedirectToAction("CurrencyIndex");
        }

        
        [HttpGet]
        public async Task<ActionResult> PortCountryIndex()
        {
            PortOfCountryVm model = new PortOfCountryVm();
            model = await _service.GetPortOfCountry();
            return View(model);
        }

        
        [HttpGet]
        public async Task<ActionResult> AddOrEditPortCountry(int? Id)
        {
            PortOfCountryVm model = new PortOfCountryVm();
            if (Id != null)
            {
                model = await _service.GetPortCountryForUpdate(Id);
                return View(model);
            }
            model =await  _service.GetCountry();

            return View(model);
        }

        //noticeboard
        
        [HttpGet]
        public async Task<ActionResult> NoticeBoard()
        {
            
          
            return View();
           
        }
        
        [HttpGet]
        public async Task<ActionResult> ErpWorkingUpDate()
        {


            return View();

        }
        
        [HttpPost]
        public async Task<ActionResult> ErpWorkingUpDate(FileViewModel data)
        {
            List<FileItem> itemlist = new List<FileItem>();
            for (int i = 0; i < data.file.Count; i++)
            {
                itemlist.Add(new FileItem
                {
                    file = data.file[i],
                    docdesc = data.fileTitle,
                    docfilename = data.file[i].FileName,
                    docid = 0,
                    FileCatagoryId = 11,
                    fileext = Path.GetExtension(data.file[i].FileName),
                    isactive = true,
                    RecDate = data.Recdate,
                    SortOrder = i,
                    userid = 12
                });
            }
            var x = await _ftpservice.UploadFileBulk(itemlist);
            return RedirectToAction(nameof(ERPNoticeBoardList));
        }

        
        [HttpPost]
        public async Task<ActionResult> NoticeBoard(FileViewModel data)
        {
            List<FileItem> itemlist = new List<FileItem>();
            for (int i = 0; i < data.file.Count; i++)
            {
                itemlist.Add(new FileItem
                {
                    file = data.file[i],
                    docdesc = data.fileTitle,
                    docfilename = data.file[i].FileName,
                    docid = 0,
                    FileCatagoryId = 10,
                    fileext = Path.GetExtension(data.file[i].FileName),
                    isactive = true,
                    RecDate = data.Recdate,
                    SortOrder = i,
                    userid = 12
                });
            }
            var x = await _ftpservice.UploadFileBulk(itemlist);
            return RedirectToAction(nameof(NoticeBoardList));
        }

        
        [HttpGet]
        public async Task<ActionResult> NoticeBoardList(DateTime? StartDate,DateTime?ToDate,string selectTitle)
        {
            

            var list = await _service.GetAllFilesByCatagory(StartDate,ToDate, selectTitle);
            return View(list);

        }


        
        [HttpGet]
        public async Task<ActionResult> NoticeBoardListNormal(DateTime? StartDate, DateTime? ToDate, string selectTitle)
        {


            var list = await _service.GetAllFilesByCatagory(StartDate, ToDate, selectTitle);
            return View(list);

        }








        public async Task<ActionResult> DeleteFTP(int docid) // its working and called
        {
            try
            {
                await _service.DeleteFile(docid);
                return RedirectToAction(nameof(NoticeBoardList));
            }
            catch (Exception ex)
            {
                return Json("No Such File", JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpGet]
        public async Task<ActionResult> ERPNoticeBoardList(DateTime? StartDate, DateTime? ToDate)
        {


            var list = await _service.GetAllFilesByCatagoryErp(StartDate, ToDate);
            return View(list);

        }
        public ActionResult GetLotNumberM(int productid)
        {

            var lot = _service.GetLotNumber(productid);

            return Json(lot, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetLotNumberF(int productid)
        {

            var lot = _service.GetLotNumberFinish(productid);

            return Json(lot, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public async Task<JsonResult> DeleteUpazila(int id)
        {

            var obj = await _service.DeleteUpazila(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLotNymberFinish(int ProductId)
        {

            var products = _service.GetAutoCompleteLotFinish(ProductId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<JsonResult> GetUpazilaById(int id)
        {
            var obj = await _service.GetUpazilaById(id);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        #region Common Finish Product Category

        public async Task<ActionResult> Incentives(int companyId)
        {

            VMIncentive vmIncentive = new VMIncentive();
            vmIncentive = await Task.Run(() => _service.GetIncentives(companyId));
            return View(vmIncentive);
        }
        [HttpPost]
        public async Task<ActionResult> Incentives(VMIncentive vmIncentive)
        {

            if (vmIncentive.ActionEum == ActionEnum.Add)
            {
                //Add 

                await _service.IncentiveAdd(vmIncentive);
            }
            else if (vmIncentive.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.IncentiveEdit(vmIncentive);
            }
            else if (vmIncentive.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.IncentiveDelete(vmIncentive.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(Incentives), new { companyId = vmIncentive.CompanyId });
        }


        











        #endregion

        #region Common Finish Product SubCategory


        public async Task<ActionResult> IncentiveDetails(int companyId, int incentiveId = 0)
        {

            VMIncentiveDetails vmIncentiveDetails = new VMIncentiveDetails();
            vmIncentiveDetails = await Task.Run(() => _service.GetIncentiveDetails(companyId, incentiveId));
            return View(vmIncentiveDetails);
        }
        [HttpPost]
        public async Task<ActionResult> IncentiveDetails(VMIncentiveDetails vmIncentiveDetails)
        {

            if (vmIncentiveDetails.ActionEum == ActionEnum.Add)
            {
                //Add                

                await _service.IncentiveDetailsAdd(vmIncentiveDetails);
            }
            else if (vmIncentiveDetails.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.IncentiveDetailsEdit(vmIncentiveDetails);
            }
            else if (vmIncentiveDetails.ActionEum == ActionEnum.Delete)
            {
                await _service.IncentiveDetailsDelete(vmIncentiveDetails.ID);
            }

            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(IncentiveDetails), new { companyId = vmIncentiveDetails.CompanyId, incentiveId = vmIncentiveDetails.IncentiveId });
        }


        #endregion





        //Azim --Requisition Product,Category and Subcategory add

        #region Common General Product Category

        public async Task<ActionResult> CommonGeneralProductCategory(int companyId)
        {

            VMCommonProductCategory vmCommonProductCategory = new VMCommonProductCategory();
            vmCommonProductCategory = await Task.Run(() => _service.GetFinishProductCategory(companyId, "G"));
            return View(vmCommonProductCategory);
        }
        [HttpPost]
        public async Task<ActionResult> CommonGeneralProductCategory(VMCommonProductCategory vmCommonProductCategory)
        {

            if (vmCommonProductCategory.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProductCategory.ProductType = "G";

                await _service.ProductFinishCategoryAdd(vmCommonProductCategory);
            }
            else if (vmCommonProductCategory.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductFinishCategoryEdit(vmCommonProductCategory);
            }
            else if (vmCommonProductCategory.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductFinishCategoryDelete(vmCommonProductCategory.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonGeneralProductCategory), new { companyId = vmCommonProductCategory.CompanyFK });
        }


        #endregion

        #region Common General Product SubCategory


        public async Task<ActionResult> CommonGeneralProductSubCategory(int companyId, int categoryId = 0)
        {

            VMCommonProductSubCategory vmCommonProductSubCategory = new VMCommonProductSubCategory();
            vmCommonProductSubCategory = await Task.Run(() => _service.GetProductSubCategory(companyId, categoryId, "G"));
            return View(vmCommonProductSubCategory);
        }
        [HttpPost]
        public async Task<ActionResult> CommonGeneralProductSubCategory(VMCommonProductSubCategory vmCommonProductSubCategory)
        {

            if (vmCommonProductSubCategory.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProductSubCategory.ProductType = "G";

                await _service.ProductSubCategoryAdd(vmCommonProductSubCategory);
            }
            else if (vmCommonProductSubCategory.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductSubCategoryEdit(vmCommonProductSubCategory);
            }
            else if (vmCommonProductSubCategory.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductSubCategoryDelete(vmCommonProductSubCategory.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonGeneralProductSubCategory), new { companyId = vmCommonProductSubCategory.CompanyFK, categoryId = vmCommonProductSubCategory.Common_ProductCategoryFk });
        }


        #endregion

        #region Common Raw Product
        public async Task<ActionResult> CommonGeneralProduct(int companyId, int categoryId = 0, int subCategoryId = 0)
        {

            VMCommonProduct vmCommonProduct = new VMCommonProduct();
            vmCommonProduct = await _service.GetProduct(companyId, categoryId, subCategoryId, "G");
            vmCommonProduct.ProductSubCategoryList= new SelectList(_service.ProductSubCategoryDropDownList(companyId,categoryId), "Value", "Text");
            vmCommonProduct.UnitList = new SelectList(_service.UnitDropDownList(companyId), "Value", "Text");
            return View(vmCommonProduct);
        }

        [HttpPost]
        public async Task<ActionResult> CommonGeneralProduct(VMCommonProduct vmCommonProduct)
        {

            //if (vmCommonProduct.ImageFile != null)
            //{
            //    vmCommonProduct.Image = _service.UploadFile(vmCommonProduct.ImageFile, "Product", _webHostEnvironment.WebRootPath);
            //}
            if (vmCommonProduct.ActionEum == ActionEnum.Add)
            {
                //Add 
                vmCommonProduct.ProductType = "G";

                await _service.ProductAdd(vmCommonProduct);
            }
            else if (vmCommonProduct.ActionEum == ActionEnum.Edit)
            {
                //Edit
                await _service.ProductEdit(vmCommonProduct);
            }
            else if (vmCommonProduct.ActionEum == ActionEnum.Delete)
            {
                //Delete
                await _service.ProductDelete(vmCommonProduct.ID);
            }
            else
            {
                return RedirectToAction("Error");
            }
            return RedirectToAction(nameof(CommonGeneralProduct), new { companyId = vmCommonProduct.CompanyFK, categoryId = vmCommonProduct.Common_ProductCategoryFk, subCategoryId = vmCommonProduct.Common_ProductSubCategoryFk });
        }




        #endregion
    }
}