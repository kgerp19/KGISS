using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KGERP.Service.ServiceModel
{
    public class KGREProjectModel
    {
        public string ButtonName
        {
            get
            {
                return ProjectId > 0 ? "Modify" : "Save";
            }
        }
        public int ProjectId { get; set; }
        [DisplayName("Company")]
        public Nullable<int> CompanyId { get; set; }
        [DisplayName("Project Name")]
        public string ProjectName { get; set; }
        public string Address { get; set; }
        [DisplayName("Total Plot")]
        public Nullable<int> TotalPlot { get; set; }
        [DisplayName("Total Plot/ Flat")]
        public Nullable<int> TotalFlat { get; set; }
        public Nullable<int> UnitRate { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public List<SelectModel> PlotFlats { get; set; }
        [DisplayName("Plot Status")]

        public List<SelectModel> PStatus { get; set; }
        public List<SelectModel> Companies { get; set; }

        #region
        public string ButtonNameP
        {
            get
            {
                return PlotId > 0 ? "Modify" : "Save";
            }
        }

        public List<SelectModel> KGREProjects { get; set; }
        [DisplayName("Plot No")]
        public int PlotId { get; set; }
        [DisplayName("Plot Face")]
        public string PlotFace { get; set; }
        [DisplayName("Plot Size")]
        public string PlotSize { get; set; }
        [DisplayName("Block No")]
        public string BlockNo { get; set; }
        public string Remark { get; set; }
        [DisplayName("Booking")]
        public Nullable<int> ProjectBooking { get; set; }
        [DisplayName("Plot No")]
        public string PlotNo { get; set; }

        [DisplayName("Plot Status")]
        public string PltStatus { get; set; }

        [DisplayName("Plot Status")]
        public Nullable<int> PlotStatus { get; set; }
        [DisplayName("Project")]
        public KGREProject KGREProject { get; set; }

        #endregion
    }
}
