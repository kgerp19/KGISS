using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class UnitService : IUnitService
    {
        private readonly ERPEntities context;
        public UnitService(ERPEntities context)
        {
            this.context = context;
        }
        public List<UnitModel> GetUnits()
        {
            List<Unit> units = context.Units.ToList();
            return ObjectConverter<Unit, UnitModel>.ConvertList(units.ToList()).ToList();
        }

        public List<SelectModel> GetUnitSelectModels(int companyId)
        {
            return context.Units.ToList().Where(x => x.CompanyId == companyId).Select(x => new SelectModel()
            {
                Text = x.ShortName,
                Value = x.UnitId
            }).OrderBy(x => x.Text).ToList();
        }
    }
}
