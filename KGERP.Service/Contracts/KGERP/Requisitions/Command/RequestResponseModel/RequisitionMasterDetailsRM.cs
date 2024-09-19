using KGERP.Data.Models;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Contracts.KGERP.Requisitions.Command.RequestResponseModel
{
    public class RequisitionMasterDetailsRM : CommonModel
    {
        public string RequisitionNo { get; set; }
        public string RequisitionBy { get; set; }
        public DateTime RequisitionDate { get; set; }
        public string Description { get; set; }
        public int FromRequisitionId { get; set; }
        public int ToRequisitionId { get; set; }
        public int OrderDetailsId { get; set; }
        public int CompanyId { get; set; }

        public List<RequisitionItemDetailRM> RequisitionItemDetail { get; set; }
    }


    public class RequisitionItemDetailRM
    {
        public int RequisitionId { get; set; }
        public int FinishProductBOMId { get; set; }
        public int RProductId { get; set; }
        public decimal RQty { get; set; }
        public bool IsActive { get; set; }
    }
}
