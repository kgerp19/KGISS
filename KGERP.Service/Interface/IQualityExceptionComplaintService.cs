using KGERP.Service.CommonModels.Model;
using KGERP.Service.Implementation.QualityExceptionComplaints;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IQualityExceptionComplaintService
    {
        Task<RResult> AddQualityExceptionItem(int DropDownTypeId, string ExceptionText, int? CompanyId = null);
        Task<QualityExceptionComplaintDetailVM> GetAllDataOfQualityExceptionComplaint(long QualityExceptionComplaintId = 0, long QualityExceptionComplaintDetailId = 0,CancellationToken cancellationToken = default);
        Task<QualityExceptionComplaintDetailVM> GetDataOfOrderDeliveryDetailsByOrderDeliveryDetailId(long OrderDeliveryDetailsId, CancellationToken cancellationToken = default);
        Task<QualityExceptionComplaintRM> SaveAndUpdateDataQualityExceptionMasterChildAndMapTable(QualityExceptionComplaintDetailVM detailVM, CancellationToken cancellationToken = default);
        Task<QualityExceptionComplaintRM> QualityExceptionComplaintDetailsSubmit(QualityExceptionComplaintDetailVM detailVM, CancellationToken cancellationToken = default);
        Task<QualityExceptionComplaintRM> ProductionInChargeSubmition(QualityExceptionComplaintDetailVM detailVM, CancellationToken cancellationToken = default);
        Task<QualityExceptionComplaintDetailVM> GetQualityExceptionComplaintList(DateTime? fromDate,DateTime? endDate);
        Task<RResult> QualityExceptionComplaintDelete(long qualityExceptionComplaintId,CancellationToken cancellationToken=default);
    }
}
