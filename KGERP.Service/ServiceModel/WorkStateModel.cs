using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class WorkStateModel
    {
        public string ButtonName
        {
            get
            {
                return WorkStateId > 0 ? "Update" : "Create";
            }
        }
        public int WorkStateId { get; set; }
        public string State { get; set; }
        public string Remarks { get; set; }
        public int OrderNo { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        public virtual WorkModel Work { get; set; }
    }
}
