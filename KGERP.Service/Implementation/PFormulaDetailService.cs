using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class PFormulaDetailService : IPFormulaDetailService
    {
        private readonly ERPEntities context;
        public PFormulaDetailService(ERPEntities context)
        {
            this.context = context;
        }

        public bool DeletePFormulaDetail(int pFormulaDetailId)
        {
            PFormulaDetail formulaDetail = context.PFormulaDetails.Find(pFormulaDetailId);
            if (formulaDetail == null)
            {
                throw new Exception("Data not found");
            }
            context.PFormulaDetails.Remove(formulaDetail);
            return context.SaveChanges() > 0;
        }

        public PFormulaDetailModel GetFormulaDetail(int pFormulaDetailId)
        {
            if (pFormulaDetailId == 0)
            {
                return new PFormulaDetailModel();
            }
            PFormulaDetail formulaDetail = context.PFormulaDetails.Find(pFormulaDetailId);
            return ObjectConverter<PFormulaDetail, PFormulaDetailModel>.Convert(formulaDetail);
        }

        public List<PFormulaDetailModel> GetFormulaDetails(int productFormulaId)
        {
            IQueryable<PFormulaDetail> queryable = context.PFormulaDetails.Include(x => x.Product).Where(x => x.ProductFormulaId == productFormulaId).OrderByDescending(x => x.RProcessLoss).ThenByDescending(x => x.RQty);
            return ObjectConverter<PFormulaDetail, PFormulaDetailModel>.ConvertList(queryable.ToList()).ToList();
        }
    }
}
