using System.Threading;
using Application;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure();

var app = builder.Build();

app.MapPost("/memo", async ([FromQuery] string content, IMediator mediator, CancellationToken cancellationToken) =>
{
    await mediator.Send(new AddMemoCommand { Content = content }, cancellationToken);
    return Results.Ok();
});

app.Run();

