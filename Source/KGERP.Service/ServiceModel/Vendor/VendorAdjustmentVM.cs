using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel.Vendor
{
    public class VendorAdjustmentVM : BaseVM
    {
        public long AdjustmentId { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string IntegratedFrom { get; set; }

        public long? OrderDeliverId { get; set; }
        public long? OrderMasterId { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
       
        public string Narration { get; set; }
       
        public DateTime? CreateDate { get; set; }
        
        public bool IsSubmit { get; set; }

        [Display(Name ="Zone")]
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public List<SelectModel> ZoneList { get; set; }
        public List<SelectModel> SubZoneList { get; set; }
        public List<SelectModelType> VendorList { get; set; }

        [Display(Name = "Subzone")]
        public int SubZoneId { get; set; }
        public string SubZoneName { get; set; }
        public string AccountingHeadName { get; set; }
        public int Accounting_HeadFK { get; set; }      
        public int UserCompnayId { get; set; }
        public int? HeadGLId { get; set; }
        public SelectList ExistingChallanList { get; set; } = new SelectList(new List<object>());
        public SelectList OrderListList { get; set; } = new SelectList(new List<object>());
        public IEnumerable<VendorAdjustmentVM> VendorAdjustmentList { get; set; }
    }
}
