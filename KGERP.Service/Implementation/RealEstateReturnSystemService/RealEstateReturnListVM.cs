using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Implementation.RealEstateReturnSystemService
{
    public class RealEstateReturnListVM
    {
        public int? CompanyId { get; set; }
        public long CGId { get; set; }
        public long DocId { get; set; }
        public int? ProjectId { get; set; }
        public int? StatusId { get; set; }
        public int? Status { get; set; }
        public long BookingId { get; set; }
        public long ReturnId { get; set; }
        public long IsAccAproval { get; set; }
        public string BookingNo { get; set; }
        [Required]
        public string FileNo { get; set; }
        public string Project { get; set; }
        public int? VendorId { get; set; }
        public string PlotName { get; set; }
        public string BlockName { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public string Particular { get; set; }
        public string StringSubmidtate  { get; set; }
        public decimal FileValue { get; set; }
        public int? Accounting_BankOrCashId { get; set; }
        public string AccountingHeadName { get; set; }
        public string message { get; set; }
        public int? AccHeadId { get; set; }
        public List<RealEstateReturnListVM> datalist { get; set; }
    }
}
