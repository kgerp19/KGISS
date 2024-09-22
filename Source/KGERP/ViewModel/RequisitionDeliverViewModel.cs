using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class RequisitionDeliverViewModel
    {
        public ICollection<RequisitionItemModel> RequisitionItems { get; set; }
        public ICollection<RequisitionItemDetailModel> RequisitonItemDetails { get; set; }
        public RequisitionDeliverModel RequisitionDeliver { get; set; }
        public RequisitionModel Requisition { get; set; }
    }
}