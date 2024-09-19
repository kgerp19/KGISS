using KGERP.Service.CommonModels.Model;
using KGERP.Service.Implementation.EmployeeResults;
using KGERP.Service.Implementation.Leave.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IEmployeeResultService
    {
        Task<EmployeeResultsVM> GetEmployeeDataByEmpId(long EmpId, long AnnualPerformanceId,long annualPerformanceDetailId=0, CancellationToken cancellationToken = default);
        Task<RResult> DeleteEducationById(int EduId, CancellationToken cancellationToken = default);
        Task<RResult> ProfileQualitiesReportSubmit(EmployeeResultsVM dataModel, CancellationToken cancellationToken = default);
        Task<RResult> EducationContentSave(int DropDownTypeId, string Namek, CancellationToken cancellationToken = default);
        Task<long> AnnualPerformanceEmployeeResultSaveAndUpdate(AnnualPerformanceEmpResultVM model, CancellationToken cancellationToken = default);
        Task<AnnualPerformanceEmpResultVM> AnnualPerformanceEmployeeResult(long AnnualPerformanceId, CancellationToken cancellationToken = default);
        Task<AnnualPerformanceEmpResultVM> GetAnnualPerformanceEmployeeResultList(long EmpId=0, CancellationToken cancellationToken = default);
        Task<RResult> GetAnnualPerformanceEmployeeResultDelete(long AnnualPerformanceId, CancellationToken cancellationToken = default);
        Task<RResult> AnnualPerformanceDetailSaveUpdate(EmployeeResultsVM model, CancellationToken cancellationToken = default);
        //Task<AnnualPerformanceEmpResultVM> AnnualPerformanceSubmit(EmployeeResultsVM model, CancellationToken cancellationToken = default);
        Task<AnnualPerformanceEmpResultVM> AnnualPerformanceSubmitAsync(EmployeeResultsVM model, CancellationToken cancellationToken = default);
        Task<EmployeeResultsVM> GetAnnualPerformanceAllInfo(long SigId, DateTime? fromDate, DateTime? toDate);
        Task<EmployeeResultsVM> GetAnnualPerformanceDetailsById( long annualPerformanceId, CancellationToken cancellationToken=default);
        Task<long> SignatoryApprovalSubmission(EmployeeResultsVM dataModel, CancellationToken cancellationToken = default);
        Task<List<SignatoryApprovalMapVM>> GetSignatoryApprovalList(long AnnualPerformanceDetailId, CancellationToken cancellationToken = default);
        Task<EmployeeResultsVM> GetEmployeeResultAllInfo(long AnnualPerformanceId, long EmpId,  long annualPerformanceDetailId, CancellationToken cancellationToken = default);
        EmployeeMarks GetEmployeeMarks(long employeeId, string employeeCode, DateTime startDate, DateTime endDate);
        Task<EmployeeInfo> GetEmployeeInfo(long EmpId,CancellationToken cancellationToken = default);
    }
}
