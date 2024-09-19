namespace KGERP.Service.ServiceModel
{
    using System;

    public class WorkQAFileModel
    {
        public long WorkQAFileId { get; set; }
        public Nullable<long> WorkQAId { get; set; }
        public Nullable<long> EmpId { get; set; }
        public string FileName { get; set; }
    }
}
