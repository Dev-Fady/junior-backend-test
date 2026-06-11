
using junior_backend_test.Application.Exceptions;
using junior_backend_test.Application.Wapper;

namespace Bar.API.Middelware
{
    public class GlobalExceptionHandle
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandle> _logger;

        public GlobalExceptionHandle(RequestDelegate next , ILogger<GlobalExceptionHandle> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized && !context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";
                    var response = Response<object>.Fail("غير مصرح لك بالوصول (Unauthorized). يرجى التأكد من تسجيل الدخول وتمرير التوكن.");
                    await context.Response.WriteAsJsonAsync(response);
                }
                else if (context.Response.StatusCode == StatusCodes.Status403Forbidden && !context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";
                    var response = Response<object>.Fail("ليس لديك الصلاحية الكافية للوصول إلى هذا المورد (Forbidden).");
                    await context.Response.WriteAsJsonAsync(response);
                }
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                int statusCode;
                Response<object> response;

                //var service = context.RequestServices.GetRequiredService<SharedLocalizationService>();
                switch (ex)
                {
                    case UnauthorizedAccessException:
                        statusCode = StatusCodes.Status401Unauthorized;
                        response =  Response<object>.Fail(ex.Message);
                        break;

                    case NotFoundException:
                        statusCode = StatusCodes.Status404NotFound;
                        response = Response<object>.Fail(ex.Message);
                        break;

                    case BusinessException:
                        statusCode = StatusCodes.Status400BadRequest;
                        response = Response<object>.Fail(ex.Message);
                        break;


                    case TimeoutException:
                        statusCode = StatusCodes.Status408RequestTimeout;
                        response = Response<object>.Fail(ex.Message);
                        break;

                    default:
                        statusCode = StatusCodes.Status500InternalServerError;
                        response = Response<object>.Fail(ex.Message);
                        break;
                }

                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
