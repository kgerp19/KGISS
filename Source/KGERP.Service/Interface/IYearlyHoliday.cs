using KGERP.Service.ServiceModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IYearlyHoliday
    {
        Task<YearlyHolidayModel> GetYearlyHolidayEvent();
    }
}
