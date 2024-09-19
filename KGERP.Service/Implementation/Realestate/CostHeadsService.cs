using KGERP.Data.Models;
using KGERP.Service.Implementation.Realestate;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class CostHeadsService : ICostHeadsService
    {
        private readonly ERPEntities context = new ERPEntities();

        public async Task<List<SelectModelInstallmentType>> GetBookingInstallmentType()
        {
            List<SelectModelInstallmentType> selectModelLiat = new List<SelectModelInstallmentType>();
            var v = await context.BookingInstallmentTypes.Where(e => e.IsActive == true).Select(x => new SelectModelInstallmentType()
            {
                Text = x.Name,
                Value = x.InstallmentTypeId,
                IsOneTime = x.IsOneTime,
                IntervalMonths = x.IntervalMonths
            }).ToListAsync();
            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }

        public async Task<List<SelectModelInstallmentType>> GetCompanyBookingInstallmentType(int companyId)
        {
            List<SelectModelInstallmentType> selectModelLiat = new List<SelectModelInstallmentType>();
            var v = await context.BookingInstallmentTypes.Where(e => e.IsActive == true && e.CompanyId == companyId).Select(x => new SelectModelInstallmentType()
            {
                Text = x.Name,
                Value = x.InstallmentTypeId,
                IsOneTime = x.IsOneTime,
                IntervalMonths = x.IntervalMonths
            }).ToListAsync();
            selectModelLiat.AddRange(v);
            return selectModelLiat;
        }

        public async Task<List<BookingHeadServiceModel>> GetCostHeadsByCompanyId(int companyId)
        {
            return await context.BookingCostHeads.Include(j => j.Company).Where(e => e.IsActive == true && e.CompanyId == companyId).Select(o => new BookingHeadServiceModel
            {
                companyName = o.Company.Name,
                CompanyId = o.CompanyId,
                CostId = o.CostId,
                CostName = o.CostName,
                Amount = 0m,
                IsSnstallmentInclude = false,
                Percentage = 0m
            }).ToListAsync();
        }

        public async Task<BookingCostHead> GetCostHeadsById(int id)
        {
            return await context.BookingCostHeads.Where(e => e.IsActive == true && e.CostId == id).SingleOrDefaultAsync();
        }

        public async Task<bool> AddCostHeads(BookingHeadInsertModel model)
        {
            int result = -1;
            BookingCostHead data = new BookingCostHead()
            {
                CostId = 0,
                CostName = model.CostName,
                CompanyId = model.CompanyId,
                IsActive = true,
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now
            };
            try
            {
                context.BookingCostHeads.Add(data);
                result = await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
            return result > 0 ? true : false;
        }

        public async Task<bool> UpdateCostHeads(BookingHeadEditModel model)
        {
            int result = -1;
            BookingCostHead data = await context.BookingCostHeads.SingleAsync(o => o.CostId == model.CostId);
            if (data != null)
            {
                data.CostName = model.CostName;
                data.CompanyId = model.CompanyId;
                data.ModifiedBy = model.ModifiedBy;
                data.ModifiedDate = DateTime.Now;

                try
                {
                    result = await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return result > 0 ? true : false;
        }
        public async Task<bool> DeleteCostHeads(int id)
        {
            int result = -1;
            BookingCostHead data = await context.BookingCostHeads.SingleAsync(o => o.CostId == id);
            if (data != null)
            {
                data.IsActive = false;

                try
                {
                    result = await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return result > 0 ? true : false;
        }

        public List<SelectDDLModel> CostHeadsList(int companyId)
        {
            return context.BookingCostHeads.Where(f=>f.CompanyId== companyId && f.IsActive==true).ToList().Select(x => new SelectDDLModel()
            {
                Text = x.CostName,
                Value = x.CostId
            }).ToList();
        }

        //public List<SelectModel> GetPaymentModeSelectModels()
        //{
        //    return context.PaymentModes.ToList().Select(x => new SelectModel()
        //    {
        //        Text = x.Name,
        //        Value = x.PaymentModeId
        //    }).ToList();
        //}

        //public List<SelectModel> GetPaymentReceiveSelectModels()
        //{
        //    return context.PaymentModes.Where(x => x.PaymentModeId == 2 || x.PaymentModeId == 4 || x.PaymentModeId == 8 || x.PaymentModeId == 9).ToList().Select(x => new SelectModel()
        //    {
        //        Text = x.Name,
        //        Value = x.PaymentModeId
        //    }).ToList();
        //}

        //public List<SelectModel> PaymentModes()
        //{
        //    return context.PaymentModes.Where(x => x.PaymentModeId == 2 || x.PaymentModeId == 4 || x.PaymentModeId == 10).ToList().Select(x => new SelectModel()
        //    {
        //        Text = x.Name,
        //        Value = x.PaymentModeId
        //    }).ToList();
        //}
    }
}
