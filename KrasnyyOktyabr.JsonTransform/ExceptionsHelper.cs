using System.Text;

namespace KrasnyyOktyabr.JsonTransform;

public static class ExceptionsHelper
{
    public static string BuildTypeNameWithParameters(Type type)
    {
        StringBuilder result = new(type.Name.Contains('`')
            ? type.Name.Substring(0, type.Name.IndexOf('`'))
            : type.Name);

        if (type.IsGenericType)
        {
            result.Append('<');
            result.Append(string.Join(", ", type.GenericTypeArguments.Select(BuildTypeNameWithParameters)));
            result.Append('>');
        }

        return result.ToString();
    }
}
