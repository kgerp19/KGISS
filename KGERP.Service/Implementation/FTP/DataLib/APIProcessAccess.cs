using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using KGERP.FunLib;

namespace KGERP.Service.DataLib
{
    public class APIProcessAccess
    {
        APIDataAccess _dataAccess;
        private Hashtable _errObj;

        public Hashtable ErrorObject
        {
            get
            {
                return this._errObj;
            }
        }

        public APIProcessAccess()
        {
            _dataAccess = new APIDataAccess();
            _errObj = new Hashtable();
        }
        public APIProcessAccess(string ConnStr, string ConnType = "API") // ConnType="Web", ConnType="WPF", ConnType="Fixed"
        {
            _dataAccess = new APIDataAccess(ConnStr, ConnType);
            _errObj = new Hashtable();
        }
        private void SetError(Exception exp)
        {
            this._errObj["Src"] = exp.Source;
            this._errObj["Msg"] = exp.Message;
            this._errObj["Location"] = exp.StackTrace;
        }
        private void SetError(Hashtable errObject)
        {
            this._errObj["Src"] = errObject["Src"];
            this._errObj["Msg"] = errObject["Msg"];
            this._errObj["Location"] = errObject["Location"];
        }
        private void ClearErrors()
        {
            this._errObj["Src"] = string.Empty;
            this._errObj["Msg"] = string.Empty;
            this._errObj["Location"] = string.Empty;
        }

        public DataSet GetSqlResult(ProcessAccessParams pap1)
        {
            try
            {
                this.ClearErrors();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = pap1.ProcName;  // "e.g. SP_ENTRY_PROJECT_CODEBOOK";
                cmd.CommandType = CommandType.StoredProcedure;
                //     cmd.Parameters.Add(new SqlParameter("@ComCod", pap1.comCod));
                cmd.Parameters.Add(new SqlParameter("@ProcessID", pap1.ProcID));
                //cmd.Parameters.Add("@Dxml01", SqlDbType.Xml).Value = (pap1.parmXml01 == null ? null : pap1.parmXml01.GetXml());
                //cmd.Parameters.Add("@Dxml02", SqlDbType.Xml).Value = (pap1.parmXml02 == null ? null : pap1.parmXml02.GetXml());
                //cmd.Parameters.Add(new SqlParameter("@Dbin01", pap1.parmBin01));

                cmd.Parameters.Add(new SqlParameter("@Pxml01", pap1.pXml01));
                cmd.Parameters.Add(new SqlParameter("@Pxml02", pap1.pXml02));
                cmd.Parameters.Add(new SqlParameter("@Pbin01", pap1.parmBin01));


                cmd.Parameters.Add(new SqlParameter("@Param01", pap1.parm01));
                cmd.Parameters.Add(new SqlParameter("@Param02", pap1.parm02));
                cmd.Parameters.Add(new SqlParameter("@Param03", pap1.parm03));
                cmd.Parameters.Add(new SqlParameter("@Param04", pap1.parm04));
                cmd.Parameters.Add(new SqlParameter("@Param05", pap1.parm05));
                cmd.Parameters.Add(new SqlParameter("@Param06", pap1.parm06));
                cmd.Parameters.Add(new SqlParameter("@Param07", pap1.parm07));
                cmd.Parameters.Add(new SqlParameter("@Param08", pap1.parm08));
                cmd.Parameters.Add(new SqlParameter("@Param09", pap1.parm09));
                cmd.Parameters.Add(new SqlParameter("@Param10", pap1.parm10));
                cmd.Parameters.Add(new SqlParameter("@Param11", pap1.parm11));
                cmd.Parameters.Add(new SqlParameter("@Param12", pap1.parm12));
                cmd.Parameters.Add(new SqlParameter("@Param13", pap1.parm13));
                cmd.Parameters.Add(new SqlParameter("@Param14", pap1.parm14));
                cmd.Parameters.Add(new SqlParameter("@Param15", pap1.parm15));
                cmd.Parameters.Add(new SqlParameter("@Param16", pap1.parm16));
                cmd.Parameters.Add(new SqlParameter("@Param17", pap1.parm17));
                cmd.Parameters.Add(new SqlParameter("@Param18", pap1.parm18));
                cmd.Parameters.Add(new SqlParameter("@Param19", pap1.parm19));
                cmd.Parameters.Add(new SqlParameter("@Param20", pap1.parm20));

                cmd.Parameters.Add(new SqlParameter("@EntID", "0102"));
                DataSet result = _dataAccess.GetDataSet(cmd);
                if (result == null)//_result==false
                {
                    this.SetError(_dataAccess.ErrorObject);
                }
                return result;
            }
            catch (Exception exp)
            {
                this.SetError(exp);
                return null;
            }
        }

        public SqlDataReader ReadSqlResult(ProcessAccessParams pap1)
        {
            try
            {
                this.ClearErrors();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = pap1.ProcName;  // "e.g. SP_ENTRY_PROJECT_CODEBOOK";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@EntID", pap1.EntID));
                cmd.Parameters.Add(new SqlParameter("@ProcessID", pap1.ProcID));




                cmd.Parameters.Add(new SqlParameter("@Param01", pap1.parm01));
                cmd.Parameters.Add(new SqlParameter("@Param02", pap1.parm02));
                cmd.Parameters.Add(new SqlParameter("@Param03", pap1.parm03));
                cmd.Parameters.Add(new SqlParameter("@Param04", pap1.parm04));
                cmd.Parameters.Add(new SqlParameter("@Param05", pap1.parm05));
                cmd.Parameters.Add(new SqlParameter("@Param06", pap1.parm06));
                cmd.Parameters.Add(new SqlParameter("@Param07", pap1.parm07));
                cmd.Parameters.Add(new SqlParameter("@Param08", pap1.parm08));
                cmd.Parameters.Add(new SqlParameter("@Param09", pap1.parm09));
                cmd.Parameters.Add(new SqlParameter("@Param10", pap1.parm10));
                cmd.Parameters.Add(new SqlParameter("@Param11", pap1.parm11));
                cmd.Parameters.Add(new SqlParameter("@Param12", pap1.parm12));
                cmd.Parameters.Add(new SqlParameter("@Param13", pap1.parm13));
                cmd.Parameters.Add(new SqlParameter("@Param14", pap1.parm14));
                cmd.Parameters.Add(new SqlParameter("@Param15", pap1.parm15));
                cmd.Parameters.Add(new SqlParameter("@Param16", pap1.parm16));
                cmd.Parameters.Add(new SqlParameter("@Param17", pap1.parm17));
                cmd.Parameters.Add(new SqlParameter("@Param18", pap1.parm18));
                cmd.Parameters.Add(new SqlParameter("@Param19", pap1.parm19));
                cmd.Parameters.Add(new SqlParameter("@Param20", pap1.parm20));
                SqlDataReader result = _dataAccess.ExecuteReader(cmd);
                if (result == null)//_result==false
                {
                    this.SetError(_dataAccess.ErrorObject);
                }
                return result;
            }
            catch (Exception exp)
            {
                this.SetError(exp);
                return null;
            }// 
        }

        public Boolean ExecuteSQLCommand(ProcessAccessParams pap1)
        {
            try
            {
                this.ClearErrors();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = pap1.ProcName;  // "e.g. SP_ENTRY_PROJECT_CODEBOOK";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@EntID", pap1.EntID));
                cmd.Parameters.Add(new SqlParameter("@ProcessID", pap1.ProcID));
                //cmd.Parameters.Add("@dxml01", SqlDbType.Xml).Value = (pap1.parmXml01 == null ? null : pap1.parmXml01.GetXml());
                //cmd.Parameters.Add("@dxml02", SqlDbType.Xml).Value = (pap1.parmXml02 == null ? null : pap1.parmXml02.GetXml());
                //cmd.Parameters.Add(new SqlParameter("@Dbin01", pap1.parmBin01));


                cmd.Parameters.Add(new SqlParameter("@Pxml01", pap1.pXml01));
                cmd.Parameters.Add(new SqlParameter("@Pxml02", pap1.pXml02));
                cmd.Parameters.Add(new SqlParameter("@Pbin01", pap1.parmBin01));



                cmd.Parameters.Add(new SqlParameter("@Param01", pap1.parm01));
                cmd.Parameters.Add(new SqlParameter("@Param02", pap1.parm02));
                cmd.Parameters.Add(new SqlParameter("@Param03", pap1.parm03));
                cmd.Parameters.Add(new SqlParameter("@Param04", pap1.parm04));
                cmd.Parameters.Add(new SqlParameter("@Param05", pap1.parm05));
                cmd.Parameters.Add(new SqlParameter("@Param06", pap1.parm06));
                cmd.Parameters.Add(new SqlParameter("@Param07", pap1.parm07));
                cmd.Parameters.Add(new SqlParameter("@Param08", pap1.parm08));
                cmd.Parameters.Add(new SqlParameter("@Param09", pap1.parm09));
                cmd.Parameters.Add(new SqlParameter("@Param10", pap1.parm10));
                cmd.Parameters.Add(new SqlParameter("@Param11", pap1.parm11));
                cmd.Parameters.Add(new SqlParameter("@Param12", pap1.parm12));
                cmd.Parameters.Add(new SqlParameter("@Param13", pap1.parm13));
                cmd.Parameters.Add(new SqlParameter("@Param14", pap1.parm14));
                cmd.Parameters.Add(new SqlParameter("@Param15", pap1.parm15));
                cmd.Parameters.Add(new SqlParameter("@Param16", pap1.parm16));
                cmd.Parameters.Add(new SqlParameter("@Param17", pap1.parm17));
                cmd.Parameters.Add(new SqlParameter("@Param18", pap1.parm18));
                cmd.Parameters.Add(new SqlParameter("@Param19", pap1.parm19));
                cmd.Parameters.Add(new SqlParameter("@Param20", pap1.parm20));
                Boolean result = _dataAccess.ExecuteCommand(cmd);
                if (result == false)//_result==false
                {
                    this.SetError(_dataAccess.ErrorObject);
                }
                return result;
            }
            catch (Exception exp)
            {
                this.SetError(exp);
                return false;
            }
        }

        public DataSet GetDataSetResult(ProcessAccessParams pap1)
        {
            DataSet ds1 = this.GetSqlResult(pap1: pap1);
            if (ds1 == null)
            {
                ds1 = new DataSet();
                ds1.Tables.Add(this.GetErrorTable());
            }
            else
                if (ds1.Tables[0].Columns[0].ColumnName.ToString().ToUpper().Contains("ERRORNUMBER"))
                ds1.Tables[0].TableName = "ErrorTable";

            return ds1;
        }

        private DataTable GetErrorTable()
        {
            // select error_number() as errornumber, error_severity() as errorseverity, error_state() as errorstate, 
            // error_procedure() as errorprocedure, @Param01 as process_id, error_line() as errorline, error_message() as errormessage;

            DataTable tbl1 = new DataTable("ErrorTable");
            tbl1.Columns.Add("errornumber", typeof(Int32));
            tbl1.Columns.Add("errorseverity", typeof(Int32));
            tbl1.Columns.Add("errorstate", typeof(Int32));
            tbl1.Columns.Add("errorprocedure", typeof(String));
            tbl1.Columns.Add("process_id", typeof(String));
            tbl1.Columns.Add("errorline", typeof(Int32));
            tbl1.Columns.Add("errormessage", typeof(String));

            //tbl1.Rows.Add(new Object[] { 0, 0, 0, pa1.ErrorObject["Location"].ToString(), pa1.ErrorObject["Src"].ToString(), 0, pa1.ErrorObject["Msg"].ToString() });
            tbl1.Rows.Add(new Object[] { 0, 0, 0, this.ErrorObject["Location"].ToString(), this.ErrorObject["Src"].ToString(), 0, this.ErrorObject["Msg"].ToString() });
            return tbl1;
        }

    }
}

