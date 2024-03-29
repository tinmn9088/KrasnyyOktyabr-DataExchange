using System.Text;

namespace KrasnyyOktyabr.JsonTransform;

public static class ExceptionsHelper
{
    public static string BuildTypeNameWithParameters(Type type)
    {
        StringBuilder result = new(type.Name.Contains('`')
            ? type.Name[..type.Name.IndexOf('`')]
            : type.Name);

        if (type.IsGenericType)
        {
            result.Append('<');
            result.AppendJoin(", ", type.GenericTypeArguments.Select(BuildTypeNameWithParameters));
            result.Append('>');
        }

        return result.ToString();
    }
}
