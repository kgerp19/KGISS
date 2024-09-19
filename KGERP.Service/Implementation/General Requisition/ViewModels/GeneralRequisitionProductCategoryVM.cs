using KGERP.Service.Configuration;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace KGERP.Service.Implementation.General_Requisition.ViewModels
{
    public class GeneralRequisitionProductCategoryVM
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ActionEnum ActionId { get; set; }
        public IEnumerable<GeneralRequisitionProductCategoryVM> DataList { get; set; }
    }
}
