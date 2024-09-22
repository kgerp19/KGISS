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

        public List<SelectModel> GetShiftSelectModels()
        {
            return shiftRepository.Shifts.ToList().Select(x => new SelectModel()
            {
                Text = x.Name.ToString(),
                Value = x.ShiftId.ToString()
            }).ToList();
        }
    }
}
