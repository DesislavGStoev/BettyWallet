using BettyWallet.Core.Game;
using Microsoft.Extensions.Options;

namespace BettyWallet.ConsoleApp;

/// <summary>Plugs the core's rule checks into the options pipeline, so bad config stops the app at startup.</summary>
public sealed class GameSettingsValidator : IValidateOptions<GameSettings>
{
    public ValidateOptionsResult Validate(string? name, GameSettings options)
    {
        var errors = options.Validate();
        return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
    }
}
