using BettyWallet.Core;
using BettyWallet.Core.Auditing;
using BettyWallet.Core.Game;
using BettyWallet.Core.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BettyWallet.ConsoleApp;

public static class Startup
{
    /// <summary>
    /// Builds the app from appsettings.json (plus environment variables and command-line overrides).
    /// Everything is a singleton: one process, one player, one wallet.
    /// </summary>
    public static IHost BuildHost(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = args,
            // Find appsettings.json next to the executable, not in whatever folder it was started from.
            ContentRootPath = AppContext.BaseDirectory,
        });

        // Log output would mix with the game's console output.
        builder.Logging.ClearProviders();

        builder.Services.AddOptions<GameSettings>().BindConfiguration(GameSettings.SectionName);
        builder.Services.AddSingleton<IValidateOptions<GameSettings>, GameSettingsValidator>();
        builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<GameSettings>>().Value);

        builder.Services.AddSingleton(TimeProvider.System);
        // Dummies until logging/metrics infrastructure exists. Swap in real implementations here.
        builder.Services.AddSingleton<IAuditLog, NullAuditLog>();
        builder.Services.AddSingleton<IMetrics, NullMetrics>();

        builder.Services.AddSingleton<IRandomSource>(new SystemRandomSource());
        builder.Services.AddSingleton<IGame, SlotGame>();
        builder.Services.AddSingleton<IWallet, Wallet>();
        builder.Services.AddSingleton<BettingService>();
        builder.Services.AddSingleton(sp => new WalletConsole(
            sp.GetRequiredService<IWallet>(),
            sp.GetRequiredService<BettingService>(),
            sp.GetRequiredService<IGame>(),
            Console.In,
            Console.Out));

        return builder.Build();
    }
}
