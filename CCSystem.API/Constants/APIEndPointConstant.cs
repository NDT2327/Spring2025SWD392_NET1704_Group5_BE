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
            public const string UpdateAccountEndpoint = AccountEndpoint + "/update" + "/{id}";
            public const string GetAccountByIdEndpoint = AccountEndpoint + "/{id}";
            public const string GetAccountProfileEndpoint = AccountEndpoint + "/profile";

        }
        public static class Category
        {
            public const string CategoryEndpoint = ApiEndpoint + "/categories";
            public const string GetAllCategoriesEndpoint = CategoryEndpoint + "/getallcategories";
            public const string GetCategoryByIdEndpoint = CategoryEndpoint + "/{id}";
            public const string CreateCategoryEndpoint = CategoryEndpoint + "/createcategory";
            public const string UpdateCategoryEndpoint = CategoryEndpoint + "/updatecategory/{id}";
            public const string DeleteCategoryEndpoint = CategoryEndpoint + "/deletecategory/{id}";
            public const string SearchCategoryEndpoint = CategoryEndpoint + "/search";
        }

        public static class Service
        {
            public const string ServiceEndpoint = ApiEndpoint + "/services";
            public const string SearchServiceEndpoint = ServiceEndpoint + "/search";
            public const string GetServiceByIdEndpoint = ServiceEndpoint + "/{id}";


        }

        public static class Payment
        {
            public const string PaymentEndpoint = ApiEndpoint + "/payments";
            public const string PaymentCallbackVnpay = PaymentEndpoint + "/PaymentCallbackVnpay";
            public const string PaymentIpnAction = ApiEndpoint + "/IpnAction";
            public const string PaymentRefund = ApiEndpoint + "/refund";

        }

        // New Report Endpoints
        public static class Report
        {
            public const string ReportEndpoint = ApiEndpoint + "/reports";
            public const string GetAllReportsEndpoint = ReportEndpoint + "/getallreports";
            public const string GetReportByIdEndpoint = ReportEndpoint + "/{id}";
            public const string CreateReportEndpoint = ReportEndpoint + "/createreport";
            public const string UpdateReportEndpoint = ReportEndpoint + "/updatereport/{id}";
            public const string DeleteReportEndpoint = ReportEndpoint + "/deletereport/{id}";
        }

        // New Review Endpoints
        public static class Review
        {
            public const string ReviewEndpoint = ApiEndpoint + "/reviews";
            public const string GetAllReviewsEndpoint = ReviewEndpoint + "/getallreviews";
            public const string GetReviewByIdEndpoint = ReviewEndpoint + "/{id}";
            public const string CreateReviewEndpoint = ReviewEndpoint + "/createreview";
            public const string UpdateReviewEndpoint = ReviewEndpoint + "/updatereview/{id}";
            public const string DeleteReviewEndpoint = ReviewEndpoint + "/deletereview/{id}";
        }


        #region Service Detail
        /// <summary>
        /// Endpoint for managing service details.
        /// </summary>
        /// 
        public static class  ServiceDetail
        {
            public const string ServiceDetailEndPoint = ApiEndpoint + "/serviceDetail";
            public const string ServiceDetailByIdEndPoint = ServiceDetailEndPoint + "/{id}";
            public const string CreateServiceDetailEndPoint = ServiceDetailEndPoint + "/create";
            public const string UpdateServiceDetailEndPoint = ServiceDetailEndPoint + "/update/{id}";
            public const string DeleteServiceDetailEndPoint = ServiceDetailEndPoint + "/delete/{id}";
            public const string GetServiceDetailByServiceIdEndpont = ServiceDetailByIdEndPoint + "/getbyserviceId"; 
        }
        #endregion

        #region Promotions
        /// <summary>
        /// Endpoint for managing Promotions.
        /// </summary>
        /// 
        public static class Promotions
        {
            public const string PromotionEndPoint = ApiEndpoint + "/promotions";
            public const string PromotionByCodeEndPoint = PromotionEndPoint + "/{code}";
            public const string CreatePromotionEndPoint = PromotionEndPoint + "/create";
            public const string UpdatePromotionEndPoint = PromotionEndPoint + "/update/{code}";
            public const string DeletePromotionEndPoint = PromotionEndPoint + "/delete/{code}";
        }
        #endregion

        public static class Booking
        {
            public const string BookingEndpoint = ApiEndpoint + "/bookings";

        }

    }
}
  
