using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GenConfig;

public class AzureDevOpsClient(string? org, string? project, string? pat)
{
    public Dictionary<string, string?> GetVariables(string? groupName)
    {
        var url = $"https://dev.azure.com/{org}/{project}/_apis/distributedtask/variablegroups?groupName={groupName}&api-version=7.1-preview.2";

        using var client = new HttpClient();
        var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

        var json = client.GetStringAsync(url).Result;
        var obj = JsonDocument.Parse(json);

        var vars = new Dictionary<string, string?>();

        foreach (var v in obj.RootElement.GetProperty("value")[0].GetProperty("variables").EnumerateObject())
        {
            vars[v.Name] = v.Value.GetProperty("value").GetString();
        }

        return vars;
    }
}