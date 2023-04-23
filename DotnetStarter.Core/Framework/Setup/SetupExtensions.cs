using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudey.Reflex.Core;
using Cloudey.Reflex.Core.Configuration;
using Cloudey.Reflex.Core.Routing;
using Cloudey.Reflex.Core.Setup;
using Cloudey.Reflex.Database;
using DotnetStarter.Core.Framework.Cache;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.GraphQl;
using DotnetStarter.Core.Framework.Identity;
using DotnetStarter.Core.Framework.Validation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Opw.HttpExceptions.AspNetCore;

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
		builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
		builder.AddReflexConfiguration();
		builder.AddReflexLogging();

		builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

		builder.Services.AddControllers();

		var assemblies = AppDomain.CurrentDomain.GetIncludedAssemblies().ToList();
		var configuration = builder.Configuration;

		builder.Services.AddGraphQl(assemblies);
		builder.Services.AddCache();
		builder.Services.AddValidation(assemblies);
		builder.Services.AddDatabase<MainDb>(configuration);
		builder.Services.AddIdentityModule(configuration);

		builder.Services.AddCors();
		builder.Services.AddHttpContextAccessor();
		builder.Services.AddMvc()
			.AddControllersAsServices()
			.UseSlugRoutes()
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