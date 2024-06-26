﻿using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class DataResolveExpression : AbstractExpression<Task<object?>>
{
    private readonly IExpression<Task<string>> _resolverExpression;

    private readonly IExpression<Task<Dictionary<string, object?>>> _argsExpression;

    private readonly IDataResolveService _dataResolveService;

    /// <exception cref="ArgumentNullException"></exception>
    public DataResolveExpression(
        IExpression<Task<string>> resolverExpression,
        IExpression<Task<Dictionary<string, object?>>> argsExpression,
        IDataResolveService dataResolveService)
    {
        _resolverExpression = resolverExpression ?? throw new ArgumentNullException(nameof(resolverExpression));
        _argsExpression = argsExpression ?? throw new ArgumentNullException(nameof(argsExpression));
        _dataResolveService = dataResolveService ?? throw new ArgumentNullException(nameof(dataResolveService));
    }

    /// <exception cref="IDataResolveService.ResolverNotFoundException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    public override async Task<object?> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            string resolver = await _resolverExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            Dictionary<string, object?> args = await _argsExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

            return await _dataResolveService.ResolveAsync(resolver, args, cancellationToken).ConfigureAwait(false);
        }
        catch (InterpretException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InterpretException(ex.Message, Mark);
        }
    }
}
