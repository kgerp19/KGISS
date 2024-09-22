using KGERP.Data.Models;
using KGERP.Service;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;


namespace KGERP.Service.JobService
{
    public class JobDescriptionService : IJobDescription
    {
        ERPEntities context = new ERPEntities();
        public async Task<long> AddJobDescription(JobDescriptionModel model)
        {
            JobDescription jobDescription = new JobDescription();
            jobDescription.JobDescriptionTypeId = model.JobDescriptionTypeId;
            jobDescription.EmployeeId = model.EmployeeId;
            jobDescription.Description = model.Description;
            jobDescription.IsActive = true;
            jobDescription.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            jobDescription.CreateDate = DateTime.Now;
            context.JobDescriptions.Add(jobDescription);
            var res = await context.SaveChangesAsync();
            if (res > 0)
            {
                return jobDescription.JobDescriptionId;
            }
            return 0;
        }

        public async Task<JobDescriptionModel> JobDescriptionList(long employeeId)
        {
            JobDescriptionModel pRoll = new JobDescriptionModel();
            pRoll = await Task.Run(() => (from t1 in context.Employees
                                          join t2 in context.Designations on t1.DesignationId equals t2.DesignationId
                                          join t3 in context.Departments on t1.DepartmentId equals t3.DepartmentId
                                          where t1.Id == employeeId
                                          select new JobDescriptionModel
                                          {
                                              EmployeeId = t1.Id,
                                              KGEmployeeId = t1.EmployeeId,
                                              EmployeeName = t1.Name,
                                              DesignationName = t2.Name,
                                              DepartmentName = t3.Name
                                          }).FirstOrDefault());
            pRoll.DataList = await Task.Run(() => (from t1 in context.JobDescriptions
                                                   join t2 in context.JobDescriptionTypes on t1.JobDescriptionTypeId equals t2.JobDescriptionTypeId

                                                   where t1.EmployeeId == employeeId
                                                   select new JobDescriptionModel
                                                   {
                                                       DescriptionTypeName = t2.Name,
                                                       Description = t1.Description,
                                                       JobDescriptionId = t1.JobDescriptionId,
                                                       IsActive = t1.IsActive
                                                   }).OrderByDescending(f => f.JobDescriptionId).ThenBy(x => x.IsActive).AsEnumerable());
            return pRoll;
        }

       

        public List<object> GetJobDescriptionTypes()
        {
            var List = new List<object>();
            context.JobDescriptionTypes
         .Where(x => x.IsActive).Select(x => x).ToList()
        .ForEach(x => List.Add(new
        {
            Value = x.JobDescriptionTypeId,
            Text = x.Name
        }));
            return List;

        }

        //public async Task<long> Delete(long id)
        //{
        //    var result = await context.PRoll_CashPayment.FirstOrDefaultAsync(f => f.CashPaymentId == id);
        //    result.IsActive = false;
        //    context.Entry(result).State = EntityState.Modified;
        //    context.SaveChanges();
        //    return 1;
        //}

        //public Task<PRoll_CashPaymentViewModel> Detalis(long id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
