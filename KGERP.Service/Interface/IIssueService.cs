using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IIssueService
    {
        List<IssueDetailInfoModel> GetRmProducts(int productId, decimal qty);
        bool SaveIssueInformation(IssueMasterInfoModel model);
    }
}
