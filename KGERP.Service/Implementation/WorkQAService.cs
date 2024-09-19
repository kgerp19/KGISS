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
//    public class WorkQAService : IWorkQAService
//    {
//        private readonly ERPEntities context;
//        public WorkQAService(ERPEntities context)
//        {
//            this.context = context;
//        }


//        public List<WorkQAModel> GetWorkQAs()
//        {
//            string employeeId = System.Web.HttpContext.Current.User.Identity.Name;
//            return context.Database.SqlQuery<WorkQAModel>("exec sp_TaskManagement_GetQuestionAnswer {0}", employeeId).ToList();
//        }

//        public WorkQAModel GetWorkQA(int id)
//        {
//            if (id == 0)
//            {
//                return new WorkQAModel();
//            }
//            return context.Database.SqlQuery<WorkQAModel>("exec sp_TaskManagement_GetQuestionAnswerByWorkQAId {0}", id).FirstOrDefault();
//        }

//        public bool SaveWorkQA(long id, WorkQAModel model)
//        {
//            if (model == null)
//            {
//                throw new Exception("Data missing!");
//            }


//            WorkQA workQA = ObjectConverter<WorkQAModel, WorkQA>.Convert(model);
//            if (id > 0)
//            {
//                workQA = context.WorkQAs.FirstOrDefault(x => x.WorkQAId == id);
//                if (workQA == null)
//                {
//                    throw new Exception("Data not found!");
//                }
//                workQA.FromEmpId = model.FromEmpId;
//                workQA.ToEmpId = model.ToEmpId;


//            }

//            else
//            {
//                long fromEmpId = context.Employees.Where(x => x.EmployeeId == System.Web.HttpContext.Current.User.Identity.Name).First().Id;

//                if (model.ParentWorkQAId == null)
//                {
//                    workQA.Conversation = model.Conversation;
//                }
//                else
//                {
//                    workQA.Conversation = model.Reply;
//                }

//                workQA.ConversationDate = DateTime.Now;
//                workQA.FromEmpId = fromEmpId;
//                workQA.ToEmpId = model.ToEmpId;
//                workQA.ParentWorkQAId = model.ParentWorkQAId;


//            }


//            context.Entry(workQA).State = workQA.WorkQAId == 0 ? EntityState.Added : EntityState.Modified;
//            return context.SaveChanges() > 0;
//        }

//        public WorkQAModel GetQuestion(int id)
//        {
//            if (id == 0)
//            {
//                return new WorkQAModel();
//            }
//            return context.Database.SqlQuery<WorkQAModel>("exec sp_TaskManagement_GetQuestionByWorkQAId {0}", id).FirstOrDefault();
//        }

//        public WorkQAModel GetAnswer(int id)
//        {
//            if (id == 0)
//            {
//                return new WorkQAModel();
//            }
//            return context.Database.SqlQuery<WorkQAModel>("exec sp_TaskManagement_GetAnswerByWorkQAId {0}", id).FirstOrDefault();
//        }

//        public long? GetCurrentEmpId()
//        {
//            return context.Employees.Where(x => x.EmployeeId.Equals(System.Web.HttpContext.Current.User.Identity.Name)).First().Id;
//        }

//        public ICollection<WorkQAFileModel> GetWorkQAFiles(long workQAId)
//        {
//            ICollection<WorkQAFile> files = context.WorkQAFiles.Where(x => x.WorkQAId == workQAId).ToList();
//            return ObjectConverter<WorkQAFile, WorkQAFileModel>.ConvertList(files.ToList()).ToList();
//        }

//        public string GetAttachFile(long workQAFileId)
//        {
//            return context.WorkQAFiles.Where(x => x.WorkQAFileId == workQAFileId).First().FileName;
//        }
//    }
//}
