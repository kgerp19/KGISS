using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class LandNLegalViewModel
    {
        // internal LandNLegalModel landNLegal;

        public LandNLegalModel _LandNLegal { get; set; } 

        public LandNLegalViewModel()
        {
            LandNLegalModel landNLegal = new LandNLegalModel();
        }
        public IEnumerable<LandNLegal> LnLs { get; set; }
        public IEnumerable<CaseComment> Comments { get; set; }
        public IEnumerable<CaseHistory> Histories { get; set; }
        //public IEnumerable<CaseAttachment> Attaches { get; set; }

        public List<SelectModel> Companies { get; set; }
        public List<SelectModel> CaseTypes { get; set; }
        public List<SelectModel> CourtNames { get; set; }
        public List<SelectModel> AdditionalDistrictCourts { get; set; }
        public List<SelectModel> CaseStatuss { get; set; }
        public List<SelectModel> ResponsiblePersons { get; set; }

        public List<SelectModel> Divisions { get; set; }
        public List<SelectModel> Districts { get; set; }
        public List<SelectModel> Upazilas { get; set; }
        public List<SelectModel> Banks { get; set; }
    }
}