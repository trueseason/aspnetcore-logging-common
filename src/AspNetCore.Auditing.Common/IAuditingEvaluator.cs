namespace AspNetCore.Auditing.Common
{
    public interface IAuditingEvaluator
    {
        bool Evaluate(string logTypeId);
    }
}
