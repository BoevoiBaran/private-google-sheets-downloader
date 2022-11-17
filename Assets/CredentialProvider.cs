using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class CredentialProvider
{
    private static readonly string Path = $"{Application.dataPath}" +
                                                    $"{System.IO.Path.DirectorySeparatorChar}Credentials" +
                                                    $"{System.IO.Path.DirectorySeparatorChar}Credentials.json";
    
    public Maybe<Credentials> GetCredentials()
    {
        if (!System.IO.File.Exists(Path))
        {
            Debug.LogError($"Credentials file:{Path} not found!");
            return Maybe.None();
        }

        var dataNode = ReadFile(Path);
        var appName = GetString(dataNode, "app_name");
        var clientId = GetString(dataNode, "client_id");
        var secret = GetString(dataNode, "client_secret");

        return new Credentials(appName, clientId, secret);
    }

    private Dictionary<string, object> ReadFile(string filePath)
    {
        using var sr = new System.IO.StreamReader(filePath);
        var text = sr.ReadToEnd();
        var dataNode = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
        return dataNode;
    }
    
    private static string GetString(IDictionary<string, object> node, string key, string defaultValue = "")
    {
        return node.TryGetValue(key, out var value) ? value?.ToString() : defaultValue;
    }
}
