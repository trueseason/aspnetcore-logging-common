namespace AspNetCore.Auditing.Common.Models
{
    public class AuditingLogType
    {
        public string Category { get; set; }
        public bool Enabled { get; set; }
        public bool LogPayload { get; set; }
        public string LogType { get; set; }
    }
}
