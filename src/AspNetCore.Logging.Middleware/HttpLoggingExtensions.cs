using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Auditing.Common.Models;

namespace AspNetCore.Logging.Middleware
{
    public static class HttpLoggingExtensions
    {
        private const int DEFAULT_REQUEST_SIZE = 500;
        private const int DEFAULT_RESPONSE_SIZE = 500;

        public static async Task<string> ReadRequestContent(this HttpContext context, bool readBody = true)
        {
            var request = context.Request;
            var strBuilder = new StringBuilder(DEFAULT_REQUEST_SIZE);
            strBuilder.AppendLine($"REQUEST {context.TraceIdentifier}:");
            strBuilder.AppendLine($"{context.Connection?.RemoteIpAddress} {context.User?.Claims?.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? ""}");
            strBuilder.AppendLine($"{request.Method} {request.Path}{request.QueryString.ToUriComponent()} {request.Protocol}");
            if (request.Headers.Any())
            {
                foreach (var header in request.Headers)
                {
                    strBuilder.AppendLine($"{header.Key}: {header.Value}");
                }
            }
            if (readBody)
            {
                strBuilder.AppendLine();
                strBuilder.AppendLine(await request.ReadBody());
            }

            return strBuilder.ToString();
        }

        public static async Task<string> ReadResponseContent(this HttpContext context, bool readBody = true)
        {
            var response = context.Response;
            var strBuilder = new StringBuilder(DEFAULT_RESPONSE_SIZE);
            strBuilder.AppendLine($"RESPONSE {context.TraceIdentifier}:");
            strBuilder.AppendLine($"{response.HttpContext.Request.Protocol} {response.StatusCode} {(HttpStatusCode)response.StatusCode}");
            if (response.Headers.Any())
            {
                foreach (var header in response.Headers)
                {
                    strBuilder.AppendLine($"{header.Key}: {header.Value}");
                }
            }
            if (readBody)
            {
                strBuilder.AppendLine();
                strBuilder.AppendLine(await response.ReadBody());
            }

            return strBuilder.ToString();
        }

        public static async Task<string> ReadBody(this HttpRequest request)
        {
            request.EnableBuffering();
            request.Body.Seek(0, SeekOrigin.Begin);
            var reqContent = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return reqContent;
        }

        public static async Task<string> ReadBody(this HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var resContent = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return resContent;
        }

        public static IApplicationBuilder UseHttpLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpLoggingMiddleware>();
        }

        public static AuditingLogType LocateHttpAuditingLogType(this AuditingConfig auditingConfig, HttpContext context, Func<AuditingConfig, HttpContext, AuditingLogType> locate)
        {
            if (locate == null)
                return null;

            return locate(auditingConfig, context);
        }

        public static IApplicationBuilder UseHttpAuditing(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpAuditingMiddleware>();
        }
    }
}
