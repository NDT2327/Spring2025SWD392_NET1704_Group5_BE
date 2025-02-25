namespace CCSystem.API.Constants
{
    public static class APIEndPointConstant
    {
        private const string RootEndPoint = "/api";
        private const string ApiVersion = "/v1";
        private const string ApiEndpoint = RootEndPoint + ApiVersion;


        public static class Authentication
        {
            public const string AuthenticationEndpoint = ApiEndpoint + "/authentications";
            public const string Login = AuthenticationEndpoint + "/login";
            public const string Register = AuthenticationEndpoint + "/register";
            public const string ReGenerationTokens = AuthenticationEndpoint + "/regeneration-tokens";
            public const string PasswordResetation = AuthenticationEndpoint + "/password-resetation";
            public const string EmailVerificationEndpoint = AuthenticationEndpoint + "/email-verification";
            public const string OTPVerificationEndpoint = AuthenticationEndpoint + "/otp-verification";
        }

        public static class Account
        {
            public const string AccountEndpoint = ApiEndpoint + "/accounts";
            public const string SearchAccountsEndpoint = AccountEndpoint + "/search";
            public const string LockAccountEndpoint = AccountEndpoint + "/{id}" + "/lock";
            public const string UnlockAccountEndpoint = AccountEndpoint + "/{id}" + "/unlock";

        }
        public static class Category
        {
            public const string CategoryEndpoint = ApiEndpoint + "/categories";
            public const string GetAllCategoriesEndpoint = CategoryEndpoint + "/getallcategories";
            public const string GetCategoryByIdEndpoint = CategoryEndpoint + "/{id}";
            public const string CreateCategoryEndpoint = CategoryEndpoint + "/createcategory";
            public const string UpdateCategoryEndpoint = CategoryEndpoint + "/updatecategory/{id}";
            public const string DeleteCategoryEndpoint = CategoryEndpoint + "/deletecategory/{id}";
        }

        public static class Service
        {
            public const string ServiceEndpoint = ApiEndpoint + "/services";
            public const string SearchServiceEndpoint = ServiceEndpoint + "/search";

        }
    }
}
