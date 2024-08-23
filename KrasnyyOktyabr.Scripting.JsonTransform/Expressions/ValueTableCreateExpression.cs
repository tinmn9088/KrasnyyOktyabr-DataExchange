using KrasnyyOktyabr.JsonTransform.Structures;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class ValueTableCreateExpression(IExpression<Task<string>> columnsStringExpression)
    : AbstractExpression<Task<IValueTable>>
{
    private readonly IExpression<Task<string>> _columnsStringExpression = columnsStringExpression ?? throw new ArgumentNullException(nameof(columnsStringExpression));

    public override async Task<IValueTable> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            string columnsString = await _columnsStringExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false)
                ?? throw new NullReferenceException();

            IEnumerable<string> columns = columnsString
                .Split(',')
                .Select(c => c.Trim());

            return new ValueTable(columns);
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
