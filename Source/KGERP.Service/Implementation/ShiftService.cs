using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Utility;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class ShiftService : IShiftService
    {
        ERPEntities shiftRepository = new ERPEntities();
        public List<Shift> GetShifts()
        {
            return shiftRepository.Shifts.ToList();
        }

        public List<SelectModel> GetShiftSelectModels(int CompanyId)
        {
            return shiftRepository.Shifts.Where(x=>x.IsActive && (x.CompanyId==null || x.CompanyId==CompanyId)).ToList().Select(x => new SelectModel()
            {
                Text = x.Name+ "("+x.StartAt+"-"+x.EndAt+")",
                Value = x.ShiftId.ToString()
            }).ToList();
        }
    }
}
