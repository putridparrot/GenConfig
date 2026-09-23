using GenConfig;

var argsList = args.ToList();

if (argsList.Count == 0)
{
    Console.WriteLine("Usage: genconfig <environment> [--template <file>] [--output <file>] or genconfig init");
    return;
}

if (argsList[0] == "init")
{
    InitCommand.Run();
    return;
}

var env = argsList[0];
var template = argsList.Contains("--template")
    ? argsList[argsList.IndexOf("--template") + 1]
    : "appsettings.template.json";

var output = argsList.Contains("--output")
    ? argsList[argsList.IndexOf("--output") + 1]
    : "appsettings.json";

ConfigCommand.Run(env, template, output);