namespace CCSystem.Presentation.Constants
{
    public static class Message
    {
        public static class AccountMessage
        {
            public const string AccountLockedSuccessfully = "Account has been locked successfully";
            public const string AccountLockedFailed = "Account cannot be locked";
            public const string AccountUnlockedSuccessfully = "Account has been unlocked successfully";
            public const string AccountUnlockedFailed = "Account cannot be unlocked";
            public const string AccountUpdatedSuccessfully = "Account has been update successfully";
            public const string AccountCreatedSuccessfully = "Account has been created successfully";
            public const string AccountCreatedFailed = "Account created failed";
        }

        public static class AuthenMessage
        {
            public const string LoginSuccess = "Login Successfully!";
            public const string LoginFailed = "Login Failure, please check your email and password!";
            public const string LogoutSuccess = "Logout Successfully!";
        }


        public static class CommonMessage
        {
            public const string IdNotFound = "ID is required";
        }
    }
}
