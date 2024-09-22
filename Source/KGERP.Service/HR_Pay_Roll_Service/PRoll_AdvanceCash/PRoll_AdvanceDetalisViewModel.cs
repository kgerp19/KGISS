using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.HR_Pay_Roll_Service.PRoll_AdvanceCash
{
    public class PRoll_AdvanceDetalisViewModel
    {
        public long AdvanceDetailId { get; set; }
        public long AdvanceId { get; set; }
        public int PaymenyMonth { get; set; }
        public int PaymenyYear { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
