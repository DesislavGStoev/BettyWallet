using BettyWallet.ConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using var host = Startup.BuildHost(args);

WalletConsole console;
try
{
    console = host.Services.GetRequiredService<WalletConsole>();
}
catch (OptionsValidationException ex)
{
    Console.Error.WriteLine("The game configuration (appsettings.json or its overrides) is invalid:");
    foreach (var failure in ex.Failures)
    {
        Console.Error.WriteLine($"  - {failure}");
    }

    return 1;
}

console.Run();

Console.WriteLine();
Console.WriteLine("Press any key to exit.");
if (!Console.IsInputRedirected)
{
    Console.ReadKey(intercept: true);
}

return 0;
