using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation.CurrencyCon;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using log4net.Config;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Humanizer.In;

namespace KGERP.Service.Implementation.TaskManagment
{
    public class TaskManagementEvolutionService : ITaskManagementEvolutionService
    {
        ERPEntities context = new ERPEntities();

        public TaskManagementEvolutionVM TaskAdd(TaskManagementEvolutionVM model)
        {

            var dataToSave = context.Works.Where(t => t.WorkId == model.WorkId).FirstOrDefault();
            TaskManagementEvolutionVM result = new TaskManagementEvolutionVM();
            try
            {
                if (dataToSave == null)
                {
                    var lastWorkEntry = context.Works.OrderByDescending(x => x.WorkNo).Select(x => x.WorkNo).FirstOrDefault();
                    if (lastWorkEntry == null || lastWorkEntry.Length < 7) { lastWorkEntry = "T042400000"; }
                    Work work = new Work();

                    work.ManagerId = model.ManagerId;
                    work.Title = model.TaskTitle;
                    work.Description = model.TaskDescription;
                    work.ExpectedEndDate = model.ExpectedEndDate;
                    work.Status = model.TaskStatusID;
                    work.WorkBoardId= (long)model.WorkboardId;
                    work.CreatedBy = Common.GetUserId();
                    work.CreatedDate = DateTime.Now;
                    work.IsActive = true;
                    work.WorkNo = WorkNoGenerator(lastWorkEntry);
                    work.IsWatched = false;
                    work.StartDate = model.StartDate;
                    work.EndDate=model.EndDate;
                    context.Works.Add(work);
                    var status = context.SaveChanges();

                    foreach (var item in model.EmpIDlList) { 
                    WorkMember workMember = new WorkMember();
                        workMember.EmployeeId = item;
                        workMember.WorkId= work.WorkId;
                        workMember.CreatedBy = Common.GetUserId();
                        workMember.CreatedDate = DateTime.Now;
                        workMember.IsActive = true;
                        context.WorkMembers.Add(workMember);
                        context.SaveChanges();

                    }
                    if (status > 0)
                    {
                        result.FeedbackMessage = "Task Added successfully";
                        result.isOK = true;
                    }
                    else
                    {
                        result.FeedbackMessage = "An unexpected error occured";
                        result.isOK = false;
                    }
                }
                else
                {
                    dataToSave.ManagerId = model.ManagerId;
                    dataToSave.Title = model.TaskTitle;
                    dataToSave.Description = model.TaskDescription;
                    dataToSave.ExpectedEndDate = model.ExpectedEndDate;
                    dataToSave.StartDate = model.StartDate;
                    dataToSave.EndDate = model.EndDate;
                    dataToSave.ModifiedBy = Common.GetUserId();
                    dataToSave.ModifiedDate = DateTime.Now;
                    context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                var message = ex;
            }
            return result;
        }
        public string WorkNoGenerator(string lastWorkNumber)
        {
            string prefix = "T";
            string datePart = DateTime.Now.ToString("MMyy");
            string counterPart = lastWorkNumber.Substring(5);
            int counter = Convert.ToInt32(counterPart);
            counter++;
            counterPart = counter.ToString("D5");
            return $"{prefix}{datePart}{counterPart}";
        }
        //public async Task<TaskManagementEvolutionVM> GetAllTask()
        //{
        //    TaskManagementEvolutionVM vmTask = new TaskManagementEvolutionVM();
        //    vmTask.DataList = (from t1 in context.Works
        //                join t2 in context.Employees on t1.ManagerId equals t2.Id
        //                where t1.IsActive == true
        //                select new TaskManagementEvolutionVM
        //                {
        //                    WorkId = t1.WorkId,
        //                    ManagerId = t1.ManagerId,
        //                    Manager = t2.Name,
        //                    WorkNo = t1.WorkNo,
        //                    Title = t1.Title,
        //                    Description = t1.Description,
        //                    Status = t1.Status,
        //                    StartDate = t1.StartDate,
        //                    EndDate = t1.EndDate,
        //                    ExpectedEndDate = t1.ExpectedEndDate
        //                }).OrderByDescending(x => x.WorkId).AsEnumerable();

        //    return vmTask;
        //}


        public async Task<TaskManagementEvolutionVM> GetWorkstate(int workboardid)
        {
            TaskManagementEvolutionVM vm = new TaskManagementEvolutionVM();
            vm.WorkStates = await context.WorkStates.Where(x => x.WorkBoardId == workboardid).ToListAsync();
            return vm;
        }


        public async Task<TaskManagementEvolutionVM> GetAllTask(long EmployeeId)
        {

            TaskManagementEvolutionVM vmTask = new TaskManagementEvolutionVM();


            vmTask.DataList = await Task.Run(() => (from t1 in context.Works
                                                    join t2 in context.Employees on t1.ManagerId equals t2.Id
                                                    join t3 in context.WorkStates on t1.Status equals t3.WorkStateId
                                                    where t1.IsActive 
                                                    select new TaskManagementEvolutionVM
                                                    {
                                                      OrderBy =  t3.OrderBy,
                                                        EmployeeId = t2.Id,
                                                        WorkId = t1.WorkId,
                                                        ManagerId = t1.ManagerId,
                                                        Manager = t2.Name,
                                                        WorkNo = t1.WorkNo,
                                                        Title = t1.Title,
                                                        Description = t1.Description,
                                                        Status = t1.Status,
                                                        StatusName = t3.State,
                                                        StartDate = t1.StartDate,
                                                        EndDate = t1.EndDate,
                                                        ExpectedEndDate = t1.ExpectedEndDate,
                                                        Seen = t1.IsWatched == true ? "true" : "false",
                                                        EmpIDlList = context.WorkMembers
                                                                .Where(wm => wm.WorkId == t1.WorkId)
                                                                .Select(wm => wm.EmployeeId)
                                                                .ToList()
                                                    }).OrderByDescending(x => x.WorkId).ToList());


            vmTask.DataList = vmTask.DataList.Where(t => t.EmpIDlList.Contains( EmployeeId) || t.ManagerId == EmployeeId).ToList();
            return vmTask;
        }

        public async Task<TaskManagementEvolutionVM> GetAllTaskBywork(long EmployeeId, long? WorkbordId)
        {

            TaskManagementEvolutionVM vmTask = new TaskManagementEvolutionVM();


            vmTask.DataList = await Task.Run(() => (from t1 in context.Works
                                                    join t2 in context.Employees on t1.ManagerId equals t2.Id
                                                    join t3 in context.WorkStates on t1.Status equals t3.WorkStateId
                                                    join t4 in context.WorkBoards on t1.WorkBoardId equals t4.WorkBoardId
                                                    where t1.IsActive && t4.WorkBoardId == WorkbordId
                                                    select new TaskManagementEvolutionVM
                                                    {
                                                        OrderBy = t3.OrderBy,
                                                        EmployeeId = t2.Id,
                                                        WorkId = t1.WorkId,
                                                        ManagerId = t1.ManagerId,
                                                        Manager = t2.Name,
                                                        WorkNo = t1.WorkNo,
                                                        Title = t1.Title,
                                                        Description = t1.Description,
                                                        Status = t1.Status,
                                                        StatusName = t3.State,
                                                        StartDate = t1.StartDate,
                                                        EndDate = t1.EndDate,
                                                        ExpectedEndDate = t1.ExpectedEndDate,
                                                        Seen = t1.IsWatched == true ? "true" : "false",
                                                        EmpIDlList = context.WorkSpacesMembers
                                                                .Where(wm => wm.WorkSpacesId == t4.WorkSpacesId)
                                                                .Select(wm => wm.EmployeeId)
                                                                .ToList()
                                                    }).OrderByDescending(x => x.WorkId).ToList());


            vmTask.DataList = vmTask.DataList.Where(t => t.EmpIDlList.Contains(EmployeeId) || t.ManagerId == EmployeeId).ToList();
            return vmTask;
        }

        public async Task<WorkSpaceVm> GetAllWorkspase(long EmployeeId)
        {

            WorkSpaceVm vmWOkspase = new WorkSpaceVm();
            var emp = context.Employees.Where(x => x.Id == EmployeeId && x.Active).FirstOrDefault();
           vmWOkspase.DataList = await Task.Run(() => 
    (from t1 in context.WorkSpaces
     join t2 in context.WorkSpacesMembers on t1.WorkSpacesId equals t2.WorkSpacesId
     where t1.IsActive &&  t2.EmployeeId == emp.Id
     select new WorkSpaceVm
     {
         WorkSpacesId = t1.WorkSpacesId,
         WorkNo = t1.Description,
         Title = t1.Title,
         Date = t1.Date,
         CreatedBy = t1.CreatedBy,
         CreatedDate = t1.CreatedDate
     })
     .OrderByDescending(x => x.WorkSpacesId)
     .ToListAsync());

            return vmWOkspase;
        }

        public async Task<long> AddWorkspace(WorkSpaceVm model)
        {

            
            try
            {
                WorkSpace workSpace = new WorkSpace()
                {
                    Title = model.Title,
                    Description = model.WorkNo,
                    Date = model.Date,
                    CreatedBy = Common.GetUserId(),
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                context.WorkSpaces.Add(workSpace);
                await context.SaveChangesAsync();
                var userid=Common.GetUserId();
                var obj=await context.Employees.Where(x=>x.EmployeeId==userid).FirstOrDefaultAsync();
                WorkSpacesMember workSpacesMember = new WorkSpacesMember()
                {
                    WorkSpacesId = workSpace.WorkSpacesId,
                    EmployeeId = obj.Id,
                    IsAdmin = true,
                    IsActive = true,
                    CreatedBy = Common.GetUserId(),
                    CreatedDate = DateTime.Now

                };
                context.WorkSpacesMembers.Add(workSpacesMember);
                await context.SaveChangesAsync();
                return workSpace.WorkSpacesId;
            }
            catch (Exception ex)
            {
               
                return -1;
            }
        }


        public async Task<bool> DeleteWorkSpacee(long id)
        {
            var workspace = await context.WorkSpaces.FindAsync(id);
            if (workspace == null)
            {
                return false;
            }
            workspace.IsActive=false;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteWorkBord(long id)
        {
            var workboard = await context.WorkBoards.FindAsync(id);
            if (workboard == null)
            {
                return false;
            }
            workboard.IsActive = false;
            await context.SaveChangesAsync();
            return true;
        }



        public async Task<TaskManagementEvolutionVM> GetTaskForUpdate(int? id)
        {
            TaskManagementEvolutionVM model = new TaskManagementEvolutionVM();
            model = await (from t1 in context.Works
                           join t2 in context.Employees
                           on t1.ManagerId equals t2.Id
                           where t1.WorkId == id
                           select new TaskManagementEvolutionVM
                           {
                               WorkId = t1.WorkId,
                               ManagerId = t1.ManagerId,
                               Manager = t2.Name,
                               TaskTitle = t1.Title,
                               TaskDescription = t1.Description,
                               TaskStatusID = t1.Status,
                               ExpectedEndDate = t1.ExpectedEndDate
                           }).FirstOrDefaultAsync();

            return model;
        }


        public async Task<bool> editModal(string newTitle, long buttonValue)
        {
            var obj = context.Works.Where(x => x.WorkId == buttonValue).FirstOrDefault();
            if (obj != null)
            {
                obj.Title = newTitle;
                obj.ModifiedBy = Common.GetUserId();
                obj.ModifiedDate = DateTime.Now;
                await context.SaveChangesAsync();
                return true;
            }
            return false;

        }

        public bool WatchModel(long workId)
        {
            var obj = context.Works.Where(x => x.WorkId == workId).FirstOrDefault();
            if (obj != null)
            {
                obj.IsWatched = true;
                context.SaveChanges();
                return true;
            }
            return false;

        }
        public bool WatchModelSeennUnseen(long workId)
        {
            var obj = context.Works.Where(x => x.WorkId == workId).FirstOrDefault();
            if (obj.IsWatched == true)
            {
                obj.IsWatched = false;
                context.SaveChanges();
                return true;
            }
            else
            {
                obj.IsWatched = true;
                context.SaveChanges();
                return true;
            }
            return false;

        }

        public async Task<TaskManagementEvolutionVM> GetAllTaskBYajax(long WorkId,int? workboardid)
        {
            string username = Common.GetUserId();
            TaskManagementEvolutionVM vmTask = await Task.Run(() => (from t1 in context.Works
                                                                     join ts in context.WorkStates on t1.Status equals ts.WorkStateId
                                                                     join Te in context.Employees on t1.ManagerId equals Te.Id
                                                                     join wl in context.WorkLabels on t1.WorkLabelId equals wl.WorkLabelId into wlGroup
                                                                     from wl in wlGroup.DefaultIfEmpty()
                                                                     where t1.WorkId == WorkId && t1.IsActive

                                                                     select new TaskManagementEvolutionVM
                                                                     {
                                                                         WorkId = t1.WorkId,
                                                                         ManagerId = t1.ManagerId,
                                                                         Manager = Te.Name,
                                                                         WorkNo = t1.WorkNo,
                                                                         Title = t1.Title,
                                                                         Description = t1.Description,
                                                                         Status = t1.Status,
                                                                         StartDate = t1.StartDate,
                                                                         EndDate = t1.EndDate,
                                                                         StatusName = ts.State,
                                                                         TaskStatusID = ts.WorkStateId,
                                                                         ExpectedEndDate = t1.ExpectedEndDate,
                                                                         Seen = t1.IsWatched ? "true" : "false",
                                                                         worklebelId = (long)t1.WorkLabelId,
                                                                         WoorklabelColor=wl.ColorClass,
                                                                         WorkLebelName = wl.Name
                                                                     }).FirstOrDefaultAsync());





            vmTask.WorkLebel = (context.WorkLabels
                                          .Where(t1 => t1.IsActive && t1.WorkBoardId==workboardid)
                                          .Select(t1 => new SelectWorkLebel
                                          {
                                              Value = t1.WorkLabelId,
                                              Text = t1.Name,
                                              ColorName=t1.ColorClass

                                          })
                                          .ToList()) ?? new List<SelectWorkLebel>();



            vmTask.WorkmeberVm = await Task.Run(() => (from t1 in context.WorkMembers
                                                       join t2 in context.Employees
                                                       on t1.EmployeeId equals t2.Id
                                                       where t1.WorkId == WorkId && t1.IsActive
                                                       select new WorkmemeberVm
                                                       {
                                                           WorkId = t1.WorkId,
                                                           WorkMemberId = (int)t1.WorkMemberId,
                                                           EmployeeId = t2.Id,
                                                           EmployeeName = t2.Name,
                                                           EmployeeKgId = t2.EmployeeId,
                                                           IsActive = t1.IsActive
                                                       }).ToListAsync());


            vmTask.WoorkStateVms = await Task.Run(() => context.WorkStates
                                                  .Where(t1 => t1.IsActive && t1.WorkBoardId == workboardid)
                                                  .Select(t1 => new SelectWorkState
                                                  {
                                                      Value = t1.WorkStateId,
                                                      Text = t1.State
                                                  })
                                                  .ToListAsync()) ?? new List<SelectWorkState>();



            vmTask.workComentVm = await Task.Run(() =>
     (from t1 in context.WorkComments
      join t2 in context.Employees on t1.EmployeeId equals t2.Id
      where t1.IsActive && t1.WorkId == WorkId
      select new WorkComentVm
      {
          WorkQAId = t1.WorkQAId,
          EmployeeId = (int)t1.EmployeeId,
          EmployeeName = t2.Name,
          EmployeeKgId = t2.EmployeeId,
          IsActive = t1.IsActive,
          Comment = t1.Comment,
          CreatedBy = t1.CreatedBy,
          CreatedDate = t1.CreatedDate,
          UserName = username,
          LikeCount = context.WorkCommentLikes.Count(wcl => wcl.WorkQAId == t1.WorkQAId && wcl.IsActive)
      })
      .OrderByDescending(x => x.WorkQAId)
      .ToList());




            vmTask.workComentLikVm = await Task.Run(() => (from t1 in context.WorkCommentLikes
                                                           join t2 in context.Employees
                                                           on t1.EmployeeId equals t2.Id
                                                           join t3 in context.WorkComments
                                                           on t1.WorkQAId equals t3.WorkQAId
                                                           join t4 in context.Works on t3.WorkId equals t4.WorkId
                                                           where t1.IsActive && t4.WorkId == WorkId
                                                           select new WorkComentLikVm
                                                           {
                                                               WorkQAId = t1.WorkQAId,
                                                               WorkLikeId = t1.WorkLikeId,
                                                               EmployeeId = (int)t1.EmployeeId,
                                                               EmployeeName = t2.Name,
                                                               EmployeeKgId = t2.EmployeeId,
                                                               IsActiveLike = t1.IsActive,
                                                               CreatedBy = t1.CreatedBy,
                                                               CreatedDate = t1.CreatedDate,
                                                               Username = username
                                                           }).ToListAsync());


            vmTask.WorkSAttachmentList = await Task.Run(() =>
         (from t1 in context.WorkAttatchments
          join t4 in context.Works on t1.WorkId equals t4.WorkId
          where  t4.WorkId == WorkId && t1.IsActive==true
          select new VmWorkAttacthment
          {
              AttatchmentId = t1.AttatchmentId,
              WorkId= (int)t1.WorkId,
              Path=t1.Path,
              AttatchmentText=t1.AttatchmentText,
              Extension=t1.Extension
          }).ToListAsync());




            return vmTask;
        }







        public bool WorkAssignSer(long[] selectedInstallments, long workId)
        {

            if (selectedInstallments != null && selectedInstallments.Length > 0 && workId > 0)
            {

                List<WorkMember> workMembers = new List<WorkMember>();


                foreach (var item in selectedInstallments)
                {

                    var obj = context.WorkMembers.FirstOrDefault(c => c.WorkId == workId && c.EmployeeId == item && c.IsActive);

                    if (obj == null)
                    {
                        WorkMember workMember = new WorkMember
                        {
                            EmployeeId = item,
                            WorkId = workId,
                            CreatedBy = Common.GetUserId(),
                            CreatedDate = DateTime.Now,
                            IsActive = true,

                        };
                        workMembers.Add(workMember);
                        context.WorkMembers.Add(workMember);
                        context.SaveChanges();
                    }



                }


                return true;
            }
            return false;

        }
        public bool RemoveWorkAssign(int workMemberId)
        {

            var obj = context.WorkMembers.Where(x => x.WorkMemberId == workMemberId).FirstOrDefault();
            if (obj != null)
            {
                obj.IsActive = false;
                obj.ModifiedBy = Common.GetUserId();
                obj.ModifiedDate = DateTime.Now;
                context.SaveChanges();
                return true;
            }
            return false;


        }
        public bool ChangeStaeValue(int selectedValue, long workid)
        {

            var obj = context.Works.Where(x => x.WorkId == workid).FirstOrDefault();
            if (obj != null)
            {
                obj.Status = selectedValue;
                obj.ModifiedBy = Common.GetUserId();
                obj.ModifiedDate = DateTime.Now;
                context.SaveChanges();
                return true;
            }
            return false;


        }
        public bool ChangeWorkLabelValue(int selectedValue, long workid)
        {

            var obj = context.Works.Where(x => x.WorkId == workid).FirstOrDefault();
            if (obj != null)
            {
                obj.WorkLabelId = selectedValue;
                obj.ModifiedBy = Common.GetUserId();
                obj.ModifiedDate = DateTime.Now;
                context.SaveChanges();
                return true;
            }
            return false;

        }

        public bool ChangeDescription(string description, long workid)
        {

            var obj = context.Works.Where(x => x.WorkId == workid).FirstOrDefault();
            if (obj != null)
            {
                obj.Description = description;
                obj.ModifiedBy = Common.GetUserId();
                obj.ModifiedDate = DateTime.Now;
                context.SaveChanges();
                return true;
            }
            return false;


        }

        public bool CommentSave(string comment, long workId)
        {
            try
            {

                var userId = Common.GetUserId();
                var employee = context.Employees.FirstOrDefault(x => x.EmployeeId == userId);
                if (employee == null)
                {

                    return false;
                }
                WorkComment workComment = new WorkComment
                {
                    EmployeeId = employee.Id,
                    WorkId = workId,
                    Comment = comment,
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };


                context.WorkComments.Add(workComment);
                context.SaveChanges();


                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error saving comment: " + ex.Message);
                return false;
            }
        }
        public bool CommentUpdate(string newText, long workQAId)
        {
            var obj = context.WorkComments.Where(x => x.WorkQAId == workQAId).FirstOrDefault();
            if (obj != null)
            {
                obj.Comment = newText;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = Common.GetUserId();
                context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool DeleteComent(long workQAId)
        {
            var obj = context.WorkComments.Where(x => x.WorkQAId == workQAId).FirstOrDefault();
            if (obj != null)
            {
                obj.IsActive = false;
                context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool LikeGive(long WorkQid)
        {
            var username = Common.GetUserId();
            var employeeid = context.Employees.Where(x => x.EmployeeId == username).FirstOrDefault();
            var obj = context.WorkCommentLikes.Where(x => x.WorkQAId == WorkQid && x.EmployeeId == employeeid.Id).FirstOrDefault();
            if (obj != null)
            {
                if (obj.IsActive)
                {
                    obj.IsActive = false;
                }
                else
                {
                    obj.IsActive = true;
                }

                context.SaveChanges();
                return true;
            }
            else
            {
                WorkCommentLike workCommentLike = new WorkCommentLike()
                {
                    WorkQAId = WorkQid,
                    EmployeeId = employeeid.Id,
                    IsActive = true,
                    CreatedBy = username,
                    CreatedDate = DateTime.Now
                };


                context.WorkCommentLikes.Add(workCommentLike);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public int TaskDelete(int id)
        {
            int result = -1;

            if (id > 0)
            {
                Work myWork = context.Works.Find(id);
                myWork.IsActive = false;
                result = context.SaveChanges();
            }
            return result;
        }
        public async Task<List<WorkComentLikVm>> LikePeople(long WorkQId)
        {
            string username = Common.GetUserId();

            var result = await (from t1 in context.WorkCommentLikes
                                join t2 in context.Employees on t1.EmployeeId equals t2.Id
                                join t3 in context.WorkComments on t1.WorkQAId equals t3.WorkQAId
                                join t4 in context.Works on t3.WorkId equals t4.WorkId
                                where t1.IsActive && t1.WorkQAId == WorkQId
                                select new WorkComentLikVm
                                {
                                    WorkQAId = t1.WorkQAId,
                                    WorkLikeId = t1.WorkLikeId,
                                    EmployeeId = (int)t1.EmployeeId,
                                    EmployeeKgId = t2.EmployeeId,
                                    EmployeeName = (t2.EmployeeId == username) ? "You" : t2.Name,
                                    IsActiveLike = t1.IsActive,
                                    CreatedBy = t1.CreatedBy,
                                    CreatedDate = t1.CreatedDate,
                                    Username = username
                                }).ToListAsync();

            return result;
        }


        public async Task<WorkBoardVm> GetWorkBoard(long WorkSpaceId)
        {
            WorkBoardVm vm= new WorkBoardVm();
            string username = Common.GetUserId();

            vm.DataList = await (from t1 in context.WorkBoards
                                join t2 in context.WorkSpaces on t1.WorkSpacesId equals t2.WorkSpacesId
                                where t1.IsActive && t1.WorkSpacesId == WorkSpaceId
                                select new WorkBoardVm
                                {
                                    WorkBoardId = t1.WorkBoardId,
                                    WorkSpacesId = t2.WorkSpacesId,
                                    BoardName=t1.BoardName,
                                    CreatedBy=t1.CreatedBy
                                }).ToListAsync();

            vm.WorkSpaceName=await context.WorkSpaces.Where(x=>x.WorkSpacesId==WorkSpaceId).Select(x=>x.Title).FirstOrDefaultAsync();

            return vm;
        }


        //public async Task<TaskManagementEvolutionVM> GetReport(int CompanyId)
        //{
        //    TaskManagementEvolutionVM taskvm = new TaskManagementEvolutionVM();

        //    taskvm.WoorkStateVms = await Task.Run(() => context.WorkStates
        //                                          .Where(t1 => t1.IsActive)
        //                                          .Select(t1 => new SelectWorkState
        //                                          {
        //                                              Value = t1.WorkStateId,
        //                                              Text = t1.State
        //                                          })
        //                                          .ToListAsync()) ?? new List<SelectWorkState>();


        //    taskvm.WorkLebel = (await Task.Run(() => context.WorkLabels
        //                                     .Where(t1 => t1.IsActive)
        //                                     .Select(t1 => new SelectWorkLebel
        //                                     {
        //                                         Value = t1.WorkLabelId,
        //                                         Text = t1.Name
        //                                     })
        //                                     .ToListAsync())) ?? new List<SelectWorkLebel>();


        //    return taskvm;
        //}

        public async Task<List<SelectWorkLebel>> GetSelectWorkLebel()
        {
            List<SelectWorkLebel> selectWorkLebels = await context.WorkLabels
                                        .Where(t1 => t1.IsActive)
                                        .Select(t1 => new SelectWorkLebel
                                        {
                                            Value = t1.WorkLabelId,
                                            Text = t1.Name
                                        })
                                        .ToListAsync();

            return selectWorkLebels;
        }

        public async Task<List<SelectWorkState>> GetSelectWorkState()
        {
            List<SelectWorkState> workStates = await context.WorkStates
                                                .Where(t1 => t1.IsActive)
                                                .Select(t1 => new SelectWorkState
                                                {
                                                    Value = t1.WorkStateId,
                                                    Text = t1.State
                                                })
                                                .ToListAsync();

            return workStates;
        }



        public async Task<long> AddWorkBoard(WorkBoard model)
        {
            try
            {
                WorkBoard workBoard = new WorkBoard()
                {
                    WorkSpacesId = model.WorkSpacesId,
                    BoardName = model.BoardName,
                    CreatedBy = Common.GetUserId(),
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                context.WorkBoards.Add(workBoard);
                await context.SaveChangesAsync();

                return workBoard.WorkBoardId;
            }
            catch (Exception ex)
            {

                return -1;
            }
        }


        public async Task<WorkSpacesMemberVM> GetWorkSpacemember(long id)
        {
            WorkSpacesMemberVM workSpacesMemberVM = new WorkSpacesMemberVM();
            workSpacesMemberVM.DataList = await Task.Run(() => (from t1 in context.WorkSpacesMembers
                                                                join t2 in context.WorkSpaces on t1.WorkSpacesId equals t2.WorkSpacesId
                                                                join t3 in context.Employees on t1.EmployeeId equals t3.Id
                                                                where t1.IsActive && t1.WorkSpacesId== id
                                                                select new WorkSpacesMemberVM
                                                                {
                                                                    WorkSpacesId= t1.WorkSpacesId,
                                                                    WorkSpacesMemberId=t1.WorkSpacesMemberId,
                                                                    EmployeeId=t1.EmployeeId,
                                                                    Name=t3.Name+" - "+t3.EmployeeId,
                                                                    IsAdmin=t1.IsAdmin
                                                                }).OrderByDescending(x => x.WorkSpacesId).ToListAsync());

            string userid=Common.GetUserId();
            var obj=context.Employees.Where(x=>x.EmployeeId==userid).FirstOrDefault();
            if (obj != null)
            {
                var admin = workSpacesMemberVM.DataList.Where(x=>x.EmployeeId==obj.Id).FirstOrDefault();

                if (admin != null)
                {
                    workSpacesMemberVM.IsAdmin = admin.IsAdmin;
                }
               
            }
      
            return workSpacesMemberVM;
        }

        public bool WorkAssignSerFoorBorad(long[] selectedInstallments, long WorkSpaceId)
        {

            if (selectedInstallments != null && selectedInstallments.Length > 0 && WorkSpaceId > 0)
            {

                List<WorkSpacesMember> workMembersSpace = new List<WorkSpacesMember>();


                foreach (var item in selectedInstallments)
                {

                    var obj = context.WorkSpacesMembers.FirstOrDefault(c => c.WorkSpacesId == WorkSpaceId && c.EmployeeId == item && c.IsActive);

                    if (obj == null)
                    {
                        WorkSpacesMember workspacemember = new WorkSpacesMember
                        {
                            EmployeeId = item,
                            WorkSpacesId = WorkSpaceId,
                            CreatedBy = Common.GetUserId(),
                            CreatedDate = DateTime.Now,
                            IsActive = true,

                        };
                        workMembersSpace.Add(workspacemember);
                        context.WorkSpacesMembers.Add(workspacemember);
                        context.SaveChanges();
                    }



                }

                return true;
            }
            return false;

        }



        public bool DeleteWorkMember(long WorkSpacesMemberId)
        {
            var obj = context.WorkSpacesMembers.Where(x => x.WorkSpacesMemberId == WorkSpacesMemberId).FirstOrDefault();
            if(obj == null)
            {
                return false;
            }
            else
            {
                obj.IsActive = false;
                context.SaveChanges();
                return true;
            }
           
        }

        public async Task<VMWorkLabelAndSpace> GetWorkLebel(long id)
        {
            VMWorkLabelAndSpace vm = new VMWorkLabelAndSpace();
            vm.LabelDataList = await Task.Run(() => (from t1 in context.WorkLabels
                                                join t2 in context.WorkBoards on t1.WorkBoardId equals t2.WorkBoardId
                                                where t1.IsActive && t1.WorkBoardId == id
                                                select new VMWorkLabelAndSpace
                                                {
                                                    WorkLabelName = t1.Name,
                                                    WorkLabelId=t1.WorkLabelId,
                                                    ColorName=t1.ColorClass
                                   
                                                }).AsEnumerable());

            vm.SpaceDataList= await Task.Run(() => (from t1 in context.WorkStates
                                                    join t2 in context.WorkBoards on t1.WorkBoardId equals t2.WorkBoardId
                                                    where t1.IsActive && t1.WorkBoardId == id
                                                    select new VMWorkLabelAndSpace
                                                    {
                                                        WorkLabelName = t1.State,
                                                        WorkStateId=t1.WorkStateId
                                                        

                                                    }).AsEnumerable());

            return vm;
        }









        public async Task<bool> SaveAttachment(HttpPostedFileBase file, long wrkid)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var fileExtension = Path.GetExtension(fileName);
                var path = "/FileUpload/TaskAttatchment/" + fileName;
                var workAttachment = new WorkAttatchment
                {
                    WorkId = wrkid,
                    Path = path,
                    CreateBy = Common.GetUserId(),
                    CreateDate = DateTime.Now,
                    AttatchmentText = fileName,
                    IsActive=true,
                    Extension= fileExtension
                };

                context.WorkAttatchments.Add(workAttachment);
                await context.SaveChangesAsync(); 
                return true;
            }

            return false;
        }

        public async Task<bool> Deleteattachment (long Attatchmentid)
        {
            var obj =await context.WorkAttatchments.Where(x=>x.AttatchmentId==Attatchmentid).FirstOrDefaultAsync();
            if(obj != null)
                
            {
                obj.IsActive = false;
                obj.ModifyDate = DateTime.Now;
                obj.ModifyBy=Common.GetUserId();
                await context.SaveChangesAsync();
                return true;

            }
            else
            {
                return false;
            }
        }
        public async Task<bool> DeleteaWoklabel(long lavelid)
        {
            var obj = await context.WorkLabels.Where(x => x.WorkLabelId == lavelid).FirstOrDefaultAsync();
            if (obj != null)

            {
                obj.IsActive = false;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = Common.GetUserId();
                await context.SaveChangesAsync();
                return true;

            }
            else
            {
                return false;
            }
        }


        public bool UpdateEndDateee(long workid, DateTime datePicker1)
        {

            var obj = context.Works.Where(x => x.WorkId == workid).FirstOrDefault();
            if (obj != null)
            {
                obj.EndDate = datePicker1;
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }


        }
        public bool AdminMake(long WorkSpacesMemberId, bool isChecked,long workspaceid)
        {

            var obj = context.WorkSpacesMembers.Where(x => x.WorkSpacesMemberId == WorkSpacesMemberId).FirstOrDefault();
            if (obj != null)
            {
                if (isChecked)
                {
                    obj.IsAdmin = true;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    var obj2 = context.Employees.Where(s => s.Id == obj.EmployeeId).FirstOrDefault();
                    var chkadmin=context.WorkSpaces.Where(y=>y.CreatedBy==obj2.EmployeeId).FirstOrDefault();
                    if(chkadmin == null)
                    {
                        obj.IsAdmin = false;
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
          
                   
                }
            }
            else
            {
                return false;
            }


        }


    }
}
