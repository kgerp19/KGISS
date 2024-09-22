using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class KGREClientInstallmentListController : Controller
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        public SqlConnection Connection { get; set; }
        public SqlCommand Command { get; set; }
        public SqlDataReader Reader { get; set; }
        public int RowCount { get; set; }
        public string Query { get; set; }
        public KGREClientInstallmentListController()
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



        // GET: KGREClientInstallmentList
        public ActionResult Index()
        {
            return View();

        }

        // GET: KGREClientInstallmentList/Details/5
        public ActionResult ClientsPaymentList()
        {
            KGREClientInstallmentListController acon = new KGREClientInstallmentListController();

            return Json(acon.ClientBookingInfo(), JsonRequestBehavior.AllowGet);
        }

        public List<ClientsInfo> ClientBookingInfo()
        {
            List<ClientsInfo> aRiciveList = new List<ClientsInfo>();
            if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
            //Query = @"select clie.ClientsAutoId,clie.Cli_ProjectName,clie.Cli_BlockNo,clie.Cli_PlotNo,clie.Cli_PlotSize,clie.Cli_Facing,clie.Cli_id,clie.Cli_Date  from  tbl_clientsInfo as clie where clie.ClientsAutoId NOT IN ( SELECT ClientAutoId  FROM tbl_PlotBooking ) ";
            Query = @"select bsci.FullName,bsci.PresentAddress,clie.ClientsAutoId,clie.Cli_ProjectName,clie.Cli_BlockNo,clie.Cli_PlotNo,clie.Cli_PlotSize,clie.Cli_Facing,clie.Cli_id,clie.Cli_Date  from  ClientsInfo as clie left join KGRECustomer as bsci on clie.ClientsAutoId=bsci.ClientId";
            Command = new SqlCommand(Query, Connection);
            Command.Parameters.Clear();
            Reader = Command.ExecuteReader();
            while (Reader.Read())
            {


                ClientsInfo aRicive = new ClientsInfo();
                aRicive.Cli_id = (Int64)Reader["Cli_id"];
                aRicive.ClientName = Reader["FullName"].ToString();
                aRicive.PresentAddress = Reader["PresentAddress"].ToString();
                aRicive.Cli_ProjectName = Convert.ToInt32(Reader["Cli_ProjectName"].ToString());
                aRicive.ClientsAutoId = Reader["ClientsAutoId"].ToString();
                aRicive.Cli_BlockNo = Reader["Cli_BlockNo"].ToString();
                aRicive.Cli_PlotNo = Reader["Cli_PlotNo"].ToString();
                aRicive.Cli_PlotSize = Reader["Cli_PlotSize"].ToString();
                aRicive.Cli_Facing = Reader["Cli_Facing"].ToString();
                //aRicive.Cli_Date = Convert.ToDateTime(Reader["Cli_Date"].ToString());
                aRiciveList.Add(aRicive);

            }
            if (Connection.State == ConnectionState.Open) { Connection.Close(); }

            return aRiciveList;
        }

        // GET: KGREClientInstallmentList/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KGREClientInstallmentList/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: KGREClientInstallmentList/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: KGREClientInstallmentList/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: KGREClientInstallmentList/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: KGREClientInstallmentList/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
