namespace GenConfig;

public static class ConfigCommand
{
    public static void Run(string env, string templateFile, string outputFile)
    {
        Console.WriteLine($"🔧 genconfig: generating configuration for environment '{env}'");

        var envFile = $".env.{env}";

        // -----------------------------
        // 1. Validate required files
        // -----------------------------
        if (!File.Exists(envFile))
        {
            Console.WriteLine($"❌ Missing environment file: {envFile}");
            return;
        }

        if (!File.Exists(templateFile))
        {
            Console.WriteLine($"❌ Missing template file: {templateFile}");
            return;
        }

        // -----------------------------
        // 2. Load environment variables
        // -----------------------------
        Console.WriteLine($"✔ Loading environment variables from {envFile}");
        var envVars = EnvLoader.Load(envFile);

        // -----------------------------
        // 3. Load template
        // -----------------------------
        Console.WriteLine($"✔ Loading template: {templateFile}");
        var template = File.ReadAllText(templateFile);

        // -----------------------------
        // 4. Load Azure DevOps variable group (optional)
        // -----------------------------
        if (envVars.ContainsKey("AZDO_GROUP"))
        {
            Console.WriteLine($"✔ Loading Azure DevOps variable group: {envVars["AZDO_GROUP"]}");

            var client = new AzureDevOpsClient(
                envVars["AZDO_ORG"],
                envVars["AZDO_PROJECT"],
                envVars["AZDO_PAT"]
            );

            try
            {
                var devopsVars = client.GetVariables(envVars["AZDO_GROUP"]);

                foreach (var kv in devopsVars)
                    envVars[kv.Key] = kv.Value;

                Console.WriteLine("✔ Azure DevOps variables loaded");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to load Azure DevOps variable group: {ex.Message}");
                return;
            }
        }

        // -----------------------------
        // 5. Load Key Vault secrets (supports multiple vaults)
        // -----------------------------
        if (envVars.ContainsKey("KEYVAULTS"))
        {
            var vaultNames = envVars["KEYVAULTS"]?.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (vaultNames != null)
            {
                Console.WriteLine($"✔ Using Key Vaults: {string.Join(", ", vaultNames)}");

                var router = new KeyVaultRouter(vaultNames);

                foreach (var key in envVars.Keys.ToList())
                {
                    if (key.StartsWith("SECRET_"))
                    {
                        try
                        {
                            // Format: SECRET_DB_PASSWORD=my-kv:DB-PASSWORD
                            envVars[key] = router.Resolve(envVars[key]);
                            Console.WriteLine($"✔ Loaded secret: {key}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Failed to load secret '{key}': {ex.Message}");
                            return;
                        }
                    }
                }
            }
        }

        // -----------------------------
        // 6. Token validation
        // -----------------------------
        Console.WriteLine("✔ Validating tokens...");

        var tokens = TokenValidator.FindTokens(template);
        var missing = TokenValidator.FindMissingTokens(tokens, envVars);

        if (missing.Any())
        {
            Console.WriteLine("❌ Missing tokens:");
            foreach (var t in missing)
                Console.WriteLine($" - {t}");

            Console.WriteLine("❌ Cannot continue until all tokens are provided.");
            return;
        }

        Console.WriteLine("✔ All tokens satisfied");

        // -----------------------------
        // 7. Apply template
        // -----------------------------
        Console.WriteLine("✔ Applying template...");
        var final = TemplateProcessor.Apply(template, envVars);

        // -----------------------------
        // 8. Write output
        // -----------------------------
        File.WriteAllText(outputFile, final);
        Console.WriteLine($"🎉 Successfully generated: {outputFile}");
    }
}
