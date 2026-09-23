using System.Text.RegularExpressions;

public static class TokenValidator
{
    public static List<string> FindTokens(string template)
    {
        var matches = Regex.Matches(template, "#\\{([^}]+)\\}");
        return matches.Select(m => m.Groups[1].Value).Distinct().ToList();
    }

    public static List<string> FindMissingTokens(List<string> tokens, Dictionary<string, string?> values)
    {
        return tokens.Where(t => !values.ContainsKey(t)).ToList();
    }
}
