using System;
using System.Web.Http;
using KrasnyyOktyabr.ApplicationNet48.DependencyInjection;
using KrasnyyOktyabr.ApplicationNet48.Modules.API.Filters;

namespace KrasnyyOktyabr.ApplicationNet48;

public static class WebApiConfig
{
    public static Action<HttpConfiguration> Register(IServiceProvider provider) => (HttpConfiguration config) =>
    {
        config.DependencyResolver = new DependencyResolver(provider);

        config.MapHttpAttributeRoutes();

        config.Filters.Add(new ValidateModelAttribute());
    };
}
