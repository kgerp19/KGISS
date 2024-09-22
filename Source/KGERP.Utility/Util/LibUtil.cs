using System;
using System.Configuration;
using System.IO;

namespace KGERP.Utility.Util
{
    //Ashraf 20200109
    public class LibUtil
    {
        public static string GetConfigValue(string key)
        {
            if (ConfigurationManager.AppSettings[key] != null)
            {
                return ConfigurationManager.AppSettings[key].ToString();
            }

            return string.Empty;
        }

        public static string GetUploadPath(string documentType)
        {
            try
            {
                return GetConfigValue(documentType);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GetUploadPathIssueDocument(string caseNumber)
        {
            return GetUploadPath(Constants.KG_FILE_LOCATION) + Constants.KG_ASSET + "\\" + caseNumber + "\\";
        }
        public static bool FileExists(string path)
        {
            FileInfo fi = new FileInfo(path);

            if (fi.Exists)
            {
                return true;
            }

            return false;
        }

        public static bool CreateDirectory(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    DirectoryInfo info = new DirectoryInfo(path);
                    if (!info.Exists)
                    {
                        info.Create();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DeleteDirectory(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    DirectoryInfo info = new DirectoryInfo(path);

                    if (info.Exists)
                    {
                        info.Delete(true);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DeleteFile(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    FileInfo fi = new FileInfo(path);

                    if (fi.Exists)
                    {
                        fi.Delete();
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void ViewFile(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    FileInfo fi = new System.IO.FileInfo(path);

                    if (fi.Exists)
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}
