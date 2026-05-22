using System.Reflection;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProjectTaskManagement.API.Extensions;
using ProjectTaskManagement.API.Middleware;
using ProjectTaskManagement.Application;
using ProjectTaskManagement.Infrastructure;
using ProjectTaskManagement.Infrastructure.Persistence;
using ProjectTaskManagement.Infrastructure.Persistence.Seed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Project & Task Management API",
        Version = "v1",
        Description = "Enterprise-grade Clean Architecture API for project and task management.",
        Contact = new OpenApiContact
        {
            Name = "Project Task Management",
            Email = "support@projecttask.com"
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("sqlserver", tags: ["ready"]);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    options.AddPolicy("Production", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Task Management API v1");
        options.RoutePrefix = "swagger";
    });
}

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseCors(app.Environment.IsProduction() ? "Production" : "Development");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

await DataSeeder.SeedAsync(app.Services);

try
{
    Log.Information("Starting Project Task Management API ({Environment})", app.Environment.EnvironmentName);
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
