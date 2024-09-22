using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel
{
    public class SmsModel
    {
    }

    public class SmsVm
    {
        public long Id { get; set; }
        public System.DateTime Date { get; set; }
        public string Subject { get; set; }
        public string PhoneNo { get; set; }
        public string strPhoneNoList  { get; set; }
        public string Message { get; set; }
        public int CompanyId { get; set; }
        public Nullable<int> Status { get; set; }
        public EnumSmSStatus EnumStatus { get { return (EnumSmSStatus)this.Status; } }

        public int TryCount { get; set; }
        public System.DateTime RowTime { get; set; }
        public string Remarks { get; set; }
        public int SmsType { get; set; }
        public string SMSTypeName { get; set; }
        public string CompanyName { get; set; }
    }
    public class SmsListVm
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public int CompanyId { get; set; }
        public int type { get; set; }
        public List<SmsType> SmsTypeList { get; set; }
        public IEnumerable<SmsVm> DataList { get; set; }
    }

    public class SmsCreateVm
    {
        public SmsVm Sms { get; set; }
        public List<SmsType> SmsTypeList { get; set; }
    }

    public class SendSMSViewModel
    {

        #region Ctor
        public SendSMSViewModel()
        {
            this.CompanyList = new List<SelectModel>();
            this.DepartmentList = new List<SelectModel>();
            this.AllEmployeeList = new List<SelectModel>();
            this.IsExternal = false;
        }
        #endregion

        #region standard properties
        [DisplayName("Company")]
        public int? CompanyId { get; set; }
        [DisplayName("Company")]
        public int? SelectedCompanyId { get; set; }
        public List<SmsType> SmsTypeList { get; set; }
        [DisplayName("SMS Catagory")]
        public int SmsTypeId { get; set; }
        public IList<SelectModel> CompanyList { set; get; }

        [DisplayName("Department")]
        public int? DepartmentId { get; set; }
        public List<SelectModel> DepartmentList { set; get; }

        [DisplayName("Employee")]
        public string[] SelectedEmployee { get; set; }

        [DisplayName("Mobile No: 11 digit Must")]
        public string PhoneNo { get; set; }
        public List<SelectModel> AllEmployeeList { set; get; }

        public int? EmployeeId { get; set; }

        [Required]
        [StringLength(450, ErrorMessage = "maximum 450 characters.")]
        public string Message { get; set; }
        public DateTime MessageDate { get; set; } = DateTime.Now;

        public string MessageOption { get; set; }
        public bool IsExternal { get; set; }
        public string ExternalName { get; set; }
        public string Subject { get; set; }
        #endregion

    }
}