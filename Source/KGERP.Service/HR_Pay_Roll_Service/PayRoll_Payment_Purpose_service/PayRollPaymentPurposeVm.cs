using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.PayRoll_Payment_Purpose_service
{
   
     
    public partial class PRoll_PaymentPurposeVm
    {
        public int PaymentPurposeId { get; set; }
        public string Name { get; set; }
        public string NameInBangla { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public List<PRoll_PaymentPurposeVm> DataList { get; set; }

    }

}
