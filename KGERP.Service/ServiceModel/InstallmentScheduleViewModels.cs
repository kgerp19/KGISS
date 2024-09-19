using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
    public class InstallmentScheduleViewModels
    {
    }
    public class InstallmentSchedulePayment
    {
        public string Title { get; set; }
        public DateTime InstallmentDate { get; set; }
        public string StringDate { get; set; }
        public decimal InAmount { get; set; }
        public int PaymentId { get; set; }
        public long? PaymentMasterId { get; set; }
        public long? InstallmentId { get; set; }
        public string InstallmentName { get; set; }
        public string ChequeNo { get; set; }
        public string MoneyReceiptNo { get; set; }
        public string ReceiveLocation { get; set; }
        public long? CGId { get; set; }
        public int? HeadGLId { get; set; }
    }
    public class InstallmentScheduleShortModel
    {
        public string Title { get; set; }
        public DateTime InstallmentDate { get; set; }
        public string StringDate { get; set; }
        public decimal PayableAmount { get; set; }
        public int SortOrder{ get; set; }
        public long InstallmentId { get; set; }
        public long CostsMappingId { get; set; }
        public string InstallmentName { get; set; }
        public decimal PaidAmount { get; set; } = 0;
        public bool  IsPaid { get; set; }
    }
    public class GeneratedInstallmentSchedule
    {
        public List<InstallmentScheduleShortModel> LstSchedules { get; set; } = new List<InstallmentScheduleShortModel>();
        public bool IsGenerated { get; set; } = false;
    }
}
