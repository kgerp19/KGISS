using KGERP.Data.Models;
using KGERP.Service.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using KGERP.Service.Implementation.TaskManagment;

namespace KGERP.Service.Implementation.Configuration
{
    public class TaskConfigurationService
    {
        private readonly ERPEntities _db;
        public TaskConfigurationService(ERPEntities db)
        {
            _db = db;
        }
        #region WorkState
        public async Task<VMWorkState> WorkStateGet(long? workboardid)
        {
            VMWorkState vMWorkState = new VMWorkState();
            vMWorkState.DataList = (from t1 in _db.WorkStates
                                    where t1.IsActive == true && t1.WorkBoardId == workboardid
                                    select new VMWorkState
                                    {
                                        StateId = t1.WorkStateId,
                                        StateName = t1.State,
                                        IsActive = t1.IsActive,
                                        CreatedBy = t1.CreatedBy
                                    }).OrderByDescending(x => x.StateId).AsEnumerable();
            return vMWorkState;
        }
        public async Task<int> WorkStateAdd(VMWorkState vMWorkState)
        {
            var result = -1;


            WorkState workState = new WorkState
            {
                State = vMWorkState.StateName,
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedDate = DateTime.Now,
                IsActive = true,
                WorkBoardId = (long)vMWorkState.WokboardId
               
            };
            _db.WorkStates.Add(workState);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = workState.WorkStateId;
            }
            return result;
        }
        public async Task<int> WorkStateEdit(VMWorkState vMWorkState)
        {
            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    var workState = await _db.WorkStates.FindAsync(vMWorkState.StateId);
                    if (workState == null)
                    {
                        return -1;
                    }

                    workState.State = vMWorkState.StateName;
                    await _db.SaveChangesAsync();

                    transaction.Commit();
                    return workState.WorkStateId;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<int> WorkStateDelete(int id)
        {
            int result = -1;

            if (id != 0)
            {
                using (DbContextTransaction dbTran = _db.Database.BeginTransaction())
                {
                    try
                    {
                        WorkState workState = _db.WorkStates.Find(id);
                        workState.IsActive = false;

                        await _db.SaveChangesAsync();
                        result = workState.WorkStateId;
                        dbTran.Commit();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
                    }
                }
            }
            return result;
        }
        #endregion
        #region WorkLabel
        public async Task<VMWorkLabel> WorkLabelGet(long? workboardid)
        {
            VMWorkLabel vMWorkLabel = new VMWorkLabel();
            vMWorkLabel.DataList = (from t1 in _db.WorkLabels
                                    where t1.IsActive == true && t1.WorkBoardId== workboardid
                                    select new VMWorkLabel
                                    {
                                        WorkLabelId = t1.WorkLabelId,
                                        WorkLabelName = t1.Name,
                                        ColorName = t1.ColorClass,
                                        IsActive = t1.IsActive,
                                        CreatedBy = t1.CreatedBy
                                    }).OrderByDescending(x => x.WorkLabelId).AsEnumerable();
            return vMWorkLabel;
        }
        public async Task<long> WorkLabelAdd(VMWorkLabel model)
        {
            long result = -1;
            WorkLabel label = new WorkLabel();
            var lastEntry = _db.WorkLabels.OrderByDescending(x => x.WorkLabelId).Select(x => x.WorkLabelId).FirstOrDefault();
            if(lastEntry == null)
            {
                label.Name = model.WorkLabelName;
                label.ColorClass = model.ColorName;
                label.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                label.CreatedDate = DateTime.Now;
                label.IsActive = true;
               
            }
            else
            {
                label.Name = model.WorkLabelName;
                label.ColorClass = model.ColorName;
                label.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                label.CreatedDate = DateTime.Now;
                label.IsActive = true;
                label.WorkLabelId = lastEntry + 1;
                if (model.WokboardId > 0)
                {
                    label.WorkBoardId = (long)model.WokboardId;
                }
            }
            
            _db.WorkLabels.Add(label);
            if (await _db.SaveChangesAsync() > 0)
            {
                result = label.WorkLabelId;
            }
            return result;
        }
        public async Task<long> WorkLabelEdit(VMWorkLabel model)
        {
            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    var workState = await _db.WorkLabels.FindAsync(model.WorkLabelId);
                    if (workState == null)
                    {
                        return -1;
                    }

                    workState.Name = model.WorkLabelName;
                    workState.ColorClass = model.ColorName;
                    await _db.SaveChangesAsync();

                    transaction.Commit();
                    return workState.WorkLabelId;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<long> WorkLabelDelete(long id)
        {
            long result = -1;

            if (id >= 0)
            {
                using (DbContextTransaction dbTran = _db.Database.BeginTransaction())
                {
                    try
                    {
                        WorkLabel workLabel = _db.WorkLabels.Find(id);
                        workLabel.IsActive = false;

                        await _db.SaveChangesAsync();
                        result = workLabel.WorkLabelId;
                        dbTran.Commit();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
