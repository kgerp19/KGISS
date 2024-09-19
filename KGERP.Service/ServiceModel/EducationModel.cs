using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace KGERP.Service.ServiceModel
{
    public class EducationModel
    {
        public string ButtonName
        {
            get
            {
                return EducationId > 0 ? "Update" : "Save";
            }

        }

        public int EducationId { get; set; }
        public Nullable<long> Id { get; set; }
        [DisplayName("Employee")]
        public string EmployeeId { get; set; }
        [Required]
        [DisplayName("Examination")]
        public Nullable<int> ExaminationId { get; set; }
        [DisplayName("Subject / Group")]
        public Nullable<int> SubjectId { get; set; }
        [DisplayName("Institute")]
        public Nullable<int> InstituteId { get; set; }
        [DisplayName("Passing Year")]
        public string PassingYear { get; set; }
        [DisplayName("Roll No")]
        public string RollNo { get; set; }
        [DisplayName("Reg No")]
        public string RegNo { get; set; }
        public string Result { get; set; }
        public string Remarks { get; set; }
        [DisplayName("Certificate")]
        public string CertificateName { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual DropDownItemModel DropDownItem { get; set; }
        public virtual DropDownItemModel DropDownItem1 { get; set; }
        public virtual DropDownItemModel DropDownItem2 { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        //---------------Extended Property---------------------------------
        public HttpPostedFileBase CertificateUpload { get; set; }

    }
}
