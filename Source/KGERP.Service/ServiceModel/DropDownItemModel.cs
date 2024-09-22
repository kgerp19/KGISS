using KGERP.Service.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class DropDownItemModel : BaseVM
    {
        public string ButtonName
        {
            get
            {
                return DropDownItemId > 0 ? "Update" : "Save";
            }
        }
        [DisplayName("Item ID")]
        public int DropDownItemId { get; set; }
        public int CompanyId { get; set; }
        [Required]
        [DisplayName("Type")]
        public Nullable<int> DropDownTypeId { get; set; }
        [Required]
        [DisplayName("Item Name")]
        public string Name { get; set; }
       
        [DisplayName("Item Value")]
        public int ItemValue { get; set; }     
        public virtual DropDownTypeModel DropDownType { get; set; }
        public string TypeName { get; set; }
        public IEnumerable<DropDownItemModel> DataList { get; set; }
    }
}
