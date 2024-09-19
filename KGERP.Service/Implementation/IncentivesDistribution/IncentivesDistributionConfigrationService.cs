using KGERP.Data.CustomModel;
using KGERP.Data.Models;
using KGERP.Service.Implementation.Configuration;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.IncentivesDistribution
{
    public class IncentivesDistributionConfigrationService : IIncentiveConfig
    {
        ERPEntities context = new ERPEntities();

        public IncentiveViewModel AddChart(IncentiveViewModel model)
        {
            IncentiveDistributionChart chart = new IncentiveDistributionChart();
            chart.IncentiveCatagoryId = model.IncentiveCatagoryId;
            chart.Commission = model.Commission;
            chart.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            chart.CreatedDate = DateTime.Now;
            chart.IsActive = true;
            chart.CompanyId = model.CompanyId;
            context.IncentiveDistributionCharts.Add(chart);
            var res = context.SaveChanges();
            if (res > 0)
            {
                model.Id = chart.Id;
                return model;
            }
            return model;
        }

        public List<SelectModelType> Catagorieslist(int companyId)
        {
            var List = new List<SelectModelType>();
            context.IncentiveCatagories
         .Where(f => f.Active == true && f.CompanyId == companyId).Select(x => x).ToList()
        .ForEach(x => List.Add(new SelectModelType
        {
            Value = x.Id,
            Text = x.Name
        }));
            return List;
        }

        public IncentiveViewModel ChartList(int companyId)
        {
            IncentiveViewModel model = new IncentiveViewModel();
            model.CompanyId = companyId;
            model.datalist = (from t1 in context.IncentiveDistributionCharts
                              join t2 in context.IncentiveCatagories on t1.IncentiveCatagoryId equals t2.Id
                              where t1.CompanyId == companyId && t1.IsActive == true
                              select new IncentiveViewModel
                              {
                                  IncentiveCatagoryName = t2.Name,
                                  IncentiveCatagoryId = t1.IncentiveCatagoryId,
                                  Id = t1.Id,
                                  CompanyId = t1.CompanyId,
                                  Commission = t1.Commission
                              }).ToList();
            model.TotalCommission = model.datalist.Select(d => d.Commission).Sum();
            return model;
        }
        public IncentiveDistributionChart GetCatagory(int id)
        {
            var res = context.IncentiveDistributionCharts.Where(f => f.IncentiveCatagoryId == id && f.IsActive == true).FirstOrDefault();
            return res;
        }

        public double GetSum(int companyId, int id)
        {
            double sum = context.IncentiveDistributionCharts.Where(f => f.IsActive == true && f.CompanyId == companyId && f.Id != id).Sum(f => f.Commission) ?? 0;
            return sum;
        }

        public bool UpdateChart(IncentiveViewModel model)
        {
            var res = context.IncentiveDistributionCharts.Where(f => f.Id == model.Id).FirstOrDefault();
            res.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            res.ModifiedDate = DateTime.Now;
            res.IncentiveCatagoryId = model.IncentiveCatagoryId;
            res.Commission = model.Commission;
            context.Entry(res).State = EntityState.Modified;
            var result = context.SaveChanges();
            if (result > 0)
            {
                return true;
            }
            return false;
        }
        public bool Delete(int id)
        {
            var res = context.IncentiveDistributionCharts.Where(f => f.Id == id).FirstOrDefault();
            res.IsActive = false;
            res.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            res.ModifiedDate = DateTime.Now;
            context.Entry(res).State = EntityState.Modified;
            var result = context.SaveChanges();
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public IncentiveDistributionChart GetBy(int id)
        {
            var res = context.IncentiveDistributionCharts.Where(f => f.Id == id).FirstOrDefault();
            return res;
        }

        public IncentiveDistributionChart Getexit(IncentiveViewModel model)
        {
            var res = context.IncentiveDistributionCharts.Where(f => f.IncentiveCatagoryId == model.IncentiveCatagoryId && f.Id != model.Id && f.IsActive == true).FirstOrDefault();
            return res;
        }

        public IncentiveDistributionDetailVm GetList(int id)
        {
            IncentiveDistributionDetailVm m = new IncentiveDistributionDetailVm();
            m.datalist = (from t1 in context.IncentiveDistributionDetails
                          join t2 in context.IncentiveDistributionCharts on t1.IncentiveDistributionChartId equals t2.Id
                          join t3 in context.Employees on t1.EmployeeId equals t3.Id
                          join t4 in context.IncentiveCatagories on t2.IncentiveCatagoryId equals t4.Id
                          where t1.IncentiveDistributionChartId == id && t1.IsActive == true
                          select new IncentiveDistributionDetailVm
                          {
                              Id = t1.Id,
                              EmployeeName = t3.Name,
                              commition = t2.Commission,
                              PromotionalPercentage = t1.PromotionalPercentage,
                              catagoryname = t4.Name,
                              CompanyId = t2.CompanyId,
                          }).ToList();

            m.commition = m.datalist.FirstOrDefault().commition;
            m.catagoryname = m.datalist.Select(x => x.catagoryname).FirstOrDefault();
            m.IncentiveDistributionChartId = id;
            m.CompanyId = m.datalist.Select(x => x.CompanyId).FirstOrDefault();

            return m;
        }


        public IncentiveDistributionDetailVm GetListForAddNewIncentive(int id, int companyId)
        {
            IncentiveDistributionDetailVm m = new IncentiveDistributionDetailVm();
            m = (from t1 in context.IncentiveDistributionDetails
                 join t2 in context.IncentiveDistributionCharts on t1.IncentiveDistributionChartId equals t2.Id
                 join t3 in context.Employees on t1.EmployeeId equals t3.Id
                 join t4 in context.IncentiveCatagories on t2.IncentiveCatagoryId equals t4.Id
                 where t1.IncentiveDistributionChartId == id && t1.IsActive == true
                 select new IncentiveDistributionDetailVm
                 {
                     Id = t1.Id,
                     commition = t2.Commission,
                     PromotionalPercentage = t1.PromotionalPercentage,
                     catagoryname = t4.Name
                 }).FirstOrDefault();



            m.SelectItem = (from t1 in context.Employees
                            where t1.CompanyId == companyId
                            select new SelectVm
                            {
                                Name = t1.Name,
                                empid = t1.Id

                            }).ToList();




            return m;
        }


        public bool objtosave(IncentiveDistributionDetailVm model)
        {
            var obj = context.IncentiveDistributionDetails.Where(x => x.IncentiveDistributionChartId == model.IncentiveDistributionChartId && x.IsActive).Select(x => x.PromotionalPercentage).Sum();

            var totalincentive = obj + model.PromotionalPercentage;
            if (totalincentive <= model.commition)
            {

                IncentiveDistributionDetail vm = new IncentiveDistributionDetail()
                {

                    IncentiveDistributionChartId = model.IncentiveDistributionChartId,
                    PromotionalPercentage = model.PromotionalPercentage,
                    EmployeeId = model.EmployeeId,
                    CompanyId = model.CompanyId,
                    IsSalesPersone = false,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    IsActive = true

                };


                context.IncentiveDistributionDetails.Add(vm);
                var result = context.SaveChanges();

                if (result > 0)
                {
                    return true;
                }

            }



            return false;
        }

        public IncentiveDistributionDetailVm GetorEditIncentive(long incentiveId)
        {
            IncentiveDistributionDetailVm vm = new IncentiveDistributionDetailVm();

            vm = (from t1 in context.IncentiveDistributionDetails
                  join t2 in context.Employees on t1.EmployeeId equals t2.Id
                  where t1.Id == incentiveId && t1.IsActive == true
                  select new IncentiveDistributionDetailVm
                  {
                      Id = t1.Id,
                      EmployeeId = t1.EmployeeId,
                      EmployeeName = t2.Name,
                      PromotionalPercentage = t1.PromotionalPercentage,
                      IncentiveDistributionChartId = t1.IncentiveDistributionChartId
                  }).FirstOrDefault();



            return vm;
        }


        public bool UpdateIncentive(IncentiveDistributionDetail model)
        {
            var res = context.IncentiveDistributionDetails.FirstOrDefault(x => x.Id == model.Id && x.IsActive);
            var inceentivechertSum = context.IncentiveDistributionDetails.Where(x => x.IncentiveDistributionChartId == model.IncentiveDistributionChartId && x.Id != model.Id && x.IsActive).Select(y => y.PromotionalPercentage).Sum();
            var totalincentivetotal = model.PromotionalPercentage + inceentivechertSum;
            var totalincentivetotal2 = context.IncentiveDistributionCharts.FirstOrDefault(x => x.Id == model.IncentiveDistributionChartId && x.IsActive == true);


            if (totalincentivetotal <= totalincentivetotal2.Commission)
            {
                res.PromotionalPercentage = model.PromotionalPercentage;
                res.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                res.ModifiedDate = DateTime.Now;
                context.Entry(res).State = EntityState.Modified;
                var result = context.SaveChanges();
                if (result > 0)
                {
                    return true;
                }


            }

            return false;
        }



        public bool Deleteincentivebyid(long id)
        {
            var res = context.IncentiveDistributionDetails.FirstOrDefault(x => x.Id == id && x.IsActive);
            res.IsActive = false;
            context.Entry(res).State = EntityState.Modified;
            var result = context.SaveChanges();
            if (result > 0)
            {
                return true;
            }


            return false;
        }

        public IncentiveCateGoryViewModel GetIncentiveCateGoryList(int CompanyId)
        {
            IncentiveCateGoryViewModel model = new IncentiveCateGoryViewModel();


            model.Datalist = (from t1 in context.IncentiveCatagories
                              where t1.CompanyId == CompanyId && t1.Active == true
                              select new IncentiveCateGoryViewModel
                              { Id = t1.Id,
                                  Name = t1.Name,
                                  CreatedDate = t1.CreatedDate

                              }).ToList();
            return model;
        }

        public bool AddOrUpdateCategoryIncentive(IncentiveCatagory model)
        {
            try
            {

                if (model.Id != 0)
                {
                    var existingCatagory = context.IncentiveCatagories.FirstOrDefault(x => x.Id == model.Id);

                    if (existingCatagory != null)
                    {
                        existingCatagory.Name = model.Name;
                        context.SaveChanges();
                    }
                    else
                    {

                        return false;
                    }
                }
                else
                {
                    IncentiveCatagory incentiveCatagory = new IncentiveCatagory()
                    {
                        Name = model.Name,
                        CreatedDate = model.CreatedDate,
                        CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                        Active = true,
                        CompanyId = model.CompanyId,

                    };

                    context.IncentiveCatagories.Add(incentiveCatagory);
                    context.SaveChanges();
                }

                return true; // Return true if the operation is successful

            }
            catch (Exception ex)
            {
                // Handle exception, log, or return false based on your requirements
                Console.WriteLine("Error: " + ex.Message);
                return false; // Return false in case of an exception
            }
        }


        public IncentiveCateGoryViewModel GetEditItem(int id)
        {
            var existingCatagory = context.IncentiveCatagories
                                           .Where(x => x.Id == id)
                                           .Select(x => new IncentiveCateGoryViewModel
                                           {
                                               Id = x.Id,
                                               Name = x.Name,
                                               
                                           })
                                           .FirstOrDefault();

            return existingCatagory;
        }



            public bool DeleteIncentivecategory(int id)
            {
            var obj = context.IncentiveCatagories.FirstOrDefault(x=>x.Id == id);
            if (obj != null)
            {
                obj.Active= false;
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

            return false;


            }




    }
}
