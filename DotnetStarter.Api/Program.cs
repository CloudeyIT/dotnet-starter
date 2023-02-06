using System.Reflection;
using Autofac;
using DotnetStarter.Core.Framework.Setup;
using Serilog;

[assembly: IncludeAssembly]

namespace DotnetStarter.Api;

public class Program
{
    public static async Task Main (params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.ConfigureContainer<ContainerBuilder>(container => { container.RegisterModule<ApiModule>(); });
        var app = builder.Setup();

        Log.Information(
            "Running version {Version}",
            typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        );
        await app.RunAsync();
    }
}