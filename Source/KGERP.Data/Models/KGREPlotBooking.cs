//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KGERP.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class KGREPlotBooking
    {
        public long BookingId { get; set; }
        public Nullable<int> ClientId { get; set; }
        public Nullable<int> PlotId { get; set; }
        public string FileNo { get; set; }
        public Nullable<double> LandPricePerKatha { get; set; }
        public Nullable<double> LandValue { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<double> LandValueAfterDiscount { get; set; }
        public Nullable<double> AdditionalCost { get; set; }
        public Nullable<double> Additional10Percent { get; set; }
        public Nullable<double> Additional15Percent { get; set; }
        public Nullable<double> Additional25Percent { get; set; }
        public string OtharCostName { get; set; }
        public Nullable<double> UtilityCost { get; set; }
        public Nullable<double> GrandTotal { get; set; }
        public Nullable<double> BookingMoney { get; set; }
        public Nullable<double> RestOfAmount { get; set; }
        public Nullable<double> InstallmentAmount { get; set; }
        public Nullable<int> SalesTypeId { get; set; }
        public Nullable<System.DateTime> InstallmentDate { get; set; }
        public Nullable<int> NoOfInstallment { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> BookingDate { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public string PayType { get; set; }
        public string BankName { get; set; }
        public string ChaqueNo { get; set; }
        public string BookingNote { get; set; }
        public string SalseOfficerId { get; set; }
        public Nullable<double> SalseCommision { get; set; }
        public Nullable<double> RegAmount { get; set; }
        public Nullable<double> MutationCost { get; set; }
        public Nullable<double> BoundaryWall { get; set; }
        public Nullable<double> NamePlate { get; set; }
        public Nullable<double> TreePlantation { get; set; }
        public Nullable<double> NameChange { get; set; }
        public Nullable<double> SecurityService { get; set; }
        public Nullable<double> BSSurvey { get; set; }
        public Nullable<double> Penalty { get; set; }
        public Nullable<double> LandValueR { get; set; }
        public Nullable<double> DiscountR { get; set; }
        public Nullable<double> LandValueAfterDiscountR { get; set; }
        public Nullable<double> AdditionalCostR { get; set; }
        public Nullable<double> Additional10PercentR { get; set; }
        public Nullable<double> Additional15PercentR { get; set; }
        public Nullable<double> Additional25PercentR { get; set; }
        public string OtharCostNameR { get; set; }
        public Nullable<double> UtilityCostR { get; set; }
        public Nullable<double> GrandTotalR { get; set; }
        public Nullable<double> BookingMoneyR { get; set; }
        public Nullable<double> RestOfAmountR { get; set; }
        public Nullable<double> SalseCommisionR { get; set; }
        public Nullable<double> MutationCostR { get; set; }
        public Nullable<double> BoundaryWallR { get; set; }
        public Nullable<double> NamePlateR { get; set; }
        public Nullable<double> TreePlantationR { get; set; }
        public Nullable<double> NameChangeR { get; set; }
        public Nullable<double> SecurityServiceR { get; set; }
        public Nullable<double> BSSurveyR { get; set; }
        public Nullable<double> RegAmountR { get; set; }
        public Nullable<double> AddDelayFineR { get; set; }
        public Nullable<double> AddDelayFine { get; set; }
        public Nullable<double> ReturnMoney { get; set; }
        public Nullable<double> PenaltyR { get; set; }
        public Nullable<double> ServiceCharge4or10Per { get; set; }
        public Nullable<double> ServiceCharge4or10PerR { get; set; }
        public Nullable<double> NetReceivedR { get; set; }
        public Nullable<double> Due { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}