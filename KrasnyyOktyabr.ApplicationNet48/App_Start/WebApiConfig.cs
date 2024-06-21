using System;
using System.Web.Http;
using KrasnyyOktyabr.ApplicationNet48.DependencyInjection;
using KrasnyyOktyabr.ApplicationNet48.Filters;

namespace KrasnyyOktyabr.ApplicationNet48;

public static class WebApiConfig
{
    public static Action<HttpConfiguration> Register(IServiceProvider provider) => (HttpConfiguration config) =>
    {
        config.DependencyResolver = new DependencyResolver(provider);

        config.MapHttpAttributeRoutes();

        config.Routes.MapHttpRoute(
            name: "HealthApi",
            routeTemplate: "api/{controller}" // HealthController
        );

        config.Routes.MapHttpRoute(
            name: "LegacyHealthApi",
            routeTemplate: "HealthService.svc/Status", // HealthController
            defaults: new { controller = "health" }
        );

        config.Routes.MapHttpRoute(
            name: "RestartApi",
            routeTemplate: "api/{controller}" // RestartController
        );

        config.Routes.MapHttpRoute(
            name: "JsonTransformApi",
            routeTemplate: "api/{controller}/{action}" // JsonTransformController
        );

        // Enable models validation
        config.Filters.Add(new ValidateModelAttribute());
    };
}
