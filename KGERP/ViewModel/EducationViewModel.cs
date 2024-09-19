using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class EducationViewModel
    {
        public EducationModel Education { get; set; }
        public List<SelectModel> Examinations { get; set; }
        public List<SelectModel> Institutions { get; set; }
        public List<SelectModel> Subjects { get; set; }
        public List<SelectModel> Years { get; set; }
    }
}