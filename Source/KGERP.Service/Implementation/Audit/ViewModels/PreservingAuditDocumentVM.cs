using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Audit.ViewModels
{
    public class PreservingAuditDocumentVM
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "The Company Name field is required.")]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string  MonthName { get; set; }
        [DisplayName("Document Type")]
        public PreservingAuditDocumentTypeEnum Type { get; set; }
        [DisplayName("Procedure Applied")]
        public string ProcedureeApplied { get; set; }
        public string Observation { get; set; }
        [DisplayName("Audit Recommendation Primary")]
        public string AuditRecommendationPrimary { get; set; }
        [DisplayName("Concern Feedback")]
        public string ConcernedFeedback { get; set; }
        [DisplayName("Audit Recommendation Final")]
        public string AuditRecommendationFinal { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public List<GLDLBookingAttachment> Attachments { get; set; }
        public List<PreservingAuditDocumentVM> DataList { get; set; }
        public long ImageDocId { get; set; }
        public long CGId { get; set; }
        public int UserCompanyId { get; set; }
        public ActionEnum ActionEnum { get; set; }
        public string DocumentNo { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set;}
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public string Title { get; set; }
    }
}
