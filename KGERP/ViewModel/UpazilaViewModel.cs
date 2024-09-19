using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class UpazilaViewModel
    {
        public UpazilaModel Upazila { get; set; }
        public List<SelectModel> Districts { get; set; }
    }
}