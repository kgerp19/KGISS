using KGERP.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class KGREPlotBookingModel
    {
        public string ButtonName
        {
            get
            {
                return BookingId > 0 ? "Modify" : "Save";
            }
        }
        public int ProjectId { get; set; }
        [DisplayName("Company")]
        public Nullable<int> CompanyId { get; set; }

        [DisplayName("File No")]
        public string FileNo { get; set; }
        [DisplayName("Installment Status")]
        public string InstallmentStatus { get; set; }
        public long BookingId { get; set; }
        public Nullable<int> ClientId { get; set; }
        [DisplayName("Plot No")]
        public Nullable<int> PlotId { get; set; }

        [DisplayName("Per Katha")]
        public Nullable<double> LandPricePerKatha { get; set; }
        [DisplayName("Land Value")]
        public Nullable<double> LandValue { get; set; }
        [DisplayName("Discount Amount")]
        public Nullable<double> DiscountAmount { get; set; }
        public Nullable<double> Discount { get; set; }
        [DisplayName("Additional 25%")]
        public Nullable<double> Additional25Percent { get; set; }
        [DisplayName("Additional 15%")]
        public Nullable<double> Additional15Percent { get; set; }
        [DisplayName("Additional 10%")]
        public Nullable<double> Additional10Percent { get; set; }
        [DisplayName("After Discount")]
        public Nullable<double> LandValueAfterDiscount { get; set; }
        [DisplayName("Additional Cost")]
        public Nullable<double> AdditionalCost { get; set; }
        [DisplayName("Other Cost")]
        public Nullable<double> OtherCostName { get; set; }
        [DisplayName("Utility Cost")]
        public Nullable<double> UtilityCost { get; set; }

        [DisplayName("Grand Total")]
        public Nullable<double> GrandTotal { get; set; }
        [DisplayName("Booking Money")]
        public Nullable<double> BookingMoney { get; set; }
        [DisplayName("Rest Of Amount")]
        public Nullable<double> RestOfAmount { get; set; }
        [DisplayName("Installment Amount")]
        public Nullable<double> InstallmentAmount { get; set; }

        [DisplayName("No of Installment")]
        public Nullable<int> NoOfInstallment { get; set; }


        [DisplayName("Booking Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> BookingDate { get; set; }
        [DisplayName("Pay Type")]
        public string PayType { get; set; }
        [DisplayName("Bank Name")]
        public string BankName { get; set; }
        [DisplayName("Chaque No")]
        public string ChaqueNo { get; set; }

        [DisplayName("Sales Type")]
        public Nullable<int> SalesTypeId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> InstallmentDate { get; set; }
        [DisplayName("Booking Note")]
        //public string Remarks { get; set; }
        public string BookingNote { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> RegistrationDate { get; set; }

        public string SalseOfficerId { get; set; }
        [DisplayName("Salse Commision")]
        public Nullable<double> SalseCommision { get; set; }
        [DisplayName("Mutation Cost")]
        public Nullable<double> MutationCost { get; set; }
        [DisplayName("Boundary Wall")]
        public Nullable<double> BoundaryWall { get; set; }
        [DisplayName("Name Plate")]
        public Nullable<double> NamePlate { get; set; }
        [DisplayName("Tree Plantation")]
        public Nullable<double> TreePlantation { get; set; }
        [DisplayName("Name Change")]
        public Nullable<double> NameChange { get; set; }
        [DisplayName("Security")]
        public Nullable<double> SecurityService { get; set; }
        [DisplayName("BS Survey")]
        public Nullable<double> BSSurvey { get; set; }
        public Nullable<double> Penalty { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }


        public string Religion { get; set; }
        public string PlotNo { get; set; }
        public string PlotSize { get; set; }
        public string PlotFace { get; set; }

        public string Designation { get; set; }
        public string FullName { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string MobileNo { get; set; }

        [DisplayName("Birth Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DateofBirth { get; set; }

        public string Email { get; set; }
        public string DepartmentOrInstitution { get; set; }
        public string ResponsibleOfficer { get; set; }
        public Nullable<double> RegAmount { get; set; } 

        [DisplayName("Land ValueR")]
        public Nullable<double> LandValueR { get; set; }
        public Nullable<double> DiscountR { get; set; }
        public Nullable<double> LandValueAfterDiscountR { get; set; }
        [DisplayName("Additional CostR")]
        public Nullable<double> AdditionalCostR { get; set; }
        [DisplayName("Additional10 PercentR")]
        public Nullable<double> Additional10PercentR { get; set; }
        [DisplayName("Additional15 PercentR")]
        public Nullable<double> Additional15PercentR { get; set; }
        [DisplayName("Additional25 PercentR")]
        public Nullable<double> Additional25PercentR { get; set; }
        [DisplayName("Other Cost NameR")]
        public string OtharCostNameR { get; set; }
        [DisplayName("Utility CostR")]
        public Nullable<double> UtilityCostR { get; set; }
        [DisplayName("Grand TotalR")]
        public Nullable<double> GrandTotalR { get; set; }
        [DisplayName("Booking MoneyR")]
        public Nullable<double> BookingMoneyR { get; set; }
        [DisplayName("Rest Of AmountR")]
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
        [DisplayName("Return Money")]
        public Nullable<double> ReturnMoney { get; set; }
        public Nullable<double> PenaltyR { get; set; }
        public Nullable<double> ServiceCharge4or10Per { get; set; }
        public Nullable<double> ServiceCharge4or10PerR { get; set; }
        public Nullable<double> NetReceivedR { get; set; }
        public Nullable<double> Due { get; set; }

        [DisplayName("Project Name")]
        public string ProjectName { get; set; }
        [DisplayName("Plot Status")]
        public string PlotStatus { get; set; }

        public virtual ICollection<FileAttachment> FileAttachments { get; set; }





        //public long BookingId { get; set; }
        //public Nullable<int> ClientId { get; set; }
        //public Nullable<int> PlotId { get; set; }
        //public string FileNo { get; set; }
        //public Nullable<double> LandPricePerKatha { get; set; }
        //public Nullable<double> LandValue { get; set; }
        //public Nullable<double> Discount { get; set; }
        //public Nullable<double> LandValueAfterDiscount { get; set; }
        //public Nullable<double> AdditionalCost { get; set; }
        //public Nullable<double> Additional10Percent { get; set; }
        //public Nullable<double> Additional15Percent { get; set; }
        //public Nullable<double> Additional25Percent { get; set; }
        //public string OtharCostName { get; set; }
        //public Nullable<double> UtilityCost { get; set; }
        //public Nullable<double> GrandTotal { get; set; }
        //public Nullable<double> BookingMoney { get; set; }
        //public Nullable<double> RestOfAmount { get; set; }
        //public Nullable<double> InstallmentAmount { get; set; }
        //public Nullable<int> SalesTypeId { get; set; }
        //public Nullable<System.DateTime> InstallmentDate { get; set; }
        //public Nullable<int> NoOfInstallment { get; set; }
        //public string Remarks { get; set; }
        //public Nullable<System.DateTime> BookingDate { get; set; }
        //public Nullable<System.DateTime> RegistrationDate { get; set; }
        //public string PayType { get; set; }
        //public string BankName { get; set; }
        //public string ChaqueNo { get; set; }
        //public string BookingNote { get; set; }
        //public string SalseOfficerId { get; set; }
        //public Nullable<double> SalseCommision { get; set; }
        //public Nullable<double> RegAmount { get; set; }
        //public Nullable<double> MutationCost { get; set; }
        //public Nullable<double> BoundaryWall { get; set; }
        //public Nullable<double> NamePlate { get; set; }
        //public Nullable<double> TreePlantation { get; set; }
        //public Nullable<double> NameChange { get; set; }
        //public Nullable<double> SecurityService { get; set; }
        //public Nullable<double> BSSurvey { get; set; }
        //public Nullable<double> Penalty { get; set; }
        //public Nullable<double> LandValueR { get; set; }
        //public Nullable<double> DiscountR { get; set; }
        //public Nullable<double> LandValueAfterDiscountR { get; set; }
        //public Nullable<double> AdditionalCostR { get; set; }
        //public Nullable<double> Additional10PercentR { get; set; }
        //public Nullable<double> Additional15PercentR { get; set; }
        //public Nullable<double> Additional25PercentR { get; set; }
        //public string OtharCostNameR { get; set; }
        //public Nullable<double> UtilityCostR { get; set; }
        //public Nullable<double> GrandTotalR { get; set; }
        //public Nullable<double> BookingMoneyR { get; set; }
        //public Nullable<double> RestOfAmountR { get; set; }
        //public Nullable<double> SalseCommisionR { get; set; }
        //public Nullable<double> MutationCostR { get; set; }
        //public Nullable<double> BoundaryWallR { get; set; }
        //public Nullable<double> NamePlateR { get; set; }
        //public Nullable<double> TreePlantationR { get; set; }
        //public Nullable<double> NameChangeR { get; set; }
        //public Nullable<double> SecurityServiceR { get; set; }
        //public Nullable<double> BSSurveyR { get; set; }
        //public Nullable<double> RegAmountR { get; set; }
        //public Nullable<double> AddDelayFineR { get; set; }
        //public Nullable<double> AddDelayFine { get; set; }
        //public Nullable<double> ReturnMoney { get; set; }
        //public Nullable<double> PenaltyR { get; set; }
        //public Nullable<double> ServiceCharge4or10Per { get; set; }
        //public Nullable<double> ServiceCharge4or10PerR { get; set; }
        //public Nullable<double> NetReceivedR { get; set; }
        //public Nullable<double> Due { get; set; }
        //public string CreatedBy { get; set; }
        //public Nullable<System.DateTime> CreatedDate { get; set; }
        //public string ModifiedBy { get; set; }
        //public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
