using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Leave.ViewModels
{
    public class LeaveAllDetailVM
    {
        public long LeaveApplicationID { get; set; }
        public long EmployeeID { get; set; }
        public long SigID { get; set; }
        public string EmployeeKGid { get; set; }
        public long SignatoryID { get; set; }
        public string ApplicantName { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public System.DateTime ApplyDate { get; set; }
        public string Reason { get; set; }
        public int LeaveDays { get; set; }
        public string LeaveCategory { get; set; }
        public int Status { get; set; }
        public int ApprovalStatus { get; set; }
        public int? CompanyID { get; set; }
        public List<LeaveAllDetailVM> LeaveApprovalDataList { get; set; }
        public List<SignatoryApprovalDetails> SignatoriesApprovalList { get; set; }
        public int LeaveStatus { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TableName { get; set; }
    }
    public class SignatoryApprovalDetails
    {
        public long SigAppMapID { get; set; }
        public long? SigIndID { get; set; }
        public string SigName { get; set; }
        public string SigDesignation { get; set; }
        public int ApprovalStatus { get; set; }
        public string SigApprovalStatus { get; set; }
        public int OrderBy { get; set; }
        public long UserId { get; set; }
        public string KgID { get; set; }
        public System.DateTime? ApproveDate { get; set; }
        public List<SignatoryApprovalDetails> SignatoryDataList { get; set; }
    }
    public class LeaveAllDetailVMM
    {
        public long LeaveApplicationID { get; set; }
        public long EmployeeID { get; set; }
        public long SigID { get; set; }
        public string EmployeeKGid { get; set; }
        public long SignatoryID { get; set; }
        public string ApplicantName { get; set; }
        public System.DateTime? StartDate { get; set; }
        public System.DateTime? EndDate { get; set; }
        public System.DateTime? ApplyDate { get; set; }
        public string Reason { get; set; }
        public int? LeaveDays { get; set; }
        public string LeaveCategory { get; set; }
        public int Status { get; set; }
        public int ApprovalStatus { get; set; }
        public int? CompanyID { get; set; }
        public List<LeaveAllDetailVMM> LeaveApprovalDataList { get; set; }
        public List<SignatoryApprovalDetails> SignatoriesApprovalList { get; set; }
        public int LeaveStatus { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TableName { get; set; }
    }
}
