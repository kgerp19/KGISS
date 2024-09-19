using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.IncentivesDistribution
{
    public class IncentiveCateGoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int CompanyId { get; set; }
        public List<SelectModelType> Catagorieslist { get; set; }
        public List<IncentiveCateGoryViewModel> Datalist { get; set; }
    }
}
