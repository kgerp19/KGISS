using KGERP.Utility;
using KGERP.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace KGERP.Controllers
{
    [CheckSession]
    public class KgreCostSetupController : Controller
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        public SqlConnection Connection { get; set; }
        public SqlCommand Command { get; set; }
        public SqlDataReader Reader { get; set; }
        public int RowCount { get; set; }
        public string Query { get; set; }
        public KgreCostSetupController()
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

        // GET: KgreCostSetup
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LodeGrvLists()
        {
            KgreCostSetupController aCon = new KgreCostSetupController();
            var list = aCon.LodeGrvList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadFabricsProcByIds(int FabricsProcId)
        {
            KgreCostSetupController aCon = new KgreCostSetupController();
            var list = aCon.LoadFabricsProcById(FabricsProcId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public int SaveFabricsProcTable(string Company, float Rate, string FabricsProcName, string Description, int FabricsProcId)
        {
            if (FabricsProcId.Equals(0))
            {
                int s = 0;
                var FabricsProcAutoId = "";
                try
                {
                    if (Connection.State == ConnectionState.Closed) { Connection.Open(); }

                    Query = @"Insert into KGRECostSetup(Company,Rate,NameofCost) Select @Company,@Rate, @NameofCost";
                    Command = new SqlCommand(Query, Connection);
                    Command.Parameters.Clear();
                    Command.Parameters.AddWithValue("@Company", Company);
                    Command.Parameters.AddWithValue("@Rate", Rate);
                    Command.Parameters.AddWithValue("@NameofCost", FabricsProcName);
                    Command.ExecuteNonQuery();
                    if (Connection.State == ConnectionState.Open) { Connection.Close(); }
                }
                catch (Exception e)
                {

                }
                return s;
            }
            else
            {
                KgreCostSetupController aCon = new KgreCostSetupController();
                aCon.FinalEditFabricsProcData(Company, Rate, FabricsProcName, Description, FabricsProcId);
                return 0;
            }

        }

        public int FinalEditFabricsProcData(string Company, float Rate, string NameofCost, string Description, int FabricsProcId)
        {
            int s = 0;
            var FabricsProcAutoId = "";
            try
            {
                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
                Query = @"Update KGRECostSetup set NameofCost=@NameofCost,Company=@Company,Rate=@Rate where FabricsProcId=@FabricsProcId";
                Command = new SqlCommand(Query, Connection);
                Command.Parameters.Clear();
                Command.Parameters.AddWithValue("@NameofCost", NameofCost);
                Command.Parameters.AddWithValue("@Company", Company);
                Command.Parameters.AddWithValue("@Rate", Rate);
                Command.Parameters.AddWithValue("@FabricsProcId", FabricsProcId);
                Command.ExecuteNonQuery();
                if (Connection.State == ConnectionState.Open) { Connection.Close(); }

                return s;

            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public List<KgreCostSetupModel> LodeGrvList()
        {
            List<KgreCostSetupModel> aListKgreCostSetupModel = new List<KgreCostSetupModel>();
            if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
            Query = @"Select Rate, NameofCost,FabricsProcId  from KGRECostSetup as fbp";
            Command = new SqlCommand(Query, Connection);
            Command.Parameters.Clear();
            Reader = Command.ExecuteReader();
            while (Reader.Read())
            {
                KgreCostSetupModel aKgreCostSetupModel = new KgreCostSetupModel();
                aKgreCostSetupModel.FabricsProcId = Convert.ToInt32(Reader["FabricsProcId"]);
                aKgreCostSetupModel.NameofCost = Reader["NameofCost"].ToString();
                aKgreCostSetupModel.Company = "Krishi";
                aKgreCostSetupModel.Rate = Convert.ToInt64(Reader["Rate"].ToString());
                aListKgreCostSetupModel.Add(aKgreCostSetupModel);

            }
            if (Connection.State == ConnectionState.Open) { Connection.Close(); }


            return aListKgreCostSetupModel;
        }

        public int DeleteRecord(long comanyId)
        {

            if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
            Query = @"delete KGRECostSetup where FabricsProcId=@comanyId";
            Command = new SqlCommand(Query, Connection);
            Command.Parameters.Clear();
            Command.Parameters.AddWithValue("@comanyId", comanyId);
            Reader = Command.ExecuteReader();

            if (Connection.State == ConnectionState.Open) { Connection.Close(); }
            return 0;
        }

        public KgreCostSetupModel LoadFabricsProcById(int FabricsProcId)
        {
            KgreCostSetupModel aFabricsProc = new KgreCostSetupModel();
            try
            {
                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
                Query = "select FabricsProcId,NameofCost,Company,Rate from KGRECostSetup where FabricsProcId=@id";
                Command = new SqlCommand(Query, Connection);
                Command.Parameters.Clear();
                Command.Parameters.AddWithValue("@id", FabricsProcId);
                Reader = Command.ExecuteReader();
                while (Reader.Read())
                {


                    aFabricsProc.FabricsProcId = Convert.ToInt32(Reader["FabricsProcId"].ToString());
                    aFabricsProc.NameofCost = Reader["NameofCost"].ToString();
                    aFabricsProc.Company = Reader["Company"].ToString();
                    aFabricsProc.Rate = Convert.ToInt64(Reader["Rate"].ToString());


                }
            }
            catch { }

            if (Connection.State == ConnectionState.Open) { Connection.Close(); }
            return aFabricsProc;
        }



    }
}
