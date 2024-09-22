using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using KGERP.Service.ServiceModel.FTP_Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Implementation.FTP
{
    public interface IFTPService
    {
        Task<FileItem> UploadFile(FileItem file, string foldername = "", bool isbulkUpload = false);
        Task<List<FileItem>> UploadFileBulk(List<FileItem> files, string FolderName = "");
        Task<bool> FileMapping(List<FileItem> itemlist, long CGId, bool isReplaced);
        Task<List<vwFTPFileInfo>> GetAllFilesByCatagory(int id = 0, bool IsBinFiles = false);
        Task<FileArchiveViewModel> GetAllFilesByCompany(int id = 0, bool IsBinFiles = false,int companyId=0);
        Task<FilePreviewModel> GetFileById(int Id);
        Task<bool> DeleteFile(long docid);
        Task<bool> DeletePermanently(long docid);
        Task<bool> DeletePermanentlyVendor(long docid);
        Task<bool> RestoreFileFromBin(long docid);
        Task<bool> EmptyRecycleBin();
        Task<FileUrl> GetFilePathByDocId(long docid);
    }
}
