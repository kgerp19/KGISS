using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel.Approval_Process_Model
{
    public class ApprovalSystemViewModel
    {
        public long ReportApprovalId { get; set; }
        public long PayarolllId { get; set; }
        public int ActionId { get; set; }
        public int ReportGroup { get; set; }
        public long ApprovalFor{ get; set; }
        public long ReportApprovalDetalisId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int CompanyId { get; set; }
        public long SectionEmployeeId { get; set; }
        public string CompanyName { get; set; }
        public string ReportName { get; set; }
        public string ValidationSMS { get; set; }
        public long ReportCategoryId { get; set; }
        public string ReportCategoryName { get; set; }
        public int FinalStatus { get; set; }
        public int ApprovalStatus { get; set; }
        public int OrderNo { get; set; }
        public int? FrowardId { get; set; }
        public string ApprovalStatusName { get; set; }
        public string FinalStatusName { get; set; }
        public long EmployeeId{ get; set; }
        public string EmployeeName{ get; set; }
        public string CreatedBy { get; set; }
        public bool Issubmited { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }       
        public DateTime? Approvdate { get; set; }       
        public string StringApprovdate { get; set; }
        public bool IsFestivalBonus { get; set; }
        public List<ApprovalSystemViewModel> datalist { get; set; }
        public List<SelectDDLModel> reportCatagoryList { get; set; }
        public List<SelectModel> YearsList { get; set; }
        public List<SelectModel> Companies { get; set; }
        public SelectList MonthList { get { return new SelectList(BaseFunctionalities.GetEnumList<MonthList>(), "Value", "Text"); } }
    }
}
