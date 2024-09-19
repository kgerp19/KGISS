using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Web;

namespace KGERP.ViewModel
{
    public class ShareHolderViewModel
    {
        public ShareHolderModel ShareHolder { get; set; }
        public List<SelectModel> Professions { get; set; }
        public List<SelectModel> EducationQualifications { get; set; }
        public List<SelectModel> Gender { get; set; }
        public List<SelectModel> ShareHolderTypes { get; set; }
        public HttpPostedFileBase ShareHolderImageUpload { get; set; }
        public HttpPostedFileBase NIDImageUpload { get; set; }

    }
}