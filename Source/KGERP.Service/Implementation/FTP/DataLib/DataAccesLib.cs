using KGERP.FunLib;
using KGERP.GeneralLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.DataLib
{
    public class DataAccesLib
    {
        /*
        var pap1 = new ProcessAccessParams()
        {
            parm01 = docid,
            parm02 = status,
            ProcID = "UPDATE_ATTACHMENTSTAT_01",
            ProcName = "SP_AUDIT_ENTRY_TRANS_01"
        };

        var result = ProcessAccessAPI.GetDataSet(pap1);
        */
        #region  // File Archive
        public static List<FileArchive01> GetFileArchivesAsync(string type)
        {
            try
            {
                var pap1 = new ProcessAccessParams()
                {
                    parm01 = type,
                    ProcID = "GET_FILEARCHIVES_01",
                    ProcName = "SP_AUDIT_INFO_REPORT_01"
                };

                var ds = ProcessAccessAPI.GetDataSet(pap1);
                if (ds.Tables[0].Columns.Contains("errornumber"))
                {
                    return null;
                }
                else
                {
                    List<FileArchive01> resp = UtilityFuns.ConvertDataTable<FileArchive01>(ds.Tables[0]);
                    //   var s = ds.Tables[0].Rows[0]["filename1"];
                    return resp;
                }
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
        public static string UpdateFileAttachmentStatus02(string docid, string status)
        {
            try
            {
                var pap1 = new ProcessAccessParams()
                {
                    parm01 = docid,
                    parm02 = status,
                    ProcID = "UPDATE_ATTACHMENTSTAT_01",
                    ProcName = "SP_AUDIT_ENTRY_TRANS_01"
                };

                var result = ProcessAccessAPI.GetDataSet(pap1);

                if (result.Tables[0].Columns.Contains("errornumber"))
                {
                    return "Something went wrong!!";
                }
                else
                {
                    //List<AuditComment01> resp = DtToList.ConvertDataTable<AuditComment01>(result.Tables[0]);
                    var s = result.Tables[0].Rows[0]["msg"];
                    return s.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public static string UpdateFileArchive01(FileArchive01 model)
        {
            try
            {
                var pap1 = new ProcessAccessParams()
                {
                    parm01 = "201",
                    parm02 = model.docfilename,
                    parm03 = model.docdesc,
                    parm04 = model.fileext,
                    parm05 = model.filepath,
                    parm06 = model.filestat,
                    parm12 = model.userid.ToString(),
                    parm13 = model.FileSize.ToString(),
                    parm14 = model.DocType,
                    parm15 = model.RecDate.ToString("dd-MMM-yyyy"),
                    ProcID = "UPDATE_FILEARCHIVE_01",
                    ProcName = "SP_AUDIT_ENTRY_TRANS_01"
                };

                var result = ProcessAccessAPI.GetDataSet(pap1);


                //   var result = await AuditProcessAccess01.GetAuditDatasetAsync("[dbo].[SP_AUDIT_ENTRY_TRANS_01]", parameters);
                if (result.Tables[0].Columns.Contains("errornumber"))
                {
                    return "Something went wrong!!";
                }
                else
                {
                    //List<AuditComment01> resp = UtilityFuns.ConvertDataTable<AuditComment01>(result.Tables[0]);
                    var s = result.Tables[0].Rows[0]["filename1"];
                    return s.ToString();
                }
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        public static List<ServerConfigFileUpload01> GetServerConfig01(string typeDesc, string type)
        {
            List<ServerConfigFileUpload01> serverConfigs = null;
            try
            {
                var pap1 = new ProcessAccessParams()
                {
                    parm01 = typeDesc,
                    parm02 = type,
                    ProcID = "ServerConfigs_01",
                    ProcName = "SP_AUDIT_INFO_REPORT_01"
                };

                var ds = ProcessAccessAPI.GetDataSet(pap1);
                if (ds.Tables[0].Columns.Contains("errornumber"))
                {
                    return serverConfigs;
                }
                serverConfigs = UtilityFuns.ConvertDataTable<ServerConfigFileUpload01>(ds.Tables[0]);

                return serverConfigs;
            }
            catch (System.Exception ex)
            {
                return serverConfigs;
            }

        }

       
       
      

    }
}
