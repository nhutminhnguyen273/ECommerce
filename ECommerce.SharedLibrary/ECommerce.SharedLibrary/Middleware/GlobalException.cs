using System.Net;
using ECommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;

namespace ECommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string title = "Error";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Internal server error";

            try
            {
                await next(context);
                
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    statusCode = StatusCodes.Status429TooManyRequests;
                    message = "Too many request";
                }
            } 
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
            }
            
        }
    }
}
