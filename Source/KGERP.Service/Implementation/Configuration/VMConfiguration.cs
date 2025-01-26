using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.RealState;
using KGERP.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Service.Configuration
{
    public abstract class BaseVM
    {

        public int ID { get; set; }
        public string UserId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? AuthorizedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorizedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ActionEnum ActionEum { get { return (ActionEnum)this.ActionId; } }
        public int ActionId { get; set; } = 1;
        public JournalEnum JournalEnum { get { return (JournalEnum)this.JournalType; } }
        public int JournalType { get; set; } = (int)JournalEnum.JournalVoucer;
        public bool IsActive { get; set; } = true;
        public bool IsExisting { get; set; }
        public int? CompanyFK { get; set; }
        public int OrderNo { get; set; }
        public string Remarks { get; set; }
       
        public string Code { get; set; }
        public string CompanyName { get; set; }
        //public string Message { get; set; }
        //public bool HasError { get; set; }

    }
    public class VMCompany : BaseVM
    {
        public int CompanyId { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int OrderNo { get; set; }
        public int? CompanyType { get; set; }
        public string MushokNo { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public int? LayerNo { get; set; }
        public string CompanyLogo { get; set; }
        public bool IsCompany { get; set; }
        public IEnumerable<VMCompany> DataList { get; set; }
        public HttpPostedFileBase CompanyLogoUpload { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Param { get; set; }

    }

    public class VMUserMenuAssignment : BaseVM
    {
        //public bool IsAllowed { get; set; }
        public string MenuName { get; set; }
        public int MenuID { get; set; }
        public string SubmenuName { get; set; }
        public int SubmenuID { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        public string Method { get; set; }
        public string CompanyName { get; set; }
        public long CompanyUserMenusId { get; set; }
        public SelectList CompanyList { get; set; } = new SelectList(new List<object>());
        public IEnumerable<VMUserMenuAssignment> DataList { get; set; }
        public int MenuPriority { get; set; }
    }

    public class VMUserMenu : BaseVM
    {
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public IEnumerable<VMUserMenu> DataList { get; set; }
        public int? LayerNo { get; set; }
        public string ShortName { get; set; }
        public SelectList CompanyList { get; set; } = new SelectList(new List<object>());

    }
    public class UrlViewModel
    {
        public UrlInfo UrlInfo { get; set; }
        public List<UrlInfo> Urls { get; set; }
        public SelectList CompanyList { get; set; } = new SelectList(new List<object>());
        public string CompanyName { get; set; }
        public int UrlId { get; set; }
        public string Url { get; set; }
        public int? UrlType { get; set; }
        public int CompanyId { get; set; }
        public List<UrlViewModel> DataList { get; set; }
    }

    public class VMGrade : BaseVM
    {

        public int GradeId { get; set; }
        public string Name { get; set; }
        public string PayScale { get; set; }
        public string GradeCode { get; set; }
        public IEnumerable<VMGrade> DataList { get; set; }

    }

    public class VMDepartment : BaseVM
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public IEnumerable<VMDepartment> DataList { get; set; }
    }
    public class VMUserSubMenu : BaseVM
    {

        public string Name { get; set; }
        public string UserMenuName { get; set; }
        public int User_MenuFk { get; set; }
        public int Priority { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int? LayerNo { get; set; }
        public string ShortName { get; set; }
        public string Param { get; set; }
        public SelectList UserMenuList { get; set; } = new SelectList(new List<object>());
        public SelectList CompanyList { get; set; } = new SelectList(new List<object>());

        public string DisplayMessage { get; set; }

        public IEnumerable<VMUserSubMenu> DataList { get; set; }

    }
    public class VMPOTremsAndConditions : BaseVM
    {
        public string Key { get; set; }
        [AllowHtml]
        public string Value { get; set; }
        public IEnumerable<VMPOTremsAndConditions> DataList { get; set; }
    }
    public class VMCommonZone : BaseVM
    {
        public string Name { get; set; }
        public string SalesOfficerName { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string MobileOffice { get; set; }
        public string MobilePersonal { get; set; }
        public string ZoneIncharge { get; set; }
        public long? ZoneInchargeId { get; set; }
        public IEnumerable<VMCommonZone> DataList { get; set; }
    }

    public class VMCommonSubZone : BaseVM
    {
        public string Name { get; set; }
        public string SalesOfficerName { get; set; }
        public long? SalesOfficerId { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string MobileOffice { get; set; }
        public string MobilePersonal { get; set; }
        public string ZoneName { get; set; }
        public SelectList ZoneList { get; set; } = new SelectList(new List<object>());

        public int ZoneId { get; set; }
        public IEnumerable<VMCommonSubZone> DataList { get; set; }


    }
    public class VMCommonSize : BaseVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsLock { get; set; }
        public IEnumerable<VMCommonSize> DataList { get; set; }


    }

    public class VMCommonUnit : BaseVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsLock { get; set; }

        public IEnumerable<VMCommonUnit> DataList { get; set; }
    }

    public class NOticeBoardViewModel
    {
        public List<HttpPostedFileBase> Attachments { get; set; }
        public string title { get; set; }
        public DateTime Createdate { get; set; }
        public List<GLDLBookingAttachment> Attachments2 { get; set; }
    }

    public class VMCommonSupplier : BaseVM
    {
        public bool IsPoultry { get; set; }
        public bool IsFish { get; set; }
        public bool IsCattle { get; set; }
        public decimal CashCommissionPoultry { get; set; }
        public decimal CashCommissionFish { get; set; }
        public decimal CashCommissionCattle { get; set; }
        public string GuarantorName { get; set; }
        public string GurantorAddress { get; set; }
        public string GurantorMobileNo { get; set; }
        public decimal YearlyIncentive { get; set; }
        public List<MonthSelectModel> Months { get; set; }
        public string Name { get; set; }
        public int VendorReferenceId { get; set; }
        public int? vendorProfessionId { get; set; }
        public  Nullable<System.DateTime> DateOfBirth { get; set; }
        public  Nullable<System.DateTime> MarriageDate { get; set; }
        public int ZoneId { get; set; }
        public long SalesOfficerEmpId { get; set; }
        [Required]
        [DisplayName("Sales Officer")]
        public string SalesOfficerEmpName { get; set; }
        public int SubZoneId { get; set; }
        public HttpPostedFileBase file { get; set; }
        public string ImageFileUrl { get; set; }
        public long ImageDocId { get; set; }
        public int Common_UpazilasFk { get; set; }
        public int Common_DistrictsFk { get; set; }
        public int Common_DivisionFk { get; set; }
        public int Common_CountriesFk { get; set; }
        public List<GLDLBookingAttachment> Attachments { get; set; }
        public string Upazila { get; set; }
        public string District { get; set; }
        public string Division { get; set; }
        public string Country { get; set; }
        public string Fax { get; set; }
        public int CustomerTypeFk { get; set; }
        public int VendorTypeId { get; set; }
        public bool BiltoBilCreditPeriod { get; set; }
        public int CreditPeriod { get; set; }
        public decimal SecurityAmount { get; set; }
        public decimal FixedIncentive { get; set; }
        public bool IsIncentiveInInvoice { get; set; }
        public decimal DepotCarrying { get; set; }
        public decimal FactoryCarrying { get; set; }
        public int? CustomerStatus { get; set; } = 1;
        public string Propietor { get; set; }
        public string PaymentType { get; set; }
        public string NomineeName { get; set; }
        public string NomineePhone { get; set; }
        public string BusinessAddress { get; set; }
        public string NomineeNID { get; set; }
        public string NomineeRelation { get; set; }
        public string EmployeeName { get; set; }
        public decimal? FixedCommissionPoultry { get; set; }
        public decimal? FixedCommissionFish { get; set; }
        public decimal? FixedCommissionCattle { get; set; }


        public SelectList UpazilasList { get; set; } = new SelectList(new List<object>());
        public SelectList EmployeeList { get; set; } = new SelectList(new List<object>());
        public SelectList ZoneListList { get; set; } = new SelectList(new List<object>());
        public SelectList CountryList { get; set; } = new SelectList(new List<object>());
        public SelectList DistrictList { get; set; } = new SelectList(new List<object>());
        public SelectList DivisionList { get; set; } = new SelectList(new List<object>());
        public SelectList TerritoryList { get; set; } = new SelectList(new List<object>());
        public SelectList PaymentTypeList { get; set; } = new SelectList(new List<object>());
        public SelectList NomineeRelationList { get; set; } = new SelectList(new List<object>());
        public SelectList vendorProfession { get; set; } = new SelectList(new List<object>());
        public SelectList Checkdetail { get; set; } = new SelectList(new List<object>());
        public SelectList CheckType { get; set; } = new SelectList(new List<object>());



        public CustomerType CustomerType { get { return (CustomerType)this.CustomerTypeFk; } }// = SupplierPaymentMethodEnum.Cash;
        public string CustomerTypeName { get { return BaseFunctionalities.GetEnumDescription(CustomerType); } }
        public SelectList CustomerTypeList { get { return new SelectList(BaseFunctionalities.GetEnumList<CustomerType>(), "Value", "Text"); } }

        public KFMALCustomerType KFMALCustomerType { get { return (KFMALCustomerType)this.CustomerProductTypeFk; } }// = SupplierPaymentMethodEnum.Cash;
        public string KFMALCustomerTypeName { get { return BaseFunctionalities.GetEnumDescription(KFMALCustomerType); } }
        public SelectList KFMALCustomerTypeList { get { return new SelectList(BaseFunctionalities.GetEnumList<KFMALCustomerType>(), "Value", "Text"); } }

        public CustomerStatusEnum CustomerStatusEnum { get { return (CustomerStatusEnum)this.CustomerStatus; } }
        public string CustomerStatusName { get { return BaseFunctionalities.GetEnumDescription(CustomerStatusEnum); } }
        public SelectList CustomerStatusEnumList { get { return new SelectList(BaseFunctionalities.GetEnumList<CustomerStatusEnum>(), "Value", "Text"); } }
        public string ContactPerson { get; set; }
        [Required]
        [RegularExpression("^[0-9]{11}$",ErrorMessage = "Phone Number Must be 11 digit")]
       
        public string Phone { get; set; }
        
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsForeign { get; set; }
        public string ZoneName { get; set; }
        public string ZoneIncharge { get; set; }
        public string NID { get; set; }
        public decimal? CreditLimit { get; set; }

        public string ACName { get; set; }
        public string ACNo { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public IEnumerable<VMCommonSupplier> DataList { get; set; }
        public List<SelectModel> CList { get; set; }
        public List<object> HeadList { get; set; }
        public long? HeadGLId { get; set; }
        public int HId { get; set; }
        public long CGId { get; set; }
        public string FileNo { get;  set; }
        public string BookingNo { get;  set; }
        public string ProductName { get;  set; }
        public decimal MonthlyIncentive { get; set; }
        public string Condition { get; set; }
        public decimal? YearlyTarget { get; set; } = 0;
        public int CreditRatioFrom { get;  set; }
        public decimal? MonthlyTarget { get; set; } = 0;
        public int CreditRatioTo { get;  set; }
        public int? NoOfCheck { get;  set; }
        public string CheckNo { get;  set; }
        public string ClosingTime { get;  set; }
        public decimal? CreditCommission { get; set; }
        public int VendorDeedId { get; set; }
        public string ETinNo { get;  set; }
        public string TradeLicense { get;  set; }
        public int CustomerProductTypeFk { get;  set; }
        public int CheckTypeId { get; set; }
        public int CheckDetailId { get; set; }
        public string NidImage { get; set; }
        public string BSAMemUrl { get; set; }
        public string SaleLiUrl { get; set; }
        public string DelerLiUrl { get; set; }
        public string TinUrl { get; set; }
        public string BankChkUrl { get; set; }
        public string TradeLicenceUrl { get; set; }
 public string Imageurl { get; set; }

        public List<string> UploadedFileUrls { get; set; }
        public string SubZoneName { get; set; }
    }
    public class VMCommonSupplierProduct : VMCommonProduct
    {
        public string SupplierName { get; set; }
        public int Common_SupplierFk { get; set; }

        public string Thana { get; set; }
        public string District { get; set; }
        public string Division { get; set; }
        public string Country { get; set; }
        public string Fax { get; set; }
        public bool BiltoBilCreditPeriod { get; set; }
        public int CreditPeriod { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        // public List<string> Common_ProductListFK { get; set; }

        // public MultiSelectList MultiProductList { get; set; } = new MultiSelectList(new List<object>());
        public IEnumerable<VMCommonSupplierProduct> DataListSupplierProduct { get; set; }

    }

    public class VMCommonSupplierContact : VMCommonSupplier
    {
        public int Common_SupplierFk { get; set; }
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public string ContactName { get; set; }
        public string Designations { get; set; }

        public IEnumerable<VMCommonSupplierContact> SupplierContactDataList { get; set; }

    }
    public class VMCommonProductCategory : BaseVM
    {
        public string Name { get; set; }
        public decimal? CashCommission { get; set; }

        public string Description { get; set; }

        public string ProductType { get; set; }
      
        public bool IsCrm { get; set; }
        public int LandSizeInKatha { get; set; }
        public string NoOfFloors { get; set; }
        public string FlatSizeSpecification { get; set; }
        public int TotalParking { get; set; }
        public DateTime? HandOverDate { get; set; }
        public int TargetCostPerSqft { get; set; }
        public int TotalFlat { get; set; }

        public IEnumerable<VMCommonProductCategory> DataList { get; set; }
    }
    public class VMIncentive : BaseVM
    {

        public int IncentiveId { get; set; }
        public string IncentiveType { get; set; }
        public int CompanyId { get; set; }
        public DateTime? MDate { get; set; }
       
        public IEnumerable<VMIncentive> DataList { get; set; }
    }

    public class VMIncentiveDetails : VMIncentive
    {
        public int IncentiveDetailId { get; set; }       
        public decimal? MinQty { get; set; }
        public decimal? MaxQty { get; set; }
        public decimal? Rate { get; set; }      

        public IEnumerable<VMIncentiveDetails> DataListDetails { get; set; }
    }

    public class VMCommonProductSubCategory : BaseVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Common_ProductCategoryFk { get; set; }
        public string CategoryName { get; set; }
        public string ProductType { get; set; }
        public bool IsLock { get; set; }
        public decimal? BaseCommissionRate { get; set; }
        public List<SelectModelType> ProductCategoryList { get; set; } 
        public IEnumerable<VMCommonProductSubCategory> DataList { get; set; }
        public List<SelectVm> BrandList { get; set; }
        public int BrandId { get; set; }

    }

    public partial class VMFinishProductBOM : VMCommonProduct
    {
        public int FProductFK { get; set; }
        public string JobOrderNo { get; set; }
        public string ORDStyle { get; set; }

        public int CompanyId { get; set; }
        public int? StatusId { get; set; }
        public int RProductFK { get; set; }
        public int FCEnumId { get; set; }
        public string FCEnumName { get; set; }
        public decimal RequiredQuantity { get; set; }
        public double Qty { get; set; }
      
        public double FinishUnitPrice { get; set; }
        public double FinishTotalPrice { get; set; }


        public decimal RProcessLoss { get; set; }
        public string RawProductName { get; set; }
        public string FinishProductName { get; set; }
        public string SupplierName { get; set; }
        public long OrderDetailId { get; set; }
        public bool IsSubmitted { get; set; }


        public IEnumerable<VMFinishProductBOM> DataListProductBOM { get; set; }
        public SelectList RawProductNameList { get; set; } = new SelectList(new List<object>());
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string ContactPerson { get; set; }
        public long OrderMasterId { get; set; }
        public string OrderNo { get; set; }
        public int Status { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerPaymentMethodEnumFK { get; set; }
        public string ExpectedDeliveryDate { get; set; }
        public string CommonCustomerName { get; set; }
        public string CustomerTypeFk { get; set; }
        public string CustomerId { get; set; }
        public string CourierCharge { get; set; }
        public string FinalDestination { get; set; }
        public string CourierNo { get; set; }
        public int SupplierId { get; set; }
        public SelectList FCEnumSelectionList { get { return new SelectList(BaseFunctionalities.GetEnumList<FormulaCalculationEnum>(), "Value", "Text"); } }
        public string LotNumbers { get; set; }

    }

    public class VMRealStateProduct : VMCommonProduct
    {
        public int? FacingId { get; set; }
        public FlatProperties FlatProp { get; set; } = new FlatProperties();
        public PlotProperties PlotProp { get; set; }
        public List<SelectModelType> FacingDropDown { get; set; }
        public List<string> LotNumbers { get;  set; }
    }

    public class VMrealStateProductsForList
    {
        public int CompanyId { get; set; }
        public string ProductType { get; set; }
        public string CompanyName { get; set; }
        public int ID { get; set; }
        public int ProductCategory { get; set; }
        public int SubProductCategory { get; set; }
        public ActionEnum ActionEum { get { return (ActionEnum)this.ActionId; } }
        public int ActionId { get; set; } = 1;
        public List<realStateProducts> DataList { get; set; } = new List<realStateProducts>();
        public realStateProducts realStateProducts { get; set; } = new realStateProducts();
        public List<SelectModelType> GetList { get; set; }
        public List<SelectModel> ProductCategoryList { get; set; }
        public SelectList ProductSubCategoryList { get; set; } = new SelectList(new List<object>());
    }
    public class realStateProducts
    {
        public int ID { get; set; }
        public string ProductType { get; set; }
        public string Code { get; set; }

        public int? FacingId { get; set; }
        public string FacingTitle { get; set; }
        public string LandFacingTitle { get; set; }
        public FlatProperties FlatProp
        {
            get
            {
                var d = JsonConvert.DeserializeObject<FlatProperties>(this.JsonData);
                if (d == null)
                {
                    return new FlatProperties();
                }
                else
                {
                    return d;
                }
            }
        }
        public PlotProperties PlotProp
        {
            get
            {
                var d = JsonConvert.DeserializeObject<PlotProperties>(this.JsonData);
                if (d == null)
                {
                    return new PlotProperties();
                }
                else
                {
                    return d;
                }
            }
        }
        public int Common_ProductInBinSlaveFk { get; set; }
        public string Name { get; set; }
        public double? PackSize { get; set; }
        public string ShortName { get; set; }
        public string Remarks { get; set; }
        public int? Common_ProductCategoryFk { get; set; }
        public int? Common_ProductSubCategoryFk { get; set; }
        public int? Common_ProductFk { get; set; }
        public int WareHouse_POReceivingFk { get; set; }
        public int WareHouse_POReceivingSlaveFk { get; set; }
        public int? Common_UnitFk { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime DiscountExpiryDate { get; set; }
        public decimal ProcessLoss { get; set; }
        public decimal CostingPrice { get; set; }
        public string UnitName { get; set; }
        public decimal MRPPrice { get; set; }
        public decimal TPPrice { get; set; }
        [Required]
        [Range(1, 99999999999999, ErrorMessage = "Unit Price is Required!! not allow zero or nagetive .")]
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PreviousStock { get; set; }
        public decimal CurrentStock { get; set; }
        public string Image { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public string SubCategoryName { get; set; }
        public string CategoryName { get; set; }
        public int Status { get; set; }
        public bool IsLock { get; set; }
        public string JsonData { get; set; }
        public List<SelectModelType> GetList { get; set; }
    }
    public class VMCommonProduct : BaseVM
    {
        [Display(Name = "Code")]
        public string ProductCode { get; set; }
        public int Common_ProductInBinSlaveFk { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? Common_ProductCategoryFk { get; set; }
        public int? Common_ProductSubCategoryFk { get; set; }
        public int? Common_ProductFk { get; set; }
        public int? ErrorStatus { get; set; }

        public int WareHouse_POReceivingFk { get; set; }
        public int WareHouse_POReceivingSlaveFk { get; set; }
        public int? Common_UnitFk { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime DiscountExpiryDate { get; set; }
        public decimal ProcessLoss { get; set; }
        public decimal CostingPrice { get; set; }
        public Nullable<int> PackId { get; set; }
        public Nullable<decimal> DieSize { get; set; }
        public Nullable<double> PackSize { get; set; }
        public string PackName { get; set; }
        public string UnitName { get; set; }
        public decimal MRPPrice { get; set; }
        public decimal TPPrice { get; set; }
        public string HcCode { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string HorsePower { get; set; }
        public string FuelPumpSlNo { get; set; }
        public string BatteryNo { get; set; }
        public string ReanType { get; set; }
        public Nullable<int> NoOfCylinder { get; set; }
        public string EngineNo { get; set; }
        public string ChassisNo { get; set; }

        [Required]
        [Range(1, 99999999999999, ErrorMessage = "Unit Price is Required!! not allow zero or nagetive .")]
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PreviousStock { get; set; }
        public decimal CurrentStock { get; set; }

        public string Image { get; set; }

        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }

        //public IFormFile ImageFile { get; set; }

        public string SubCategoryName { get; set; }
        public string CategoryName { get; set; }

        public bool IsLock { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal? CreditSalePrice { get; set; }
        public decimal DamageQuantity { get; set; }
         
        public double DeliveredQty { get; set; }
        public decimal RawConsumeQuantity { get; set; }
        public string Description { get; set; }


        public decimal LaborCost { get; set; }
        public decimal ManufacturingOverhead { get; set; }
        public decimal TransportationOverhead { get; set; }
        public decimal OthersCost { get; set; }
        public string OthersCostNote { get; set; }
        public decimal RemainingStockInQty { get; set; }
        public IEnumerable<VMCommonProduct> DataList { get; set; }
        public List<SelectModelType> GetProductCategoryList { get; set; }
        public SelectList ProductCategoryList { get; set; } = new SelectList(new List<object>());
        public SelectList ProductList { get; set; } = new SelectList(new List<object>());
        public SelectList WareHousePOReceivingList { get; set; } = new SelectList(new List<object>());


        public SelectList ProductSubCategoryList { get; set; } = new SelectList(new List<object>());
        public SelectList UnitList { get; set; } = new SelectList(new List<object>());
        public SelectList StatusList { get; set; } = new SelectList(new List<object>());

        public string ProductType { get; set; }
        public int Status { get; set; }
        public decimal? ReturnQuntity { get; set; }

        public decimal? FormulaQty { get; set; }
    }


    public class VMCommonCustomer : BaseVM
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string MemberShipNo { get; set; }

        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime DOB { get; set; }
        public string Sex { get; set; }

        public string CustomerLoyalityPoint { get; set; }
        public string CustomerTypeName { get; set; }
        public int CustomerTypeEnumFk { get; set; }

        //public int Common_ThanaFk { get; set; }
        //public int Common_CountriesFk { get; set; }
        //public int Common_DistrictsFk { get; set; }
        //public int Common_DivisionFk { get; set; }

        //public string Division { get; set; }
        //public string Thana { get; set; }
        //public string District { get; set; }
        //public string Country { get; set; }

        //public SelectList CountryList { get; set; } = new SelectList(new List<object>());
        //public SelectList DivisionList { get; set; } = new SelectList(new List<object>());
        //public SelectList DistrictList { get; set; } = new SelectList(new List<object>());
        //public SelectList ThanaList { get; set; } = new SelectList(new List<object>());

        public IEnumerable<VMCommonCustomer> DataList { get; set; }


    }

    public class VMCommonCustomerContact : VMCommonCustomer
    {
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public string ContactName { get; set; }
        public int Common_CustomerFk { get; set; }
        public IEnumerable<VMCommonCustomerContact> CustomerContactDataList { get; set; }

    }
    public class VMCommonShop : BaseVM
    {
        public string Name { get; set; }



        public int Common_ThanaFk { get; set; }

        public int Common_CountriesFk { get; set; }
        public int Common_DistrictsFk { get; set; }
        public int Common_DivisionFk { get; set; }
        public int ShopTypeEnumFk { get; set; }



        public string Thana { get; set; }
        public string Division { get; set; }
        public string District { get; set; }
        public string Country { get; set; }


        public string Address { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public bool OwnDeliveryService { get; set; }
        public string ServiceStartTime { get; set; }
        public string ServiceEndTime { get; set; }
        public string Description { get; set; }

        public SelectList ThanaList { get; set; } = new SelectList(new List<object>());
        public SelectList DistrictList { get; set; } = new SelectList(new List<object>());
        public SelectList DivisionList { get; set; } = new SelectList(new List<object>());
        public SelectList CountryList { get; set; } = new SelectList(new List<object>());

        //public ShopTypeEnum STypeEnum { get { return (ShopTypeEnum)this.ShopTypeEnumFk; } }
        //public string ShopTypeEnumName { get { return BaseFunctionalities.GetEnumDescription(STypeEnum); } }
        //public SelectList ShopTypeList { get { return new SelectList(BaseFunctionalities.GetEnumList<ShopTypeEnum>(), "Value", "Text"); } }


        public string TradeLicenceNumber { get; set; }
        public DateTime TradeLicenceExpireDate { get; set; }
        public string TradeLicenceUrl { get; set; }

        public IEnumerable<VMCommonShop> DataList { get; set; }
    }
    public class VMCommonShopOwner : VMCommonShop
    {
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public string ContactName { get; set; }
        public IEnumerable<VMCommonShopOwner> CommonShopOwnerDataList { get; set; }
    }
    public class VMCommonShopDeliveryService : VMCommonShop
    {
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public string ContactName { get; set; }
        public IEnumerable<VMCommonShopDeliveryService> ShopDeliveryServiceDataList { get; set; }
    }
    public class VMCommonShopCounter : BaseVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public SelectList ShopList { get; set; } = new SelectList(new List<object>());
        public string ShopName { get; set; }
        public IEnumerable<VMCommonShopCounter> DataList { get; set; }
        public VMCommonShop VMCommonShop { get; set; }

    }
    public class VMCommonBin : BaseVM
    {
        public string UnitName { get; set; }
        public string ShopName { get; set; }
        public int RackNo { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public IEnumerable<VMCommonBin> DataList { get; set; }
        public SelectList ShopList { get; set; } = new SelectList(new List<object>());

    }
    public class VMCommonBinSlave : VMCommonBin
    {

        public int Common_BinFk { get; set; }
        public string CID { get; set; }
        public string Dimension { get; set; }
        public List<string> ProductNameList { get; set; }
        public string ProductName { get; set; }
        public VMCommonProduct VMCommonProduct { get; set; }
        public int Common_ProductInBinSlaveFk { get; set; }
        public int Common_ProductFK { get; set; }
        public IEnumerable<VMCommonBinSlave> DataListSlave { get; set; }
        public List<VMCommonBinSlave> DataSlaveToList { get; set; }
        public MultiSelectList ProductList { get; set; } = new MultiSelectList(new List<object>());

    }
    public class VMCommonCountries : BaseVM
    {
        public string Name { get; set; }
        public string BnName { get; set; }
        public string Url { get; set; }

    }
    public class VMCommonDivisions : BaseVM
    {
        public string Name { get; set; }
        public string BnName { get; set; }
        public string Url { get; set; }

        public int Common_CountriesFk { get; set; }
        public IEnumerable<VMCommonDivisions> DataList { get; set; }


    }
    public class VMCommonDistricts : BaseVM
    {
        public string Name { get; set; }
        public string BnName { get; set; }
        public string Url { get; set; }
        public int Common_DivisionsFk { get; set; }
        public string DivisionsName { get; set; }

        public string ShorName { get; set; }
        public IEnumerable<VMCommonDistricts> DataList { get; set; }
        public SelectList DivisionList { get; set; } = new SelectList(new List<object>());


    }

   

    public class VMCommonThana : BaseVM
    {
        public string Name { get; set; }
        public string BnName { get; set; }
        public string Url { get; set; }
        public int Common_DistrictsFk { get; set; }
        public string DistictName { get; set; }
        public int Common_DivisionsFk { get; set; }
        public string DivisionsName { get; set; }
        public string ShorName { get; set; }
        public IEnumerable<VMCommonThana> DataList { get; set; }

        public List<District> DistrictList { get; set; }

    }
    public partial class VMAccountingSignatory : BaseVM
    {

        public int SignatoryId { get; set; }
        public string SignatoryName { get; set; }
        public int EmployeeId { get; set; }
        public string SignatoryType { get; set; }
        public int OrderBy { get; set; }
        public int Priority { get; set; }
        public IEnumerable<VMAccountingSignatory> DataList { get; set; }
        public SelectList CompanyList { get; set; } = new SelectList(new List<object>());
        public List<SelectListItem> DDLEmployee { get; set; }
    }

    public partial class VMPackagingPurchaseRequisition : BaseVM
    {
        public decimal TotalQty { get; set; }
        public long IssueMasterId { get; set; }
        public decimal? PriviousIssueQty { get; set; }
        public decimal? RemainingQuantity { get; set; }
        public long IssueDetailsId { get; set; }
        public decimal? IssueQty { get; set; }
        public decimal CostingPrice { get; set; }
        public decimal CurrenntStock { get; set; }
        public string OrderNo { get; set; }
        public string RequisitionBy { get; set; }
        public string IssueNo { get; set; }
        public int ProductId { get; set; }
        public int? AccountingHeadId { get; set; }
        public long IssueById { get; set; }
        public string ProductName { get; set; }
        public long OrderMasterId { get; set; }
        public int OrderDetailsId { get; set; }
        public int RequisitionId { get; set; }
        public string RequisitionNo { get; set; }
        [Display(Name = "Requisition Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? RequisitionDate { get; set; }
        public string RequisitionDateMsg { get { return RequisitionDate.Value.ToString("dd-MMM-yyyy"); } }
        public string Description { get; set; }
        public string RequisitionStatus { get; set; }
        public Guid? RequistionItemDetailId { get; set; }
        public int CompanyId { get; set; }
        public int RequisitionType { get; set; }
        public string CustomerId { get; set; }
      
        [Required]
        public int FromDepartmentReqId { get; set; }
        public int FromDepartmentIssueId { get; set; }
        public string FromDepartmentIssueName { get; set; }
        [Required]
        public int ToDepartmentReqId { get; set; }
        
        public int ToDepartmentIssueId { get; set; }
        public string ToDepartmentIssueName { get; set; }
        //DetailsItem
        public int RequisitionItemId { get; set; }
        public decimal? Qty { get; set; }
        public decimal? AllocatedQuantity { get; set; }
        public string RequisitionItemStatus { get; set; }
        public string IssueBy { get; set; }
        [Display(Name = "Issue Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime IssueDate { get; set; }
        public string RawProductName { get; set; }
        public SelectList PoList { get; set; } = new SelectList(new List<object>());
        public IEnumerable<VMPackagingPurchaseRequisition> DataList { get; set; }
        public List<VMPackagingPurchaseRequisition> DataListPro { get; set; }

        public List<SelectListItem> DDLStockDepartmetn { get; set; }

        public List<SelectListItem> DDLStockDepartment { get; set; }
        public bool Achknolagement { get;  set; }
        public string AchknolagementIs { get;  set; }
        public string AchknologeByName { get;  set; }
        public DateTime? AcknologeDate { get;  set; }
        public string AcknologeDateMsg { get;  set; }
        public string AchknologeBy { get;  set; }
        public long AchknologeById { get;  set; }
        public string CustomerName { get; set; }
        public string IntregrationFrom { get; set; }
        public string JobOrderNo { get; set; }
        public bool IsSubmited { get;  set; }
        public string FromRequisitionName { get;  set; }
        public string ToRequisitionName { get;  set; }
        public DateTime OrderDate { get;  set; }
        public DateTime? ExpectedDeliveryDate { get;  set; }
        public string StockName { get;  set; }
        public string ProductNames { get;  set; }
        public decimal AllocatedQty { get;  set; }
        public decimal? RQty { get;  set; }
        public int RProductId { get;  set; }
    }



    public partial class VMPackagingProductionRequisitions : BaseVM
    {
        public decimal TotalQty { get; set; }
        public long IssueMasterId { get; set; }
        public decimal? PriviousIssueQty { get; set; }
        public decimal? RemainingQuantity { get; set; }
        public long IssueDetailsId { get; set; }
        public decimal IssueQty { get; set; }
        public string OrderNo { get; set; }
        public string JobOrderNo { get; set; }
        public bool IsSubmitted { get; set; }
        public string FromRequisitionName { get; set; }
        public string ToRequisitionName { get; set; }
        public string IssueNo { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public long OrderMasterId { get; set; }
        public int OrderDetailsId { get; set; }
        public int RequisitionId { get; set; }
        public string RequisitionNo { get; set; }
        [Display(Name = "Requisition Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? RequisitionDate { get; set; }
        public string RequisitionDateMsg { get { return RequisitionDate.Value.ToString("dd-MMM-yyyy"); } }
        public string Description { get; set; }
        public string RequisitionStatus { get; set; }
        public int CompanyId { get; set; }
        public int RequisitionType { get; set; }
        public string CustomerId { get; set; }
        [Required]
        public int FromDepartmentReqId { get; set; }
        [Required]
        public int ToDepartmentReqId { get; set; }
        //DetailsItem
        public int RequisitionItemId { get; set; }
        public decimal? Qty { get; set; }
        public string RequisitionItemStatus { get; set; }
        public string IssueBy { get; set; }
        public DateTime IssueDate { get; set; }
        public string RawProductName { get; set; }
        public SelectList PoList { get; set; } = new SelectList(new List<object>());
        public IEnumerable<VMPackagingProductionRequisitions> DataList { get; set; }
        public List<VMPackagingProductionRequisitions> DataListPro { get; set; }

        public List<SelectListItem> DDLStockDepartmetn { get; set; }
    }

    #region Bank
    public class VMCommonDesignation : BaseVM
    {
        
        public string Name { get; set; }
        public IEnumerable<VMCommonDesignation> DataList { get; set; }
    }

    public class VMCommonBank : BaseVM
    {

        public string Name { get; set; }
        public IEnumerable<VMCommonBank> DataList { get; set; }
        public string ShortName { get; set; }
    }

    public class VMCommonShift : BaseVM
    {

        public string Name { get; set; }
        public string StarAt { get; set; }
        public string EndAt { get; set; }
        public bool PostFlag { get; set; }
        public IEnumerable<VMCommonShift> DataList { get; set; }
    }

    public class VMCommonBankBranch : BaseVM
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ZIPCode { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public SelectList BankList { get; set; } = new SelectList(new List<object>());
        public int BankId { get; set; }
        public string BankName { get; set; }
        public IEnumerable<VMCommonBankBranch> DataList { get; set; }
        public string AccountNumber { get; set; }

    }

    public class VMCommonActChequeInfo : BaseVM
    {
        public string AccountNo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/YYYY}")]
        public DateTime ChequeDate { get; set; }
        public string PayTo { get; set; }
        public decimal Amount { get; set; }
        public SelectList BankList { get; set; } = new SelectList(new List<object>());
        public SelectList BankBranchList { get; set; } = new SelectList(new List<object>());

        public SelectList SignatoryList { get; set; } = new SelectList(new List<object>());

        public int BankId { get; set; }
        public string BankName { get; set; }
        public int BankBranchId { get; set; }

        public string BankBranchName { get; set; }
        public int SignatoryId { get; set; }

        public string SignatoryName { get; set; }

        public IEnumerable<VMCommonActChequeInfo> DataList { get; set; }

    }
    #endregion
    #region Task_Management
    public class VMWorkState : BaseVM
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public long? WokboardId { get; set; }
        public long? WorkspaceId { get; set; }
        public IEnumerable<VMWorkState> DataList { get; set; }
    }
    public class VMWorkLabel : BaseVM
    {
        public long WorkLabelId { get; set; }
        public string WorkLabelName { get; set; }
        public string ColorName { get; set; }
        public long? WokboardId { get; set; }
        public long? WorkspaceId { get; set; }
        public IEnumerable<VMWorkLabel> DataList { get; set; }
    }
    public class VMWorkLabelAndSpace : BaseVM
    {
        public long WorkLabelId { get; set; }
        public string WorkLabelName { get; set; }
        public string ColorName { get; set; }
        public IEnumerable<VMWorkLabelAndSpace> LabelDataList { get; set; }
        public long WorkStateId { get; set; }
        public long WorkspaceName { get; set; }
      
        public IEnumerable<VMWorkLabelAndSpace> SpaceDataList { get; set; }
    }





 
    #endregion
}