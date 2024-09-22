using KGERP.Service.HR_Pay_Roll_Service.Food_Bill_Services;
using KGERP.Service.HR_Pay_Roll_Service.Mobile_Bill_Service;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.HR_Pay_Roll_Service.Hr_PRoll_FineDeducation
{
    public class FineDeductionVm
    {
        public long FineDeducationId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        [StringLength(100)]
        public string Remarks { get; set; }
        public int CompanyId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public List<SelectModel> YearsList { get; set; }
        public List<SelectModel> Companies { get; set; }
        public SelectList MonthList { get { return new SelectList(BaseFunctionalities.GetEnumList<MonthList>(), "Value", "Text"); } }
        public List<FineDeductionDetailsViewModel> FineDeductionDetalies { get; set; }
        public FineDeductionDetailsViewModel DetaliesObject { get; set; }
        public IEnumerable<FineDeductionDetailsViewModel> FineDeductionlDetaliesList { get; set; }
        public IEnumerable<FineDeductionVm> FIneDeductionList { get; set; }
        public string CompanyName { get; set; }
        public string StrCreatedDate { get; set; }
    }
}
