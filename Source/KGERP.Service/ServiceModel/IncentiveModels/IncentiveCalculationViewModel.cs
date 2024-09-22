using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.ServiceModel.IncentiveModels
{
    public class IncentiveCalculationViewModel
    {
        public long CGId { set; get; }
        public long MasterId { set; get; }
        public int CompanyId { get; set; }
        public int ProjectId { get; set; }
        public long Bookingid { set; get; }
        public long SalePersone { set; get; }
        public string Fileno { set; get; }
        public string CompanyName { set; get; }
        public int Month { set; get; }
        public int Years { set; get; }
        public string BookingDateString { set; get; }
        public decimal TotalFileValue { set; get; }
        public decimal TotalCollaction { set; get; }
        public decimal TotalCollactionPercentage { set; get; }
        public decimal Totalsum { set; get; }
        public double CollactionPercentage { set; get; }
        public double IncentivePercentage { set; get; }
        public double TotalPercentage { set; get; }
        public decimal IncentiveAmount { set; get; }
        public decimal RestofAmount { set; get; }
        public decimal BookingAmt { set; get; }
        public bool IsIncentive { set; get; }
        public DateTime IncentiveDate { set; get; }
        public List<IncentiveDetailsViewModel> IncentiveDetailsList { set; get; }
        public List<IncentiveCalculationViewModel> MappVm { set; get; }
        public List<SelectModelType> ProjectList { get; set; }
        public SelectList GroupList { get; set; } = new SelectList(new List<object>());
    }
    public class IncentiveDetailsViewModel
    {
        public long Id { get; set; }
        public long MasterId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal Amount { get; set; }
        public double Commission { get; set; }
        public int IncentiveDistributionDetailsid { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }

    }
    public class MyModel
    {
        public long cgID { get; set; }


    }
}
