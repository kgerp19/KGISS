using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class EmployeeLoanModel
    {
        public string ButtonName
        {
            get
            {
                return LoanID > 0 ? "Update" : "Save";
            }

        }
        public int LoanID { get; set; }
        public string EmployeeId { get; set; }
        [DisplayName("Loan Purpose")]
        public string LoanPurpose { get; set; }
        public Nullable<int> Amount { get; set; }
        [DisplayName("Loan Type")]
        public Nullable<int> LoanType { get; set; }

        [DisplayName("Loan Type")]
        public string LoanTypeName { get; set; }
        [DisplayName("No of Installment")]
        public Nullable<int> NoOfInstallment { get; set; }

        [DisplayName("Loan End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> LoanEndDate { get; set; }

        [DisplayName("Loan Apply Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> LoanApplyDate { get; set; }

        [DisplayName("Installment Amount")]
        public Nullable<int> InstallmentAmount { get; set; }

        [DisplayName("Install Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> InstallStartDate { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public List<SelectModel> LoanTypes { get; set; }

    }
}
