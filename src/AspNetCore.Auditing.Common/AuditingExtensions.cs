using System;
using System.Linq;
using AspNetCore.Auditing.Common.Models;

namespace AspNetCore.Auditing.Common
{
    public static class AuditingExtensions
    {
        public static bool IsEnabled(this AuditingConfig auditingConfig)
        {
            return auditingConfig.AuditingEnabled;
        }

        public static bool IsEnabled(this AuditingConfig auditingConfig, string logTypeId, string category = null)
        {
            if (!auditingConfig.AuditingEnabled)
                return auditingConfig.AuditingEnabled;

            var logType = auditingConfig.GetAuditingLogType(logTypeId, category);
            return logType?.Enabled ?? false;
        }

        public static AuditingLogType GetAuditingLogType(this AuditingConfig auditingConfig, string logType, string category = null)
        {
            return (from item in auditingConfig?.LogTypes
                    where string.Equals(item.LogType, logType, StringComparison.OrdinalIgnoreCase)
                    && (category == null || string.Equals(item.Category, category, StringComparison.OrdinalIgnoreCase))
                    select item).FirstOrDefault();
        }
    }
}
