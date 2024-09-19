using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class MonthlyTargetModel
    {
        public string ButtonName
        {
            get
            {
                return MonthlyTargetId > 0 ? "Update" : "Save";
            }
        }
        public int VendorType { get; set; }
        public int MonthlyTargetId { get; set; }
        [DisplayName("Company")]
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Customer")]
        public Nullable<int> CustomerId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
        public Nullable<decimal> CustAmount { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        public virtual CompanyModel Company { get; set; }

        //-------------Extended Properties-------------
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public string MonthName { get; set; }
    }
}
