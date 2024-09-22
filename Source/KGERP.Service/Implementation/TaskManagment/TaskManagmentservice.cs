using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.TaskManagment
{
    public class TaskManagmentservice
    {
        private readonly ERPEntities context;
        public TaskManagmentservice(ERPEntities context)
        {
            this.context = context;
        }
        public async Task<TicketingViewModel> RequestTicket(TicketingViewModel model)
        {
            Ticketing ticketing = new Ticketing();
            var count = context.Ticketings.Count();
            if (count == 0)
            {
                model.TaskNo = "KGSL100" + 1;
            }
            else
            {
                model.TaskNo = "KGSL100" + count;
            }
            ticketing.EmployeeId = model.EmployeeId;
            ticketing.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            ticketing.CreatedDate = DateTime.Now;
            ticketing.CompanyId = (int)model.CompanyIdFK;
            ticketing.Status = 1;
            ticketing.TaskNo = model.TaskNo;
            ticketing.Subject = model.Subject;
            ticketing.Description = model.Description;
            ticketing.TaskType = model.TaskType.Value;
            using (var scope = context.Database.BeginTransaction())
            {
                try
                {
                    context.Ticketings.Add(ticketing);
                    scope.Commit();
                    if (await context.SaveChangesAsync() > 0)
                    {
                        return model;
                    }
                    return model;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return model;
                }
            }
            return model;
        }

        public dynamic RequestTicketList(int CompanyId, int EmployeeId)
        {
            var list = context.Ticketings.Where(d => d.EmployeeId == EmployeeId).ToList().OrderByDescending(d => d.Id);
            List<TicketingViewModel> listof = new List<TicketingViewModel>();
            foreach (var model in list)
            {
                TicketingViewModel ticketing = showModel(model);
                listof.Add(ticketing);
            }
            TicketingViewModel model1 = new TicketingViewModel();
            model1.DataList = listof;
            model1.CompanyId = CompanyId;

            return model1;
        }



        public dynamic RequestDelete(TicketingViewModel model)
        {
            var item = context.Ticketings.Find(model.Id);
            context.Ticketings.Remove(item);
            context.SaveChanges();
            return true;
        }
        public dynamic ChangeStatus(TicketingViewModel model)
        {
            var item = context.Ticketings.Find(model.Id);
            item.Status = model.Status;
            item.ModifyBy = System.Web.HttpContext.Current.User.Identity.Name;
            item.ModifyDate = DateTime.Now;
            context.Ticketings.Add(item);
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
            return true;
        }

        public dynamic update(TicketingViewModel model)
        {
            var item = context.Ticketings.Find(model.Id);
            item.ModifyBy = System.Web.HttpContext.Current.User.Identity.Name;
            item.ModifyDate = DateTime.Now;
            item.CompanyId = (int)model.CompanyIdFK;
            item.Subject = model.Subject;
            item.TaskType = (int)model.TaskType;
            item.Description = model.Description;
            context.Ticketings.Add(item);
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
            return true;
        }


        public async Task<TicketingViewModel> GetAllList(int companyId, int CompanyIdFK, int type, DateTime? fromDate, DateTime? toDate, int status)
        {
            var list = context.Ticketings.Where(d => d.CreatedDate >= fromDate && d.CreatedDate <= toDate).ToList();
            if (CompanyIdFK != 0)
            {
                list = list.Where(d => d.CompanyId == CompanyIdFK).ToList();
            }
            if (type != 0)
            {
                list = list.Where(d => d.TaskType == type).ToList();
            }

            if (status != 0)
            {
                if (status != 5)
                {
                    list = list.Where(d => d.Status == status).ToList();
                }
            }
            List<TicketingViewModel> listof = new List<TicketingViewModel>();
            foreach (var model in list)
            {
                TicketingViewModel ticketing = showModel(model); listof.Add(ticketing);
            }
            TicketingViewModel model1 = new TicketingViewModel();
            model1.DataList = listof.OrderByDescending(d => d.Id);
            return model1;
          }

        public async Task<TicketingViewModel> Erplist(int companyId, int CompanyIdFK, DateTime? fromDate, DateTime? toDate, int status)
        {
            var list = context.Ticketings.Where(d => d.CreatedDate >= fromDate && d.CreatedDate <= toDate && d.TaskType == 1).ToList();
            if (CompanyIdFK != 0)
            {
                list = list.Where(d => d.CompanyId == CompanyIdFK).ToList();
            }

            if (status != 0)
            {
                if (status != 5)
                {
                    list = list.Where(d => d.Status == status).ToList();
                }

            }

            List<TicketingViewModel> listof = new List<TicketingViewModel>();
            foreach (var model in list)
            {
                TicketingViewModel ticketing = showModel(model);
                listof.Add(ticketing);
            }
            TicketingViewModel model1 = new TicketingViewModel();
            model1.DataList = listof.OrderByDescending(d => d.Id);
            return model1;

        }


        public async Task<TicketingViewModel> Networklist(int companyId, int CompanyIdFK, DateTime? fromDate, DateTime? toDate, int status)
        {
            var list = context.Ticketings.Where(d => d.CreatedDate >= fromDate && d.CreatedDate <= toDate && d.TaskType == 2).ToList();
            if (CompanyIdFK != 0)
            {
                list = list.Where(d => d.CompanyId == CompanyIdFK).ToList();
            }

            if (status != 0)
            {
                if (status != 5)
                {
                    list = list.Where(d => d.Status == status).ToList();
                }
            }
            List<TicketingViewModel> listof = new List<TicketingViewModel>();
            foreach (var model in list)
            {
                TicketingViewModel ticketing = showModel(model); listof.Add(ticketing);
            }
            TicketingViewModel model1 = new TicketingViewModel();
            model1.DataList = listof.OrderByDescending(d => d.Id);
            return model1;

        }

        public async Task<TicketingViewModel> Adminlist(int companyId, int CompanyIdFK, DateTime? fromDate, DateTime? toDate, int status)
        {
            var list = context.Ticketings.Where(d => d.CreatedDate >= fromDate && d.CreatedDate <= toDate && d.TaskType == 3).ToList();
            if (CompanyIdFK != 0)
            {
                list = list.Where(d => d.CompanyId == CompanyIdFK).ToList();
            }

            if (status != 0)
            {
                if (status != 5)
                {
                    list = list.Where(d => d.Status == status).ToList();
                }

            }

            List<TicketingViewModel> listof = new List<TicketingViewModel>();
            foreach (var model in list)
            {
                TicketingViewModel ticketing = showModel(model); listof.Add(ticketing);
            }
            TicketingViewModel model1 = new TicketingViewModel();
            model1.DataList = listof.OrderByDescending(d => d.Id);
            return model1;

        }

        public async Task<TicketingViewModel> Accountslist(int companyId, int CompanyIdFK, DateTime? fromDate, DateTime? toDate, int status)
        {
            var list = context.Ticketings.Where(d => d.CreatedDate >= fromDate && d.CreatedDate <= toDate && d.TaskType == 4).ToList();
            if (CompanyIdFK != 0)
            {
                list = list.Where(d => d.CompanyId == CompanyIdFK).ToList();
            }

            if (status != 0)
            {
                if (status != 5)
                {
                    list = list.Where(d => d.Status == status).ToList();
                }

            }

            List<TicketingViewModel> listof = new List<TicketingViewModel>();
            foreach (var model in list)
            {
                TicketingViewModel ticketing = showModel(model); listof.Add(ticketing);
            }
            TicketingViewModel model1 = new TicketingViewModel();
            model1.DataList = listof.OrderByDescending(d => d.Id);
            return model1;

        }

        public async Task<TicketingViewModel> Engineeringlist(int companyId, int CompanyIdFK, DateTime? fromDate, DateTime? toDate, int status)
        {
            var list = context.Ticketings.Where(d => d.CreatedDate >= fromDate && d.CreatedDate <= toDate && d.TaskType == 5).ToList();
            if (CompanyIdFK != 0)
            {
                list = list.Where(d => d.CompanyId == CompanyIdFK).ToList();
            }

            if (status != 0)
            {
                if (status != 5)
                {
                    list = list.Where(d => d.Status == status).ToList();
                }

            }

            List<TicketingViewModel> listof = new List<TicketingViewModel>();
            foreach (var model in list)
            {
                TicketingViewModel ticketing = showModel(model); listof.Add(ticketing);
            }
            TicketingViewModel model1 = new TicketingViewModel();
            model1.DataList = listof.OrderByDescending(d => d.Id);
            return model1;
        }
        private TicketingViewModel showModel(Ticketing model)
        {
            var emp = context.Employees.FirstOrDefault(f => f.Id == model.EmployeeId);
            TicketingViewModel ticketing = new TicketingViewModel();
            ticketing.EmployeeId = model.EmployeeId;
            ticketing.Id = model.Id;
            ticketing.CreatedBy = model.CreatedBy;
            ticketing.Date = model.CreatedDate.Value.ToLongDateString();
            ticketing.CompanyIdFK = model.CompanyId;
            ticketing.Status = model.Status.Value;
            ticketing.TaskNo = model.TaskNo;
            ticketing.Subject = model.Subject;
            ticketing.Description = model.Description;
            ticketing.TaskType = model.TaskType;
            ticketing.CompanyName = model.CompanyId == 0 ? "" : context.Companies.FirstOrDefault(g => g.CompanyId == model.CompanyId).Name;
            ticketing.EmpName = model.EmployeeId == 0 ? "" : emp.Name;
            ticketing.DesignationName = emp == null ? "" : context.Designations.FirstOrDefault(g => g.DesignationId == emp.DesignationId).Name;
            return ticketing;
        }

    }
}
