using DMSMicroservice.DocumentManagementService.Models;

namespace DMSMicroservice.DocumentManagementService.BlobService
{
    public interface IAzureBlobService
    {
        Task<AzureBlobResponse> UploadFileAsync(string blobContainer, string directoryName, 
            string fileName, Stream fileStream );
    }
}
