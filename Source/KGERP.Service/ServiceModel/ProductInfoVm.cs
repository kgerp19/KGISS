using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
   public class ProductInfoVm
    {
        public decimal? UnitPrice { get; set; }
        public decimal? CostingPrice { get; set; }
        public double? PackSize { get; set; }
        public string UnitName { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductModel { get; set; }
        public string HorsePower { get; set; }
      
    }
}
