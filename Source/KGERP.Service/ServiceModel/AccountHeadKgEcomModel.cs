using System;

namespace KGERP.Service.ServiceModel
{
    public class AccountHeadKgEcomModel
    {
        public long OID { get; set; }
        public string Actcode { get; set; }
        public string Actname { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
