using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace KrasnyyOktyabr.ApplicationNet48.DependencyInjection;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddControllers(this IServiceCollection services)
    {
        List<Type> controllerTypes = Assembly.GetExecutingAssembly().GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => typeof(IHttpController).IsAssignableFrom(t))
            .ToList();

        foreach (Type type in controllerTypes)
        {
            services.AddTransient(type);
        }

        return services;
    }
}
