using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation;
using KGERP.Service.Implementation.LcInfoServices;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Mvc;
//using System.Text;

namespace KGERP.Services.Procurement
{
    public class VMPurchaseOrder : BaseVM
    {
        public int CompanyId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }

        public long PurchaseOrderId { get; set; }

        public string CID { get; set; }

        public string RequisitionCID { get; set; }

        public int? Common_SupplierFK { get; set; }
        public int? StockInfoId { get; set; }
        public int SupplierPaymentMethodEnumFK { get; set; }
        public DateTime? OrderDate { get; set; } = DateTime.Now;
        public decimal TotalPOValue { get; set; }
        [AllowHtml]
        public string TermsAndCondition { get; set; }
        public string Description { get; set; }
        public string DeliveryAddress { get; set; }
        public string VendorReferenceNo { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public bool IsHold { get; set; }
        public bool IsCancel { get; set; }
        public int TermsAndConditionFk { get; set; }
        public int EmployeeId { get; set; }

        public int Status { get; set; }

        public int Procurement_PurchaseRequisitionFK { get; set; }

        public string SupplierName { get; set; }
        public string SupplierPropietor { get; set; }
        public string SupplierMobile { get; set; }
        public string SupplierAddress { get; set; }


        public string PINo { get; set; }
        public string LCNo { get; set; }
        public int? LCHeadGLId { get; set; }
        public int? LCId { get; set; }

        public decimal LCValue { get; set; }
        public decimal LCValueInBDT { get; set; }
        public string InsuranceNo { get; set; }
        public decimal PremiumValue { get; set; }

        public string ShippedBy { get; set; }
        public string PortOfLoading { get; set; }
        public string PortOfDischarge { get; set; }

        public int? CountryId { get; set; }
        public Nullable<int> FinalDestinationCountryFk { get; set; }
        public decimal FreightCharge { get; set; }
        public decimal OtherCharge { get; set; }



        public IEnumerable<VMPurchaseOrder> DataList { get; set; }
        public SelectList SupplierList { get; set; } = new SelectList(new List<object>());
        public SelectList TermNCondition { get; set; } = new SelectList(new List<object>());
        public SelectList PRList { get; set; } = new SelectList(new List<object>());

        public SelectList CountryList { get; set; } = new SelectList(new List<object>());
        public SelectList EmployeeList { get; set; } = new SelectList(new List<object>());
        public SelectList Businessheadlist { get; set; } = new SelectList(new List<object>());
        public SelectList StockInfoList { get; set; } = new SelectList(new List<object>());

        public SelectList ShippedByList { get; set; } = new SelectList(new List<object>());
        public SelectList LCList { get; set; } = new SelectList(new List<object>());
        public LcInfoViewModel LcValueList { get; set; } 

        public VendorsPaymentMethodEnum POPaymentMethod { get { return (VendorsPaymentMethodEnum)this.SupplierPaymentMethodEnumFK; } }// = SupplierPaymentMethodEnum.Cash;
        public string POPaymentMethodName { get { return BaseFunctionalities.GetEnumDescription(POPaymentMethod); } }
        public SelectList POPaymentMethodList { get { return new SelectList(BaseFunctionalities.GetEnumList<VendorsPaymentMethodEnum>(), "Value", "Text"); } }




        public EnumPOStatus EnumStatus { get { return (EnumPOStatus)this.Status; } }
        public string EnumStatusName { get { return BaseFunctionalities.GetEnumDescription(this.EnumStatus); } }
        public SelectList EnumStatusList { get { return new SelectList(BaseFunctionalities.GetEnumList<EnumPOStatus>(), "Value", "Text"); } }



        public decimal RequiredQuantity { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal PaidAmount { get; set; }

        public decimal InAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int PaymentId { get; set; }
        #region new_fields
        public decimal TDSDeduction { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal VATAddition { get; set; }
        #endregion
    }

    public class IssueMasterInfoVM : BaseVM
    {
        public long IssueMasterId { get; set; }
        public string IssueNo { get; set; }
        public DateTime IssueDate { get; set; }
        public long IssuedBy { get; set; }
        public int VendorId { get; set; }
        public int CompanyId { get; set; }
        public string ModifedBy { get; set; }
        public bool Achknolagement { get; set; }
        public long AchknologeBy { get; set; }
        
        public DateTime? AcknologeDate { get; set; }
    }

    public class IssueDetailInfoVM : IssueMasterInfoVM
    {
        public long IssueDetailId { get; set; }
        public int? RProductId { get; set; }
        public decimal? RMQ { get; set; }
        public decimal CostingPrice { get; set; }
        public decimal RemainingStock { get; set; }
        public string IssueBy { get; set; }
        public string CustomerBy { get; set; }
        public string ProductName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerMobile { get; set; }
        public string EmployeeMobile { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string UnitName { get; set; }
        public string AchknologeName { get; set; }
        public bool IsSubmit { get; set; }
        public List<IssueDetailInfoVM> DataListSlave { get; set; }
       
    }


    public class VMPurchaseOrderSlave : VMPurchaseOrder
    {
        public DateTime lcDate { get; set; }

        public long PurchaseOrderDetailId { get; set; }
        public int FinishProductBOMId { get; set; }
        public string OrderNo { get; set; }
        public string StyleNo { get; set; }
        public decimal Consumption { get; set; }

        public int DemandId { get; set; } = 0;
        public string DemandNo { get; set; } = "";
        public long OrderMasterId { get; set; }
        public int Common_ProductFK { get; set; }
        public int Common_ProductCategoryFK { get; set; }
        public int Common_ProductSubCategoryFK { get; set; }

        public decimal PurchaseQuantity { get; set; }
        public decimal ProcuredQuantity { get; set; }
        public string ProductName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeDesignation { get; set; }
        public string EmployeeMobile { get; set; }

        public string DescriptionSlave { get; set; }
        public decimal? RequisitionQuantity { get; set; }
        public decimal PurchasingPrice { get; set; }
        public decimal TotalPrice { get { return PurchaseQuantity * PurchasingPrice; } }

        public string TotalPriceInWord { get; set; }

        public string UnitName { get; set; }
        public SelectList ProductCategoryList { get; set; } = new SelectList(new List<object>());
        public SelectList ProductSubCategoryList { get; set; } = new SelectList(new List<object>());
        public SelectList ProductList { get; set; } = new SelectList(new List<object>());
        public List<VMPurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        public IEnumerable<VMPurchaseOrderSlave> DataListSlave { get; set; }
        public List<VMPurchaseOrderSlave> DataListPur { get; set; }
        public decimal PurchaseAmount { get; set; }

        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string Companylogo { get; set; }
        public string CommonCustomerName { get; set; }
        public string CommonCustomerCode { get; set; }

        public long CustomerId { get; set; }
        public long CustomerTypeFk { get; set; }
        public int TermsConditionTypeId { get; set; }
        public string Currencyname { get; set; }
        [AllowHtml]
        public string TermAndCondition { get; set; }
        public string currencysign { get; set; }
        public decimal CurrencyRate { get; set; }
        public decimal? FreightCharges { get; set; }
        public decimal? CommissionValue { get; set; }
        public decimal? InsuranceValue { get; set; }
        public decimal? AmendmentIncrase { get; set; }
        public decimal? AmendmentDiccrase { get; set; }
        public decimal? CashMarginAmount { get; set; }
        public decimal? CashMarginPercent { get; set; }
        public decimal? BankCharge { get; set; }
        #region new_fields
        public decimal TDSDeduction { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal VATAddition { get; set; }
        public bool IsVATIncluded { get;  set; }
        #endregion

    }

    public class VMPurchaseOrderDetail
    {
        public string ProductName { get; set; }
        public string CurSign { get; set; }
        public decimal? CureencyRate { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal PurchasingPrice { get; set; }
       

    }



    public partial class VMLCAmendment : BaseVM
    {
        public long LCAmendmentId { get; set; }
        public long LCId { get; set; }
        public System.DateTime AmendmentDate { get; set; }
        public decimal AmendmentValue { get; set; }
        public bool IsIncrase { get; set; }
        public bool IsSubmit { get; set; }
        public int? AccountingHeadId { get; set; }
        public int? AccountingBankHeadId { get; set; }
        public int CompanyId { get; set; }
        public IEnumerable<VMLCAmendment> DataList { get; set; }
        public string LCNO { get; set; }
    }

    public class LcValueAmmendmentvm:BaseVM
    {
        public long LCValueAmendmentId { get; set; }
        public long LCId { get; set; }
        public System.DateTime AmendmentDate { get; set; }
        public decimal LCValue { get; set; }
        public decimal LCValueInBDT { get; set; }
        public decimal CashMarginPercent { get; set; }
        public decimal CashMarginAmount { get; set; }
        public decimal BankCharge { get; set; }
        public decimal InsuranceValue { get; set; }
        public decimal CommissionValue { get; set; }
        public decimal FreightCharges { get; set; }
        public decimal OtherCharge { get; set; }
        public bool IsActive { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public List<LcValueAmmendmentvm> lcValueAmmendmentvms { get; set; }
        public int? AccountingHeadId { get;  set; }
        public int? AccountingBankHeadId { get;  set; }
        public string LCNo { get;  set; }
    }




    public partial class VMPromtionalOffer : BaseVM
    {
        public int PromtionalOfferId { get; set; }
        public string PromoCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CompanyId { get; set; }
        public int PromtionType { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public bool IsSumitted { get; set; }
        public IEnumerable<VMPromtionalOffer> DataList { get; set; }

        public PromotionTypeEnum PromotionType { get { return (PromotionTypeEnum)this.PromtionType; } }
        public string PromotionTypeName { get { return BaseFunctionalities.GetEnumDescription(PromotionType); } }
        public SelectList PromtionTypeList { get { return new SelectList(BaseFunctionalities.GetEnumList<PromotionTypeEnum>(), "Value", "Text"); } }

    }

    public partial class VMPromtionalOfferDetail : VMPromtionalOffer
    {
        public int PromtionalOfferDetailId { get; set; }
        public int ProductId { get; set; }
        public decimal PromoQuantity { get; set; }
        public decimal PromoAmount { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }


        public IEnumerable<VMPromtionalOfferDetail> DataListSlave { get; set; }
    }
    public class VMSalesOrder : BaseVM
    {
        public int CompanyId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public string CustomerPONo { get; set; }
        public long OrderMasterId { get; set; }
        public string OrderNo { get; set; }
        public string OrderNoMsg { get; set; }
        public string ProductType { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public int? PromotionalOfferId { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public int CustomerPaymentMethodEnumFK { get; set; }
        public string Description { get; set; }
        public double TotalAmount { get; set; }
        public double PayableAmount { get; set; }
        public decimal ReturnAmount { get; set; }
        public decimal? CreditLimit { get; set; }
        public double PaidAmount { get; set; }
        public int Accounting_BankOrCashParantId { get; set; }
        public int Accounting_BankOrCashId { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal ProductDiscountUnit { get; set; }
        public decimal ProductDiscountTotal { get; set; }

        public decimal NetUnitPrice { get; set; }

        public decimal DiscountAmount { get; set; }
        public int Status { get; set; }
        public string PaymentToHeadGLName { get; set; }
        public int StockInfoId { get; set; } //warehouse / Depot

        public int SalePersonId { get; set; }
        public string CompanyAddress { get; set; }
        public string ReelDirection { get; set; }
        public string PouchDerection { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string CommonCustomerName { get; set; }
        public string CommonCustomerCode { get; set; }

        public string SubZonesName { get; set; }

        public string SubZoneIncharge { get; set; }
        public string SubZoneInchargeMobile { get; set; }
        public string Propietor { get; set; }

        public string ZoneName { get; set; }
        public string WareHouse { get; set; }

        public string ZoneIncharge { get; set; }
        public int CustomerTypeFk { get; set; }

        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerEmail { get; set; }

        public string ContactPerson { get; set; }
        public string OrderStatus { get; set; }

        public IEnumerable<VMSalesOrder> DataList { get; set; }
        public IEnumerable<OrderDetail> style { get; set; }

        public SelectList CustomerList { get; set; } = new SelectList(new List<object>());
        public SelectList TermNCondition { get; set; } = new SelectList(new List<object>());
        public SelectList OrderMusterList { get; set; } = new SelectList(new List<object>());
        public SelectList BankOrCashParantList { get; set; } = new SelectList(new List<object>());
        public SelectList BankOrCashGLList { get; set; } = new SelectList(new List<object>());
        public SelectList PromoOfferList { get; set; } = new SelectList(new List<object>());
        public SelectList StockInfoList { get; set; } = new SelectList(new List<object>());
        public SelectList MarketingOfficers { get; set; } = new SelectList(new List<object>());


        public VendorsPaymentMethodEnum POPaymentMethod { get { return (VendorsPaymentMethodEnum)this.CustomerPaymentMethodEnumFK; } }// = SupplierPaymentMethodEnum.Cash;
        public string POPaymentMethodName { get { return BaseFunctionalities.GetEnumDescription(POPaymentMethod); } }
        public SelectList POPaymentMethodList { get { return new SelectList(BaseFunctionalities.GetEnumList<VendorsPaymentMethodEnum>(), "Value", "Text"); } }


        public EnumPOStatus EnumStatus { get { return (EnumPOStatus)this.Status; } }
        public string EnumStatusName { get { return BaseFunctionalities.GetEnumDescription(this.EnumStatus); } }
        public SelectList EnumStatusList { get { return new SelectList(BaseFunctionalities.GetEnumList<EnumPOStatus>(), "Value", "Text"); } }

        public decimal InAmount { get; set; }

        public decimal TotalInvoiceDiscount { get; set; }
        public decimal TotalAmountAfterDiscount { get; set; }

        public int PaymentId { get; set; }
        public string FinalDestination { get; set; }
        public string CourierNo { get; set; }
        public double CourierCharge { get; set; }
        public string StockInfoNameMsg { get;  set; }
    }


    public class VMSalesOrderSlave : VMSalesOrder
    {
        public int ProductId;

        public string ProductName { get; set; }
        public string ComLogo { get; set; }
        public int DemandId { get; set; } = 0;
        public string DemandNo { get; set; } = "";

        public long OrderDetailId { get; set; }
        public int FProductId { get; set; }
        public int? AccountingIncomeHeadId { get; set; }
        public double Qty { get; set; }
        public double UnitPrice { get; set; }
        public double? Consumption { get; set; }
        public double? PackQuantity { get; set; }
        public decimal? QtyInPack { get; set; }
        public double? PackSize { get; set; }

        public double TotalPrice { get { return Qty * UnitPrice; } }
        public string TotalPriceInWord { get; set; }
        public string UnitName { get; set; }        
        public string ProductCategoryName { get; set; }
        public string ProductSubCategoryName { get; set; }
        public string OrderNo { get; set; }
        public SelectList ProductCategoryList { get; set; } = new SelectList(new List<object>());
        public SelectList ProductSubCategoryList { get; set; } = new SelectList(new List<object>());
        public SelectList ProductList { get; set; } = new SelectList(new List<object>());
        public SelectList SubZoneList { get; set; } = new SelectList(new List<object>());
        public SelectList ZoneList { get; set; } = new SelectList(new List<object>());
        public SelectList StockInfoList { get; set; } = new SelectList(new List<object>());

        public IEnumerable<VMSalesOrderSlave> DataListSlave { get; set; }
        public int ProductSubCategoryId { get; set; }
        public int? AccountingHeadId { get; set; }
        public int ProductCategoryId { get; set; }
        public int SubZoneFk { get; set; }
        public int ZoneFk { get; set; }
        public string OfficerNAme { get; set; }
        public string IntegratedFrom { get; set; }
        public decimal CashDiscountPercent { get; set; }
        public decimal SpecialDiscount { get; set; }
        public decimal VATPercent { get; set; }
        public double VATAmount { get; set; }
        public decimal TDSPercent { get; set; }
        public decimal? CashCommission { get; set; } = 0;
        public decimal? BaseCommission { get; set; } = 0;
        public decimal? CarryingCommission { get; set; } = 0;
        public decimal? AdditionPrice { get; set; } = 0;
        public decimal? MonthlyIncentive { get; set; } = 0;
        public decimal? YearlyIncentive { get; set; } = 0;
        
        public bool IsService { get; set; }
        public string JobOrderNo { get; set; }
        public string LotNumber { get; set; }
        public bool IsVATInclude { get; set; }
        public decimal SpecialBaseCommission { get;  set; }
        public decimal DiscountUnit { get; set; }
    }
    public partial class FeedOrderMaster : VMFeedPayment
    {
       
        public int DemandId { get; set; }
       
        public Nullable<System.DateTime> ExpectedDeliveryDate { get; set; }
        public string OrderMonthYear { get; set; }
       
        public Nullable<long> SalePersonId { get; set; }
        public string SalePersonName { get; set; }
        public Nullable<int> StockInfoId { get; set; }
        
        public string OrderStatus { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }
      
        
        public System.DateTime CreateDate { get; set; }

        public int Status { get; set; }
        public int PaymentMethod { get; set; }
    
        public bool IsOpening { get; set; }
        public decimal CurrentPayable { get; set; }
        public string WareHouse { get; set; }
        public string Propietor { get; set; }
      
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string ContactPerson { get; set; }
        public string CustomerAddress { get; set; }
        public string CompanyAddress { get; set; }
        public string CustomerName { get; set; }
        public bool HaveShortCredit { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string ZoneName { get; set; }
    
        public int CustomerTypeFk { get; set; }


       
        public decimal BaseCommission { get; set; }

        public bool IsIncentiveInInvoice { get; set; }
    
        public decimal CarryingCommission { get; set; }
    }
    public class FeedSalesOrderModel : FeedOrderMaster
    {

        public long CurrentEmployeeIntId { get; set; }

        public string ProductName { get; set; }
        public string ComLogo { get; set; }
        
        public long OrderDetailId { get; set; }
        public int FProductId { get; set; }
        public double Qty { get; set; }
        public double UnitPrice { get; set; }
        public double? Consumption { get; set; }
        public double? PackQuantity { get; set; }

        public double TotalPrice { get { return Qty * UnitPrice; } }
        public string TotalPriceInWord { get; set; }
        public string UnitName { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductSubCategoryName { get; set; }
        public string SalesOrderNo { get; set; }

      
        public SelectList ProductList { get; set; } = new SelectList(new List<object>());
       
        public SelectList StockInfoList { get; set; } = new SelectList(new List<object>());
        public SelectList MarketingOfficers { get; set; } = new SelectList(new List<object>());

        public IEnumerable<FeedSalesOrderModel> DataListSlave { get; set; }
        public int ProductSubCategoryId { get; set; }
        public int ProductCategoryId { get; set; }
      
        public string OfficerNAme { get; set; }
        public decimal CashCommission { get; set; }
        public decimal SpecialDiscount { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal CurrentInvoiceAdvanced { get; set; }
        public decimal PendingInvoiceAdvanced { get; set; }
        public decimal MonthlyIncentiveInInvoice { get; set; }
        public decimal YearlyIncentiveInInvoice { get; set; }
        public Head5 head5 { get; set; }
        public HeadGL headGl { get; set; }
        public decimal LedgerBalance { get;  set; }
    }

    public class VmTransaction
    {

        public int ID { get; set; }
        public DateTime? Date { get; set; }
        public DateTime FirstCreateDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }


        public long PurchaseOrdersId { get; set; }
        public long OrderMasterId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public double CustomerCredit { get; set; }

        public decimal Balance { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public VMCommonSupplier VMCommonSupplier { get; set; }
        public bool DrIncrease { get; set; }
        public decimal? CostOfGoodsSold { get; set; }
        public decimal? GrossProfit { get; set; }
        public decimal? NetProfit { get; set; }
        public decimal? Sales { get; set; }
        public decimal? Purchased { get; set; }
        public decimal? OperatingExp { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public decimal CurrencyRate { get; set; }
        public bool IsPoDeleted { get; set; }
        public string IpoNumber { get; set; }
        public int? CompanyFK { get; set; }
        public int? VendorFK { get; set; }


        public List<VmTransaction> DataList { get; set; }
        public double CourierCharge { get; set; }
    }

    public class VmInventoryDetails
    {

        public int ID { get; set; }
        public DateTime? Date { get; set; }
        public DateTime FirstCreateDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }


        public long PurchaseOrdersId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public double Debit { get; set; }
        public decimal DebitDecimal { get; set; }

        public decimal Credit { get; set; }
        public double CustomerCredit { get; set; }

        public decimal Balance { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public VMCommonProduct VMCommonProduct { get; set; }
        public bool DrIncrease { get; set; }
        public decimal? CostOfGoodsSold { get; set; }
        public decimal? GrossProfit { get; set; }
        public decimal? NetProfit { get; set; }
        public decimal? Sales { get; set; }
        public decimal? Purchased { get; set; }
        public decimal? OperatingExp { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public decimal CurrencyRate { get; set; }
        public bool IsPoDeleted { get; set; }
        public string IpoNumber { get; set; }
        public int? CompanyFK { get; set; }
        public int? ProductFK { get; set; }


        public List<VmInventoryDetails> DataList { get; set; }
        public double CourierCharge { get; set; }



    }

    public class VMProductStock : BaseVM
    {
        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public int ProductSubCategoryId { get; set; }
        public string ProductSubCategoryName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TPPrice { get; set; }
        public decimal? CreditSalePrice { get; set; }

        public decimal CostingPrice { get; set; }
        public decimal ReceiveQty { get; set; }
        public decimal SalesQuantity { get; set; }
        public decimal SaleReturnQuantity { get; set; }
        public decimal StockAdjustExcessQty { get; set; }
        public decimal StockAdjustLessQty { get; set; }
        public decimal ClosingQty { get; set; }
        public decimal ClosingRate { get; set; }
        public decimal CurrentStock { get; set; }


    }


    public class VmCustomerAgeingPrint
    {
        public string AsOnDate { get; set; }
        public IEnumerable<VmCustomerAgeing> DataList { get; set; }
    }
    public class VmCustomerAgeing : BaseVM
    {
        public string ReportType { get; set; }

        public string ZoneName { get; set; }
        public string ZoneCode { get; set; }
        public string ZoneIncharge { get; set; }
        public string SubZoneName { get; set; }
        public string SubZoneCode { get; set; }
        public string SalesOfficerName { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public int VendorId { get; set; }
        public string CustomerMode { get; set; }
        public decimal GrossSaleAmount { get; set; }
        public decimal SaleReturnAmount { get; set; }
        public decimal RecoverAmount { get; set; }
        public decimal ZeroToThirtyDayes { get; set; }
        public decimal ThirtyOneToSixtyDayes { get; set; }
        public decimal SixtyOneToNintyDayes { get; set; }
        public decimal NintyOneTo120Dayes { get; set; }
        public decimal OneH21To150Dayes { get; set; }
        public decimal OneH51To180Dayes { get; set; }
        public decimal OneH81To210Dayes { get; set; }
        public decimal TwoH11To240Dayes { get; set; }
        public decimal TwoH41To270Dayes { get; set; }
        public decimal TwoH71To360Dayes { get; set; }
        public decimal Over360Dayes { get; set; }
        public decimal CurrentMonthCollection { get; set; }
        public decimal CurrentMonthGrossSale { get; set; }

        public string AsOnDate { get; set; }
        public int? ZoneId { get; set; }
        public int? SubZoneId { get; set; }
        public IEnumerable<VmCustomerAgeing> DataList { get; set; }
        public SelectList ZoneListList { get; set; } = new SelectList(new List<object>());
        public SelectList TerritoryList { get; set; } = new SelectList(new List<object>());
        public string ReportName { get; set; }
    }

    public class VmDemandService : BaseVM
    {
        public long DemandId { get; set; }
        public int CompanyId { get; set; }
        public string DemandNo { get; set; }

        public int RequisitionType { get; set; }
        public DateTime DemandDate { get; set; }
        public int Status { get; set; }
        public string CID { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsInvoiceCreated { get; set; }
        public int StockInfoId { get; set; }
        public string StockInfoName { get; set; }
        public int PromotionalOfferId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string SubZoneFkName { get; set; }
        public int? SubZoneFk { get; set; }
        public string RequisitionCID { get; set; }
        public SelectList ProductList { get; set; } = new SelectList(new List<object>());
        public IEnumerable<VmDemandItemService> vmDemandItems { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<VmDemandService> dataList { get; set; }

        public SelectList PaymentByList { get; set; } = new SelectList(new List<object>());
        public SelectList CustomerList { get; set; } = new SelectList(new List<object>());
        public SelectList StockInfoList { get; set; } = new SelectList(new List<object>());
        public SelectList SubZoneList { get; set; } = new SelectList(new List<object>());
        public SelectList ZoneList { get; set; } = new SelectList(new List<object>());
        public SelectList PromoOfferList { get; set; } = new SelectList(new List<object>());
        public decimal TotalAmountAfterDiscount { get; set; }
        public int SupplierPaymentMethodEnumFK { get; set; }
        public int CustomerPaymentMethodEnumFK { get; set; }
        public VendorsPaymentMethodEnum POPaymentMethod { get { return (VendorsPaymentMethodEnum)this.CustomerPaymentMethodEnumFK; } }
        public string POPaymentMethodName { get { return BaseFunctionalities.GetEnumDescription(POPaymentMethod); } }
        public SelectList POPaymentMethodList { get { return new SelectList(BaseFunctionalities.GetEnumList<VendorsPaymentMethodEnum>(), "Value", "Text"); } }
        public decimal GlobalDiscount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal ProductDiscountAmount { get; set; }
        [Required]
        [Range(1, 99999999999999999, ErrorMessage = "Pay Amount field is required.")]
        public decimal PayAmount { get; set; } = 0;
        public decimal SalesStatus { get; set; } = 0;
        public decimal CreditStatus { get; set; } = 0;

        public string AccCode { get; set; }
        public string AccName { get; set; }
        [Required]
        public int HeadGLId { get; set; }
        public string ReceiptDateString { get; set; }
        public string ChequeDateString { get; set; }
    }

    public class VmDemandItemService : VmDemandService
    {
        public long DemandItemId { get; set; }
        public int ProductId { get; set; }
        [Required]
        [Range(1, 99999999999999999, ErrorMessage = "Quantity field is required.")]
        public long ItemQuantity { get; set; }
        public string BankName { get; set; }
        public string ProductName { get; set; }
        public string ProductCategories { get; set; }
        public string ProductSubCategories { get; set; }
        public string UnitName { get; set; }
        public decimal ProductDiscountUnit { get; set; }
        public decimal CashDiscountPercent { get; set; }
        public decimal CshDiscountAmount { get; set; }
        public decimal SpecialDiscount { get; set; }
        public decimal ProductDiscount { get; set; }

        public decimal ProductDiscountTotal { get; set; }
        public double TotalAmount { get; set; }
        public double UnitPrice { get; set; }
        public double ProductPrice { get; set; }
        public decimal DiscountRate { get; set; }
        public double TotalPrice { get { return ItemQuantity * UnitPrice; } }
        public DPayment DPayment { get; set; }
        public List<VmDemandItemService> DemandItemList { get; set; }
        public List<SelectModelType> BankList { get; set; }
    }
    public class DemandOrderItems
    {

        public int DemandItemId { get; set; } = 0;
        public int ProductId { get; set; }
        public double qty { get; set; }
        public double UnitPrice { get; set; }
        public decimal UnitDiscount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalAmmount { get; set; }
    }
    public class DPayment
    {
        public string MoneyReceiptNo { get; set; }
        public string ReceiptDateString { get; set; }
        public decimal RefAmount { get; set; }
        public string ChequeDateString { get; set; }
        public string ChequeNo { get; set; }
    }
    public class OfficerSalestargetVm
    {
        public long OfficerSalesTargetId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public long EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public decimal TargetedQuantity { get; set; }
        public string CreatedBy { get; set; }
        public string EmployeeName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public long ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public List<SelectModelEmp> SelectModels { get; set; }
        public List<OfficerSalestargetVm> officerSalestargetVms { get; set; }
    }

    public class DemandOrderPaymentViewmodel
    {
        public String MoneyReceiptNo { get; set; }
        public DateTime? MoneyReceiptDate { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string ChequeNo { get; set; }
        public decimal BankCharge { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public int HeadGLId { get; set; }
        public Decimal? RefAmount { get; set; }
        public string ReceiptDateString { get; set; }
        public string ChequeDateString { get; set; }
    }

    public class OrderDetailVM
    {
        public long OrderDetailID { get; set; }
        public long companyId { get; set; }
        public string CompanyName { get; set; }
        public int ItemId { get; set; }
        public int? StatusId { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public double Qty { get; set; }
        public string InvoiceNo { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Remarks { get; set; }
        public string Structure { get; set; }

        public string ReelDirection { get; set; }
        public string PouchDerection { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public IEnumerable<OrderDetailVM> DataList { get; set; }
        public List<OrderDetailVM> OrderDetailList { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
    }
}