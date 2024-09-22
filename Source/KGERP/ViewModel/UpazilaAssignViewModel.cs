using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class UpazilaAssignViewModel
    {
        public UpazilaAssignModel UpazilaAssign { get; set; }
        public List<UpazilaAssignModel> UpazilaAssigns { get; set; }
        public List<SelectModel> Districts { get; set; }
    }
}