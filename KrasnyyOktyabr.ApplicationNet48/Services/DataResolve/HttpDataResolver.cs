using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;

namespace KrasnyyOktyabr.ApplicationNet48.Services.DataResolve;

/// <exception cref="ArgumentNullException"></exception>
public class HttpDataResolver(HttpClient httpClient, HttpRequestMessage request) : IDataResolver
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    private readonly HttpRequestMessage _request = request ?? throw new ArgumentNullException(nameof(request));

#nullable enable
    /// <exception cref="HttpRequestException"></exception>
    public async ValueTask<object?> ResolveAsync(CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _httpClient.SendAsync(_request, cancellationToken).ConfigureAwait(false);

        string responseContent = await response.Content.ReadAsStringAsync();

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new HttpDataResolverException($"{ex.Message} {responseContent}");
        }

        return responseContent;
    }

    public class HttpDataResolverException : Exception
    {
        internal HttpDataResolverException(string message) : base(message)
        {
        }
    }
}
