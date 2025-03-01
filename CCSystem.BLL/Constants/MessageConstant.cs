using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSystem.BLL.Constants
{
    public static class MessageConstant
    {
        public static class CommonMessage
        {
            public const string NotExistEmail = "Email does not exist in the system.";
            public const string AlreadyExistEmail = "Email already exists in the system.";
            public const string NotExistAccountId = "Account id does not exist in the system.";
            public const string NotExistCategoryId = "Category id does not exist in the system";
            public const string NotExistCategoryName = "Category name does not exist in the system";
            public const string NotExistFile = "No file is provided";
            public const string NotExistServiceId = "Service id does not exist in the system";
            public const string NotExistPaymentId = "Payment id does not exist in the system";
            public const string NotExistBookingId = "Booking id does not exist in the system";
            public const string NotExistBookingDetailId = "Booking Detail id does not exist in the system";
            public const string NotExistServiceDetailId = "Service detail id does not exist in the system";

        }

        public static class AuthenticationMessage
        {
            public const string ResetPasswordSuccessfully = "Reset Password Successfully.";
        }

        public static class RegisterMessage
        {
            public const string FailRegister = "Register Failed";
        }

        public static class LoginMessage
        {
            public const string DisabledAccount = "Account has been disabled.";
            public const string InvalidEmailOrPassword = "Email or Password is invalid.";
        }

        public static class AccountMessage
        {
            public const string AccountIdNotBelongYourAccount = "Account id does not belong to your account.";
            public const string AccountNoLongerActive = "Your account is no longer active.";
        }

        public static class ReGenerationMessage
        {
            public const string InvalidAccessToken = "Access token is invalid.";
            public const string NotExpiredAccessToken = "Access token has not yet expired.";
            public const string NotExistAuthenticationToken = "You do not have the authentication tokens in the system.";
            public const string NotExistRefreshToken = "Refresh token does not exist in the system.";
            public const string NotMatchAccessToken = "Your access token does not match the registered access token.";
            public const string ExpiredRefreshToken = "Refresh token expired.";
        }

        public static class VerificationMessage
        {
            public const string SentEmailConfirmationSuccessfully = "Sent Email Confirmation Successfully.";
            public const string ConfirmedOTPCodeSuccessfully = "Confirmed OTP Code Successfully.";
            public const string NotAuthenticatedEmailBefore = "Email has not been previously authenticated.";
            public const string ExpiredOTPCode = "OTP code has expired.";
            public const string NotMatchOTPCode = "Your OTP code does not match the previously sent OTP code.";
        }

        public static class ChangePasswordMessage
        {
            public const string NotAuthenticatedEmail = "Email has not been previously authenticated.";
            public const string NotVerifiedEmail = "Email is not yet authenticated with the previously sent OTP code.";
        }

        public static class ServiceMessage
        {
            public const string CreatedNewServiceSuccessfully = "Create a new service Successfully";

        }

        public static class BookingMessage
        {
            public const string BookingIsPaid = "Booking is paid";

        }

    }
}
