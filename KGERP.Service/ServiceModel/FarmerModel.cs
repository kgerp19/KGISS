using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class FarmerModel
    {
        public long FarmerId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Zone")]
        public Nullable<int> ZoneId { get; set; }
        [Required(ErrorMessage = "Farmer's Name is Required")]
        public string Name { get; set; }
        [DisplayName("Birth Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        [DisplayName("National ID")]
        public string NationalId { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public string Spouse { get; set; }
        public string Remarks { get; set; }
        [DisplayName("Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> StartDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        [DisplayName("Responsible Officer")]
        public long? OfficerId { get; set; }
        //-----------------Extended Properties---------------
        public string StrStartDate { get; set; }
        public string OfficerName { get; set; }
        public string Status { get; set; }
    }
}
