#nullable enable

using System.Net.Http;
using System;
using System.Linq;

namespace KrasnyyOktyabr.ApplicationNet48.Controllers;

public static class ControllersHelper
{
    public static string GetRequiredQueryParameter(HttpRequestMessage request, string name)
    {
        try
        {
            return request.GetQueryNameValuePairs()
                .Where(p => p.Key == name)
                .Select(p => p.Value)
                .First()
                .ToString();
        }
        catch (Exception)
        {
            throw new ArgumentException($"'{name}' query parameter missing");
        }
    }

    public static string? GetOptionalQueryParameter(HttpRequestMessage request, string name)
    {
        return request.GetQueryNameValuePairs()
                .Where(p => p.Key == name)
                .Select(p => p.Value)
                .FirstOrDefault();
    }
}
