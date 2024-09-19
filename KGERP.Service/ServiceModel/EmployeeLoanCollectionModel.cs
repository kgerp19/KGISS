using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class EmployeeLoanCollectionModel
    {
        public string ButtonName
        {
            get
            {
                return LoanCollectionId > 0 ? "Update" : "Save";
            }
        }

        public int LoanCollectionId { get; set; }
        public Nullable<int> LoanId { get; set; }
        public string EmployeeId { get; set; }

        [DisplayName("For Month Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ForMonthDate { get; set; }
        public Nullable<double> Amount { get; set; }
        [DisplayName("Loan Type Name")]
        public string LoanTypeName { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public List<SelectModel> LoanTypes { get; set; }

    }
}
