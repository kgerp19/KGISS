using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IECMemberService
    {
        List<ECMemberModel> GetECMembers(string searchText);
        ECMemberModel GetECMember(int id);
        bool SaveECMember(int v, ECMemberModel model);
    }
}
