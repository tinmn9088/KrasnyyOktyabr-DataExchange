﻿namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Returns passed <paramref name="value"/>.
/// </summary>
public sealed class ConstExpression(object? value) : IExpression<Task<object?>>
{
    private readonly Task<object?> _valueTask = Task.FromResult(value);

    public Task<object?> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        return _valueTask;
    }
}