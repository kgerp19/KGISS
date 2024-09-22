using BEPZA_AUDIT.ViewModel;
using KGERP.Data.Models;
using KGERP.FunLib;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.FTP_Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.FTP
{
    public class FTPService : IFTPService
    {
        private readonly ERPEntities context = new ERPEntities();
        public ServerConfig ftpConfig = new ServerConfig();
        FTPFileProvider01 fTPFileProvider01 = new FTPFileProvider01();
        public FTPService()
        {
            this.ftpConfig = context.ServerConfigs.Where(e => e.Status == true).SingleOrDefault();
            this.fTPFileProvider01.SetFTPConnection(this.ftpConfig.Path, this.ftpConfig.UserName, this.ftpConfig.Password);
        }

        public async Task<FileUrl> GetFilePathByDocId(long docid)
        {
            FileUrl model = new FileUrl();
            var file = await context.FileArchives.SingleOrDefaultAsync(e => e.docid == docid && e.isactive && e.InBinFolder == false);
            if (file != null)
            {
                model.DocId = file.docid;
                model.FilePath = $"{ this.ftpConfig.Path}{file.filepath}";


            }
            else { model.IsFileExists = false; model.Error = "file does not exists"; }
            return model;
        }

        public async Task<FileItem> UploadFile(FileItem file, string foldername = "", bool isbulkUpload = false)
        {
            try
            {
                file.fileext = Path.GetExtension(file.file.FileName);
                var folderpath = context.FileCatagories.Where(e => e.FileCatagoryId == file.FileCatagoryId).Single().FolderPath;
                string YearMonth = DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.ToString("MM") + "/";
                string firstfolderpath = $"{this.ftpConfig.Path}{folderpath}";
                if (!fTPFileProvider01.DoesFtpDirectoryExist(firstfolderpath))
                {
                    fTPFileProvider01.CreateFTPDirectory(firstfolderpath);
                }
                if (!String.IsNullOrEmpty(foldername.Trim()))
                {
                    string secondFolderPath = firstfolderpath + "/" + foldername;
                    if (!fTPFileProvider01.DoesFtpDirectoryExist(secondFolderPath))
                    {
                        fTPFileProvider01.CreateFTPDirectory(secondFolderPath);
                    }
                    file.filepath = $"{secondFolderPath}/{Guid.NewGuid()}{file.fileext}";
                }
                else
                {
                    file.filepath = $"{this.ftpConfig.Path}{folderpath}/{Guid.NewGuid()}{file.fileext}";
                }
                file.filestat = "ACTIVE";
                file.filesize = file.file.ContentLength;
                var result = await fTPFileProvider01.UploadIFileToFtp(file);
                if (result)
                {
                    file.IsUploaded = true;
                    if (!isbulkUpload)
                    {
                        FileArchive Dbentry = ConvertObject(file);
                        Dbentry.filepath = Dbentry.filepath.Replace(this.ftpConfig.Path, "");
                        context.FileArchives.Add(Dbentry);
                        await context.SaveChangesAsync();
                        file.docid = Dbentry.docid;
                    }
                    else
                    {
                        file.filepath = file.filepath.Replace(this.ftpConfig.Path, "");
                    }
                }

            }
            catch (Exception Ex)
            {
                file.IsUploaded = false;
                file.ErrorMessage = Ex.Message;
            }
            return file;
        }

        //public async Task<FileItem> UpdateFile(FileItem file, long oldDocId)
        //{
        //    try
        //    {
        //        var exist = context.FileArchives.FirstOrDefault(x => x.docid == oldDocId);
        //        if (exist == null)
        //        {
        //            return file;
        //        }

        //        var isDeleted = await fTPFileProvider01.DeleteFilePermanently(exist.filepath, false);
        //        if (isDeleted)
        //        {

        //        }
        //        file.fileext = Path.GetExtension(file.file.FileName);
        //        var folderpath = context.FileCatagories.Where(e => e.FileCatagoryId == file.FileCatagoryId).Single().FolderPath;
        //        string YearMonth = DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.ToString("MM") + "/";
        //        string firstfolderpath = $"{this.ftpConfig.Path}{folderpath}";
        //        if (!fTPFileProvider01.DoesFtpDirectoryExist(firstfolderpath))
        //        {
        //            fTPFileProvider01.CreateFTPDirectory(firstfolderpath);
        //        }
        //        if (!String.IsNullOrEmpty(foldername.Trim()))
        //        {
        //            string secondFolderPath = firstfolderpath + "/" + foldername;
        //            if (!fTPFileProvider01.DoesFtpDirectoryExist(secondFolderPath))
        //            {
        //                fTPFileProvider01.CreateFTPDirectory(secondFolderPath);
        //            }
        //            file.filepath = $"{secondFolderPath}/{Guid.NewGuid()}{file.fileext}";
        //        }
        //        else
        //        {
        //            file.filepath = $"{this.ftpConfig.Path}{folderpath}/{Guid.NewGuid()}{file.fileext}";
        //        }
        //        file.filestat = "ACTIVE";
        //        file.filesize = file.file.ContentLength;
        //        var result = await fTPFileProvider01.UploadIFileToFtp(file);
        //        if (result)
        //        {
        //            file.IsUploaded = true;
        //            if (!isbulkUpload)
        //            {
        //                FileArchive Dbentry = ConvertObject(file);
        //                Dbentry.filepath = Dbentry.filepath.Replace(this.ftpConfig.Path, "");
        //                context.FileArchives.Add(Dbentry);
        //                await context.SaveChangesAsync();
        //                file.docid = Dbentry.docid;
        //            }
        //            else
        //            {
        //                file.filepath = file.filepath.Replace(this.ftpConfig.Path, "");
        //            }
        //        }

        //    }
        //    catch (Exception Ex)
        //    {
        //        file.IsUploaded = false;
        //        file.ErrorMessage = Ex.Message;
        //    }
        //    return file;
        //}


        public async Task<bool> FileMapping(List<FileItem> itemlist, long CGId, bool isReplaced)
        {
            try
            {
                List<CustomerBookingFileMapping> fileMappings = new List<CustomerBookingFileMapping>();
                int res = 0;
                foreach (var item in itemlist)
                {
                    if (isReplaced)
                    {
                        var exist = context.CustomerBookingFileMappings.FirstOrDefault(x => x.CGId == CGId);
                        if (exist != null)
                        {
                            exist.DocId = item.docid;
                            exist.FileTitel = item.docdesc;
                            exist.IsActive = true;
                            res += await context.SaveChangesAsync();
                            continue;
                        }
                    }
                    fileMappings.Add(new CustomerBookingFileMapping
                    {
                        DocId = item.docid,
                        CGId = CGId,
                        FileTitel = item.docdesc,
                        IsActive = true
                    });
                }
                context.CustomerBookingFileMappings.AddRange(fileMappings);
                res += await context.SaveChangesAsync();
                if (res == 0) { return false; } else { return true; }
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        public async Task<List<vwFTPFileInfo>> GetAllFilesByCatagory(int id = 0, bool IsBinFiles = false)
        {
            if (id < 0)
            {
                return await context.vwFTPFileInfoes.Where(o => o.FileCatagoryId == id && o.InBinFolder == IsBinFiles).ToListAsync();
            }
            else
            {
                return await context.vwFTPFileInfoes.Where(o => o.InBinFolder == IsBinFiles).ToListAsync();
            }
        }

        public async Task<List<FileItem>> UploadFileBulk(List<FileItem> files, string FolderName = "")
        {
            List<FileArchive> DbEntries = new List<FileArchive>();
            for (int i = 0; i < files.Count; i++)
            {
                files[i] = await this.UploadFile(files[i], FolderName, true);
            }
            DbEntries = this.ConvertBulk(files.Where(e => e.IsUploaded == true).ToList());
            context.FileArchives.AddRange(DbEntries);
            await context.SaveChangesAsync();
            for (int i = 0; i < files.Where(e => e.IsUploaded == true).ToList().Count(); i++)
            {
                files[i].docid = DbEntries[i].docid;
            }
            return files;
        }
        public async Task<FilePreviewModel> GetFileById(int Id)
        {
            FilePreviewModel model = new FilePreviewModel();
            var archive = await context.FileArchives.Where(e => e.docid == Id && e.isactive && e.InBinFolder == false).SingleAsync();
            if (archive != null)
            {
                try
                {
                    model.Data = await this.fTPFileProvider01.DownloadFileByPath(archive.filepath);
                    var ext = Path.GetExtension(archive.filepath).ToLowerInvariant(); // ".pdf";
                    model.mimeType = UtilityFuns.GetMimeTypes()[ext];
                    model.FileName = archive.docdesc+archive.fileext;
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = ex.Message;
                }
            }
            else { model.ErrorMessage = "File not found in the directory"; }

            return model;
        }

        public async Task<bool> DeleteFile(long docid)
        {
            var file = await context.FileArchives.SingleAsync(e => e.docid == docid);
            if (file != null)
            {
                var isMoved = await fTPFileProvider01.MoveToRecycleBin(file.filepath);
                try
                {
                    if (isMoved)
                    {
                        file.InBinFolder = isMoved;
                        file.isactive = false;
                        await context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else return false;
        }
        public async Task<bool> RestoreFileFromBin(long docid)
        {
            var file = await context.FileArchives.SingleAsync(e => e.docid == docid && e.InBinFolder);
            if (file != null)
            {
                var isMoved = await fTPFileProvider01.RestoreFileFromRecycleBin(file.filepath);
                try
                {
                    if (isMoved)
                    {
                        file.InBinFolder = false;
                        await context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {

                    return false;
                }

            }
            else return false;
        }


        public async Task<bool> EmptyRecycleBin()
        {
            List<FileArchive> binFiles = new List<FileArchive>();
            binFiles = await context.FileArchives.Where(e => e.InBinFolder && e.isactive).ToListAsync();
            if (binFiles != null && binFiles.Count() > 0)
            {
                try
                {
                    for (int i = 0; i < binFiles.Count(); i++)
                    {
                        var isDeleted = await fTPFileProvider01.DeleteFilePermanently(binFiles[i].filepath, true);
                        binFiles[i].isactive = !isDeleted;
                    }
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    return false;
                }

            }
            return true;

        }
        public async Task<bool> DeletePermanently(long docid)
        {
            var file = await context.FileArchives.SingleAsync(o => o.docid == docid);
            if (file != null)
            {
                var IsDeleted = await fTPFileProvider01.DeleteFilePermanently(file.filepath, file.InBinFolder);
                if (IsDeleted)
                {
                    try
                    {
                        file.isactive = false;
                        file.InBinFolder = true;
                        await context.SaveChangesAsync();
                        return true;
                    }
                    catch
                    {

                        return false;
                    }
                }
            }
            return false;
        }

        private FileArchive ConvertObject(FileItem data)
        {
            FileArchive archive = new FileArchive();
            archive.docdesc = data.docdesc;
            archive.docfilename = data.docfilename;
            archive.docid = data.docid;
            archive.FileCatagoryId = data.FileCatagoryId;
            archive.fileext = data.fileext;
            archive.filepath = data.filepath;
            archive.filesize = data.filesize;
            archive.filestat = data.filestat;
            archive.userid = data.userid;
            archive.UpdateTime = data.UpdateTime;
            archive.RecDate = data.RecDate;
            archive.SortOrder = data.SortOrder;
            archive.isactive = data.isactive;
            archive.RecDate = data.RecDate;
            return archive;

        }
        private List<FileArchive> ConvertBulk(List<FileItem> datas)
        {
            List<FileArchive> result = new List<FileArchive>();
            foreach (var item in datas)
            {
                result.Add(ConvertObject(item));
            }
            return result;
        }

        public async Task<bool> DeletePermanentlyVendor(long docid)
        {
            Vendor vendor = await context.Vendors.FirstOrDefaultAsync(d => d.docid == docid);
            var file = await context.FileArchives.SingleAsync(o => o.docid == docid);
            if (file != null)
            {
                var IsDeleted = await fTPFileProvider01.DeleteFilePermanently(file.filepath, file.InBinFolder);
                if (IsDeleted)
                {
                    using (var scope = context.Database.BeginTransaction())
                    {
                        try
                        {
                            file.isactive = false;
                            file.InBinFolder = true;
                            context.SaveChanges();
                            vendor.docid = 0;
                            vendor.ImageUrl = null;
                            context.Entry(vendor).State = EntityState.Modified;
                            context.SaveChanges();
                            scope.Commit();
                            return true;
                        }
                        catch
                        {
                            scope.Rollback();
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        public async Task<FileArchiveViewModel> GetAllFilesByCompany(int id = 0, bool IsBinFiles = false, int companyId = 0)
        {
            FileArchiveViewModel model = new FileArchiveViewModel();
            model.FileArchive = await Task.Run(() => (from t1 in context.CustomerGroupInfoes.Where(d => d.IsActive==true && d.CompanyId == companyId)
                     join t2 in context.CustomerBookingFileMappings on t1.CGId equals t2.CGId
                     join t4 in context.ProductBookingInfoes on t1.CGId equals t4.CGId
                     join t3 in context.FileArchives.Where(d => d.FileCatagoryId==id&&d.InBinFolder== false) on t2.DocId equals t3.docid
                     select new FileArchiveViewModel
                     {
                         CompanyId=t1.CompanyId.Value,
                         CGId = t1.CGId,
                         Title=t2.FileTitel,
                         docid=t2.DocId,
                         filepath=t3.filepath,
                         FileNo=t4.FileNo,
                         FileCatagoryId=t3.FileCatagoryId,
                         fileext=t3.fileext,
                         InBinFolder=t3.InBinFolder
                     }).ToListAsync());
            return model;
        }
    }
}
