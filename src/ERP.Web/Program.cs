using System.Text;
using ERP.Master.Infrastructure.Data;
using ERP.Master.Models;
using ERP.Shared.Interfaces;
using ERP.Master.Services;
using ERP.Shared.Settings;
using ERP.Tenant.Infrastructure.Data;
using ERP.Web.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("Starting ERP SaaS Web API...");
    builder.Host.UseSerilog();

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

    var dbSettings = builder.Configuration.GetSection("AppSettings:Database").Get<DatabaseSettings>()
        ?? throw new InvalidOperationException("AppSettings:Database configuration is required.");
    var jwtSettings = builder.Configuration.GetSection("AppSettings:Jwt").Get<JwtSettings>()
        ?? throw new InvalidOperationException("AppSettings:Jwt configuration is required.");

    if (string.IsNullOrWhiteSpace(dbSettings.MasterConnectionString))
        throw new InvalidOperationException("Master database connection string is required.");
    if (string.IsNullOrWhiteSpace(jwtSettings.Secret) || jwtSettings.Secret.Length < 32)
        throw new InvalidOperationException("JWT secret must be provided and contain at least 32 characters.");

    builder.Services.AddDbContext<MasterDbContext>(options =>
    {
        options.UseNpgsql(dbSettings.MasterConnectionString);
        options.EnableDetailedErrors(dbSettings.EnableDetailedErrors);
        options.EnableSensitiveDataLogging(dbSettings.EnableSensitiveDataLogging);
    });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<TenantDbContext>(serviceProvider =>
    {
        var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext
            ?? throw new InvalidOperationException("HTTP context is required for tenant-scoped data access.");
        var tenantContext = httpContext.GetTenantContext();
        if (tenantContext?.TenantId is not Guid tenantId || tenantId == Guid.Empty)
            throw new InvalidOperationException("A valid tenant is required for tenant-scoped data access.");
        var tenantConnectionString = tenantContext.ConnectionString;
        if (string.IsNullOrWhiteSpace(tenantConnectionString))
            throw new InvalidOperationException("The resolved tenant does not have a database connection configured.");

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(tenantConnectionString)
            .EnableDetailedErrors(dbSettings.EnableDetailedErrors)
            .EnableSensitiveDataLogging(dbSettings.EnableSensitiveDataLogging)
            .Options;
        return new TenantDbContext(options, tenantId, tenantContext.TenantName ?? tenantId.ToString());
    });

    builder.Services.AddIdentity<User, Role>(options =>
    {
        var securitySettings = builder.Configuration.GetSection("AppSettings:Security").Get<SecuritySettings>()
            ?? new SecuritySettings();
        options.Password.RequireDigit = securitySettings.RequireDigit;
        options.Password.RequireLowercase = securitySettings.RequireLowercase;
        options.Password.RequireUppercase = securitySettings.RequireUppercase;
        options.Password.RequireNonAlphanumeric = securitySettings.RequireNonAlphanumeric;
        options.Password.RequiredLength = securitySettings.MinimumPasswordLength;
        options.Password.RequiredUniqueChars = securitySettings.RequiredUniqueChars;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(securitySettings.LockoutMinutes);
        options.Lockout.MaxFailedAccessAttempts = securitySettings.MaxFailedAccessAttempts;
        options.Lockout.AllowedForNewUsers = securitySettings.AllowAccountLockout;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddEntityFrameworkStores<MasterDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
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

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
        options.AddPolicy("RequireTenantAdmin", policy => policy.RequireRole("TenantAdmin"));
        options.AddPolicy("RequireUser", policy => policy.RequireRole("User"));
        options.AddPolicy("Require2FA", policy => policy.RequireClaim("two_factor_enabled", "true"));
    });

    var rateLimitSettings = builder.Configuration.GetSection("AppSettings:RateLimiting").Get<RateLimitSettings>()
        ?? new RateLimitSettings();
    if (rateLimitSettings.EnableRateLimiting)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitSettings.RequestsPerMinute,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
        });
    }

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddCors(options =>
    {
        var corsSettings = builder.Configuration.GetSection("AppSettings:Cors").Get<CorsSettings>()
            ?? new CorsSettings();
        options.AddPolicy("Default", policy =>
        {
            policy.WithOrigins(corsSettings.AllowedOrigins.ToArray())
                .WithMethods(corsSettings.AllowedMethods.ToArray())
                .WithHeaders(corsSettings.AllowedHeaders.ToArray())
                .AllowCredentials()
                .SetPreflightMaxAge(TimeSpan.FromSeconds(corsSettings.PreflightCacheDurationSeconds));
        });
    });

    builder.Services.AddHttpClient();
    builder.Services.AddScoped<IRepository<UserToken, Guid>, EfRepository<UserToken, Guid>>();
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<TwoFactorService>();

    var app = builder.Build();

    app.UseGlobalExceptionHandling();
    app.UseForwardedHeaders();
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors("Default");
    if (rateLimitSettings.EnableRateLimiting)
        app.UseRateLimiter();
    app.UseAuthentication();
    app.UseTenantResolution();
    app.UseTenantValidation();
    app.UseAuthorization();
    app.UseAuthorizationMiddleware();
    app.UseAuditLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    if (app.Environment.IsDevelopment() && dbSettings.EnableAutomaticMigrations)
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

public partial class Program { }
