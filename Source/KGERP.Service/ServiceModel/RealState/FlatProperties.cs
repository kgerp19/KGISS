using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel.RealState
{
    public class FlatProperties
    {
        public bool IsDuplex { get; set; }
        public List<string> Floors { get; set; } = new List<string>();
        public int LiftCount { get; set; }
        public int BedRoomCount { get; set; }
        public int WashroomRoomCount { get; set; }
        public int verandahCount { get; set; }
        public int DiningRoomCount { get; set; }
        public int DrawingRoomCount { get; set; }
        [Display(Name = "Dining Drawing Combined ")]
        public bool IsDiningDrawingCombined { get; set; }
        public int CompletionStatus { get; set; }
        public int LandFacing { get; set; }
        [Display(Name = "Parking ")]
        public bool HasParking { get; set; }
        public FlatUtility Utilies { get; set; } = new FlatUtility();


    }

    public class FlatUtility
    {
        [Display(Name = "Gas")]
        public bool HasGas { get; set; }
        [Display(Name = "Electricity")]
        public bool HasElectricity { get; set; }
        [Display(Name = "WaterSupply")]
        public bool HasWaterSupply { get; set; }
        [Display(Name = "SolarPanel")]
        public bool HasSolarPanel { get; set; }
    }
}
