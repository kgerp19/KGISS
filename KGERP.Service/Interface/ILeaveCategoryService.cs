using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface ILeaveCategoryService
    {
        List<LeaveCategoryModel> GetLeaveCategories();
        List<SelectModel> GetLeaveCategorySelectModels();
        LeaveCategoryModel GetLeaveCategory(int leaveCategoryId);
        bool SaveLeaveCategory(int id, LeaveCategoryModel model);
    }
}
