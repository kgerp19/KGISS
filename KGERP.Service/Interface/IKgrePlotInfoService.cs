using KGERP.Data.Models;

namespace KGERP.Service.Interface
{
    public interface IKgrePlotInfoService
    {
        object LoadProjectName(int companyId);
        object LoadPlotStatus();
        object LoadPlotInfo();
        object LoadData(int id);
        //int SaveProjectInfo(PloatInfoSetup BasicInfo);
        int SaveProjectInfo(KGREPlot kGREPlot);

    }
}
