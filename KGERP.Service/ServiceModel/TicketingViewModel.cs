using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel
{
   public class TicketingViewModel
    {
        public long Id { set; get; }
        public long EmployeeId { set; get; }
        public string TaskNo { set; get; }
        public string StatusName { set; get; }

        [Required]
        public string Subject { set; get; }
        [Required]
        public string Description { set; get; }
        public string DesignationName { set; get; }
        public int CompanyId { set; get; }
        public int? CompanyIdFK { set; get; }
        public string CompanyName { set; get; }

        public int? TaskType { set; get; }
        public int Status { set; get; }
        public string CreatedBy { set; get; }
        public string EmpName { set; get; }
        public string ModifyBy { set; get; }
        public DateTime CreatedDate { set; get; }
        public string Date { set; get; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public DateTime ModifyDate { set; get; }
        public IEnumerable<TicketingViewModel> DataList { get; set; }
        public List<SelectModel> Companies { get; set; }

        public SelectList TaskTypeList { get { return new SelectList(BaseFunctionalities.GetEnumList<TaskType>(), "Value", "Text"); } }
        public SelectList TicketingStatuslist { get { return new SelectList(BaseFunctionalities.GetEnumList<TicketingStatus>(), "Value", "Text"); } }


    }


}
