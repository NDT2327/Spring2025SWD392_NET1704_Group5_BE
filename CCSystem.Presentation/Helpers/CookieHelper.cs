namespace CCSystem.Presentation.Helpers
{
    public class CookieHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CookieHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        //save token into cookie
        public void SetCookie(string key, string value, int expireMinute)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,//prevent javascript to access
                Secure = true, //work on Https
                SameSite = SameSiteMode.Strict, //prevent CSRF attack
                Expires = DateTime.UtcNow.AddMinutes(expireMinute),
            };
            _contextAccessor.HttpContext?.Response.Cookies.Append(key, value, options);
        }
        //get token
        public string? GetCookie(string key)
        {
            return _contextAccessor.HttpContext?.Request.Cookies[key];
        }
        //remove token
        public void RemoveCookie(string key)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(key);

        }
    }
}
