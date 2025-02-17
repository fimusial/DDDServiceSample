using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebAPI;

public class CommonHttpResponseHeadersMiddleware
{
    private readonly RequestDelegate next;

    public CommonHttpResponseHeadersMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers.Append("Server-HostName", Dns.GetHostName());
        await next(context);
    }
}