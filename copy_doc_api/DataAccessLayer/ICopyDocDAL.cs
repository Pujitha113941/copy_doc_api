using copy_doc_api.Models;

namespace copy_doc_api.DataAccessLayer
{
    public interface ICopyDocDAL
    {
        Task<string> GetSourceFolderId(string sourceFolderName, string sharePointSiteName);
        Task<string> GetSourceDocumentId(string sourceFolderName,string sourceDocumentName,string sharePointSiteName);
        Task<string> GetTargetFolderId(string targetFolderName, string sharePointSiteName);
        Task<string> Copy(string sharePointSiteName, string sourceFolderName, string sourceDocumentName, string targetFolderName);
        Task<string> GetSiteId(string sharePointSiteName);

    }
}
