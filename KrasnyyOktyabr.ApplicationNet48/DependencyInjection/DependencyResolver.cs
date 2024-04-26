using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace KrasnyyOktyabr.ApplicationNet48.DependencyInjection;

public class DependencyResolver(IServiceProvider serviceProvider) : IDependencyResolver
{
    private IServiceScope _serviceScope;

    protected IServiceProvider ServiceProvider { get; set; } = serviceProvider;

    public object GetService(Type serviceType)
    {
        return ServiceProvider.GetService(serviceType);
    }

    public IEnumerable<object> GetServices(Type serviceType)
    {
        return ServiceProvider.GetServices(serviceType);
    }

    public IDependencyScope BeginScope()
    {
        _serviceScope = ServiceProvider.CreateScope();

        return new DependencyResolver(_serviceScope.ServiceProvider);
    }

    public void Dispose()
    {
        _serviceScope?.Dispose();
    }
}
