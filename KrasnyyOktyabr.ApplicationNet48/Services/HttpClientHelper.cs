#nullable enable

using System;
using System.Net.Http.Headers;
using System.Text;

namespace KrasnyyOktyabr.ApplicationNet48.Services;
public static class HttpClientHelper
{
    public static AuthenticationHeaderValue GetAuthenticationHeaderValue(string username, string? password)
    {
        string credential = $"{username}:{password}";
        string encodedCredential = Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));
        return new AuthenticationHeaderValue("Basic", encodedCredential);
    }
}
