using UserService.API.Middlewares;
using UserService.Application.Extensions;
using UserService.Application.Interfaces;
using UserService.Application.Models.Settings;
using UserService.Infrastructure.Extensions;
using UserService.Infrastructure.ProductClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ProductService:BaseUrl"]);
    client.DefaultRequestHeaders.Add("User-Agent", "UserService");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddApplicationServices();
builder.Services.AddValidation();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
