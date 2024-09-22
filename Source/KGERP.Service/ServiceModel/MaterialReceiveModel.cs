using KGERP.Data.Models;
using KGERP.Services.WareHouse;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Protocols.WSTrust;

namespace KGERP.Service.ServiceModel
{
    public class MaterialReceiveModel
    {
        public long MaterialReceiveId { get; set; }
        public int CompanyId { get; set; }
        public Nullable<long> PurchaseOrderId { get; set; }
        public string MaterialType { get; set; }
        [DisplayName("Receive No")]
        public string ReceiveNo { get; set; }
        [DisplayName("Stock")]
        [Required]
        public Nullable<int> StockInfoId { get; set; }
        public int VendorId { get; set; }
        [DisplayName("Received By")]
        [Required]
        public Nullable<long> ReceivedBy { get; set; }
        [Required]
        [DisplayName("Received Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        [Required]
        public string ChallanNo { get; set; }
        [Required]
        [DisplayName("Challan Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ChallanDate { get; set; }
        [Required]
        [DisplayName("Unloading Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> UnloadingDate { get; set; }
        public string TruckNo { get; set; }
        public string DriverName { get; set; }
        public decimal TruckFare { get; set; }
        public decimal LabourBill { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public string MaterialReceiveStatus { get; set; }
        public Nullable<bool> IsSubmitted { get; set; }

        public virtual IList<MaterialReceiveDetailModel> MaterialReceiveDetails { get; set; }

        //------------------Extended Models----------------------
        [DisplayName("PO No")]
        public string PurchaseOrderNo { get; set; }
        [DisplayName("PO Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> PurchaseOrderDate { get; set; }
        public string SupplierName { get; set; }
        public string StoreName { get; set; }

        [Required]
        [DisplayName("Receiver")]
        public string ReceiverName { get; set; }
        public string DemandNo { get; set; }
        [DisplayName("Is Closed")]
        public bool IsOrderClose { get; set; }
        [DisplayName("Demand Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DemandDate { get; set; }
        public bool AllowLabourBill { get; set; }
        public string ResponsiblePerson { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<MaterialReceiveModel> DataList { get; set; }
    }
    public class SeedMaterialRcvViewModel
    {
        public List<VMMaterialRcvList> MRlist { get; set; }
        public int companyId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
    }
    public class KFMALMaterialRcvViewModel
    {
        public List<VMMaterialRcvList> MRlist { get; set; }
        public int companyId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
    }
    public class GCCLMaterialRecieveVm
    {
        public long MaterialReceiveId { get; set; }
        public int CompanyId { get; set; }
        public string ReceiveNo { get; set; }
        public string PoNo { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.DateTime> PoDate { get; set; }
        public string SupplierName { get; set; }
        public string Status { get; set; } 
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsSubmitted { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<GCCLMaterialRecieveVm> DataList { get; set; }  


    }

    public class KFMALMaterialRecieveVm
    {
        public long MaterialReceiveId { get; set; }
        public int CompanyId { get; set; }
        public int SupplierPaymentMethodId { get; set; }
        public string ReceiveNo { get; set; }
        public string ChallanNo { get; set; }
        public string PoNo { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.DateTime> PoDate { get; set; }
        public string SupplierName { get; set; }
        public string Status { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsSubmitted { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<KFMALMaterialRecieveVm> DataList { get; set; }


    }


    public class VMRawMaterialStock
    {
        
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public string CompanyLogo { get; set; }
        public string StockName { get; set; }
        public int ProductCategoryId { get; set; }
        public int ProductSubCategoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductCategory { get; set; }
        public string ProductSubCategory { get; set; }
        public string Product { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal ReceivedRate { get; set; }
        public decimal ReceivedValue { get; set; }
        public decimal ConsumptionQty { get; set; }
        public decimal ConsumptionValue { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal ReturnValue { get; set; }
        public decimal SaleQty { get; set; }
        public decimal SaleValue { get; set; }
        public decimal ExcessQty { get; set; }
        public decimal ExcessValue { get; set; }
        public decimal LessQty { get; set; }
        public decimal LessValue { get; set; }
        public decimal ClosingQty { get; set; }
        public decimal ClosingRate { get; set; }
        public decimal StockAmount { get; set; }
    }
}
