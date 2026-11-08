using BettyWallet.ConsoleApp;
using BettyWallet.Tests.Core;

namespace BettyWallet.Tests.ConsoleApp;

public class GameSettingsValidatorTests
{
    [Fact]
    public void Valid_settings_pass()
    {
        var result = new GameSettingsValidator().Validate(null, TestSettings.Default());

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Invalid_settings_fail_with_every_problem_listed()
    {
        var settings = TestSettings.Default();
        settings.MinBet = 0m;
        settings.Outcomes[0].Probability = 0.9m;

        var result = new GameSettingsValidator().Validate(null, settings);

        Assert.True(result.Failed);
        Assert.Equal(2, result.Failures!.Count());
    }
}
