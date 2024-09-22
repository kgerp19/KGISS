using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class RequisitionModel
    {

        public int RequisitionId { get; set; }
        [Required]
        [Display(Name = "Requisition No")]
        public string RequisitionNo { get; set; }
        [Display(Name = "Requisition Date")]
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> RequisitionDate { get; set; }
        [Display(Name = "Requisition By")]
        public string RequisitionBy { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        [Display(Name = "Delivery No")]
        public string DeliveryNo { get; set; }
        [Display(Name = "Status")]
        public string RequisitionStatus { get; set; }
        public string DeliveredBy { get; set; }
        [Required]
        [Display(Name = "Delivery Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DeliveredDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public virtual ICollection<RequisitionItemModel> RequisitionItems { get; set; }
        public List<RequisitionItemModel> Items { get; set; } = new List<RequisitionItemModel>();

        public virtual ICollection<RequisitionDeliverModel> ProductionDeliveries { get; set; }
        public virtual ICollection<StoreModel> Stores { get; set; }
        public IEnumerable<RequisitionModel> DataList { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }

        public IEnumerable<RequisitionItemModel> RequisitionItemDataList { get; set; }
        public IEnumerable<RequisitionItemModel> BagDataList { get; set; }

        public IEnumerable<RequisitionItemDetailModel> RequisitionItemDetailDataList { get; set; }
        //----------------Extendex Properties-------------------
        public string FormulaMessage { get; set; }

        public bool IsSubmitted { get; set; }
        public string IntegratedFrom { get; set; }

        public ActionEnum ActionEum { get { return (ActionEnum)this.ActionId; } }
        public int ActionId { get; set; } = 1;

        public int Status { get; set; }
        public EnumReqStatus EnumStatus { get { return (EnumReqStatus)this.Status; } }

        // for item details
        public long RequisitionItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CurrentDate { get; set; }
        public decimal Qty { get; set; }
        public decimal InputQty { get; set; }
        public decimal OutputQty { get; set; }
        public decimal ActualProcessLoss { get; set; }
        public decimal OverHead { get; set; }
        public decimal ProcessLoss { get; set; }

    }




}