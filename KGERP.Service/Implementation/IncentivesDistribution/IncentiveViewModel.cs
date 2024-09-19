using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.IncentivesDistribution
{
    public class IncentiveViewModel
    {
        public int Id { get; set; }
        public int? IncentiveCatagoryId { get; set; }
        public string IncentiveCatagoryName { get; set; }
        public double? Commission { get; set; }
        public double? TotalCommission { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public int CompanyId { get; set; }
        public string  CompanyName { get; set; }
        public List<SelectModelType> Catagorieslist { get; set; }
        public List<IncentiveViewModel> datalist { get; set; }
    }
}
