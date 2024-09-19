using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.QualityExceptionComplaints
{
    public class QualityExceptionComplaintVM
    {
        public long QualityExceptionComplaintId { get; set; }
        public string NameOfClientOrCompany { get; set; }
        public long ReportingPerson { get; set; }
        public bool IsSubmited { get; set; }
    }

    public class QualityExceptionComplaintDetailVM: QualityExceptionComplaintVM
    {
        public long QualityExceptionComplaintDetailId { get; set; }
        public long OrderDeliverDetailId { get; set; }
        public string DescriptionQualityException { get; set; }
        public string OtherRemarks { get; set; }
        public int? QualityException { get; set; }
        public long? PersonsResponsibleForQualityException { get; set; }
        public string ExceptionExplanation { get; set; }
        public string StepsTakenToPreventRecurrence { get; set; }
        public long? SignatureOfProductionInCharge { get; set; }
        public decimal? DeliveredAffectedQty { get; set; }
        public decimal? DeliveredAffectedWeight { get; set; }
        public double DeliveredQty { get; set; }
        public decimal NetWeight { get; set; }
        public bool IsSubmitedDetails { get; set; }
        public int CompanyId { get; set; }
        [Required]
        public long ResponsiblePersons { get; set; }

        public List<DropDownItemForQualityException> DropDownItemForQualityExceptionList { get; set; }

        [Required]
        public int CustomerId { get; set; }
        [Required]
        public long OrderMasterId { get; set; }
        [Required]
        public long OrderDeliverId { get; set; }
        [Required]
        public long OrderDeliverDetailsId { get; set; }
        public List<SelectListItem> DDLCustomer { get; set; }
        public List<SelectListItem> DDLOrder { get; set; }
        public List<SelectListItem> DDLOrderDelivery { get; set; }
        public List<SelectListItem> DDLOrderDeliveryDetail { get; set; }
        public List<SelectListItem> DDLEmployee { get; set; }
        public string VendorName { get;  set; }
        public string ProductName { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string ReportingPersonName { get; set; }
        public string PersonsResponsible { get; set; }
        public string FormDate { get; set; }
        public string ToDate { get; set; }

        public IEnumerable<QualityExceptionComplaintDetailVM> DataList { get; set; }
    }

    public class QualityExceptionComplaintMapVM
    {
        public long QualityExceptionComplaintMapId { get; set; }
        public long QualityExceptionComplaintDetailId { get; set; }
        public int DesQualityExceptionDropDownItemId { get; set; }
        public bool DescriptionOfQualityExceptionValue { get; set; }
    }

    public class DropDownItemForQualityException:SelectListItem
    {
        public bool QualityExceptionValue { get; set; }
    }

    public class QualityExceptionComplaintRM
    {
        public long QualityExceptionComplaintId { get; set; }
        public long QualityExceptionComplaintDetailId { get; set; }
        public int CompanyId { get; set; }
    }
}
