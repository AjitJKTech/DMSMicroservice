namespace DMSMicroservice.UserManagementService
{
    public class ApiEndPoints
    {
        private const string Base = "/api";
        public  class UserApiEndPoints
        {
            public const string ApiBase = Base;
            public const string Users = $"{Base}/GetAllUsers";
            public const string DeleteUser = $"{Base}/DeleteUser/{{id}}";
            public const string GetAllRoles = $"{Base}/GetAllRoles";
            public const string AddNewRole = $"{Base}/AddNewRole/{{roleName}}";
            public const string DeleteRole = $"{Base}/DeleteRole/{{id}}";
            public const string ChangeUserRole = $"{Base}/ChangeUserRole";
            public const string GetAdminInfo = $"{Base}/GetAdminInfo/{{email}}";
            public const string UpdateAdminInfo = $"{Base}/UpdateAdminInfo";
            public const string ChangeAdminPassword = $"{Base}/ChangeAdminPassword";
            
        }
    }
}
