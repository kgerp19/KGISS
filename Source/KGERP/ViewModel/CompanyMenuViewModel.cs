using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class CompanyMenuViewModel
    {
        public CompanyMenuModel CompanyMenu { get; set; }
        public List<SelectModel> Companies { get; set; }
    }
}