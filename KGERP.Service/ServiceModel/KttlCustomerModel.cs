using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace KGERP.Service.ServiceModel
{
    public class KttlCustomerModel
    {
        public string ButtonName
        {
            get
            {
                return Id > 0 ? "Update" : "Save";
            }
        }

        #region Utility
        public long Id { get; set; }
        [DisplayName("Client Id")]
        public int ClientId { get; set; }
        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Next Meeting Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? NextScheduleDate { get; set; }

        [DisplayName("Purpose of Meeting")]
        public string PurposeOfMeeting { get; set; }

        [DisplayName("Last Meeting Date1")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? LastMeetingDate1 { get; set; }
        [DisplayName("Last Meeting Date2")]

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? LastMeetingDate2 { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        #endregion

        #region Customer Details & Requirement
        [DisplayName("Title")]
        public string CTitle { get; set; }
        [DisplayName("Name")]
        public string FullName { get; set; }
        [Required]
        [DisplayName("Sur Name")]
        public string SurName { get; set; }
        [DisplayName("Given Name")]
        [Required]
        public string GivenName { get; set; }

        [DisplayName("Passport No")]
        public string PassportNo { get; set; } 

        [DisplayName("Service Year")]
        public Nullable<int> ServiceYear { get; set; }

        [DisplayName("Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateOfBirth { get; set; }
        [DisplayName("Date of Issue")]

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateOfIssue { get; set; }

        [DisplayName("Date of Expire")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateOfExpire { get; set; }
        public string Nationality { get; set; }
        [DisplayName("Client Age")]
        public string ClientAge { get; set; }
        public string Spouse { get; set; }

        [DisplayName("Phone(Home)")]
        public string PhoneHome { get; set; }
        [DisplayName("Phone(Office)")]
        public string PhoneOffice { get; set; }

        [DisplayName("Cell No(WIV)")]
        public string MobileNo2 { get; set; }
        [Required(ErrorMessage = "Required")]
        [DisplayName("Cell No")]
        [StringLength(15, MinimumLength = 11)]
        public string MobileNo { get; set; }
        [DisplayName("Cell No")]
        public string CellNo { get; set; } 
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Email 2")]
        public string Email1 { get; set; }

        [DisplayName("No of Child")]
        public string NoOfChild { get; set; }
        public string Religion { get; set; }
        [DisplayName("Blood Group")]
        public string BloodGroup { get; set; }
        [DisplayName("Blood Group")]
        public Nullable<int> BloodGroupId { get; set; }

        [DisplayName("Facebook Id")]
        public string SocialId { get; set; }

        public List<int> ServiceYears { get; set; }
        [DisplayName("Profession")]
        public int ProfessionId { get; set; }

        [DisplayName("Place of Birth")]
        public int PlacesOfBirthId { get; set; }

        public string ImageUrl { get; set; }
        [DisplayName("Client Status")]
        public string ClientStatus { get; set; }
        public string Organization { get; set; }
        [DisplayName("Type of Service")]
        public string Services { get; set; }

        [DisplayName("Services Notes")]
        public string ServicesDescription { get; set; }

        [DisplayName("Contact Officer")]
        public string ResponsiblePerson { get; set; }

        [DisplayName("NID No")] 
        public string NationalID { get; set; }

        [DisplayName("Birth Reg.No")] 
        public string BirthID { get; set; }

        public string Remarks { get; set; }
        #endregion

        #region Customer Locations
        [DisplayName("Present Address")]
        public string PresentAddress { get; set; }
        [DisplayName("Permanent Address")]
        public string PermanentAddress { get; set; }
        [DisplayName("Office Address")]
        public string OfficeAddress { get; set; }

        public int DivisionId { get; set; }
        public int DistrictId { get; set; }
        public int ThanaId { get; set; }
        public int Division { get; set; }
        public int District { get; set; }
        [DisplayName("Police Station")]
        public int ThanaUpazila { get; set; }
        [DisplayName("House/Village")]
        public string VillageMohalla { get; set; }

        [DisplayName("Division")]
        public int Division1 { get; set; }
        [DisplayName("District")]
        public int District1 { get; set; }
        [DisplayName("Police Station")]
        public int ThanaUpazila1 { get; set; }
        [DisplayName("House/Village")]
        public string VillageMohalla1 { get; set; }

        [DisplayName("Post Office")]
        public string PostOffice { get; set; }         
        public string Sector { get; set; }
        public string Block { get; set; }
        [DisplayName("Passport Validity")]
        public int PassportValidityId { get; set; }

        #endregion

        #region Family & Emergency Contact
        public string Gender { get; set; }
        [DisplayName("Father's Name")]
        public string FatherName { get; set; }
        [DisplayName("Mother's Name")]
        public string MotherName { get; set; }
        [DisplayName("Marital Status")]
        public string MaritalStatus { get; set; }
        [DisplayName("Name")]
        public string ContactName { get; set; }
        [DisplayName("Address")]
        public string ContactAddress { get; set; }
        [DisplayName("Cell No")]
        public string ContactCellNo { get; set; }
        [DisplayName("Email")]
        public string ContactEmail { get; set; }
        [DisplayName("Relation")]
        public string ContactRelation { get; set; }

        [DisplayName("Contact Notes")]
        public string ContactNotes { get; set; }
        #endregion

        #region Customer Requirement    OR Service Details  

        [DisplayName("Type of Client")]
        public int TypeOfClientId { get; set; }
        [DisplayName("Source of Media")]
        public string SourceOfMedia { get; set; }
        [DisplayName("Promotional Offer")]
        public string PromotionalOffer { get; set; }
        #endregion 

        #region All DDL

        public List<SelectModel> Divisions { get; set; }
        public List<SelectModel> Districts { get; set; }
        public List<SelectModel> Upazilas { get; set; }
        public List<SelectModel> PlacesOfBirth { get; set; }
        public List<SelectModel> Divisions2 { get; set; }
        public List<SelectModel> Districts2 { get; set; }
        public List<SelectModel> Upazilas2 { get; set; }
        public List<SelectModel> Banks { get; set; }
        public List<SelectModel> Religions { get; set; }
        public List<SelectModel> BloodGroups { get; set; }
        public List<SelectModelType> Countries { get; set; }
        public List<SelectModel> MaritalTypes { get; set; }
        public List<SelectModel> Genders { get; set; }
        public List<SelectModel> Titles { get; set; }
        public List<SelectModel> ClientStatuss { get; set; }
        public List<SelectModel> Servicess { get; set; }
        public List<SelectModel> SourceOfMedias { get; set; }
        public List<SelectModel> ResponsiblePersons { get; set; }
        public List<SelectModel> Professions { get; set; }
        public List<SelectModel> PassportValidities { get; set; }
        public List<SelectModel> TypeOfClients { get; set; }
        #endregion


        [DisplayName("Upload File")]
        public string FilePath { get; set; }
        public HttpPostedFileBase ExcelFile { get; set; }
    }
}
