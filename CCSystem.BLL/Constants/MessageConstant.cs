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
            public const string NotExistAssignId = "Assign id does not exist in the system";



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
            public const string NoMatchingAccountsFound = "No accounts matching the criteria were found.";
            public const string AccountIdNotBelongYourAccount = "Account id does not belong to your account.";
            public const string AccountNoLongerActive = "Your account is no longer active.";
            public const string InvalidDateOfBirth = "Invalid date values.";
            public const string AccountNotExistOrAlreadyLocked = "Account does not exist or is already locked.";
            public const string AccountLockedSuccessfully = "Account locked successfully.";
            public const string InternalServerError = "An internal error occurred.";
            public const string AccountNotFound = "Account not found.";
            public const string InvalidRequest = "Invalid request data.";
            public const string AccountUpdatedSuccessfully = "Account updated successfully.";
            public const string UpdateError = "An error occurred while updating the account.";
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

        public static class ScheduleAssign
        {
            public const string AlreadyAssign = "Housekeeper already has an assigned task in this time slot";
            public const string CreatedNewScheduleAssignSuccessfully = "Create a new Schedule Assignment Successfully";
            public const string StatusValidate = "Status request must ASSIGNED or CANCELLED or COMPLETED or INPROGRESS";
        }

        public static class PaymentMessage
        {
            public const string StatusMustSuccessOrFailedorPending = "Status request must SUCCESS or FAILED or PENDING";
        }
        public static class ReviewMessage
        {
            public const string ValidationFailed = "Validation failed.";
            public const string EmptyReviewRequest = "Review request cannot be empty.";
            public const string NoReviewsFound = "No reviews found for this customer.";
            public const string Success = "Reviews retrieved successfully.";
            public const string UpdatedSuccessfully = "Review with ID {0} has been updated successfully.";
            public const string CreatedSuccessfully = "Review created successfully.";
            public const string CreateFailed = "An error occurred while creating the review.";
            public const string DeletedSuccessfully = "Review with ID {0} deleted successfully.";
            public const string DeleteFailed = "An error occurred while deleting the review.";
        }
        public static class CategoryMessage
        {
            public const string UpdateCategoryFailed = "Failed to update category.";
            public const string CategoryNotFound = "Category not found.";
            public const string Success = "Success";
            public const string CategoryDeleted = "Category deleted successfully.";
            public const string CategoryUpdated = "Category updated successfully.";
            public const string CategoryCreated = "Category created successfully.";
        }

        public static class PromotionMessage
            {
            public const string PromotionUpdated = "Promotion updated successfully.";
            public const string AlreadyExistPromotionCode = "Promotion code already exists in the system.";
            public const string FailedToCreatePromotion = "Failed to create promotion.";
            public const string NotExistPromotion = "Promotion not found.";
            public const string FailedToUpdatePromotion = "Failed to update promotion.";
            public const string InvalidRequest = "Request body cannot be null.";
            public const string ValidationFailed = "Validation failed.";
            public const string PromotionDeleted = "Promotion deleted successfully.";


        }

        public static class ReportMessage
        {
            public const string ReportCreated = "Report created successfully.";
            public const string EmptyRequest = "Request cannot be empty.";
            public const string FailedToCreateReport = "Failed to create report.";
            public const string ReportNotFound = "Report with ID {0} not found.";
            public const string InvalidHousekeeperOrAssignId = "HousekeeperId and AssignId must have valid values.";
        }
    }


    }

