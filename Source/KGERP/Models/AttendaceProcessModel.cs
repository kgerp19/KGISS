using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace KGERP.Models
{
    public class AttendaceProcessModel
    {
        [DisplayName("Upload File")]
        public string FilePath { get; set; }
        public HttpPostedFileBase TextFile { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public Nullable<DateTime> AttendenceDate { get; set; }
    }
}