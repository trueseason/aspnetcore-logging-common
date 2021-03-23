using System.Threading.Tasks;

namespace AspNetCore.Auditing.Common
{
    public interface IAuditingLogger
    {
        bool IsEnabled(string logType);
        Task LogAuditingInfoAsync(string logType, object logObject, string user);
    }
}
