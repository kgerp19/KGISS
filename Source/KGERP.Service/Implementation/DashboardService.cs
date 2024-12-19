using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class DashboardService
    {
        private readonly ERPEntities _context;
        public DashboardService(ERPEntities context)
        {
            _context = context;

        }
        public DashBoardViewModel AllCount(int companyId)
        {
            DashBoardViewModel vm = new DashBoardViewModel();


            vm.Totalsupplier = _context.Vendors.Where(x => x.CompanyId == companyId && x.VendorTypeId == 1 && x.IsActive).Count();
            vm.Totalvendor = _context.Vendors.Where(x => x.CompanyId == companyId && x.VendorTypeId == 2 && x.IsActive).Count();
            vm.TotalPurchase = _context.PurchaseOrders.Where(x => x.CompanyId == companyId && x.PurchaseDate == DateTime.Today && x.IsActive).Count();
            //vm.TotalPurcaseAmmount = (from t1 in _context.PurchaseOrders
            //                          join t2 in _context.PurchaseOrderDetails on t1.PurchaseOrderId equals t2.PurchaseOrderId
            //                          where t1.CompanyId == companyId && t1.PurchaseDate == DateTime.Today && t1.IsActive
            //                          select t2.PurchaseAmount).DefaultIfEmpty(0).Sum();
            vm.TotalSale = _context.OrderMasters.Where(x => x.CompanyId == companyId && x.OrderDate == DateTime.Today && x.IsActive).Count();
            vm.TotalSaleAmmount = (from t1 in _context.OrderMasters
                                   join t2 in _context.OrderDetails on t1.OrderMasterId equals t2.OrderMasterId
                                   where t1.CompanyId == companyId && t1.OrderDate == DateTime.Today && t1.IsActive
                                   select t2.Amount).DefaultIfEmpty(0).Sum();
            vm.Payment = (decimal)(from t1 in _context.Vendors
                                   join t2 in _context.Payments on t1.VendorId equals t2.VendorId
                                   join t3 in _context.PaymentMasters on t2.PaymentMasterId equals t3.PaymentMasterId
                                   where t1.VendorTypeId == 1 && t1.CompanyId == companyId && t1.IsActive && t2.IsActive && t3.IsActive && t3.TransactionDate == DateTime.Today
                                   select t2.InAmount).DefaultIfEmpty(0).Sum();

            vm.Collection = (decimal)(from t1 in _context.Payments
                                      join t2 in _context.Vendors on t1.VendorId equals t2.VendorId
                                      join t3 in _context.PaymentMasters on t1.PaymentMasterId equals t3.PaymentMasterId
                                      where t2.VendorTypeId == 2 && t1.CompanyId == companyId && t1.IsActive && t2.IsActive && t3.IsActive && t3.TransactionDate == DateTime.Today
                                      select t1.OutAmount ?? 0).DefaultIfEmpty(0).Sum();

            var enddate = DateTime.Now.AddMonths(-1);
            vm.TotalmonthPurchaseAmmount = (from t1 in _context.PurchaseOrders
                                            join t2 in _context.PurchaseOrderDetails on t1.PurchaseOrderId equals t2.PurchaseOrderId
                                            where t1.CompanyId == companyId && t1.PurchaseDate >= enddate && t1.PurchaseDate <= DateTime.Now && t1.IsActive && t2.IsActive
                                            select t2.PurchaseAmount).DefaultIfEmpty(0).Sum();

            vm.TotalMonthSeleAmmount = (from t1 in _context.OrderMasters
                                        join t2 in _context.OrderDetails on t1.OrderMasterId equals t2.OrderMasterId
                                        where t1.CompanyId == companyId && t1.IsActive && t2.IsActive && t1.OrderDate >= enddate && t1.OrderDate <= DateTime.Now
                                        select t2.Amount).DefaultIfEmpty(0).Sum();


            vm.MonthPurchasePayment = (decimal)(from t1 in _context.Vendors
                                                join t2 in _context.Payments on t1.VendorId equals t2.VendorId
                                                join t3 in _context.PaymentMasters on t2.PaymentMasterId equals t3.PaymentMasterId
                                                where t1.VendorTypeId == 1 && t1.IsActive && t2.IsActive && t3.IsActive && t1.CompanyId == companyId && t2.TransactionDate >= enddate && t3.TransactionDate <= DateTime.Now
                                                select t2.OutAmount ?? 0).DefaultIfEmpty(0).Sum();


            vm.MonthSaleCollection = (decimal)(from t1 in _context.Payments
                                               join t2 in _context.Vendors on t1.VendorId equals t2.VendorId
                                               join t3 in _context.PaymentMasters on t1.PaymentMasterId equals t3.PaymentMasterId

                                               where t2.VendorTypeId == 2 && t1.IsActive && t2.IsActive && t3.IsActive && t1.CompanyId == companyId && t1.TransactionDate >= enddate && t1.TransactionDate <= DateTime.Now
                                               select t1.InAmount).DefaultIfEmpty(0).Sum();

            return vm;
        }

        public DashBoardViewModel DashBoardData(int companyId)
        {
            DashBoardViewModel vm = new DashBoardViewModel();
            DateTime currentDate = DateTime.Now;
            DateTime today = DateTime.Now;
            DateTime firstDateOfLastMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
            DateTime lastDateOfLastMonth = firstDateOfLastMonth.AddMonths(1).AddDays(-1);


            vm.TodayPurchase = TodayPurches(companyId);
            vm.TodaySales = TodaySales(companyId);
            vm.TotalPreviousMonthSeleAmmount = TotalSalesOfPreviousMonth(companyId, firstDateOfLastMonth, lastDateOfLastMonth);
            vm.TotalPreviousMonthPurchesAmmount = TotalPurchesOfPreviousMonth(companyId, firstDateOfLastMonth, lastDateOfLastMonth);
            vm.Totalvendor = _context.Vendors.Where(x => x.CompanyId == companyId && x.VendorTypeId == 2 && x.IsActive).Count();
            vm.Totalsupplier = _context.Vendors.Where(x => x.CompanyId == companyId && x.VendorTypeId == 1 && x.IsActive).Count();
            vm.TotalEmployee = _context.Employees.Where(x => x.CompanyId == companyId && x.Active).Count();
            vm.TotalPurcaseAmmount = TotalPurches(companyId);
            vm.TotalSalesAmmount = TotalSales(companyId);





            return vm;
        }

        private double TodaySales(int companyId)
        {
            var today = DateTime.Today;
            var sales = (from t1 in _context.Vouchers
                         where t1.IsActive
                               && t1.IsSubmit
                               && t1.VoucherDate.HasValue
                               && t1.VoucherDate.Value == today
                               && t1.CompanyId == companyId
                         join t2 in _context.VoucherDetails on t1.VoucherId equals t2.VoucherId
                         where t2.IsActive
                         join t3 in _context.HeadGLs on t2.AccountHeadId equals t3.Id
                         where t3.IsActive
                         join t4 in _context.Head5 on t3.ParentId equals t4.Id
                         join t5 in _context.Head4 on t4.ParentId equals t5.Id
                         join t6 in _context.Head3 on t5.ParentId equals t6.Id
                         join t7 in _context.Head2 on t6.ParentId equals t7.Id
                         where t7.AccCode == "31"
                         select new
                         {
                             Credit = t2.CreditAmount,
                             Debit = t2.DebitAmount
                         }).ToList();   // Execute the query and retrieve the results  

            // Summing Credits and Debits  
            double totalSales = sales.Sum(x => x.Credit) - sales.Sum(x => x.Debit);

            // Handling Null (similar to ISNULL in SQL)  
            totalSales = totalSales > 0 ? totalSales : 0;
            return totalSales;
        }


        private double TodayPurches(int companyId)
        {
            var today = DateTime.Today; // Store today's date in a variable  
            var purches = (from t1 in _context.Vouchers
                           where t1.IsActive
                                 && t1.IsSubmit
                                 && t1.VoucherDate.HasValue
                                 && t1.VoucherDate.Value == today
                                 && t1.CompanyId == companyId
                           join t2 in _context.VoucherDetails on t1.VoucherId equals t2.VoucherId
                           where t2.IsActive
                           join t3 in _context.HeadGLs on t2.AccountHeadId equals t3.Id
                           where t3.IsActive
                           join t4 in _context.Head5 on t3.ParentId equals t4.Id
                           join t5 in _context.Head4 on t4.ParentId equals t5.Id
                           join t6 in _context.Head3 on t5.ParentId equals t6.Id
                           join t7 in _context.Head2 on t6.ParentId equals t7.Id
                           where t7.AccCode == "41"
                           select new
                           {
                               Credit = t2.CreditAmount,
                               Debit = t2.DebitAmount
                           }).ToList();

            // Summing Credits and Debits  
            double totalpurches = purches.Sum(x => x.Debit) - purches.Sum(x => x.Credit);

            // Handling Null (similar to ISNULL in SQL)  
            totalpurches = totalpurches > 0 ? totalpurches : 0;
            return totalpurches;
        }


        private double TotalSalesOfPreviousMonth(int companyId, DateTime firstDate, DateTime lastDate)
        {
            var today = DateTime.Today; // Store today's date in a variable  
            var salesOfMonth = (from t1 in _context.Vouchers
                                where t1.IsActive
                                      && t1.IsSubmit
                                      && t1.VoucherDate.HasValue
                                      && (t1.VoucherDate.Value >= firstDate && t1.VoucherDate.Value <= lastDate)
                                      && t1.CompanyId == companyId
                                join t2 in _context.VoucherDetails on t1.VoucherId equals t2.VoucherId
                                where t2.IsActive
                                join t3 in _context.HeadGLs on t2.AccountHeadId equals t3.Id
                                where t3.IsActive
                                join t4 in _context.Head5 on t3.ParentId equals t4.Id
                                join t5 in _context.Head4 on t4.ParentId equals t5.Id
                                join t6 in _context.Head3 on t5.ParentId equals t6.Id
                                join t7 in _context.Head2 on t6.ParentId equals t7.Id
                                where t7.AccCode == "31"
                                select new
                                {
                                    Credit = t2.CreditAmount,
                                    Debit = t2.DebitAmount
                                }).ToList();

            // Summing Credits and Debits  
            double totalsalesOfMonth = salesOfMonth.Sum(x => x.Credit) - salesOfMonth.Sum(x => x.Debit);

            // Handling Null (similar to ISNULL in SQL)  
            totalsalesOfMonth = totalsalesOfMonth > 0 ? totalsalesOfMonth : 0;
            return totalsalesOfMonth;
        }

        private double TotalPurchesOfPreviousMonth(int companyId, DateTime firstDate, DateTime lastDate)
        {
            var today = DateTime.Today; // Store today's date in a variable  
            var PurchesOfMonth = (from t1 in _context.Vouchers
                                  where t1.IsActive
                                        && t1.IsSubmit
                                        && t1.VoucherDate.HasValue
                                        && (t1.VoucherDate.Value >= firstDate && t1.VoucherDate.Value <= lastDate)
                                        && t1.CompanyId == companyId
                                  join t2 in _context.VoucherDetails on t1.VoucherId equals t2.VoucherId
                                  where t2.IsActive
                                  join t3 in _context.HeadGLs on t2.AccountHeadId equals t3.Id
                                  where t3.IsActive
                                  join t4 in _context.Head5 on t3.ParentId equals t4.Id
                                  join t5 in _context.Head4 on t4.ParentId equals t5.Id
                                  join t6 in _context.Head3 on t5.ParentId equals t6.Id
                                  join t7 in _context.Head2 on t6.ParentId equals t7.Id
                                  where t7.AccCode == "41"
                                  select new
                                  {
                                      Credit = t2.CreditAmount,
                                      Debit = t2.DebitAmount
                                  }).ToList();

            // Summing Credits and Debits  
            double totalPurchesOfMonth = PurchesOfMonth.Sum(x => x.Debit) - PurchesOfMonth.Sum(x => x.Credit);

            // Handling Null (similar to ISNULL in SQL)  
            totalPurchesOfMonth = totalPurchesOfMonth > 0 ? totalPurchesOfMonth : 0;
            return totalPurchesOfMonth;
        }


        private double TotalPurches(int companyId)
        {
            var today = DateTime.Today; // Store today's date in a variable  
            var PurchesOfMonth = (from t1 in _context.Vouchers
                                  where t1.IsActive
                                        && t1.IsSubmit
                                        && t1.CompanyId == companyId
                                  join t2 in _context.VoucherDetails on t1.VoucherId equals t2.VoucherId
                                  where t2.IsActive
                                  join t3 in _context.HeadGLs on t2.AccountHeadId equals t3.Id
                                  where t3.IsActive
                                  join t4 in _context.Head5 on t3.ParentId equals t4.Id
                                  join t5 in _context.Head4 on t4.ParentId equals t5.Id
                                  join t6 in _context.Head3 on t5.ParentId equals t6.Id
                                  join t7 in _context.Head2 on t6.ParentId equals t7.Id
                                  where t7.AccCode == "41"
                                  select new
                                  {
                                      Credit = t2.CreditAmount,
                                      Debit = t2.DebitAmount
                                  }).ToList();

            // Summing Credits and Debits  
            double totalPurchesOfMonth = PurchesOfMonth.Sum(x => x.Debit) - PurchesOfMonth.Sum(x => x.Credit);

            // Handling Null (similar to ISNULL in SQL)  
            totalPurchesOfMonth = totalPurchesOfMonth > 0 ? totalPurchesOfMonth : 0;
            return totalPurchesOfMonth;
        }


        private double TotalSales(int companyId)
        {
            var today = DateTime.Today; // Store today's date in a variable  
            var salesOfMonth = (from t1 in _context.Vouchers
                                where t1.IsActive
                                      && t1.IsSubmit
                                      && t1.CompanyId == companyId
                                join t2 in _context.VoucherDetails on t1.VoucherId equals t2.VoucherId
                                where t2.IsActive
                                join t3 in _context.HeadGLs on t2.AccountHeadId equals t3.Id
                                where t3.IsActive
                                join t4 in _context.Head5 on t3.ParentId equals t4.Id
                                join t5 in _context.Head4 on t4.ParentId equals t5.Id
                                join t6 in _context.Head3 on t5.ParentId equals t6.Id
                                join t7 in _context.Head2 on t6.ParentId equals t7.Id
                                where t7.AccCode == "31"
                                select new
                                {
                                    Credit = t2.CreditAmount,
                                    Debit = t2.DebitAmount
                                }).ToList();

            // Summing Credits and Debits  
            double totalsalesOfMonth = salesOfMonth.Sum(x => x.Credit) - salesOfMonth.Sum(x => x.Debit);

            // Handling Null (similar to ISNULL in SQL)  
            totalsalesOfMonth = totalsalesOfMonth > 0 ? totalsalesOfMonth : 0;
            return totalsalesOfMonth;
        }
    }
}
