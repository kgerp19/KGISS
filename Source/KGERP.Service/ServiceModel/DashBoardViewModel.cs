using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
    public class DashBoardViewModel
    {
        public int CompanyId { get; set; }
        public double Totalvendor { get; set; }
        public int Totalsupplier { get; set; }
        public int TotalEmployee { get; set; }
        public int TotalPurchase { get; set; }
        public double TodayPurchase { get; set; }
        public double TodaySales { get; set; }
        public double TotalPurcaseAmmount { get; set; }
        public double TotalSalesAmmount { get; set; }
        public int TotalSale { get; set; }
        public double TotalSaleAmmount { get; set; }
        public decimal Payment { get; set; }
        public decimal Collection { get; set; }
        public decimal MonthPurchasePayment { get; set; }
        public decimal MonthSaleCollection { get; set; }
        public decimal TotalmonthPurchaseAmmount { get; set; }
        public double TotalMonthSeleAmmount { get; set; }
        public double TotalPreviousMonthPurchesAmmount { get; set; }
        public double TotalPreviousMonthSeleAmmount { get; set; }
    }
}
