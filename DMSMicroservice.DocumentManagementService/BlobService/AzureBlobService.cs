using Azure;
using Azure.Storage.Blobs;
using DMSMicroservice.DocumentManagementService.Models;

namespace DMSMicroservice.DocumentManagementService.BlobService
{
    public class AzureBlobService  : IAzureBlobService
    {
        
        private readonly BlobServiceClient _blobServiceClient;
        public AzureBlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        public async Task<AzureBlobResponse> UploadFileAsync(string blobContainer, string directoryName,
            string fileName, Stream fileStream)
        {
            try
            {
                // Create blob container if it doesn't exist
                var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainer);
                await containerClient.CreateIfNotExistsAsync();
                // Create blob client
                var blobClient = containerClient.GetBlobClient($"{directoryName}/{fileName}");
                // Upload the file
                var response = await blobClient.UploadAsync(fileStream, true);
                // Return the response
                return new AzureBlobResponse
                {
                    StatusCode = response.GetRawResponse().Status.ToString(),
                    ReasonPhrase = response.GetRawResponse().ReasonPhrase,
                    BlogUri = blobClient.Uri.ToString(),
                    IsError = false,
                    FileName = fileName,
                    FileBytes = null,
                    ErrorMessage = string.Empty
                };
            }
            catch (RequestFailedException)
            {
                return new AzureBlobResponse
                {
                    StatusCode = "400",
                    ReasonPhrase = "Bad Request",
                    BlogUri = string.Empty,
                    IsError = true,
                    FileName = fileName,
                    FileBytes = null,
                    ErrorMessage = "Request failed"
                };
            }
            catch (ArgumentNullException)
            {
                return new AzureBlobResponse
                {
                    StatusCode = "400",
                    ReasonPhrase = "Bad Request",
                    BlogUri = string.Empty,
                    IsError = true,
                    FileName = fileName,
                    FileBytes = null,
                    ErrorMessage = "File stream is null"
                };
            }
            catch (Exception ex)
            {
                return new AzureBlobResponse
                {
                    StatusCode = "500",
                    ReasonPhrase = "Internal Server Error",
                    BlogUri = string.Empty,
                    IsError = true,
                    FileName = fileName,
                    FileBytes = null,
                    ErrorMessage = ex.Message
                };
            }

            

        }
    }
   
}
