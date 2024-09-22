using KGERP.Data.Models;

namespace KGERP.ViewModel
{
    public class AttendenceViewModel
    {
        public AttendenceApproveApplication Attendence { get; set; }
        public AttendenceViewModel()
        {
            AttendenceApproveApplication attendence = new AttendenceApproveApplication();
        }
    }
}