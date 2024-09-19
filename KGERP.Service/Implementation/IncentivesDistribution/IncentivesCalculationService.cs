using AutoMapper;
using KGERP.Data.Models;
using KGERP.Service.ServiceModel.IncentiveModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KGERP.Service.Implementation.IncentivesDistribution
{
    public class IncentivesCalculationService
    {
        private readonly ERPEntities _db;
        public IncentivesCalculationService(ERPEntities db)
        {
            _db = db;
        }

        public IncentiveCalculationViewModel incentives(int companyid,long CGId)
        {
            double per =0;
            if (companyid == 7)
            {
                per = 1.5;
            }
            if (companyid == 9)
            {
                per = 1.5;
            }
            IncentiveCalculationViewModel model = new IncentiveCalculationViewModel();
            ProductBookingInfo productBookingInfo = _db.ProductBookingInfoes.Where(f => f.CGId == CGId).FirstOrDefault();
            var moneylist = _db.MoneyReceipts.Where(f => f.CGId == CGId && f.IsActive == true && f.IsSubmitted == true).ToList();

            if (productBookingInfo != null)
            {
                model.CGId = productBookingInfo.CGId;
                model.Bookingid = productBookingInfo.BookingId;
                model.Fileno = productBookingInfo.FileNo;
                model.SalePersone = (long)productBookingInfo.SoldBy;
                model.TotalFileValue = productBookingInfo.RestofAmount + productBookingInfo.BookingAmt;
                if (moneylist.Count() > 0)
                {
                    model.TotalCollaction = (decimal)moneylist.Sum(f => f.Amount);
                }
                if (model.TotalCollaction > 0)
                {
                    model.TotalCollactionPercentage = (model.TotalCollaction / model.TotalFileValue) * 100;
                    model.IncentiveAmount = (model.TotalFileValue / 100) * (decimal)per;
                }
                if (model.TotalCollactionPercentage >= 20)
                {
                  
                    model.IsIncentive = true;
                }
            }
            return model;
        }

        public IncentiveCalculationViewModel IncentiveCalculation(IncentiveCalculationViewModel model)
        {
            using (var scope = _db.Database.BeginTransaction())
            {

                try
                {
                    var res = _db.IncentiveDistributionCharts.Where(f => f.CompanyId == model.CompanyId && f.IsActive == true).ToList();
                    var res2 = _db.IncentiveDistributionDetails.Where(f => f.CompanyId == model.CompanyId && f.IsActive == true).ToList();
                   

                    foreach (var item in model.MappVm)
                    {
                        IncentiveProcessMaster master = new IncentiveProcessMaster();
                        List<IncentiveProcessDetail> details = new List<IncentiveProcessDetail>();
                        ProductBookingInfo info = _db.ProductBookingInfoes.FirstOrDefault(f => f.CGId ==item.CGId);
                        master.CGId = item.CGId;
                        master.CollactionPersetage = (double)item.TotalCollactionPercentage;
                        master.IncentiveAmt = item.IncentiveAmount;
                        master.Month = model.IncentiveDate.Month;
                        master.Years = model.IncentiveDate.Year;
                        master.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                        master.CreatedDate = DateTime.Now;
                        master.Active = true;
                        if (model.CompanyId == 7)
                        {
                            master.IncentivePersetage = 1.5;
                        }
                        if (model.CompanyId == 9)
                        {
                            master.IncentivePersetage = 1;
                        }
                        master.CompanyId = model.CompanyId;
                        _db.IncentiveProcessMasters.Add(master);
                        _db.SaveChanges();
                        info.IsIncentive = true;
                        _db.Entry(info).State = EntityState.Modified;
                        _db.SaveChanges();

                        foreach (var vm in res2)
                        {
                            IncentiveProcessDetail detail = new IncentiveProcessDetail();
                            detail.MasterId = master.Id;

                            detail.Commission = vm.PromotionalPercentage;
                            if (vm.IsSalesPersone == true)
                            {
                                detail.EmployeeId = item.SalePersone;
                            }
                            else
                            {
                                detail.EmployeeId = vm.EmployeeId;
                            }
                            detail.Amount = (item.IncentiveAmount / 100) * (decimal)vm.PromotionalPercentage;
                            detail.IncentiveDistributionDetailsid = (int)vm.Id;
                            detail.IsActive = true;
                            detail.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                            detail.CreatedDate = DateTime.Now;
                            details.Add(detail);
                        }
                        _db.IncentiveProcessDetails.AddRange(details);
                        _db.SaveChanges();
                    }
                    scope.Commit();
                    return model;
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    return model;
                }
            }
        }

        public List<IncentiveCalculationViewModel> datalist(int companyId)
        {
            IncentiveCalculationViewModel model = new IncentiveCalculationViewModel();
            model.CompanyId = companyId;
            model.MappVm = (from t1 in _db.IncentiveProcessMasters.Where(f => f.CompanyId == companyId && f.Active == true)
                            join t2 in _db.ProductBookingInfoes on t1.CGId equals t2.CGId
                            select new IncentiveCalculationViewModel
                            {
                                CompanyId = t1.CompanyId,
                                MasterId = t1.Id,
                                CGId = t1.CGId,
                                IncentiveAmount = t1.IncentiveAmt,
                                IncentivePercentage = t1.IncentivePersetage,
                                Fileno = t2.FileNo,
                                Month = t1.Month,
                                Years = t1.Years,
                                RestofAmount = t2.RestofAmount,
                                BookingAmt = t2.BookingAmt,
                            }).ToList();
            return model.MappVm;
        }
        public IncentiveCalculationViewModel IncentiveProcessDetails(long id)
        {
            IncentiveCalculationViewModel model = new IncentiveCalculationViewModel();
            model = (from t1 in _db.IncentiveProcessMasters.Where(f => f.Id == id)
                     join t2 in _db.ProductBookingInfoes on t1.CGId equals t2.CGId
                     select new IncentiveCalculationViewModel
                     {
                         CompanyId = t1.CompanyId,
                         MasterId = t1.Id,
                         CGId = t1.CGId,
                         IncentiveAmount = t1.IncentiveAmt,
                         IncentivePercentage = t1.IncentivePersetage,
                         Fileno = t2.FileNo,
                         Month = t1.Month,
                         Years = t1.Years,
                         RestofAmount = t2.RestofAmount,
                         BookingAmt = t2.BookingAmt,
                         CollactionPercentage = t1.CollactionPersetage,
                     }).FirstOrDefault();
            model.IncentiveDetailsList = (from t1 in _db.IncentiveProcessDetails.Where(f => f.MasterId == id)
                                          join t2 in _db.IncentiveProcessMasters on t1.MasterId equals t2.Id
                                          join t3 in _db.Employees on t1.EmployeeId equals t3.Id
                                          select new IncentiveDetailsViewModel
                                          {
                                              EmployeeName = t3.Name,
                                              EmployeeId = t3.Id,
                                              Amount = t1.Amount,
                                              Commission = t1.Commission
                                          }).ToList();
            model.Totalsum = model.IncentiveDetailsList.Select(d => d.Amount).Sum();
            model.TotalPercentage = model.IncentiveDetailsList.Select(d => d.Commission).Sum();
            return model;
        }


       public bool IsIncentive(int companyId, long[] Areey)
        {

            foreach (var item in Areey)
            {
                var result = _db.ProductBookingInfoes.FirstOrDefault(x => x.CGId == item);

                if (result != null)
                {
                    result.IsIncentive = true;
                }
                else
                {
                    
                    return false;
                }
                _db.SaveChanges();
            }

            return true ;
        }





    }
}
