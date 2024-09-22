using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IProductDetailService : IDisposable
    {
        List<ProductDetailModel> GetProductDetail();
        ProductDetailModel GetProductDetailById(int id);
        bool SaveOrEdit(List<ProductDetailModel> model);
        bool Update(ProductDetailModel model);
        List<SelectModel> ProductDetail(int id);
        List<IntSelectModel> ProductDetailByLcNo(string lcNo);
    }
}
