using Autofac;
using DotnetStarter.Core.Framework.Setup;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.Identity;
using DotnetStarter.Core.Framework.Validation;

[assembly: IncludeAssembly]

namespace DotnetStarter.Core;

public class CoreModule : Module
{
    protected override void Load (ContainerBuilder builder)
    {
        builder.RegisterModule<DatabaseModule>();
        builder.RegisterModule<IdentityModule>();
        builder.RegisterModule<ValidationModule>();
    }
}