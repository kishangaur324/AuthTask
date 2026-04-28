using System.Threading.RateLimiting;
using AuthTask.Application.DTOs;
using AuthTask.Application.Validators;
using AuthTask.Domain.Entities;
using AuthTask.Extensions;
using AuthTask.Infrastructure.Data;
using AuthTask.MapperProfiles;
using AuthTask.Middlewares;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add health check to verify if app is running
builder.Services.AddHealthChecks();

// Add services to the container
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Registering AuthDbContext for dependency injection
builder.Services.AddDbContext<AuthDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("Default"))
        .EnableThreadSafetyChecks()
        .EnableDetailedErrors()
        .EnableServiceProviderCaching()
        .EnableSensitiveDataLogging()
);

// Configuring .NetCore Identity to use the AuthDbContext for managing the Users, Roles etc.
builder
    .Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// Registering automapper
builder.Services.AddAutoMapper(x => x.AddProfile<MapperProfile>());

builder.Services.RegisterAuth(builder.Configuration);

// Regitering fluent validators
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

builder.Services.RegisterDependencies();

builder.Services.RegisterHttpClient();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var allowedCorsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "OpenApiUi",
        policy =>
        {
            // Only allow explicitly configured origins. If none are configured,
            // keep a safe local-development fallback.
            if (allowedCorsOrigins is { Length: > 0 })
            {
                policy.WithOrigins(allowedCorsOrigins);
            }
            else if (builder.Environment.IsDevelopment())
            {
                policy.WithOrigins(
                    "https://localhost:7111",
                    "http://localhost:5043",
                    "http://localhost:3000"
                );
            }

            policy
                .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                .WithHeaders("Authorization", "Content-Type", "X-API-KEY", "Accept")
                .WithExposedHeaders("X-Tracking-Id");
        }
    );
});

builder.Services.AddRateLimiter(options =>
{
    // Protects the full API surface from aggressive clients by IP.
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress!.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 500,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
            }
        )
    );

    // Additional tighter policy used by login/register endpoints.
    options.AddFixedWindowLimiter(
        "auth",
        opt =>
        {
            opt.PermitLimit = 50;
            opt.Window = TimeSpan.FromMinutes(1);
            opt.QueueLimit = 0;
        }
    );

    // Adding status and response when the request get rejected due to quota
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        await context.HttpContext.Response.WriteAsync(
            "Too many requests! Calm down, dude. Are you trying to break the server?",
            token
        );
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// A global exception handler
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;

        logger.LogError(exception, "Unhandled exception occurred");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new ApiResponse<string> { Error = "Something went wrong please try aagin." };

        await context.Response.WriteAsJsonAsync(response);
    });
});

// Attach tracking header early so every response includes a request id.
app.UseMiddleware<RequestTrackingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("OpenApiUi");

app.UseRateLimiter();

// Authentication must execute before authorization checks.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initializing the database for the any new migrations and seeding the default roles data.
await app.InitializeDatabaseAsync();

app.MapHealthChecks("/health");

app.Run();
