using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IIngredientStandardService
    {
        List<IngredientStandardModel> GetIngredientStandards(int companyId, string searchText);
        IngredientStandardModel GetIngredientStandard(int ingredientStandardId);
        IngredientStandardDetailModel GetIngredientStandardDetail(int v);
        List<IngredientStandardDetailModel> GetIngredientStandardDetails(int ingredientStandardId);
        bool SaveIngredientStandardDetail(int id, IngredientStandardDetailModel ingredientStandardDetail);
        bool DeleteIngredientStandardDetail(int ingredientStandardDetailId);
        bool SaveIngredientStandard(int id, IngredientStandardModel ingredientStandard, out string message);
    }
}
