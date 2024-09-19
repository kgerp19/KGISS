using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_AttendanceLog_Service
{
    public class PRoll_AttendanceLogViewModel
    {
        public long AttendanceLogId { get; set; }
        public long CompanyId { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int Year { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StrFromDate { get; set; }
        public string CompanyName { get; set; }
        public string StrToDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool? IsFinalize { get; set; }
        public IEnumerable<PRoll_AttendanceLogDetailViewModel> detailViewModels { get; set; }  
        public PRoll_AttendanceLogDetailViewModel detailViewModel { get; set; }  
        public List<PRoll_AttendanceLogViewModel> dataList { get; set; }
        public List<SelectModel> YearsList { get; set; }
        public List<SelectModel> Companies { get; set; }
        public SelectList MonthList { get { return new SelectList(BaseFunctionalities.GetEnumList<MonthList>(), "Value", "Text"); } }
    }
}
