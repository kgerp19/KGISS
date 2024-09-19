using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IAssignAssetService : IDisposable
    {
        List<AssetAssignModel> Index();
        List<SelectModel> Company();
        List<SelectModel> AssetLocation();
        List<SelectModel> AssetSubLocation(int locationId);
        List<SelectModel> AssetCategory();
        List<SelectModel> AssetType(int categoryId);
        List<SelectModel> SerialNo(int assetTypeId, int companyId);

        bool SaveOrEdit(AssetAssignModel model);

        AssetAssignModel GetAssetAssignById(int id);
    }
}
