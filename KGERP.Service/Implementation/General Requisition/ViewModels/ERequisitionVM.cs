using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KGERP.Service.Implementation.General_Requisition.ViewModels
{
    public class ERequisitionVM
    {
        public long Id { get; set; }

        public string ActionButton { get; set; }
        public string  EmployeeIdList { get; set; }

       // public int[] EmployeeIdList { get; set; }
        public string Remarks { get; set; }
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequisitionDate { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyName { get; set; }
        public long EmployeeId { get; set; }
        public GeneralRequisitionStatusEnum ERequisitionStatus { get; set; }
        public EFileSignatoryStatusEnum SignatoryStatus { get; set; }
        public StatusEnum Status { get; set; }
        public int UserCompanyId { get; set; }
        public string RequisitionNumber { get; set; }

        public int SignatoryId { get; set; }
        public List<ERequisitionDetailVM> DetailsList { get; set; }
        public List<ERequisitionSignatoryVM> SignatoryList { get; set; }

        public int ERequisitionDetailId { get; set; }
        public string Description { get; set; }
        public HttpPostedFileBase Attachment { get; set; }
        public List<AttachmentVM> Attachments { get; set; }
        public string AttachmentString { get; set; }
        public long AttachmentId { get; set; }
        public List<HttpPostedFileBase> FileAttachments { get; set; }
        public bool IsApprovedForwardStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string Comment { get; set; }
        public int? LastSignatoryStatusInt { get; set; }
        public EFileSignatoryStatusEnum LastSignatoryStatus 
        {
            get
            {
                if (LastSignatoryStatusInt != null && LastSignatoryStatusInt > 0)
                {
                    return (EFileSignatoryStatusEnum)LastSignatoryStatusInt.Value;
                }
                return EFileSignatoryStatusEnum.Pending;
            }
        }
    }

    public class ERequisitionDetailVM
    {
        public long Id { get; set; }
        public int RequisitionId { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string Attachment { get; set; }
    }

    //only used for signatory load
    public class EDocumentApprovalVM
    {
        public int RequisitionApprovalId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeIdString { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public EFileSignatoryStatusEnum Status { get; set; }
        public string StatusString { get; set; }
        public string ApprovedTime { get; set; }
        public int OrderBy { get; set; }
        public string Comment { get; set; }
    }
    public class ERequisitionSignatoryVM
    {
        public long Id { get; set; }
        public long EmployeeIdInt { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public int OrderBy { get; set; }
        public string Comment { get; set; }
        public EFileSignatoryStatusEnum SignatoryStatus { get; set; }
        public DateTime ApprovedTime { get; set; }
        public string ApprovedTimeString 
        {
            get
            {
                if (ApprovedTime > DateTime.MinValue)
                {
                    return ApprovedTime.ToString("dd-MM-yyyy hh:mm:tt");
                }
                return "";
            }
        }
    }


    public class AttachmentVM
    {
        public string Title { get; set; }
        public long DocId { get; set; } = 0;
        public HttpPostedFileBase File { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
        public string FileNameWithExtension { get; set; }
    }


    public class EFileApprovalVM
    {
        //int companyId, HttpPostedFileBase Attachment, int requisitionSignatoryId, SignatoryStatusEnum status, string comment, int? forwardTo
        public int companyId { get; set; }
        public HttpPostedFileBase Attachment { get; set; }
        public int requisitionSignatoryId { get; set; }
        public EFileSignatoryStatusEnum status { get; set; }
        public string comment { get; set; }
        public int? forwardTo { get; set; }
        public int SignatoryAction { get; set; }

        public EFileSignatoryStatusEnum SignatoryStatusEnum { get; set; }
    }
}
