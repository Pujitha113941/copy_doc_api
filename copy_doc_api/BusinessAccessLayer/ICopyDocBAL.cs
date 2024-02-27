namespace copy_doc_api.BusinessAccessLayer
{
    public interface ICopyDocBAL
    {
        Task<string> Copy(string sharePointSiteName, string sourceFolderName, string sourceDocumentName, string targetFolderName);
    }
}
