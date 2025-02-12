using CCSystem.BLL.Errors;
using CCSystem.BLL.Exceptions;
using Newtonsoft.Json;

namespace CCSystem.API.Middlewares
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            //define logging
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                //logging
                await next(context);
            }
            catch (Exception ex)
            {
                //logging
                await HandleException(context, ex);
            }
        }

        private static async Task HandleException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            switch (ex)
            {
                case NotFoundException _:
                    context.Response.StatusCode = (int)StatusCodes.Status404NotFound;
                    break;
                case BadRequestException _:
                    context.Response.StatusCode = (int)StatusCodes.Status400BadRequest;
                    break;
                case ConflictException _:
                    context.Response.StatusCode = (int)StatusCodes.Status409Conflict;
                    break;
                default:
                    context.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
                    break;
            }

            //Error error = new Error()
            //{
            //    StatusCode = context.Response.StatusCode,
            //    Message = JsonConvert.DeserializeObject<List<ErrorDetail>>(ex.Message)
            //};
            List<ErrorDetail> errorDetails;
            try
            {
                // Cố gắng deserialize ex.Message thành danh sách ErrorDetail
                errorDetails = JsonConvert.DeserializeObject<List<ErrorDetail>>(ex.Message);
            }
            catch
            {
                // Nếu không thành công, tạo danh sách mặc định với ex.Message
                errorDetails = new List<ErrorDetail>
                {
                    new ErrorDetail
                    {
                    FieldNameError = "Exception",
                    DescriptionError = new List<string> { ex.Message }
                    }
                };
            }

            // Tạo đối tượng Error với thông tin mã trạng thái và danh sách ErrorDetail
            var error = new Error
            {
                StatusCode = context.Response.StatusCode,
                Message = errorDetails
            };

            // Serialize đối tượng Error thành JSON và gửi về client
            string json = JsonConvert.SerializeObject(error);
            await context.Response.WriteAsync(json);


            //await context.Response.WriteAsync(error.ToString());


        }
    }
}
