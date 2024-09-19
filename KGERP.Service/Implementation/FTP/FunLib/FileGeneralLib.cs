using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.GeneralLib
{
    public class FileGeneralLib
    {
    }

    public class ServerConfigFileUpload01 
    {
        #region props
        public int Id { get; set; }
        public string Path { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string TypeDesc { get; set; }
        public bool Status { get; set; }
        #endregion
    }
    public class FileArchive01
    {
        #region properties
        public FileArchive01()
        {
            attachments01s = new List<Attachments01>();
        }
        IList<Attachments01> attachments01s { get; set; }
        public int docid { get; set; }
        [DisplayName("ফাইলের বিবরণ")]
        public string docdesc { get; set; }
        public string DocType { get; set; }
        public string docfilename { get; set; }
        [DisplayName("ফাইল নাম")]
        public string docfilename1 { get; set; }
        public string fileext { get; set; }

        public string filepath { get; set; }
        public long FileSize { get; set; }
        public int userid { get; set; }
        [DisplayName("স্ট্যাটাস")]
        public string filestat { get; set; }
        public string filestat1 { get; set; }
        public string rowtime1 { get; set; }
        public DateTime RecDate { get; set; }
        #endregion

    }
    public class Attachments01
    {
        #region properties
        public Attachments01()
        {
            attachments01s = new List<Attachments01>();
        }
        IList<Attachments01> attachments01s { get; set; }
        public int docid { get; set; }
        [DisplayName("ফাইলের বিবরণ")]
        public string docdesc { get; set; }
        public string docfilename { get; set; }
        [DisplayName("ফাইল নাম")]
        public string docfilename1 { get; set; }
        public string fileext { get; set; }

        public string filepath { get; set; }
        public int auditid { get; set; }
        public int acomid { get; set; }
        [DisplayName("স্ট্যাটাস")]
        public string filestat { get; set; }
        public string filestat1 { get; set; }
        #endregion

    }

    

}
