using AspNetCore.Auditing.Common;
using AspNetCore.Auditing.Common.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Logging.Middleware
{
    public class AuditingAttribute : ActionFilterAttribute
    {
        private readonly IAuditingLogger _auditingLogger;
        private readonly AuditingConfig _auditingConfig;
        private readonly IHttpAuditingLogTypeLocator _locator;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _streamManager;

        public AuditingAttribute(IAuditingLogger auditingLogger, IOptions<AuditingConfig> auditingConfig, ILoggerFactory loggerFactory, IHttpAuditingLogTypeLocator locator)

        {
            _auditingLogger = auditingLogger;
            _auditingConfig = auditingConfig.Value;
            _locator = locator;
            _logger = loggerFactory.CreateLogger<HttpAuditingMiddleware>();
            _streamManager = new RecyclableMemoryStreamManager();
        }

        public sealed async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string reqContent = null;
            AuditingLogType logType = null;
            Stream originalResStream = null;
            MemoryStream newResStream = null;

            try
            {
                if (_auditingConfig.AuditingEnabled)
                {
                    logType = _auditingConfig.LocateHttpAuditingLogType(context.HttpContext, _locator.Locate);
                    if (logType?.Enabled ?? true)   //default to log when no logging entry configured
                    {
                        originalResStream = context.HttpContext.Response.Body;
                        newResStream = _streamManager.GetStream();
                        context.HttpContext.Response.Body = newResStream;
                        reqContent = await context.HttpContext.ReadRequestContent(logType.LogPayload);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetHttpAuditingMessage Exception");
            }

            await next();

            if (reqContent != null)
            {
                try
                {
                    var resContent = await context.HttpContext.ReadResponseContent(logType.LogPayload);
                    context.HttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    await context.HttpContext.Response.Body.CopyToAsync(originalResStream);
                    await _auditingLogger.LogAuditingInfoAsync($"{logType.LogType}", $"{reqContent}{Environment.NewLine}{resContent}", context.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? "");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "LogAuditingInfo Exception");
                }
            }
        }
    }
}
