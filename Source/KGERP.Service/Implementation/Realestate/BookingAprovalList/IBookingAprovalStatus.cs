using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate.BookingAprovalList
{
  public interface IBookingAprovalStatus
    {
        Task<GLDLBookingViewModel> BookingAprovalDraft(int status,int companyId);
        Task<GLDLBookingViewModel> BookingAprovalRecheck(int status,int companyId);
        Task<GLDLBookingViewModel> BookingAprovalApprove(int status, int companyId);
        Task<GLDLBookingViewModel> MdAprovalApprove(int status, int companyId);
        Task<GLDLBookingViewModel> DMdAprovalApprove(int status, int companyId);
        Task<GLDLBookingViewModel> BookingStatusChange(GLDLBookingViewModel model);
        Task<GLDLBookingViewModel> BookingforDealingOfficer(long EmployeeId, int companyId);
        Task<GLDLBookingViewModel> BookingforTeamLead(long EmployeeId, int companyId);
    }
}
