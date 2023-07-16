using DotnetStarter.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace DotnetStarter.Test;

public class FakeWebApplicationFactory : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost (IWebHostBuilder builder)
	{
		builder.UseEnvironment("Testing")
			.ConfigureAppConfiguration(
				(_, configurationBuilder) =>
				{
					configurationBuilder
						.AddJsonFile("appsettings.yaml", true)
						.AddJsonFile("appsettings.Testing.yaml", true)
						.AddJsonFile("appsettings.Testing.Local.yaml", true)
						.AddEnvironmentVariables("APP__");
				}
			);
	}
}