using System.Collections.Generic;

namespace AspNetCore.Auditing.Common.Models
{
    public class AuditingConfig
    {
        public bool AuditingEnabled { get; set; }
        public int AuditLogTTLDays { get; set; }
        public string SourceName { get; set; }
        public List<AuditingLogType> LogTypes { get; set; }
    }
}
