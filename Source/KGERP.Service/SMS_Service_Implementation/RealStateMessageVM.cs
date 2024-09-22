using KGERP.Data.Models;
using KGERP.Service.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.SMS_Service_Implementation
{
    public class RealStateMessageVM
    {
        public long ID { get; set; }
        public int CompanyId { get; set; }
        public int Tag { get; set; }
        public int Agenda { get; set; }
        public int MediaType { get; set; }
        public string Message { get; set; }
        public string Address { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public int TryCount { get; set; }
        public string TrackId { get; set; }
        public string SMSLog { get; set; }
        public List<RealStateMessage> reals { get; set; }
        public RealStateMessageVM vM { get; set; }  
    }
    public class MessageViewModel : BaseVM
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<MyStoredProcedureResult> Result { get; set; }
        public int CompanyId { get; set; }
        public string Message { get; set; }
        public int SelectedOption { get; set; }


    }
    public class MyStoredProcedureResult
    {
        public int CompanyId { get; set; }
        public int Agenda { get; set; }
        public int MediaType { get; set; }
        public string Message { get; set; }
        public string Address { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int Status { get; set; }
        public int IsActive { get; set; }
        public int TryCount { get; set; }
        public int TrackId { get; set; }
        public string SMSLog { get; set; }
    }

}
