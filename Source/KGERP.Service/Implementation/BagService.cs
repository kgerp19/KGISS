using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class BagService : IBagService
    {
        private bool disposed = false;
        private readonly ERPEntities context;
        public BagService(ERPEntities context)
        {
            this.context = context;
        }
      
        public async Task<BagModel> GetBags(int companyId)
        {
            BagModel bagModel = new BagModel();
            bagModel.CompanyId = companyId;
            bagModel.DataList = await Task.Run(() => (from t1 in context.Bags                      
                                                                where t1.CompanyId == companyId       
                                                                select new BagModel
                                                                {
                                                                    BagName = t1.BagName,
                                                                    BagId = t1.BagId,
                                                                    BagSize = t1.BagSize ??0 ,
                                                                    BagValue = t1.BagValue
                                                                }).OrderBy(o => o.BagName).AsEnumerable());
            return bagModel;
        }
        public List<SelectModel> GetBagWeightSelectModels(int companyId)
        {
            List<Bag> bags = context.Bags.Where(x => x.CompanyId == companyId).ToList();
            return bags.Select(x => new SelectModel { Text = x.BagName, Value = x.BagId }).OrderByDescending(x => x.Value).ToList();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public BagModel GetBag(int id)
        {
            if (id == 0)
            {
                return new BagModel();
            }
            Bag bag = context.Bags.Find(id);
            return ObjectConverter<Bag, BagModel>.Convert(bag);
        }

        public decimal GetBagWeightByBagId(int bagId)
        {
            Bag bag = context.Bags.Where(x => x.BagId == bagId).FirstOrDefault();
            if (bag == null)
            {
                return 0;
            }
            return bag.BagValue;
        }
    }
}
