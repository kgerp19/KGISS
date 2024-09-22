namespace KGERP.ViewModel
{
    public class KgreCostSetupModel
    {
        public long Id { get; set; }
        public string NameofCost { get; set; }
        public int FabricsProcId { get; set; }
        public string Company { get; set; }
        public float Rate { get; set; }
        public string Description { get; set; }
    }
}