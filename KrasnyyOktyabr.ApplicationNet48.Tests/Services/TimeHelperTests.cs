using System;
using System.Threading.Tasks;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Tests;

[TestClass]
public class TimeHelperTests
{
    [TestMethod]
    public void IsInPeriod_ShouldDetermineIsInPeriod()
    {
        int startHours = 10;

        DateTimeOffset dateTimeBefore = GetDateTime(startHours - 1);
        DateTimeOffset dateTimeInPeriod1 = GetDateTime(startHours);
        DateTimeOffset dateTimeInPeriod2 = GetDateTime(startHours + 1);
        DateTimeOffset dateTimeInPeriod3 = GetDateTime(startHours + 2);
        DateTimeOffset dateTimeAfter = GetDateTime(startHours + 3);

        TimePeriod period = new()
        {
            Start = TimeSpan.FromHours(startHours),
            Duration = TimeSpan.FromHours(2)
        };

        Assert.IsFalse(dateTimeBefore.IsInPeriod(period));
        Assert.IsTrue(dateTimeInPeriod1.IsInPeriod(period));
        Assert.IsTrue(dateTimeInPeriod2.IsInPeriod(period));
        Assert.IsTrue(dateTimeInPeriod3.IsInPeriod(period));
        Assert.IsFalse(dateTimeAfter.IsInPeriod(period));
    }

    [TestMethod]
    public void IsInPeriod_WhenPeriodWithMidnight_ShouldDetermineIsInPeriod()
    {
        int startHours = 23;

        DateTimeOffset dateTimeBefore = GetDateTime(startHours - 1);
        DateTimeOffset dateTimeInPeriod1 = GetDateTime(startHours);
        DateTimeOffset dateTimeInPeriod2 = GetDateTime(startHours + 1);
        DateTimeOffset dateTimeInPeriod3 = GetDateTime(startHours + 2);
        DateTimeOffset dateTimeAfter = GetDateTime(startHours + 3);

        TimePeriod period = new()
        {
            Start = TimeSpan.FromHours(startHours),
            Duration = TimeSpan.FromHours(2)
        };

        Assert.IsFalse(dateTimeBefore.IsInPeriod(period));
        Assert.IsTrue(dateTimeInPeriod1.IsInPeriod(period));
        Assert.IsTrue(dateTimeInPeriod2.IsInPeriod(period));
        Assert.IsTrue(dateTimeInPeriod3.IsInPeriod(period));
        Assert.IsFalse(dateTimeAfter.IsInPeriod(period));
    }

    [TestMethod]
    public void IsInPeriod_WhenPeriodMultipleDays_ShouldReturnFalse()
    {
        int startHours = 10;

        DateTimeOffset dateTimeInPeriod1 = GetDateTime(startHours - 1);
        DateTimeOffset dateTimeInPeriod2 = GetDateTime(startHours);
        DateTimeOffset dateTimeInPeriod3 = GetDateTime(startHours + 3);

        TimePeriod period = new()
        {
            Start = TimeSpan.FromHours(startHours),
            Duration = TimeSpan.FromDays(2)
        };

        Assert.IsTrue(dateTimeInPeriod1.IsInPeriod(period));
        Assert.IsTrue(dateTimeInPeriod2.IsInPeriod(period));
        Assert.IsTrue(dateTimeInPeriod3.IsInPeriod(period));
    }

    private static DateTimeOffset GetDateTime(int hours) => new(2024, 6, 3 + (hours / 24), hours % 24, 0, 0, TimeSpan.FromTicks(0));
}
