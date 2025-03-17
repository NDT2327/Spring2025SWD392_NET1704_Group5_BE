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
            public const string GetAccountProfileEndpoint = AccountEndpoint + "/profile" + "/{id}";

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
            public const string UpdateServiceEndpoint = ServiceEndpoint + "/update" + "/{id}";
            public const string DeleteServiceEndpoint = ServiceEndpoint + "/delete" + "/{serviceid}";
            public const string GetServiceByIdEndpoint = ServiceEndpoint + "/{id}";
            public const string GetByCategoryId = ServiceEndpoint + "/category" + "/{id}";

        }

        public static class Payment
        {
            public const string PaymentEndpoint = ApiEndpoint + "/payments";
            public const string PaymentCallbackVnpay = PaymentEndpoint + "/PaymentCallbackVnpay";
            public const string PaymentIpnAction = ApiEndpoint + "/IpnAction";
            public const string PaymentRefund = ApiEndpoint + "/refund";
            public const string GetPaymentByCustomerId = ApiEndpoint + "/customer" + "/{id}";
            public const string GetPaymentByBookingId = ApiEndpoint + "/booking" + "/{id}";


        }

        // New Report Endpoints
        public static class Report
        {
            public const string ReportEndpoint = ApiEndpoint + "/reports";
            public const string GetAllReportsEndpoint = ReportEndpoint + "/getallreports";
            public const string GetReportByIdEndpoint = ReportEndpoint + "/{id}";
            public const string GetReportByHousekeeperIdEndpoint = ReportEndpoint +"/Housekeeper"+ "/{id}";
            public const string GetReportByAssignIdEndpoint = ReportEndpoint + "/assign" + "/{id}";
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
            public const string GetReviewByCustomerIdEndpoint = ReviewEndpoint + "/customer" + "/{id}";
            public const string GetReviewByDetailIdEndpoint = ReviewEndpoint + "/detail" + "/{detailId}";
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
            public const string GetServiceDetailByServiceIdEndpont = ServiceDetailEndPoint + "/service" + "/{id}";
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
            public const string GetBookingById = BookingEndpoint + "/{id}";
            public const string GetBookingByCusId = BookingEndpoint + "/customer" + "/{id}";
            public const string GetAllBookingsEndpoint = BookingEndpoint + "/getallbookings";
            public const string GetBookingByPromotionCode = BookingEndpoint + "/promotion" + "/{promotionCode}";
            public const string RequestCancel = BookingEndpoint + "/request-cancel";
            public const string ProcessRefund = BookingEndpoint + "/process-refund";
			public const string GetCancelRequestedBookingsEndpoint = BookingEndpoint + "/cancel-booking";

		}

		public static class BookingDetail
        {
            public const string BookingDetailEndpoint = ApiEndpoint + "/bookingDetails";
            public const string GetBDetailByIdEndpoint = BookingDetailEndpoint + "/{id}";
            public const string GetBDetailByBookIdEndpoint = BookingDetailEndpoint + "/booking" + "/{id}";
            public const string GetAllBDetailEndpoint = BookingDetailEndpoint + "/getallbookingdetail";
            public const string GetActiveBDetailEndpoint = BookingDetailEndpoint + "/active";
            public const string GetBookingDetailByServiceId = BookingDetailEndpoint + "/service" + "/{id}";
            public const string GetBookingDetailByServiceDetailId = BookingDetailEndpoint + "/servicedetail" + "/{id}";
            public const string RescheduleBookingDetail = BookingDetailEndpoint + "/reschedule" + "/{detailId}";
            public const string ConfirmReschedule = BookingDetailEndpoint + "/confirm" + "/{detailId}";
        }

        public static class ScheduleAssign
        {
            public const string ScheduleAssignEndpoint = ApiEndpoint + "/scheduleAssigns";
            public const string GetScheduleAssignByIdEndpoint = ScheduleAssignEndpoint + "/{id}";
            public const string GetScheduleAssignByHousekeeperIdEndpoint = ScheduleAssignEndpoint + "/housekeeper" + "/{id}";
            public const string ChangeStatusEndpoint = ScheduleAssignEndpoint + "/status";
            public const string CompleteAssignmentEndpoint = ScheduleAssignEndpoint + "/complete";
            public const string ConfirmAssignmentEndpoint = ScheduleAssignEndpoint + "/confirm";
            public const string HousekeeperRequestCancelEndpoint = ScheduleAssignEndpoint + "housekeeper-request-cancel";
			public const string GetRequestCancelEndpoint = ScheduleAssignEndpoint + "cancel-requests";
            public const string ConfirmHousekeeperCancelEndpoint = ScheduleAssignEndpoint + "confirm-housekeeper-cancel";


		}
	}
}
  
