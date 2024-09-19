using KGERP.Data.Models;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;

namespace KGERP.Service.HR_Pay_Roll_Service.SpecialAddition
{
    public class SpecialAdditionViewModel
    {
        public long SpecialAdditionId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int CompanyId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string CompanyName { get; set; }
        public HttpPostedFileBase SpecialAdditionExcel { get; set; }
        public List<SelectModel> YearsList { get; set; }
        public List<SelectModel> Companies { get; set; }
        public SelectList MonthList { get { return new SelectList(BaseFunctionalities.GetEnumList<MonthList>(), "Value", "Text"); } }
        public List<SpecialAdditionDetailsViewModel> SpecialAditionDetaliesListDetalies { get; set; }
        public SpecialAdditionDetailsViewModel DetaliesObject { get; set; }
        public IEnumerable<SpecialAdditionDetailsViewModel> SpecialAditionDetaliesList { get; set; }
        public IEnumerable<SpecialAdditionViewModel> SpecialAditionList{ get; set; }
        public string StrCreatedDate { get; set; }
    }
}
