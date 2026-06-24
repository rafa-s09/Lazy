namespace Lazy.Extensions;

/// <summary>
/// Defines the character encoding to use when converting between
/// <see cref="string"/> and <see cref="T:byte[]"/>.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>
///   <term><see cref="ASCII"/></term>
///   <description>7-bit encoding. Only supports characters U+0000 to U+007F. Non-ASCII characters are lost.</description>
/// </item>
/// <item>
///   <term><see cref="UTF8"/></term>
///   <description>Variable-width encoding. Handles all Unicode characters. Default and recommended for most use cases.</description>
/// </item>
/// <item>
///   <term><see cref="UTF16"/></term>
///   <description>Big-endian UTF-16. Used in some network protocols and Java interop scenarios.</description>
/// </item>
/// <item>
///   <term><see cref="UTF32"/></term>
///   <description>Fixed 4-byte encoding per character. Rarely used; avoids surrogate pairs.</description>
/// </item>
/// <item>
///   <term><see cref="Unicode"/></term>
///   <description>Little-endian UTF-16. Native encoding for Windows and .NET's internal string representation.</description>
/// </item>
/// <item>
///   <term><see cref="Latin1"/></term>
///   <description>ISO-8859-1. Single-byte encoding covering Western European characters. Common in legacy systems.</description>
/// </item>
/// </list>
/// </remarks>
public enum TextEncode
{
    /// <summary>
    /// 7-bit encoding. Only supports characters U+0000 to U+007F. Non-ASCII characters are lost.
    /// </summary>
    ASCII = 0,
    /// <summary>
    /// Variable-width encoding. Handles all Unicode characters. Default and recommended for most use cases.
    /// </summary>
    UTF8 = 1,
    /// <summary>
    /// Big-endian UTF-16. Used in some network protocols and Java interop scenarios.
    /// </summary>
    UTF16 = 2,
    /// <summary>
    /// Fixed 4-byte encoding per character. Rarely used; avoids surrogate pairs.
    /// </summary>
    UTF32 = 3,
    /// <summary>
    /// Little-endian UTF-16. Native encoding for Windows and .NET's internal string representation.
    /// </summary>
    Unicode = 4,
    /// <summary>
    /// ISO-8859-1. Single-byte encoding covering Western European characters. Common in legacy systems.
    /// </summary>
    Latin1 = 5
}