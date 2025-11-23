using Hellang.Middleware.ProblemDetails;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.Extensions;
using ProductService.Application.Extensions;
using ProductService.Infrastructure.DataAccess;
using ProductService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddValidators();
builder.Services.ConfigureProblemDetails();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ProductServiceDbContext>();

        Console.WriteLine("Waiting for database...");
        var canConnect = false;
        for (var i = 0; i < 10; i++)
        {
            try
            {
                canConnect = context.Database.CanConnect();
                if (canConnect) break;
            }
            catch
            {
                Console.WriteLine($"Database not ready yet... attempt {i + 1}/10");
                Thread.Sleep(5000);
            }
        }

        if (canConnect)
        {
            Console.WriteLine("Database connected successfully!");

            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"Applying {pendingMigrations.Count} pending migrations...");
                context.Database.Migrate();
                Console.WriteLine("Migrations applied successfully!");
            }
            else
            {
                Console.WriteLine("No pending migrations.");
            }
        }
        else
        {
            Console.WriteLine("WARNING: Could not connect to database after 10 attempts.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR during database initialization: {ex.Message}");
    }
}

app.UseProblemDetails();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("ProductService API started successfully!");
app.Run();