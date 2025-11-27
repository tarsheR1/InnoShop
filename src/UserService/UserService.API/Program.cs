using UserService.Application.Extensions;
using UserService.Application.Models.Settings;
using UserService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
