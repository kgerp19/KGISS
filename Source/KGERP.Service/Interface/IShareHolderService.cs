using KGERP.Service.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IShareHolderService
    {
        Task<ShareHolderModel> GetShareHolders(int companyId);
        bool BulkSave(List<ShareHolderModel> shareHolders);
        IQueryable<ShareHolderModel> GetAllShareHolders(string searhValue, out int count);
        ShareHolderModel GetShareHolder(int id);
        bool SaveShareHolder(long id, ShareHolderModel shareHolder, out string message);
        //bool GLBulkSave(List<AccGLModel> gls);
    }
}
