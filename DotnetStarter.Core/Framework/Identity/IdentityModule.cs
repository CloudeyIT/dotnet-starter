using Autofac;
using Cloudey.Reflex.Authorization;
using Cloudey.Reflex.Core.Setup;
using DotnetStarter.Core.Framework.Identity.Services;

namespace DotnetStarter.Core.Framework.Identity;

public class IdentityModule : Module
{
	public const string Name = "Identity";

	protected override void Load (ContainerBuilder builder)
	{
		var assemblies = AppDomain.CurrentDomain.GetIncludedAssemblies().ToArray();

		builder.RegisterType<TokenService>()
			.AsSelf()
			.SingleInstance();

		builder.RegisterReflexAuthorization(assemblies);
	}
}