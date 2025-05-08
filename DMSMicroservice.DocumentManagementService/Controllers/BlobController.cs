using DMSMicroservice.DocumentManagementService.BlobService;
using DMSMicroservice.DocumentManagementService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMSMicroservice.DocumentManagementService.Controllers
{
    [ApiController]
    public class BlobController : ControllerBase
    {

        private readonly IAzureBlobService _azureBlobService;
        public BlobController(IAzureBlobService azureBlobService)
        {
            _azureBlobService = azureBlobService;
        }
        [HttpPost(ApiEndPoints.BlobApiEndPoints.Upload),DisableRequestSizeLimitAttribute]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadRequest fileUploadRequest)
        {
            if (fileUploadRequest?.File == null || fileUploadRequest.File.Length == 0)
            {
                return BadRequest("Invalid file upload request.");
            }

            try
            {
                var response = await _azureBlobService.UploadFileAsync(fileUploadRequest.BlobContainer, fileUploadRequest.DirectoryName,
                    fileUploadRequest.FileName, fileUploadRequest.FileStream);

                if (response != null && response.IsError)
                {
                    return Ok("File uploaded successfully.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, response?.ErrorMessage ?? "File upload failed.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}
                 