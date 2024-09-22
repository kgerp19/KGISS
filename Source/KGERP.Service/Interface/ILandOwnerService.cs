using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface ILandOwnerService
    {
        List<LandReceiverModel> GetLandReceiver(string searchText);
        List<LandUserModel> GetLandUser(string searchText);

        LandUserModel GetLandUser(int id);
        LandReceiverModel GetLandReceiver(int id);

        bool SaveLandReceiver(int id, LandReceiverModel model);
        bool SaveLandUser(int id, LandUserModel model);
    }
}
