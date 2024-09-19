using KGERP.Data.Models;
using KGERP.Service.Implementation.OrderApproval;
using KGERP.Service.Implementation.OrderApproval.ViewModels;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.SMS_Service_Implementation
{
    public class FeedErpSMS
    {
        private readonly ERPEntities _context=new ERPEntities();
        private readonly FeedSingleMeassagesServices sms=new FeedSingleMeassagesServices();    
        public async Task<bool> orderMasterSms(int orderMasterId,bool status)
        {
            List<OrderMasterSignatoryVM> list = new List<OrderMasterSignatoryVM>();   
             list = await (from approval in _context.ApprovalOrderMasters
                              join a in _context.OrderMasterSignatories on approval.SalesOrderSignatoryId equals a.SalesOrderSignatoryId
                              join e in _context.Employees on a.EmployeeId equals e.Id
                              join c in _context.Companies on e.CompanyId equals c.CompanyId
                              join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                              join ds in _context.Designations on e.DesignationId equals ds.DesignationId
                              join order in _context.OrderMasters on approval.OrderMasterId equals order.OrderMasterId
                              join cus in _context.Vendors on order.CustomerId equals cus.VendorId
                              join salse in _context.Employees on order.SalePersonId equals salse.Id
                              where a.IsActive && approval.OrderMasterId == orderMasterId
                              select new OrderMasterSignatoryVM()
                              {
                                  SignatoryId = a.SalesOrderSignatoryId,
                                  EmployeeId = e.EmployeeId,
                                  EmployeeName = e.Name,
                                  CompanyId = a.CompanyId,
                                  CompanyName = c.Name,
                                  DepartmentName = d.Name,
                                  DesignationName = ds.Name,
                                  Precedence = a.Precedence,
                                  EmployeeMobile = e.MobileNo,
                                  SignatoryStatus = (SignatoryStatusEnum)approval.ApprovalStatus,
                                  OrderNo= order.OrderNo,
                                  Customer=cus.Name,
                                  SalsePersone=salse.Name,
                                  OrderDate= order.OrderDate,   
                              }).OrderBy(x => x.Precedence).ToListAsync();
            var res= list.FirstOrDefault(f=>f.SignatoryStatus== (int)SignatoryStatusEnum.Pending);
            if (res == null&&status==false) {
                return true;
            }
            if (res !=null && status == false)
            {
                var url = "http://103.132.94.84:90/Procurement/FeedProcurementSalesOrderSlave?companyId=8&productType=F&orderMasterId=" + orderMasterId + "";
                ErpSMS erpSMS = new ErpSMS
                {
                    Message = "Dear Sir," + System.Environment.NewLine + System.Environment.NewLine + "A D/O of (" + res.Customer + ") has been sent by (" + res.SalsePersone + "), Please Approve. " + url + "" + System.Environment.NewLine + System.Environment.NewLine + " KRISHIBID FEED LTD.",
                    CompanyId = 8,
                    Date = res.OrderDate,
                    Status = (int)EnumSmSStatus.Pending,
                    PhoneNo = res.EmployeeMobile,
                    // PhoneNo = "01840019826", 
                    SmsType = 2,
                    Remarks = res.OrderNo,
                    TryCount = 0,
                    RowTime = DateTime.Now,
                    Subject = "Sale Order Notification"
                };
                _context.ErpSMS.Add(erpSMS);
                await _context.SaveChangesAsync();
                await sms.FeedSingleMeassagesAsync(erpSMS);
            }

            if (res == null && status == true)
            {
                var res2 = list.FirstOrDefault(f => f.SignatoryStatus != (int)SignatoryStatusEnum.Pending);
                string[] listofmobile = { "01700729228", "01700729163", "01700729172" };
                foreach (var mobile in listofmobile)
                {
                    var url = "http://103.132.94.84:90/Procurement/FeedProcurementSalesOrderSlave?companyId=8&productType=F&orderMasterId=" + orderMasterId + "";
                    ErpSMS erpSMS = new ErpSMS
                    {
                        Message = "Dear Sir," + System.Environment.NewLine + System.Environment.NewLine + "A D/O of (" + res2.Customer + ") has been Approved by Accounts, Please Deliver. " + url + "" + System.Environment.NewLine + System.Environment.NewLine + " KRISHIBID FEED LTD.",
                        CompanyId = 8,
                        Date = res2.OrderDate,
                        Status = (int)EnumSmSStatus.Pending,
                        PhoneNo = mobile,
                        // PhoneNo = "01840019826",
                        SmsType = 2,
                        Remarks = res2.OrderNo,
                        TryCount = 0,
                        RowTime = DateTime.Now,
                        Subject = "Sale Order Notification"
                    };
                    _context.ErpSMS.Add(erpSMS);
                    await _context.SaveChangesAsync();
                    await sms.FeedSingleMeassagesAsync(erpSMS);
                }
            }
            return true;
        }

    }
}
