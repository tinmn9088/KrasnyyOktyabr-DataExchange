namespace KrasnyyOktyabr.Application.Services.Kafka;

public static class V77ApplicationProducersHelper
{
    public readonly struct ObjectFilter
    {
        public required string Id { get; init; }

        public required int Depth { get; init; }
    }

    public static string DefaultErtRelativePath => @"ExtForms\EDO\Test\UN_JSON_Synch.ert";

    public static char ObjectFilterValuesSeparator => ':';

    public static int ObjectFilterDefaultDepth => 1;

    public static string TransactionTypePropertyName => "ТипТранзакции";

    public static string ObjectDatePropertyName => "ДатаДокИзЛогов";

    public class FailedToGetObjectException : Exception
    {
        internal FailedToGetObjectException(string objectId)
            : base($"Failed to get object with id '{objectId}'")
        {
        }
    }
}
