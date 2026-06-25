namespace Lazy.Extensions;

public static partial class NumericExtensions
{
    #region Range    

    /// <summary>
    /// Checks if a value is within a specified range (inclusive).
    /// </summary>
    /// <typeparam name="T">The numeric type implementing INumber.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    /// <returns>True if the value is between min and max (inclusive); otherwise, false.</returns>
    /// <example>
    /// <code>
    /// 5.OnRange(1, 10); // returns true
    /// </code>
    /// </example>
    public static bool OnRange<T>(this T value, T min, T max) where T : INumber<T> => value >= min && value <= max;

    /// <summary>
    /// Checks if a value is between a specified min and max, optionally inclusive.
    /// </summary>
    /// <typeparam name="T">The numeric type implementing INumber.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum bound.</param>
    /// <param name="max">The maximum bound.</param>
    /// <param name="inclusive">True to include bounds; false to exclude. Defaults to true.</param>
    /// <returns>True if the value is within the range based on the inclusive parameter.</returns>
    /// <example>
    /// <code>
    /// 5.IsBetween(1, 10); // returns true
    /// 5.IsBetween(5, 10, false); // returns false
    /// </code>
    /// </example>
    public static bool IsBetween<T>(this T value, T min, T max, bool inclusive = true) where T : INumber<T> => inclusive ? value >= min && value <= max : value > min && value < max;

    #endregion Range

    #region Percentage-based fluid treatment

    /// <summary>
    /// Calculates what percentage the value is of the given total.
    /// </summary>
    /// <typeparam name="T">The numeric type implementing INumber.</typeparam>
    /// <param name="value">The current value.</param>
    /// <param name="total">The total value.</param>
    /// <returns>The calculated percentage, or zero if the total is zero.</returns>
    /// <example>
    /// <code>
    /// 50.PercentageOf(200); // returns 25
    /// </code>
    /// </example>
    public static T PercentageOf<T>(this T value, T total) where T : INumber<T>
    {
        if (total == T.Zero)
            return T.Zero;

        return (value * T.CreateChecked(100)) / total;
    }

    /// <summary>
    /// Calculates the given percentage of the value.
    /// </summary>
    /// <typeparam name="T">The numeric type implementing INumber.</typeparam>
    /// <param name="value">The value to calculate the percentage for.</param>
    /// <param name="percentage">The percentage to calculate.</param>
    /// <returns>The calculated value representing the percentage.</returns>
    /// <example>
    /// <code>
    /// 200.Percent(25); // returns 50
    /// </code>
    /// </example>
    public static T Percent<T>(this T value, T percentage) where T : INumber<T> => (value * percentage) / T.CreateChecked(100);

    #endregion Percentage-based fluid treatment

    #region Clean Parity Check

    /// <summary>
    /// Determines whether the specified integer value is even.
    /// </summary>
    /// <typeparam name="T">The numeric type implementing IBinaryInteger.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is even; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// 4.IsEven(); // returns true
    /// </code>
    /// </example>
    public static bool IsEven<T>(this T value) where T : IBinaryInteger<T> => T.IsEvenInteger(value);

    /// <summary>
    /// Determines whether the specified integer value is odd.
    /// </summary>
    /// <typeparam name="T">The numeric type implementing IBinaryInteger.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is odd; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// 3.IsOdd(); // returns true
    /// </code>
    /// </example>
    public static bool IsOdd<T>(this T value) where T : IBinaryInteger<T> => T.IsOddInteger(value);

    #endregion Clean Parity Check

    #region Fluid Rounding for Floating-Point Types

    /// <summary>
    /// Rounds a decimal value to a specified number of fractional digits.
    /// </summary>
    /// <param name="value">The decimal value to round.</param>
    /// <param name="decimals">The number of decimal places in the return value. Defaults to 2.</param>
    /// <param name="mode">The rounding strategy. Defaults to AwayFromZero.</param>
    /// <returns>The rounded decimal value.</returns>
    /// <example>
    /// <code>
    /// 1.235m.Round(2); // returns 1.24m
    /// </code>
    /// </example>
    public static decimal Round(this decimal value, int decimals = 2, MidpointRounding mode = MidpointRounding.AwayFromZero) => decimal.Round(value, decimals, mode);

    /// <summary>
    /// Rounds a double value to a specified number of fractional digits.
    /// </summary>
    /// <param name="value">The double value to round.</param>
    /// <param name="decimals">The number of decimal places in the return value. Defaults to 2.</param>
    /// <param name="mode">The rounding strategy. Defaults to AwayFromZero.</param>
    /// <returns>The rounded double value.</returns>
    /// <example>
    /// <code>
    /// 1.235.Round(2); // returns 1.24
    /// </code>
    /// </example>
    public static double Round(this double value, int decimals = 2, MidpointRounding mode = MidpointRounding.AwayFromZero) => Math.Round(value, decimals, mode);

    #endregion Fluid Rounding for Floating-Point Types

    #region Safe comparison of floats
    /// <summary>
    /// Checks if two double values are approximately equal within a specified tolerance.
    /// </summary>
    /// <param name="value">The current double value.</param>
    /// <param name="target">The target double value to compare to.</param>
    /// <param name="tolerance">The maximum difference allowed. Defaults to 0.00001.</param>
    /// <returns>True if the absolute difference is less than or equal to the tolerance; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// 1.000001.IsApproximately(1.0); // returns true
    /// </code>
    /// </example>
    public static bool IsApproximately(this double value, double target, double tolerance = 0.00001) => Math.Abs(value - target) <= tolerance;

    #endregion Safe comparison of floats
}
