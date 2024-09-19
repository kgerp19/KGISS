using KGERP.Data.Models;
using KGERP.Services.Procurement;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.LcInfoServices
{
    public class LcInfoViewModel
    {
        public long LCId { get; set; }
        public long LCAmendmentId { get; set; }
        public string LCNo { get; set; }
        public DateTime LCDate { get; set; }
        public DateTime LCDateAmmendMend { get; set; }
        public int CompanyId { get; set; }
        public int BankOrHeahGL { get; set; }
        public string CompanyNmae { get; set; }
        public int? CompanyBankBranchId { get; set; }
        public int? AdvanceOrLoanGLId { get; set; }
        public string CompanyBankNmae { get; set; }
        public int SupplierId { get; set; }
        public int SupplierBankBranchId { get; set; }
        public string SupplierBankName { get; set; }
        public string SupplierName { get; set; }
        public string SupplierPhone { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierEmail { get; set; }
        public int LCType { get; set; }
        public string PINo { get; set; }
        public DateTime? PIDate { get; set; }
        public decimal LCValue { get; set; }
        public decimal LCValueAmendMendt{ get; set; }
        public decimal LCValueInBDT { get; set; }
        public decimal LCValueInBDTAmendMendt { get; set; }
        public int ProductOriginId { get; set; }
        public int AcCostCenterId { get; set; }
        public int CountryofOriginId { get; set; }
        public string CountryofOriginName { get; set; }
        public int PortOfLoadingId { get; set; }
        public string PortOfLoadingName { get; set; }
        public int CountryOfDestinationId { get; set; }
        public string CountryOfDestinationName { get; set; }
        public int PortOfDischargeId { get; set; }
        public string PortOfDischargeName { get; set; }
        public string NotefyParty { get; set; }
        public DateTime ShipmentDate { get; set; }
        public DateTime ExparyDate { get; set; }
        public decimal AmendmentIncrase { get; set; }
        public decimal AmendmentDiccrase { get; set; }
        public decimal OtherCharge { get; set; }
        public decimal OtherChargeAmendMendt { get; set; }
        public decimal CashMarginPercent { get; set; }
        public decimal CashMarginPercentAmendMendt { get; set; }
        public decimal CashMarginAmount { get; set; }
        public decimal CashMarginAmountAmendMendt { get; set; }
        public decimal BankCharge { get; set; }
        public decimal BankChargeAmendMendt { get; set; }
        public decimal FreightCharges { get; set; }
        public decimal FreightChargesAmendMendt { get; set; }
        public decimal InsuranceValue { get; set; }
        public decimal InsuranceValueAmendMendt { get; set; }
        public decimal CommissionValue { get; set; }
        public decimal CommissionValueAmendMendt { get; set; }
        public int CurrenceyId { get; set; }
        public string CurrenceyName { get; set; }
        public decimal CurrenceyRate { get; set; }
        public string CurrencySign { get; set; }
        public string CurrencySignBDT { get; set; } = "৳";
        public string PaymentTerms { get; set; }
        public string ShippingMark { get; set; }
        public int VesselId { get; set; }
        public bool IsActive { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public VesselTypeEnum Vessal { get; set; }
        public LcTypeEnum Lc { get; set; }
        public List<SelectModelType> GetAllBanks { get; set; }
        public List<SelectModelType> AdvanceAndLoan { get; set; }
        public List<SelectModelType> GetAllVendorList { get; set; }
        public List<SelectModelType> GetAllContry { get; set; }
        public List<SelectModelType> GetAllCurrencyList { get; set; }
        public SelectList PortOfLoadingList { get; set; } = new SelectList(new List<object>());
        public SelectList PortOfDischargeList { get; set; } = new SelectList(new List<object>());
        public String CompanyBankName { get; set; }
        public String SupllierBankName { get; set; }
        public String CountryName { get; set; }
        public List<LcInfoViewModel> DataList { get; set; }
        public List<LcInfoViewModel> listdata { get; set; }
        public List<VMPurchaseOrderSlave> PurchaseorderList { get; set; }
        public List<VMLCAmendment> AmendmentList { get; set; }
        public List<LcValueAmmendmentvm> LcValueAmendmentList { get; set; }
        public Nullable<decimal> FinancialCharge { get; set; }
        public int? AccountingHeadId { get;  set; }
        public int? AccountingBankHeadId { get;  set; }
        public decimal TotalLandedValue { get;  set; }
        //public DateTime? AmendmentDate { get; set; }
        public DateTime AmendmentDate { get; set; }
        public decimal AmendmentValue { get; set; }
        public bool IsIncrase { get; set; }
        public string Remark { get; set; }
        public string RemarkAmendMendt { get; set; }
        public long LCValueAmendmentId { get; set; }
    }
}