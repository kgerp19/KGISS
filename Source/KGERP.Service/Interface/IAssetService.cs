using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IAssetService : IDisposable
    {
        Task<AssetModelVm> GetOfficeAssets(int? companyId);

        List<AssetModel> Index();
        List<OfficeAssetModel> FinalAssetList();
        OfficeAssetModel AssetDetails(int id);
        bool SaveOrEditAsset(OfficeAssetModel model);
        bool SaveOrEditKGLandAsset(AssetModel model);
        OfficeAssetModel GetFinalAsset(int id);

        List<AssetModel> LandIndex(string searchText);
        Task<VMAsset> LandList(int companyId);
        Task<AssetModel2> GetLandList(int companyId, int? landReceiverId, int? districtId, int? upazilaId, int? selectedCompanyId);

        List<AssetModel> KGLandList(string searchText);
        List<SelectModel> Company();
        List<SelectModel> AssetLocation();
        List<SelectModel> AssetSubLocation(int locationId);
        List<SelectModel> AssetSubLocationByLocationId(int? locationId);
        List<SelectModel> AssetCategory();
        List<SelectModel> AssetType(int categoryId);
        List<SelectModel> AssetTypeByCategoryId(int? categoryId);
        List<SelectModel> AssetStatus();
        List<SelectModel> Colour();
        bool SaveOrEdit(AssetModel model);
        AssetModel GetAsset(int id);
        AssetModel GetKGAsset(int id);
        AssetModel LandDetails(int id);
        List<SelectModel> DisputedList();
        List<SelectModel> Project();
        List<SelectModel> LandReceiver();
        List<SelectModel> LandUser();



    }
}
