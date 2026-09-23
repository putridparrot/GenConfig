using GenConfig;

public static class ListCommand
{
    public static void Run()
    {
        Console.WriteLine("Environments:");
        foreach (var f in Directory.GetFiles(".", ".env.*"))
            Console.WriteLine(" - " + f);

        Console.WriteLine("\nTemplates:");
        foreach (var f in Directory.GetFiles(".", "*.template.json"))
            Console.WriteLine(" - " + f);

        Console.WriteLine("\nKey Vaults:");
        foreach (var env in Directory.GetFiles(".", ".env.*"))
        {
            var vars = EnvLoader.Load(env);
            if (vars.ContainsKey("KEYVAULTS"))
                Console.WriteLine(" - " + vars["KEYVAULTS"]);
        }

        Console.WriteLine("\nAzure DevOps Variable Groups:");
        foreach (var env in Directory.GetFiles(".", ".env.*"))
        {
            var vars = EnvLoader.Load(env);
            if (vars.ContainsKey("AZDO_GROUP"))
                Console.WriteLine(" - " + vars["AZDO_GROUP"]);
        }
    }
}
