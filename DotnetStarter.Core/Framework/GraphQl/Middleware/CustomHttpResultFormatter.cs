using System.Net;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;

namespace DotnetStarter.Core.Framework.GraphQl.Middleware;

public class CustomHttpResultFormatter : DefaultHttpResponseFormatter
{
    protected override HttpStatusCode GetStatusCode (IQueryResult result, FormatInfo format, HttpStatusCode? proposedStatusCode)
    {
        if (result.Errors?.Any(e => e.Extensions?.TryGetValue("code", out var code) is true && code is "FairyBread_ValidationError") ?? false)
        {
            return HttpStatusCode.OK;
        }

        return proposedStatusCode ?? HttpStatusCode.OK;
    }
}


