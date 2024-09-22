namespace KGERP.Data.CustomModel
{
    public class MonthlyTargetCM
    {
        public string CompanyName { get; set; }
        public string MonthName { get; set; }
        public int MonthNo { get; set; }
        public int YearNo { get; set; }

        public decimal TotalDue { get; set; }
        public decimal TotalCollection { get; set; }

        public decimal OpeningDue { get; set; }
        public decimal CollectionTarget { get; set; }
        public decimal ActualCollection { get; set; }
        public decimal Achievement { get; set; }
        public decimal ClosingDue { get; set; }

    }
}
