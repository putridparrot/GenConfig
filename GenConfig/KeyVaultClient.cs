using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace GenConfig;

public class KeyVaultClient
{
    private readonly SecretClient _client;

    public KeyVaultClient(string vaultName)
    {
        var uri = new Uri($"https://{vaultName}.vault.azure.net/");
        _client = new SecretClient(uri, new DefaultAzureCredential());
    }

    public string GetSecret(string name)
    {
        return _client.GetSecret(name).Value.Value;
    }
}