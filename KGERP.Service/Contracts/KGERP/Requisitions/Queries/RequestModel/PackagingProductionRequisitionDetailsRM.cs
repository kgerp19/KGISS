using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Contracts.KGERP.Requisitions.Queries.RequestModel
{
    public class PackagingProductionRequisitionDetailsRM
    {
        public string CompanyName { get; set; }
        public int RequisitionId { get; set; }
        public string RequisitionNo { get; set; }
        public string RequisitionBy { get; set; }
        public DateTime RequisitionDate { get; set; }
        public string Description { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string StockName { get; set; }
        public int FromRequisitionId { get; set; }
        public string FromRequisitionName { get; set; }
        public int ToRequisitionId { get; set; }
        public string ToRequisitionName { get; set; }
        public int OrderDetailsId { get; set; }
        public int CompanyId { get; set; }
        public int ProductId { get; set; }
        public bool IsSubmited { get; set; }
        //For View
        public decimal? Qty { get; set; }
        public Guid? RequisitionItemDetailId { get; set; }
        public int CustomerId { get; set; }
        public int FinishProductBOMId { get; set; }
        public long OrderMasterId { get; set; }
        public string ProductNames { get; set; }
        public string RawItem { get; set; }
        public List<SelectListItem> GetDDLOrderNo { get; set; }
        public List<SelectListItem> GetDDLOrderDetailsId { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }

        public List<RequisitionItemDetailListRM> RequisitionItemDetail { get; set; }

        public List<SelectListItem> DDLProductList { get; set; }
        public string CustomerName { get;  set; }
        public string JobOrderNo { get;  set; }
    }

    public class RequisitionItemDetailListRM
    {
        public int RequisitionId { get; set; }
        public int RProductId { get; set; }
        public string ProductName { get; set; }
        public decimal RQty { get; set; }
        public bool IsActive { get; set; }
        public Guid RequistionItemDetailId { get; set; }
        public decimal AllocatedQty { get;  set; }
    }
}
