using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class KttlServiceService : IKttlService
    {
        private bool disposed = false;
        ERPEntities context = new ERPEntities();
        public bool DeleteKttlService(int id)
        {
            throw new NotImplementedException();
        }

        public KttlCustomerModel GetKttlCustomer(int id)
        {
            if (id == 0)
            {
                return new KttlCustomerModel() { ClientId = id };
            }
            KttlCustomer kttlCustomer = context.KttlCustomers.Find(id);
            return ObjectConverter<KttlCustomer, KttlCustomerModel>.Convert(kttlCustomer);
        }

        public KttlServiceModel GetKttlService(int id)
        {
            if (id == 0)
            {
                return new KttlServiceModel() { ClientId = id };
            }
            KttlService kttlService = context.KttlServices.Find(id);
            return ObjectConverter<KttlService, KttlServiceModel>.Convert(kttlService);
        }

        public List<KttlServiceModel> GetKttlServices(string searchText)
        {
            // IQueryable<KttlService> kttlCustomers = context.KttlServices.Where(x => x.ServiceType.Contains(searchText) || x.Destination.Contains(searchText) || x.DepartureFrom.Contains(searchText) || x.DepartureTo.Contains(searchText)).OrderBy(x => x.ClientId);
            //return ObjectConverter<KttlService, KttlServiceModel>.ConvertList(kttlCustomers.ToList()).ToList();

            var query = from o in context.KttlServices
                        join c in context.KttlCustomers on o.ClientId equals c.ClientId
                        where (o.ServiceType.Contains(searchText) || o.Destination.Contains(searchText) || o.DepartureFrom.Contains(searchText) || o.DepartureTo.Contains(searchText))
                        select new KttlServiceModel()
                        {
                            OID = o.OID,
                            ClientId = o.ClientId,
                            ClientName = c.FullName,
                            DepartureDate = o.DepartureDate,
                            ReturnDate = o.ReturnDate,
                            DepartureFrom = o.DepartureFrom,
                            DepartureTo = o.DepartureTo,
                            HotelAddress = o.HotelAddress,
                            TicketNo = o.TicketNo,
                            ServiceType = o.ServiceType,
                            HazzPackage = o.HazzPackage,
                            PackageRate = o.PackageRate,
                        };
            return query.ToList();
        }

        public bool SaveKttlService(int id, KttlServiceModel model)
        {
            KttlService kttlService = ObjectConverter<KttlServiceModel, KttlService>.Convert(model);
            if (id > 0)
            {
                kttlService.ModifiedDate = DateTime.Now;
                kttlService.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }
            else
            {
                kttlService.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                kttlService.CreatedDate = DateTime.Now;
            }

            kttlService.ServiceType = model.ServiceType;
            if (model.ServiceYear != null)
            {
                kttlService.ServiceYear = model.ServiceYear.ToString();
            }
            kttlService.DepartureFrom = model.DepartureFrom;
            kttlService.DepartureTo = model.DepartureTo;
            kttlService.DepartureDate = model.DepartureDate;
            kttlService.ReturnDate = model.ReturnDate;
            kttlService.Destination = model.Destination;
            kttlService.TicketNo = model.TicketNo;
            kttlService.HotelAddress = model.HotelAddress;
            kttlService.TotalPassenger = model.TotalPassenger;
            kttlService.HazzPackage = model.HazzPackage;
            kttlService.PackageRate = model.PackageRate;


            kttlService.DurationofStay = model.DurationofStay;
            kttlService.TotalAdult = model.TotalAdult;
            kttlService.TotalChild = model.TotalChild;
            kttlService.TotalInfant = model.TotalInfant;
            kttlService.AirlinesName = model.AirlinesName;
            kttlService.AirportDrop = model.AirportDrop;
            kttlService.AirportPickup = model.AirportPickup;
            kttlService.HotelType = model.HotelType;
            kttlService.HotelName = model.HotelName;
            kttlService.RoomType = model.RoomType;
            kttlService.TotalNights = model.TotalNights;


            kttlService.TotalRoom = model.TotalRoom;
            kttlService.TotalRoomPrice = model.TotalRoomPrice;
            kttlService.TransportationType = model.TransportationType;
            kttlService.Route = model.Route;
            kttlService.TotalHajDay = model.TotalHajDay;
            kttlService.ResponsiblePerson = model.ResponsiblePerson;
            kttlService.NextScheduleDate = model.NextScheduleDate;
            kttlService.LastMeetingDate1 = model.LastMeetingDate1;
            kttlService.LastMeetingDate2 = model.LastMeetingDate2;
            kttlService.SourceOfMedia = model.SourceOfMedia;
            kttlService.PromotionalOffer = model.PromotionalOffer;

            kttlService.Remarks = model.Remarks;



            context.Entry(kttlService).State = kttlService.OID == 0 ? EntityState.Added : EntityState.Modified;
            return context.SaveChanges() > 0;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }
    }
}
