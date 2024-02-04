using System.Threading;
using Application;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddWebAPI();

var app = builder.Build();

app.MapPost("/memo", async ([FromQuery] string content, IMediator mediator, CancellationToken cancellationToken) =>
{
    await mediator.Send(new AddMemoCommand { Content = content }, cancellationToken);
    return Results.Ok();
});

app.Services.GetRequiredService<RabbitMQIntegrationEventConsumer>().Subscribe();

app.Run();