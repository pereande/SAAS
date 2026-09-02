namespace ERP.Shared.Settings;

/// <summary>
/// Configurações gerais da aplicação
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Ambiente da aplicação
    /// </summary>
    public string Environment { get; set; } = "Development";

    /// <summary>
    /// Nome da aplicação
    /// </summary>
    public string AppName { get; set; } = "ERP SaaS";

    /// <summary>
    /// Versão da aplicação
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Descrição da aplicação
    /// </summary>
    public string Description { get; set; } = "Sistema ERP SaaS Multiempresa";

    /// <summary>
    /// URL da aplicação
    /// </summary>
    public string AppUrl { get; set; } = "https://localhost:5000";

    /// <summary>
    /// URL da API
    /// </summary>
    public string ApiUrl { get; set; } = "https://localhost:5000/api";

    /// <summary>
    /// URL do frontend
    /// </summary>
    public string FrontendUrl { get; set; } = "https://localhost:3000";

    /// <summary>
    /// Habilitar Swagger
    /// </summary>
    public bool EnableSwagger { get; set; } = true;

    /// <summary>
    /// Habilitar documentação do Swagger
    /// </summary>
    public bool EnableSwaggerUi { get; set; } = true;

    /// <summary>
    /// Caminho do Swagger
    /// </summary>
    public string SwaggerPath { get; set; } = "/swagger";

    /// <summary>
    /// Título do Swagger
    /// </summary>
    public string SwaggerTitle { get; set; } = "ERP SaaS API";

    /// <summary>
    /// Versão do Swagger
    /// </summary>
    public string SwaggerVersion { get; set; } = "v1";

    /// <summary>
    /// Descrição do Swagger
    /// </summary>
    public string SwaggerDescription { get; set; } = "API do Sistema ERP SaaS Multiempresa";

    /// <summary>
    /// Contato do Swagger
    /// </summary>
    public SwaggerContact SwaggerContact { get; set; } = new SwaggerContact();

    /// <summary>
    /// Licença do Swagger
    /// </summary>
    public SwaggerLicense SwaggerLicense { get; set; } = new SwaggerLicense();

    /// <summary>
    /// Configurações de JWT
    /// </summary>
    public JwtSettings Jwt { get; set; } = new JwtSettings();

    /// <summary>
    /// Configurações de 2FA
    /// </summary>
    public TwoFactorSettings TwoFactor { get; set; } = new TwoFactorSettings();

    /// <summary>
    /// Configurações de segurança
    /// </summary>
    public SecuritySettings Security { get; set; } = new SecuritySettings();

    /// <summary>
    /// Configurações de sessão
    /// </summary>
    public SessionSettings Session { get; set; } = new SessionSettings();

    /// <summary>
    /// Configurações de rate limiting
    /// </summary>
    public RateLimitSettings RateLimiting { get; set; } = new RateLimitSettings();

    /// <summary>
    /// Configurações de CORS
    /// </summary>
    public CorsSettings Cors { get; set; } = new CorsSettings();

    /// <summary>
    /// Configurações do banco de dados
    /// </summary>
    public DatabaseSettings Database { get; set; } = new DatabaseSettings();

    /// <summary>
    /// Configurações de logging
    /// </summary>
    public LoggingSettings Logging { get; set; } = new LoggingSettings();

    /// <summary>
    /// Configurações de cache
    /// </summary>
    public CacheSettings Cache { get; set; } = new CacheSettings();

    /// <summary>
    /// Configurações de multi-tenancy
    /// </summary>
    public MultiTenancySettings MultiTenancy { get; set; } = new MultiTenancySettings();
}

/// <summary>
/// Contato do Swagger
/// </summary>
public class SwaggerContact
{
    /// <summary>
    /// Nome
    /// </summary>
    public string Name { get; set; } = "Suporte ERP SaaS";

    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; } = "suporte@erpsaas.com.br";

    /// <summary>
    /// URL
    /// </summary>
    public string Url { get; set; } = "https://erpsaas.com.br";
}

/// <summary>
/// Licença do Swagger
/// </summary>
public class SwaggerLicense
{
    /// <summary>
    /// Nome
    /// </summary>
    public string Name { get; set; } = "MIT License";

    /// <summary>
    /// URL
    /// </summary>
    public string Url { get; set; } = "https://opensource.org/licenses/MIT";
}

/// <summary>
/// Configurações do banco de dados
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// Provider do banco de dados
    /// </summary>
    public string Provider { get; set; } = "PostgreSQL";

    /// <summary>
    /// Connection string do banco master
    /// </summary>
    public string MasterConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Prefixo do nome do banco de dados dos tenants
    /// </summary>
    public string TenantDatabasePrefix { get; set; } = "erp_tenant_";

    /// <summary>
    /// Tempo de timeout (em segundos)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Tamanho máximo do pool de conexões
    /// </summary>
    public int MaxPoolSize { get; set; } = 100;

    /// <summary>
    /// Tamanho mínimo do pool de conexões
    /// </summary>
    public int MinPoolSize { get; set; } = 0;

    /// <summary>
    /// Habilitar logging de SQL
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Habilitar logging de SQL (apenas para desenvolvimento)
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Habilitar migrations automáticas
    /// </summary>
    public bool EnableAutomaticMigrations { get; set; } = false;
}

/// <summary>
/// Configurações de logging
/// </summary>
public class LoggingSettings
{
    /// <summary>
    /// Nível mínimo de logging
    /// </summary>
    public string MinimumLogLevel { get; set; } = "Information";

    /// <summary>
    /// Nível mínimo de logging para Microsoft
    /// </summary>
    public string MicrosoftLogLevel { get; set; } = "Warning";

    /// <summary>
    /// Habilitar logging em console
    /// </summary>
    public bool EnableConsoleLogging { get; set; } = true;

    /// <summary>
    /// Habilitar logging em arquivo
    /// </summary>
    public bool EnableFileLogging { get; set; } = true;

    /// <summary>
    /// Caminho do arquivo de log
    /// </summary>
    public string FileLogPath { get; set; } = "Logs";

    /// <summary>
    /// Nome do arquivo de log
    /// </summary>
    public string FileLogName { get; set; } = "erpsaas-";

    /// <summary>
    /// Tamanho máximo do arquivo de log (em MB)
    /// </summary>
    public long FileLogMaxSizeMb { get; set; } = 100;

    /// <summary>
    /// Número máximo de arquivos de log
    /// </summary>
    public int FileLogMaxFiles { get; set; } = 30;

    /// <summary>
    /// Habilitar logging no Seq
    /// </summary>
    public bool EnableSeqLogging { get; set; } = false;

    /// <summary>
    /// URL do Seq
    /// </summary>
    public string SeqUrl { get; set; } = "http://localhost:5341";

    /// <summary>
    /// API Key do Seq
    /// </summary>
    public string SeqApiKey { get; set; } = string.Empty;
}

/// <summary>
/// Configurações de cache
/// </summary>
public class CacheSettings
{
    /// <summary>
    /// Provider de cache
    /// </summary>
    public string Provider { get; set; } = "Memory";

    /// <summary>
    /// Connection string do Redis
    /// </summary>
    public string RedisConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Tempo de expiração padrão (em minutos)
    /// </summary>
    public int DefaultExpiryMinutes { get; set; } = 30;

    /// <summary>
    /// Tamanho máximo do cache (em MB)
    /// </summary>
    public long MaxCacheSizeMb { get; set; } = 100;

    /// <summary>
    /// Habilitar cache distribuído
    /// </summary>
    public bool EnableDistributedCache { get; set; } = false;
}

/// <summary>
/// Configurações de multi-tenancy
/// </summary>
public class MultiTenancySettings
{
    /// <summary>
    /// Estratégia de multi-tenancy
    /// </summary>
    public string Strategy { get; set; } = "DatabasePerTenant";

    /// <summary>
    /// Header para identificar o tenant
    /// </summary>
    public string TenantHeader { get; set; } = "X-Tenant-Id";

    /// <summary>
    /// Header para identificar o subdomínio do tenant
    /// </summary>
    public string TenantSubdomainHeader { get; set; } = "X-Tenant-Subdomain";

    /// <summary>
    /// Usar subdomínio para identificar tenant
    /// </summary>
    public bool UseSubdomain { get; set; } = false;

    /// <summary>
    /// Domínio base
    /// </summary>
    public string BaseDomain { get; set; } = "erpsaas.com.br";

    /// <summary>
    /// Permitir tenant padrão
    /// </summary>
    public bool AllowDefaultTenant { get; set; } = true;

    /// <summary>
    /// ID do tenant padrão
    /// </summary>
    public Guid? DefaultTenantId { get; set; }

    /// <summary>
    /// Validar isolamento de tenant
    /// </summary>
    public bool ValidateTenantIsolation { get; set; } = true;
}
