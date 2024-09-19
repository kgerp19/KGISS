using KGERP.Data.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Configuration
{


    public class IncentiveDistributionDetailVm
    {
        public long Id { get; set; }
        public int IncentiveDistributionChartId { get; set; }
        public double PromotionalPercentage { get; set; }
        public double? commition { get; set; }
        public bool IsSalesPersone { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public long EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public string EmployeeName { get; set; }
        public string catagoryname { get; set; }
        public List<IncentiveDistributionDetailVm> datalist { get; set; }
        public List<SelectVm> SelectItem { get; set; }


    }
}
