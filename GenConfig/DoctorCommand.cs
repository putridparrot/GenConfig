using GenConfig;

public static class DoctorCommand
{
    public static void Run(string env, string template)
    {
        Console.WriteLine("Running diagnostics...\n");

        var envFile = $".env.{env}";
        if (!File.Exists(envFile))
            Console.WriteLine($"❌ Missing env file: {envFile}");
        else
            Console.WriteLine($"✔ Found env file: {envFile}");

        if (!File.Exists(template))
            Console.WriteLine($"❌ Missing template: {template}");
        else
            Console.WriteLine($"✔ Found template: {template}");

        var envVars = EnvLoader.Load(envFile);
        var templateText = File.ReadAllText(template);

        var tokens = TokenValidator.FindTokens(templateText);
        var missing = TokenValidator.FindMissingTokens(tokens, envVars);

        if (missing.Any())
        {
            Console.WriteLine("\n❌ Missing tokens:");
            missing.ForEach(t => Console.WriteLine($" - {t}"));
        }
        else
        {
            Console.WriteLine("\n✔ All tokens satisfied");
        }

        Console.WriteLine("\n✔ Doctor complete");
    }
}