using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IUpazilaAssignService
    {
        List<UpazilaAssignModel> GetUpazilaListByDistrictAndEmployee(int districtId, long employeeId);
        bool SaveUpazilaAssign(List<UpazilaAssignModel> upazilaAssigns);
    }
}
