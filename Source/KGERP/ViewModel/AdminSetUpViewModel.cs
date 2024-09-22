using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class AdminSetUpViewModel
    {
        public AdminSetUpModel AdminSetUp { get; set; }
        public List<SelectModel> Employees { get; set; }
        public List<SelectModel> StatusSelectModels { get; set; }
    }
}