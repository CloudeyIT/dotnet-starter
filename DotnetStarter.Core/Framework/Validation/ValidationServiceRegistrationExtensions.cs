using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace DotnetStarter.Core.Framework.Validation;

public static class ValidationServiceRegistrationExtensions
{
    public static IServiceCollection AddValidation (this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblies(assemblies);

        return services;
    }
}