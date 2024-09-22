using KGERP.CustomModel;
using KGERP.Data.CustomModel;
using KGERP.Service.Implementation.Leave.ViewModels;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.AttendanceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IAttendenceService : IDisposable
    {
        List<AttendenceEntity> GetDailyAttendence(DateTime date);
        Task<AttendanceVm> GetSelfAttendance(DateTime? fromDate, DateTime? toDate);
        Task<AttendanceVm> GetDailyAttendanceTeamWise( long managerId, DateTime? fromDate, DateTime? toDate);
        Task<AttendanceVm> GetDailyAttendanceOfTeam(long managerId, DateTime? fromDate, DateTime? toDate);
        Task<AttendenceApproval> GetPersonalAttendenceStatus(long id);
        Task<AttendenceApproval> GetPersonalAttendenceOnFieldTour(long id);
        AttendenceApproveApplicationModel GetAttendenceApprovalStatus(int id);
        List<AttendenceApprovalAction> AttendenceApprovalAction(long managerId);

        List<AttendenceApprovalAction> HrAttendenceApprovalAction(long hrAdminId);
        List<InTimeOutTime> GetTime(string empId, DateTime date);
        bool ApprovalAction(int id, string comments);
        bool HrApprovalAction(int id, string comments);
        bool DeniedAction(int id, string comments);
        bool HrDeniedAction(int id, string comments);
        string GetEmpId(int? id);
        bool SaveRequest(long id, AttendenceApproveApplicationModel approvalReq);
        bool PrecessAttendenceInFinalTable(DateTime attendenceDate);
        List<AttendenceEntity> GetEmployeeAttendence(string FromDate, string ToDate, string EmployeeId, int? DepartmentId);
        Task<AttendenceSummeries> MonthlyAttendanceSummery(DateTime? fromDate, DateTime? toDate);
        List<CalculativeAttendanceVM> GetCalculativeAttendance(int? companyId, int year, int month);
        bool excuteattendance(string date);
        bool excutemachinedata(string date);

        #region New Field or Tour
        Task<AttendenceApproval> GetOnFieldOrTourListByEmployee(long EmpID, DateTime? fromDate, DateTime? toDate);
        Task<SignatoryApprovalDetails> GetSignatoriesApprovalInfo(long applicationId, long sigId);
        int DoLeaveApproval(LeaveAllDetailVM vm);
        Task<LeaveAllDetailVMM> GetLeaveApprovalRelatedAllInfo(long SigId, DateTime? fromDate, DateTime? toDate);
        #endregion

        //List<AttendenceEntity> GetEmployeeAttendence();
    }
}
