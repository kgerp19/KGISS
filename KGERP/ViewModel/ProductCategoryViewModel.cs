using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class ProductSubCategoryViewModel
    {
        public ProductSubCategoryModel ProductSubCategory { get; set; }
        public string ProductType { get; set; }
        public List<SelectModel> ProductCategories { get; set; }
    }
}