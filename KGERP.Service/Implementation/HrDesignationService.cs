using KGERP.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KGERP.Service.Implementation
{
    public class HrDesignationService
    {
        private readonly ERPEntities _context = new ERPEntities();
        public HrDesignationService(ERPEntities context)
        {
            _context = context;
        }

        public class DesignationViewModel{

            public int DesignationId { get; set; }
            public int CompanyId { get; set; }
            public string Name { get; set; }
            public int CreatedBy { get; set; }
            public System.DateTime CreatedDate { get; set; }
            public IEnumerable<DesignationViewModel> DataList { get; set; }
        }

        public DesignationViewModel AddDes(DesignationViewModel vm)
        {
            Designation designation = new Designation();
            //designation.CreatedBy = HttpContext.Current.User.Identity.Name;
            //designation.CreatedBy = 1;
            designation.CreatedDate = DateTime.Now;
            designation.Name = vm.Name;
            _context.Designations.Add(designation);
            _context.SaveChangesAsync();
           
            return vm;
        }
        public DesignationViewModel Updatedes(DesignationViewModel designation)
        {
            var exit = _context.Designations.FirstOrDefault(f => f.DesignationId == designation.DesignationId);
            exit.Name = designation.Name;
              _context.Entry(exit).State = EntityState.Modified;
              _context.SaveChangesAsync();
            return designation;
        }

        public DesignationViewModel desilist(DesignationViewModel designation)
        {
            List<DesignationViewModel> model = new List<DesignationViewModel>();
            var listof = _context.Designations.ToList();
            foreach (var item in listof)
            {
                DesignationViewModel vm = new DesignationViewModel();
                vm.DesignationId = item.DesignationId;
                vm.Name = item.Name;
                vm.CreatedBy = (int)item.CreatedBy;
                model.Add(vm);
            }
            designation.DataList = model;
            return designation;

        }

        public bool checckname(DesignationViewModel designation)
        {
            var exit = _context.Designations.FirstOrDefault(f => f.Name.Trim() == designation.Name.Trim());
            if (exit==null)
            {
                return true;
            }
            return false;
        }

        }
}
