using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

public static class V77ApplicationHelper
{
    public readonly struct ObjectFilter(string id, int depth)
    {
        public string Id { get; } = id;

        public int Depth { get; } = depth;
    }

    public static string DefaultErtRelativePath => @"ExtForms\EDO\Test\UN_JSON_Synch.ert";

    public static char ObjectFilterValuesSeparator => ':';

    public static int ObjectFilterDefaultDepth => 1;

    public static string TransactionTypePropertyName => "ТипТранзакции";

    public static string ObjectDatePropertyName => "ДатаДокИзЛогов";

    public static async ValueTask WaitRdSessionsAllowed(IWmiService wmiService, ILogger logger)
    {
        try
        {
            bool? areRdSessionsAllowed = wmiService.AreRdSessionsAllowed();

            while (areRdSessionsAllowed == false)
            {
                logger.LogTrace("Wait until RDP is allowed");

                await Task.Delay(TimeSpan.FromSeconds(1));

                areRdSessionsAllowed = wmiService.AreRdSessionsAllowed();
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to check is RDP allowed");
        }
    }

    public class FailedToGetObjectException : Exception
    {
        internal FailedToGetObjectException(string objectId)
            : base($"Failed to get object with id '{objectId}'")
        {
        }
    }
}
