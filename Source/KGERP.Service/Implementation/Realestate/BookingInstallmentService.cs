using Humanizer;
using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.Realestate
{
    public class BookingInstallmentService : IBookingInstallmentService
    {
        private readonly ERPEntities context = new ERPEntities();

        public async Task<GeneratedInstallmentSchedule> GenerateInstallmentSchedule(int companyId, int installmentTypeId, int NoOfInstallment, decimal PayableAmount, DateTime BookingDate)
        {
            string stringdata= "";
            if (companyId==7)
            {
                stringdata = "Plot Payment";
            }
            else
            {
                stringdata = "Flat Payment";
            }

           
            GeneratedInstallmentSchedule model = new GeneratedInstallmentSchedule();
            var InstallmentTypeInfo = await context.BookingInstallmentTypes.SingleAsync(e => e.IsActive && e.InstallmentTypeId == installmentTypeId && e.CompanyId == companyId);
            if (InstallmentTypeInfo != null)
            {
                model.IsGenerated = true;
        
                if (InstallmentTypeInfo.IsOneTime)
                {
                    model.LstSchedules.Add(new InstallmentScheduleShortModel
                    {
                        InstallmentDate = BookingDate,
                        InstallmentId= installmentTypeId,
                        InstallmentName= InstallmentTypeInfo.Name,
                        StringDate =BookingDate.ToString("dd-MMM-yyyy"),
                        SortOrder = 1,
                        Title = stringdata,
                        PayableAmount = PayableAmount
                    });
                }
                else
                {
                    var AmountPerInstallment = PayableAmount / NoOfInstallment;
                    for (int i = 0; i < NoOfInstallment; i++)
                    {
                        int temp = i + 1;
                        var months = (int)InstallmentTypeInfo.IntervalMonths * temp;
                        months = months - (int)InstallmentTypeInfo.IntervalMonths;
                        var installmentDate = BookingDate.AddMonths(months);
                        model.LstSchedules.Add(new InstallmentScheduleShortModel
                        {
                            InstallmentDate = BookingDate.AddMonths(months),
                            StringDate = BookingDate.AddMonths(months).ToString("dd-MMM-yyyy"),
                            SortOrder = temp,
                            //Title = $"{temp.ToOrdinalWords()} Installment",
                            Title = $"{temp.ToOrdinalWords()} Ins({installmentDate:MMM yyyy})",
                            InstallmentId = installmentTypeId,
                            InstallmentName = InstallmentTypeInfo.Name,
                            PayableAmount = AmountPerInstallment
                        });
                    }
                }
            }
            return model;
        } 
        public async Task<GeneratedInstallmentSchedule> GolobalInstallmentSchedule(int companyId, int installmentTypeId, int NoOfInstallment, decimal PayableAmount, DateTime BookingDate)
        {
            GeneratedInstallmentSchedule model = new GeneratedInstallmentSchedule();
            var InstallmentTypeInfo = await context.BookingInstallmentTypes.FirstOrDefaultAsync(e => e.IsActive && e.InstallmentTypeId == installmentTypeId && e.CompanyId == 0);
            if (InstallmentTypeInfo != null)
            {
                model.IsGenerated = true;
                if (InstallmentTypeInfo.IsOneTime)
                {
                    model.LstSchedules.Add(new InstallmentScheduleShortModel
                    {
                        InstallmentDate = BookingDate,
                        InstallmentId= installmentTypeId,
                        InstallmentName= InstallmentTypeInfo.Name,
                        StringDate =BookingDate.ToString("dd-MMM-yyyy"),
                        SortOrder = 1,
                        Title = "Full n Final Payment",
                        PayableAmount = PayableAmount
                    });
                }
                else
                {
                    var AmountPerInstallment = PayableAmount / NoOfInstallment;
                    for (int i = 0; i < NoOfInstallment; i++)
                    {
                        int temp = i + 1;
                        var months = (int)InstallmentTypeInfo.IntervalMonths * temp;
                        months = months - (int)InstallmentTypeInfo.IntervalMonths;
                        model.LstSchedules.Add(new InstallmentScheduleShortModel
                        {
                            InstallmentDate = BookingDate.AddMonths(months),
                            StringDate = BookingDate.AddMonths(months).ToString("dd-MMM-yyyy"),
                            SortOrder = temp,
                            Title = $"{temp.ToOrdinalWords()} Installment",
                            InstallmentId = installmentTypeId,
                            InstallmentName = InstallmentTypeInfo.Name,
                            PayableAmount = AmountPerInstallment
                        });
                    }
                }
            }
            return model;
        }
        public async Task<bool> SaveInstallmentSchedule(long CGId, long BookingId, List<InstallmentScheduleShortModel> List)
        {
            List<BookingInstallmentSchedule> schedules = new List<BookingInstallmentSchedule>();
            try
            {
                foreach (var item in List)
                {
                    schedules.Add(this.ConvertShortToDbModel(CGId, BookingId, item));
                }
                context.BookingInstallmentSchedules.AddRange(schedules);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }

        private BookingInstallmentSchedule ConvertShortToDbModel(long CGId, long BookingId, InstallmentScheduleShortModel sm)
        {
            BookingInstallmentSchedule m = new BookingInstallmentSchedule()
            {
                Amount = sm.PayableAmount,
                BookingId = BookingId,
                CGID = CGId,
                CreatedBy = "",
                CreatedDate = DateTime.Now,
                Date = sm.InstallmentDate,
                InstallmentId = 0,
                IsActive = true,
                IsLate = false,
                IsPaid = sm.PaidAmount > 0 ? true : false,
                IsPartlyPaid = sm.PayableAmount > sm.PaidAmount ? true : false,
                PaidAmount = sm.PaidAmount,
                Remarks = "",
                InstallmentTitle = sm.Title
            };
            return m;
        }
    }
}
