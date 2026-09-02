namespace ERP.Shared.Constants;

/// <summary>
/// Mensagens de erro padronizadas
/// </summary>
public static class ErrorMessages
{
    // Autenticação
    public const string InvalidCredentials = "Credenciais inválidas.";
    public const string UserNotFound = "Usuário não encontrado.";
    public const string InvalidPassword = "Senha incorreta.";
    public const string AccountLocked = "Conta bloqueada. Tente novamente mais tarde.";
    public const string AccountInactive = "Conta inativa.";
    public const string InvalidToken = "Token inválido.";
    public const string ExpiredToken = "Token expirado.";
    public const string RevokedToken = "Token revogado.";
    public const string InvalidRefreshToken = "Refresh token inválido.";
    public const string TwoFactorRequired = "Autenticação de dois fatores necessária.";
    public const string InvalidTwoFactorCode = "Código de dois fatores inválido.";
    public const string TwoFactorAlreadyEnabled = "Autenticação de dois fatores já está habilitada.";
    public const string TwoFactorNotEnabled = "Autenticação de dois fatores não está habilitada.";
    public const string InvalidRecoveryCode = "Código de recuperação inválido.";

    // Autorização
    public const string Unauthorized = "Não autorizado.";
    public const string Forbidden = "Acesso negado. Você não tem permissão para realizar esta ação.";
    public const string InsufficientPermissions = "Permissões insuficientes.";
    public const string RoleNotFound = "Perfil não encontrado.";
    public const string PermissionNotFound = "Permissão não encontrada.";

    // Tenant
    public const string TenantNotFound = "Empresa não encontrada.";
    public const string TenantInactive = "Empresa inativa.";
    public const string TenantSuspended = "Empresa suspensa.";
    public const string TenantDeleted = "Empresa excluída.";
    public const string InvalidTenantId = "ID da empresa inválido.";
    public const string TenantAlreadyExists = "Empresa já cadastrada.";
    public const string TenantLimitExceeded = "Limite de empresas excedido.";

    // Assinatura
    public const string SubscriptionNotFound = "Assinatura não encontrada.";
    public const string SubscriptionInactive = "Assinatura inativa.";
    public const string SubscriptionExpired = "Assinatura expirada.";
    public const string PlanNotFound = "Plano não encontrado.";
    public const string PlanInactive = "Plano inativo.";
    public const string ModuleNotAvailable = "Módulo não disponível no plano atual.";
    public const string StorageLimitExceeded = "Limite de armazenamento excedido.";
    public const string UserLimitExceeded = "Limite de usuários excedido.";

    // Usuário
    public const string EmailAlreadyExists = "E-mail já cadastrado.";
    public const string UsernameAlreadyExists = "Nome de usuário já cadastrado.";
    public const string EmailNotVerified = "E-mail não verificado.";
    public const string UserAlreadyExists = "Usuário já cadastrado.";
    public const string PasswordTooWeak = "Senha muito fraca. A senha deve conter pelo menos 8 caracteres, uma letra maiúscula, uma letra minúscula, um número e um caractere especial.";
    public const string PasswordsDoNotMatch = "As senhas não coincidem.";
    public const string CurrentPasswordIncorrect = "Senha atual incorreta.";

    // Validação
    public const string RequiredField = "O campo {0} é obrigatório.";
    public const string InvalidEmail = "E-mail inválido.";
    public const string InvalidPhone = "Telefone inválido.";
    public const string InvalidCnpj = "CNPJ inválido.";
    public const string InvalidCpf = "CPF inválido.";
    public const string InvalidDate = "Data inválida.";
    public const string FutureDate = "Data não pode ser no futuro.";
    public const string PastDate = "Data não pode ser no passado.";
    public const string InvalidRange = "Valor deve estar entre {0} e {1}.";
    public const string InvalidLength = "O campo {0} deve ter entre {1} e {2} caracteres.";
    public const string InvalidFormat = "Formato inválido para o campo {0}.";

    // Banco de dados
    public const string DatabaseError = "Erro ao acessar o banco de dados.";
    public const string DatabaseConnectionFailed = "Falha na conexão com o banco de dados.";
    public const string EntityNotFound = "Registro não encontrado.";
    public const string DuplicateEntry = "Registro já existe.";
    public const string ConstraintViolation = "Violação de restrição de integridade.";

    // Geral
    public const string InternalServerError = "Erro interno do servidor.";
    public const string BadRequest = "Requisição inválida.";
    public const string NotFound = "Recurso não encontrado.";
    public const string Conflict = "Conflito de dados.";
    public const string InvalidOperation = "Operação inválida.";
    public const string OperationCancelled = "Operação cancelada.";
    public const string Timeout = "Tempo limite excedido.";

    // Fiscal
    public const string InvalidCfop = "CFOP inválido.";
    public const string InvalidNcm = "NCM inválido.";
    public const string InvalidCst = "CST inválido.";
    public const string InvalidCsosn = "CSOSN inválido.";

    // Financeiro
    public const string InvalidBankAccount = "Conta bancária inválida.";
    public const string InsufficientBalance = "Saldo insuficiente.";
    public const string PaymentFailed = "Falha no pagamento.";
    public const string InvoiceNotFound = "Nota fiscal não encontrada.";

    // Estoque
    public const string InsufficientStock = "Estoque insuficiente.";
    public const string ProductNotFound = "Produto não encontrado.";
    public const string ProductInactive = "Produto inativo.";

    // Vendas
    public const string SaleNotFound = "Venda não encontrada.";
    public const string SaleAlreadyCancelled = "Venda já cancelada.";
    public const string SaleAlreadyConfirmed = "Venda já confirmada.";
    public const string InvalidSaleStatus = "Status da venda inválido.";

    // Compras
    public const string PurchaseNotFound = "Compra não encontrada.";
    public const string PurchaseAlreadyCancelled = "Compra já cancelada.";
    public const string PurchaseAlreadyReceived = "Compra já recebida.";
    public const string SupplierNotFound = "Fornecedor não encontrado.";
}
