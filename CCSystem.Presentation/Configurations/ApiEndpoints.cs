namespace CCSystem.Presentation.Configurations
{
    public class ApiEndpoints
    {
        public string BaseUrl { get; set; }
        public AccountEndpoints Account { get; set; }
        public AuthenticationEndpoints Authentication { get; set; }

        public ServiceEndpoints Service { get; set; }

        public ApiEndpoints()
        {
            BaseUrl = "https://localhost:7207/api/v1/";
            Account = new AccountEndpoints();
            Authentication = new AuthenticationEndpoints();
            Service = new ServiceEndpoints();
        }

        // Chuẩn hóa URL để tránh lỗi đường dẫn
        public string GetFullUrl(string endpoint)
        {
            return $"{BaseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
        }

        public class AccountEndpoints
        {
            public string GetAccounts { get; } = "accounts";
            public string GetAccountDetail { get; } = "accounts/profile/{id}";
            public string CreateAccount { get; } = "authentications/register";
            public string SearchAccount { get; } = "accounts/search";
            public string UpdateAccount { get; } = "update/{id}";
            public string LockAccount { get; } = "accounts/{id}/lock";
            public string UnlockAccount { get; } = "accounts/{id}/unlock";

            public string GetAccountDetailsUrl(int id) => $"accounts/profile/{id}";
            public string UpdateAccountUrl(int id) => $"update/{id}";
            public string LockAccountUrl(int id) => $"accounts/{id}/lock";
            public string UnlockAccountUrl(int id) => $"accounts/{id}/unlock";
        }

        public class AuthenticationEndpoints
        {
            public string Register { get; } = "authentications/register";
            public string Login { get; } = "authentications/login";
            public string EmailVerification { get; } = "authentications/email-verification";
            public string OtpVerification { get; } = "authentications/otp-verification";
            public string ResetPassword { get; } = "authentications/password-resetation";
            public string RegenerationTokens { get; } = "authentications/regeneration-tokens";
        }

        public class ServiceEndpoints
        {
            public string GetServices { get; } = "services";
            public string GetServiceById(int id) => $"services/{id}";
            public string GetServicesByCategory(int categoryId) => $"services/category/{categoryId}";
            public string CreateService { get; } = "services";
            public string UpdateService(int id) => $"services/update/{id}";
            public string DeleteService(int id) => $"services/delete/{id}";
            public string SearchServices { get; } = "services/search";
        }
    }
}
