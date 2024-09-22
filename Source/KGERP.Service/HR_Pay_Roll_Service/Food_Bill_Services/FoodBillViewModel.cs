using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services
{
    public class FoodBillViewModel
    {
        public long FoodBillId {get; set; }
        public int Month {get; set; }
        public int Year {get; set; }
        public int CompanyId {get; set; }
        public string CompanyName {get; set; }
        public string Remarks {get; set; }
        public decimal LunchRate {get; set; }
        public decimal BreakfastRate {get; set; }
        public string CreatedBy {get; set; }
        public DateTime CreatedDate {get; set; }
        public string ModifiedBy {get; set; }
        public string StrCreatedDate { get; set; }

        public List<SelectModel> YearsList { get; set; }
        public List<SelectModel> Companies { get; set; }
        public SelectList MonthList { get { return new SelectList(BaseFunctionalities.GetEnumList<MonthList>(), "Value", "Text"); } }
        public List<FoodBillDetaliesViewModel> FoodBillDetalies { get; set; }
        public FoodBillDetaliesViewModel DetaliesObject { get; set; }
        public IEnumerable<FoodBillDetaliesViewModel> FoodBillDetaliesList { get; set; }
        public IEnumerable<FoodBillViewModel> FoodBillList { get; set; }
    }
}
