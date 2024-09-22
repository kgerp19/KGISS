using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class CreditRecoverDetailModel
    {
        public long CreditRecoverDetailId { get; set; }
        public Nullable<long> CreditRecoverId { get; set; }
        [DisplayName("Recover Amount")]
        public decimal RecoverAmount { get; set; }
        [DisplayName("Recover Date")]
        [Required(ErrorMessage = "Recover Date is Required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> RecoverDate { get; set; }
        public string Note { get; set; }

        public virtual CreditRecoverModel CreditRecover { get; set; }
        //-------------Extended Properties

        public string StrRecoverDate { get; set; }
    }
}
