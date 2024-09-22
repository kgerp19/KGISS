using System;


namespace KGERP.Utility.Util
{
    public sealed class Constants
    {
        public static string DBTypeSQLServer = "MSSQL";
        public static string FileType = "File Extension Is InValid - Only Upload JPG/JPEG/PNG/BMP File";
        public static string DBTypeMySQLServer = "MySQL";
        public static string DBTypeORACLEServer = "ORACLE";

        public static DateTime NullDateTime = DateTime.MinValue;
        public static bool BooleanData = false;
        public static decimal NullDecimal = decimal.MinValue;
        public static double NullDouble = double.MinValue;
        public static Guid NullGuid = Guid.Empty;
        public static int NullInt = int.MinValue;
        public static long NullLong = long.MinValue;
        public static float NullFloat = float.MinValue;
        public static string NullString = string.Empty;


        public static string USER_STATUS_ACTIVE_YES = "Yes";
        public static string USER_STATUS_ACTIVE_NO = "No";

        public static string CONFIG_PARAM_ADMIN_USER = "SuperAdminUser";
        public static string CONFIG_PARAM_ADMIN_ROLE = "SuperAdminRole";

        public static string ROLE_ADMIN = "Admin";
        public static string ROLE_DEPT_ADMIN = "Department Admin";

        public const string CONFIG_PARAM_SMTP_FROM = "SMTP.From";

        public const string CONFIG_PARAM_SMTP_HOST = "SMTP.Host";
        public const string CONFIG_PARAM_SMTP_PASSWORD = "SMTP.Password";
        public const string CONFIG_PARAM_SMTP_PORT = "SMTP.Port";
        public const string CONFIG_PARAM_SMTP_USERNAME = "SMTP.UserName";

        public static string FILE_UPLOAD_MAX_LENGTH = "MaxFileUploadSize";
        public static string CASE_FILE_UPLOAD_MAX_LENGTH = "CaseMaxFileUploadSize";
        public static string NOTIFICATION_UPLOAD_FILE_SIZE_EXCEED = "Your Selected file exceeds maximum allowed file size: ";
        public static string NOTIFICATION_FILE_SIZE_MB = "MB";


        public static string KG_FILE_LOCATION = "KGFiles";
        public static string KG_ASSET = "Assets";
        public static string KG_ASSET_OFFICE = "OfficeAssets";
        public static string KG_ASSET_LAND = "LandAssets";

        public static string KG_FTP_LOCATION_CASE = "CASE";

        public static string NOTIFICATION_ERROR_DATA_SAVING = "Error in saving data.";
        public static string NOTIFICATION_ERROR_DATA_DELETING = "Error in deleting data.";

        public static string NOTIFICATION_SUCCESS_DATA_SAVING = "Data saved successfully.";
        public static string NOTIFICATION_SUCCESS_DATA_DELETING = "Data deleted successfully.";

        public static string NOTIFICATION_ERROR_FILE_SAVING = "Error in saving file.";
        public static string NOTIFICATION_ERROR_FILE_DELETING = "Error in deleting file.";

        public static string NOTIFICATION_SUCCESS_FILE_SAVING = "File saved successfully.";
        public static string NOTIFICATION_SUCCESS_FILE_DELETING = "File deleted successfully.";
        public static string NOTIFICATION_SUCCESS_MAIL_SENDING = "Mail Sent successfully.";

        public static string NOTIFICATION_ERROR_FILE_SIZE_OR_EXTENTION = "Error In FileSize or Extention.";
        public static string NOTIFICATION_ERROR_FILE_NAME_OR_EXTENTION = "Error In FileName or Extention.";
        public static string NOTIFICATION_ERROR_SAME_FILE_EXIST = "File with same name already exists. Please change the file name.";
        public static string NOTIFICATION_ERROR_SAME_FILE_COUNT = "You have already attahced 5 file";


        public static string STATUS_PENDING = "Pending";
        public static string STATUS_INPROGRESS = "In Progress";
        public static string STATUS_DONE = "Done";
        public static string STATUS_NOT_APPLICABLE = "Not Applicable";
        public static string STATUS_REJECTED = "Rejected";
        public static string STATUS_ON_HOLD = "On Hold";
        public const string REPORT_MODULE_PATH = "~\\Reports\\";

        public static string DATA_NOT_FOUND = "Data not found !";
        public static string OPERATION_FAILE = "Operation Failed due to some exception!";

    }

}

