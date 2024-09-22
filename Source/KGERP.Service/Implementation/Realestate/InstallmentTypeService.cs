using KGERP.Data.Models;
using KGERP.Service.Implementation.Realestate;
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
    public class InstallmentTypeService : IInstallmentTypeService
    {
        private readonly ERPEntities context = new ERPEntities();

        public async Task<List<BookingInstallmentType>> GetAllInstallmentTypesByCompanyId(int companyId)
        {
            return await context.BookingInstallmentTypes.Where(e => e.IsActive == true && e.CompanyId == companyId)
                .ToListAsync();
        }
        public async Task<BookingInstallmentType> GetInstallmentTypeById(int id)
        {
            return await context.BookingInstallmentTypes.Where(e => e.IsActive == true && e.InstallmentTypeId == id)
               .SingleOrDefaultAsync();
        }

        public async Task<bool> AddInstallmentType(InstallmentTypeInsertModel model)
        {
            int result = -1;
            BookingInstallmentType data = new BookingInstallmentType()
            {
                Name = model.Name,
                IsActive = true,
                CreatedBy = model.CreatedBy,
                CreatedDate = DateTime.Now,
                InstallmentTypeId = 0,
                IsOneTime = model.IsOneTime,
                CompanyId = model.CompanyId,
                IntervalMonths = model.IntervalMonths
            };
            try
            {
                context.BookingInstallmentTypes.Add(data);
                result = await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
            return result > 0 ? true : false;
        }

        public async Task<bool> UpdateInstallmentType(InstallmentTypeEditModel model)
        {
            int result = -1;
            BookingInstallmentType data = await context.BookingInstallmentTypes.SingleAsync(o => o.InstallmentTypeId == model.InstallmentTypeId);
            if (data != null)
            {
                data.ModifiedBy = model.ModifiedBy;
                data.ModifiedDate = DateTime.Now;
                data.Name = model.Name;
                data.IsOneTime = model.IsOneTime;
                data.IntervalMonths = model.IntervalMonths;
                data.CompanyId = model.CompanyId;

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
        public async Task<bool> DeleteInstallmentType(int id)
        {
            int result = -1;
            BookingInstallmentType data = await context.BookingInstallmentTypes.SingleAsync(o => o.InstallmentTypeId == id);
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



    }
}
