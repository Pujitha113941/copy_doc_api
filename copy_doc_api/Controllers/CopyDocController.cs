using copy_doc_api.BusinessAccessLayer;
using copy_doc_api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace copy_doc_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class CopyDocController : ControllerBase
    {
        private readonly ICopyDocBAL _copyDocBAL;
        private readonly ILogger<CopyDocController> _logger;

        public CopyDocController(ICopyDocBAL copyDocBAL, ILogger<CopyDocController> logger)
        {
            _copyDocBAL = copyDocBAL;
            _logger = logger;
        }

        [HttpPost("copy")]
        public async Task<ActionResult> CopyDoc([FromBody] Copy_Doc request)
        {
            try
            {
                string response = await _copyDocBAL.Copy(request.sharePointSiteName,request.sourceFolderName, request.sourceDocumentName, request.targetFolderName);
                if (response.Contains("Not Found"))
                {
                    return BadRequest(response);
                }
               
                if (response.Contains("Bad Request"))
                {
                    return BadRequest(response);
                }
               return Created(response,null); 
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Contains("NotFound"))
                {
                    ModelState.AddModelError("error", "Invalid");
                    return BadRequest(ModelState);
                }
                else
                {
                    return BadRequest(ex.Message);
                }

               
            }
        }
    }
}