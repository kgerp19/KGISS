using KGERP.Service.ServiceModel;
using System.Collections.Generic;


namespace KGERP.Service.Interface
{
    public interface IMonthlyTargetService
    {
        List<MonthlyTargetModel> GetMonthlyTargets(string searchText);
        List<MonthlyTargetModel> GetMonthlyCompanyTargets(string searchText, int companyId);
        MonthlyTargetModel GetMonthlyTarget(int id);
        bool SaveMonthlyTarget(int id, MonthlyTargetModel model, out string message);
        bool DeleteMonthlyTarget(int monthlyTargetId);
        //bool MonthlyTargetUpazila(int id);
        //List<SelectModel> GetUpazilaSelectModelsByDistrict(int districtId);
        //string GetUpazilaCodeByDistrict(int districtId);
        //List<SelectModel> GetUpzilaSelectModels();
        //List<SelectModel> GetUpzilaByDistrictName(string name);
    }
}
