using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KrasnyyOktyabr.DataResolve;

public interface IDataResolveService
{
    /// <exception cref="ResolverNotFoundException"></exception>
    ValueTask<object> ResolveAsync(string resolverName, Dictionary<string, object> args, CancellationToken cancellationToken);

    public class ResolverNotFoundException(string resolverName) : Exception($"No '{resolverName}' resolver registered")
    {
    }
}
