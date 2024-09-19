using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Service.Implementation
{
    public class BoardOfDirectorService : IBoardOfDirectorService
    {
        private readonly ERPEntities context;
        public BoardOfDirectorService(ERPEntities context)
        {
            this.context = context;
        }
        public async Task<BoardOfDirectorModel> GetBoardOfDirectors(int companyId)
        {
            BoardOfDirectorModel boardOfDirectorModel = new BoardOfDirectorModel();

            boardOfDirectorModel.DataList = await Task.Run(() => (from t1 in context.BoardOfDirectors
                                                                  where t1.CompanyId == companyId
                                                           select new BoardOfDirectorModel
                                                           {
                                                               MemberImage = string.IsNullOrEmpty(t1.MemberImage) ? "default.png" : t1.MemberImage,                                       
                                                               MemberName  = t1.MemberName,
                                                               Phone= t1.Phone,
                                                               Email=t1.Email,
                                                               BoardOfDirectorId= t1.BoardOfDirectorId,
                                                               PresentAddress= t1.PresentAddress,
                                                               IsActive=t1.IsActive,
                                                               MemberOrder= t1.MemberOrder,
                                                              
                                                            
                                                           }).OrderBy(o => o.MemberOrder).AsEnumerable());
            return boardOfDirectorModel;
        }
      
        public List<BoardOfDirectorModel> GetAllBoardOfDirectors(string searchText)
        {
            IQueryable<BoardOfDirectorModel> boardOfDirectors = context.Database.SqlQuery<BoardOfDirectorModel>(@"select   BoardOfDirectorId,
                                                                                                                           (select ShortName from Company where CompanyId=BoardOfDirector.CompanyId) as CompanyName,
                                                                                                      		               MemberImage,
                                                                                                      		               MemberName,MemberOrder,StartDate,Phone,Email,EndDate,IsActive 
                                                                                                                  from     BoardOfDirector").AsQueryable();
            boardOfDirectors = boardOfDirectors.Where(x => (x.CompanyName.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText)) ||
                                                           (x.MemberName.ToLower().Contains(searchText.ToLower()) || string.IsNullOrEmpty(searchText)));
            return boardOfDirectors.ToList();
        }


        public BoardOfDirectorModel GetBoardOfDirector(int id)
        {
            if (id == 0)
            {
                return new BoardOfDirectorModel()
                {
                    IsActive = true,
                    MemberImage = "default.png",
                    CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"])
                };
            }
            BoardOfDirector boardOfDirector = context.BoardOfDirectors.Find(id);
            return ObjectConverter<BoardOfDirector, BoardOfDirectorModel>.Convert(boardOfDirector);
        }

        public bool SaveBoardOfDirector(int id, BoardOfDirectorModel model)
        {
            if (model == null)
            {
                throw new Exception("Member data missing!");
            }

            bool exist = context.BoardOfDirectors.Where(x => x.CompanyId == model.CompanyId && x.MemberName.ToLower().Equals(model.MemberName.ToLower()) && x.BoardOfDirectorId != id).Any();

            if (exist)
            {
                throw new Exception("Member already exist!");
            }
            BoardOfDirector boardOfDirector = ObjectConverter<BoardOfDirectorModel, BoardOfDirector>.Convert(model);
            if (id > 0)
            {
                boardOfDirector = context.BoardOfDirectors.FirstOrDefault(x => x.BoardOfDirectorId == id);
                if (boardOfDirector == null)
                {
                    throw new Exception("Member not found!");
                }


                boardOfDirector.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                boardOfDirector.ModifiedDate = DateTime.Now;
            }

            else
            {
                boardOfDirector.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                boardOfDirector.CreatedDate = DateTime.Now;
            }

            if (!string.IsNullOrEmpty(model.MemberImage))
            {
                boardOfDirector.MemberImage = model.MemberImage;
            }

            boardOfDirector.CompanyId = model.CompanyId;
            boardOfDirector.MemberName = model.MemberName;
            boardOfDirector.MemberOrder = model.MemberOrder;
            boardOfDirector.StartDate = model.StartDate;
            boardOfDirector.EndDate = model.EndDate;
            boardOfDirector.Remarks = model.Remarks;
            boardOfDirector.DateOfBirth = model.DateOfBirth;
            boardOfDirector.NID = model.NID;
            boardOfDirector.Phone = model.Phone;
            boardOfDirector.HomePhone = model.HomePhone;
            boardOfDirector.OfficePhone = model.OfficePhone;
            boardOfDirector.Email = model.Email;
            boardOfDirector.PresentAddress = model.PresentAddress;
            boardOfDirector.PermanentAdress = model.PermanentAdress;
            boardOfDirector.FatherName = model.FatherName;
            boardOfDirector.MotherName = model.MotherName;
            boardOfDirector.Spouse = model.Spouse;
            boardOfDirector.EducationQualificationId = model.EducationQualificationId;
            boardOfDirector.ProfessionId = model.ProfessionId;
            boardOfDirector.Organization = model.Organization;
            boardOfDirector.Designation = model.Designation;


            boardOfDirector.IsActive = model.IsActive;

            context.Entry(boardOfDirector).State = boardOfDirector.BoardOfDirectorId == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        public bool DeleteDistrict(int id)
        {
            District district = context.Districts.Find(id);
            if (district == null)
            {
                throw new Exception("District not found");
            }
            context.Districts.Remove(district);
            return context.SaveChanges() > 0;
        }

        public List<SelectModel> GetCountriesSelectModels()
        {
            return context.Countries.ToList().Select(x => new SelectModel()
            {
                Text = x.CountryName,
                Value = x.CountryId
            }).OrderBy(x => x.Text).ToList();
        }

        public bool DeleteBoardOfDirector(int id)
        {

            BoardOfDirector boardOfDirector = context.BoardOfDirectors.Find(id);
            if (boardOfDirector == null)
            {
                throw new Exception("Member not found");
            }
            context.BoardOfDirectors.Remove(boardOfDirector);
            return context.SaveChanges() > 0;
        }

        public bool BulkSave(List<BoardOfDirectorModel> models)
        {
            if (!models.Any())
            {
                return false;
            }
            List<BoardOfDirector> boardOfDirectors = ObjectConverter<BoardOfDirectorModel, BoardOfDirector>.ConvertList(models.ToList()).ToList();
            context.BoardOfDirectors.AddRange(boardOfDirectors);
            return context.SaveChanges() > 0;
        }
    }
}
