using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.RealEstateReturnSystemService
{
    public class RealEstateReturnSystemVM
    {
        public int CompanyId { get; set; }
        public DateTime CancelDate { get; set; }
        public string strCancelDate { get; set; }
        public long CGId { get; set; }
        public long DocId { get; set; }
        public int? ProjectId { get; set; }
        [Display(Name = "Status")]
        public int? StatusId { get; set; }
        [Display(Name = "Narration")]
        public string Purpose { get; set; }
        [Display(Name = "Return  Fee")]
        public decimal ReturnFee { get; set; }
        [Display(Name = "Return Percentage Fee")]
        public decimal ReturnFeePercentage { get; set; }
        [Display(Name = "Total Collaction")]
        public decimal TotalCollaction { get; set; }
        [Display(Name = "Uploaded File")]
        public HttpPostedFileBase UploadedFile { get; set; }
        public string UploadedFileurl { get; set; }
        public List<SelectModelType> ProjectList { get; set; }
        public SelectList GroupList { get; set; } = new SelectList(new List<object>());
        public SelectList RealEstateReturn { get { return new SelectList(BaseFunctionalities.GetEnumList<RealStateReturnsStatus>(), "Value", "Text"); } }
       public  GLDLBookingViewModel BookingViewModel { get; set; }

    }
}
