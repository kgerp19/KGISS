using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
    public class VMMaterialRcvList
    {
        public long MaterialReceiveId { get; set; }
        public int CompanyId { get; set; }
        public long PurchaseOrderId { get; set; }
        public string MaterialType { get; set; }
        public string ReceiveNo { get; set; }
        public Nullable<long> ReceivedBy { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public Nullable<System.DateTime> ChallanDate { get; set; }
        public string MaterialReceiveStatus { get; set; }
        public bool IsSubmitted { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string Remarks { get; set; }
    }
}
