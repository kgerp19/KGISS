using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KGERP.Service.ServiceModel
{
    public class KttlServiceModel
    {
        public string ButtonName
        {
            get
            {
                return OID > 0 ? "Update" : "Save";
            }
        }

        public int OID { get; set; }
        public Nullable<int> ClientId { get; set; }
        [DisplayName("Service Type")]
        [Required]
        public string ServiceType { get; set; }
        [DisplayName("Departure Airport")]
        public string DepartureFrom { get; set; }
        [DisplayName("Arrival Airport")]
        public string DepartureTo { get; set; }
        public string Destination { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Departure Date")]
        public Nullable<System.DateTime> DepartureDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Return Date")]
        public Nullable<System.DateTime> ReturnDate { get; set; }
        [DisplayName("Ticket No")]
        public string TicketNo { get; set; }
        [DisplayName("Hotel Address")]
        public string HotelAddress { get; set; }
        [DisplayName("Total Passenger")]
        public string TotalPassenger { get; set; }
        [DisplayName("Package Rate")]
        public string PackageRate { get; set; }
        [DisplayName("Travel Class")]
        public string TravelClass { get; set; }
        [DisplayName("Hazz Package")]
        public string HazzPackage { get; set; }
        public string Remarks { get; set; }
        [DisplayName("Client Name")]
        public string ClientName { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }


        [DisplayName("Adults (12+)")]
        public Nullable<int> TotalAdult { get; set; }
        [DisplayName("Children (2-11)")]
        public Nullable<int> TotalChild { get; set; }
        [DisplayName("Infants (‹2 yrs)")]
        public Nullable<int> TotalInfant { get; set; }
        [DisplayName("Airlines Name")]
        public string AirlinesName { get; set; }
        [DisplayName("Airport Name")]
        public string AirportName { get; set; }
        [DisplayName("Hotel Type")]
        public string HotelType { get; set; }
        [DisplayName("Hotel Name")]
        public string HotelName { get; set; }
        [DisplayName("Room Type")]
        public string RoomType { get; set; }
        [DisplayName("Total Nights")]
        public string TotalNights { get; set; }
        [DisplayName("Total Room")]
        public Nullable<int> TotalRoom { get; set; }
        [DisplayName("Total Room Price")]

        public Nullable<int> TotalRoomPrice { get; set; }
        [DisplayName("Transportation Type")]

        public string TransportationType { get; set; }
        [DisplayName("Airport Pickup")]

        public string AirportPickup { get; set; }
        [DisplayName("Airport Drop")]

        public string AirportDrop { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Airport Pickup Date")]
        public Nullable<System.DateTime> AirportPickupDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Airport Drop Date")]
        public Nullable<System.DateTime> AirportDropDate { get; set; }
        [DisplayName("Makkah Ziyarah")]
        public string MakkahZiyarah { get; set; }
        [DisplayName("Madinah Ziyarah")]
        public string MadinahZiyarah { get; set; }
        public string Route { get; set; }
        [DisplayName("Total Haj Day")]
        public Nullable<int> TotalHajDay { get; set; }
        [DisplayName("Responsible Person")]
        public string ResponsiblePerson { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Next Schedule Date")]
        public Nullable<System.DateTime> NextScheduleDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Last Meeting Date1")]
        public Nullable<System.DateTime> LastMeetingDate1 { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Last Meeting Date2")]
        public Nullable<System.DateTime> LastMeetingDate2 { get; set; }
        [DisplayName("Source of Media")]
        public string SourceOfMedia { get; set; }
        [DisplayName("Promotional Offer")]
        public string PromotionalOffer { get; set; }
        [DisplayName("Stay Day")]
        public int DurationofStay { get; set; }
        [DisplayName("Service Year")]
        public Nullable<int> ServiceYear { get; set; }

        // ddl binding
        public List<int> ServiceYears { get; set; }
        public List<SelectModel> HazzPackages { get; set; }
        public List<SelectModel> IsMakkah2Madinah { get; set; }
        public List<SelectModel> IsMakkahZiyarah { get; set; }
        public List<SelectModel> IsMadinahZiyarah { get; set; }
        public List<SelectModel> AirportPickups { get; set; }
        public List<SelectModel> AirportDrops { get; set; }
        public List<SelectModel> ServiceTypes { get; set; }
        public List<SelectModel> RoomTypes { get; set; }
        public List<SelectModel> TransportTypes { get; set; }
        public List<SelectModel> HotelTypes { get; set; }
        public List<SelectModel> AirportNames { get; set; }
        public List<SelectModel> AirlinesNames { get; set; }

    }
}
