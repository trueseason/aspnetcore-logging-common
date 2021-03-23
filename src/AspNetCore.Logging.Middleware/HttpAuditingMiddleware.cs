﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Auditing.Common;
using AspNetCore.Auditing.Common.Models;

namespace AspNetCore.Logging.Middleware
{
    public class HttpAuditingMiddleware
    {
        private readonly IAuditingLogger _auditingLogger;
        private readonly AuditingConfig _auditingConfig;
        private readonly IHttpAuditingLogTypeLocator _locator;
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _streamManager;

        public HttpAuditingMiddleware(RequestDelegate next, IAuditingLogger auditingLogger, IOptions<AuditingConfig> auditingConfig, ILoggerFactory loggerFactory, IHttpAuditingLogTypeLocator locator)
        {
            _next = next;
            _auditingLogger = auditingLogger;
            _auditingConfig = auditingConfig.Value;
            _locator = locator;
            _logger = loggerFactory.CreateLogger<HttpAuditingMiddleware>();
            _streamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            string reqContent = null;
            AuditingLogType logType = null;
            Stream originalResStream = null;
            MemoryStream newResStream = null;

            try
            {
                if (_auditingConfig.AuditingEnabled)
                {
                    logType = _auditingConfig.LocateHttpAuditingLogType(context, _locator.Locate);
                    if (logType?.Enabled ?? false)  //default not to log when no logging entry configured
                    {
                        originalResStream = context.Response.Body;
                        newResStream = _streamManager.GetStream();
                        context.Response.Body = newResStream;
                        reqContent = await context.ReadRequestContent(logType.LogPayload);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetHttpAuditingMessage Exception");
            }

            await _next(context);

            if (reqContent != null)
            {
                try
                {
                    var resContent = await context.ReadResponseContent(logType.LogPayload);
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    await context.Response.Body.CopyToAsync(originalResStream);
                    await _auditingLogger.LogAuditingInfoAsync($"{logType.LogType}", $"{reqContent}{Environment.NewLine}{resContent}", context.User?.Claims?.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? "");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "LogAuditingInfo Exception");
                }
            }
        }
    }
}
