using KGERP.Data.CustomModel;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace KGERP.ViewModel
{
    public class VendorViewModel
    {
        public VendorModel Vendor { get; set; }
        public List<CustomerReceivableCustomModel> CustomerReceivables { get; set; }
        public SelectList PaymentTypeList { get; set; } = new SelectList(new List<object>());

        public List<PaymentModel> Payments { get; set; }
        public List<SelectModel> Districts { get; set; }
        public List<SelectModel> Countries { get; set; }
        public List<SelectModel> Zones { get; set; }
        public List<SelectModel> Upazilas { get; set; }
        public List<SelectModel> SubZones { get; set; }
        public List<IntDropDownModel> CustomerCodes { get; set; }
        public List<MonthSelectModel> Months { get; set; }
        public HttpPostedFileBase VendorImageUpload { get; set; }
        public HttpPostedFileBase NomineeImageUpload { get; set; }

    }
}