using Amazon.Runtime.Internal;
using copy_doc_api.DataAccessLayer;

namespace copy_doc_api.BusinessAccessLayer
{
    public class CopyDocBAL : ICopyDocBAL
    {
        private readonly ICopyDocDAL _copyDocDAL;
          private readonly IConfiguration _configuration;
        public CopyDocBAL(ICopyDocDAL copyDocDAL,IConfiguration configuration)
        {
           _copyDocDAL=copyDocDAL;
            _configuration=configuration;
        }
        public async Task<string> Copy( string sharePointSiteName, string sourceFolderName, string sourceDocumentName, string targetFolderName)
        {
            try
            {
              
                return await _copyDocDAL.Copy(sharePointSiteName,sourceFolderName, sourceDocumentName,targetFolderName);
            }
            catch (HttpRequestException ex)
            {
                return ex.Message;
            }
        }
    }
}
