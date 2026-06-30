using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Web.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Beklenmeyen bir hata oluştu. Path: {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                throw exception;
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (IsApiRequest(context))
            {
                context.Response.ContentType = "application/json";
                var response = new Dictionary<string, object>
                {
                    ["success"] = false,
                    ["message"] = "Beklenmeyen bir hata oluştu.",
                    ["statusCode"] = context.Response.StatusCode
                };

                if (_environment.IsDevelopment())
                {
                    response["detail"] = exception.Message;
                }

                await context.Response.WriteAsJsonAsync(response);
                return;
            }

            if (context.Request.Path.StartsWithSegments("/Home/Error"))
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync("Beklenmeyen bir hata oluştu.");
                return;
            }

            context.Response.Redirect("/Home/Error");
        }

        private static bool IsApiRequest(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/api") ||
                   context.Request.Headers.Accept.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase);
        }
    }
}