using System;

namespace KGERP.Service.ServiceModel
{
    public class HeadGLModel
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public string AccCode { get; set; }
        public string AccName { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public Nullable<int> LayerNo { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public virtual Head5Model Head5 { get; set; }
    }
}
