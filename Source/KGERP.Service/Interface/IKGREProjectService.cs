using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IKGREProjectService : IDisposable
    {
        List<KGREProjectModel> GetKGREProjects(string searchText);
        List<KGREProjectModel> GetKGREPlotList(string searchText);
        List<KGREProjectModel> GetKGREPlotListByPlotId(int plotId);
        List<SelectModel> GetProjects(int? companyId);
        List<SelectModel> GetKGREPlots(int? projectId);
        KGREProjectModel GetKGREProject(int id);
        KGREProjectModel GetKGREPlot(int id);
        List<KGREProjectModel> GetKGREProjectByCompanyId(int id);
        bool SaveKGREProject(int id, KGREProjectModel model);
        bool SaveKGREPlot(int id, KGREProjectModel model);
        bool DeleteKGREProject(int id);
    }
}
