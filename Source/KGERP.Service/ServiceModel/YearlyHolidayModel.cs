using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class YearlyHolidayModel
    {
        public int YearlyHolidayId { get; set; }
        [DisplayName("Holiday Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime HolidayDate { get; set; }
        [DisplayName("Holiday Category")]
        public string HolidayCategory { get; set; }
        public string Purpose { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public IEnumerable<YearlyHolidayModel> DataList { get; set; }


        public bool IsUpcoming { get; set; }
    }
}
