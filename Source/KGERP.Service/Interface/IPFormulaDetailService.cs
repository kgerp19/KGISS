using KGERP.Service.ServiceModel;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IPFormulaDetailService
    {
        List<PFormulaDetailModel> GetFormulaDetails(int productFormulaId);
        PFormulaDetailModel GetFormulaDetail(int pFormulaDetailId);
        bool DeletePFormulaDetail(int pFormulaDetailId);
    }
}
