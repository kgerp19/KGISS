using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.General_Requisition
{
    public class GeneralRequisitionMasterDetailsVM
    {
        public long Id { get; set; }
        public long RequisitionId { get; set; }
        public string ProductName { get; set; }
        public string UniName { get; set; }
        public decimal Quantity { get; set; }
        public int UnitId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal DiscountProductParcent { get; set; }

    }
}
