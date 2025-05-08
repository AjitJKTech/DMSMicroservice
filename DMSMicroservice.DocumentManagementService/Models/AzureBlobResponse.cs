namespace DMSMicroservice.DocumentManagementService.Models
{
    public class AzureBlobResponse
    {
        public string StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public string BlogUri { get; set; }
        public bool IsError { get; set; }
        public string FileName { get; set; }
        public byte[]? FileBytes { get; set; }
        public string ErrorMessage { get; set; }



    }
}
