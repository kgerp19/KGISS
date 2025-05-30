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
    
    public partial class AttendenceApproveApplication
    {
        public int Id { get; set; }
        public Nullable<long> EmployeeId { get; set; }
        public Nullable<long> ManagerId { get; set; }
        public Nullable<int> ManagerStatus { get; set; }
        public Nullable<long> HrId { get; set; }
        public Nullable<int> HrStatus { get; set; }
        public string Resion { get; set; }
        public string ApproveFor { get; set; }
        public Nullable<System.DateTime> AttendenceDate { get; set; }
        public Nullable<System.DateTime> ApplicationDate { get; set; }
        public Nullable<System.TimeSpan> InTime { get; set; }
        public Nullable<System.TimeSpan> OutTime { get; set; }
        public Nullable<System.TimeSpan> ModifiedInTime { get; set; }
        public Nullable<System.TimeSpan> ModifiedOutTime { get; set; }
        public string HrNote { get; set; }
        public string ManagerNote { get; set; }
        public Nullable<System.DateTime> FromDateForOnField { get; set; }
        public Nullable<System.DateTime> ToDateForOnField { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> ManagerApprovalDate { get; set; }
        public Nullable<System.DateTime> HRApprovalDate { get; set; }
        public Nullable<int> TourDays { get; set; }
    }
}
