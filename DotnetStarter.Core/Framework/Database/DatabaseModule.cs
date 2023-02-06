using Autofac;
using DotnetStarter.Core.Framework.Setup;
using EntityFrameworkCore.Triggers;

namespace DotnetStarter.Core.Framework.Database;

public class DatabaseModule : Module
{
    protected override void Load (ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(Triggers<,>))
            .As(typeof(ITriggers<,>))
            .SingleInstance();

        builder.RegisterGeneric(typeof(Triggers<>))
            .As(typeof(ITriggers<>))
            .SingleInstance();

        builder.RegisterType(typeof(Triggers))
            .As(typeof(ITriggers))
            .SingleInstance();

        AppDomain.CurrentDomain.GetIncludedAssemblies()
            .ForEach(
                assembly => builder.RegisterAssemblyTypes(assembly)
                    .AssignableTo<Entity>()
                    .AsSelf()
            );

        builder.Register(
                context => context.Resolve<IDbContextFactory<MainDb>>().CreateDbContext()
            )
            .AsSelf()
            .AsImplementedInterfaces();
    }
}