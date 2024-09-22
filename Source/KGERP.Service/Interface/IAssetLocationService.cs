using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{

    //gjhgjg

    public interface IAssetLocationService : IDisposable
    {
        List<AssetLocationModel> GetLocation();


        AssetLocationModel GetAssetLocationById(int id);
        bool SaveOrEdit(int id, AssetLocationModel asset);
        List<AssetSubLocationModel> GetSubLocation();
        AssetSubLocationModel GetAssetSubLocationById(int id);
        List<SelectModel> GetLocationSelectModels();
        string GetSerialNo(int locationId);
        bool SaveOrEditAssetSubLocation(int id, AssetSubLocationModel asset);
    }
}
