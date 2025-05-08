namespace DMSMicroservice.DocumentManagementService
{
    public class ApiEndPoints
    {
        private const string Base = "/api";

        public class BlobApiEndPoints
        {
            public const string ApiBase = Base;
            public const string Upload = ApiBase + "/Upload";
        }
    }
}
