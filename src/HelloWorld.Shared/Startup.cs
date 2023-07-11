using Microsoft.Extensions.DependencyInjection;

namespace HelloWorld.Shared;

public static class Startup
{
    public static ServiceProvider Services { get; private set; }

    public static void ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<INetworkService, NetworkService>();

        Services = serviceCollection.BuildServiceProvider();
    }
}
