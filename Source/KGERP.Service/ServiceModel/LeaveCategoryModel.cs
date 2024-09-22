using System;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class LeaveCategoryModel
    {
        public string ButtonName
        {
            get
            {
                return LeaveCategoryId > 0 ? "Update" : "Create";
            }
        }
        public int LeaveCategoryId { get; set; }
        [DisplayName("Leave Category")]
        public string Name { get; set; }
        [DisplayName("Grade")]
        public Nullable<int> GradeId { get; set; }
        public string Type { get; set; }
        [DisplayName("Maximum Days")]
        public Nullable<int> MaxDays { get; set; }
        [DisplayName("Post Flag")]
        public Nullable<int> PostFlag { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public bool IsActive { get; set; }

        public int Sequence { get; set; }

    }
}
