using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IAssetCategoryService : IDisposable
    {
        List<AssetCategoryModel> GetAssetCategory();
        AssetCategoryModel GetAssetCategoryById(int id);
        bool SaveOrEdit(int id, AssetCategoryModel asset);
        List<AssetTypeModel> GetAssetType();
        AssetTypeModel GetAssetTypeById(int id);
        List<SelectModel> GetAssetCategorySelectModels();
        string GetSerialNo(int assetCategoryId);
        bool SaveOrEditAssetSubType(int id, AssetTypeModel asset);
    }
}
