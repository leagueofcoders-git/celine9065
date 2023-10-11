using System;
using Microsoft.EntityFrameworkCore;
using urlShortener.DBContext;
using urlShortener.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "URL Shortener",
        Version = "v1",
        Description = "ASP.NET Core 6 Web API Course Project",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Celine Tan",
            Email = "celinetan2009@hotmail.com"
        }
    });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Add APIKey
builder.Services.AddScoped<ApiKeyHeaderAttribute>();

var dbHost = "127.0.0.1//sqlexpress";
var dbName = "urldb";
var UserID = "sa";
var dbPassword = "P@ssw0rd";
var dbConnectionString = $"Data Source={dbHost}; Initial Catalog={dbName}; User Id={UserID}; Password={dbPassword}";


builder.Services.AddDbContext<shortnerDBContext>(options => options.UseSqlServer(dbConnectionString));

// Dependency injection 
//builder.Services.AddDbContext<shortnerDBContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("urlShortenerDB"));
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Celine v1");
        c.InjectStylesheet("swagger-ui.css");
        c.InjectJavascript("swagger-ui-bundle.js");
        c.InjectJavascript("swagger-ui-standalone-preset.js");
        c.RoutePrefix = string.Empty;
    });

}


//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiDemo v1");
//    c.RoutePrefix = string.Empty;
//});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
