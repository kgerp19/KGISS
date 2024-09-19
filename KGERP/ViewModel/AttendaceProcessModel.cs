using System.ComponentModel;
using System.Web;

namespace KGERP.ViewModel
{
    public class AttendaceProcessModel
    {

        [DisplayName("Upload Image")]
        public string FilePath { get; set; }
        public HttpPostedFileBase ExcelFile { get; set; }
    }
}