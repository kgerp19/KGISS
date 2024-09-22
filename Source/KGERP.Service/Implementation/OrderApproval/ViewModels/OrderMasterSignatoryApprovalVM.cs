using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace KGERP.Service.Implementation.OrderApproval.ViewModels
{
    public class OrderMasterSignatoryApprovalVM
    {
        public long Id { get; set; }
        public int Precedence { get; set; }
        public long ApproverEmployeeIntId { get; set; }

        public int ApprovalOrderMasterSignatoryId { get; set; }
        public int? CompanyId { get; set; }
        public long OrderMasterId { get; set; }

        public string Comments { get; set; }
        public string ProductType { get; set; }
        public string OrderNo { get; set; }
        public string CoustomerName { get; set; }


        public DateTime? ApprovalDate { get; set; }
        public DateTime? OrderDate { get; set; }

        public SignatoryStatusEnum SignatoryStatus { get; set; }



        public string EmployeeName { get; set; }
        public long EmployeeIdInt { get; set; }
        public string EmployeeId { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public int Status { get; set; }

        public bool IsPreviousApproved { get; set; }
        public List<OrderMasterSignatoryApprovalVM> datalist { get; set; }
    }
}
