using KGERP.Data.Models;
using KGERP.Service.Implementation.General_Requisition.ViewModels;
using KGERP.Service.Implementation.Marketing;
using KGERP.Service.Implementation.OrderApproval.ViewModels;
using KGERP.Service.SMS_Service_Implementation;
using KGERP.Services.Procurement;
using KGERP.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.OrderApproval
{ 
    public class OrderApprovalService : IOrderApprovalService
    {
        private readonly ERPEntities _context = new ERPEntities();
        private readonly FeedErpSMS smsservice = new FeedErpSMS();
        private readonly AccountingService _accountingService;

        public OrderApprovalService(AccountingService accountingService)
        {
            _accountingService = accountingService;
        }
        public async Task<bool> AddOrderMasterSignatory(OrderMasterSignatory orderMaster)
        {
            _context.OrderMasterSignatories.Add(orderMaster);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateOrderMasterSignatory(OrderMasterSignatory orderMaster)
        {
            _context.OrderMasterSignatories.AddOrUpdate(orderMaster);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> RemoveMasterSignatory(int id)
        {
            var exist = await _context.OrderMasterSignatories.FindAsync(id);
            if (exist != null)
            {
                exist.IsActive = false;
                if (await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }
            return false;
        }


        public async Task<IEnumerable<OrderMasterSignatoryVM>> GetAllOrderMasterSignatories(int? companyId)
        {
            var data = await (from a in _context.OrderMasterSignatories
                              join e in _context.Employees on a.EmployeeId equals e.Id
                              join c in _context.Companies on e.CompanyId equals c.CompanyId
                              join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                              join ds in _context.Designations on e.DesignationId equals ds.DesignationId
                              where a.IsActive
                              && (companyId == null || a.CompanyId == companyId)
                              select new OrderMasterSignatoryVM()
                              {
                                  SignatoryId = a.SalesOrderSignatoryId,
                                  EmployeeId = e.EmployeeId,
                                  EmployeeName = e.Name,
                                  CompanyId = a.CompanyId,
                                  CompanyName = c.Name,
                                  DepartmentName = d.Name,
                                  DesignationName = ds.Name,
                                  Precedence = a.Precedence
                              }).ToListAsync();
            return data;
        }
        public async Task<OrderMasterSignatoryVM> GetOrderMasterSignatoryById(int orderMasterSignatoryid)
        {
            var data = await (from a in _context.OrderMasterSignatories
                              join e in _context.Employees on a.EmployeeId equals e.Id
                              join c in _context.Companies on e.CompanyId equals c.CompanyId
                              join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                              join ds in _context.Designations on e.DesignationId equals ds.DesignationId
                              where a.IsActive && a.SalesOrderSignatoryId == orderMasterSignatoryid
                              select new OrderMasterSignatoryVM()
                              {
                                  SignatoryId = a.SalesOrderSignatoryId,
                                  EmployeeId = e.EmployeeId,
                                  EmployeeName = e.Name,
                                  CompanyId = a.CompanyId,
                                  CompanyName = c.Name,
                                  DepartmentName = d.Name,
                                  DesignationName = ds.Name,
                                  Precedence = a.Precedence
                              }).FirstOrDefaultAsync();
            return data;
        }






        public async Task<bool> AddSignatoryApproval(ApprovalOrderMaster orderMaster)
        {
            _context.ApprovalOrderMasters.Add(orderMaster);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateSignatoryApproval(ApprovalOrderMaster orderMaster)
        {
            _context.ApprovalOrderMasters.Add(orderMaster);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> RemoveSignatoryApproval(int id)
        {
            //var exist = await _context.ApprovalOrderMasters.FindAsync(id);
            //if (exist != null)
            //{
            //    exist.IsActive = false;
            //    if (await _context.SaveChangesAsync() > 0)
            //    {
            //        return true;
            //    }
            //}
            return false;
        }


        public async Task<IEnumerable<OrderMasterSignatoryVM>> GetAllSignatoryApprovals(int orderMasterId)
        {
            var data = await (from approval in _context.ApprovalOrderMasters
                              join a in _context.OrderMasterSignatories on approval.SalesOrderSignatoryId equals a.SalesOrderSignatoryId
                              join e in _context.Employees on a.EmployeeId equals e.Id
                              join c in _context.Companies on e.CompanyId equals c.CompanyId
                              join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                              join ds in _context.Designations on e.DesignationId equals ds.DesignationId
                              where a.IsActive && approval.IsActive && approval.OrderMasterId == orderMasterId
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
                                  SignatoryStatus = (SignatoryStatusEnum)approval.ApprovalStatus
                              }).OrderBy(x => x.Precedence).ToListAsync();
            return data;
        }
        public async Task<OrderMasterSignatoryVM> GetSignatoryApprovalById(int signatoryApprovalId)
        {
            var data = await (from approval in _context.ApprovalOrderMasters
                              join a in _context.OrderMasterSignatories on approval.SalesOrderSignatoryId equals a.SalesOrderSignatoryId
                              join e in _context.Employees on a.EmployeeId equals e.Id
                              join c in _context.Companies on e.CompanyId equals c.CompanyId
                              join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                              join ds in _context.Designations on e.DesignationId equals ds.DesignationId
                              where a.IsActive && approval.IsActive && approval.Id == signatoryApprovalId
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
                                  SignatoryStatus = (SignatoryStatusEnum)approval.ApprovalStatus
                              }).FirstOrDefaultAsync();
            return data;
        }


        public DateTime? ReceivedDate { get; set; }
        public async Task<bool> AddAllMappingSignatoryApproval(int orderMasterId)
        {
            var order = await _context.OrderMasters.FindAsync(orderMasterId);

            if (order == null)
            {
                return false;
            }
            int companyId = 8;


            var signatories = await (from a in _context.OrderMasterSignatories
                                     join e in _context.Employees on a.EmployeeId equals e.Id
                                     join c in _context.Companies on e.CompanyId equals c.CompanyId
                                     join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                                     join ds in _context.Designations on e.DesignationId equals ds.DesignationId
                                     where a.IsActive && a.CompanyId == companyId
                                     select new OrderMasterSignatoryVM()
                                     {
                                         SignatoryId = a.SalesOrderSignatoryId,
                                         //EmployeeId = e.EmployeeId,
                                         //EmployeeName = e.Name,
                                         //CompanyId = a.CompanyId,
                                         //CompanyName = c.Name,
                                         //DepartmentName = d.Name,
                                         //DesignationName = ds.Name,
                                         Precedence = a.Precedence

                                     }).OrderBy(x => x.Precedence).ToListAsync();



            //var signatories = await ( from t1 in _context.ApprovalOrderMasters
            //                          join a in _context.OrderMasterSignatories on t1.SalesOrderSignatoryId equals a.SalesOrderSignatoryId
            //                         join e in _context.Employees on a.EmployeeId equals e.Id
            //                         join c in _context.Companies on e.CompanyId equals c.CompanyId
            //                         join d in _context.Departments on e.DepartmentId equals d.DepartmentId
            //                         join ds in _context.Designations on e.DesignationId equals ds.DesignationId
            //                         where t1.IsActive && a.CompanyId == companyId
            //                         select new OrderMasterSignatoryVM()
            //                         {
            //                             SignatoryId = a.SalesOrderSignatoryId,
            //                             //EmployeeId = e.EmployeeId,
            //                             //EmployeeName = e.Name,
            //                             //CompanyId = a.CompanyId,
            //                             //CompanyName = c.Name,
            //                             //DepartmentName = d.Name,
            //                             //DesignationName = ds.Name,
            //                             Precedence = a.Precedence

            //                         }).OrderBy(x => x.Precedence).ToListAsync();

            if (signatories == null)
            {
                return false;
            }
            int status = 0;

            foreach (var item in signatories)
            {
                if (item.Precedence == 1)
                {
                    ReceivedDate = DateTime.Now;
                }
                ApprovalOrderMaster approval = new ApprovalOrderMaster()
                {
                    //ApprovalDate = DateTime.Now,
                    ApprovalStatus = (int)SignatoryStatusEnum.Pending,
                    OrderMasterId = orderMasterId,
                    Comments = "",
                    IsActive = true,
                    SalesOrderSignatoryId = item.SignatoryId,
                    Precedence = item.Precedence,
                    ReceivedDate = ReceivedDate,
                };
                _context.ApprovalOrderMasters.Add(approval);
                status += await _context.SaveChangesAsync();
            }

            if (status == signatories.Count())
            {
                order.Status = (int)EnumFeedSalesStatus.Submit;

                var res = await _context.SaveChangesAsync();
                if (res > 0)
                {
                    await smsservice.orderMasterSms(orderMasterId, false);
                }
                return true;
            }

            return false;
        }

        public async Task<VMFeedPayment> FeedPaymentGet(long orderMasterId)
        {
            VMFeedPayment VMPayment = new VMFeedPayment();
            VMPayment.DataListPayment = await Task.Run(() => (from t1 in _context.Payments.Where(x => x.IsActive)
                                                              join t0 in _context.Vendors on t1.VendorId equals t0.VendorId
                                                              join t5 in _context.HeadGLs on t1.BankId equals t5.Id
                                                              where t1.OrderMasterId == orderMasterId
                                                              && t1.BranchName != "Advance" && t1.BranchName != "Short Credit" // && t1.BranchName != ""
                                                              select new VMFeedPayment
                                                              {
                                                                  HeadGLId = t0.HeadGLId,
                                                                  PaymentFromHeadGLId = t1.PaymentFromHeadGLId,
                                                                  CommonCustomerName = t0.Name,
                                                                  CommonCustomerCode = t0.Code,

                                                                  Accounting_BankOrCashId = t5.Id,
                                                                  BankName = (t1.BranchName == "Advance" ? t1.BranchName + " In " + t5.AccName : t5.AccName) ?? t1.BranchName, // Check if BranchName is "advance"

                                                                  InAmount = t1.InAmount,

                                                                  TransactionDate = t1.TransactionDate,
                                                                  MoneyReceiptNo = t1.MoneyReceiptNo,
                                                                  ReferenceNo = t1.ReferenceNo,
                                                                  CreatedBy = t1.CreatedBy,


                                                                  CustomerId = t1.VendorId,
                                                                  CompanyId = t1.CompanyId,
                                                                  PaymentId = t1.PaymentId

                                                              }).OrderByDescending(x => x.PaymentId).AsEnumerable());

            return VMPayment;
        }

        public async Task<long> UpdateSignatoryApproval(int signatoryApprovalId, SignatoryStatusEnum statusEnum, string comments)
        {
            var exist = _context.ApprovalOrderMasters.Find(signatoryApprovalId);
            if (exist == null)
            {
                return 0;
            }

            exist.ApprovalDate = DateTime.Now;
            exist.Comments = comments;
            exist.ApprovalStatus = (int)statusEnum;

            if (await _context.SaveChangesAsync() > 0)
            {

                if (statusEnum == SignatoryStatusEnum.Approved)
                {

                    var orderMaster = _context.OrderMasters.FirstOrDefault(x => x.OrderMasterId == exist.OrderMasterId);
                    if (orderMaster != null)
                    {
                        orderMaster.Status = exist.Precedence == 1 ? (int)EnumFeedSalesStatus.GMApproval : 
                                             exist.Precedence == 2 ? (int)EnumFeedSalesStatus.MDApproval :
                                             exist.Precedence == 3 ? (int)EnumFeedSalesStatus.AccountsApproval:
                                             exist.Precedence == 4 ? (int)EnumFeedSalesStatus.StoreAcceptance
                                             : (int)EnumFeedSalesStatus.Submit;

                        await _context.SaveChangesAsync();
                        await smsservice.orderMasterSms((int)orderMaster.OrderMasterId, false);
                        if (exist.Precedence == 4)
                        {
                            VMFeedPayment vmPayment = await Task.Run(() => FeedPaymentGet(exist.OrderMasterId));
                            await _accountingService.CullectionPushFeed(orderMaster.CompanyId.Value, vmPayment, (int)FeedJournalEnum.CreditVoucher);
                            await smsservice.orderMasterSms((int)orderMaster.OrderMasterId, true);
                        }


                        if (exist.Precedence == 1)
                        {
                            var updateRecevedate = _context.ApprovalOrderMasters.Where(x => x.Precedence == 2 && x.OrderMasterId == orderMaster.OrderMasterId && x.IsActive).FirstOrDefault();
                            updateRecevedate.ReceivedDate = DateTime.Now;
                        }
                        if (exist.Precedence == 2)
                        {
                            var updateRecevedate = _context.ApprovalOrderMasters.Where(x => x.Precedence == 3 && x.OrderMasterId == orderMaster.OrderMasterId && x.IsActive).FirstOrDefault();
                            updateRecevedate.ReceivedDate = DateTime.Now;
                        }
                        if (exist.Precedence == 3)
                        {
                            var updateRecevedate = _context.ApprovalOrderMasters.Where(x => x.Precedence == 4 && x.OrderMasterId == orderMaster.OrderMasterId && x.IsActive).FirstOrDefault();
                            updateRecevedate.ReceivedDate = DateTime.Now;
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                else if (statusEnum == SignatoryStatusEnum.Rejected)
                {
                    var appval = _context.ApprovalOrderMasters.Where(x => x.OrderMasterId == exist.OrderMasterId && x.IsActive).ToList();
                    appval.ForEach(x => x.IsActive = false);

                    var orderMaster = _context.OrderMasters.Find(exist.OrderMasterId);
                    orderMaster.Status = (int)EnumFeedSalesStatus.Draft;
                    await _context.SaveChangesAsync();


                }

                return exist.OrderMasterId;
            }
            return 0;
        }

        public async Task<OrderMasterSignatoryApprovalVM> LoadRejectedOrderData(DateTime? fromDate, DateTime? toDate)
        {

            OrderMasterSignatoryApprovalVM order = new OrderMasterSignatoryApprovalVM();
            order.datalist = await (from approval in _context.ApprovalOrderMasters
                                    join a in _context.OrderMasterSignatories on approval.SalesOrderSignatoryId equals a.SalesOrderSignatoryId
                                    join o in _context.OrderMasters on approval.OrderMasterId equals o.OrderMasterId
                                    join v in _context.Vendors on o.CustomerId equals v.VendorId
                                    join e in _context.Employees on a.EmployeeId equals e.Id
                                    join c in _context.Companies on e.CompanyId equals c.CompanyId
                                    join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                                    join ds in _context.Designations on e.DesignationId equals ds.DesignationId
                                    where !approval.IsActive 
                                        &&  approval.ApprovalStatus == (int)SignatoryStatusEnum.Rejected
                                        && (fromDate == null || o.OrderDate >= fromDate)
                                        && (toDate == null || o.OrderDate <= toDate)

                                    select new OrderMasterSignatoryApprovalVM()
                                    {
                                        CompanyId = o.CompanyId,
                                        ProductType = o.ProductType,
                                        Id = approval.Id,
                                        EmployeeId = e.EmployeeId,
                                        EmployeeName = e.Name,
                                        CompanyName = c.Name,
                                        DepartmentName = d.Name,
                                        DesignationName = ds.Name,
                                        OrderNo = o.OrderNo,
                                        OrderMasterId = o.OrderMasterId,
                                        OrderDate = o.OrderDate,
                                        CoustomerName = v.Name,
                                        ApprovalOrderMasterSignatoryId = a.SalesOrderSignatoryId,
                                        ApprovalDate = approval.ApprovalDate,
                                        Comments = approval.Comments,
                                        Status = approval.ApprovalStatus,
                                        SignatoryStatus = (SignatoryStatusEnum)approval.ApprovalStatus
                                       
                                    }).AsQueryable()
                           
                           .ToListAsync();
            return order;
        }

        public async Task<OrderMasterSignatoryApprovalVM> LoadApprovalData(DateTime? fromDate, DateTime? toDate, SignatoryStatusEnum? status)
        {

            long userId = Common.GetIntUserId();
            //userId = 624;
            //  userId = 139;
            //userId = 1300;
            OrderMasterSignatoryApprovalVM order = new OrderMasterSignatoryApprovalVM();
            order.datalist = await (from approval in _context.ApprovalOrderMasters
                                    join a in _context.OrderMasterSignatories on approval.SalesOrderSignatoryId equals a.SalesOrderSignatoryId
                                    join o in _context.OrderMasters on approval.OrderMasterId equals o.OrderMasterId
                                    join v in _context.Vendors on o.CustomerId equals v.VendorId
                                    join e in _context.Employees on a.EmployeeId equals e.Id
                                    join c in _context.Companies on e.CompanyId equals c.CompanyId
                                    join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                                    join ds in _context.Designations on e.DesignationId equals ds.DesignationId
                                    where a.IsActive && approval.IsActive && a.EmployeeId == userId
                                        && (status == null || approval.ApprovalStatus == (int?)status)
                                        && (fromDate == null || o.OrderDate >= fromDate)
                                        && (toDate == null || o.OrderDate <= toDate)

                                    select new OrderMasterSignatoryApprovalVM()
                                    {
                                        CompanyId = o.CompanyId,
                                        ProductType = o.ProductType,
                                        Id = approval.Id,
                                        EmployeeId = e.EmployeeId,
                                        EmployeeName = e.Name,
                                        CompanyName = c.Name,
                                        DepartmentName = d.Name,
                                        DesignationName = ds.Name,
                                        OrderNo = o.OrderNo,
                                        OrderMasterId = o.OrderMasterId,
                                        OrderDate = o.OrderDate,
                                        CoustomerName = v.Name,
                                        ApprovalOrderMasterSignatoryId = a.SalesOrderSignatoryId,
                                        ApprovalDate = approval.ApprovalDate,
                                        Comments = approval.Comments,
                                        Status = o.Status,
                                        SignatoryStatus = (SignatoryStatusEnum)approval.ApprovalStatus,
                                        IsPreviousApproved = _context.ApprovalOrderMasters.Where(x => x.IsActive)
                                                            .Any(x => x.OrderMasterId == approval.OrderMasterId &&
                                                                    x.Id != approval.Id && (x.ApprovalStatus != (int)SignatoryStatusEnum.Approved) &&
                                                                    _context.OrderMasterSignatories.Where(o => o.IsActive)
                                                                            .Any(y => y.SalesOrderSignatoryId == x.SalesOrderSignatoryId &&
                                                                                    y.Precedence < a.Precedence &&
                                                                                    y.IsActive))
                                    }).Where(x => !x.IsPreviousApproved)
                           .AsQueryable()
                           .OrderBy(x => x.Status)
                           .ThenByDescending(x => x.OrderDate)
                           .ToListAsync();
            return order;
        }

        public async Task<OrderMasterSignatoryApprovalVM> LoadApprovalDataSeed(DateTime? fromDate, DateTime? toDate, SignatoryStatusEnum? status)
        {

            long userId = Common.GetIntUserId();
            //userId = 624;
            //  userId = 139;
            //userId = 1300;
            OrderMasterSignatoryApprovalVM order = new OrderMasterSignatoryApprovalVM();
            order.datalist = await (from approval in _context.ApprovalOrderMasters
                                    join a in _context.OrderMasterSignatories on approval.SalesOrderSignatoryId equals a.SalesOrderSignatoryId
                                    join o in _context.OrderMasters on approval.OrderMasterId equals o.OrderMasterId
                                    join v in _context.Vendors on o.CustomerId equals v.VendorId
                                    join e in _context.Employees on a.EmployeeId equals e.Id
                                    join c in _context.Companies on e.CompanyId equals c.CompanyId
                                    join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                                    join ds in _context.Designations on e.DesignationId equals ds.DesignationId
                                    where a.IsActive && approval.IsActive && a.EmployeeId == userId
                                        && (status == null || approval.ApprovalStatus == (int?)status)
                                        && (fromDate == null || o.OrderDate >= fromDate)
                                        && (toDate == null || o.OrderDate <= toDate)

                                    select new OrderMasterSignatoryApprovalVM()
                                    {
                                        CompanyId = o.CompanyId,
                                        ProductType = o.ProductType,
                                        Id = approval.Id,
                                        EmployeeId = e.EmployeeId,
                                        EmployeeName = e.Name,
                                        CompanyName = c.Name,
                                        DepartmentName = d.Name,
                                        DesignationName = ds.Name,
                                        OrderNo = o.OrderNo,
                                        OrderMasterId = o.OrderMasterId,
                                        OrderDate = o.OrderDate,
                                        CoustomerName = v.Name,
                                        ApprovalOrderMasterSignatoryId = a.SalesOrderSignatoryId,
                                        ApprovalDate = approval.ApprovalDate,
                                        Comments = approval.Comments,
                                        Status = o.Status,
                                        SignatoryStatus = (SignatoryStatusEnum)approval.ApprovalStatus,
                                        IsPreviousApproved = _context.ApprovalOrderMasters.Where(x => x.IsActive)
                                                            .Any(x => x.OrderMasterId == approval.OrderMasterId &&
                                                                    x.Id != approval.Id && (x.ApprovalStatus != (int)SignatoryStatusEnum.Approved) &&
                                                                    _context.OrderMasterSignatories.Where(o => o.IsActive)
                                                                            .Any(y => y.SalesOrderSignatoryId == x.SalesOrderSignatoryId &&
                                                                                    y.Precedence < a.Precedence &&
                                                                                    y.IsActive))
                                    }).Where(x => !x.IsPreviousApproved)
                           .AsQueryable()
                           .OrderBy(x => x.Status)
                           .ThenByDescending(x => x.OrderDate)
                           .ToListAsync();
            return order;
        }
        public async Task<IEnumerable<OrderMasterSignatoryApprovalVM>> GetAllApproval(int orderMasterId)
        {
            var approvalList = await (from approval in _context.ApprovalOrderMasters
                                      join a in _context.OrderMasterSignatories on approval.SalesOrderSignatoryId equals a.SalesOrderSignatoryId
                                      join o in _context.OrderMasters on approval.OrderMasterId equals o.OrderMasterId
                                      join v in _context.Vendors on o.CustomerId equals v.VendorId
                                      join e in _context.Employees on a.EmployeeId equals e.Id
                                      join c in _context.Companies on e.CompanyId equals c.CompanyId
                                      join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                                      join ds in _context.Designations on e.DesignationId equals ds.DesignationId
                                      where approval.IsActive
                                            && approval.OrderMasterId == orderMasterId
                                      select new OrderMasterSignatoryApprovalVM()
                                      {
                                          CompanyId = o.CompanyId,
                                          ProductType = o.ProductType,
                                          Id = approval.Id,
                                          EmployeeId = e.EmployeeId,
                                          ApproverEmployeeIntId = a.EmployeeId.Value,
                                          EmployeeName = e.Name,
                                          CompanyName = c.Name,
                                          DepartmentName = d.Name,
                                          DesignationName = ds.Name,
                                          OrderNo = o.OrderNo,
                                          OrderMasterId = o.OrderMasterId,
                                          OrderDate = o.OrderDate,
                                          CoustomerName = v.Name,
                                          ApprovalOrderMasterSignatoryId = a.SalesOrderSignatoryId,
                                          ApprovalDate = approval.ApprovalDate,
                                          Comments = approval.Comments,
                                          SignatoryStatus = (SignatoryStatusEnum)approval.ApprovalStatus,
                                          IsPreviousApproved = _context.ApprovalOrderMasters.Where(x => x.IsActive)
                                              .Any(x => x.OrderMasterId == approval.OrderMasterId
                                                    && x.Id != approval.Id
                                                    && (x.ApprovalStatus != (int)SignatoryStatusEnum.Approved)
                                                    && _context.OrderMasterSignatories.Where(o => o.IsActive)
                                                        .Any(y => y.SalesOrderSignatoryId == x.SalesOrderSignatoryId
                                                              && y.Precedence < a.Precedence
                                                              && y.IsActive))
                                      }).ToListAsync();

            var SignatoryApprovalList = approvalList.OrderBy(a => a.Precedence).ToList();
            return SignatoryApprovalList;
        }

    }
}
