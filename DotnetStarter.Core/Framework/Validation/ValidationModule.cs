﻿using Autofac;
using Cloudey.Reflex.Core.Setup;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace DotnetStarter.Core.Framework.Validation;

public class ValidationModule : Module
{
	protected override void Load (ContainerBuilder builder)
	{
		AppDomain.CurrentDomain.GetIncludedAssemblies()
			.ForEach(
				assembly =>
				{
					builder.RegisterAssemblyTypes(assembly)
						.AsSelf()
						.AsClosedTypesOf(typeof(IValidator<>));

					builder.RegisterAssemblyOpenGenericTypes(assembly)
						.AssignableTo(typeof(IValidator<>))
						.AsSelf()
						.AsImplementedInterfaces();

					builder.RegisterAssemblyTypes(assembly)
						.AssignableTo(typeof(IValidatorInterceptor))
						.As(typeof(IValidatorInterceptor));
				}
			);
	}
}