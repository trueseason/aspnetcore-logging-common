using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using AspNetCore.Auditing.Common.Models;

namespace AspNetCore.Auditing.Common
{
    public abstract class AuditingLogger : IAuditingLogger
    {
        private readonly AuditingConfig _auditingConfig;
        private readonly IAuditingEvaluator _auditingEvaluator;

        public AuditingLogger(IOptions<AuditingConfig> auditingConfig, IAuditingEvaluator auditingEvaluator)
        {
            _auditingConfig = auditingConfig.Value;
            _auditingEvaluator = auditingEvaluator;
        }

        public bool IsEnabled(string logType)
        {
            return _auditingEvaluator.Evaluate(logType);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task LogAuditingInfoAsync(string logType, object logObject, string user)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
        }
    }
}
