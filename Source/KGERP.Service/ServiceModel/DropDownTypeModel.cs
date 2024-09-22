using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class DropDownTypeModel
    {
        [DisplayName("Id")]
        public int DropDownTypeId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        public IEnumerable<DropDownTypeModel> DataList { get; set; }
    }
}
