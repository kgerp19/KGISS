using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.DataLib
{
    public class APIDataAccess
    {

        public SqlConnection m_Conn;
        private Hashtable m_Erroobj;
        public APIDataAccess()
        {
            m_Conn = new SqlConnection(this.MSSQLConnStrWeb("SQLDBConnection")); // Its Works//18.12.2021
            m_Erroobj = new Hashtable();
        }
        public APIDataAccess(string ConnStr, string ConnType = "API") // ConnType="Web", ConnType="WPF", ConnType="Fixed"
        {
            switch (ConnType)
            {
                case "Fixed":
                    m_Conn = new SqlConnection(ConnStr);
                    break;
                //case "Web":
                //    m_Conn = new SqlConnection(this.MSSQLConnStrWeb(ConnStr));
                //    break;
                case "API":
                    m_Conn = new SqlConnection(this.MSSQLConnStrWeb(ConnStr));
                    break;
                    //case "WPF":
                    //    m_Conn = new SqlConnection(this.MSSQLConnStrWPF(ConnStr));
                    //    break;
            }
            m_Erroobj = new Hashtable();
        }
        //private String MSSQLConnStrAPI(string ConnStr1)// .net core mvc
        //{
        //    string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
        //    IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(projectPath).AddJsonFile("appsettings.json").Build();
        //    string ii = configuration.GetConnectionString(ConnStr1);
        //    //IConfiguration configuration ;
        //    //string ii = configuration.GetConnectionString("TestDB")// System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath.ToString();

        //    //string ii = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath.ToString();
        //    //System.Configuration.Configuration Config1 = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(ii);
        //    //ii = Config1.ConnectionStrings.ConnectionStrings[ConnStr1].ToString().Trim();
        //    SqlConnectionStringBuilder Builder1 = new SqlConnectionStringBuilder(ii);
        //    ii = Builder1.ConnectionString;
        //    return ii;
        //    //return "";
        //}
        private String MSSQLConnStrWeb(string ConnStr1)//for .net framwork mvc
        {
            string ii = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath.ToString();
            System.Configuration.Configuration Config1 = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(ii);
            ii = Config1.ConnectionStrings.ConnectionStrings[ConnStr1].ToString().Trim();
            SqlConnectionStringBuilder Builder1 = new SqlConnectionStringBuilder(ii);
            ii = Builder1.ConnectionString;
            return ii;
        }
        //private String MSSQLConnStrWPF(string ConnStr1)
        //{
        //    string ii = System.IO.Path.Combine(System.Environment.CurrentDirectory, System.Windows.Forms.Application.ProductName + ".EXE");
        //    System.Configuration.Configuration Config1 = System.Configuration.ConfigurationManager.OpenExeConfiguration(ii);
        //    ii = Config1.ConnectionStrings.ConnectionStrings[ConnStr1].ToString().Trim();
        //    SqlConnectionStringBuilder Builder1 = new SqlConnectionStringBuilder(ii);
        //    ii = Builder1.ConnectionString;
        //    return ii;
        //}

        public Hashtable ErrorObject
        {
            get
            {
                return this.m_Erroobj;
            }
        }
        public DataTable GetTable(string SQL)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = SQL;
                Cmd.Connection = this.m_Conn;
                Cmd.CommandTimeout = 120;
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = Cmd;
                DataTable dt = new DataTable();
                adp.Fill(dt);
                return dt;
            }

            catch (Exception ex)
            {
                this.SetError(ex);
                return null;
            }
        }

        public DataSet GetDataSet(String SQL)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandType = CommandType.Text;
                Cmd.CommandText = SQL;
                Cmd.Connection = this.m_Conn;
                Cmd.CommandTimeout = 120;
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = Cmd;
                DataSet ds = new DataSet();
                adp.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                this.SetError(ex);
                return null;
            }
        }

        public DataTable GetTable(SqlCommand Cmd)
        {
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = Cmd;
                Cmd.Connection = this.m_Conn;
                Cmd.CommandTimeout = 120;
                DataTable dt = new DataTable();
                adp.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                this.SetError(ex);
                return null;
            }
        }

        public DataSet GetDataSet(SqlCommand Cmd)
        {
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter();
                adp.SelectCommand = Cmd;
                Cmd.Connection = this.m_Conn;
                Cmd.CommandTimeout = 120;
                DataSet ds = new DataSet();
                adp.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                this.SetError(ex);
                this.m_Conn.Close();
                return null;
            }
        }

        public Boolean ExecuteCommand(string SQL)
        {
            SqlCommand Cmd = new SqlCommand();
            Cmd.CommandText = SQL;
            Cmd.Connection = this.m_Conn;
            Cmd.CommandTimeout = 120;
            try
            {
                this.m_Conn.Open();
                Cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                this.SetError(ex);
                return false;
            }
            finally
            {
                this.m_Conn.Close();
            }
        }

        public Boolean ExecuteCommand(SqlCommand Cmd)
        {
            Cmd.Connection = this.m_Conn;
            try
            {
                this.m_Conn.Open();
                Cmd.CommandTimeout = 120;
                Cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                this.SetError(ex);
                return false;
            }
            finally
            {
                this.m_Conn.Close();
            }
        }

        public SqlDataReader ExecuteReader(SqlCommand cmd)
        {
            cmd.Connection = this.m_Conn;
            cmd.CommandTimeout = 120;
            try
            {
                this.m_Conn.Close();
                this.m_Conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                this.SetError(ex);
                return null;
            }
        }
        private void SetError(Exception ex)
        {
            this.m_Erroobj["Src"] = ex.Source;
            this.m_Erroobj["Msg"] = ex.Message;
            this.m_Erroobj["Location"] = ex.StackTrace;
        }
    }
}

