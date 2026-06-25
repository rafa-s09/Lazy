using Lazy.Extensions;

namespace Lazy.Tests;

public class DateTimeExtensionsTests
{
    [Fact]
    public void IsExpired_DateTime_ReturnsCorrectly()
    {
        DateTime past = DateTime.UtcNow.AddDays(-1);
        DateTime future = DateTime.UtcNow.AddDays(1);

        Assert.True(past.IsExpired());
        Assert.False(future.IsExpired());
    }

    [Fact]
    public void IsExpired_DateTimeOffset_ReturnsCorrectly()
    {
        DateTimeOffset past = DateTimeOffset.UtcNow.AddDays(-1);
        DateTimeOffset future = DateTimeOffset.UtcNow.AddDays(1);

        Assert.True(past.IsExpired());
        Assert.False(future.IsExpired());
    }

    [Fact]
    public void IsWithinRange_DateTime_ReturnsCorrectly()
    {
        DateTime date = new DateTime(2024, 6, 15);
        DateTime start = new DateTime(2024, 6, 1);
        DateTime end = new DateTime(2024, 6, 30);

        Assert.True(date.IsWithinRange(start, end));
        Assert.False(new DateTime(2024, 5, 1).IsWithinRange(start, end));
    }

    [Fact]
    public void IsWithinRange_DateTimeOffset_ReturnsCorrectly()
    {
        DateTimeOffset date = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset start = new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero);
        DateTimeOffset end = new DateTimeOffset(2024, 6, 30, 0, 0, 0, TimeSpan.Zero);

        Assert.True(date.IsWithinRange(start, end));
        Assert.False(new DateTimeOffset(2024, 5, 1, 0, 0, 0, TimeSpan.Zero).IsWithinRange(start, end));
    }

    [Fact]
    public void ToUtcSafe_Local_ConvertsToUtc()
    {
        DateTime local = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Local);
        DateTime utc = local.ToUtcSafe();
        Assert.Equal(DateTimeKind.Utc, utc.Kind);
    }

    [Fact]
    public void ToStartOfDay_ReturnsMidnightUtc()
    {
        DateTime date = new DateTime(2024, 6, 15, 14, 30, 0, DateTimeKind.Utc);
        DateTime startOfDay = date.ToStartOfDay();
        Assert.Equal(new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc), startOfDay);
        Assert.Equal(DateTimeKind.Utc, startOfDay.Kind);
    }

    [Fact]
    public void ToEndOfDay_ReturnsLastMomentUtc()
    {
        DateTime date = new DateTime(2024, 6, 15, 14, 30, 0, DateTimeKind.Utc);
        DateTime endOfDay = date.ToEndOfDay();
        Assert.Equal(new DateTime(2024, 6, 15, 23, 59, 59, 999, DateTimeKind.Utc).AddTicks(9999), endOfDay);
        Assert.Equal(DateTimeKind.Utc, endOfDay.Kind);
    }

    [Fact]
    public void FirstDayOfMonth_ReturnsCorrectDate()
    {
        DateTime date = new DateTime(2024, 6, 15, 14, 30, 0, DateTimeKind.Utc);
        DateTime firstDay = date.FirstDayOfMonth();
        Assert.Equal(new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc), firstDay);
    }

    [Fact]
    public void LastDayOfMonth_ReturnsCorrectDate()
    {
        DateTime date = new DateTime(2024, 2, 10, 14, 30, 0, DateTimeKind.Utc);
        DateTime lastDay = date.LastDayOfMonth();
        Assert.Equal(new DateTime(2024, 2, 29, 0, 0, 0, DateTimeKind.Utc), lastDay); // Leap year
    }

    [Fact]
    public void UnixTimestamp_Roundtrip_Successful()
    {
        DateTime date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        long timestamp = date.ToUnixTimestamp();
        DateTime roundtrip = timestamp.FromUnixTimestamp();
        Assert.Equal(date, roundtrip);
    }
}
