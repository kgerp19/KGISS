using System;
using System.IO;
using System.Web;

namespace KGERP.Utility.Util
{
    public class FileUtil
    {
        //Ashraf 20200109
        public static bool ValidateCaseFileSize(int fileSizeInBytes)
        {
            try
            {
                int allowedSize = 0;
                int.TryParse(LibUtil.GetConfigValue(Constants.CASE_FILE_UPLOAD_MAX_LENGTH), out allowedSize);

                if (allowedSize != 0)
                {
                    ///In bytes
                    allowedSize = allowedSize * 1024;
                    if (fileSizeInBytes > allowedSize)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //Ashraf 20200109
        public void DownloadHelper(string filePath, bool blnForceDownload)
        {
            try
            {
                FileInfo objFileInfo = new FileInfo(filePath);
                //exit if file does not exist
                if (!File.Exists(filePath))
                {
                    return;
                }

                //add headers to enable dialog display
                HttpContext.Current.Response.Clear();

                if (!blnForceDownload)
                {
                    string strWithoutStingSpace = objFileInfo.FullName.Replace(" ", "");
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + strWithoutStingSpace);
                }
                else
                {
                    string strWithoutStingSpace = objFileInfo.FullName.Replace(" ", "");
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "inline; filename=" + strWithoutStingSpace);
                }

                HttpContext.Current.Response.AddHeader("Content-Length", objFileInfo.Length.ToString());

                string strContentType = string.Empty;
                string strExtension = objFileInfo.Extension.Replace(".", "");
                switch (strExtension)
                {
                    case "txt":
                        strContentType = "text/plain";
                        break;
                    case "htm":
                    case "html":
                        strContentType = "text/html";
                        break;
                    case "rtf":
                        strContentType = "text/richtext";
                        break;
                    case "jpg":
                    case "jpeg":
                        strContentType = "image/jpeg";
                        break;
                    case "gif":
                        strContentType = "image/gif";
                        break;
                    case "bmp":
                        strContentType = "image/bmp";
                        break;
                    case "mpg":
                    case "mpeg":
                        strContentType = "video/mpeg";
                        break;
                    case "avi":
                        strContentType = "video/avi";
                        break;
                    case "mp4":
                        strContentType = "video/mp4";
                        break;
                    case "pdf":
                        strContentType = "application/pdf";
                        break;
                    case "doc":
                    case "dot":
                        strContentType = "application/msword";
                        break;
                    case "docx":
                        strContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    case "csv":
                    case "xls":
                    case "xlt":
                        strContentType = "application/x-msexcel";
                        break;
                    case "xlsx":
                        strContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    default:
                        strContentType = "application/octet-stream";
                        break;
                }

                HttpContext.Current.Response.ContentType = strContentType;
                HttpContext.Current.Response.WriteFile(objFileInfo.FullName);

            }
            catch (Exception ex)
            {
                ///_Logger.Error(ex);
            }
        }

    }


}
