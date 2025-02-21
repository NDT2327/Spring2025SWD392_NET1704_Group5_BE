using CCSystem.API.Middlewares;

namespace CCSystem.API.Extentions
{
    public static class ExceptionMiddlewareExtention
    {
        public static void ConfigureExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
