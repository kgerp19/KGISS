using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class WorkAssignFileModel
    {
        public long WorkAssignFileId { get; set; }
        public long WorkAssignId { get; set; }
        [DisplayName("File Name")]
        public string FileName { get; set; }
        [DisplayName("File Status")]
        public string FileStatus { get; set; }
    }
}
