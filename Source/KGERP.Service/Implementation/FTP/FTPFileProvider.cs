using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
//using Syncfusion.EJ2.FileManager.Base;

namespace KGERP.Service
{
    public class FTPFileProvider
    {
        protected string HostName;
        protected string RootPath;
        protected string RootName;
        protected string UserName;
        protected string Password;
        protected NetworkCredential Credentials = null;
     //   AccessDetails AccessDetails = new AccessDetails();

        public FTPFileProvider() { }

        public void SetFTPConnection(string ftpRootPath, string ftpUserName, string ftpPassword)
        {
            this.RootPath = ftpRootPath;
            this.UserName = ftpUserName;
            this.Password = ftpPassword;
            this.RootName = ftpRootPath.Split('/').Where(f => !string.IsNullOrEmpty(f)).LastOrDefault();
            FtpWebRequest request = this.CreateRequest(ftpRootPath);
            this.HostName = request.RequestUri.Host;
        }

        protected FtpWebRequest CreateRequest(string pathName)
        {
            FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(pathName);
            if (this.Credentials == null)
                this.Credentials = new NetworkCredential(this.UserName, this.Password);
            result.Credentials = this.Credentials;
            return result;
        }

        protected FtpWebResponse CreateResponse(string fullPath, string method)
        {
            FtpWebRequest request = this.CreateRequest(fullPath);
            request.Method = method;
            return (FtpWebResponse)request.GetResponse();
        }

        protected StreamReader CreateReader(string fullPath, string method)
        {
            FtpWebResponse response = this.CreateResponse(fullPath, method);
            Stream responseStream = response.GetResponseStream();
            return new StreamReader(responseStream);
        }

        protected bool IsFile(string fullPath, string name)
        {
            StreamReader reader = this.CreateReader(fullPath, WebRequestMethods.Ftp.ListDirectory);
            bool isFile = false;
            int index = 0;
            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                FTPFileDetails detail = ParseDirectoryListLine(line);
                index++;
                if (detail.Name == name)
                {
                    isFile = detail.IsFile;
                    break;
                }
                line = reader.ReadLine();
            }
            reader.Close();
            return isFile;
        }

        protected bool IsExist(string fullPath, string name, bool ignoreCase = false)
        {
            StreamReader reader = this.CreateReader(fullPath, WebRequestMethods.Ftp.ListDirectory);
            bool isExist = false;
            int index = 0;
            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                index++;
                if (line == name || (ignoreCase && line.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    isExist = true;
                    break;
                }
                line = reader.ReadLine();
            }
            reader.Close();
            return isExist;
        }

        protected long GetFolderSize(string fileName, long size)
        {
            try
            {
                StreamReader reader = this.CreateReader(fileName, WebRequestMethods.Ftp.ListDirectoryDetails);
                string line = reader.ReadLine();

                while (!string.IsNullOrEmpty(line))
                {
                    FTPFileDetails detail = ParseDirectoryListLine(line);
                    bool isFile = detail.IsFile;
                    string fullPath = fileName + detail.Name;
                    if (isFile)
                    {
                        size += detail.Size;
                    }
                    else
                    {
                        size = this.GetFolderSize(fullPath + "/", size);
                    }
                    line = reader.ReadLine();
                }
                return size;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected string GetFileExtension(string fileName)
        {
            int index = fileName.LastIndexOf(".");
            return (index == -1) ? "" : fileName.Substring(index);
        }

        protected string GetFileNameWithoutExtension(string fileName)
        {
            int index = fileName.LastIndexOf(".");
            return (index == -1) ? fileName : fileName.Substring(0, index);
        }

        protected bool HasChild(string path)
        {
            StreamReader reader = this.CreateReader(path, WebRequestMethods.Ftp.ListDirectoryDetails);
            bool hasChild = false;
            string[] list = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            reader.Close();
            for (int i = 0; i < list.Length; i++)
            {
                FTPFileDetails details = ParseDirectoryListLine(list[i]);
                hasChild = !details.IsFile;
                if (hasChild) { break; }
            }
            return hasChild;
        }

        protected string GetFilterPath(string path)
        {
            return path.Substring(this.RootPath.Length).Replace("/", "\\");
        }

        //protected FileManagerDirectoryContent GetPathDetails(string fullPath, string path, FileManagerDirectoryContent data)
        //{
        //    FileManagerDirectoryContent cwd = new FileManagerDirectoryContent();
        //    cwd.Name = fullPath.Split('/').Where(f => !string.IsNullOrEmpty(f)).LastOrDefault();
        //    cwd.IsFile = false;
        //    cwd.Size = 0;
        //    cwd.DateModified = data == null ? DateTime.Now : data.DateModified;
        //    cwd.DateCreated = cwd.DateModified;
        //    cwd.HasChild = this.HasChild(fullPath);
        //    cwd.Type = "";
        //    cwd.FilterPath = (path == "/") ? "" : this.GetFilterPath(this.SplitPath(fullPath)[0]);
        //    return cwd;
        //}

        protected string[] SplitPath(string path, bool isFile = false)
        {
            string[] str_array = path.Split('/'), fileDetails = new string[2];
            string parentPath = "";
            int len = str_array.Length - (isFile ? 1 : 2);
            for (int i = 0; i < len; i++)
            {
                parentPath += str_array[i] + "/";
            }
            fileDetails[0] = parentPath;
            fileDetails[1] = str_array[len];
            return fileDetails;
        }

        protected string GetPath(string path)
        {
            return (this.RootPath + path);
        }

        protected byte[] ConvertByte(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream file = new MemoryStream())
            {
                int count;
                while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    file.Write(buffer, 0, count);
                }
                return file.ToArray();
            }
        }

        protected string GetPattern(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                              .Replace(@"\*", ".*")
                              .Replace(@"\?", ".")
                       + "$";
        }

        protected string ByteConversion(long fileSize)
        {
            try
            {
                string[] index = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
                if (fileSize == 0)
                {
                    return "0 " + index[0];
                }

                long bytes = Math.Abs(fileSize);
                int loc = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                double num = Math.Round(bytes / Math.Pow(1024, loc), 1);
                return (Math.Sign(fileSize) * num).ToString() + " " + index[loc];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //protected void NestedSearch(string searchPath, List<FileManagerDirectoryContent> foundedFiles, string searchString, bool caseSensitive)
        //{
        //    try
        //    {
        //        StreamReader reader = this.CreateReader(searchPath, WebRequestMethods.Ftp.ListDirectoryDetails);
        //        int index = 0;
        //        string line = reader.ReadLine();
        //        while (!string.IsNullOrEmpty(line))
        //        {
        //            FTPFileDetails detail = ParseDirectoryListLine(line);
        //            index++;
        //            bool matched = new Regex(this.GetPattern(searchString), (caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase)).IsMatch(detail.Name);
        //            if (matched)
        //            {
        //                FileManagerDirectoryContent item = new FileManagerDirectoryContent();
        //                item.Name = detail.Name;
        //                item.IsFile = detail.IsFile;
        //                item.FilterPath = this.GetFilterPath(searchPath);
        //                if (detail.IsFile)
        //                {
        //                    item.Size = detail.Size;
        //                    this.UpdateFileDetails(foundedFiles, item, searchPath, detail.Name);
        //                }
        //                else
        //                {
        //                    string nestedPath = searchPath + detail.Name + "/";
        //                    item.DateModified = detail.Modified;
        //                    this.UpdateFolderDetails(foundedFiles, item, searchPath, detail.Name);

        //                    if (item.Permission == null || item.Permission.Read)
        //                    {
        //                        this.NestedSearch(nestedPath, foundedFiles, searchString, caseSensitive);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (!detail.IsFile)
        //                {
        //                    string nestedPath = searchPath + detail.Name + "/";

        //                    this.NestedSearch(nestedPath, foundedFiles, searchString, caseSensitive);
        //                }
        //            }
        //            line = reader.ReadLine();
        //        }
        //        reader.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        //protected void UpdateFileDetails(List<FileManagerDirectoryContent> items, FileManagerDirectoryContent item, string fullPath, string line)
        //{
        //    item.DateCreated = item.DateModified;
        //    item.HasChild = false;
        //    item.Type = this.GetFileExtension(line);
        //    items.Add(item);
        //}

        //protected void UpdateFolderDetails(List<FileManagerDirectoryContent> items, FileManagerDirectoryContent item, string fullPath, string line)
        //{
        //    string nestedPath = fullPath + line + "/";
        //    item.Size = 0;
        //    item.DateCreated = item.DateModified;
        //    item.HasChild = this.HasChild(nestedPath);
        //    item.Type = "";
        //    items.Add(item);
        //}

        //protected FileManagerDirectoryContent GetFileDetails(string fullPath, string name, bool isFile, long size = 0)
        //{
        //    string nestedPath = fullPath + name + "/";
        //    FileManagerDirectoryContent item = new FileManagerDirectoryContent();
        //    item.Name = name;
        //    item.IsFile = isFile;
        //    item.Size = size;
        //    item.DateModified = DateTime.Now;
        //    item.DateCreated = item.DateModified;
        //    item.HasChild = isFile ? false : this.HasChild(nestedPath);
        //    item.Type = isFile ? this.GetFileExtension(name) : "";
        //    item.FilterPath = this.GetFilterPath(fullPath);
        //    return item;
        //}
        protected void DeleteFile(string fullPath)
        {
            FtpWebResponse response = this.CreateResponse(fullPath, WebRequestMethods.Ftp.DeleteFile);
            response.Close();
        }
        protected void CreateFolder(string fullPath)
        {
            FtpWebResponse response = this.CreateResponse(fullPath, WebRequestMethods.Ftp.DeleteFile);
            response.Close();
        }

        protected void DeleteFolder(string fullPath)
        {
            FtpWebResponse response = this.CreateResponse(fullPath, WebRequestMethods.Ftp.RemoveDirectory);
            response.Close();
        }

        protected void NestedDelete(string folderName)
        {
            try
            {
                string fullPath = folderName + "/";
                StreamReader reader = this.CreateReader(fullPath, WebRequestMethods.Ftp.ListDirectoryDetails);
                string line = reader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    FTPFileDetails detail = ParseDirectoryListLine(line);
                    bool isFile = detail.IsFile;
                    if (isFile)
                    {
                        this.DeleteFile(fullPath + detail.Name);
                    }
                    else
                    {
                        this.NestedDelete(fullPath + detail.Name);
                    }
                    line = reader.ReadLine();
                }
                if (string.IsNullOrEmpty(line))
                {
                    this.DeleteFolder(folderName);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected void UploadFile(IFormFile file, string fileName)
        {
            FtpWebRequest request = this.CreateRequest(fileName);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            using (Stream stream = request.GetRequestStream())
            {
                file.CopyTo(stream);
            }
        }

        protected FileStreamResult DownloadFile(string fullPath, string folderPath)
        {
            string tempPath = this.GetTempFilePath(fullPath, folderPath);
            FileStream fileStreamInput = new FileStream(tempPath, FileMode.Open, FileAccess.Read);
            FileStreamResult fileStreamResult = new FileStreamResult(fileStreamInput, "APPLICATION/octet-stream");
            return fileStreamResult;
        }

        protected string GetTempFilePath(string fullPath, string folderPath)
        {
            string tempPath = Path.Combine(folderPath, DateTime.Now.ToFileTime().ToString()) + fullPath.Split('/').Last();
            this.CopyFile(fullPath, tempPath);
            return tempPath;
        }

        protected void CopyFile(string fileName, string tempPath)
        {
            FtpWebResponse response = this.CreateResponse(fileName, WebRequestMethods.Ftp.DownloadFile);
            byte[] buffer = this.ConvertByte(response.GetResponseStream());
            using (Stream file = File.OpenWrite(tempPath))
            {
                file.Write(buffer, 0, buffer.Length);
            }
            response.Close();
        }

        protected void CopyFileToServer(string fileName, string tempPath)
        {
            FtpWebResponse response = this.CreateResponse(fileName, WebRequestMethods.Ftp.DownloadFile);
            byte[] buffer = this.ConvertByte(response.GetResponseStream());
            FtpWebRequest request = this.CreateRequest(tempPath);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(buffer, 0, buffer.Length);
            }
            response.Close();
        }

        //protected FileStreamResult DownloadFolder(string path, string[] names, string tempPath, string folderPath, FileManagerDirectoryContent[] data)
        //{
        //    string basePath = this.RootPath + path;
        //    List<string> fileList = new List<string>();
        //    List<string> folderList = new List<string>();
        //    string fileName;
        //    ZipArchiveEntry zipEntry;
        //    ZipArchive archive;
        //    for (int i = 0; i < names.Count(); i++)
        //    {
        //        this.UpdateDownloadPath(folderPath, basePath, names[i], fileList, folderList, data[i]);
        //    }
        //    using (archive = ZipFile.Open(tempPath, ZipArchiveMode.Update))
        //    {
        //        for (int j = 0; j < folderList.Count; j++)
        //        {
        //            fileName = folderList[j].Substring(folderPath.Length + 1);
        //            zipEntry = archive.CreateEntry(fileName);
        //        }
        //        for (int j = 0; j < fileList.Count; j++)
        //        {
        //            fileName = fileList[j].Substring(folderPath.Length + 1);
        //            zipEntry = archive.CreateEntryFromFile(fileList[j], fileName, CompressionLevel.Fastest);
        //        }
        //    }
        //    FileStream fileStreamInput = new FileStream(tempPath, FileMode.Open, FileAccess.Read, FileShare.Delete);
        //    FileStreamResult fileStreamResult = new FileStreamResult(fileStreamInput, "APPLICATION/octet-stream");
        //    if (names.Length == 1)
        //    {
        //        fileStreamResult.FileDownloadName = data[0].Name + ".zip";
        //    }
        //    else
        //    {
        //        fileStreamResult.FileDownloadName = "folders.zip";
        //    }
        //    return fileStreamResult;
        //}

        //protected void UpdateDownloadPath(string folderPath, string basePath, string folderName, List<string> fileList, List<string> folderList, FileManagerDirectoryContent data)
        //{
        //    string fullPath = basePath + folderName;
        //    string newFolderName = folderName.Replace("/", "\\");
        //    string[] fileDetails = this.SplitPath(fullPath, true);
        //    bool isFile = data.IsFile;
        //    if (isFile)
        //    {
        //        string path = Path.Combine(folderPath, newFolderName.Substring(0, newFolderName.Length - fileDetails[1].Length));
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        string tempPath = Path.Combine(folderPath, newFolderName);
        //        if (!fileList.Contains(tempPath))
        //        {
        //            this.CopyFile(fullPath, tempPath);
        //            fileList.Add(tempPath);
        //        }
        //    }
        //    else
        //    {
        //        string path = Path.Combine(folderPath, newFolderName);
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        string validPath = fullPath + "/";
        //        StreamReader reader = this.CreateReader(validPath, WebRequestMethods.Ftp.ListDirectoryDetails);
        //        int index = 0;
        //        string line = reader.ReadLine();
        //        while (!string.IsNullOrEmpty(line))
        //        {
        //            FTPFileDetails detail = ParseDirectoryListLine(line);
        //            index++;
        //            bool isSubFile = detail.IsFile;
        //            if (isSubFile)
        //            {
        //                string fileName = validPath + detail.Name;
        //                string tempPath = Path.Combine(folderPath, newFolderName, detail.Name);
        //                if (!fileList.Contains(tempPath))
        //                {
        //                    this.CopyFile(fileName, tempPath);
        //                    fileList.Add(tempPath);
        //                }
        //            }
        //            else
        //            {
        //                FileManagerDirectoryContent item = new FileManagerDirectoryContent();
        //                item.IsFile = false;
        //                this.UpdateDownloadPath(folderPath, basePath, folderName + "/" + detail.Name, fileList, folderList, item);
        //            }
        //            line = reader.ReadLine();
        //        }
        //        if (index == 0) { folderList.Add(path + "\\"); }
        //    }
        //}

        protected void RemoveTempImage()
        {
            string folderPath = Path.Combine(Path.GetTempPath(), "image_temp");
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
            Directory.CreateDirectory(folderPath);
        }

        protected string GetCopyName(string fullPath, string name)
        {
            string newName = name;
            int index = newName.LastIndexOf(".");
            if (index >= 0)
                newName = newName.Substring(0, index);
            int fileCount = 0;
            string extn = this.GetFileExtension(name);
            while (this.IsExist(fullPath, newName + (fileCount > 0 ? "(" + fileCount.ToString() + ")" : "") + extn))
            {
                fileCount++;
            }
            newName = newName + (fileCount > 0 ? "(" + fileCount.ToString() + ")" : "") + extn;
            return newName;
        }

        protected void CopyDirectory(string srcName, string desName)
        {
            FtpWebRequest request = this.CreateRequest(desName);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            string srcPath = srcName + "/";
            string desPath = desName + "/";
            StreamReader reader = this.CreateReader(srcPath, WebRequestMethods.Ftp.ListDirectoryDetails);
            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                FTPFileDetails detail = ParseDirectoryListLine(line);
                string srcFileName = srcPath + detail.Name;
                string desFileName = desPath + detail.Name;
                bool isSubFile = detail.IsFile;
                if (isSubFile)
                {
                    this.CopyFileToServer(srcFileName, desFileName);
                }
                else
                {
                    this.CopyDirectory(srcFileName, desFileName);
                }
                line = reader.ReadLine();
            }
            reader.Close();
        }

        private FTPFileDetails ParseDirectoryListLine(string line)
        {
            FTPFileDetails details = new FTPFileDetails();
            string unixRegex = @"^(?<DIR>[-d])((?:[-r][-w][-xs]){3})\s+\d+\s+\w+(?:\s+\w+)?\s+(?<FileSize>\d+)\s+(?<Modified>\w+\s+\d+(?:\s+\d+(?::\d+)?))\s+(?!(?:\.|\.\.)\s*$)(?<FileName>.+?)\s*$";
            string msDosRegex = @"^(?<Modified>\d{2}\-\d{2}\-(\d{2,4})\s+\d{2}:\d{2}[Aa|Pp][mM])\s+(?<DIR>\<\w+\>){0,1}(?<FileSize>\d+){0,1}\s+(?<FileName>.+)";
            Match parsedLine;
            if (Regex.IsMatch(line, unixRegex))
            {
                parsedLine = Regex.Match(line, unixRegex);
                details.IsFile = parsedLine.Groups["DIR"].Value != "d";
            }
            else if (Regex.IsMatch(line, msDosRegex))
            {
                parsedLine = Regex.Match(line, msDosRegex);
                details.IsFile = parsedLine.Groups["DIR"].Value != "<DIR>";
            }
            else
            {
                throw new Exception("Non implemented response format");
            }
            details.Modified = Convert.ToDateTime(parsedLine.Groups["Modified"].Value);
            details.Size = details.IsFile ? Convert.ToInt64(parsedLine.Groups["FileSize"].Value) : 0;
            details.Name = parsedLine.Groups["FileName"].Value;
            return details;
        }
        private class FTPFileDetails
        {
            public bool IsFile { get; set; }

            public DateTime Modified { get; set; }

            public string Name { get; set; }

            public long Size { get; set; }
        }
    }
}
