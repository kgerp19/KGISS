using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class WorkAssignViewModel
    {
        public WorkAssignModel WorkAssign { get; set; }
        public List<WorkAssignModel> WorkAssigns { get; set; }
        public List<SelectModel> AssignMembers { get; set; }
    }
}