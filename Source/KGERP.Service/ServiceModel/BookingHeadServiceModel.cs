using KGERP.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
    public class BookingHeadServiceModel:BookingCostHead
    {
        public string companyName { get; set; }
        public long BookingId { get; set; }
        public decimal Amount { get; set; }
        public decimal ReceiveAmount { get; set; }
        public decimal AddNewAmount { get; set; }
        public decimal Percentage { get; set; }
        public decimal PaidAmount { get; set; }
        public bool IsSnstallmentInclude { get; set; }
        public long CostsMappingId { get; set; }
    }
    public class BookingCostHeadListViewModel
    {
        public int CompanyId { get; set; }
        public List<BookingHeadServiceModel> List { get; set; }
    }

    public class BookingHeadInsertModel
    {
       
        public int CompanyId { get; set; }
        [Display(Name ="Cost Head Title")]
        public string CostName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
    public class BookingHeadEditModel
    {
        public long CostId { get; set; }
        public int CompanyId { get; set; }
        public string CostName { get; set; }
        public bool IsActive { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
