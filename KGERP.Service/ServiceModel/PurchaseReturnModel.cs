using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
    public class PurchaseReturnModel
    {
        
        public long PurchaseReturnId { get; set; }
        public int CompanyId { get; set; }
        [DisplayName("Product Type")]
        public string ProductType { get; set; }
        [DisplayName("Return No")]
        public string ReturnNo { get; set; }
        [DisplayName("Return Date")]
        [Required(ErrorMessage = "Please select Return Date.")]
        public Nullable<System.DateTime> ReturnDate { get; set; }
        [Required]
        [DisplayName("Reason")]
        public string ReturnReason { get; set; }
        [DisplayName("Warehouse")]
        public int StockInfoId { get; set; }
        [DisplayName("Supplier")]
        public int SupplierId { get; set; }
        [DisplayName("Return Person")]
        public Nullable<long> ReturnBy { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Reference No")]
        public string ReferenceNo { get; set; }
        public bool Active { get; set; }


        public virtual ICollection<PurchaseReturnDetailModel> PurchaseReturnDetails { get; set; }

        //---------------------- Extended properties--------------------
        [Required]
        public string SupplierName { get; set; }

        public int SupplierAccoutHeadId { get; set; }
        [Required]
        [DisplayName("Return Person")]
        public string ReturnPersonName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<PurchaseReturnModel> DataList { get; set; }
    }


    public class PurchaseReturnVm
    {
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
        public long PurchaseReturnId { get; set; }
        public int CompanyId { get; set; }
        public string ProductType { get; set; }
        public string ReturnNo { get; set; }
        public string SupplierName { get;set; }
        public string ReturnPersonName { get; set; }
        public int SupplierId { get; set; }
        public bool Active { get; set; }
        public List<PurchaseReturnVm> DataList { get; set; }
        public Nullable<System.DateTime> ReturnDate { get; set; }
        public string ReturnReason { get; set; }

    }






}
