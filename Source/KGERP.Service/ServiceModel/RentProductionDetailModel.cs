using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
    public class RentProductionDetailModel
    {
        public long RentProductionDetailId { get; set; }
        public Nullable<int> RentProductionId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public decimal Rate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
    }
}
