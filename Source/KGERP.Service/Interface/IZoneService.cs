using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IZoneService
    {
        List<SelectModel> GetZoneSelectModels(int companyId);
        List<SelectModel> GetSubZoneSelectModelsByZone(int zoneId);
        decimal GetCarryingRate(int? customerId);
        List<ZoneModel> GetZones(int companyId, string searchText);
        bool SaveZone(int zoneId, ZoneModel model);
        ZoneModel GetZone(int id);


        List<SubZoneModel> GetSubZones(int companyId, string searchText);
        bool SaveSubZone(int id, SubZoneModel model);
        SubZoneModel GetSubZone(int id);
        bool DeleteSubZone(int id, int companyId);
    }
}
