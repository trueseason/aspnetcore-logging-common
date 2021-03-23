using Microsoft.AspNetCore.Http;
using AspNetCore.Auditing.Common.Models;

namespace AspNetCore.Logging.Middleware
{
    public interface IHttpAuditingLogTypeLocator
    {
        AuditingLogType Locate(AuditingConfig auditingConfig, HttpContext context);
    }
}
