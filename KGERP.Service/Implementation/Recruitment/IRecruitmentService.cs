using KGERP.Data.Models;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Service.Implementation.Recruitment.ViewModels;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Recruitment
{
    public interface IRecruitmentService
    {
        MethodFeedbackVM Add(RecruitmentVM model);
        MethodFeedbackVM Update(RecruitmentVM model);
        MethodFeedbackVM UpdateStatus(int Id, RecruitmentStatusEnum status,out RecruitmentRequisition model);
        MethodFeedbackVM Remove(int id, out RecruitmentRequisition model);
        MethodFeedbackVM AddRecruitmentItemDetail(RecruitmentVM model);
        MethodFeedbackVM UpdateRecruitmentItemDetail(RecruitmentVM modelVM);
        RecruitmentDetailsVM GetRequisitionItemDetailById(int id);
        MethodFeedbackVM RemoveRequisitionDetials(int id, out RecruitmentRequisitionDetail model);
        IEnumerable<RecruitmentVM> GetRequisitionList(RecruitmentStatusEnum? status, DateTime? fromDate, DateTime? toDate, int? companyId = 0);

        RecruitmentVM GetById(int id);
        List<RecruitmentApprovalVM> GetApprovalList(SignatoryStatusEnum? SignatoryStatus, DateTime? fromDate, DateTime? toDate);
        List<RequisitionApprovalVM> GetRecruitmentSignatoryList(int id);
        bool UpdateRecruitmentSignatoryApprovalStatus(long id, SignatoryStatusEnum status, string comment);
        string GetRecruitmentNumber();
    }
}
