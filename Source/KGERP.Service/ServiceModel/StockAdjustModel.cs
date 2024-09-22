using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel
{
    public class StockAdjustModel
    {
        public int StockAdjustId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Invoice No")]
        public string InvoiceNo { get; set; }
        [DisplayName("Adjust Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime AdjustDate { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<StockAdjustDetailModel> StockAdjustDetails { get; set; }
        public bool IsSubmited { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<StockAdjustModel> DataList { get; set; }

    }

    public class IssueVm
    {
        public int IssueId { get; set; }
        public bool IsSubmited { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public int CompanyId { get; set; }

        public string IssueToName { get; set; }
        public string IssueNo { get; set; }
        public string IssueDateTxt { get; set; }
        public DateTime IssueDate { get; set; }
        public int TypeId { get; set; }
        public long IssueTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }
        public EnumIssueStatus EnumStatus { get { return (EnumIssueStatus)this.Status; } }
        public SelectList EnumStatusList { get { return new SelectList(BaseFunctionalities.GetEnumList<EnumPOStatus>(), "Value", "Text"); } }

        public IEnumerable<IssueVm> DataList { get; set; }

        public string Remarks { get; set; }

        public ActionEnum ActionEum { get { return (ActionEnum)this.ActionId; } }
        public int ActionId { get; set; } = 1;

        // for issue details 
        public int IssueDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitName { get; set; }
        public decimal Quantity { get; set; }
        public List<IssusDetailVm> Items { get; set; } = new List<IssusDetailVm>();
    }

    public class IssusDetailVm
    {
        public int IssueDetailId { get; set; }
        public int IssueId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public bool IsActive { get; set; }

    }
   

}
