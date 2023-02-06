﻿namespace DotnetStarter.Core.Framework.Setup;

/// <summary>
///     Include this assembly when scanning for services
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class IncludeAssemblyAttribute : Attribute { }