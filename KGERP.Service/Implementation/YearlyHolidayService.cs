using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class YearlyHolidayService : IYearlyHoliday
    {
        ERPEntities _context = new ERPEntities();


        public async Task<YearlyHolidayModel> GetYearlyHolidayEvent()
        {
            YearlyHolidayModel model = new YearlyHolidayModel();

            //model.DataList = await Task.Run(() => _context.YearlyHolidays
            //.Where(q => q.HolidayDate.Year == DateTime.Now.Year).Select(s => new YearlyHolidayModel
            //{
            //    YearlyHolidayId = s.YearlyHolidayId,
            //    HolidayDate = s.HolidayDate,
            //    HolidayCategory = s.HolidayCategory,
            //    Purpose = s.Purpose
            //}).AsEnumerable());
            model.DataList = await _context.YearlyHolidays
                    .Where(q => q.HolidayDate.Year == DateTime.Now.Year)
                    .Select(s => new YearlyHolidayModel
                    {
                        YearlyHolidayId = s.YearlyHolidayId,
                        HolidayDate = s.HolidayDate,
                        HolidayCategory = s.HolidayCategory,
                        Purpose = s.Purpose
                    })
                    .OrderBy(x => x.HolidayDate)
                    .AsNoTracking()
                    .ToListAsync();

            var today = DateTime.Today;

            var closestData = model.DataList.Where(x=> x.HolidayDate > today).FirstOrDefault();
            if (closestData != null)
            {
               closestData.IsUpcoming = true;
            }

            return model;
        }
        public List<YearlyHolidayModel> GetYearlyHolidays(string searchText)
        {
            return ObjectConverter<YearlyHoliday, YearlyHolidayModel>.ConvertList(_context.YearlyHolidays.Where(x => x.HolidayDate.Year == DateTime.Now.Year || x.HolidayCategory.Contains(searchText) || x.Purpose.Contains(searchText)).OrderByDescending(x => x.HolidayDate).ToList()).ToList();

        }

        //hhhhhhhhhhhhhh  boss try abar
        public YearlyHolidayModel GetYearlyHoliday(int id)
        {
            if (id == 0)
            {
                return new YearlyHolidayModel();
            }
            return ObjectConverter<YearlyHoliday, YearlyHolidayModel>.Convert(_context.YearlyHolidays.FirstOrDefault(x => x.YearlyHolidayId == id));
        }

        public bool SaveYearlyHoliday(int id, YearlyHolidayModel model)
        {
            if (model == null)
            {
                throw new Exception("Holiday data missing");
            }
            YearlyHoliday yearlyHoliday = ObjectConverter<YearlyHolidayModel, YearlyHoliday>.Convert(model);


            if (id > 0)
            {
                yearlyHoliday = _context.YearlyHolidays.FirstOrDefault(x => x.YearlyHolidayId == id);
                if (yearlyHoliday == null)
                {
                    throw new Exception("Holiday Data not found!");
                }

                //yearlyHoliday.ModifiedDate = DateTime.Now;
                //yearlyHoliday.ModifedBy = "";
                //employeeRepository.Entry(employee).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                yearlyHoliday.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                yearlyHoliday.CreatedDate = DateTime.Now;
            }

            yearlyHoliday.HolidayDate = model.HolidayDate;
            yearlyHoliday.HolidayCategory = model.HolidayCategory;
            yearlyHoliday.Purpose = model.Purpose;

            _context.Entry(yearlyHoliday).State = yearlyHoliday.YearlyHolidayId == 0 ? EntityState.Added : EntityState.Modified;
            return _context.SaveChanges() > 0;
        }



        //bool IYearlyHolidayService.DeleteYearlyHoliday(int id)
        //{
        //    YearlyHoliday yearlyHoliday = _context.YearlyHolidays.FirstOrDefault(x => x.YearlyHolidayId == id);
        //    _context.YearlyHolidays.Remove(yearlyHoliday);
        //    return _context.SaveChanges() > 0;
        //}


    }
}
