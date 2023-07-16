using Autofac;
using Cloudey.Reflex.Core;
using Cloudey.Reflex.Core.Setup;
using Cloudey.Reflex.Database;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.GraphQl;
using DotnetStarter.Core.Framework.Identity;
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

		builder.AddReflexCore((_, container) => { container.RegisterModule<CoreModule>(); });

		var assemblies = AppDomain.CurrentDomain.GetIncludedAssemblies().ToList();
		var configuration = builder.Configuration;

		builder.Services.AddGraphQl(assemblies);
		builder.Services.AddDatabase<MainDb>(configuration);
		builder.Services.AddIdentityModule(configuration);

		builder.Services.AddSwaggerGen(
			c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "DotnetStarter", Version = "v1" }); }
		);
		
		var app = builder.Build();

		app.UseRouting();
		app.UseWebSockets();
		app.UseIdentityModule();
		app.UseHttpExceptions();
		app.UseCors();
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