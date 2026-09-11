using ERP.Master.Data;
using ERP.Master.Models;
using ERP.Shared.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ==================== CONFIGURAÇÕES BÁSICAS ====================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ==================== DATABASE (MASTER) ====================
builder.Services.AddDbContext<MasterDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MasterConnection"));
    options.UseSnakeCaseNamingConvention();
});

// ==================== IDENTITY ====================
builder.Services.AddIdentity<User, Role>(options =>
{
    // Senha forte
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Bloqueio de conta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Usuário
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<MasterDbContext>()
.AddDefaultTokenProviders();

// ==================== JWT ====================
builder.Services.AddJwtAuthentication(builder.Configuration);

// ==================== 2FA ====================
builder.Services.AddScoped<TwoFactorService>();

// ==================== SERVIÇOS ====================
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();

// ==================== CORS ====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// ==================== RATE LIMITING ====================
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ApiLimiter", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
    });
});

// ==================== SWAGGER ====================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ERP SaaS API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});

// ==================== BUILD ====================
var app = builder.Build();

// ==================== MIDDLEWARES ====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ERP SaaS API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// ==================== MIDDLEWARES CUSTOMIZADOS ====================
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseMiddleware<TenantValidationMiddleware>();
app.UseMiddleware<TenantDbContextMiddleware>();
// app.UseMiddleware<ExceptionMiddleware>(); // Já existe (se necessário criar)

app.MapControllers();

// ==================== SEED DATA (Criar admin padrão) ====================
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

    // Criar role Admin
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new Role 
        { 
            Name = "Admin", 
            NormalizedName = "ADMIN",
            Description = "Administrador do sistema" 
        });
    }

    // Criar usuário Admin
    var adminEmail = "admin@erp.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "ERP SaaS",
            IsActive = true
        };

        var result = await userManager.CreateAsync(admin, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
            Console.WriteLine("✅ Usuário admin criado: admin@erp.com / Admin@123");
        }
    }
}

app.Run();
