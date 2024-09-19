using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate.BookingCollaction
{
  public  interface IBookingCollaction
    {

        Task<CollactionBillViewModel> CustomerBookingView(CollactionBillViewModel model);
        Task<CollactionBillViewModel> CollactionBookingView(int companyId, int paymentMasterId, long CGId);
        Task<CollactionBillViewModel> CollactionDelete(CollactionBillViewModel model);
        Task<CollactionBillViewModel> CollactionUpdate(CollactionBillViewModel model);
        Task<long> CollactionStatusUpdate(long id);
        object GetBookingList(int companyId,string prefix);
       
        Task<CollactionBillViewModel> NewCollaction(CollactionBillViewModel model);
        Task<CollactionBillViewModel> CollactionItemDelete(CollactionBillViewModel model);
        Task<CollactionBillViewModel> CollactionList(int companyId);
    }
}
