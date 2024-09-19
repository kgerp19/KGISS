using KGERP.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
    public class InstallmentTypeViewModel
        
    {
        public List<BookingInstallmentType> List { get; set; }
        public int CompanyId { get; set; }
    }
    public class InstallmentTypeInsertModel
    {
        [Display(Name="Title")]
        public string Name { get; set; }
        [Display(Name = "Is One Time Payment?")]
        public bool IsOneTime { get; set; }

        public int CompanyId { get; set; }
        [Display(Name = "Interval Months")]
        public int IntervalMonths { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }=DateTime.Now;
    }
    public class InstallmentTypeEditModel
    {
        public int InstallmentTypeId { get; set; }
        public string Name { get; set; }
        [Display(Name ="Is One Time Payment?")]
        public bool IsOneTime { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "Interval Months")]
        public int IntervalMonths { get; set; }
        public string ModifiedBy { get; set; } 
        public Nullable<System.DateTime> ModifiedDate { get; set; } = DateTime.Now;
    }
}
