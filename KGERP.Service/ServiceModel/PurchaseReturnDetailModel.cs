using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
   public class PurchaseReturnDetailModel
    {
        public long PurchaseReturnDetailId { get; set; }
        public long PurchaseReturnId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
    }
}
