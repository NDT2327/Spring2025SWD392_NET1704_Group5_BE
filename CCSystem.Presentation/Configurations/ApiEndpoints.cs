namespace CCSystem.Presentation.Configurations
{
    public class ApiEndpoints
    {
        public string BaseUrl { get; set; }
        public AccountEndpoints Account { get; set; }
        public AuthenticationEndpoints Authentication { get; set; }
        public CategoryEndpoints Category { get; set; }
        public ServiceDetailEndpoints ServiceDetail { get; set; }
        

        public ServiceEndpoints Service { get; set; }

        public ApiEndpoints()
        {
            BaseUrl = "https://localhost:7207/api/v1/";
            Account = new AccountEndpoints();
            Authentication = new AuthenticationEndpoints();
            Category = new CategoryEndpoints();
            ServiceDetail = new ServiceDetailEndpoints();
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
            public string CreateAccount { get; } = "authentications/register";
            public string SearchAccount { get; } = "accounts/search";

            public string GetAccountDetailsUrl(int id) => $"accounts/profile/{id}";
            public string UpdateAccountUrl(int id) => $"accounts/update/{id}";
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

        //category
        public class CategoryEndpoints
        {
            public string GetAllCategories { get; } = "categories/getallcategories";
  
            public string CreateCategory { get; } = "categories/createcategory";
            public string GetCategory(int id) => $"categories/{id}";
            public string UpdateCategory(int id) => $"categories/updatecategory/{id}";
            public string DeleteCategory(int id) => $"categories/deletecategory/{id}";
            public string SearchCategory (string query) => $"categories/search?{query}";

        }

        //service

        //service detail
        public class ServiceDetailEndpoints
        {
            public string GetServiceDetailByService(int id) => $"servicedetail/service/{id}";
            public string GetServiceDetail(int id) => $"servicedetail/{id}";
            public string UpdateServiceDetail(int id) => $"servicedetail/update/{id}";
            public string DeleteServiceDetail(int id) => $"servicedetail/delete/{id}";
            public string CreateServiceDetail { get; } = "categories/create";
        }

        //promotion
        public class PromotionEndpoints
        {
            public string GetAllPromotions { get; } = "promotions";
            public string CreatePromotion { get; } = "promotions/create";
            public string GetPromtion(string code) => $"promotions/{code}";
            public string UpdatePromotion(string code) => $"promotions/update/{code}";
            public string DeletePromotion(string code) => $"promotions/delete/{code}";
        }

        //review

        //reports
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
