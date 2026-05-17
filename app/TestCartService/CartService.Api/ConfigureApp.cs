using CartService.Api.Endpoints;
using CartService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace CartService.Api;

public static class ConfigureApp
{
    public static void StartWebApi(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            scope.ServiceProvider
                .GetRequiredService<CartDbContext>()
                .Database.Migrate();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.RegisterCartEndpoints();

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        app.Run();
    }
}