using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace KGERP.Service.ServiceModel
{
    public class ECMemberModel
    {
        public string ButtonName
        {
            get
            {
                return ECMemberId > 0 ? "Update" : "Create";
            }
        }
        public int ECMemberId { get; set; }
        [Required]
        [DisplayName("Member Name")]
        public string MemberName { get; set; }
        [DisplayName("Member Priority")]
        public int MemberOrder { get; set; }
        [DisplayName("Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [DisplayName("End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Remarks { get; set; }
        [DisplayName("Member Picture")]
        public string MemberImage { get; set; }
        [DisplayName("Active")]
        public bool IsActive { get; set; }
        //------------------Extended Properties----------------
        public HttpPostedFileBase MemberImageUpload { get; set; }
    }
}
