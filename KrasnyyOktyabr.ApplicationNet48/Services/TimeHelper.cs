using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration;
using Microsoft.Extensions.Logging;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public static class TimeHelper
{
    private static TimeSpan WaitPeriodCheckInterval => TimeSpan.FromMinutes(1);

    /// <remarks>
    /// When <paramref name="period"/> is greater or equal to 1 day always returns <c>false</c>.
    /// </remarks>
    public static bool IsInPeriod(this DateTimeOffset dateTime, TimePeriod period)
    {
        TimeSpan time = dateTime.TimeOfDay;

        TimeSpan end = period.Start + period.Duration;

        if (end.Days >= 1 && time + TimeSpan.FromDays(end.Days) <= end)
        {
            time += TimeSpan.FromDays(end.Days);
        }

        return time >= period.Start && time <= end;
    }

    /// <summary>
    /// Uses <see cref="IsInPeriod(DateTimeOffset, TimePeriod)"/>.
    /// </summary>
    /// <exception cref="OperationCanceledException"></exception>
    public static async ValueTask WaitPeriodsEndAsync(Func<DateTimeOffset> dateTimeProvider, IEnumerable<TimePeriod> periods, CancellationToken cancellationToken = default, ILogger logger = null)
    {
        int periodNumber = 1;

        foreach (TimePeriod period in periods)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DateTimeOffset currentTime = dateTimeProvider();

            TimeSpan endTime = period.Start + period.Duration;

            if (period.Duration.Days >= 1)
            {
                logger.LogWarning("Period {PeriodNumber} never ends ({Days} days)", periodNumber, period.Duration.Days);
            }

            endTime -= TimeSpan.FromDays(endTime.Days);

            while (currentTime.IsInPeriod(period))
            {
                logger?.LogTrace("Waiting until {EndTime} (period {PeriodNumber}) ...", endTime, periodNumber);

                await Task.Delay(WaitPeriodCheckInterval, cancellationToken).ConfigureAwait(false);

                currentTime = dateTimeProvider();
            }

            periodNumber++;
        }
    }
}
