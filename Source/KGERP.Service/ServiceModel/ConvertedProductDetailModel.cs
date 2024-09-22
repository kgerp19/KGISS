using KGERP.Data.Models;
using System;

namespace KGERP.Service.ServiceModel
{
    public class ConvertedProductDetailModel
    {
        public Nullable<int> ConvertedProductDetailId { get; set; }
        public Nullable<int> ConvertedProductId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public int Qty { get; set; }

        public virtual ConvertedProduct ConvertedProduct { get; set; }
        public virtual Product Product { get; set; }
    }
}
