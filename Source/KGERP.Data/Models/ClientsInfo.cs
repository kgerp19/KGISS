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
    
    public partial class ClientsInfo
    {
        public long Cli_id { get; set; }
        public string ClientsAutoId { get; set; }
        public string Cli_BlockNo { get; set; }
        public string Cli_PlotNo { get; set; }
        public string Cli_PlotSize { get; set; }
        public string Cli_Facing { get; set; }
        public Nullable<System.DateTime> Cli_Date { get; set; }
        public Nullable<int> Cli_ProjectName { get; set; }
        public Nullable<double> LandPricePerKatha { get; set; }
        public Nullable<double> PlotSize { get; set; }
        public Nullable<double> LandValue { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<double> LandValueAfterDiscount { get; set; }
        public Nullable<double> AdditionalCost { get; set; }
        public Nullable<double> UtilityCost { get; set; }
        public Nullable<double> GrandTotal { get; set; }
        public Nullable<double> BokkingMoney { get; set; }
        public Nullable<double> RestOfAmount { get; set; }
        public Nullable<int> OneTime { get; set; }
        public Nullable<int> InstallMent { get; set; }
        public Nullable<int> NoOfInstallment { get; set; }
        public Nullable<double> InstallMentAmount { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> Booking_Date { get; set; }
        public string ClientName { get; set; }
        public string PresentAddress { get; set; }
    }
}