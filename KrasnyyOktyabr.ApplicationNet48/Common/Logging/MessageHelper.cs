using System.Text;

namespace KrasnyyOktyabr.ApplicationNet48.Common.Logging;

public static class MessageHelper
{
    public static string ShortenMessage(string message, int lengthLimit)
    {
        if (message.Length <= lengthLimit)
        {
            return message;
        }

        string startEndSeparator = " ... ";

        if (message.Length <= startEndSeparator.Length)
        {
            return message;
        }

        StringBuilder stringBuilder = new();

        int leftOffset = (lengthLimit - startEndSeparator.Length) / 2;
        int rightOffset = message.Length - (lengthLimit - (leftOffset + startEndSeparator.Length));

        stringBuilder.Append(message.Substring(0, leftOffset));
        stringBuilder.Append(startEndSeparator);
        stringBuilder.Append(message.Substring(rightOffset));

        return stringBuilder.ToString();
    }
}
