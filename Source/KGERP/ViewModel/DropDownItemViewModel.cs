using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KGERP.ViewModel
{
    
    public class DropDownItemViewModel
    {
        public DropDownItemModel DropDownItem { get; set; }
        public List<SelectModel> DropDownTypes { get; set; }
    }
}