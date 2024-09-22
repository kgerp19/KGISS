using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace KGERP.Service.ServiceModel
{
    public class BoardOfDirectorModel
    {
        public string ButtonName
        {
            get
            {
                return BoardOfDirectorId > 0 ? "Update" : "Save";
            }

        }
        public int BoardOfDirectorId { get; set; }
        [DisplayName("Company")]
        public int CompanyId { get; set; }
        [DisplayName("Member Name")]
        public string MemberName { get; set; }
        [DisplayName("Member Priority")]
        public int MemberOrder { get; set; }
        [Required]
        [DisplayName("Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [DisplayName("End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Remarks { get; set; }
        [DisplayName("Picture")]
        public string MemberImage { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public string NID { get; set; }
        [DisplayName("Birth Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string Phone { get; set; }
        [DisplayName("Home Phone")]
        public string HomePhone { get; set; }
        [DisplayName("Office Phone")]
        public string OfficePhone { get; set; }
        public string Email { get; set; }
        [DisplayName("Present Address")]
        public string PresentAddress { get; set; }
        [DisplayName("Permanent Address")]
        public string PermanentAdress { get; set; }
        [DisplayName("Father Name")]
        public string FatherName { get; set; }
        [DisplayName("Mother Name")]
        public string MotherName { get; set; }
        public string Spouse { get; set; }
        [DisplayName("Education Qualification")]
        public Nullable<int> EducationQualificationId { get; set; }
        [DisplayName("Profession")]
        public Nullable<int> ProfessionId { get; set; }
        public string Organization { get; set; }
        public string Designation { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        //------------------Extended Properties-------------------
        public string CompanyName { get; set; }
        public HttpPostedFileBase UploadedFile { get; set; }
        public HttpPostedFileBase MemberImageUpload { get; set; }

        public IEnumerable<BoardOfDirectorModel> DataList { get; set; }

    }
}
