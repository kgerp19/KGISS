using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.KfmlInstallments
{
    public interface IKfml_Installment
    {
        Task<IKfml_InstallmentViewModel> AddInstallment(IKfml_InstallmentViewModel model);
        Task<IKfml_InstallmentViewModel> InstallmentList(int CompanyId);
        Task<IKfml_InstallmentViewModel> CustomerBookingView(int companyId, long bookingId);
        Task<long> AccountingSalesPushKFML(int CompanyFK, IKfml_InstallmentViewModel bookingVM, int journalType);
    }
}
