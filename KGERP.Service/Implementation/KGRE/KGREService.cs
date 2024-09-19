using KGERP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.KGRE
{

    public class KGREService
    {
        private readonly ERPEntities _db;

        public KGREService(ERPEntities db)
        {
            _db = db;
        }

        #region Customer
        public async Task<KGRECRMVM> GetCustomerByCompany(int companyId)
        {
            KGRECRMVM kGRECRMVM = new KGRECRMVM();
            kGRECRMVM.CompanyFK = companyId;
            kGRECRMVM.DataList = await Task.Run(() => (from t1 in _db.KGRECustomers.Where(x => x.CompanyId == companyId)

                                                       select new KGRECRMVM
                                                       {
                                                           ClientId = t1.ClientId,
                                                           FullName = t1.FullName,
                                                           Email = t1.Email,
                                                           MobileNo = t1.MobileNo,
                                                           CompanyFK = t1.CompanyId,
                                                           MobileNo2 = t1.MobileNo2,
                                                           ResponsibleOfficer = t1.ResponsibleOfficer,
                                                           PresentAddress = t1.PresentAddress,
                                                           PermanentAddress = t1.PermanentAddress,
                                                           CreatedBy = t1.CreatedBy,
                                                           Remarks = t1.Remarks,
                                                       }).OrderByDescending(x => x.ClientId).AsEnumerable());


            return kGRECRMVM;
        }
        public KGRECRMVM GetCustomerById(int clientId)
        {
            KGRECRMVM kGRECRMVM = new KGRECRMVM();
            var client = _db.KGRECustomers.Find(clientId);
            kGRECRMVM.FullName = client.FullName;
            return kGRECRMVM;
        }
        #endregion

    }
}
