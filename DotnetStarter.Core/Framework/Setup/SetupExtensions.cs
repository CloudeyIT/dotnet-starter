using Autofac;
using Autofac.Extensions.DependencyInjection;
using DotnetStarter.Core.Framework.Cache;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.GraphQl;
using DotnetStarter.Core.Framework.Identity;
using DotnetStarter.Core.Framework.Routing;
using DotnetStarter.Core.Framework.Validation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi.Models;
using Opw.HttpExceptions.AspNetCore;
using Sentry.Extensibility;
using Sentry.Serilog;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace DotnetStarter.Core.Framework.Setup;

public static class SetupExtensions
{
	/// <summary>
	///     Setup the application and register all the services and modules
	/// </summary>
	/// <param name="builder">The WebApplicationBuilder from Program.cs</param>
	/// <returns>The configured WebApplicationBuilder</returns>
	public static WebApplication Setup (this WebApplicationBuilder builder)
	{
		builder.Configuration.AddJsonFile("appsettings.json", true, true);
		builder.Configuration.AddYamlFile("appsettings.yaml", true, true);
		builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
		builder.Configuration.AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.yaml", true, true);
		builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);
		builder.Configuration.AddYamlFile("appsettings.Local.yaml", true, true);
		builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.Local.json", true, true);
		builder.Configuration.AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.Local.yaml", true, true);
		builder.Configuration.AddEnvironmentVariables("APP__");

		var logLevel = builder.Environment.IsProduction() ? LogEventLevel.Information : LogEventLevel.Verbose;
		var sentryConfig = builder.Configuration.GetSection("Sentry").Get<SentrySerilogOptions>();

		builder.Host.UseSerilog(
			(_, configuration) => configuration
				.Enrich.FromLogContext()
				.Enrich.WithExceptionDetails()
				.Enrich.WithEnvironmentName()
				.Enrich.WithMachineName()
				.WriteTo.Console(logLevel)
				.WriteTo.Debug(logLevel)
				.WriteTo.Sentry(
					sentry =>
					{
						if (sentryConfig?.Dsn is null) return;

						sentry.Dsn = sentryConfig.Dsn;
						sentry.TracesSampleRate = sentryConfig.TracesSampleRate;
						sentry.AttachStacktrace = true;
						sentry.InitializeSdk = true;
						sentry.AutoSessionTracking = true;
					}
				)
		);

		if (sentryConfig?.Dsn is not null)
		{
			builder.WebHost.UseSentry(
				(_, sentry) =>
				{
					sentry.InitializeSdk = false;
					sentry.MaxRequestBodySize = RequestSize.Always;
				}
			);
		}

		builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

		builder.Services.AddControllers();

		var assemblies = AppDomain.CurrentDomain.GetIncludedAssemblies().ToList();
		var configuration = builder.Configuration;

		builder.Services.AddGraphQl(assemblies);
		builder.Services.AddCache();
		builder.Services.AddValidation(assemblies);
		builder.Services.AddDatabase(configuration);
		builder.Services.AddIdentityModule(configuration);

		builder.Services.AddCors();
		builder.Services.AddHttpContextAccessor();
		builder.Services.AddMvc()
			.AddControllersAsServices()
			.AddMvcOptions(
				mvcOptions =>
				{
					mvcOptions.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
				}
			)
			.AddHttpExceptions();
		builder.Services.AddFluentValidationAutoValidation()
			.AddFluentValidationClientsideAdapters();

		builder.Services.AddSwaggerGen(
			c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "DotnetStarter", Version = "v1" }); }
		);


		builder.Host.ConfigureContainer<ContainerBuilder>(container => { container.RegisterModule<CoreModule>(); });

		var app = builder.Build();

		app.UseRouting();
		app.UseWebSockets();
		app.UseIdentityModule();
		app.UseHttpExceptions();
		app.UseCors(
			cors => cors
				.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" })
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials()
		);
		app.MapControllers();
		app.MapGraphQL();
		app.MapGet(
			"/",
			context =>
			{
				context.Response.Redirect("/graphql", true);
				return Task.CompletedTask;
			}
		);

		app.UseAuthorization();
		app.MapControllers();

		return app;
	}
}