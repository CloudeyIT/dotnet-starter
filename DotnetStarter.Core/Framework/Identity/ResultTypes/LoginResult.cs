﻿namespace DotnetStarter.Core.Framework.Identity.ResultTypes;

public record LoginResult
{
	public string Token { get; init; } = null!;
}