using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate.CustomersBooking
{
   public interface ICustomerBookingService
    {
        Task<GLDLBookingViewModel> CustomerBookingView(int companyId, long CGId);
        Task<CollactionBillViewModel> CustomerInstallmentSchedule(int companyId, long CGId);
        Task<int> UpdateInsatllment(InstallmentScheduleShortModel model);
        List<SceduleInstallment> GetByInstallmentSchedule(int companyId, long CGId);
        Task<GLDLBookingViewModel> CustomerBookingList(int companyId, DateTime? fromDate, DateTime? toDate, int? ProductCategoryId);
        Task<GLDLBookingViewModel> SPCustomerBookingList(int companyId, DateTime? fromDate, DateTime? toDate, int? ProductCategoryId);
        Task<GLDLBookingViewModel> CustomerBookingListInactive(int companyId, DateTime? fromDate, DateTime? toDate, int? ProductCategoryId);
        Task<GLDLBookingViewModel> ReSaleBookingList(int companyId);
        Task<BookingInstallmentSchedule> InstallmentScheduleById(long id);
        Task<InstallmentScheduleShortModel> getInstallmetClient(long id);
        Task<GLDLBookingViewModel> SaveNewInstallMent(GLDLBookingViewModel model);
        Task<bool> deleteInstallment(int InstallmentId);
        Task<List<SelectModelType>> PRMRelation();
        Task<long> SubmitBooking(GLDLBookingViewModel bookingViewModel);
        Task<long> SubmitStatusChange(GLDLBookingViewModel bookingViewModel);
        Task<bool> DeleteProductBookingInfo(long CGID);
         Task<bool> UndoDeleteProductBookingInfo(long CGID);

    }
}
