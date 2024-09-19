using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class ProductDetailViewModel
    {
        public ProductDetailModel ProductDetail { get; set; }
        public List<ProductDetailModel> ProductDetails { get; set; }
        public List<SelectModel> Product { get; set; }
    }
}