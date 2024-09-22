using KGERP.Data.Models;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.General_Requisition
{
    public interface IGeneralRequisitionService
    {
        Task<bool> Add(GeneralRequisitionMasterVM generalRequisitionMasterVM);
        Task<bool> Update(GeneralRequisitionMasterVM generalRequisitionMasterVM);
        Task<bool> Remove(int id);
        Task<bool> UpdateStatus(long id,int statusId);
        Task<bool> UpdateRequisitionSignatoryApprovalStatus(long id, int statusId, string comment);
        Task<bool> UpdateEDocumentSignatoryApprovalStatus(long id, EFileSignatoryStatusEnum statusId, string comment);
        Task<GeneralRequisitionMasterVM> Get(int id);
        Task<IEnumerable<GeneralRequisitionMasterVM>> GetAllAsync(int? companyId, DateTime? fromDate, DateTime? toDate,GeneralRequisitionStatusEnum? status);
        IEnumerable<GeneralRequisitionMasterVM> GetAll(int? companyId, DateTime? fromDate, DateTime? toDate);
        IEnumerable<RequisitionApprovalVM> GetAllApproval(int requisitionId);
        IEnumerable<EDocumentApprovalVM> GetAllEDocumentApprovalList(int requisitionId);
        IEnumerable<GeneralRequisitionMasterVM> GetAllRequisitionApprovalList(int? companyId,DateTime? fromDate, DateTime? toDate, long? userId, SignatoryStatusEnum? approvalStatus);
        Task<bool> AddRequisitionItemDetail(GeneralRequisitionMasterVM generalRequisitionMasterVM);
        Task<bool> UpdateRequisitionDetail(GeneralRequisitionMasterVM generalRequisitionMasterVM);
        Task<IEnumerable<GeneralRequisitionMasterVM>> GetAllRequisitionAsync(int? companyId, DateTime? fromDate, DateTime? toDate, int? Status);
        Task<bool> RemoveRequisitionDetail(GeneralRequisitionMasterVM requisition);
        Task<string> GetGeneralRequisitionName(int companyOrDivisionId, BusinessTypeEnum companyType);
        Task<string> GetEFileName();

       

        #region GeneralRequisitionCategory
        Task<bool> AddRequisitionCategory(GeneralRequisitionProductCategoryVM categoryVM);
       Task<bool> UpdateRequisitionCategory(GeneralRequisitionProductCategoryVM categoryVM);
       Task<GeneralRequisitionProductCategoryVM> GetRequisitionCategory(int id);

       Task<IEnumerable<GeneralRequisitionProductCategoryVM>> GetAllRequisitionCategoryAsync();
       IEnumerable<GeneralRequisitionProductCategoryVM> GetAllGeneralRequisition();
       Task<bool> RemoveGeneralRequisitionCategory(int id);
        #endregion GeneralRequisitionCategory

        #region RequisitionSignatory
        Task<bool> AddRequisitionSignatory(RequisitionSignatoryVM model);
        Task<bool> UpdateRequisitionSignatory(RequisitionSignatoryVM model);
       Task<bool> DeleteRequisitionSignatory(RequisitionSignatoryVM model);
       Task<IEnumerable<RequisitionSignatoryVM>> GetAllRequisitionSignatory();
        Task<IEnumerable<RequisitionSignatoryVM>> GetAllRequisitionSignatoryByCompany(int companyId, string integrateWith);
       Task<IEnumerable<RequisitionSignatoryVM>> GetAllRequisitionSignatoryByCategory(int categoryId);
       Task<IEnumerable<RequisitionSignatoryVM>> GetAllRequisitionSignatoryByDivision(int divisionId);
        #endregion RequisitionSignatory

        #region ERequisition
        Task<bool> AddERequisitionItemDetail(ERequisitionVM generalRequisitionMasterVM);
        Task<bool> UpdateERequisitionItemDetail(ERequisitionVM generalRequisitionMasterVM);
        Task<bool> DeleteERequisitionItemDetail(int id);
        Task<bool> AddERequisition(ERequisitionVM generalRequisitionMasterVM);
        Task<bool> UpdateERequisition(ERequisitionVM generalRequisitionMasterVM);
        Task<ERequisitionVM> GetERequistionById(int id);
        Task<bool> ForwardRequisition(ERequisitionVM model);
        Task<bool> DeleteRequisitionSignatory(int signatoryId);
        Task<IEnumerable<ERequisitionVM>> GetAllEDocumentList(int? companyId, DateTime? fromDate, DateTime? toDate, GeneralRequisitionStatusEnum? status);
        Task<IEnumerable<ERequisitionVM>> GetAllEDocumentApprovalList(int? companyId, DateTime? fromDate, DateTime? toDate, long? userId, EFileSignatoryStatusEnum? approvalStatus);
        Task<bool> UpdateEDocumentSignatoryApprovalStatus(long id, int statusId, string comment);
     #endregion ERequisition
    }
}
