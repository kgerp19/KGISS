using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_AttendanceLog_Service
{
    public interface IPRoll_AttendanceLogService
    {
        Task<long> AddNew(PRoll_AttendanceLogViewModel model);
        Task<PRoll_AttendanceLogViewModel> AttendanceList(int companyId);
        Task<long> Delete(long id);
        Task<PRoll_AttendanceLogViewModel> Details(int companyid,long id);
        Task<PRoll_AttendanceLogViewModel> DetailsEdit(PRoll_AttendanceLogViewModel model);
        Task<long> AttendanceProcessPayroll(long id);
    }
}
