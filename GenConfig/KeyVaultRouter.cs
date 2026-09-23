namespace GenConfig;

public class KeyVaultRouter
{
    private readonly Dictionary<string, KeyVaultClient> _vaults = new();

    public KeyVaultRouter(string[] vaultNames)
    {
        foreach (var v in vaultNames)
            _vaults[v] = new KeyVaultClient(v);
    }

    public string Resolve(string? vaultAndSecret)
    {
        if (vaultAndSecret == null)
            throw new ArgumentNullException(nameof(vaultAndSecret));

        var parts = vaultAndSecret.Split(':');
        var vault = parts[0];
        var secret = parts[1];

        return _vaults[vault].GetSecret(secret);
    }
}