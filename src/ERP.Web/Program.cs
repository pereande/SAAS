using System.Text;
using ERP.Master.Infrastructure.Data;
using ERP.Master.Models;
using ERP.Shared.Interfaces;
using ERP.Shared.Settings;
using ERP.Web.Middleware;
using ERP.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("Starting ERP SaaS Web API...");

    // Add services to the container.
    builder.Host.UseSerilog();

    // Configuration
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("AppSettings:Jwt"));
    builder.Services.Configure<TwoFactorSettings>(builder.Configuration.GetSection("AppSettings:TwoFactor"));
    builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection("AppSettings:Security"));
    builder.Services.Configure<SessionSettings>(builder.Configuration.GetSection("AppSettings:Session"));
    builder.Services.Configure<RateLimitSettings>(builder.Configuration.GetSection("AppSettings:RateLimiting"));
    builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection("AppSettings:Cors"));
    builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("AppSettings:Database"));
    builder.Services.Configure<LoggingSettings>(builder.Configuration.GetSection("AppSettings:Logging"));
    builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("AppSettings:Cache"));
    builder.Services.Configure<MultiTenancySettings>(builder.Configuration.GetSection("AppSettings:MultiTenancy"));

    // Database Contexts
    builder.Services.AddDbContext<MasterDbContext>(options =>
    {
        var dbSettings = builder.Configuration.GetSection("AppSettings:Database").Get<DatabaseSettings>();
        options.UseNpgsql(dbSettings.MasterConnectionString);
        options.UseSnakeCaseNamingConvention();
    });

    // Identity
    builder.Services.AddIdentity<User, Role>(options =>
    {
        var securitySettings = builder.Configuration.GetSection("AppSettings:Security").Get<SecuritySettings>();
        
        // Password settings
        options.Password.RequireDigit = securitySettings.RequireDigit;
        options.Password.RequireLowercase = securitySettings.RequireLowercase;
        options.Password.RequireUppercase = securitySettings.RequireUppercase;
        options.Password.RequireNonAlphanumeric = securitySettings.RequireNonAlphanumeric;
        options.Password.RequiredLength = securitySettings.MinimumPasswordLength;
        options.Password.RequiredUniqueChars = securitySettings.RequiredUniqueChars;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(securitySettings.LockoutMinutes);
        options.Lockout.MaxFailedAccessAttempts = securitySettings.MaxFailedAccessAttempts;
        options.Lockout.AllowedForNewUsers = securitySettings.AllowAccountLockout;

        // User settings
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;

        // Sign in settings
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddEntityFrameworkStores<MasterDbContext>()
    .AddDefaultTokenProviders();

    // Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("AppSettings:Jwt").Get<JwtSettings>();
        
        options.SaveToken = true;
        // Safe for local development (no HTTPS), strict in production
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = jwtSettings.ValidateIssuer,
            ValidateAudience = jwtSettings.ValidateAudience,
            ValidateLifetime = jwtSettings.ValidateLifetime,
            ValidateIssuerSigningKey = jwtSettings.ValidateSignature,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    });

    // Authorization
    builder.Services.AddAuthorization(options =>
    {
        // Políticas de autorização
        options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
        options.AddPolicy("RequireTenantAdmin", policy => policy.RequireRole("TenantAdmin"));
        options.AddPolicy("RequireUser", policy => policy.RequireRole("User"));
        options.AddPolicy("Require2FA", policy => policy.RequireClaim("two_factor_enabled", "true"));
    });

    // Controllers and Swagger
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // CORS
    builder.Services.AddCors(options =>
    {
        var corsSettings = builder.Configuration.GetSection("AppSettings:Cors").Get<CorsSettings>();
        
        options.AddPolicy("Default", policy =>
        {
            policy.WithOrigins(corsSettings.AllowedOrigins.ToArray())
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    // AutoMapper
    builder.Services.AddAutoMapper(typeof(Program));

    // HttpClient
    builder.Services.AddHttpClient();

    // Repositories
    builder.Services.AddScoped(typeof(IRepository<,>), typeof(ERP.Web.Repositories.Repository<,>));

    // Services
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<TwoFactorService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Middleware pipeline (order matters):
    // 1. Exception handling (outermost — catches all unhandled errors)
    // 2. HTTPS redirection
    // 3. Routing
    // 4. CORS
    // 5. Authentication
    // 6. Tenant resolution
    // 7. Tenant validation (DB/status check)
    // 8. Custom authorization (RBAC)
    // 9. Built-in authorization
    // 10. Audit logging
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors("Default");
    app.UseAuthentication();
    app.UseTenantResolution();
    app.UseTenantValidation();
    app.UseAuthorizationMiddleware();
    app.UseAuthorization();
    app.UseAuditLogging();
    app.MapControllers();

    // Aplicar migrations no startup (apenas em desenvolvimento)
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
        dbContext.Database.Migrate();
    }

    Log.Information("ERP SaaS Web API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ERP SaaS Web API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
