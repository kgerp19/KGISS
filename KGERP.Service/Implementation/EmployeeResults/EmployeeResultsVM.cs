using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.EmployeeResults
{
    public class SignatoryApprovalVM
    {
        public long SigAppMapID { get; set; }
        public long? SigIndID { get; set; }
        public string SignatoryName { get; set; }
        public string DesignationName { get; set; }
        public string SignatoryCompany { get; set; }
        public string SignatoryDepartment { get; set; }
        public int Status { get; set; }
        public int OrderBy { get; set; }
        public string SigApprovalStatus { get; set; }
        public long UserId { get; set; }
        public string Comment { get; set; }
        public string KgID { get; set; }
        public System.DateTime? ApproveDate { get; set; }
    }

    public class SignatoryApprovalMapVM
    {
        public string SignatoryApprovalName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmpDigName { get; set; }
        public int Status { get; set; }
        public int OrderBy { get; set; }
        public System.DateTime? CreatedDate { get; set; }
        public string CreatedDateFormatted => CreatedDate==null?"----------": CreatedDate.Value.ToString("dd-MMM-yyyy");
    }

    public class AnnualPerformanceEmpResultVM: SignatoryApprovalVM
    {
        public long AnnualPerformanceId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public long? AnnualPerformanceDetailId { get; set; }
        public long? EmployeeId { get; set; }
        public string APRNO { get; set; }
        public System.DateTime? CreatedDate { get; set; }
        public DateTime AssessmentFrom { get; set; }
        public string AssessmentFromMsg { get { return AssessmentFrom.ToString("dd-MMM-yyyy"); } }
        public DateTime AssessmentTo { get; set; }
        public DateTime AprEndingDate { get; set; }
        public string AssessmentToMsg { get { return AssessmentTo.ToString("dd-MMM-yyyy"); } }
        [AllowHtml]
        public string Remarks { get; set; }
        public bool IsSubmited { get; set; }
        public List<AnnualPerformanceEmpResultVM> GetDDLAnnualPerformanceEmpResult { get; set; }
        public List<SignatoryApprovalVM> signatoryApprovalList { get; set; }
        public EmployeeInfo employeeInfo { get; set; }
    }
    public class EmployeeResultsVM: AnnualPerformanceEmpResultVM
    {
        public DateTime APREmpCreateDate { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public string ContractNumber { get; set; }
        public int EmpRsultStatus { get; set; }
        public string EmployeeCode { get; set; }
        [AllowHtml]
        public string PerformanceInReportingYear { get; set; }
        [AllowHtml]
        public string PlanforNextYear { get; set; }
        [AllowHtml]
        public string AdditionalPerformance { get; set; }
        [AllowHtml]
        public string performingPlanForNextYear { get; set; }
        public string MajorWorkOne { get; set; }
        public string MajorWorkTwo { get; set; }
        public string MajorWorkThree { get; set; }
        public string MajorWorkFour { get; set; }
        public decimal? WorkWithReportingOfficer { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string ManagerName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string AppointmentDateMsg => AppointmentDate?.ToString("dd-MMM-yyyy") ?? string.Empty;
        public string DateOfBirthMsg => DateOfBirth?.ToString("dd-MMM-yyyy") ?? string.Empty;
        public string AsseFromDate { get; set; }
        public string AsseToDate { get; set; }
        public string MaritalStatus { get; set; }
        public string GradeName { get; set; }
        public int? EmployeeType { get; set; }
        public string ActivityDescription { get; set; }
        public int Recommendation { get; set; }
        public decimal SalesTarget { get; set; }
        public decimal? SalesAchievement { get; set; }
        public decimal RecoveryTarget { get; set; }
        public decimal? RecoveryAchievement { get; set; }
        public int IncreaseProMonth { get; set; }
        [AllowHtml]
        public string FinalCommentOfManagement { get; set; }
        [AllowHtml]
        public string RemarksOfMdOrCeo { get; set; }
        [AllowHtml]
        public string RemarksOfReportingOfficer { get; set; }
        [AllowHtml]
        public string GeneralSpecialRemarks { get; set; }
        public string HOdAdminHr { get; set; }
        public List<EmployeeEqucationList> EmployeeEqucationList { get; set; }
        public List<ProfileQualitiesList> ProfileQualitiesList { get; set; }
        public List<SelectModel> Companies { get; set; }
        public List<SelectModel> DDLDepartments { get; set; }
        public List<EmployeeResultsVM> employeeResultsList { get; set; }
        public IEnumerable<EmployeeResultsVM> DataList { get; set; }
        public List<SelectListItem> DDLRecommendation { get; set; }
        public List<CustomRecommendationList> CustomRecommendationList { get; set; }
        public List<Recommendation> DDLRecommendationModel { get; set; }
        public string CreatedBy { get;  set; }
        public int? ServiceTypeId { get; set; }
        public int? EmployeeServiceType { get; set; }
        public decimal? FinalMarkings { get; set; }
        public int AssessmentOfTheApplicant { get; set; }
        
    }

    public class EmployeeEqucationList
    {
        public int SubjectDropDownTypeId { get; set; }
        public int ExaminationDropDownTypeId { get; set; }
        public int InstituteDropDownTypeId { get; set; }
        public int EducationId { get; set; }
        public string Qualification { get; set; }
        public string Subject { get; set; }
        public string Result { get; set; }
        public string PassedYear { get; set; }
        public string Institute { get; set; }
    }

    public class ProfileQualitiesList
    {
        public string ProfileQualitiesGroupName { get; set; }
        public string ProfileQualitiesName { get; set; }
        public int DropDownTypeId { get; set; }
        public int DropDownItemId { get; set; }
        public int? DropDownItemIdAPR { get; set; }
        public int? ScoreApr { get; set; }
        public string Institute { get; set; }
        public int ItemValue { get; set; }
    }

    public class Recommendation
    {
        public string RecommendationName { get; set; }
        public int DropDownItemId { get; set; }
        public int? DropDownTypeId { get; set; }
        public int? DropDownItemIdEmpApr { get; set; }
        public int? IncreaseProMonth { get; set; }
    }


    public class EmpAttendanceStatus
    {
        public string EmpStatus { get; set; }
        public int StatusCount { get; set; }
        public decimal MarkingEmpAttendance { get; set; }
        public decimal YourAttendanceMark { get; set; }
    }

    public class EmpLeaveCategory
    {
        public int LeaveCategoryId { get; set; }
        public string LeaveCategory { get; set; }
        public decimal TotalLeaveDays { get; set; }
        public decimal GetLeavs { get; set; }
        public decimal Marking { get; set; }
        public decimal YourMark { get; set; }
    }

    public class EmpDisciplineStatus
    {
        public int LogTypeId { get; set; }
        public string EmployeeDisciplineStatus { get; set; }
        public int LogCount { get; set; }
        public int Marking { get; set; }
        public decimal YourMark { get; set; }
    }

    public class EmployeeMarks
    {
        public List<EmpAttendanceStatus> EmpAttendanceStatus { get; set; }
        public List<EmpLeaveCategory> EmpLeaveCategory { get; set; }
        public List<EmpDisciplineStatus> EmpDisciplineStatus { get; set; }

        public decimal AttendanceMark { get; set; }
        public decimal DisciplineMarks { get; set; }
        public decimal TotalEmployeePerformanceMark { get; set; }
    }

    public class CustomRecommendationList:SelectListItem
    {
        public int? DropDownTypeId { get; set; }
    }

    public class EmployeeInfo
    {
        public string EmployeeNameSrt { get; set; }
        public string EmployeeCodeSrt { get; set; }
    }
}
