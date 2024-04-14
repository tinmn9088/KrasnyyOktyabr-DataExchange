namespace KrasnyyOktyabr.ComV77Application.Contracts.Configuration;

public record ConnectionProperties
{
    public required string InfobasePath { get; init; }

    public required string Username { get; init; }

    public required string Password { get; init; }
}
