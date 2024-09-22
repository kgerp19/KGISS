using KGERP.Data.Models;
using KGERP.Service.Interface;
using KGERP.Service.ServiceModel;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation
{
    public class KGREPaymentInfoService : IKGREPaymentInfoService
    {
        private readonly ERPEntities context = new ERPEntities();
        //public KGREPaymentInfoService(ERPEntities context)
        //{
        //    this.context = context;
        //}
        #region Old Code
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        public SqlConnection Connection { get; set; }
        public SqlCommand Command { get; set; }
        public SqlDataReader Reader { get; set; }
        public int RowCount { get; set; }
        public string Query { get; set; }
        public KGREPaymentInfoService()
        {
            Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            Command = new SqlCommand();
            Command.Connection = Connection;
        }
        public SqlCommand MyCommand(string command, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = command;
            cmd.Connection = connection;
            cmd.CommandTimeout = 10000;
            return cmd;
        }
        public PlotBooking LoadPayInfoById(string Autoid)
        {
            PlotBooking aRicive = new PlotBooking();
            if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
            Query = @"select top 1 InstallMentAmount,NoOfInstallment, BokkingMoney as LastPayAmount, Booking_Date as LastPayDate,(select sum(BokkingMoney) from PlotBooking where ClientAutoId=@Autoid) as TotalPayAmount,((select max(GrandTotal) from PlotBooking where  ClientAutoId=@Autoid)-(select sum(BokkingMoney) from PlotBooking where ClientAutoId=@Autoid)) as DueAmount from PlotBooking where ClientAutoId=@Autoid order by booking_id desc";
            Command = new SqlCommand(Query, Connection);
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("@Autoid", Autoid);
            Reader = Command.ExecuteReader();
            while (Reader.Read())
            {
                aRicive.InstallMentAmount = Convert.ToInt64(Reader["InstallMentAmount"]);
                aRicive.NoOfInstallment = Convert.ToInt32(Reader["NoOfInstallment"]);
                aRicive.BokkingMoney = Convert.ToInt64(Reader["LastPayAmount"]);
                aRicive.Booking_Date = Convert.ToDateTime(Reader["LastPayDate"]);
                aRicive.GrandTotal = Convert.ToInt64(Reader["TotalPayAmount"]);
                aRicive.RestOfAmount = Convert.ToInt64(Reader["DueAmount"]);
            }
            if (Connection.State == ConnectionState.Open) { Connection.Close(); }
            return aRicive;
        }
        public object LoadClientPayInfoById(string id)
        {
            ERPEntities db = new ERPEntities();
            object BasicInfo = null;
            BasicInfo = (from basic in db.ClientsInfoes
                         join plo in db.KGREProjects on basic.Cli_ProjectName equals plo.ProjectId
                         where basic.ClientsAutoId == id

                         select new
                         {
                             id = basic.Cli_id,
                             basic.Cli_ProjectName,
                             basic.ClientsAutoId,
                             basic.Cli_BlockNo,
                             basic.Cli_PlotNo,
                             basic.Cli_PlotSize,
                             basic.Cli_Facing,
                             basic.Cli_Date,
                             plo.ProjectName
                         });

            return BasicInfo;
        }
        public void SaveOfficeInfo(PlotBooking ClientOffInfo)
        {
            ERPEntities db = new ERPEntities();
            PlotBooking clientBookModel = new PlotBooking();
            clientBookModel.ClientAutoId = ClientOffInfo.ClientAutoId;
            clientBookModel.BlockNo = ClientOffInfo.BlockNo;
            clientBookModel.PloatNo = ClientOffInfo.PloatNo;
            clientBookModel.PloatSize = ClientOffInfo.PloatSize;
            clientBookModel.Facing = ClientOffInfo.Facing;
            clientBookModel.PlotSize = ClientOffInfo.PlotSize;
            clientBookModel.LandPricePerKatha = ClientOffInfo.LandPricePerKatha;
            clientBookModel.LandValue = ClientOffInfo.LandValue;
            clientBookModel.Discount = ClientOffInfo.Discount;
            clientBookModel.LandValueAfterDiscount = ClientOffInfo.LandValueAfterDiscount;
            clientBookModel.AdditionalCost = ClientOffInfo.AdditionalCost;
            clientBookModel.UtilityCost = ClientOffInfo.UtilityCost;
            clientBookModel.GrandTotal = ClientOffInfo.GrandTotal;
            clientBookModel.BokkingMoney = ClientOffInfo.BokkingMoney;
            clientBookModel.RestOfAmount = ClientOffInfo.RestOfAmount;
            clientBookModel.OneTime = ClientOffInfo.OneTime;
            clientBookModel.InstallMent = ClientOffInfo.InstallMent;
            clientBookModel.NoOfInstallment = ClientOffInfo.NoOfInstallment;
            clientBookModel.InstallMentAmount = ClientOffInfo.InstallMentAmount;
            //clientBookModel.Booking_Date = ClientOffInfo.Booking_Date;
            db.PlotBookings.Add(clientBookModel);
            db.SaveChanges();
        }
        public List<DateTime> CalculateNextPayDateFrom1(string id)
        {
            ERPEntities db = new ERPEntities();
            int pp = 30;
            var nRow = (from p in db.PlotBookings
                        where p.ClientAutoId == id
                        select p.NoOfInstallment).Max();

            var date = (from p in db.PlotBookings
                        where p.ClientAutoId == id
                        select p.Booking_Date).FirstOrDefault();

            DateTime dateTime = Convert.ToDateTime(date);

            DateTime date1 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            var day = Convert.ToInt32(nRow) * pp;

            List<DateTime> dateTimes = new List<DateTime>();
            var firstEverPayment = date1;
            var nextPayment = firstEverPayment;

            while (day != 0)
            {
                //day = 600 / 30;
                nextPayment += new TimeSpan(30, 0, 0, 0);
                dateTimes.Add(nextPayment);
                day -= pp;
            }
            return dateTimes;
        }
        public object ClientPaymentHistory(string id)
        {
            ERPEntities db = new ERPEntities();
            var count = db.PlotBookings.Where(me => me.ClientAutoId == id).Count();
            object BasicInfo = null;

            BasicInfo = (from basic in db.PlotBookings
                         where basic.ClientAutoId == id
                         select new
                         {
                             basic.GrandTotal,
                             basic.booking_id,
                             basic.BokkingMoney,
                             basic.InstallMentAmount,
                             basic.Booking_Date,
                             basic.NoOfInstallment,
                             paidInstalment = count
                         }).OrderBy(u => u.booking_id).Skip(1).ToList();

            return BasicInfo;
        }
        public void Edit(float PayDueAmount, int Id)
        {
            ERPEntities db = new ERPEntities();
            var Paydue = (from p in db.PlotBookings
                          where p.booking_id == Id
                          select p.BokkingMoney).FirstOrDefault();
            var money = Paydue + PayDueAmount;
            PlotBooking clientBookModel = new PlotBooking();
            clientBookModel = (from i in db.PlotBookings
                               where i.booking_id == Id
                               select i).FirstOrDefault();
            clientBookModel.BokkingMoney = money;
            db.SaveChanges();
        }
        #endregion

        #region KGRE Payment Model
        public List<KGREPaymentModel> GetPaymentsByCustomer(int customerId)
        {
            List<SelectModel> paymentMethods = Enum.GetValues(typeof(KgRePaymentMethod)).Cast<KgRePaymentMethod>().Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt32(v) }).ToList();
            List<KGREPayment> payments = context.KGREPayments.Include("Vendor").Where(x => x.ClientId == customerId).ToList();
            List<KGREPaymentModel> paymentModels = ObjectConverter<KGREPayment, KGREPaymentModel>.ConvertList(payments).ToList();

            paymentModels = paymentModels.Select(p => new KGREPaymentModel
            {
                PaymentId = p.PaymentId,
                ClientId = p.ClientId,
                InAmount = p.InAmount,
                TransactionDate = p.TransactionDate
            }).OrderByDescending(x => x.PaymentId).ToList();
            return paymentModels;
        }
        public async Task<KGREPaymentModel> GetPayment(int id)
        {
            if (id <= 0)
            {
                return new KGREPaymentModel() { TransactionDate = DateTime.Now };
            }
            KGREPayment payment = context.KGREPayments.Find(id);
            return ObjectConverter<KGREPayment, KGREPaymentModel>.Convert(payment);
        }
        public List<SelectModel> GetPaymentMethodSelectModels()
        {
            List<SelectModel> paymentMethods = Enum.GetValues(typeof(KgRePaymentMethod)).Cast<KgRePaymentMethod>().Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt32(v) }).ToList();
            return paymentMethods;
        }

        public List<KGREPaymentModel> GetPayments(string searchDate, string searchText, int companyId)
        {

            dynamic result = context.Database.SqlQuery<KGREPaymentModel>("KGRE_GetPayment").ToList();
            return result;

            //List<KGREPayment> payments = context.KGREPayments.ToList();
            //List<KGREPaymentModel> paymentModels = ObjectConverter<KGREPayment, KGREPaymentModel>.ConvertList(payments).ToList();

            //return paymentModels;
            //DateTime? dateSearch = null;
            //dateSearch = !string.IsNullOrEmpty(searchDate) ? Convert.ToDateTime(searchDate) : dateSearch;

            //List<KGREPaymentModel> payments = context.Database.SqlQuery<KGREPaymentModel>("spGetCustomerPayments {0}", companyId).ToList();

            //if (dateSearch == null)
            //{
            //    return payments.Where(x => (x.BankName.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
            //                         (x.PaymentMode.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
            //                         (x.ChkName.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
            //                         (x.FileNo.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText))
            //                        ).OrderByDescending(x => x.PaymentId).ToList();
            //}
            //if (string.IsNullOrEmpty(searchText) && dateSearch != null)
            //{
            //    return payments.Where(x => x.TransactionDate == dateSearch).OrderByDescending(x => x.PaymentId).ToList();
            //}


            //return payments.Where(x => x.TransactionDate == dateSearch &&
            //                         ((x.BankName.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) || 
            //                         (x.PaymentMode.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
            //                         (x.FileNo.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText)) ||
            //                         (x.ChkName.ToLower().Contains(searchText.ToLower()) || String.IsNullOrEmpty(searchText))
            //                        )).OrderByDescending(x => x.PaymentId).ToList();

        }

        public bool SavePayment(int id, KGREPaymentModel model, out string message)
        {
            message = string.Empty;
            KGREPayment payment = ObjectConverter<KGREPaymentModel, KGREPayment>.Convert(model);
            if (id > 0)
            {
                payment = context.KGREPayments.Where(x => x.PaymentId == id).FirstOrDefault();
                if (payment == null)
                {
                    throw new Exception("Data not found!");
                }
                payment.ModifiedDate = DateTime.Now;
                payment.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            }
            else
            {
                payment.CreatedDate = DateTime.Now;
                payment.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
                //payment.IsActive = true;
            }
            payment.PlotId = model.PlotId;
            payment.InstallmentId = model.InstallmentId;
            payment.FileNo = model.FileNo;
            payment.InAmount = model.InAmount;

            if (payment.TransactionType == "CR")
            {
                payment.OutAmount = model.InAmount;
                payment.InAmount = 0;
            }
            payment.TransactionDate = model.TransactionDate;

            payment.BankId = model.BankId;
            payment.CompanyId = model.CompanyId;
            payment.ProjectId = model.ProjectId;
            payment.ChkDate = model.ChkDate;
            payment.ChkName = model.ChkName;
            payment.BranchName = model.BranchName;
            payment.ChkNo = model.ChkNo;

            context.Entry(payment).State = payment.PaymentId == 0 ? EntityState.Added : EntityState.Modified;
            try
            {
                if (context.SaveChanges() > 0)
                {
                    message = "Payment completed successfully";

                    KGREPlotBooking booking = context.KGREPlotBookings.Where(x => x.FileNo == payment.FileNo).FirstOrDefault();
                    if (payment.TransactionType == "CR")
                    {
                        if (payment.PaymentFor == 1501) //1501    Land Value
                        {
                            booking.LandValue = booking.LandValue + payment.OutAmount;
                        }
                        else if (payment.PaymentFor == 1502) //1502    Additional 25 %
                        {
                            booking.Additional25Percent = booking.Additional25Percent + payment.OutAmount;
                        }
                        else if (payment.PaymentFor == 1503) //1503    Additional 15 %
                        {
                            booking.Additional15Percent = booking.Additional15Percent + payment.OutAmount;
                        }
                        else if (payment.PaymentFor == 1504) //1504    Additional 10 %
                        {
                            booking.Additional10Percent = booking.Additional10Percent + payment.OutAmount;
                        }
                        else if (payment.PaymentFor == 1505) //1505    Utility Cost
                        {
                            booking.UtilityCost = booking.UtilityCost + payment.OutAmount;
                        }
                        else if (payment.PaymentFor == 1506) //1506    Registration
                        {
                            booking.RegAmount = booking.RegAmount + payment.OutAmount;
                        }
                        else if (payment.PaymentFor == 1507) //1507    Mutation
                        {
                            booking.MutationCost = booking.MutationCost + payment.OutAmount;
                        }
                        else if (payment.PaymentFor == 1508) //1508    Boundary Wall
                        {
                            booking.BoundaryWall = booking.BoundaryWall + payment.OutAmount;
                        }
                        else if (payment.PaymentFor == 1509) //1509    Name Plate
                        {
                            booking.NamePlate = booking.NamePlate + payment.OutAmount;
                        }
                        else if (payment.PaymentFor == 1510) //1510    Tree Plantation
                        {
                            if(booking.TreePlantation==null)
                            {
                                booking.TreePlantation =  payment.OutAmount;
                            }
                            else
                            {
                                booking.TreePlantation = booking.TreePlantation + payment.OutAmount;
                            }                         
                        }
                        else if (payment.PaymentFor == 1511) //1511    BS Survey
                        {
                            booking.BSSurvey = booking.BSSurvey + payment.OutAmount;
                        }
                        else if (payment.PaymentFor == 1512) //1512    Security Fee
                        {
                            booking.SecurityService = booking.BSSurvey + payment.OutAmount;
                        }
                        else if (payment.PaymentFor == 1513)   //1513    Add Delay Fine
                        {
                            booking.AddDelayFine = booking.BSSurvey + payment.OutAmount;
                        }
                        else
                        {
                            booking.OtharCostName = booking.OtharCostName + payment.OutAmount;
                        }
                    }
                    else
                    {
                        if (payment.PaymentFor == 1501) //1501    Land Value
                        {
                            booking.LandValueR = booking.LandValueR + payment.InAmount;
                        }
                        else if (payment.PaymentFor == 1502) //1502    Additional 25 %
                        {
                            booking.Additional25PercentR = booking.Additional25PercentR + payment.InAmount;
                        }
                        else if (payment.PaymentFor == 1503) //1503    Additional 15 %
                        {
                            booking.Additional15PercentR = booking.Additional15PercentR + payment.InAmount;
                        }
                        else if (payment.PaymentFor == 1504) //1504    Additional 10 %
                        {
                            booking.Additional10PercentR = booking.Additional10PercentR + payment.InAmount;
                        }
                        else if (payment.PaymentFor == 1505) //1505    Utility Cost
                        {
                            booking.UtilityCostR = booking.UtilityCostR + payment.InAmount;
                        }
                        else if (payment.PaymentFor == 1506) //1506    Registration
                        {
                            booking.RegAmountR = booking.RegAmountR + payment.InAmount;
                        }
                        else if (payment.PaymentFor == 1507) //1507    Mutation
                        {
                            booking.MutationCostR = booking.MutationCostR + payment.InAmount;
                        }
                        else if (payment.PaymentFor == 1508) //1508    Boundary Wall
                        {
                            booking.BoundaryWallR = booking.BoundaryWallR + payment.InAmount;
                        }
                        else if (payment.PaymentFor == 1509) //1509    Name Plate
                        {
                            booking.NamePlateR = booking.NamePlateR + payment.InAmount;
                        }
                        else if (payment.PaymentFor == 1510) //1510    Tree Plantation
                        {
                            booking.TreePlantationR = booking.TreePlantationR + payment.InAmount;
                        }
                        else if (payment.PaymentFor == 1511) //1511    BS Survey
                        {
                            booking.BSSurveyR = booking.BSSurveyR + payment.InAmount;
                        }
                        else if (payment.PaymentFor == 1512) //1512    Security Fee
                        {
                            booking.SecurityServiceR = booking.SecurityServiceR + payment.InAmount;
                        }
                        else if (payment.PaymentFor == 1513)   //1513    Add Delay Fine
                        {
                            booking.AddDelayFineR = booking.AddDelayFineR + payment.InAmount;
                        }
                        else
                        {
                            booking.OtharCostNameR = booking.OtharCostNameR + payment.InAmount;
                        }
                    }
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        context.KGREPayments.Remove(payment);
                        context.SaveChanges();
                        return false;
                    }

                    //KGREPlotBooking booking = context.KGREPlotBookings.Where(x => x.FileNo == payment.FileNo).FirstOrDefault();
                    //booking.Due = (booking.Due - payment.InAmount);
                    //try
                    //{
                    //    context.SaveChanges();
                    //}
                    //catch (Exception ex)
                    //{
                    //    context.KGREPayments.Remove(payment);
                    //    context.SaveChanges();
                    //    return false;
                    //}

                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;

            }
            return true;
        }

        public bool SaveCustomer(int id, KgReCrmModel vendor, out string message)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
