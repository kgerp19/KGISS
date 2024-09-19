using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class CompanyUserMenuViewModel
    {
        public CompanyUserMenuModel CompanyUserMenu { get; set; }
        public List<SelectModel> Companies { get; set; }
        public List<SelectModel> CompanyMenus { get; set; }
        public List<SelectModel> CompanySubMenus { get; set; }
    }
}