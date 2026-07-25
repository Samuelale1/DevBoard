using DevBoard.Api.Endpoints;
using DevBoard.Application.Services.Implementations;
using DevBoard.Application.Services.Interfaces;
using DevBoard.Domain.Interfaces;
using DevBoard.Infrastructure.Persistence;
using DevBoard.Infrastructure.Repositories;
using DevBoard.Application.Options;
using DevBoard.Api.Validators;
using FluentValidation;
using DevBoard.Api.Middleware;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DevBoard.Infrastructure.Hubs;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IIssueService, IssueService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateIssueRequestValidator>();


builder.Services.AddOpenApi();

builder.Services.AddOptions<SmtpOptions>()
    .BindConfiguration("Smtp")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<FeatureFlagOptions>()
    .BindConfiguration("FeatureFlags");

builder.Services.AddOptions<JwtOptions>().BindConfiguration("Jwt").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSignalR();
builder.Services.AddAuthorization();




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
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


/* app.UseExceptionHandler(builder =>
{
    builder.Run(async context =>
    {
        var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        var (statusCode, message) = ex switch
        {
            DevBoard.Domain.Exceptions.DevBoardException dbEx => (dbEx.StatusCode, dbEx.Message),
            _ => (500, "An unexpected error occurred.")
        };
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = message });
    });
}); */
app.MapHub<BoardHub>("/hubs/board");
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/api/auth").MapAuth();
app.MapGroup("/api/projects").RequireAuthorization().MapProjects();
app.MapGroup("/api/issues").RequireAuthorization().MapIssues();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.MapGroup("/api/projects").MapProjects();
app.MapGroup("/api/issues").MapIssues();

app.Run();