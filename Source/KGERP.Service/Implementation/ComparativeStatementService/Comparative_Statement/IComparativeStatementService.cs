using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.ComparativeStatementService.Comparative_Statement
{
    public interface IComparativeStatementService
    {
        ComparativeStatementVm GetCS(int CompanyId);
        string GetCSNO(int CompanyId);
        object GetProductsByCompany(int CompanyId, string prefix, string type);
        object GetSupplier(int CompanyId, string prefix);
        long SaveComparativeStateMentDetails(ComparativeStatementVm Model);
        long SaveComparativeStateMent(ComparativeStatementVm Model);
        ComparativeStatementVm GetComparativeStatement(long? CsId);
        bool EditComparativeStateMentDetails(ComparativeStatementDetailVm Model);
        bool DeleteComparativeStateMentDetails(long csDetailID);
        bool MakeRecomended(long csDetailID);
         bool Submitstatus(ComparativeStatementVm vm);
        ComparativeStatementVm GetForEdit(long? CSID);
        IEnumerable<RequisitionApprovalVM> GetSignatureList(int companyId, DateTime? fromDate, DateTime? toDate, long? userId, SignatoryStatusEnum? approvalStatus);
        IEnumerable<RequisitionApprovalVM> GetAllApproval(int CSID);
         bool Deletecs(long? CSID);
        bool UpdateCSSignatoryApprovalStatus(long id, int statusId, string comment);
    }
}
