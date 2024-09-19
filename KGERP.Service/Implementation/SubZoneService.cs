using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class SubZoneService : ISubZoneService
    {
        private readonly ERPEntities context;
        public SubZoneService(ERPEntities context)
        {
            this.context = context;
        }

        public decimal GetCarryingRate(int? customerId)
        {
            throw new NotImplementedException();
        }

        public List<SelectModel> GetSubZoneSelectModelsByZone(int? zoneId)
        {
            return context.Head5.Where(x => x.ParentId == zoneId).ToList().Select(x => new SelectModel()
            {
                Text = "[" + x.AccCode.ToString() + "] " + x.AccName.ToString(),
                Value = x.Id
            }).OrderBy(x => x.Text).ToList();
        }
    }
}
