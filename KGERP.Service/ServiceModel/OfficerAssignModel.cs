using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class OfficerAssignModel
    {
        public int OfficerAssignId { get; set; }
        public int CompanyId { get; set; }
        [Required]
        [DisplayName("Zone")]
        public int ZoneId { get; set; }
        [DisplayName("Marketing Officer")]
        public long EmpId { get; set; }
        [Required]
        [DisplayName("Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [DisplayName("End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        public virtual EmployeeModel Employee { get; set; }
        public virtual ZoneModel Zone { get; set; }
        public int subZoneId { get; set; }
        public string ZoneName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string SubzoneName { get; set; }
        //-----------------Extended Properties-----------
        [Required]
        public string OfficerName { get; set; }
        public IEnumerable<OfficerAssignModel> DataList { get; set; }
        public List<SelectModel> ZoneList { get; set; }
        public List<SelectMarketingModel> ZoneList2 { get; set; }
        public List<SelectMarketingModel> SubZoneList2 { get; set; }
        public List<SelectModel> SubZoneList { get; set; }
        public Data.CustomModel.Officervwmodel officervm { get; set; }

    }

   
}
