using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class ComplainManagementModel
    {
        public int ComplainId { get; set; }
        [DisplayName("Complain Type")]
        public Nullable<int> ComplainTypeId { get; set; }
        [DisplayName("Invoice No")]
        public string InvoiceNo { get; set; }
        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }
        public string Address { get; set; }
        [DisplayName("Mobile No")]
        public string MobileNo { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> Qty { get; set; }
        [DisplayName("Complain Description")]
        public string ComplainDescription { get; set; }
        [DisplayName("Action Description")]
        public string ActionDescription { get; set; }
        [DisplayName("Invoice Date")]
        public Nullable<System.DateTime> InvoiceDate { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> ManagerAction { get; set; }
        [DisplayName("Is Closed")]
        public bool IsComplainClosed { get; set; }

        public virtual ComplainTypeModel ComplainType { get; set; }

        public string ActionTakerBy { get; set; }
        public Nullable<System.DateTime> ActionTakerDate { get; set; }
        public string ActionTakenBy { get; set; }
        public Nullable<System.DateTime> ActionTakenDate { get; set; }
        public Nullable<System.DateTime> ActionModifiedDate { get; set; }
        public Nullable<int> IsActionTaked { get; set; }

        public string DeleverdBy { get; set; }
        [DisplayName("Assign To")]
        public string ActionAssignTo { get; set; }
        [DisplayName("Solution")]
        public string SolvingDescription { get; set; }
        [DisplayName("Closing Remarks")]
        public string Remarks { get; set; }
        public bool IsComplainSolved { get; set; }


        //.....Extra.....

        public string ComplainTypeName { get; set; }
        public string OrderDate { get; set; }
        public string ComplainDate { get; set; }
        public string ComplainStatus { get; set; }

    }
}
