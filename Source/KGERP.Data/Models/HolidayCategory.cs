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
    
    public partial class HolidayCategory
    {
        public int holiday_no { get; set; }
        public System.DateTime entry_date { get; set; }
        public string holiday_type { get; set; }
        public string holiday_name { get; set; }
        public int holiday_day { get; set; }
        public int holiday_month { get; set; }
        public string holiday_date { get; set; }
        public System.DateTime save_time { get; set; }
    }
}