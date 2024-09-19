using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IPFDataService : IDisposable
    {
        List<PFDataModel> GetPFDatas(string searchText);
        List<SelectModel> GetPFDatas();
        PFDataModel GetPFData(int id);
        bool SavePFData(int id, PFDataModel model);
        bool DeletePFData(int id);
    }
}
