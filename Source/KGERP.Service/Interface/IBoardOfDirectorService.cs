using KGERP.Service.ServiceModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGERP.Service.Interface
{
    public interface IBoardOfDirectorService
    {
        Task<BoardOfDirectorModel> GetBoardOfDirectors(int companyId);
        BoardOfDirectorModel GetBoardOfDirector(int id);
        bool SaveBoardOfDirector(int id, BoardOfDirectorModel model);
        List<BoardOfDirectorModel> GetAllBoardOfDirectors(string searchText);
        bool DeleteBoardOfDirector(int id);
        bool BulkSave(List<BoardOfDirectorModel> boardOfDirectors);

        //List<SelectModel> GetDistrictSelectModels();
        //List<SelectModel> GetCountriesSelectModels();
        //bool DeleteDistrict(int id);
    }
}
