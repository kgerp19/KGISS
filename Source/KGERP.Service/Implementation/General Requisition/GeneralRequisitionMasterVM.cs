using KGERP.Service.Configuration;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.General_Requisition
{
    public class GeneralRequisitionMasterVM
    {
        public long Id { get; set; }
        public string RequisitionNumber { get; set; }
        public DateTime RequisitionDate { get; set; }
        [DisplayName("Requisition To")]
        [Required(ErrorMessage = "This field is required.")]
        public BusinessTypeEnum GeneralRequisitionType { get; set; }
        public RequisitionAssetTypeEnum RequisitionAssetType { get; set; }
        public bool IsAsset { get; set; }
        public IEnumerable<VMCommonUnit> UnitList { get; set; }
        //company or divisionId
        [Required(ErrorMessage ="This field is required.")]
        public int CommonId { get; set; }
        public int UnitId { get; set; }

        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int UserCompanyId { get; set; }
        public string UserCompanyName { get; set; }


        //public int RequisitionFromCompanyId { get; set; }
        //public string RequisitionFromCompanyName { get; set; }
        public int? DivisionId { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
     
        public string DivisionName { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Category Name")]
        public int CategoryId { get; set; }
        [DisplayName("Category Name")]
        public string CategoryName { get; set; }
        public long EemployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int? RequisitionToCompanyId { get; set; }
        public string Remarks { get; set; }
        public int PriorityLavel { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? Todate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public decimal RequisitionTotalAmount { get; set; }
        public decimal AITParcent { get; set; }
        public decimal DiscountParcent { get; set; }
        public decimal DiscountProductParcent { get; set; }
        public IList<GeneralRequisitionMasterDetailsVM> RequisitionProductList { get; set; }
        public List<GeneralRequisitionMasterVM> DataList { get; set; }
        public IList<RequisitionApprovalVM> RequisitionApprovalLIst { get; set; }
        public GeneralRequisitionStatusEnum Status { get; set; }
        public long RequisitionItemId { get; set; }
        public int  ProductId { get; set; }
        public string ProductName { get;set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string UnitName { get; set; }
        public bool PreviousSignatoryStatus { get; set; }
        public SignatoryStatusEnum RequisitionSignatoryStatus { get; set; }
        public long RequisitionSignatoryId { get; set; }
    }
}
