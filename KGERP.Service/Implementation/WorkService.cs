//using KGERP.Data.CustomModel;
//using KGERP.Data.Models;
//using KGERP.Service.Interface;
//using KGERP.Service.ServiceModel;
//using KGERP.Utility;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;

//namespace KGERP.Service.Implementation
//{
//    public class WorkService : IWorkService
//    {
//        private readonly ERPEntities context;
//        public WorkService(ERPEntities context)
//        {
//            this.context = context;
//        }
//        public List<WorkModel> GetWorks(string searchText)
//        {
//            long managerId = context.Employees.Where(x => x.EmployeeId.Equals(System.Web.HttpContext.Current.User.Identity.Name.ToString())).FirstOrDefault().Id;
//            IQueryable<Work> queryable = context.Works.Include(x => x.WorkAssigns).Include(x => x.WorkState).
//                Where(x => x.ManagerId == managerId && ((x.WorkNo.Contains(searchText) || String.IsNullOrEmpty(searchText)) ||
//               (x.WorkState.State.Contains(searchText) || String.IsNullOrEmpty(searchText)) ||
//               (x.WorkAssigns.Where(a => a.Employee.Name.Contains(searchText)).Count() > 0))
//               ).OrderByDescending(x => x.WorkId);
//            return ObjectConverter<Work, WorkModel>.ConvertList(queryable.ToList()).ToList();
//        }
//        public WorkModel GetWork(int id)
//        {
//            if (id == 0)
//            {
//                string workNo = string.Empty;
//                long managerId = context.Employees.Where(x => x.EmployeeId.Equals(System.Web.HttpContext.Current.User.Identity.Name.ToString())).FirstOrDefault().Id;

//                IQueryable<Work> works = context.Works.Where(x => x.ManagerId == managerId);
//                int count = works.Count();
//                if (count == 0)
//                {
//                    return new WorkModel()
//                    {
//                        WorkNo = GenerateSequenceNumber(0),
//                        ManagerId = managerId,
//                        IsActive = true
//                    };
//                }

//                works = works.Where(x => x.ManagerId == managerId).OrderByDescending(x => x.WorkId).Take(1);
//                workNo = works.ToList().FirstOrDefault().WorkNo;

//                string numberPart = workNo.Substring(2, 4);
//                int lastNumberPart = Convert.ToInt32(numberPart);
//                workNo = GenerateSequenceNumber(lastNumberPart);
//                return new WorkModel()
//                {
//                    WorkNo = workNo,
//                    ManagerId = managerId,
//                    IsActive = true
//                };

//            }

//            Work work = context.Works.Find(id);
//            return ObjectConverter<Work, WorkModel>.Convert(work);
//        }

//        private string GenerateSequenceNumber(int lastWorkNo)
//        {
//            int num = ++lastWorkNo;
//            return "T-" + num.ToString().PadLeft(4, '0');
//        }

//        public List<WorkAssignModel> GetWorkAssigns(int workId)
//        {
//            IQueryable<WorkAssign> queryable = context.WorkAssigns.Include(x => x.WorkState).Include(x => x.Employee.Designation).Where(x => x.WorkId == workId).OrderBy(x => x.Employee.EmployeeId);
//            return ObjectConverter<WorkAssign, WorkAssignModel>.ConvertList(queryable.ToList()).ToList();
//        }



//        public bool SaveWork(int id, WorkModel model)
//        {
//            if (model == null)
//            {
//                throw new Exception("Task data missing!");
//            }
//            long managerId = context.Employees.Where(x => x.EmployeeId.Equals(System.Web.HttpContext.Current.User.Identity.Name.ToString())).FirstOrDefault().Id;

//            Work work = ObjectConverter<WorkModel, Work>.Convert(model);
//            if (id > 0)
//            {
//                work = context.Works.FirstOrDefault(x => x.WorkId == id);
//                if (work == null)
//                {
//                    throw new Exception("Task not found!");
//                }
//                work.ModifiedDate = DateTime.Now;
//                work.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
//                work.ManagerId = model.ManagerId;
//                //work.WorkStateId = model.WorkStateId;
//            }

//            else
//            {
//                work.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
//                work.CreatedDate = DateTime.Now;
//                work.ManagerId = managerId;
//              //  work.WorkStateId = 5;
//            }

//            work.WorkNo = model.WorkNo;
//            //work.WorkTopic = model.WorkTopic;
//            //work.WorkDetail = model.WorkDetail;
//            //work.Remarks = model.Remarks;
//            //work.EntryDate = model.EntryDate;
//            work.ExpectedEndDate = model.ExpectedEndDate;
//            work.EndDate = model.EndDate;
//            work.IsActive = model.IsActive;
//            context.Entry(work).State = work.WorkId == 0 ? EntityState.Added : EntityState.Modified;
//            return context.SaveChanges() > 0;
//        }

//        public List<SelectModel> GetAssignMemberSelectModels(int workId)
//        {
//            string managerId = System.Web.HttpContext.Current.User.Identity.Name;
//            List<WorkMemberModel> members = context.Database.SqlQuery<WorkMemberModel>("exec sp_TaskManagement_MemberList {0}", managerId).ToList();
//            return members.OrderBy(x => x.EmployeeId).Select(x => new SelectModel { Text = "[" + x.EmployeeId + "] " + x.Name, Value = x.MemberId }).ToList();
//        }

//        public bool SaveWorkAssign(int id, WorkAssignModel model)
//        {
//            WorkAssign workAssign = ObjectConverter<WorkAssignModel, WorkAssign>.Convert(model);
//            workAssign.MemberId = model.MemberId;
//            workAssign.WorkId = model.WorkId;
//            if (id <= 0)
//            {
//                workAssign.WorkStateId = 13;
//            }
//            else
//            {
//                workAssign.WorkStateId = model.WorkStateId;
//            }

//            context.WorkAssigns.Add(workAssign);
//            return context.SaveChanges() > 0;
//        }

//        public WorkAssignModel GetWorkAssign(int id)
//        {
//            Work work = context.Works.Find(id);
//            return new WorkAssignModel() { WorkId = id, WorkTopic =""};// work.WorkTopic 
//        }

//        public bool DeleteMember(int workAssignId)
//        {
//            WorkAssign workAssign = context.WorkAssigns.Find(workAssignId);
//            if (workAssign == null)
//            {
//                return false;
//            }
//            context.WorkAssigns.Remove(workAssign);
//            return context.SaveChanges() > 0;
//        }

//        public bool DeleteWork(int workId)
//        {
//            Work work = context.Works.Find(workId);
//            if (work == null)
//            {
//                return false;
//            }
//            context.Works.Remove(work);
//            try
//            {
//                return context.SaveChanges() > 0;
//            }
//            catch (Exception)
//            {

//                return false;
//            }

//        }

//        public List<WorkCustomModel> GetManagerWorks()
//        {
//            return context.Database.SqlQuery<WorkCustomModel>("exec spGetManagerTaskList").ToList();
//        }

//        public List<WorkAssignModel> GetEmployeeWorks()
//        {
//            long employeeId = context.Employees.Where(x => x.EmployeeId.Equals(System.Web.HttpContext.Current.User.Identity.Name.ToString())).FirstOrDefault().Id;
//            return context.Database.SqlQuery<WorkAssignModel>("exec spGetEmployeeWorkList {0}", employeeId).ToList();
//        }

//        public bool ChangeMemberState(WorkAssignModel model)
//        {

//            WorkAssign workAssign = ObjectConverter<WorkAssignModel, WorkAssign>.Convert(model);
//            workAssign = context.WorkAssigns.Where(x => x.WorkAssignId == model.WorkAssignId).FirstOrDefault();
//            workAssign.WorkStateId = model.WorkStateId;
//            workAssign.Report = model.Report;
//            context.Entry(workAssign).State = workAssign.WorkAssignId == 0 ? EntityState.Added : EntityState.Modified;
//            return context.SaveChanges() > 0;
//        }

//        public List<WorkMemberModel> GetWorkMembers(string searchText)
//        {
//            IQueryable<WorkMemberModel> queryable = context.Database.SqlQuery<WorkMemberModel>("exec sp_TaskManagement_GetSpecialEmployeeList").AsQueryable();
//            return queryable.Where(x => (x.EmployeeId.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
//                                       (x.Name.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText))).ToList();

//        }

//        public WorkMemberModel GetWorkMember(int id)
//        {
//            WorkMember workMember = context.WorkMembers.Find(id);
//            return ObjectConverter<WorkMember, WorkMemberModel>.Convert(workMember);
//        }

//        public bool SaveWorkMember(int id, WorkMemberModel model)
//        {
//            if (model == null)
//            {
//                throw new Exception("Date missing!");
//            }

//            WorkMember workMember = ObjectConverter<WorkMemberModel, WorkMember>.Convert(model);
//            //workMember.MemberId = model.MemberId;
//            context.Entry(workMember).State = workMember.WorkMemberId == 0 ? EntityState.Added : EntityState.Modified;
//            return context.SaveChanges() > 0;
//        }

//        public bool DeleteWorkMember(int id)
//        {
//            WorkMember workMember = context.WorkMembers.Find(id);
//            if (workMember == null)
//            {
//                return false;
//            }
//            context.WorkMembers.Remove(workMember);
//            return context.SaveChanges() > 0;
//        }
//    }
//}
