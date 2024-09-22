using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class CreditRecoverModel
    {
        public long CreditRecoverId { get; set; }
        [DisplayName("Company")]
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Customer")]
        public Nullable<int> VendorId { get; set; }
        [DisplayName("Total Due")]
        public decimal Amount { get; set; }
        [DisplayName("Start Date")]
        [Required(ErrorMessage = "Begin Date is Required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> StartDate { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        //--------------Extended Properties------------

        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public decimal RecoverAmount { get; set; }
        public string StrStartDate { get; set; }
        public string StrLastPaymentDate { get; set; }
    }
}
