using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace KGERP.Service.ServiceModel
{
    public class LandNLegalModel
    {
        public string ButtonName
        {
            get
            {
                return OID > 0 ? "Modify Case" : "Save";
            }
        }

        public long OID { get; set; }
        [Required]
        [DisplayName("Case No")]
        public string CaseNo { get; set; }
        [DisplayName("Filing Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]

        public Nullable<System.DateTime> FilingDate { get; set; }
        [DisplayName("Case Name")]
        public string CaseName { get; set; }
        [DisplayName("Case Summary")]
        public string CaseSummary { get; set; }
        [Required]
        [DisplayName("Court Name")]
        public string CourtName { get; set; }
        [DisplayName("Court No")]
        public string AdditionalDistrictCourt { get; set; }
        [Required]
        [DisplayName("Plaintiff/ Appellant")]
        public string PlaintiffAppellant { get; set; }
        [Required]
        [DisplayName("Defendant Name")]
        public string DefendantName { get; set; }

        [DisplayName("Defendant's Father")]
        public string DefendantFatherName { get; set; }

        [DisplayName("Defendant's Mother")]
        public string DefendantMotherName { get; set; }

        [DisplayName("Mobile No")]
        public string MobileNo { get; set; }
        [DisplayName("Business Address")]
        public string BusinessAddress { get; set; }
        [DisplayName("Organization Name")]
        public string BusinessOrganizationName { get; set; }
        public string DivisionId { get; set; }
        public string DistrictId { get; set; }
        public string ThanaId { get; set; }
        public string Division { get; set; }
        public string District { get; set; }
        [DisplayName("Thana /Upazila")]
        public string ThanaUpazila { get; set; }
        [DisplayName("Village /Road")]
        public string VillageMohalla { get; set; }
        [DisplayName("Trade Licence No")]
        public string TradeLicenceNo { get; set; }
        [DisplayName("NID/Passport No")]
        public string NIDnPassportNo { get; set; }

        #region BankInformation

        [DisplayName("Bank Information")]
        public string BankInformation { get; set; }


        public Nullable<int> BankId { get; set; }
        [DisplayName("Bank Name")]
        public string BankName { get; set; }
        [DisplayName("Branch Name")]
        public string BranchName { get; set; }

        [DisplayName("Account Name")]
        public string AccountName { get; set; }
        [DisplayName("Account No")]
        public string AccountNo { get; set; }
        [DisplayName("Cheque Number")]
        public string ChequeNumber { get; set; }
        [DisplayName("Cheque Issue Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ChequeIssueDate { get; set; }

        #endregion

        [DisplayName("Case Type")]
        [Required]
        public string CaseType { get; set; }

        [DisplayName("Case Status")]
        public string CaseStatus { get; set; }
        [DisplayName("Previous Date1")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> PreviousDate1 { get; set; }

        [DisplayName("Previous Date2")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> PreviousDate2 { get; set; }

        [DisplayName("Next Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> NextDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,0.00}")]
        public Nullable<decimal> Amount { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        [DisplayName("Company Name")]
        [Required]
        public string CompanyName { get; set; }
        [Required]
        [DisplayName("Responsible Lawyer/Person")]
        public string ResponsibleLayer { get; set; }
        public virtual CompanyModel Company { get; set; }
        public List<SelectModel> Companies { get; set; }
        public List<SelectModel> CaseTypes { get; set; }
        public List<SelectModel> ResponsiblePersons { get; set; }
        [DisplayName("Case Comments")]

        public string CaseComments { get; set; }
        [DisplayName("Case History")]
        public string Case_History { get; set; }
        [DisplayName("Case Attachment")]
        public string Case_Attachment { get; set; }
        public virtual CaseCommentModel CaseComment { get; set; }
        public virtual CaseHistoryModel CaseHistory { get; set; }
        public virtual CaseAttachmentModel CaseAttachment { get; set; }

        [DisplayName("Upload File")]
        public string AttachFilePath { get; set; }
        public HttpPostedFileBase AttachFile { get; set; }

        public virtual ICollection<FileAttachment> FileAttachments { get; set; }

        #region //Advance Search field
        [DisplayName("Company Name")]
        public string Company_Name { get; set; }
        [DisplayName("Responsible Lawyer/Person")]
        public string ResponsibleLawyer { get; set; }

        [DisplayName("Case Type")]
        public string Case_Type { get; set; }
        #endregion
    }
}
