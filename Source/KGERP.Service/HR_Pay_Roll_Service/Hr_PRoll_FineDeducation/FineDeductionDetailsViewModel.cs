using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.HR_Pay_Roll_Service.Hr_PRoll_FineDeducation
{
    public class FineDeductionDetailsViewModel
    {
        public long FineDeducationDetailId { get; set; }
        public long FineDeducationId { get; set; }
        public long EmployeeId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string EmployeeKgId { get; set; }
        public string EmployeeName { get; set; }
    }
}
