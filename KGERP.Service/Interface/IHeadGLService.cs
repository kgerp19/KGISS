using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IHeadGLService : IDisposable
    {
        //List<AccountHeadModel> GetAccountHeads(int companyId, string searchText); 
        //AccountHeadModel GetAccountHead(long id); 
        //object GetAccountHeadAutoComplete(string prefix,int companyId);
        //List<SelectModel> GetParentAccountHeadSelectModelByCompany(int companyId);
        //bool SaveAccountHead(long id, AccountHeadModel model);
        //bool DeleteAccountHead(long id);
        //string GenerateNewAccountHead(int? parentId);
        //object GetAccountHeadTreeView();
        //List<SelectModel> GetAccountHeadSelectModelsByCompany(int companyId);
        //List<AccountHeadModel> GetAccountHeadsByCompany(int companyId);
        List<Head1> GetAccountHeadsTreeViewByCompany(int companyId);
        AccountHeadProcessModel GetAccountHeadProcessCreate(int accountHeadId, int layerNo, string status);
        AccountHeadProcessModel GetSelectedItem(int accountHeadId, int layerNo, string accCode, string accName);
        bool SaveAccountHead(AccountHeadProcessModel model);
        AccountHeadProcessModel GetAccountHeadProcessUpdate(int accountHeadId, int layerNo, string status);
        object GetAccountHeadAutoComplete(string prefix, int companyId);
        object GetMemberHeadAutoComplete(string prefix, int companyId);
        object GetPFHeadAutoComplete(string prefix, int companyId);
        object AllAccountsHead(string prefix, int companyId);

        AccountHeadProcessModel GetAccountHeadProcessDelete(int accountHeadId, int layerNo, string status);
        List<SelectModel> GetTeritorySelectModelsByZone(int companyId, int zoneId);
        List<IntDropDownModel> GetInsertCustomerCode(int vendorTypeId, int companyId);
        List<IntDropDownModel> GetUpdateCustomerCode(int vendorTypeId, int companyId);

        Task<VMAccountHead> GetAccountingHeadList(int companyId);


    }
}
