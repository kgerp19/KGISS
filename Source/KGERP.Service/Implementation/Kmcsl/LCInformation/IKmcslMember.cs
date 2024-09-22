using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.LcInfoServices.LCInformation
{
    public interface IShereMember
    {
        //long AddShereMember(ShereMemberModel model);
        //ShereMemberModel ShereMemberDetails(long id);
        ShereMemberModel ShereMemberList(int companyId);
        //int ShereMemberDelete(ShereMemberModel model);
        //Task<int> ShereMemberSubmit(ShereMemberModel model);
        //long UpdateShereMember(ShereMemberModel model);
    }
}
