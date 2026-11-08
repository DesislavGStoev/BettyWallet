namespace BettyWallet.Core.Game;

/// <summary>
/// A slot game the player can bet on. The wallet and betting flow only depend on this,
/// so a different slot game can be plugged in by registering another implementation in Startup.
/// </summary>
public interface IGame
{
    decimal MinBet { get; }

    decimal MaxBet { get; }

    OperationError ValidateBet(decimal betAmount);

    decimal Spin(decimal betAmount);
}
