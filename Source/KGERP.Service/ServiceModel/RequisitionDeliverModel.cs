using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class RequisitionDeliverModel
    {
        public int RequisitionDeliverId { get; set; }
        public Nullable<int> RequisitionId { get; set; }
        [Display(Name = "Delivery Date")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime RequisitionDeliverDate { get; set; }
        public string RequisitionDeliverBy { get; set; }
        public Nullable<int> Status { get; set; }
        public string ChallanNo { get; set; }

        public virtual RequisitionModel Requisition { get; set; }

        public virtual ICollection<RequisitionDeliverItemModel> RequisitionDeliverItems { get; set; }
    }
}
