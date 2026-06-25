namespace Lazy.Extensions;

public static partial class NumericExtensions
{
    #region Range    

    public static bool OnRange<T>(this T value, T min, T max) where T : INumber<T> => value >= min && value <= max;

    public static bool IsBetween<T>(this T value, T min, T max, bool inclusive = true) where T : INumber<T> => inclusive ? value >= min && value <= max : value > min && value < max;

    #endregion Range

    #region Percentage-based fluid treatment

    public static T PercentageOf<T>(this T value, T total) where T : INumber<T>
    {
        if (total == T.Zero)
            return T.Zero;

        return (value * T.CreateChecked(100)) / total;
    }

    public static T Percent<T>(this T value, T percentage) where T : INumber<T> => (value * percentage) / T.CreateChecked(100);

    #endregion Percentage-based fluid treatment

    #region Clean Parity Check

    public static bool IsEven<T>(this T value) where T : IBinaryInteger<T> => T.IsEvenInteger(value);

    public static bool IsOdd<T>(this T value) where T : IBinaryInteger<T> => T.IsOddInteger(value);

    #endregion Clean Parity Check

    #region Fluid Rounding for Floating-Point Types

    public static decimal Round(this decimal value, int decimals = 2, MidpointRounding mode = MidpointRounding.AwayFromZero) => decimal.Round(value, decimals, mode);

    public static double Round(this double value, int decimals = 2, MidpointRounding mode = MidpointRounding.AwayFromZero) => Math.Round(value, decimals, mode);

    #endregion Fluid Rounding for Floating-Point Types

    #region Safe comparison of floats
    public static bool IsApproximately(this double value, double target, double tolerance = 0.00001) => Math.Abs(value - target) <= tolerance;

    #endregion Safe comparison of floats
}
