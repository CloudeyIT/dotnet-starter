using Autofac;
using DotnetStarter.Cli;
using DotnetStarter.Core.Framework.Cli;
using DotnetStarter.Core.Framework.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MoreLinq;
using Spectre.Console;
using Spectre.Console.Cli;

var builder = WebApplication.CreateBuilder(args);

builder.Environment.EnvironmentName = "Cli";
builder.Host.ConfigureContainer<ContainerBuilder>(c => c.RegisterModule<CliModule>());
builder.Setup();
builder.Configuration.AddYamlFile("appsettings.Cli.yaml", true, true);
builder.Configuration.AddYamlFile("appsettings.Cli.Local.yaml", true, true);

var app = new CommandApp(new TypeRegistrar(builder.Services));

app.Configure(
	config =>
	{
		config.SetExceptionHandler(
			exception => { AnsiConsole.WriteException(exception); }
		);

		config.AddDelegate(
			"hello",
			_ =>
			{
				AnsiConsole.WriteLine("Hello World!");
				return 0;
			}
		);

		var branches = new Dictionary<string, List<Type>>();

		AppDomain.CurrentDomain.GetIncludedAssemblies()
			.ForEach(
				assembly => assembly.GetTypes()
					.Where(type => type.GetCustomAttributes(typeof(CommandAttribute), false).Any())
					.ToList()
					.ForEach(
						type =>
						{
							var command =
								(CommandAttribute)type.GetCustomAttributes(typeof(CommandAttribute), false).First();

							if (string.IsNullOrWhiteSpace(command.Group))
							{
								config.GetType().GetMethod(nameof(config.AddCommand))!.MakeGenericMethod(type)
									.Invoke(config, new object[] { command.Name });
							}
							else
							{
								if (!branches.ContainsKey(command.Group))
								{
									branches.Add(command.Group, new List<Type>());
								}

								branches[command.Group].Add(type);
							}
						}
					)
			);
		
		branches.ForEach(b => config.AddBranch(b.Key,
			branch =>
			{
				b.Value.ForEach(
					type =>
					{
						var command =
							(CommandAttribute)type.GetCustomAttributes(typeof(CommandAttribute), false).First();
						branch.GetType().GetMethod(nameof(branch.AddCommand))!.MakeGenericMethod(type)
							.Invoke(branch, new object[] { command.Name });
					}
				);
			}));
	}
);

await app.RunAsync(args);