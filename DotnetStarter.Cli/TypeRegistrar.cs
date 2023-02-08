using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using Spectre.Console.Cli;

namespace DotnetStarter.Cli;

public class TypeRegistrar : ITypeRegistrar
{
	private readonly IServiceCollection _builder;

	public TypeRegistrar (IServiceCollection services)
	{
		_builder = new ServiceCollection();
		services.ForEach(_builder.Add);
	}
	
	public void Register (Type service, Type implementation)
	{
		_builder.AddSingleton(service, implementation);
	}

	public void RegisterInstance (Type service, object implementation)
	{
		_builder.AddSingleton(service, implementation);
	}

	public void RegisterLazy (Type service, Func<object> func)
	{
		if (func is null)
		{
			throw new ArgumentNullException(nameof(func));
		}

		_builder.AddSingleton(service, _ => func());
	}
	
	public ITypeResolver Build ()
	{
		return new TypeResolver(_builder.BuildServiceProvider());
	}
}