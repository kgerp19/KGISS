using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
    public class BusinessHeadModel
    {
        public long Id { get; set; }
        public int BusinessTypeInt { get; set; }
        public BusinessTypeEnum BusinessType { get; set; }

        [DisplayName("Business Name")]
        [Required(ErrorMessage ="This field is required")]
        public int BusineesId_Fk { get; set; }
        public string BusinessName { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string FromDateString 
        {
            get
            {
                if (FromDate != null)
                {
                    return FromDate.ToString("yyyy-MM-dd");
                }
                return "";
            }
        }
        public string ToDateString 
        { 
            get
            {
                if (ToDate != null)
                {
                    return ToDate.Value.ToString("yyyy-MM-dd");
                }
                return "";
            }
        }
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ToDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<BusinessHeadModel> DataList { get; set; }
        public ActionEnum ActionType { get; set; }

        [Range(0, 5)]
        public int Precedence { get; set; }
    }
}
