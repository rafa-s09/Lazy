using Lazy.Extensions;

namespace Lazy.Tests;

public class NumericExtensionsTests
{
    [Fact]
    public void OnRange_Inclusive_ReturnsTrue()
    {
        Assert.True(5.OnRange(1, 10));
        Assert.True(1.OnRange(1, 10));
        Assert.True(10.OnRange(1, 10));
    }

    [Fact]
    public void OnRange_Exclusive_ReturnsFalse()
    {
        Assert.False(0.OnRange(1, 10));
        Assert.False(11.OnRange(1, 10));
    }

    [Fact]
    public void IsBetween_Inclusive_ReturnsCorrectly()
    {
        Assert.True(5.IsBetween(1, 10, true));
        Assert.True(1.IsBetween(1, 10, true));
        Assert.True(10.IsBetween(1, 10, true));
        Assert.False(0.IsBetween(1, 10, true));
        Assert.False(11.IsBetween(1, 10, true));
    }

    [Fact]
    public void IsBetween_Exclusive_ReturnsCorrectly()
    {
        Assert.True(5.IsBetween(1, 10, false));
        Assert.False(1.IsBetween(1, 10, false));
        Assert.False(10.IsBetween(1, 10, false));
    }

    [Fact]
    public void PercentageOf_ReturnsCorrectly()
    {
        Assert.Equal(25, 50.PercentageOf(200));
        Assert.Equal(0, 0.PercentageOf(200));
        Assert.Equal(0, 50.PercentageOf(0));
    }

    [Fact]
    public void Percent_ReturnsCorrectly()
    {
        Assert.Equal(50, 200.Percent(25));
        Assert.Equal(0, 0.Percent(25));
        Assert.Equal(0, 200.Percent(0));
    }

    [Fact]
    public void IsEven_ReturnsCorrectly()
    {
        Assert.True(4.IsEven());
        Assert.True(0.IsEven());
        Assert.False(3.IsEven());
    }

    [Fact]
    public void IsOdd_ReturnsCorrectly()
    {
        Assert.True(3.IsOdd());
        Assert.False(4.IsOdd());
        Assert.False(0.IsOdd());
    }

    [Fact]
    public void Round_Decimal_ReturnsCorrectly()
    {
        Assert.Equal(1.24m, 1.235m.Round(2));
        Assert.Equal(1.23m, 1.234m.Round(2));
        Assert.Equal(1m, 1.235m.Round(0));
    }

    [Fact]
    public void Round_Double_ReturnsCorrectly()
    {
        Assert.Equal(1.24, 1.235.Round(2));
        Assert.Equal(1.23, 1.234.Round(2));
        Assert.Equal(1.0, 1.235.Round(0));
    }

    [Fact]
    public void IsApproximately_ReturnsCorrectly()
    {
        Assert.True(1.000001.IsApproximately(1.0));
        Assert.False(1.01.IsApproximately(1.0));
        Assert.True(1.0.IsApproximately(1.0, 0));
    }
}
