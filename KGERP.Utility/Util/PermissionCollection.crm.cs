using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Utility.Util
{
    public partial class PermissionCollection
    {
        [Description("1")]
        public sealed class Crms
        {
            [Description("101")]
            public sealed class Client
            {
                public const int CanViewAllClient = 10101;
                public const int CanViewMyClient = 10102;
                public const int CanUploadClientBatch = 10103;
                public const int CanAddNewClient = 10104;
               
                public const int CanEditClient = 10112;

                public const int CanChangeClientCompany = 10105;
                public const int CanChangeClientStatus = 10106;
                public const int CanChangeClientResponsibleOfficer = 10107;
                public const int CanDownloadClientList = 10108;
                public const int CanClientDetailsView = 10109;
                public const int CanExportClientExcel = 10110;
                public const int CanExportClientUploadBatchExcel = 10111;
                public const int OfficerWorkingupdate = 10113;
                public const int WorkingMdifyHistory = 10114;
                public const int CarRequsinCreate = 10115;
                public const int CarRequsinAssign = 10116;



            }
            [Description("102")]
            public sealed class Settings
            {
                public const int CanViewTeamList = 10201;
                public const int CanAddEditServiceStatus = 10202;
                public const int CanAddEditChoiceArea = 10203;
                public const int CanAddEditPromotionalOffer = 10204;
            }
        }
    }
}
