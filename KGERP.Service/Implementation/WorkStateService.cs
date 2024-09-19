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
//    public class WorkStateService : IWorkStateService
//    {
//        private readonly ERPEntities context;

//        public WorkStateService(ERPEntities context)
//        {
//            this.context = context;
//        }

//        public List<WorkStateModel> GetWorkStates()
//        {
//            List<WorkStateModel> models = ObjectConverter<WorkState, WorkStateModel>.ConvertList(context.WorkStates.ToList()).ToList();
//            return models;
//        }

//        public WorkStateModel GetWorkState(int id)
//        {
//            if (id == 0)
//            {
//                return new WorkStateModel() { IsActive = true };
//            }
//            WorkState workState = context.WorkStates.Find(id);
//            return ObjectConverter<WorkState, WorkStateModel>.Convert(workState);
//        }



//        public bool SaveWorkState(int id, WorkStateModel model)
//        {
//            if (model == null)
//            {
//                throw new Exception("Task State data missing!");
//            }


//            WorkState workState = ObjectConverter<WorkStateModel, WorkState>.Convert(model);
//            if (id > 0)
//            {
//                workState = context.WorkStates.FirstOrDefault(x => x.WorkStateId == id);
//                if (workState == null)
//                {
//                    throw new Exception("Task State not found!");
//                }
//                workState.ModifiedDate = DateTime.Now;
//                workState.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
//            }

//            else
//            {
//                workState.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
//                workState.CreatedDate = DateTime.Now;

//            }

//            workState.State = model.State;
//            workState.Remarks = model.Remarks;
//            workState.OrderNo = model.OrderNo;
//            workState.IsActive = model.IsActive;
//            context.Entry(workState).State = workState.WorkStateId == 0 ? EntityState.Added : EntityState.Modified;
//            return context.SaveChanges() > 0;
//        }


//        public List<SelectModel> GetManagerWorkStateSelectModels()
//        {
//            return context.WorkStates.Where(x => x.IsActive && x.StateType.Equals("M")).ToList().Select(x => new SelectModel()
//            {
//                Text = x.State,
//                Value = x.WorkStateId
//            }).OrderBy(x => x.Text).ToList();
//        }
//        public List<SelectModel> GetMemberWorkStateSelectModels(int id)
//        {
//            return context.WorkStates.Where(x => x.IsActive && x.StateType.Equals("E")).ToList().Select(x => new SelectModel()
//            {
//                Text = x.State,
//                Value = x.WorkStateId
//            }).OrderBy(x => x.Text).ToList();
//        }
//    }
//}
