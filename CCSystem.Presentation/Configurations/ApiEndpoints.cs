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
            public string CreateAccount { get; } = "accounts";
            public string SearchAccount { get; } = "account/search";
            public string UpdateAccount { get; } = "update/{id}";
            public string LockAccount { get; } = "account/{id}/lock";
            public string UnlockAccount { get; } = "account/{id}/unlock";

            public string GetAccountDetailsUrl(string id) => $"accounts/profile/{id}";
            public string UpdateAccountUrl(string id) => $"update/{id}";
            public string LockAccountUrl(string id) => $"account/{id}/lock";
            public string UnlockAccountUrl(string id) => $"account/{id}/unlock";
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
        }
    }
}
