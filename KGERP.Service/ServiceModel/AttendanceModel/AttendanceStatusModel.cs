using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel.AttendanceModel
{
    public static class AttendanceStatusModel
    {
        public const string Ok = "OK";
        public const string Absent = "Absent";
        public const string Weekend = "Off Day";
        public const string Holiday = "Holiday";
        public const string OnField = "On Field";
        public const string OnLeave = "On Leave";
        public const string Tour = "Tour";
        public const string LateIn = "Late In";
        public const string EarlyOut = "Early Out";
        public const string LateInEarlyOut = "Late In & Early Out";
        public const string OnFieldDutyRequest = "On Field Duty";
        public const string AttendanceModifiedRequest = "Attendance Modify";
    }
}
