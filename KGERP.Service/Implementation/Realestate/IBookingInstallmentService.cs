using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate
{
    public interface IBookingInstallmentService
    {
        Task<GeneratedInstallmentSchedule>  GenerateInstallmentSchedule(int companyId, int installmentTypeId, int NoOfInstallment, decimal PayableAmount, DateTime BookingDate);
        Task<GeneratedInstallmentSchedule>  GolobalInstallmentSchedule(int companyId, int installmentTypeId, int NoOfInstallment, decimal PayableAmount, DateTime BookingDate);
        Task<bool> SaveInstallmentSchedule(long CGId, long BookingId, List<InstallmentScheduleShortModel> List);
    }
}
