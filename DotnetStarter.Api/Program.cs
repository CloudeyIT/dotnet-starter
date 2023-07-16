using System.Reflection;
using Autofac;
using Cloudey.Reflex.Core.Setup;
using DotnetStarter.Api;
using DotnetStarter.Core.Framework.Setup;
using Serilog;

[assembly: IncludeAssembly]

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureContainer<ContainerBuilder>(container => { container.RegisterModule<ApiModule>(); });
var app = builder.Setup();

Log.Information(
	"Running version {Version}",
	typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
);

await app.RunAsync();

public partial class Program { }