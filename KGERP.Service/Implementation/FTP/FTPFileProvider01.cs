using KGERP.Service.ServiceModel.FTP_Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BEPZA_AUDIT.ViewModel
{
    public class FTPFileProvider01
    {
        protected string HostName;
        protected string BinFolderName = "RecycleBin";
        protected string RootPath;
        protected string RootName;
        protected string UserName;
        protected string Password;
        protected NetworkCredential Credentials = null;
        public static string FtpRootPath { get; set; }
        public static string FtpUserName { get; set; }
        public static string FtpPassword { get; set; }
        public FTPFileProvider01()
        {


        }
        public async Task<bool> MoveToRecycleBin(string FilePath)
        {
            try
            {
                string FileName = Path.GetFileName(FilePath);
                Uri serverFile = new Uri(this.RootPath + FilePath);
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(serverFile);
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.Credentials = new NetworkCredential(this.UserName, this.Password);
                reqFTP.RenameTo = "/" + this.BinFolderName + "/" + FileName;

                var resp = await reqFTP.GetResponseAsync();
                resp.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RestoreFileFromRecycleBin(string FilePath)
        {
            try
            {
                string FileName = Path.GetFileName(FilePath);
                Uri serverFile = new Uri(this.RootPath + this.BinFolderName + "/" + FileName);
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(serverFile);
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.Credentials = new NetworkCredential(this.UserName, this.Password);
                reqFTP.RenameTo = "/" + FilePath;

                var resp = await reqFTP.GetResponseAsync();
                resp.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteFilePermanently(string FilePath, bool InBinFolder = false)
        {
            try
            {

                string filedirectory = "";
                if (InBinFolder)
                {
                    var filename = Path.GetFileName(FilePath);
                    filedirectory = this.RootPath + BinFolderName + "/" + filename;
                }
                else
                {
                    filedirectory = this.RootPath + FilePath;
                }
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filedirectory);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(this.UserName, this.Password);
                FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }




        public void SetFTPConnection(string ftpRootPath, string ftpUserName, string ftpPassword)
        {
            if (ftpRootPath != null && ftpPassword != null)
            {
                this.RootPath = ftpRootPath;
                this.UserName = ftpUserName;
                this.Password = ftpPassword;
                this.RootName = ftpRootPath.Split('/').Where(f => !string.IsNullOrEmpty(f)).LastOrDefault();
                FtpWebRequest request = this.CreateRequest(ftpRootPath);
                this.HostName = request.RequestUri.Host;
                FtpRootPath = ftpRootPath;
                FtpUserName = ftpUserName;
                FtpPassword = ftpPassword;
            }
        }
        protected FtpWebRequest CreateRequest(string pathName)
        {
            FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(pathName);
            if (this.Credentials == null)
                this.Credentials = new NetworkCredential(this.UserName, this.Password);
            result.Credentials = this.Credentials;
            return result;
        }

        public async Task<bool> UploadIFileToFtp(FileItem ufile)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ufile.filepath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(this.UserName, this.Password);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;
                // rest of your FtpWebRequest setup here

                using (Stream reqStream = await request.GetRequestStreamAsync())
                {
                    await ufile.file.InputStream.CopyToAsync(reqStream);
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<MemoryStream> DownloadFileByPath(string FilePath)
        {
            MemoryStream memory = new MemoryStream();
            /* Create an FTP Request */
            var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(this.RootPath + FilePath);
            /* Log in to the FTP Server with the User Name and Password Provided */
            ftpRequest.Credentials = new NetworkCredential(FTPFileProvider01.FtpUserName, FTPFileProvider01.FtpPassword);
            /* When in doubt, use these options */
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = true;
            /* Specify the Type of FTP Request */
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            /* Establish Return Communication with the FTP Server */
            var ftpResponse = (FtpWebResponse)await ftpRequest.GetResponseAsync();
            /* Get the FTP Server's Response Stream */
            var ftpStream = ftpResponse.GetResponseStream();


            using (var stream = ftpStream) // FileStream(fileLink, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return memory;
        }
        public async Task<string> UploadFileToFtp(string url, string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var request = (FtpWebRequest)WebRequest.Create(url + fileName);

            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(this.UserName, this.Password);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            using (var fileStream = System.IO.File.OpenRead(filePath))
            {

                try
                {
                    using (var requestStream = request.GetRequestStream())
                    {
                        await fileStream.CopyToAsync(requestStream);
                        requestStream.Close();
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }


            }

            var response = (FtpWebResponse)request.GetResponse();
            Console.WriteLine("Upload done: {0}", response.StatusDescription);
            string s = response.StatusDescription;
            response.Close();

            return s;
        }

        public void FtpFolderRename(string folderName = "")
        {

            string ftpDirectory = this.RootPath + "Maruf1";


            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpDirectory);
            request.Method = WebRequestMethods.Ftp.Rename;
            request.Credentials = new NetworkCredential(this.UserName, this.Password);
            request.RenameTo = "Maruf2";
        }

        public bool DoesFtpDirectoryExist(string dirPath)
        {
            // string ftpDirectory = "ftp://localhost/ARCHIVE_ROOT/test_if_exist_directory/";

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dirPath);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(this.UserName, this.Password);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool CreateFTPDirectory(string directory)
        {

            try
            {
                //create the directory

                var x = new Uri(directory);
                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri(directory));
                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                requestDir.Credentials = new NetworkCredential(this.UserName, this.Password);
                requestDir.UsePassive = true;
                requestDir.UseBinary = true;
                requestDir.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void DownloadFileFTP()
        {
            String RemoteFtpPath = "ftp://ftp.csidata.com:21/Futures.20150305.gz";
            String LocalDestinationPath = "Futures.20150305.gz";
            String Username = "yourusername";
            String Password = "yourpassword";
            Boolean UseBinary = true; // use true for .zip file or false for a text file
            Boolean UsePassive = false;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(RemoteFtpPath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.KeepAlive = true;
            request.UsePassive = UsePassive;
            request.UseBinary = UseBinary;

            request.Credentials = new NetworkCredential(Username, Password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            using (FileStream writer = new FileStream(LocalDestinationPath, FileMode.Create))
            {

                long length = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[2048];

                readCount = responseStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    writer.Write(buffer, 0, readCount);
                    readCount = responseStream.Read(buffer, 0, bufferSize);
                }
            }

            reader.Close();
            response.Close();
        }




    }
}
