using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class BoardOfDirectorViewModel
    {
        public BoardOfDirectorModel BoardOfDirector { get; set; }
        public List<SelectModel> Professions { get; set; }
        public List<SelectModel> EducationQualifications { get; set; }
    }
}