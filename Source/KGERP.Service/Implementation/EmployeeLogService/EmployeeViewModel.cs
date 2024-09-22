using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.EmployeeLogService
{
    public class EmployeeViewModel
    {
        public long Id { get; set; }
        public long EmpId { get; set; }
        public string EmpName { get; set; }
        public int logtype { get; set; }
        public string logtypeName { get; set; }

        [AllowHtml]
        public string Description { get; set; }
        public string Colorcode { get; set; }
        public DateTime logdate { get; set; }
        public string stringlogdate { get; set; }
        //public List<EmployeeLogType> EmployeeLogTypes { get; set; }
        public SelectList EmployeeLogTypes { get; set; } = new SelectList(new List<SelectModel>());
        public List<EmployeeViewModel> models { get; set; }
        public List<HttpPostedFileBase> Attachments { get; set; }
        public List<GLDLBookingAttachment> Attachments2 { get; set; }
    }
}
