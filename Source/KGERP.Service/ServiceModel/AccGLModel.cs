using System;
using System.Web;

namespace KGERP.Service.ServiceModel
{
    public class AccGLModel
    {
        public long AccGLId { get; set; }
        public string vnumber { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public string actcode { get; set; }
        public string actname { get; set; }
        public string mod_pay { get; set; }
        public Nullable<System.DateTime> chq_date { get; set; }
        public Nullable<double> chq_no { get; set; }
        public Nullable<double> damount { get; set; }
        public Nullable<double> camount { get; set; }
        public string opb { get; set; }
        public string bank { get; set; }
        public string proj { get; set; }
        public string chq_nm { get; set; }
        public string ap { get; set; }

        //-------------------Extended Properties----------------------
        public HttpPostedFileBase UploadedFile { get; set; }
        public int CompanyId { get; set; }
    }
}
