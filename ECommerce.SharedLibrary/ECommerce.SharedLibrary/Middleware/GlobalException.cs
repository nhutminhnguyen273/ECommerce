using System.Net;
using System.Text.Json;
using ECommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Declare default variables
            string title = "Error";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Internal server error";

            try
            {
                await next(context);
                
                // Check if Response is Too Many Request => 429 status code
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    statusCode = StatusCodes.Status429TooManyRequests;
                    message = "Too many request";
                    await ModifyHeader(context, title, message, statusCode);
                }

                // If Response is UnAuthorized => 401 status code
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = "You are not authorized to access";
                    await ModifyHeader(context, title, message, statusCode);
                }

                // If Response is Forbidden => 403 status code
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Access";
                    statusCode = StatusCodes.Status403Forbidden;
                    message = "Your are not allowed//required to access";
                    await ModifyHeader(context, title, message, statusCode);
                }
            } 
            catch (Exception ex)
            {
                // Log Original Exceptions /File, Debugger, Console
                LogException.LogExceptions(ex);
                
                // Check if Exception is Timeout => 408 status code
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    statusCode = StatusCodes.Status408RequestTimeout;
                    message = "Request timeout... try again";
                }

                // If exception is caught
                // If none of the exception then do the default
                await ModifyHeader(context, title, message, statusCode); ;
            }
            
        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            // Display scary-free message to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title
            }), CancellationToken.None);
            return;
        }
    }
}
