namespace BettyWallet.Core.Auditing;

/// <summary>Placeholder until logging infrastructure exists: accepts entries and discards them.</summary>
public sealed class NullAuditLog : IAuditLog
{
    public void Record(AuditEntry entry)
    {
    }
}
