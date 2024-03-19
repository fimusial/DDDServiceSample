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

app.MapGet("/memo/{id}", async ([FromRoute] int id, IMediator mediator, CancellationToken cancellationToken) =>
{
    var memo = await mediator.Send(new GetMemoQuery { Id = id }, cancellationToken);
    return memo is not null ? Results.Ok(memo) : Results.NotFound();
});

app.MapGet("/memo/search", async ([FromQuery] string term, IMediator mediator, CancellationToken cancellationToken) =>
{
    var results = await mediator.Send(new SearchMemoContentQuery { Term = term }, cancellationToken);
    return Results.Ok(results);
});

app.Services.GetRequiredService<RabbitMQIntegrationEventConsumer>().Subscribe();

app.Run();