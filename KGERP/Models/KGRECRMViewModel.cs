
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KGERP.Models
{
    public class KGRECRMViewModel
    {

    }
    public class KGREClientSViewModel
    {
        public List<SelectModel> SourceOfMedias { get; set; }
        public List<SelectModel> PromotionalOffers { get; set; }
        public List<SelectModel> PlotFlats { get; set; }
        public List<SelectModel> Impressions { get; set; }
        public List<SelectModel> StatusLevels { get; set; }
        public List<SelectModel> KGREProjects { get; set; }
        public List<SelectModel> KGREChoiceAreas { get; set; }
        public List<SelectModel> ResponsiblePersons { get; set; }
        public int? CompanyId { get; set; }
    }
    public class KGREClientsShortViewModel
    {
       public Nullable<DateTime> CreatedDate { get; set; }
        
        public string strCreatedDate { get; set; } = "";
       public string ResponsibleOfficer { get; set; }
       public string FullName { get; set; }
       public string Designation { get; set; }
       public string DepartmentOrInstitution { get; set; }
       public string SourceOfMedia { get; set; }
       public string StatusLevel { get; set; }
       public string MobileNo { get; set; }
       public string Project { get; set; }
       public int ClientId { get; set; }
        public int? ProjectId { get; set; }
        public int? CompanyId { get; set; }
    }
}