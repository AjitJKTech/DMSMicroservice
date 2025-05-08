namespace DMSMicroservice.AuthService
{
    public class ApiEndPoints
    {
        private const string Base = "/api";

        public class AuthEndPoints
        {
            public const string ApiBase = Base;
            public const string Login = ApiBase + "/Login";
            public const string Register = ApiBase + "/Register";
            public const string RefreshToken = ApiBase + "/RefreshToken";
            public const string Logout = ApiBase + "/Logout";

        }
    }
}
