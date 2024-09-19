using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface ISubZoneService
    {
        List<SelectModel> GetSubZoneSelectModelsByZone(int? zoneId);
        decimal GetCarryingRate(int? customerId);
    }
}
