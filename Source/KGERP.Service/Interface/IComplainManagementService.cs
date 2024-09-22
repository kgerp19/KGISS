
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IComplainManagementService : IDisposable
    {
        (List<ComplainManagementModel> complain, int totalRows) GetAllComplain(int start, int length, string searchValue, string sortColumnName, string sortDirection, int companyId);
        List<SelectItemList> GetComplainType(int companyId);
        bool SaveOrEdit(ComplainManagementModel model);
        ComplainManagementModel GetComplain(int id);
        bool DeleteComplain(int id);


    }
}
