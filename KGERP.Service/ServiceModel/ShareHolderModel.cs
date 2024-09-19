using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace KGERP.Service.ServiceModel
{
    public class ShareHolderModel
    {
        public string ButtonName
        {
            get
            {
                return ShareHolderId > 0 ? "Update" : "Save";
            }
        }
        public long ShareHolderId { get; set; }
        [DisplayName("Company")]
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Type")]
        public Nullable<int> ShareHolderTypeId { get; set; }
        public string Name { get; set; }
        [DisplayName("Gender")]
        public Nullable<int> GenderId { get; set; }
        public string NID { get; set; }
        public string BIN { get; set; }
        [DisplayName("Birth Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        [DisplayName("Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> StartDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        [DisplayName("Present Address")]
        public string PresentAddress { get; set; }
        [DisplayName("Permanent Adress")]
        public string PermanentAdress { get; set; }
        [DisplayName("Father Name")]
        public string FatherName { get; set; }
        [DisplayName("Mother Name")]
        public string MotherName { get; set; }
        public string Spouse { get; set; }
        [DisplayName("Home Phone")]
        public string HomePhone { get; set; }
        [DisplayName("Office Phone")]
        public string OfficePhone { get; set; }
        public string Fax { get; set; }
        [DisplayName("Education")]
        public Nullable<int> EducationQualificationId { get; set; }
        [DisplayName("Profession")]
        public Nullable<int> ProfessionId { get; set; }
        public string Organization { get; set; }
        public string Designation { get; set; }
        [DisplayName("No Of Share")]
        public int NoOfShare { get; set; }
        public int Amount { get; set; }
        public string Nominee { get; set; }
        public string TIN { get; set; }
        [DisplayName("Shareholder Picture")]
        public string ShareHolderImage { get; set; }
        [DisplayName("NID Picture")]
        public string NIDImage { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string MemberId { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        //-------------------Extended Properties----------------------
        public int NoOfRecords { get; set; }
        public string StrStartDate { get; set; }
        public string StrDateOfBirth { get; set; }
        public string ShareHolderType { get; set; }
        public string CompanyName { get; set; }
        public HttpPostedFileBase UploadedFile { get; set; }

        public IEnumerable<ShareHolderModel> DataList { get; set; }

    }
}
