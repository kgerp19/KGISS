using System;
using System.Collections.Generic;

namespace KGERP.Data.CustomModel
{
    public class Officervwmodel
    {
       
        public string EmployeeName { get; set; }
        public int OfficerAssignId { get; set; }
        public int CompanyId { get; set; }
        public int ZoneId { get; set; }
        public long EmpId { get; set; }
        public int SubZoneId { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }

    
}
}
