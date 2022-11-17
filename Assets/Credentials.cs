using System;

public readonly struct Credentials
{
    public readonly string AppName;
    public readonly string ClientId;
    public readonly string Secret;

    public bool IsCorrect()
    {
        return !string.IsNullOrEmpty(AppName) && !string.IsNullOrEmpty(ClientId) && !string.IsNullOrEmpty(Secret);
    }

    public Credentials(string appName, string clientId, string secret)
    {
        AppName = appName;
        ClientId = clientId;
        Secret = secret;
    }
}
