using KGERP.Data.Models;
using KGERP.Service.CommonModels.Model;
using KGERP.Service.Configuration;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.Approval_Process_Model;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KGERP.Service.Implementation.EmployeeLogService
{

    public class EmployeeLogServices
    {
        ERPEntities db = new ERPEntities();
        public async Task<List<SelectModel>> EmployeeLogType()
        {
            var list= db.EmployeeLogTypes.ToList().Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.Id
            }).ToList();
          
            return list;
        }

        public async Task<EmployeeViewModel> Addlog(EmployeeViewModel model)
        {
            EmployeeLog log = new EmployeeLog();
            log.LogDate = model.logdate;
            log.EmployeeId = model.EmpId;
            log.LogType = model.logtype;
            log.Description = model.Description;
            log.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            log.CreatedDate = DateTime.Now;
            log.IsActive = true;
            db.EmployeeLogs.Add(log);
            var res = await db.SaveChangesAsync();
            if (res > 0)
            {
                model.Id = log.Id;
            }
            return model;
        }

        public async Task<EmployeeViewModel> Updatelog(EmployeeViewModel model)
        {
            EmployeeLog log = await db.EmployeeLogs.FirstOrDefaultAsync(d => d.Id == model.Id);
            log.LogDate = model.logdate;
            log.EmployeeId = model.EmpId;
            log.LogType = model.logtype;
            log.Description = model.Description;
            log.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            log.ModifiedDate = DateTime.Now;
            db.Entry(log).State = EntityState.Modified;
            var res = await db.SaveChangesAsync();
            if (res > 0)
            {
                model.Id = log.Id;
            }
            return model;
        }

        public async Task<EmployeeViewModel> logList(long EmpId)
        {
            var log = await db.EmployeeLogs.Where(d => d.EmployeeId == EmpId && d.IsActive==true).ToListAsync();
         
            EmployeeViewModel employeeViewModel = new EmployeeViewModel();
          
            List<EmployeeViewModel> model = new List<EmployeeViewModel>();
            foreach (var item in log)
            {
                EmployeeViewModel employee = new EmployeeViewModel();
                employee.Id = item.Id;
                employee.EmpId = item.EmployeeId;
                employee.Description = item.Description;
                employee.logdate = item.LogDate;               
                employee.stringlogdate = item.LogDate.ToShortDateString();               
                employee.EmpName=db.Employees.FirstOrDefault(f => f.Id==item.EmployeeId).Name;
                var res = db.EmployeeLogTypes.FirstOrDefault(f => f.Id == item.LogType);
                employee.logtypeName= res.Name;
                employee.Colorcode = res.Colorcode;

                employee.Attachments2 = await Task.Run(() => (
           from t1 in db.EmployeeLogs
           join t2 in db.CustomerBookingFileMappings on t1.Id equals t2.CGId
           join t3 in db.FileArchives.Where(x => x.FileCatagoryId == 9) on t2.DocId equals t3.docid
           where t1.Id == item.Id
           select new GLDLBookingAttachment
           {
               DocId = t2.DocId,
               Title = t2.FileTitel,
               Extension = t3.fileext
           }).ToListAsync());
                model.Add(employee);

            }

            employeeViewModel.models = model;
            return employeeViewModel;
        }
        public async Task<EmployeeViewModel> logAttactmentbyId(long id)
        {
            var log = await db.EmployeeLogs.Where(d => d.EmployeeId == id).ToListAsync();

            EmployeeViewModel employeeViewModel = new EmployeeViewModel();
            employeeViewModel.Attachments2 = await Task.Run(() => (
             from t1 in db.EmployeeLogs
             join t2 in db.CustomerBookingFileMappings on t1.Id equals t2.CGId
             join t3 in db.FileArchives.Where(x => x.FileCatagoryId == 9) on t2.DocId equals t3.docid
             where t1.Id == id
             select new GLDLBookingAttachment
             {
                 DocId = t2.DocId,
                 Title = t2.FileTitel,
                 Extension = t3.fileext
             }).ToListAsync());
            return employeeViewModel;



        }

        public async Task<RResult> DeleteEmpLogsById(long EmpLogsId,CancellationToken cancellationToken=default)
        {
            RResult rResult = new RResult();

            if (EmpLogsId>0)
            {
                var dbEntityData =await db.EmployeeLogs.FirstOrDefaultAsync(x => x.Id == EmpLogsId);
                if (dbEntityData!=null)
                {
                    dbEntityData.IsActive = false;
                    db.Entry(dbEntityData).State = EntityState.Modified;
                    if (await db.SaveChangesAsync()>0)
                    {
                        rResult.result = 1;
                        rResult.message = "Data Deleted Successfully!";
                    }
                    else
                    {
                        rResult.result = 0;
                        rResult.message = "Failed!";
                    }
                      
                }
            }
            return rResult;
        }

    }
}
