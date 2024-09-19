using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.ComparativeStatementService.Comparative_Statement
{
    public class ComparativeStatementVm
    {
        public long CSID { get; set; }
        public string CSNO { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime CSDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DeliveryDate { get; set; }

        public Nullable<int> ProductID { get; set; }
        public string Origin { get; set; }
        public string Brand { get; set; }
        public string Specification { get; set; }
        public Nullable<decimal> RequiredQuantity { get; set; }
        public bool Status { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public List<ComparativeStatementDetailVm> ComparativeStatementDetailList { get; set; }
        public ComparativeStatementVm DetaliesObject { get; set; }
        public ComparativeStatementDetailVm DetaliesObjectVm { get; set; }
        public IEnumerable<ComparativeStatementDetailVm> ComparativeStatementDetailvmList { get; set; }
        public IEnumerable<RequisitionApprovalVM> RADataList { get; set; }
        public IEnumerable<ComparativeStatementVm> ComparetiveStatemnetList { get; set; }
        public List<SelectModelPr> ProductList { get; set; }
        public int CompanyId { get; set; }
        public string PrductName { get; set; }



    }
}
