﻿using System.IO;
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
						.AddYamlFile("appsettings.yaml", true)
						.AddYamlFile("appsettings.Testing.yaml", true)
						.AddYamlFile("appsettings.Testing.Local.yaml", true)
						.AddEnvironmentVariables("APP__");
				}
			);
	}
}