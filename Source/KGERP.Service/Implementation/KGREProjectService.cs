using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class KGREProjectService : IKGREProjectService
    {
        private bool disposed = false;
        ERPEntities context = new ERPEntities();
        public bool DeleteKGREProject(int id)
        {
            throw new NotImplementedException();
        }

        public KGREProjectModel GetKGREProject(int id)
        {
            if (id == 0)
            {
                return new KGREProjectModel() { ProjectId = id };
            }
            KGREProject kGREProject = context.KGREProjects.Find(id);
            return ObjectConverter<KGREProject, KGREProjectModel>.Convert(kGREProject);
        }

        public List<KGREProjectModel> GetKGREProjectByCompanyId(int CompanyId)
        {
            IQueryable<KGREProject> KGREProjects = context.KGREProjects.Where(x => x.CompanyId == CompanyId);
            return ObjectConverter<KGREProject, KGREProjectModel>.ConvertList(KGREProjects.ToList()).ToList();
        }

        public List<KGREProjectModel> GetKGREProjects(string searchText)
        {
            IQueryable<KGREProject> KGREProjects = context.KGREProjects.Where(x => x.ProjectName.Contains(searchText) || x.Remarks.Contains(searchText) || x.Address.Contains(searchText)).OrderBy(x => x.ProjectId);
            return ObjectConverter<KGREProject, KGREProjectModel>.ConvertList(KGREProjects.ToList()).ToList();
        }

        #region Plot Information

        public KGREProjectModel GetKGREPlot(int id)
        {
            if (id == 0)
            {
                return new KGREProjectModel() { PlotId = id };
            }
            KGREPlot kGREPlot = context.KGREPlots.Find(id);
            return ObjectConverter<KGREPlot, KGREProjectModel>.Convert(kGREPlot);

        }
        public List<KGREProjectModel> GetKGREPlotList(string searchText)
        {
            dynamic result = context.Database.SqlQuery<KGREProjectModel>("exec KGRE_PlotList").ToList();
            return result;
        }

        public List<SelectModel> GetProjects(int? companyId)
        {
            return context.KGREProjects.Where(x => x.CompanyId == companyId).ToList().Select(x => new SelectModel()
            {
                Text = x.ProjectName.ToString(),
                Value = x.ProjectId
            }).ToList();
        }
        public List<KGREProjectModel> GetKGREPlotListByPlotId(int plotId)
        {
            return context.KGREPlots.Where(x => x.PlotId == plotId).ToList().Select(x => new KGREProjectModel()
            {
                BlockNo = x.BlockNo,
                PlotFace = x.PlotFace,
                PlotSize = x.PlotSize,
                PlotNo = x.PlotNo,
                PlotId = x.PlotId
            }).ToList();
        }
        public List<SelectModel> GetKGREPlots(int? projectId)
        {
            return context.KGREPlots.Where(x => x.ProjectId == projectId).ToList().Select(x => new SelectModel()
            {
                Text = x.PlotNo.ToString(),
                Value = x.PlotId
            }).ToList();
        }
        public bool SaveKGREPlot(int id, KGREProjectModel model)
        {
            KGREPlot _KGREPlot = ObjectConverter<KGREProjectModel, KGREPlot>.Convert(model);
            if (id > 0)
            {
                _KGREPlot.ModifiedDate = DateTime.Now;
                _KGREPlot.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }
            else
            {
                _KGREPlot.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                _KGREPlot.CreatedDate = DateTime.Now;
            }

            _KGREPlot.PlotSize = model.PlotSize;
            _KGREPlot.BlockNo = model.BlockNo;
            _KGREPlot.PlotFace = model.PlotFace;
            _KGREPlot.ProjectId = model.ProjectId;
            _KGREPlot.ProjectBooking = model.ProjectBooking;
            _KGREPlot.PlotStatus = model.PlotStatus;
            _KGREPlot.Remark = model.Remark;

            context.Entry(_KGREPlot).State = _KGREPlot.PlotId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }


        #endregion
        public bool SaveKGREProject(int id, KGREProjectModel model)
        {
            KGREProject _KGREProject = ObjectConverter<KGREProjectModel, KGREProject>.Convert(model);
            if (id > 0)
            {
                _KGREProject.ModifiedDate = DateTime.Now;
                _KGREProject.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }
            else
            {
                _KGREProject.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                _KGREProject.CreatedDate = DateTime.Now;
            }

            _KGREProject.ProjectName = model.ProjectName;
            _KGREProject.Address = model.Address;
            _KGREProject.CompanyId = model.CompanyId;
            _KGREProject.TotalFlat = model.TotalFlat;
            _KGREProject.UnitRate = model.UnitRate;
            _KGREProject.Remarks = model.Remarks;

            context.Entry(_KGREProject).State = _KGREProject.ProjectId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }
    }
}
