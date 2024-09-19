using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;

namespace KGERP.Service.Implementation
{
    public class AdminSetUpService : IAdminSetUpService
    {
        ERPEntities adminSetUpRepository = new ERPEntities();

        //public string HttpContext { get; private set; }

        public AdminSetUpModel GetAdminSetUps()
        {
            return null;       }

        //public AdminSetUpModel GetAdminSetUp(long id)
        //{
        //    if (id == 0)
        //    {
        //        AdminSetUp adminSetUp = adminSetUpRepository.AdminSetUps.FirstOrDefault(x => x.AdminId == id);
        //        if (adminSetUp == null)
        //        {
        //            return new AdminSetUpModel() { IsActive=true};
        //        }
        //    }

        //    return ObjectConverter<AdminSetUp, AdminSetUpModel>.Convert(adminSetUpRepository.AdminSetUps.FirstOrDefault(x => x.AdminId == id));
        //}

        //public List<SelectModel> GetEmployeeSelectModels()
        //{
        //    return adminSetUpRepository.AdminSetUps.Include("Employee").Select(x => new SelectModel()
        //    {
        //        Text = "[" + x.Employee.EmployeeId.ToString() + "] " + x.Employee.Name,
        //        Value = x.Id.ToString()
        //    }).ToList();
        //}

        //public List<SelectModel> StatusSelectModels()
        //{
        //    List<SelectModel> selectModels = new List<SelectModel>()
        //    {
        //         new SelectModel(){ Text="Active",Value="True"},
        //          new SelectModel(){ Text="InActive",Value="False"},
        //    };

        //    return selectModels;
        //}

        //public bool SaveAdminSetUp(long id, AdminSetUpModel model)
        //{
        //    if (model == null)
        //    {
        //        throw new Exception("Admin SetUp data missing!");
        //    }
        //    AdminSetUp adminSetUp = ObjectConverter<AdminSetUpModel, AdminSetUp>.Convert(model);

        //    if (id > 0)
        //    {
        //        adminSetUp = adminSetUpRepository.AdminSetUps.FirstOrDefault(x => x.AdminId == id);
        //        if (adminSetUp == null)
        //        {
        //            throw new Exception("Admin SetUp not found!");
        //        }
        //        adminSetUp.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
        //        adminSetUp.ModifiedDate = DateTime.Now;

        //    }
        //    else
        //    {
        //        adminSetUp.CreatededBy = System.Web.HttpContext.Current.User.Identity.Name;
        //        adminSetUp.CreatedDate = DateTime.Now;
        //    }

        //    adminSetUp.Id = model.Id;
        //    adminSetUp.IsActive = model.IsActive;

        //    adminSetUpRepository.Entry(adminSetUp).State = adminSetUp.AdminId == 0 ? EntityState.Added : EntityState.Modified;
        //    return adminSetUpRepository.SaveChanges() > 0;
        //}
    }
}
