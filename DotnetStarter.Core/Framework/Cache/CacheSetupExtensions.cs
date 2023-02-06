namespace DotnetStarter.Core.Framework.Cache;

public static class CacheSetupExtensions
{
	public static IServiceCollection AddCache (this IServiceCollection services)
	{
		return services.AddMemoryCache()
			.AddSha256DocumentHashProvider();
	}
}