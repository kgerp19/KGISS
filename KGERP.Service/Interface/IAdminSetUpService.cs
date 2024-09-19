using KGERP.Service.ServiceModel;

namespace KGERP.Service.Interface
{
    public interface IAdminSetUpService
    {
        AdminSetUpModel GetAdminSetUps();
        //List<SelectModel> GetEmployeeSelectModels();
        //AdminSetUpModel GetAdminSetUp(long id);
        //List<SelectModel> StatusSelectModels();
        //bool SaveAdminSetUp(long id, AdminSetUpModel adminSetUp);
    }
}
