using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IEmiService : IDisposable
    {
        bool SaveOrEditEmi(EMIModel model);
        EMIModel GetEmi(long id);
        List<EMIModel> GetEmiInfoList();
        List<SelectModel> GetOrderinvoiceByCustomer(int customerId, int companyId);
        decimal GetSalesValue(int orderId);
        List<EmiDetailModel> GetEmiDetails(DateTime installmentDate, int noOfInstallment, int installmentAmount);
    }
}
