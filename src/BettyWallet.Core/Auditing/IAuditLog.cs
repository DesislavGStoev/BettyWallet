namespace BettyWallet.Core.Auditing;

/// <summary>
/// Records every attempted money operation for later review (disputes, compliance).
/// The app currently uses <see cref="NullAuditLog"/>; a real implementation plugs in once logging infrastructure exists.
/// </summary>
public interface IAuditLog
{
    void Record(AuditEntry entry);
}
