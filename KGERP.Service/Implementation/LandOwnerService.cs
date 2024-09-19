using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KGERP.Service.Implementation
{
    public class LandOwnerService : ILandOwnerService
    {
        private readonly ERPEntities context;
        public LandOwnerService(ERPEntities context)
        {
            this.context = context;
        }
        public List<LandReceiverModel> GetLandReceiver(string searchText)
        {
            //searchText = searchText ?? string.Empty;
            //IQueryable<LandOwner> landOwner = context.LandOwners.Where(x => x.LandOwnerName.ToLower().Contains(searchText.ToLower()));
            IQueryable<LandReceiver> landReceiver = context.LandReceivers;
            List<LandReceiverModel> models = ObjectConverter<LandReceiver, LandReceiverModel>.ConvertList(landReceiver.ToList()).ToList();
            return models;
        }

        public LandReceiverModel GetLandReceiver(int id)
        {
            LandReceiver receiver = context.LandReceivers.FirstOrDefault(x => x.LandReceiverId == id);
            if (receiver == null)
            {
                receiver = new LandReceiver();
            }
            return ObjectConverter<LandReceiver, LandReceiverModel>.Convert(receiver);
        }

        public List<LandUserModel> GetLandUser(string searchText)
        {
            //IQueryable<LandUser> landUser = context.LandUsers.Where(x => x.LandUserName.ToLower().Contains(searchText.ToLower()));
            IQueryable<LandUser> landUser = context.LandUsers;
            List<LandUserModel> models = ObjectConverter<LandUser, LandUserModel>.ConvertList(landUser.ToList()).ToList();
            return models;
        }

        public LandUserModel GetLandUser(int id)
        {
            LandUser user = context.LandUsers.FirstOrDefault(x => x.LandUserId == id);
            if (user == null)
            {
                user = new LandUser();
            }
            return ObjectConverter<LandUser, LandUserModel>.Convert(user);
        }

        public bool SaveLandReceiver(int id, LandReceiverModel model)
        {
            if (model == null)
            {
                throw new Exception("Land Receiver data missing!");
            }

            LandReceiver receiver = ObjectConverter<LandReceiverModel, LandReceiver>.Convert(model);
            if (id > 0)
            {
                receiver = context.LandReceivers.FirstOrDefault(x => x.LandReceiverId == id);
                if (receiver == null)
                {
                    throw new Exception("Land Owner Menu not found!");
                }


                receiver.LandReceiverId = model.LandReceiverId;
                receiver.LandReceiverName = model.LandReceiverName;
                receiver.ShortName = model.ShortName;
                receiver.Address = model.Address;
                receiver.Phone = model.Phone;
                receiver.Email = model.Email;
                return context.SaveChanges() > 0;
            }

            else
            {



                context.LandReceivers.Add(receiver);
                return context.SaveChanges() > 0;
            }
        }


        public bool SaveLandUser(int id, LandUserModel model)
        {
            if (model == null)
            {
                throw new Exception("Land User data missing!");
            }

            LandUser user = ObjectConverter<LandUserModel, LandUser>.Convert(model);
            if (id > 0)
            {
                user = context.LandUsers.FirstOrDefault(x => x.LandUserId == id);
                if (user == null)
                {
                    throw new Exception("Land Owner Menu not found!");
                }


                user.LandUserId = model.LandUserId;
                user.LandUserName = model.LandUserName;
                user.ShortName = model.ShortName;
                user.Phone = model.Phone;
                user.Email = model.Email;
                return context.SaveChanges() > 0;
            }

            else
            {



                context.LandUsers.Add(user);
                return context.SaveChanges() > 0;
            }
        }

    }
}
