using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class StoreModel
    {
        public string ButtonName
        {
            get
            {
                return StoreId > 0 ? "Update" : "Create";
            }
        }
        public long StoreId { get; set; }
        [Required(ErrorMessage = "Select Store Name")]
        [DisplayName("Store")]
        public Nullable<int> StockInfoId { get; set; }
        [DisplayName("Company Name")]
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Supplier")]
        [Required]
        public Nullable<int> VendorId { get; set; }
        [Required]
        [DisplayName("Receive Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        [DisplayName("Receive Code")]
        public string ReceivedCode { get; set; }
        [DisplayName("Purchase Order No")]
        public Nullable<long> PurchaseOrderId { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public string Time { get; set; }
        public Nullable<long> RequisitionId { get; set; }
        [Required]
        [DisplayName("Challan No")]
        public string ChallanNo { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Challan Date")]
        public Nullable<System.DateTime> ChallanDate { get; set; }
        public string ParchaseOrderNo { get; set; }
        public string TallyNo { get; set; }
        [DisplayName("Unload Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> UnloadingDate { get; set; }
        public string TruckNo { get; set; }
        public string DriversName { get; set; }
        public Nullable<long> ReceivedBy { get; set; }
        public decimal TruckFare { get; set; }
        public decimal LabourBill { get; set; }
        public Nullable<System.DateTime> PODate { get; set; }
        public Nullable<System.DateTime> DemandDate { get; set; }
        [DisplayName("L/C No ")]
        public string LcNo { get; set; }
        [DisplayName("L/C Qty")]
        public decimal LcQty { get; set; }
        [DisplayName("L/C Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> LcDate { get; set; }
        [DisplayName("L/C Value")]
        public decimal LcValue { get; set; }
        [DisplayName("BDT Value")]
        public decimal LcValueBDT { get; set; }
        public string Currency { get; set; }
        [DisplayName("Conv. Rate")]
        public decimal ConvRateBDT { get; set; }
        [DisplayName("Insurence Premium Charge")]
        public decimal InsurencePremiumCharge { get; set; }
        [DisplayName("Bank Charge")]
        public decimal BankCharge { get; set; }
        [DisplayName("Customer Duty Charge")]
        public decimal CustomDutyCharge { get; set; }
        [DisplayName("Other Charge")]
        public decimal OtherCharge { get; set; }



        public virtual CompanyModel Company { get; set; }
        public List<SelectModel> Vendors { get; set; }
        public List<SelectModel> StockInfos { get; set; }
        public virtual StockInfoModel StockInfo { get; set; }
        public virtual VendorModel Vendor { get; set; }
        public virtual PurchaseOrderModel PurchaseOrder { get; set; }
        public virtual RequisitionModel Requisition { get; set; }

        [Required]
        public virtual ICollection<StoreDetailModel> StoreDetails { get; set; }


        //---------------------Additional field-----------------------------
        [DisplayName("Total Qty (Kg)")]
        public double? TotalQty { get; set; }
        public string StoreName { get; set; }
        public string Type { get; set; }
        [DisplayName("Demand No")]
        public string DemandNo { get; set; }
        public int ProductId { get; set; }
        public string SupplierName { get; set; }
        public string ReceiverName { get; set; }

        public string RequisitionNo { get; set; }

        public bool IsSubmited { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<StoreModel> DataList { get; set; }
        public string ReceivedByName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string ProductName { get; internal set; }
        public double Qty { get; internal set; }
        public decimal UnitPrice { get; internal set; }
        public int? BagId { get; internal set; }
        public string BagName { get; internal set; }
        public decimal Amount { get; internal set; }
        public int BagQty { get; internal set; }
    }

    public partial class VmStoreModel
    {
        public long StoreId { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string ReceivedCode { get; set; }
        public string SupplierName { get; set; }
        public string ReceiverName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string StoreName { get; set; }
        public string Remarks { get; set; }
        public int CompanyId { get; set; }
        public bool IsSubmited { get; set; }

    }


    public partial class VMStoreDetail : VmStoreModel
    {
        public long StoreDetailId { get; set; }
        public string ProductName { get; set; }
        public int? ProductId { get; set; }
        public double? Qty { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal TPPrice { get; set; }

        public IEnumerable<VMStoreDetail> DataListDetail { get; set; }
        public bool IsActive { get; set; }
        public string UnitName { get; set; }
        public int? AccountingExpenseHeadId { get; set; }
        public int? AccountingHeadId { get; set; }
        public int BagId { get;  set; }
        public string BagName { get;  set; }
        public int? BagAccountingHeadId { get;  set; }
        public int BagQty { get;  set; }
        public decimal? BagUnitPrice { get; set; }
    }
}
