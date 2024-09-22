using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class CompanySubMenuViewModel
    {
        public CompanySubMenuModel CompanySubMenu { get; set; }
        public List<SelectModel> Companies { get; set; }
        public List<SelectModel> CompanyMenus { get; set; }
    }
}