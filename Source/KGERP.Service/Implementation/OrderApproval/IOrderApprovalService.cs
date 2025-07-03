using KGERP.Data.Models;
using KGERP.Service.Implementation.OrderApproval.ViewModels;
using KGERP.Services.Procurement;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KGERP.Service.Implementation.OrderApproval
{
    public interface IOrderApprovalService
    {

        Task<bool> AddOrderMasterSignatory(OrderMasterSignatory orderMaster);
        Task<bool> UpdateOrderMasterSignatory(OrderMasterSignatory orderMaster);
        Task<bool> RemoveMasterSignatory(int id);


        Task<IEnumerable<OrderMasterSignatoryVM>> GetAllOrderMasterSignatories(int? companyId);
        Task<OrderMasterSignatoryVM> GetOrderMasterSignatoryById(int orderMastersignatoryid);






        Task<bool> AddSignatoryApproval(ApprovalOrderMaster orderMaster);
        Task<bool> UpdateSignatoryApproval(ApprovalOrderMaster orderMaster);
        Task<bool> RemoveSignatoryApproval(int id);


        Task<IEnumerable<OrderMasterSignatoryVM>> GetAllSignatoryApprovals(int orderMasterId);
        Task<OrderMasterSignatoryVM> GetSignatoryApprovalById(int signatoryApprovalId);



        Task<bool> AddAllMappingSignatoryApproval(int orderMasterId);

        Task<long> UpdateSignatoryApproval(int signatoryApprovalId, SignatoryStatusEnum statusEnum, string comments);


        Task<OrderMasterSignatoryApprovalVM> LoadApprovalData(DateTime? fromDate, DateTime? toDate, SignatoryStatusEnum? status);
        Task<OrderMasterSignatoryApprovalVM> LoadApprovalDataSeed(DateTime? fromDate, DateTime? toDate, SignatoryStatusEnum? status);
        Task<OrderMasterSignatoryApprovalVM> LoadRejectedOrderData(DateTime? fromDate, DateTime? toDate);

        Task<IEnumerable<OrderMasterSignatoryApprovalVM>> GetAllApproval(int orderMasterId);
    }
}
