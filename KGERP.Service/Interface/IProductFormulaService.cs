using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IProductFormulaService
    {
        List<ProductFormulaModel> GetRawMaterials(int productId);
        ProductFormulaModel GetRawMaterial(int id, int productId);
        bool SaveProductFormula(int id, ProductFormulaModel model, out string message);
        ProductFormulaModel GetProductFormula(int id);
        bool DeleteProductFormula(int id);
        Task<ProductFormulaModel> GetProductFormulas(int companyId, DateTime? fromDate, DateTime? toDate);
        bool SavePFormulaDetail(int id, PFormulaDetailModel model);
        PFormulaDetailModel GetPFormulaDetail(int pFormulaDetailId);
        ProductFormulaModel GetProductFormulaUsingProductId(int productId);
    }
}
