using System.Diagnostics;
using aminsys_api.Models;
using aminsys_api.Services;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

Env.Load();
// Add services to the container.

// builder.Services.Configure<PeopleDatabaseSettings>(builder.Configuration.GetSection("PeopleDatabase")); // registered in the Dependency Injection (DI)
builder.Services.Configure<PeopleDatabaseSettings>(m => {
    m.ConnectionString = Environment.GetEnvironmentVariable("ConnectionString");
    m.DatabaseName = Environment.GetEnvironmentVariable("DatabaseName");
    m.CollectionName = Environment.GetEnvironmentVariable("CollectionName");
});
builder.Services.AddSingleton<PeopleService>(); // registered with DI to support constructor injection in consuming classes. The singleton service lifetime is most appropriate

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Expresence MongoDB API", Version = "1.0.0" });
    c.EnableAnnotations();
});

var app = builder.Build();

// Remember to remove this IF this app is ever used in production
app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();

app.MapControllers();

app.Run();
