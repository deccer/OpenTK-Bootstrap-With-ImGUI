using System;
using Client.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Serilog.Events;

namespace Client
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = CreateServiceProvider();

            var game = serviceProvider.GetService<ClientGame>();
            game!.Run();
        }

        private static IServiceProvider CreateServiceProvider()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("client.log")
                .MinimumLevel.Is(LogEventLevel.Information)
                .CreateLogger();

            var services = new ServiceCollection();
            services.AddSingleton(Log.Logger);
            services.AddSingleton(new GameSettings(1280, 720, false));
            services.AddSingleton<ClientGame>();
            return services.BuildServiceProvider();
        }
    }
}
