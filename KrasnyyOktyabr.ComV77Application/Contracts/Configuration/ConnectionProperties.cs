namespace KrasnyyOktyabr.ComV77Application.Contracts.Configuration;

public record ConnectionProperties : IEquatable<ConnectionProperties>
{
    public ConnectionProperties(string infobasePath, string username, string? password, int retryTimes = 1)
    {
        InfobasePath = infobasePath;
        Username = username;
        Password = password;
        RetryTimes = retryTimes > 1 ? retryTimes : 1;
    }

    public string InfobasePath { get; }

    public string Username { get; }

    public string? Password { get; }

    public int RetryTimes { get; set; }
}
