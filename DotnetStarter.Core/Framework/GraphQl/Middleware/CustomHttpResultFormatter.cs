using System.Net;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;

namespace DotnetStarter.Core.Framework.GraphQl.Middleware;

public class CustomHttpResultFormatter : DefaultHttpResponseFormatter
{
	protected override HttpStatusCode OnDetermineStatusCode (
		IQueryResult result,
		FormatInfo format,
		HttpStatusCode? proposedStatusCode
	)
	{
		if (result.Errors?.Any(
			    e => e.Extensions?.TryGetValue("code", out var code) is true && code is "FairyBread_ValidationError"
		    ) ??
		    false)
		{
			return HttpStatusCode.OK;
		}

		if (result.Errors?.Any(
			    e => e.Extensions?.TryGetValue("code", out var code) is true && code is "AUTH_NOT_AUTHORIZED"
		    ) ??
		    false)
		{
			return HttpStatusCode.Unauthorized;
		}

		return base.OnDetermineStatusCode(result, format, proposedStatusCode);
	}
}