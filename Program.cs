using CapWebAPI.Data;
using Microsoft.EntityFrameworkCore;
using CapWebAPI.Services;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddControllers();

services.AddKeycloakWebApiAuthentication(builder.Configuration, options =>
{
    options.RequireHttpsMetadata = false;
});

services.AddAuthorization();
services?.AddAuthorization(options =>
{
     options.AddPolicy("isAdmin", policy =>
                    policy.RequireClaim(ClaimTypes.Role, "realm-admin")
                    .RequireClaim(ClaimTypes.Email,"deneme2@deneme.com")
                    );    

     options.AddPolicy("isEmployee", policy =>
                    policy.RequireClaim(ClaimTypes.Role, "employee"));    
});


services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{builder.Configuration["Keycloak:Authority"]}/protocol/openid-connect/auth"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "Access your basic profile" },
                    {"profile" , "profile"}
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "OAuth2",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new string[] {}
        }
    });
});

// MariaDB bağlantıları
var MariaDbConnection = builder.Configuration.GetConnectionString("MariaDbConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(MariaDbConnection, ServerVersion.AutoDetect(MariaDbConnection)));

// CAP ayarları
builder.Services.AddCap((capOptions) =>
{
    capOptions.UseDashboard();
    capOptions.UseEntityFramework<ApplicationDbContext>();
    capOptions.UseRabbitMQ(rabbitMQOptions =>
    {
        rabbitMQOptions.HostName = builder.Configuration.GetSection("RabbitMq:HostName").Value ?? "";
        int rabbitPort;
        Int32.TryParse(builder.Configuration.GetSection("RabbitMq:Port").Value, out rabbitPort);
        rabbitMQOptions.Port = rabbitPort;
    });
});

builder.Services.AddScoped<CustomerProducer>();
builder.Services.AddScoped<CustomerConsumer>();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();


//app.MapCustomerEndpoints();
app.MapControllers();
app.Run();