using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Web;

namespace KGERP.ViewModel
{
    public class WorkViewModel
    {
        public WorkModel Work { get; set; }
        public WorkAssignModel WorkAssign { get; set; }
        public List<SelectModel> ManagerWorkStates { get; set; }
        public List<SelectModel> MemberWorkStates { get; set; }
        public HttpPostedFileBase[] files { get; set; }
    }
}