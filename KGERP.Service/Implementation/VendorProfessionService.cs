using KGERP.Data.Models;
using KGERP.Service.Configuration;
using KGERP.Service.Implementation.Configuration;
using KGERP.Service.Implementation;
using KGERP.Service.Interface;
using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace KGERP.Service.Implementation.VendorProfessions
{
    public class VendorProfessionService : IVendorProfession
    {
        private readonly ERPEntities context;
        public VendorProfessionService(ERPEntities context)
        {
            this.context = context;
        }

       public VendorProfessionVm GetVendorProfessionVm() 
        {
            
            
            VendorProfessionVm vm = new VendorProfessionVm();
            vm.Datalist= (from t1 in context.VendorProfessions
                          where t1.IsActive
                          select new VendorProfessionVm
                          {
                              ProfessionId=t1.ProfessionId,
                           Name= t1.Name,
                          }).ToList();




            return vm;
        }

        public bool AddNewName(VendorProfession model)
        {
            var obj=context.VendorProfessions.Where(x=>x.Name==model.Name).ToList();

            if(model != null)
            {
                VendorProfession vendorProfession = new VendorProfession()
                {
                    Name = model.Name,
                    CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                context.VendorProfessions.Add(vendorProfession);
                var result = context.SaveChanges();
                if (result > 0)
                {
                    return true;
                }

            }

            return false;
        }

        public VendorProfession GetNameById(int id)
        {
            var res = context.VendorProfessions.FirstOrDefault(x => x.ProfessionId == id);


            return res;

        }


        public bool Editsave(VendorProfession model)
        {
            var obj = context.VendorProfessions.FirstOrDefault(x => x.ProfessionId == model.ProfessionId);

            if (obj != null) {


                obj.Name = model.Name;
                obj.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
                obj.ModifiedDate = DateTime.Now;
                var result = context.SaveChanges();
                if (result > 0)
                {
                    return true;

                }

            }

            return false;
        }

        public bool DeletVenodorByiD (int id)
        {
            var obj = context.VendorProfessions.FirstOrDefault(x => x.ProfessionId == id);

            if (obj != null)
            {

                obj.IsActive = false;
                var result = context.SaveChanges();
                if (result > 0)
                {
                    return true;

                }

            }

            return false;
        }

        public List<object> getfordropdown()
        {
            var list = new List<object>();
            var v = context.VendorProfessions.Where(a => a.IsActive == true).ToList();
            foreach (var x in v)
            {
                list.Add(new { Text = x.Name, Value = x.ProfessionId });
            }
            return list;
        }
    }
}
