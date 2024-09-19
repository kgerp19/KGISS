using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class IngredientStandardViewModel
    {
        public IngredientStandardModel IngredientStandard { get; set; }
        public IngredientStandardDetailModel IngredientStandardDetail { get; set; }
        public List<IngredientStandardDetailModel> IngredientStandardDetails { get; set; }
        public List<SelectModel> ProductSubCategories { get; set; }
        public List<SelectModel> Products { get; set; }
    }
}