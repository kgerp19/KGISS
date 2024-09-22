using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.OrderApproval.ViewModels
{
    public class OrderMasterSignatoryVM
    {

        public int SignatoryId { get; set; }
        public int Precedence { get; set; }
        public int CompanyId { get; set; }


        public long EmployeeIdInt { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Customer { get; set; }
        public string SalsePersone { get; set; }
        public string EmployeeMobile { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }

        public SignatoryStatusEnum  SignatoryStatus { get; set; }



    }
}
