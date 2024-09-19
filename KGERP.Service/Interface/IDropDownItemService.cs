using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KGERP.Service.Interface
{
    public interface IDropDownItemService
    {
        List<DropDownItemModel> GetDropDownItems(string searchText);
        List<SelectModel> GetDropDownItemSelectModels(int id);
        DropDownItemModel GetDropDownItem(int id);
        bool SaveDropDownItem(int id, DropDownItemModel upazila, out string message);
        bool DeleteDropDownItem(int id);
        Task<DropDownItemModel> GetDropDownItems(int companyId);
        // List<SelectModel> getCompanySelectModels();
        List<SelectModel> GetRegionSelectModels();

        List<SelectModel> GetDDLVendors(int companyId);

        Task<List<SelectListItem>> GetDDLProductList(int companyId);

        Task<List<SelectListItem>> GetDDLCompanyWiseOrder(int CustomerId);

        Task<List<SelectListItem>> GetDDLOrderWiseProduct(int orderMasterId);
        Task<List<SelectListItem>> GetDDLCustomerByCompany(int companyId,CancellationToken cancellationToken=default);
        Task<List<SelectListItem>> GetDDLOrderByCustomer(long VendorId,CancellationToken cancellationToken=default);
        Task<List<SelectListItem>> GetDDLOrderDeliveryByOrderMaster(long OrderMasterId, CancellationToken cancellationToken=default);
        Task<List<SelectListItem>> GetDDLOrderDeliveryDetailsByOrderDelivery(long OrderDeliveryId, CancellationToken cancellationToken=default);
        Task<List<SelectListItem>> GetDDLAllEmployeeByCompanyId(int CompanyId=0, CancellationToken cancellationToken=default);
    }
}
