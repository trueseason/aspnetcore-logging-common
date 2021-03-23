using Microsoft.Extensions.Options;
using AspNetCore.Auditing.Common.Models;

namespace AspNetCore.Auditing.Common
{
    public class AuditingEvaluator : IAuditingEvaluator
    {
        private readonly AuditingConfig _auditingConfig;

        public AuditingEvaluator(IOptions<AuditingConfig> auditingConfig)
        {
            _auditingConfig = auditingConfig.Value;
        }

        bool IAuditingEvaluator.Evaluate(string logTypeId)
        {
            return _auditingConfig.IsEnabled(logTypeId);
        }
    }
}
