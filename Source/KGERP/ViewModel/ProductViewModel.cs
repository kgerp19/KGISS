using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{

    public class ProductViewModel
    {
        public ProductModel Product { get; set; }
        public List<SelectModel> ProductCategories { get; set; }
        public List<SelectModel> ProductSubCategories { get; set; }
        public List<SelectModel> Units { get; set; }
        public string ProductType { get; set; }
        public string PackName { get; set; }

    }
}