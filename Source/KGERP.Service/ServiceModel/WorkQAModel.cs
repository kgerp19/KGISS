using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

namespace KGERP.Service.ServiceModel
{
    public class WorkQAModel
    {
        public long WorkQAId { get; set; }
        public Nullable<long> FromEmpId { get; set; }
        public Nullable<long> ToEmpId { get; set; }
        [DisplayName("Comments")]
        public string Conversation { get; set; }
        public Nullable<System.DateTime> ConversationDate { get; set; }
        public Nullable<long> ParentWorkQAId { get; set; }


        public virtual ICollection<WorkQAFileModel> WorkQAFiles { get; set; }

        //----------------------------Extended Properties----------------------

        public HttpPostedFileBase[] files { get; set; }
        public string FromKGID { get; set; }
        public string ToKGID { get; set; }
        [DisplayName("From")]
        public string FromName { get; set; }
        [DisplayName("To")]
        public string ToName { get; set; }
        public string FromEmpImage { get; set; }
        public string ToEmpImage { get; set; }
        public string Reply { get; set; }


    }
}
