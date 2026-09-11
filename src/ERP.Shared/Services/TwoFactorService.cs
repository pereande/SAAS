using OtpNet;

namespace ERP.Shared.Services;

public class TwoFactorService
{
    public string GenerateTotpSecret()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    public string[] GenerateRecoveryCodes(int count = 10)
    {
        var codes = new string[count];
        for (int i = 0; i < count; i++)
        {
            codes[i] = GenerateRecoveryCode();
        }
        return codes;
    }

    private string GenerateRecoveryCode()
    {
        var randomNumber = new byte[4];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return BitConverter.ToUInt32(randomNumber, 0).ToString("D8");
    }

    public bool VerifyTotp(string secret, string code)
    {
        try
        {
            var key = Base32Encoding.ToBytes(secret);
            var totp = new Totp(key);
            
            // Verificar código com janela de 1 período para frente e para trás (tolerância de tempo)
            var verified = totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));
            return verified;
        }
        catch
        {
            return false;
        }
    }

    public string GetTotpProvisioningUri(string secret, string email, string issuer)
    {
        var key = Base32Encoding.ToBytes(secret);
        var totp = new Totp(key);
        return totp.GetKeyEncodedUri(email, issuer);
    }
}
