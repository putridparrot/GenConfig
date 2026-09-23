using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GenConfig;

public static class ConfigCommand
{
    public static void Run(string env, string templateFile, string outputFile)
    {
        var envFile = $".env.{env}";

        Console.WriteLine($"Generating config for {env}");
        Console.WriteLine($"Template: {templateFile}");
        Console.WriteLine($"Output:   {outputFile}");

        if (!File.Exists(envFile))
        {
            Console.WriteLine($"Missing env file: {envFile}");
            return;
        }

        if (!File.Exists(templateFile))
        {
            Console.WriteLine($"Missing template file: {templateFile}");
            return;
        }

        var envVars = EnvLoader.Load(envFile);
        var template = File.ReadAllText(templateFile);

        // Key Vault secrets
        if (envVars.TryGetValue("KEYVAULT_NAME", out var vaultName))
        {
            var kv = new KeyVaultClient(vaultName);

            foreach (var key in envVars.Keys.ToList())
            {
                if (key.StartsWith("SECRET_"))
                {
                    var secretName = envVars[key];
                    envVars[key] = kv.GetSecret(secretName);
                }
            }
        }

        var final = TemplateProcessor.Apply(template, envVars);
        File.WriteAllText(outputFile, final);

        Console.WriteLine($"Generated {outputFile}");
    }
}