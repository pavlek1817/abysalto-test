using CartService.Api;
using CartService.Application;
using CartService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterAppConfig();

builder.Services.AddInfrastructure()
    .AddApplication()
    .AddWebServices();

var app = builder.Build();

app.StartWebApi();