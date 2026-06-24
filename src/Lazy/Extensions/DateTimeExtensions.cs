namespace Lazy.Extensions;

public static partial class DateTimeExtensions
{
    #region Comparisons

    #region DateTime

    /// <summary>
    /// Determines whether the expiry date has already passed relative to <see cref="DateTime.UtcNow"/>.
    /// The date is safely converted to UTC before comparison via <see cref="ToUtcSafe"/>.
    /// </summary>
    /// <param name="expiryDate">The expiry date to evaluate.</param>
    /// <param name="ignoreTime">
    /// When <see langword="true"/>, only the date component is compared, ignoring the time of day.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="expiryDate"/> is in the past; otherwise <see langword="false"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// DateTime.UtcNow.AddDays(-1).IsExpired();               // true
    /// DateTime.UtcNow.AddDays(1).IsExpired();                // false
    /// DateTime.UtcNow.AddHours(-1).IsExpired(ignoreTime: true); // false (same UTC day)
    /// </code>
    /// </example>
    public static bool IsExpired(this DateTime expiryDate, bool ignoreTime = false)
    {
        DateTime utcExpiry = expiryDate.ToUtcSafe();

        if (ignoreTime)
            return utcExpiry.Date < DateTime.UtcNow.Date;

        return utcExpiry < DateTime.UtcNow;
    }

    /// <summary>
    /// Determines whether the date falls within the specified range (inclusive on both ends).
    /// All values are safely converted to UTC before comparison via <see cref="ToUtcSafe"/>.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <param name="start">The start of the range (inclusive).</param>
    /// <param name="end">The end of the range (inclusive).</param>
    /// <param name="ignoreTime">
    /// When <see langword="true"/>, only the date component is compared, ignoring the time of day.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="date"/> falls within the range; otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="start"/> is greater than <paramref name="end"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// var date  = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc);
    /// var start = new DateTime(2024, 6,  1, 0, 0, 0, DateTimeKind.Utc);
    /// var end   = new DateTime(2024, 6, 30, 0, 0, 0, DateTimeKind.Utc);
    ///
    /// date.IsWithinRange(start, end);                   // true
    /// date.IsWithinRange(start, end, ignoreTime: true); // true
    /// </code>
    /// </example>
    public static bool IsWithinRange(this DateTime date, DateTime start, DateTime end, bool ignoreTime = false)
    {
        DateTime utcDate = date.ToUtcSafe();
        DateTime utcStart = start.ToUtcSafe();
        DateTime utcEnd = end.ToUtcSafe();

        if (utcStart > utcEnd)
            throw new ArgumentException("The start date must be less than or equal to the end date.", nameof(start));

        if (ignoreTime)
            return utcDate.Date >= utcStart.Date && utcDate.Date <= utcEnd.Date;

        return utcDate >= utcStart && utcDate <= utcEnd;
    }
    #endregion DateTime

    #region DateTimeOffset

    /// <summary>
    /// Determines whether the expiry date has already passed relative to <see cref="DateTimeOffset.UtcNow"/>.
    /// All comparisons are performed in UTC.
    /// </summary>
    /// <param name="expiryDate">The expiry date to evaluate.</param>
    /// <param name="ignoreTime">
    /// When <see langword="true"/>, only the UTC date component is compared, ignoring the time of day.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="expiryDate"/> is in the past; otherwise <see langword="false"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// DateTimeOffset.UtcNow.AddDays(-1).IsExpired();               // true
    /// DateTimeOffset.UtcNow.AddDays(1).IsExpired();                // false
    /// DateTimeOffset.UtcNow.AddHours(-1).IsExpired(ignoreTime: true); // false (same UTC day)
    /// </code>
    /// </example>
    public static bool IsExpired(this DateTimeOffset expiryDate, bool ignoreTime = false) => ignoreTime ? expiryDate.UtcDateTime.Date < DateTime.UtcNow.Date : expiryDate < DateTimeOffset.UtcNow;

    /// <summary>
    /// Determines whether the date falls within the specified range (inclusive on both ends).
    /// All comparisons are performed in UTC.
    /// </summary>
    /// <param name="date">The date to evaluate.</param>
    /// <param name="start">The start of the range (inclusive).</param>
    /// <param name="end">The end of the range (inclusive).</param>
    /// <param name="ignoreTime">
    /// When <see langword="true"/>, only the UTC date component is compared, ignoring the time of day.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="date"/> falls within the range; otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="start"/> is greater than <paramref name="end"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// var date  = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
    /// var start = new DateTimeOffset(2024, 6,  1, 0, 0, 0, TimeSpan.Zero);
    /// var end   = new DateTimeOffset(2024, 6, 30, 0, 0, 0, TimeSpan.Zero);
    ///
    /// date.IsWithinRange(start, end);                   // true
    /// date.IsWithinRange(start, end, ignoreTime: true); // true
    /// </code>
    /// </example>
    public static bool IsWithinRange(this DateTimeOffset date, DateTimeOffset start, DateTimeOffset end, bool ignoreTime = false)
    {
        if (ignoreTime)
        {
            if (start.UtcDateTime.Date > end.UtcDateTime.Date)
                throw new ArgumentException("The start date must be less than or equal to the end date.", nameof(start));

            return date.UtcDateTime.Date >= start.UtcDateTime.Date && date.UtcDateTime.Date <= end.UtcDateTime.Date;
        }

        if (start > end)
            throw new ArgumentException("The start date must be less than or equal to the end date.", nameof(start));

        return date >= start && date <= end;
    }
    #endregion DateTimeOffset

    #endregion Comparisons

    #region Conversions

    /// <summary>
    /// Safely converts a <see cref="DateTime"/> to UTC, handling all <see cref="DateTimeKind"/> cases.
    /// </summary>
    /// <remarks>
    /// The conversion strategy depends on <see cref="DateTime.Kind"/>:
    /// <list type="bullet">
    ///   <item>
    ///     <term><see cref="DateTimeKind.Local"/></term>
    ///     <description>Converted to UTC via <see cref="DateTime.ToUniversalTime()"/>, applying the local timezone offset.</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="DateTimeKind.Utc"/></term>
    ///     <description>Returned as-is — already UTC.</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="DateTimeKind.Unspecified"/></term>
    ///     <description>
    ///     Treated as UTC by specifying the kind via <see cref="DateTime.SpecifyKind"/>.
    ///     No offset is applied since the timezone is unknown.
    ///     </description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <param name="date">The <see cref="DateTime"/> to convert.</param>
    /// <returns>A <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/>.</returns>
    /// <example>
    /// <code>
    /// // Local: applies timezone offset
    /// DateTime.Now.ToUtcSafe();
    ///
    /// // Utc: returned unchanged
    /// DateTime.UtcNow.ToUtcSafe();
    ///
    /// // Unspecified: Kind is set to Utc, no offset applied
    /// new DateTime(2024, 6, 15).ToUtcSafe();
    /// </code>
    /// </example>
    public static DateTime ToUtcSafe(this DateTime date) => date.Kind == DateTimeKind.Local ? date.ToUniversalTime() : DateTime.SpecifyKind(date, DateTimeKind.Utc);

    #endregion Conversions

    #region Boundaries

    /// <summary>
    /// Returns a new <see cref="DateTime"/> representing the start of the day (00:00:00.000)
    /// for the given date, in UTC.
    /// </summary>
    /// <remarks>
    /// The date is safely converted to UTC via <see cref="ToUtcSafe"/> before
    /// the time component is stripped, ensuring consistent behavior regardless
    /// of the original <see cref="DateTime.Kind"/>.
    /// </remarks>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>
    /// A <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> set to
    /// midnight (00:00:00.000) on the same calendar date as <paramref name="date"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// new DateTime(2024, 6, 15, 14, 30, 0, DateTimeKind.Utc).ToStartOfDay();
    /// // returns 2024-06-15 00:00:00.000 UTC
    /// </code>
    /// </example>
    public static DateTime ToStartOfDay(this DateTime date) => DateTime.SpecifyKind(date.ToUtcSafe().Date, DateTimeKind.Utc);

    /// <summary>
    /// Returns a new <see cref="DateTime"/> representing the last moment of the day
    /// (23:59:59.9999999) for the given date, in UTC.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The date is safely converted to UTC via <see cref="ToUtcSafe"/> before
    /// the end-of-day time is applied.
    /// </para>
    /// <para>
    /// Uses <see cref="TimeOnly.MaxValue"/> (<c>23:59:59.9999999</c>) to represent
    /// the very last tick of the day, suitable for inclusive range queries.
    /// </para>
    /// </remarks>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>
    /// A <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> set to
    /// <c>23:59:59.9999999</c> on the same calendar date as <paramref name="date"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// new DateTime(2024, 6, 15, 14, 30, 0, DateTimeKind.Utc).ToEndOfDay();
    /// // returns 2024-06-15 23:59:59.9999999 UTC
    /// </code>
    /// </example>
    public static DateTime ToEndOfDay(this DateTime date)
    {
        DateTime utc = date.ToUtcSafe();
        return DateTime.SpecifyKind(utc.Date.Add(TimeOnly.MaxValue.ToTimeSpan()), DateTimeKind.Utc);
    }

    /// <summary>
    /// Returns a new <see cref="DateTime"/> representing the first day of the month
    /// at midnight (00:00:00.000), in UTC.
    /// </summary>
    /// <remarks>
    /// The date is safely converted to UTC via <see cref="ToUtcSafe"/> before
    /// the day component is set to 1.
    /// </remarks>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>
    /// A <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> set to the
    /// first day of the same month and year as <paramref name="date"/>, at midnight.
    /// </returns>
    /// <example>
    /// <code>
    /// new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc).FirstDayOfMonth();
    /// // returns 2024-06-01 00:00:00.000 UTC
    /// </code>
    /// </example>
    public static DateTime FirstDayOfMonth(this DateTime date)
    {
        DateTime utc = date.ToUtcSafe();
        return DateTime.SpecifyKind(new DateTime(utc.Year, utc.Month, 1), DateTimeKind.Utc);
    }

    /// <summary>
    /// Returns a new <see cref="DateTime"/> representing the last day of the month
    /// at midnight (00:00:00.000), in UTC.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The date is safely converted to UTC via <see cref="ToUtcSafe"/> before
    /// the last day is resolved.
    /// </para>
    /// <para>
    /// Uses <see cref="DateTime.DaysInMonth"/> to correctly handle months with
    /// different lengths, including leap years for February.
    /// </para>
    /// </remarks>
    /// <param name="date">The date to evaluate.</param>
    /// <returns>
    /// A <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> set to the
    /// last day of the same month and year as <paramref name="date"/>, at midnight.
    /// </returns>
    /// <example>
    /// <code>
    /// new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Utc).LastDayOfMonth();
    /// // returns 2024-02-29 00:00:00.000 UTC  (2024 is a leap year)
    ///
    /// new DateTime(2023, 2, 10, 0, 0, 0, DateTimeKind.Utc).LastDayOfMonth();
    /// // returns 2023-02-28 00:00:00.000 UTC
    /// </code>
    /// </example>
    public static DateTime LastDayOfMonth(this DateTime date)
    {
        DateTime utc = date.ToUtcSafe();
        int lastDay = DateTime.DaysInMonth(utc.Year, utc.Month);
        return DateTime.SpecifyKind(new DateTime(utc.Year, utc.Month, lastDay), DateTimeKind.Utc);
    }

    #endregion Boundaries

    #region Unix

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a Unix timestamp (seconds since 1970-01-01 00:00:00 UTC).
    /// </summary>
    /// <remarks>
    /// The date is safely converted to UTC via <see cref="ToUtcSafe"/> before
    /// the Unix offset is calculated, ensuring correct results regardless of
    /// the original <see cref="DateTime.Kind"/>.
    /// </remarks>
    /// <param name="date">The date to convert.</param>
    /// <returns>
    /// A <see cref="long"/> representing the number of seconds elapsed since
    /// the Unix epoch (1970-01-01 00:00:00 UTC).
    /// Negative values represent dates before the epoch.
    /// </returns>
    /// <example>
    /// <code>
    /// new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTimestamp();
    /// // returns 1704067200
    ///
    /// new DateTime(1969, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTimestamp();
    /// // returns -31536000  (before epoch)
    /// </code>
    /// </example>
    public static long ToUnixTimestamp(this DateTime date) => new DateTimeOffset(date.ToUtcSafe()).ToUnixTimeSeconds();

    /// <summary>
    /// Converts a Unix timestamp (seconds since 1970-01-01 00:00:00 UTC)
    /// to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/>.
    /// </summary>
    /// <param name="unixTimestamp">
    /// The number of seconds since the Unix epoch.
    /// Negative values represent dates before 1970-01-01.
    /// </param>
    /// <returns>
    /// A <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/>
    /// corresponding to the given Unix timestamp.
    /// </returns>
    /// <example>
    /// <code>
    /// 1704067200L.FromUnixTimestamp();
    /// // returns 2024-01-01 00:00:00 UTC
    ///
    /// 0L.FromUnixTimestamp();
    /// // returns 1970-01-01 00:00:00 UTC
    /// </code>
    /// </example>
    public static DateTime FromUnixTimestamp(this long unixTimestamp) => DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;

    #endregion Unix
}
