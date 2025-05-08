namespace DMSMicroservice.DocumentManagementService.Models
{
    public class FileUploadRequest
    {
        public IFormFile File { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string EmailId { get; init; }
        public string UserId { get; init; }
        public string BlobContainer { get; set; }
        public string DirectoryName { get; set; }
        public string FileName { get; init; }
        public Stream FileStream { get; set; } = Stream.Null; // Initialize with a default value to avoid nullability issues  
    }
}
