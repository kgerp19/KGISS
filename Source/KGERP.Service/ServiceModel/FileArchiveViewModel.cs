using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.ServiceModel
{
    public class FileArchiveViewModel
    {
        public long CGId { get; set; }
        public int CompanyId { get; set; }
        public long docid { get; set; }
        public string GroupName { get; set; }
        public string CompanyName { get; set; }
        public string FileNo { get; set; }
        public string Title { get; set; }
        public int FileCatagoryId { get; set; }
        public bool InBinFolder { get; set; }
        public string docfilename { get; set; }
        public string docdesc { get; set; }
        public string fileext { get; set; }
        public string filepath { get; set; }
        public Nullable<long> filesize { get; set; }

        public List<FileArchiveViewModel> FileArchive { get; set; }

    }
}
