using KGERP.Service.Configuration;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.Production
{

    public partial class VMProdReference : BaseVM
    {
        public int ProdReferenceId { get; set; }
        public long ProductionId { get; set; }
        public int ProdReferenceSlaveID { get; set; }
        public string ManagerReferenceName { get; set; }
        public string ReferenceNo { get; set; }
        public string ProductionNo { get; set; }
        public string CustomerPONo { get; set; }
        public string OrderMaterNo { get; set; }
        [Required]
        public DateTime ReferenceDate { get; set; }
        public DateTime ProductionDate { get; set; }
        [Required]
        public DateTime ExpensesDate { get; set; }

        [Required]
        public DateTime ProductionItemDate { get; set; }
        public bool IsSubmitted { get; set; }
        public bool? IsAuthorized { get; set; }

        public string CompanyName { get; set; }
        public string CustomerPhone { get; set; }

        public string ManagerName { get; set; }
        [Required]
        public string CustomerBy { get; set; }
        [Required]
        public long Order { get; set; }
        public string JobNo { get; set; }
        public string ReelDirection { get; set; }
        public string PouchDerection { get; set; }
        public long OrderDetailId { get; set; }
        [Required]
        public int ManagerBy { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public decimal TotalRawConsumedAmount { get; set; }
        public decimal TotalFactoryExpensessAmount { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime? JobDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public int PaymentMethod { get; set; }
        public IEnumerable<VMProdReference> DataList { get; set; }
        public SelectList PoList { get; set; } = new SelectList(new List<object>());
        public SelectList VoucherPaymentMethodList
        {
            get { return new SelectList(BaseFunctionalities.GetEnumList<CheckType>(), "Value", "Text"); }

        }

    }
    public partial class VMProdReferenceSlave : VMProdReference
    {
        public int ProdReferenceSlaveID { get; set; }
        public long ProductionItemId { get; set; }
        public bool IsVoucher { get; set; }
        public int FProductId { get; set; }
        public int RProductId { get; set; }
        public decimal RawConsumeQuantity { get; set; }
        public int? FactoryExpensesHeadGLId { get; set; }
        public int? AdvanceHeadGLId { get; set; }
        public string AdvanceHeadGLName { get; set; }
        public long OrderDetailId { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Required Quantity must be greater than 0.")]
        public decimal Quantity { get; set; }
        public double OrderQty { get; set; }
        public double ProductionQty { get; set; }
        public decimal QuantityOver { get; set; }
        public decimal QuantityLess { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal PriviousProcessQuantity { get; set; }
        public decimal ExcessQuantity { get; set; }
        public string ChallanNo { get; set; }
        public string OrderMasterNo { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public long OrderMasterId { get; set; }
        public int MaterialReceiveFk { get; set; }
        public IEnumerable<VMProdReferenceSlave> DataListSlave { get; set; }

        public IEnumerable<VMProdReferenceSlave> RawDataListSlave { get; set; }
        public IEnumerable<VMProdReferenceSlave> FinishDataListSlave { get; set; }
        public int? AccountingHeadId { get; set; }
        public string ProductName { get; set; }
        public string FactoryExpecsesHeadName { get; set; }

        public string RawProductName { get; set; }

        public string UnitName { get; set; }
        public bool MakeFinishInventory { get; set; }
        public decimal CostingPrice { get; set; }
        public decimal FectoryExpensesAmount { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal PurchasePrice { get; set; }

        public int ResultFlg { get; set; }
        public SelectList FactoryExpensesList { get; set; } = new SelectList(new List<object>());
        public SelectList PaymentList { get; set; } = new SelectList(new List<object>());
        public SelectList AdvanceHeadList { get; set; } = new SelectList(new List<object>());

        public List<VMProdReferenceSlaveConsumption> RowProductConsumeList { get; set; }
        public List<VMProdReferenceSlave> DataToList { get; set; }
        public bool IsVATInclude { get;  set; }
        public double UnitPrice { get;  set; }
        public decimal VATPercent { get;  set; }
        public int ProductCategoryId { get; set; }
        public string LotNumber { get; set; }
    }
    public partial class VMProdReferenceSlaveConsumption : BaseVM
    {
        public int ProdReferenceSlaveConsumptionID { get; set; }
        public int ProdReferenceSlaveID { get; set; }
        public int RProductId { get; set; }
        public string RProductName { get; set; }
        public string FProductName { get; set; }
        public string RSubCategoryName { get; set; }
        public string RCategoryName { get; set; }

        public string UnitName { get; set; }
        public string FUnitName { get; set; }

        public decimal TotalConsumeQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal COGS { get; set; }
        public decimal TotalCOGS { get; set; }

        public int? AccountingHeadId { get; set; }
        public string LotNumber { get;  set; }

    }


    public partial class AccountList
    {
        public int? AccountingHeadId { get; set; }

        public decimal Value { get; set; }



    }
}
