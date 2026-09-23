using System.IO;

namespace GenConfig;

public static class InitCommand
{
    public static void Run()
    {
        if (!File.Exists("appsettings.template.json"))
        {
            File.WriteAllText("appsettings.template.json",
@"{
  ""Example"": ""#{EXAMPLE_VALUE}""
}");
            Console.WriteLine("Created appsettings.template.json");
        }

        foreach (var env in new[] { "dev", "test", "prod" })
        {
            var file = $".env.{env}";
            if (!File.Exists(file))
            {
                File.WriteAllText(file,
$@"# Non-secret values for {env}
ENVIRONMENT={env.ToUpper()}
API_BASE_URL=https://{env}.api.example.com

# Key Vault
KEYVAULT_NAME=my-kv

# Secrets (refer to Key Vault secret names)
SECRET_DB_PASSWORD=DB-PASSWORD
");
                Console.WriteLine($"Created {file}");
            }
        }

        Console.WriteLine("Initialization complete.");
    }
}
