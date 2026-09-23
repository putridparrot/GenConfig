namespace GenConfig;

public static class TemplateProcessor
{
    public static string Apply(string template, Dictionary<string, string?> values)
    {
        var output = template;

        foreach (var kv in values)
        {
            output = output.Replace($"#{{{kv.Key}}}", kv.Value);
        }

        return output;
    }
}