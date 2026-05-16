using CartService.Api;
using CartService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterAppConfig();

builder.Services.AddInfrastructure()
    .AddWebServices();

var app = builder.Build();

app.StartWebApi();