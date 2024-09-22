using KGERP.Data.Models;

namespace KGERP.Service.Interface
{
    public interface IAccountSignatoryService
    {
        bool SaveSignatory(int id, Accounting_Signatory model);
    }
}
