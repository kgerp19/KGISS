using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel
{
    public class PurchaseOrderModel
    {
        public string ButtonName
        {
            get
            {
                return PurchaseOrderId > 0 ? "Update" : "Save";
            }
        }
        public long PurchaseOrderId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("PO No")]
        public string PurchaseOrderNo { get; set; }
        [DisplayName("Demand No")]
        [Required]
        public Nullable<long> DemandId { get; set; }
        [DisplayName("Supplier")]
        public Nullable<int> SupplierId { get; set; }
        [DisplayName("PO Date")]
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        [DisplayName("Mode Of Purchase")]
        public Nullable<int> ModeOfPurchaseId { get; set; }
        public string ModeOfPurchase { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }
        [DisplayName("Responsible Person")]
        public Nullable<long> EmpId { get; set; }
        [DisplayName("Product Origin")]
        public Nullable<int> ProductOriginId { get; set; }
        public string ProductOrigin { get; set; }

        [DisplayName("Country")]
        public Nullable<int> CountryId { get; set; }
        public string CountryName { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("Credit Duration (Days)")]
        public string Days { get; set; }
        [Required]
        [DisplayName("Delivery Date")]
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        [DisplayName("Status")]
        public string PurchaseOrderStatus { get; set; }
        public Nullable<System.Guid> ReferenceNo { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }


        public string PINo { get; set; }
        public string ShippedBy { get; set; }
        public string PortOfLoading { get; set; }
        public string PortOfDischarge { get; set; }
        public Nullable<int> FinalDestinationCountryFk { get; set; }
        public decimal FreightCharge { get; set; }
        public decimal OtherCharge { get; set; }
        public decimal TDSDeduction { get; set; }
        public string LCNo { get; set; }
        public int? LCHeadGLId { get; set; }
        public decimal LCValue { get; set; }
        public string InsuranceNo { get; set; }
        public decimal PremiumValue { get; set; }
        public SelectList ShippedByList { get; set; } = new SelectList(new List<object>());
        public SelectList LCList { get; set; } = new SelectList(new List<object>());


        public virtual DemandModel Demand { get; set; }
        public virtual VendorModel Vendor { get; set; }
        public virtual ICollection<PurchaseOrderDetailModel> PurchaseOrderDetails { get; set; }
        public List<PurchaseOrderDetailModel> ItemList { get; set; } = new List<PurchaseOrderDetailModel>();


        //-------------------------Extended Property----------------
        [Required]
        [DisplayName("Supplier Name")]
        public string SupplierName { get; set; }

        public int AccountHeadId { get; set; }

        [Required]
        [DisplayName("Responsible Person")]
        public string EmployeeName { get; set; }

        [DisplayName("Demand No")]
        public string DemandNo { get; set; }

        public string PurchseOrderMessage { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<PurchaseOrderModel> DataList { get; set; }
        public long LCId { get; set; }


        [AllowHtml]
        public string TermAndCondition { get; set; }

    }
}
