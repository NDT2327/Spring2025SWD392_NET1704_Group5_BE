namespace CCSystem.Presentation.Configurations
{
    public class ApiEndpoints
    {
        public string BaseUrl { get; set; }
        public AccountEndpoints Account { get; set; }
        public AuthenticationEndpoints Authentication { get; set; }
        public CategoryEndpoints Category { get; set; }
        public ServiceDetailEndpoints ServiceDetail { get; set; }
        public BookingDetailEndpoints BookingDetail { get; set; }
        public BookingEndpoints Booking { get; set; }
        public ReviewEndpoints Review { get; set; }
        public ReportEndpoints Report { get; set; }
        public AssignEndpoints Assign { get; set; }
        public ServiceEndpoints Service { get; set; }
        public PaymentEndpoints Payment { get; set; }
        public PromotionEndpoints Promotion { get; set; }

        public ApiEndpoints()
        {
            BaseUrl = "https://localhost:7207/api/v1/";
            //BaseUrl = "https://ccsystemapi20250305144905.azurewebsites.net/api/v1/";
            Account = new AccountEndpoints();
            Authentication = new AuthenticationEndpoints();
            Category = new CategoryEndpoints();
            ServiceDetail = new ServiceDetailEndpoints();
            Service = new ServiceEndpoints();
            Payment = new PaymentEndpoints();
            Review = new ReviewEndpoints();
            Booking = new BookingEndpoints();
            BookingDetail = new BookingDetailEndpoints();
            Report = new ReportEndpoints();
            Assign = new AssignEndpoints();
            Promotion = new PromotionEndpoints();

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

        //service detail
        public class ServiceDetailEndpoints
        {
            public string GetServiceDetailByService(int id) => $"servicedetail/service/{id}";
            public string GetServiceDetail(int id) => $"servicedetail/{id}";
            public string UpdateServiceDetail(int id) => $"servicedetail/update/{id}";
            public string DeleteServiceDetail(int id) => $"servicedetail/delete/{id}";
            public string CreateServiceDetail { get; } = "servicedetail/create";
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

        //assigns
        public class AssignEndpoints
        {
            public string GetAllScheduleAssigns { get; } = "scheduleassigns";
            public string CreateScheduleAssign { get; } = "scheduleassigns";
            public string UpdateScheduleAssign { get; } = "scheduleassigns/status";
            public string CompleteScheduleAssign { get; } = "scheduleassigns/complete";
            public string ConfirmScheduleAssign { get; } = "scheduleassigns/confirm";
            public string CancelScheduleAssign { get; } = "scheduleassignshousekeeper-request-cancel";
            public string GetCancelScheduleAssign { get; } = "scheduleassignscancel-requests";
            public string ConfirmCancelScheduleAssign { get; } = "scheduleassignsconfirm-housekeeper-cancel";

            public string GetAssign(int id) => $"scheduleassigns/{id}";
            public string GetAssingByHousekeeper(int id) => $"scheduleassigns/housekeeper/{id}";
        }

        //Booking
        public class BookingEndpoints
        {
            public string CreateBooking { get; } = "bookings";
            public string GetAllBookings { get; } = "bookings/getallbookings";
            public string CancelBooking { get; } = "bookings/request-cancel";
            public string Refund { get; } = "bookings/process-refund";
            public string GetCancelBookings { get; } = "bookings/cancel-booking";

            public string GetBooking(int id) => $"bookings/{id}";
            public string GetBookingByCustomer(int id) => $"bookings/customer/{id}";
            public string GetBookingByPromotionCode(string code) => $"bookings/promotion/{code}";

        }

        //Booking Detail
        public class BookingDetailEndpoints
        {
            public string GetActiveBookingDetails { get; } = "bookingdetails/active";
            public string GetAllBookingDetails { get; } = "bookingdetails/getallbookingdetail";
            public string GetChangeScheduleEndpoint { get; } = "bookingdetails/change-schedule";
            public string GetBookingDetail(int id) => $"bookingdetails/{id}";
            public string GetBookingDetailByBooking(int id) => $"bookingdetails/booking/{id}";
            public string GetDetailByService(int id) => $"bookingdetails/service/{id}";
            public string GetDetailByServiceDetail(int id) => $"bookingdetails/servicedetail/{id}";
            public string Reschedule(int id) => $"bookingdetails/reschedule/{id}";
            public string ConfirmBookingDetail(int id) => $"bookingdetails/confirm/{id}";
        }

        //Payment
        public class PaymentEndpoints {
            public string CreatePaymentUrl { get; } = "payments";
            public string UpdatePayment { get; } = "payments";
            public string IpnAction { get; } = "ipnaction";
            public string CallBackVnPay { get; } = "payments/paymentcallbackvnpay";
            public string GetByCustomer(int id) => $"customer/{id}";
        }



        //review
        public class ReviewEndpoints
        {
            public string GetAllReviews { get; } = "reviews/getallreviews";
            public string CreateReview { get; } = "reviews/createreview";
            public string GetByCustomer(int id) => $"reviews/customer/{id}";
            public string GetReview(int id) => $"reviews/{id}";
            public string GetByBookingDetail(int id) => $"reviews/detail/{id}";
            public string UpdateReview(int id) => $"reviews/updatereview/{id}";
            public string DeleteReview(int id) => $"reviews/deletereview{id}";
        }

        //reports
        public class ReportEndpoints
        {
            public string CreateReport { get; } = "reports/createreport";
            public string GetAllReports { get; } = "reports/getallreports";
            public string UpdateReport(int id) => $"reports/updatereport/{id}";
            public string DeleteReport(int id) => $"reports/deletereport/{id}";
            public string GetReport(int id) => $"reports/{id}";
            public string GetByHousekeeper(int id) => $"reports/housekeeper/{id}";
            public string GetByAssign(int id) => $"reports/assign/{id}";
        }

        //services
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
