
using KGERP.Service.CommonModels.Model;
using KGERP.Service.Configuration;
using KGERP.Service.Contracts.KGERP.Requisitions.Command.RequestResponseModel;
using KGERP.Service.Contracts.KGERP.Requisitions.Queries.RequestModel;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IRequisitionService
    {
        int GetRequisitionNo();
        
        ProductModel GetProcessLossAmount(int productId);
        Task<int> CreateProductionRequisition(RequisitionModel model);
        Task<int> CreateProductionRequisitionItem(RequisitionModel model);

        
        bool DeleteRequisition(int requisitionId);
        List<RequisitionItemModel> GetRequisitionItemIssueStatus(int requisitionId);
        Task<RequisitionModel> GetRequisition(int companyId, int requisitionId);
        RequisitionModel GetRequisitionById(int requisitionId);
       List<RequisitionItemModel> GetRequisitionItems(int requisitionId);
        List<RequisitionItemDetailModel> GetRequisitionItemDetails(int requisitionId);
        List<RequisitionItemDetailModel> GetRequisitionItemDetails(int requisitionId, DateTime deliveryDate);

        int CreateOrEdit(RequisitionModel requisition);
        Task<RequisitionModel> RequisitionList(int companyId, DateTime? fromDate, DateTime? toDate);
        Task<RequisitionModel> RequisitionDeliveryPendingList(int companyId, DateTime? fromDate, DateTime? toDate);
        Task<RequisitionModel> RequisitionIssuePendingList(int companyId, DateTime? fromDate, DateTime? toDate);
        string GetFormulaMessage(int requisitionId);
        List<RequisitionItemModel> GetProductionItems(int companyId,int requisitionId, DateTime issueDate);
        Task<long> EditProductionReqisitionDetail(RequisitionModel model);

       // Task<RResult> RequisitionSaveForPKL(RequisitionMasterDetailsRM model);
        Task<int> PackagingGeneralRequisition(VMPackagingPurchaseRequisition model);
        Task<int> PackagingGeneralRequisitionSubmit(int requisitionId,bool isSubmited=false);
        Task<int> RequisitionItemDetailDeleteConfirm(int RequisitionId,Guid? RequistionItemDetailId);
        Task<int> RequisitionItemDetailUpdate(int requisitionId, decimal RQuantity,int RproductionId, Guid? RequistionItemDetailId);
        int RequisitionItemDetailSave(int requisitionId, decimal RQuantity, int RProductId, int FinishProductBOMId);
        Task<VMPackagingPurchaseRequisition> GetRequisitionForPKLRM(int companyId,int requisition);
        RResult RequisitionDetailsSaveForPKL(RequisitionMasterDetailsRM model);
        Task<RResult> PackagingProductionRequisitionDelete(int RequisitionId);
        Task<PackagingProductionRequisitionDetailsRM> PackagingProductionRequisitionDetails(int RequisitionId,int CompanyId,CancellationToken cancellationToken=default);

        Task<RResult> RequisitionItemDetailDelete(Guid? RequistionItemDetailId);
        Task<int> RequisitionSubmitied(int requisitionId,  CancellationToken cancellationToken=default);

        Task<RResult> UpdateProductAndQuantityInRequisitionItemDetail(int ProductId,decimal Quentity, Guid? RequistionItemDetailId);

        Task<ResuisitionRM> GetResuisitionDataByRequisitionId(int ReqId, CancellationToken cancellationToken = default);
        Task<ResuisitionRM> GetGeneralResuisitionDataByRequisitionId(int ReqId, CancellationToken cancellationToken = default);
      
        Task<RResult> KPLRequisitionUpdate(ResuisitionRM model, CancellationToken cancellationToken = default);
        Task<RResult> KPLRequisitionIssuedDelete(long issuedMaseterId, CancellationToken cancellationToken = default);
        Task<RResult> KPLRequisitionIssueUpdate(VMPackagingPurchaseRequisition model, CancellationToken cancellationToken = default);

        Task<RResult> KPLRequisitionIssuedDetailsDelete(VMPackagingPurchaseRequisition model, CancellationToken cancellationToken = default);
       // void KPLRequisitionIssuedAchknolagementSubmit(VMPackagingPurchaseRequisition model);
    }
}
