using DevBoard.Api.Endpoints;
using DevBoard.Application.Services.Implementations;
using DevBoard.Application.Services.Interfaces;
using DevBoard.Domain.Interfaces;
using DevBoard.Infrastructure.Persistence;
using DevBoard.Infrastructure.Repositories;
using DevBoard.Application.Options;
using DevBoard.Api.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DevBoard.Infrastructure.Hubs;
using DevBoard.Infrastructure.BackgroundServices;
using DevBoard.Domain.Exceptions;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IIssueService, IssueService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDailyDigestService, DailyDigestService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<WebhookChannel>();
builder.Services.AddHostedService<WebhookDeliveryWorker>();
builder.Services.AddHostedService<StaleIssueCloserWorker>();
builder.Services.AddHostedService<DailyDigestWorker>();
builder.Services.AddOptions<SmtpOptions>().BindConfiguration("Smtp").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<FeatureFlagOptions>().BindConfiguration("FeatureFlags");
builder.Services.AddOptions<JwtOptions>().BindConfiguration("Jwt").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddValidatorsFromAssemblyContaining<CreateIssueRequestValidator>();
builder.Services.AddHttpClient("webhook", c => c.Timeout = TimeSpan.FromSeconds(10));

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Secret"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database");

builder.Services.AddOpenApi();




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}



 app.UseExceptionHandler(errbuilder =>
{
    errbuilder.Run(async context =>
    {
        var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

        Console.WriteLine($"[EXCEPTION HANDLER] Type: {ex?.GetType().FullName} | Message: {ex?.Message}");

        var (statusCode, message) = ex switch
        {
            DevBoardException dbEx => (dbEx.StatusCode, dbEx.Message),
            System.Text.Json.JsonException => (400, "The request body is malformed or contains invalid values."),
            BadHttpRequestException => (400, "The request body is malformed or contains invalid values."),
            _ => (500, $"DEBUG - Unmatched exception type: {ex?.GetType().FullName} | Message: {ex?.Message}")
        };
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = message });
    });
}); 
// Auto Migrate to Db
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
if (db.Database.IsRelational())
{
    await db.Database.MigrateAsync();   
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/api/auth").MapAuth();
app.MapGroup("/api/projects").RequireAuthorization().MapProjects();
app.MapGroup("/api/issues").RequireAuthorization().MapIssues();

app.MapHub<BoardHub>("/hubs/board");


app.MapHealthChecks("/health");




app.Run();
public partial class Program { }