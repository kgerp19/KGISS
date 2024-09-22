using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
    public class PurchaseReturnnewViewModel
    {
        
        public long PurchaseReturnId { get; set; }
        public int CompanyId { get; set; }
        [DisplayName("Product Type")]
        [Required(ErrorMessage = "Please select Product Type.")]
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
        public bool IsSubmited { get; set; }
        public string message { get; set; }
        public string ReturnByName { get; set; }
        public string ReturnById{ get; set; }

        //---------------------- Extended properties--------------------
        [Required]
        public string SupplierName { get; set; }
        public string StockInfoName { get; set; }

        public int? AccoutHeadId { get; set; }
        [Required]
        [DisplayName("Return Person")]
        public string ReturnPersonName { get; set; }


        ///------------------------item ...............
        public IEnumerable<PurchaseReturnDetailViewModel> PurchaseReturnDetailItem { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<PurchaseReturnnewViewModel> ReturnList{ get; set; }
        public List<SelectModel> Stocks { get; set; }
        public List<SelectModel> ProductTypes { get; set; }
        public Nullable<int> ProductId { get; set; }
        [Required(ErrorMessage = "Please select Product.")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Required Quantity.")]
        public Nullable<decimal> Qty { get; set; }
        [Required(ErrorMessage = "Required Rate.")]
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> GrossAmount { get; set; }
        public long PurchaseReturnDetailId { get; set; }
        public string IntegratedFrom { get; set; }
    }

    public class PurchaseReturnDetailViewModel
    {
        public long PurchaseReturnDetailId { get; set; }
        public long PurchaseReturnId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string ProductName { get; set; }
        public string CatagoryName { get; set; }
        public string SubCatagoryName { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> TotalRate { get; set; }
        public decimal? COGS { get; set; }
        public int? AccountingHeadId { get; set; }
        public int? AccountingExpenseHeadId { get; set; }

    }
}
