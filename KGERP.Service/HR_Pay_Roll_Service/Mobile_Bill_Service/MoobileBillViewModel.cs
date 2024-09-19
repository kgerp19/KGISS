using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service
{
    public class MoobileBillViewModel
    {
        public long MobileBillId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StrCreatedDate { get; set; }


        public HttpPostedFileBase MobileBillExcel { get; set; }
        public List<SelectModel> YearsList { get; set; }
        public List<SelectModel> Companies { get; set; }
        public SelectList MonthList { get { return new SelectList(BaseFunctionalities.GetEnumList<MonthList>(), "Value", "Text"); } }
        public List<MobileBillDetaliesViewModel> MobileBillDetalies { get; set; }
        public MobileBillDetaliesViewModel DetaliesObject { get; set; }
        public IEnumerable<MobileBillDetaliesViewModel> MobileBillDetaliesList { get; set; }
        public IEnumerable<MoobileBillViewModel> MobileBillList { get; set; }
    }



    public class ExcelMobileBillMapperVM
    {

        public long Id { get; set; }
        public string EmployeeId { get; set; }

        public decimal Amount { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        public int CompanyId { get; set; }
    }
}
