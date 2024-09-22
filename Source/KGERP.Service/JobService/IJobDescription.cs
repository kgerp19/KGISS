using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.JobService
{
    public interface IJobDescription
    {
        Task<long> AddJobDescription(JobDescriptionModel model);
        Task<JobDescriptionModel> JobDescriptionList(long employeeId);
        List<object> GetJobDescriptionTypes();
        //Task<PRoll_CashPaymentViewModel> Detalis(long id);
        //Task<long> Delete(long id);
    }
}
