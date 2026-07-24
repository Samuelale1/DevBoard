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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

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

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.MapGroup("/api/projects").MapProjects();
app.MapGroup("/api/issues").MapIssues();

app.Run();