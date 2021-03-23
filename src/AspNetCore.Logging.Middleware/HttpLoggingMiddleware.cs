using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Logging.Middleware
{
    public class HttpLoggingMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _streamManager;

        public HttpLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<HttpLoggingMiddleware>();
            _streamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            Stream originalResStream = null;
            MemoryStream newResStream = null;
            try
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    originalResStream = context.Response.Body;
                    newResStream = _streamManager.GetStream();
                    context.Response.Body = newResStream;
                    _logger.LogTrace(await context.ReadRequestContent());
                }
                else if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Service invoked: {context.Connection?.RemoteIpAddress} {context.User?.Claims?.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? ""} {context.Request?.Method} {context.Request?.Path}{context.Request?.QueryString.ToUriComponent()} {context.Request?.Protocol}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Http request logging Exception");
            }

            await _next(context);

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                try
                {
                    var resContent = await context.ReadResponseContent();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    await context.Response.Body.CopyToAsync(originalResStream);
                    _logger.LogTrace(resContent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Http response logging Exception");
                }
            }
        }
    }
}
