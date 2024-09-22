using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.ViewModel
{
    public class PFormulaDetailViewModel
    {
        public ProductFormulaModel ProductFormula { get; set; }
        public PFormulaDetailModel PFormulaDetail { get; set; }
        public List<PFormulaDetailModel> PFormulaDetails { get; set; }
        public List<SelectModel> RawMaterials { get; set; }
    }
}