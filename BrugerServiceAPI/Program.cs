using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;
using BrugerServiceAPI.Models;
using BrugerServiceAPI.Service;
using System;
using NLog;
using NLog.Web;
using BrugerServiceAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;



var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    var vaultService = new VaultService(configuration);

    string mySecret = await vaultService.GetSecretAsync("secrets", "SecretKey") ?? "5Jw9yT4fb9T5XrwKUz23QzA5D9BuY3p6";
    string myIssuer = await vaultService.GetSecretAsync("secrets", "IssuerKey") ?? "gAdDxQDQq7UYNxF3F8pLjVmGuU5u8g3y";
    string myConnectionString = await vaultService.GetSecretAsync("secrets", "MongoConnectionString") ?? "mongodb+srv://admin:1234@4semproj.npem60f.mongodb.net/";

    configuration["MongoConnectionString"] = myConnectionString;

     builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = myIssuer,
            ValidAudience = "http://localhost",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mySecret)),
            ClockSkew = TimeSpan.Zero 
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                    logger.Error("Token expired: {0}", context.Exception.Message);
                }
                return Task.CompletedTask;
            }
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("UserRolePolicy", policy => policy.RequireRole("1"));
        options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("2"));
        options.AddPolicy("LegalRolePolicy", policy => policy.RequireRole("3"));

    });

    builder.Services.AddCors(options => 
    {
        options.AddPolicy("AllowOrigin", builder =>
        {
            builder.AllowAnyHeader()
                .AllowAnyMethod();
        });
            
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();
    builder.Services.AddTransient<VaultService>();
    builder.Services.AddSingleton<MongoDBContext>();
    builder.Services.AddSingleton<IUserInterface, UserMongoDBService>();

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
