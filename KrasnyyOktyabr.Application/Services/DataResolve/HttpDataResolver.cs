using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;

namespace KrasnyyOktyabr.Application.Services.DataResolve;

public class HttpDataResolver : IDataResolver
{
    private readonly HttpClient _httpClient;

    private readonly HttpRequestMessage _request;

    /// <exception cref="ArgumentNullException"></exception>
    public HttpDataResolver(HttpClient httpClient, HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(request);

        _httpClient = httpClient;
        _request = request;
    }

    /// <exception cref="HttpRequestException"></exception>
    public async ValueTask<object?> ResolveAsync(CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _httpClient.SendAsync(_request, cancellationToken).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
