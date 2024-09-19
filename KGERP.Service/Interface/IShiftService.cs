using KGERP.Data.Models;
using KGERP.Utility;
using System.Collections.Generic;

namespace KGERP.Service.Interface
{
    public interface IShiftService
    {
        List<Shift> GetShifts();
        List<SelectModel> GetShiftSelectModels();
    }
}
