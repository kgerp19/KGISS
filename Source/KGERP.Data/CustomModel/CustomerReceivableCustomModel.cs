using System;
using System.ComponentModel;

namespace KGERP.Data.CustomModel
{
    public class CustomerReceivableCustomModel
    {
        public long OrderDetailId { get; set; }
        public int VendorId { get; set; }
        public string Customer { get; set; }
        [DisplayName("Customer Code")]
        public string CustomerCode { get; set; }
        [DisplayName("Order No")]
        public string OrderNo { get; set; }
        [DisplayName("Delivered Date")]
        public DateTime DeliveryDate { get; set; }
        [DisplayName("Challan No")]
        public string ChallanNo { get; set; }
        [DisplayName("Vehicle No")]
        public string VehicleInfo { get; set; }
        [DisplayName("Amount")]
        public decimal DeliverAmount { get; set; }
    }
}
