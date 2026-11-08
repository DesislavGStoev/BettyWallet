namespace BettyWallet.Core;

public readonly record struct OperationResult(OperationError Error)
{
    public bool IsSuccess => Error == OperationError.None;

    public static OperationResult Success { get; } = new(OperationError.None);

    public static OperationResult Failure(OperationError error) => new(error);
}
