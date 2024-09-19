using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Contracts.KGERP.Requisitions.Queries.RequestModel
{
    public class ResuisitionRM
    {
        public string CreadedBy { get; set; }
        public string RequisitionNo { get; set; }
        public string RequisitionStatus { get; set; }
        public string OrderNo { get; set; }
        public string JobOrderNo { get; set; }
        public string FromRequisitionName { get; set; }
        public string ToRequisitionName { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Structure { get; set; }
        public int FromRequisitionId { get; set; }
        public double Qty { get; set; }
        public int RequisitionId { get; set; }
        public int ToRequisitionId { get; set; }
        public int OrderDetailsId { get; set; }
        public long OrderMasterId { get; set; }
        public DateTime? RequisitionDate { get; set; }
        public DateTime OrderDate { get; set; }
        public string RequisitionDateMsg { get { return RequisitionDate.Value.ToString("dd-MMM-yyyy"); } }

        public List<SelectListItem> DDLGerOrderList { get; set; }
    }
}
