namespace Lazy.Extensions;

public static partial class StringExtensions
{
    #region Regex

    /// <summary>
    /// Matches any character that is not alphanumeric, a space, or a comma.
    /// Used internally to strip non-alphanumeric characters from strings.
    /// </summary>
    /// <remarks>
    /// Alphanumeric characters are defined as any Unicode letter
    /// (<c>\p{L}</c>) or number (<c>\p{N}</c>), plus spaces and commas.
    /// All other characters are considered non-alphanumeric and will be
    /// removed or replaced.
    /// </remarks>
    [GeneratedRegex(@"[^\p{L}\p{N} ,]")]
    private static partial Regex AlphanumericSymbolsRegex();

    #endregion Regex

    #region Symbols and Characters

    /// <summary>
    /// Removes diacritical marks (accents) from all characters in the string,
    /// replacing accented characters with their base equivalents.
    /// </summary>
    /// <remarks>
    /// Uses Unicode Normalization Form D (NFD) to decompose characters into their
    /// base form and combining marks, then discards the combining marks.
    /// For strings up to 512 characters, a stack-allocated buffer is used to avoid
    /// heap allocations. For longer strings, a rented array from
    /// <see cref="ArrayPool{T}"/> is used.
    /// </remarks>
    /// <param name="value">The string from which diacritics will be removed.</param>
    /// <returns>A new string with all diacritical marks removed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// "café".RemoveDiacritics();   // returns "cafe"
    /// "Ação".RemoveDiacritics();   // returns "Acao"
    /// "façade".RemoveDiacritics(); // returns "facade"
    /// </code>
    /// </example>
    public static string RemoveDiacritics(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return value;

        string normalizedString = value.Normalize(NormalizationForm.FormD);
        char[]? rentedArray = null;
        Span<char> destination = normalizedString.Length <= 512 ? stackalloc char[normalizedString.Length] : (rentedArray = ArrayPool<char>.Shared.Rent(normalizedString.Length));

        try
        {
            int index = 0;

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    destination[index++] = c;
            }

            return new string(destination[..index]);
        }
        finally
        {
            if (rentedArray is not null)
                ArrayPool<char>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Removes all non-alphanumeric characters from the string,
    /// optionally replacing them with a space instead of discarding them.
    /// </summary>
    /// <remarks>
    /// Alphanumeric characters are defined as any Unicode letter
    /// (<c>\p{L}</c>) or number (<c>\p{N}</c>), plus spaces and commas.
    /// All other characters are considered non-alphanumeric and will be
    /// removed or replaced.
    /// </remarks>
    /// <param name="value">The string to process.</param>
    /// <param name="useSpace">
    /// When <see langword="true"/>, non-alphanumeric characters are replaced with a space.
    /// When <see langword="false"/> (default), they are removed entirely.
    /// </param>
    /// <returns>
    /// A new string containing only alphanumeric characters,
    /// with non-alphanumeric characters removed or replaced by spaces.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// "hello!world".RemoveNonAlphanumeric();            // returns "helloworld"
    /// "hello!world".RemoveNonAlphanumeric(useSpace: true); // returns "hello world"
    /// </code>
    /// </example>
    public static string RemoveNonAlphanumeric(this string value, bool useSpace = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return value;

        return AlphanumericSymbolsRegex().Replace(value, useSpace ? " " : "");
    }

    /// <summary>
    /// Sanitizes the string by removing diacritical marks and non-alphanumeric
    /// characters, producing a normalized text representation.
    /// </summary>
    /// <remarks>
    /// This method combines <see cref="RemoveDiacritics"/> and
    /// <see cref="RemoveNonAlphanumeric"/>: first all diacritical marks are removed, then any remaining
    /// non-alphanumeric characters are removed or replaced.
    /// </remarks>
    /// <param name="value">The string to sanitize.</param>
    /// <param name="useSpace">
    /// When <see langword="true"/>, non-alphanumeric characters are replaced with a space.
    /// When <see langword="false"/> (default), they are removed entirely.
    /// </param>
    /// <returns>
    /// A sanitized string containing only Unicode letters, Unicode numbers, spaces and commas, 
    /// with diacritical marks removed and non-alphanumeric characters removed or replaced by spaces.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// "Héllo, wörld!".Sanitize();              // returns "Hello, world" 
    /// "café &amp; tê".Sanitize(useSpace: true);    // returns "cafe   te"
    /// "Ação!".Sanitize();                      // returns "Acao"
    /// </code>
    /// </example>
    public static string Sanitize(this string value, bool useSpace = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return value;

        return value.RemoveDiacritics().RemoveNonAlphanumeric(useSpace);
    }

    /// <summary>
    /// Trims all leading and trailing whitespace from the string using a
    /// <see cref="ReadOnlySpan{T}"/> slice, avoiding heap allocations when
    /// no whitespace is present.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike <see cref="string.Trim()"/>, this method short-circuits and returns
    /// the original <paramref name="value"/> reference when no whitespace is detected,
    /// resulting in zero heap allocations for already-trimmed strings.
    /// A new string is only allocated when whitespace is actually removed.
    /// </para>
    /// <para>
    /// <b>When to prefer this over <see cref="string.Trim()"/>:</b>
    /// use in hot paths or tight loops where most inputs are already trimmed and
    /// avoiding unnecessary allocations matters.
    /// For general use, <see cref="string.Trim()"/> is equivalent.
    /// </para>
    /// </remarks>
    /// <param name="value">The string to trim.</param>
    /// <returns>
    /// The original <paramref name="value"/> reference if no whitespace was removed;
    /// otherwise, a new trimmed string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "  hello  ".TrimSpan(); // returns "hello"   (new allocation)
    /// "hello".TrimSpan();     // returns "hello"   (same reference, zero allocation)
    /// "  ".TrimSpan();        // returns ""
    /// </code>
    /// </example>
    public static string TrimSpan(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        ReadOnlySpan<char> span = value.AsSpan();
        ReadOnlySpan<char> trimmed = span.Trim();

        if (span.Length == trimmed.Length)
            return value;

        return trimmed.ToString();
    }

    /// <summary>
    /// Trims all trailing whitespace from the string using a
    /// <see cref="ReadOnlySpan{T}"/> slice, avoiding heap allocations when
    /// no whitespace is present.
    /// </summary>
    /// <remarks>
    /// Returns the original <paramref name="value"/> reference when no trailing
    /// whitespace is detected, resulting in zero heap allocations for already-trimmed strings.
    /// A new string is only allocated when whitespace is actually removed.
    /// </remarks>
    /// <param name="value">The string to trim.</param>
    /// <returns>
    /// The original <paramref name="value"/> reference if no trailing whitespace was removed;
    /// otherwise, a new trimmed string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "hello  ".TrimEndSpan(); // returns "hello"  (new allocation)
    /// "hello".TrimEndSpan();   // returns "hello"  (same reference, zero allocation)
    /// "  hello  ".TrimEndSpan(); // returns "  hello"
    /// </code>
    /// </example>
    public static string TrimEndSpan(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        ReadOnlySpan<char> span = value.AsSpan();
        ReadOnlySpan<char> trimmed = span.TrimEnd();

        if (span.Length == trimmed.Length)
            return value;

        return trimmed.ToString();
    }

    /// <summary>
    /// Trims all leading whitespace from the string using a
    /// <see cref="ReadOnlySpan{T}"/> slice, avoiding heap allocations when
    /// no whitespace is present.
    /// </summary>
    /// <remarks>
    /// Returns the original <paramref name="value"/> reference when no leading
    /// whitespace is detected, resulting in zero heap allocations for already-trimmed strings.
    /// A new string is only allocated when whitespace is actually removed.
    /// </remarks>
    /// <param name="value">The string to trim.</param>
    /// <returns>
    /// The original <paramref name="value"/> reference if no leading whitespace was removed;
    /// otherwise, a new trimmed string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "  hello".TrimStartSpan(); // returns "hello"    (new allocation)
    /// "hello".TrimStartSpan();   // returns "hello"    (same reference, zero allocation)
    /// "  hello  ".TrimStartSpan(); // returns "hello  "
    /// </code>
    /// </example>
    public static string TrimStartSpan(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        ReadOnlySpan<char> span = value.AsSpan();
        ReadOnlySpan<char> trimmed = span.TrimStart();

        if (span.Length == trimmed.Length)
            return value;

        return trimmed.ToString();
    }

    /// <summary>
    /// Normalizes a string by trimming leading and trailing whitespace, collapsing consecutive whitespace characters into a single space,
    /// converting all characters to lowercase, and capitalizing the first letter of the string and the first letter after sentence-ending
    /// punctuation (<c>.</c>, <c>!</c>, <c>?</c>).
    /// </summary>
    /// <param name="text">The input string to normalize.</param>
    /// <returns>
    /// A normalized string with proper sentence casing, or
    /// <see cref="string.Empty"/> if <paramref name="text"/> is null,
    /// empty, or whitespace.
    /// </returns>
    /// <example>
    /// <code>
    /// "Bom Dia, como esta voCe? estoU Bem.".ToSentenceCase();  // returns "Bom dia, como esta voce? Estou bem."
    /// "OLÁ!  tUDO  BEM?  sim.".ToSentenceCase();               // returns "Olá! Tudo bem? Sim."
    /// "Olá\t\tmundo\r\n\r\ncomo vai?".ToSentenceCase();        // returns "Olá mundo como vai?"
    /// </code>
    /// </example>
    public static string ToSentenceCase(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Trim and collapse consecutive whitespace into a single space
        ReadOnlySpan<char> trimmed = text.AsSpan().Trim();
        char[]? rentedArray = null;
        Span<char> buffer = trimmed.Length <= 512 ? stackalloc char[trimmed.Length] : (rentedArray = ArrayPool<char>.Shared.Rent(trimmed.Length));

        try
        {
            int writeIndex = 0;
            bool capitalizeNext = true;
            bool lastWasWhitespace = false;

            foreach (char c in trimmed)
            {
                // Collapse consecutive whitespace into a single space
                if (char.IsWhiteSpace(c))
                {
                    if (lastWasWhitespace)
                        continue;

                    lastWasWhitespace = true;
                    buffer[writeIndex++] = ' ';
                    continue;
                }

                lastWasWhitespace = false;

                if (capitalizeNext && !char.IsPunctuation(c))
                {
                    buffer[writeIndex++] = char.ToUpperInvariant(c);

                    // Only stop capitalizing after encountering a letter
                    if (char.IsLetter(c))
                        capitalizeNext = false;

                    continue;
                }

                buffer[writeIndex++] = char.ToLowerInvariant(c);

                // Schedule capitalization after sentence-ending punctuation
                if (c is '.' or '!' or '?')
                    capitalizeNext = true;
            }

            Span<char> result = buffer[..writeIndex];

            // Ensure the first actual letter is capitalized
            for (int i = 0; i < result.Length; i++)
            {
                if (!char.IsLetter(result[i]))
                    continue;

                result[i] = char.ToUpperInvariant(result[i]);
                break;
            }

            return new string(result);
        }
        finally
        {
            if (rentedArray is not null)
                ArrayPool<char>.Shared.Return(rentedArray);
        }
    }

    #endregion Symbols and Characters

    #region Get and Contains

    /// <summary>
    /// Returns the substring from the beginning of the string up to (but not including)
    /// the first occurrence of <paramref name="stopAt"/>.
    /// </summary>
    /// <param name="text">The input string.</param>
    /// <param name="stopAt">The character to stop at.</param>
    /// <param name="orEmpty">
    /// When <see langword="true"/>, returns <see cref="string.Empty"/> if
    /// <paramref name="stopAt"/> is not found or if <paramref name="text"/>
    /// is null, empty, or whitespace.
    /// When <see langword="false"/>, returns the original string when the
    /// character is not found.
    /// </param>
    /// <returns>
    /// The substring before the first occurrence of <paramref name="stopAt"/>.
    /// Returns <see cref="string.Empty"/> when:
    /// <list type="bullet">
    /// <item><paramref name="text"/> is null, empty, or whitespace.</item>
    /// <item><paramref name="orEmpty"/> is <see langword="true"/> and the character is not found.</item>
    /// <item><paramref name="stopAt"/> occurs at index 0.</item>
    /// </list>
    /// Otherwise returns the original string when the character is not found.
    /// </returns>
    /// <example>
    /// <code>
    /// "hello.world".GetUntil('.');         // returns "hello"
    /// "/api/users".GetUntil('/');          // returns "" (stopAt is at index 0)
    /// "helloworld".GetUntil('.');          // returns "helloworld" (not found, orEmpty: false)
    /// "helloworld".GetUntil('.', orEmpty: true); // returns "" (not found, orEmpty: true)
    /// </code>
    /// </example>
    public static string GetUntil(this string text, char stopAt, bool orEmpty = false)
    {
        if (string.IsNullOrWhiteSpace(text))
            return orEmpty ? string.Empty : text ?? string.Empty;

        int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

        if (charLocation < 0)
            return orEmpty ? string.Empty : text;

        if (charLocation == 0)
            return string.Empty;

        return text[..charLocation];
    }

    /// <summary>
    /// Returns the substring after the first occurrence of <paramref name="startAt"/>,
    /// excluding the character itself.
    /// </summary>
    /// <param name="text">The input string.</param>
    /// <param name="startAt">The character to search for.</param>
    /// <param name="orEmpty">
    /// When <see langword="true"/>, returns <see cref="string.Empty"/> if
    /// <paramref name="startAt"/> is not found or if <paramref name="text"/>
    /// is null, empty, or whitespace.
    /// When <see langword="false"/>, returns the original string when the
    /// character is not found.
    /// </param>
    /// <returns>
    /// The substring after the first occurrence of <paramref name="startAt"/>.
    /// Returns <see cref="string.Empty"/> when:
    /// <list type="bullet">
    /// <item><paramref name="text"/> is null, empty, or whitespace.</item>
    /// <item><paramref name="orEmpty"/> is <see langword="true"/> and the character is not found.</item>
    /// <item>The character occurs as the last character in the string.</item>
    /// </list>
    /// Otherwise returns the original string when the character is not found.
    /// </returns>
    /// <example>
    /// <code>
    /// "/api/users".GetAfter('/');               // returns "api/users"
    /// "hello.world".GetAfter('.');             // returns "world"
    /// "helloworld".GetAfter('.');              // returns "helloworld" (not found, orEmpty: false)
    /// "helloworld".GetAfter('.', orEmpty: true); // returns "" (not found, orEmpty: true)
    /// "hello/".GetAfter('/');                  // returns "" (nothing after the char)
    /// </code>
    /// </example>
    public static string GetAfter(this string text, char startAt, bool orEmpty = false)
    {
        if (string.IsNullOrWhiteSpace(text))
            return orEmpty ? string.Empty : text ?? string.Empty;

        int charLocation = text.IndexOf(startAt, StringComparison.Ordinal);

        if (charLocation < 0)
            return orEmpty ? string.Empty : text;

        if (charLocation == text.Length - 1)
            return string.Empty;

        return text[(charLocation + 1)..];
    }

    /// <summary>
    /// Counts the number of non-overlapping occurrences of <paramref name="search"/>
    /// within the string.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="MemoryExtensions.IndexOf(ReadOnlySpan{char}, ReadOnlySpan{char}, StringComparison)"/>
    /// over a <see cref="ReadOnlySpan{char}"/> slice to avoid heap allocations during the search loop.
    /// </remarks>
    /// <param name="value">The string to search within.</param>
    /// <param name="search">The character to count.</param>
    /// <returns>
    /// The number of times <paramref name="search"/> appears in <paramref name="value"/>,
    /// or zero if <paramref name="value"/> is empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "banana".CountOccurrences('a');   // returns 3
    /// "hello".CountOccurrences('z');    // returns 0
    /// "aaa".CountOccurrences('a');      // returns 3
    /// </code>
    /// </example>
    public static int CountOccurrences(this string value, char search)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return 0;

        ArgumentNullException.ThrowIfNull(value);
        return value.AsSpan().Count(search);
    }

    /// <summary>
    /// Counts the number of non-overlapping occurrences of the string <paramref name="search"/>
    /// within the string.
    /// </summary>
    /// <remarks>
    /// Searches using <see cref="StringComparison.Ordinal"/> by default for performance.
    /// Uses <see cref="ReadOnlySpan{char}"/> slicing to advance through the string
    /// without allocating substrings.
    /// </remarks>
    /// <param name="value">The string to search within.</param>
    /// <param name="search">The substring to count.</param>
    /// <param name="comparison">
    /// The string comparison rule to use. Defaults to <see cref="StringComparison.Ordinal"/>.
    /// </param>
    /// <returns>
    /// The number of non-overlapping times <paramref name="search"/> appears in <paramref name="value"/>,
    /// or zero if <paramref name="value"/> is empty or <paramref name="search"/> is empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> or <paramref name="search"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "banana".CountOccurrences("an");                                         // returns 2
    /// "aaa".CountOccurrences("aa");                                            // returns 1 (non-overlapping)
    /// "hello".CountOccurrences("z");                                           // returns 0
    /// "Hello Hello".CountOccurrences("hello", StringComparison.OrdinalIgnoreCase); // returns 2
    /// </code>
    /// </example>
    public static int CountOccurrences(this string value, string search, StringComparison comparison = StringComparison.Ordinal)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(search);

        if (value.Length == 0 || search.Length == 0)
            return 0;

        int count = 0;
        ReadOnlySpan<char> remaining = value.AsSpan();

        while (true)
        {
            int index = remaining.IndexOf(search.AsSpan(), comparison);
            if (index < 0)
                break;

            count++;
            remaining = remaining[(index + search.Length)..];
        }

        return count;
    }

    /// <summary>
    /// Extracts the substring between the first occurrence of <paramref name="open"/>
    /// and the next occurrence of <paramref name="close"/> after it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The search is performed using <see cref="StringComparison.Ordinal"/>.
    /// Neither delimiter is included in the returned substring.
    /// </para>
    /// <para>
    /// Returns <see cref="string.Empty"/> if either delimiter is not found,
    /// or if there is no content between them.
    /// </para>
    /// </remarks>
    /// <param name="value">The string to search within.</param>
    /// <param name="open">The opening delimiter character.</param>
    /// <param name="close">The closing delimiter character.</param>
    /// <returns>
    /// The substring between <paramref name="open"/> and <paramref name="close"/>,
    /// or <see cref="string.Empty"/> if either delimiter is missing or there is no content between them.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "{token}".GetBetween('{', '}');          // returns "token"
    /// "&lt;tag&gt;content&lt;/tag&gt;".GetBetween('&lt;', '&gt;'); // returns "tag"
    /// "(hello (world))".GetBetween('(', ')'); // returns "hello (world" (first pair only)
    /// "no-delimiters".GetBetween('[', ']');   // returns ""
    /// </code>
    /// </example>
    public static string GetBetween(this string value, char open, char close)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return string.Empty;

        ReadOnlySpan<char> span = value.AsSpan();

        int openIndex = span.IndexOf(open);

        if (openIndex < 0 || openIndex + 1 >= span.Length)
            return string.Empty;

        ReadOnlySpan<char> afterOpen = span[(openIndex + 1)..];
        int closeIndex = afterOpen.IndexOf(close);

        if (closeIndex <= 0)
            return string.Empty;

        return new string(afterOpen[..closeIndex]);
    }

    /// <summary>
    /// Extracts the substring between the first occurrence of the string <paramref name="open"/>
    /// and the next occurrence of the string <paramref name="close"/> after it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Neither delimiter is included in the returned substring.
    /// The comparison is performed using <see cref="StringComparison.Ordinal"/> by default.
    /// </para>
    /// <para>
    /// Returns <see cref="string.Empty"/> if either delimiter is not found,
    /// or if there is no content between them.
    /// </para>
    /// </remarks>
    /// <param name="value">The string to search within.</param>
    /// <param name="open">The opening delimiter string.</param>
    /// <param name="close">The closing delimiter string.</param>
    /// <param name="comparison">
    /// The string comparison rule to use. Defaults to <see cref="StringComparison.Ordinal"/>.
    /// </param>
    /// <returns>
    /// The substring between <paramref name="open"/> and <paramref name="close"/>,
    /// or <see cref="string.Empty"/> if either delimiter is missing or there is no content between them.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/>, <paramref name="open"/>, or <paramref name="close"/>
    /// is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "Hello [World]!".GetBetween("[", "]");               // returns "World"
    /// "&lt;div&gt;content&lt;/div&gt;".GetBetween("&lt;div&gt;", "&lt;/div&gt;");  // returns "content"
    /// "Hello [World]!".GetBetween("[", "]", StringComparison.OrdinalIgnoreCase); // returns "World"
    /// "no-delimiters".GetBetween("{", "}");                // returns ""
    /// </code>
    /// </example>
    public static string GetBetween(this string value, string open, string close, StringComparison comparison = StringComparison.Ordinal)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(open);
        ArgumentNullException.ThrowIfNull(close);

        if (value.Length == 0 || open.Length == 0 || close.Length == 0)
            return string.Empty;

        ReadOnlySpan<char> span = value.AsSpan();

        int openIndex = span.IndexOf(open.AsSpan(), comparison);

        if (openIndex < 0)
            return string.Empty;

        ReadOnlySpan<char> afterOpen = span[(openIndex + open.Length)..];

        if (afterOpen.Length == 0)
            return string.Empty;

        int closeIndex = afterOpen.IndexOf(close.AsSpan(), comparison);

        if (closeIndex <= 0)
            return string.Empty;

        return new string(afterOpen[..closeIndex]);
    }

    /// <summary>
    /// Determines whether the string contains any character present in <paramref name="invalidChars"/>.
    /// </summary>
    /// <param name="text">The string to check.</param>
    /// <param name="invalidChars">
    /// A string whose individual characters are treated as the set of invalid characters to search for.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="text"/> contains at least one character
    /// from <paramref name="invalidChars"/>; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="text"/> or <paramref name="invalidChars"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "hello!".ContainsAny("!@#");   // returns true
    /// "!hello".ContainsAny("!@#");   // returns true  (position 0, previously bugged)
    /// "hXello".ContainsAny("!@#");   // returns false
    /// "".ContainsAny("!@#");         // returns false
    /// </code>
    /// </example>
    public static bool ContainsAny(this string text, string invalidChars)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(invalidChars);

        if (text.Length == 0 || invalidChars.Length == 0)
            return false;

        return text.AsSpan().IndexOfAny(invalidChars.AsSpan()) >= 0;
    }

    // <summary>
    /// Determines whether the string contains <paramref name="search"/>
    /// using a case-insensitive, culture-invariant comparison.
    /// </summary>
    /// <remarks>
    /// Equivalent to calling
    /// <c>value.Contains(search, StringComparison.OrdinalIgnoreCase)</c>,
    /// exposed as a named extension for readability in conditional expressions.
    /// </remarks>
    /// <param name="value">The string to search within.</param>
    /// <param name="search">The substring to look for.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="search"/> is found within
    /// <paramref name="value"/> regardless of casing; otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> or <paramref name="search"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "Hello World".ContainsIgnoreCase("hello");  // true
    /// "Hello World".ContainsIgnoreCase("WORLD");  // true
    /// "Hello World".ContainsIgnoreCase("xyz");    // false
    /// </code>
    /// </example>
    public static bool ContainsIgnoreCase(this string value, string search)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(search);

        return value.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the string starts with any of the provided prefixes.
    /// </summary>
    /// <remarks>
    /// Comparison is performed using <see cref="StringComparison.Ordinal"/> by default.
    /// Evaluation stops at the first matching prefix (short-circuit).
    /// </remarks>
    /// <param name="value">The string to evaluate.</param>
    /// <param name="prefixes">The prefixes to check against.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> starts with at least one
    /// of <paramref name="prefixes"/>; otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> or <paramref name="prefixes"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "https://example.com".StartsWithAny("http://", "https://"); // true
    /// "ftp://example.com".StartsWithAny("http://", "https://");   // false
    /// "Hello".StartsWithAny("He", "Hi", "Ho");                   // true
    /// </code>
    /// </example>
    public static bool StartsWithAny(this string value, params string[] prefixes)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefixes);

        foreach (string prefix in prefixes)
        {
            if (prefix is not null && value.StartsWith(prefix, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the string starts with any of the provided prefixes
    /// using the specified string comparison.
    /// </summary>
    /// <param name="value">The string to evaluate.</param>
    /// <param name="comparison">The comparison rule to apply to each prefix check.</param>
    /// <param name="prefixes">The prefixes to check against.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> starts with at least one
    /// of <paramref name="prefixes"/>; otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> or <paramref name="prefixes"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "Hello".StartsWithAny(StringComparison.OrdinalIgnoreCase, "he", "hi"); // true
    /// </code>
    /// </example>
    public static bool StartsWithAny(this string value, StringComparison comparison, params string[] prefixes)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefixes);

        foreach (string prefix in prefixes)
        {
            if (prefix is not null && value.StartsWith(prefix, comparison))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the string ends with any of the provided suffixes.
    /// </summary>
    /// <remarks>
    /// Comparison is performed using <see cref="StringComparison.Ordinal"/> by default.
    /// Evaluation stops at the first matching suffix (short-circuit).
    /// </remarks>
    /// <param name="value">The string to evaluate.</param>
    /// <param name="suffixes">The suffixes to check against.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> ends with at least one
    /// of <paramref name="suffixes"/>; otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> or <paramref name="suffixes"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "readme.md".EndsWithAny(".md", ".txt", ".rst"); // true
    /// "image.png".EndsWithAny(".jpg", ".gif");        // false
    /// "HELLO.MD".EndsWithAny(".md", ".txt");          // false (case-sensitive)
    /// </code>
    /// </example>
    public static bool EndsWithAny(this string value, params string[] suffixes)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffixes);

        foreach (string suffix in suffixes)
        {
            if (suffix is not null && value.EndsWith(suffix, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the string ends with any of the provided suffixes
    /// using the specified string comparison.
    /// </summary>
    /// <param name="value">The string to evaluate.</param>
    /// <param name="comparison">The comparison rule to apply to each suffix check.</param>
    /// <param name="suffixes">The suffixes to check against.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> ends with at least one
    /// of <paramref name="suffixes"/>; otherwise <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> or <paramref name="suffixes"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "HELLO.MD".EndsWithAny(StringComparison.OrdinalIgnoreCase, ".md", ".txt"); // true
    /// </code>
    /// </example>
    public static bool EndsWithAny(this string value, StringComparison comparison, params string[] suffixes)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffixes);

        foreach (string suffix in suffixes)
        {
            if (suffix is not null && value.EndsWith(suffix, comparison))
                return true;
        }

        return false;
    }

    #endregion Get and Contains

    #region Comparisons

    /// <summary>
    /// Determines whether two strings are content-equivalent by performing a highly normalized, 
    /// culture-invariant, and case-insensitive comparison.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is specifically designed to reconcile common data-entry discrepancies, such as those 
    /// introduced when text is copied from external software (e.g., Microsoft Excel). It normalizes 
    /// both strings by stripping leading/trailing whitespace, removing diacritics (accents), filtering out 
    /// non-alphanumeric characters, and evaluating the result without linguistic or casing variance.
    /// </para>
    /// <para>
    /// <b>Pipeline Execution Order:</b>
    /// <list type="number">
    /// <item><description>Trims external whitespace using memory-efficient Span operations (<see cref="TrimSpan"/>).</description></item>
    /// <item><description>Sanitizes the text to remove accents and non-alphanumeric symbols (<see cref="Sanitize(string, bool)"/>).</description></item>
    /// <item><description>Compares the final outputs using <see cref="StringComparison.InvariantCultureIgnoreCase"/>.</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <param name="text">The source string to compare.</param>
    /// <param name="other">The target string to compare against.</param>
    /// <returns>
    /// <see langword="true"/> if both strings match after undergoing the full normalization pipeline; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="text"/> or <paramref name="other"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "Café ".MatchWith("cafe");          // Returns true  (Trimmed + Diacritics removed + Case-insensitive)
    /// "  Hello! ".MatchWith("hello");     // Returns true  (Trimmed + Punctuation stripped)
    /// "São Paulo".MatchWith("sao paulo"); // Returns true  (Diacritics removed + Case-insensitive)
    /// "abc".MatchWith("xyz");             // Returns false
    /// </code>
    /// </example>
    public static bool MatchWith(this string text, string other)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(other);

        string a = text.TrimSpan().Sanitize();
        string b = other.TrimSpan().Sanitize();

        return a.Equals(b, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Indicates whether the string is <see langword="null"/>, empty, or consists only of whitespace characters.
    /// This is an extension method alias for <see cref="string.IsNullOrWhiteSpace(string)"/>.
    /// </summary>
    /// <param name="text">The string to test.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="text"/> is <see langword="null"/>, empty,
    /// or whitespace only; otherwise <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// The <paramref name="text"/> parameter is intentionally annotated as nullable (<see langword="null"/>-safe),
    /// as the primary purpose of this method is to handle <see langword="null"/> inputs gracefully.
    /// The <c>[AllowNull]</c> attribute is used to suppress the compiler warning CS8625 that would
    /// otherwise be emitted when calling this extension method on a <see langword="null"/> literal,
    /// while keeping the parameter type as non-nullable <see cref="string"/> to match the signature
    /// of <see cref="string.IsNullOrWhiteSpace(string)"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// "".IsNullOrBlank();    // true
    /// "   ".IsNullOrBlank(); // true
    /// null.IsNullOrBlank();  // true
    /// "hi".IsNullOrBlank();  // false
    /// </code>
    /// </example>
    public static bool IsNullOrBlank(this string? text) => string.IsNullOrWhiteSpace(text); 

    /// <summary>
    /// Analyzes the memory segment to determine whether every single character in the sequence is identical.
    /// </summary>
    /// <remarks>
    /// This helper is used as a fast-failing optimization checkpoint to filter out invalid sequential patterns like all zeros.
    /// It operates by looping from index one through the rest of the span, matching each character against the value at index zero.
    /// </remarks>
    /// <param name="span">The continuous read-only character segment extracted from the target document string.</param>
    /// <returns>True if the sequence completes without detecting any variance, indicating an invalid uniform repeating block; otherwise, false.</returns>
    public static bool IsAllSameDigits(this ReadOnlySpan<char> span)
    {
        for (int i = 1; i < span.Length; i++)
        {
            if (span[i] != span[0])
                return false;
        }
        return true;
    }

    #endregion Comparisons

    #region Modifiers

    /// <summary>
    /// Truncates the string to a maximum length, optionally appending a suffix
    /// (such as an ellipsis) to indicate that content was cut.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <paramref name="maxLength"/> includes the suffix length. For example,
    /// with <c>maxLength: 10</c> and <c>suffix: "..."</c>, the base content is
    /// limited to 7 characters, and the suffix occupies the remaining 3.
    /// </para>
    /// <para>
    /// If the suffix alone exceeds <paramref name="maxLength"/>, the suffix itself
    /// is truncated to fit within <paramref name="maxLength"/> characters.
    /// </para>
    /// </remarks>
    /// <param name="value">The string to truncate.</param>
    /// <param name="maxLength">
    /// The maximum length of the returned string, including the suffix.
    /// Must be greater than zero.
    /// </param>
    /// <param name="suffix">
    /// An optional string appended when truncation occurs (e.g., <c>"..."</c>).
    /// Defaults to <see cref="string.Empty"/>.
    /// </param>
    /// <returns>
    /// The original string if its length does not exceed <paramref name="maxLength"/>;
    /// otherwise, a truncated string of exactly <paramref name="maxLength"/> characters
    /// (including the suffix).
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="maxLength"/> is less than or equal to zero.
    /// </exception>
    /// <example>
    /// <code>
    /// "Hello, world!".Truncate(5);          // returns "Hello"
    /// "Hello, world!".Truncate(8, "...");   // returns "Hello..."
    /// "Hi".Truncate(10);                    // returns "Hi"        (no truncation)
    /// "Hello".Truncate(2, "...");           // returns ".."        (suffix truncated to fit)
    /// </code>
    /// </example>
    public static string Truncate(this string value, int maxLength, string suffix = "")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxLength);

        if (value.Length <= maxLength)
            return value;

        int subLength = maxLength - suffix.Length;
        if (subLength <= 0)
            return suffix[..maxLength];

        return string.Concat(value.AsSpan(0, subLength), suffix);
    }

    /// <summary>
    /// Converts the string into a URL-friendly slug by removing diacritics,
    /// replacing whitespace and non-alphanumeric characters with hyphens,
    /// collapsing consecutive hyphens, and lowercasing the result.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Pipeline execution order:</b>
    /// <list type="number">
    /// <item><description>Trim leading and trailing whitespace.</description></item>
    /// <item><description>Remove diacritical marks via <see cref="RemoveDiacritics"/>.</description></item>
    /// <item><description>Lowercase all characters via <see cref="char.ToLowerInvariant"/>.</description></item>
    /// <item><description>Replace any non-alphanumeric character with a hyphen.</description></item>
    /// <item><description>Collapse consecutive hyphens into a single hyphen.</description></item>
    /// <item><description>Trim leading and trailing hyphens from the result.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Uses <see cref="ArrayPool{T}"/> for strings longer than 512 characters
    /// to avoid unnecessary heap allocations.
    /// </para>
    /// </remarks>
    /// <param name="value">The string to slugify.</param>
    /// <returns>
    /// A lowercase, hyphen-separated slug safe for use in URLs and file names,
    /// or <see cref="string.Empty"/> if <paramref name="value"/> is empty or whitespace.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "São Paulo, Brasil!".Slugify(); // returns "sao-paulo-brasil"
    /// "  Hello   World  ".Slugify(); // returns "hello-world"
    /// "C# is great!".Slugify();      // returns "c-is-great"
    /// "---test---".Slugify();        // returns "test"
    /// </code>
    /// </example>
    public static string Slugify(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return value;

        string withoutDiacritics = value.TrimSpan().RemoveDiacritics();

        if (withoutDiacritics.Length == 0)
            return string.Empty;

        char[]? rentedArray = null;
        Span<char> buffer = withoutDiacritics.Length <= 512 ? stackalloc char[withoutDiacritics.Length] : (rentedArray = ArrayPool<char>.Shared.Rent(withoutDiacritics.Length));

        try
        {
            int writeIndex = 0;
            bool lastWasHyphen = false;

            foreach (char c in withoutDiacritics)
            {
                if (char.IsLetterOrDigit(c))
                {
                    buffer[writeIndex++] = char.ToLowerInvariant(c);
                    lastWasHyphen = false;
                }
                else if (!lastWasHyphen)
                {
                    buffer[writeIndex++] = '-';
                    lastWasHyphen = true;
                }
            }

            // Trim leading and trailing hyphens from result
            int start = 0;
            int end = writeIndex;

            while (start < end && buffer[start] == '-')
                start++;

            while (end > start && buffer[end - 1] == '-')
                end--;

            return new string(buffer[start..end]);
        }
        finally
        {
            if (rentedArray is not null)
                ArrayPool<char>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Returns a new string in which the current string is repeated
    /// <paramref name="count"/> times.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="string.Create{TState}"/> internally to allocate
    /// the result in a single operation, avoiding intermediate string allocations.
    /// </remarks>
    /// <param name="value">The string to repeat.</param>
    /// <param name="count">
    /// The number of times to repeat <paramref name="value"/>.
    /// Must be greater than or equal to zero.
    /// </param>
    /// <returns>
    /// A new string consisting of <paramref name="value"/> repeated
    /// <paramref name="count"/> times, or <see cref="string.Empty"/> if
    /// <paramref name="value"/> is empty or <paramref name="count"/> is zero.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="count"/> is negative.
    /// </exception>
    /// <example>
    /// <code>
    /// "*".Repeat(5);    // returns "*****"
    /// "ab".Repeat(3);   // returns "ababab"
    /// "x".Repeat(0);    // returns ""
    /// "-".Repeat(20);   // returns "--------------------"
    /// </code>
    /// </example>
    public static string Repeat(this string value, int count)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if (count == 0 || value.Length == 0)
            return string.Empty;

        if (count == 1)
            return value;

        return string.Create(value.Length * count, (value, count), static (span, state) =>
        {
            (string val, int cnt) = state;
            int position = 0;

            for (int i = 0; i < cnt; i++)
            {
                val.AsSpan().CopyTo(span[position..]);
                position += val.Length;
            }
        });
    }

    /// <summary>
    /// Centers the string within a field of <paramref name="totalWidth"/> characters,
    /// padding both sides evenly with <paramref name="paddingChar"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When the required padding is odd, the extra character is added to the right side.
    /// </para>
    /// <para>
    /// If <paramref name="value"/> is longer than <paramref name="totalWidth"/>,
    /// the original string is returned unchanged — no truncation occurs.
    /// </para>
    /// </remarks>
    /// <param name="value">The string to center.</param>
    /// <param name="totalWidth">
    /// The total length of the resulting string including padding.
    /// If less than or equal to the length of <paramref name="value"/>,
    /// the original string is returned.
    /// </param>
    /// <param name="paddingChar">
    /// The character used to pad both sides. Defaults to a space (<c>' '</c>).
    /// </param>
    /// <returns>
    /// A new string of length <paramref name="totalWidth"/> with
    /// <paramref name="value"/> centered and padded on both sides,
    /// or the original string if no padding is needed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="totalWidth"/> is negative.
    /// </exception>
    /// <example>
    /// <code>
    /// "hi".PadCenter(10);        // returns "    hi    "
    /// "hi".PadCenter(10, '-');   // returns "----hi----"
    /// "hi".PadCenter(11, '-');   // returns "----hi-----"  (extra on the right)
    /// "toolong".PadCenter(3);    // returns "toolong"       (no truncation)
    /// </code>
    /// </example>
    public static string PadCenter(this string value, int totalWidth, char paddingChar = ' ')
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(totalWidth);

        int padding = totalWidth - value.Length;

        if (padding <= 0)
            return value;

        int padLeft = padding / 2;
        int padRight = padding - padLeft;

        return string.Create(totalWidth, (value, padLeft, padRight, paddingChar), static (span, state) =>
        {
            (string val, int left, int right, char pad) = state;

            span[..left].Fill(pad);
            val.AsSpan().CopyTo(span[left..]);
            span[(left + val.Length)..].Fill(pad);
        });
    }

    /// <summary>
    /// Masks a portion of the string by replacing characters with <paramref name="maskChar"/>,
    /// preserving a specified number of characters at the start, end, or both.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Useful for partially obscuring sensitive data such as CPF, credit card numbers,
    /// phone numbers, or tokens in logs and UI displays.
    /// </para>
    /// <para>
    /// If the sum of <paramref name="visibleStart"/> and <paramref name="visibleEnd"/>
    /// is greater than or equal to the length of <paramref name="value"/>,
    /// the original string is returned unchanged — no masking is applied.
    /// </para>
    /// </remarks>
    /// <param name="value">The string to mask.</param>
    /// <param name="visibleStart">
    /// The number of characters to keep visible at the beginning of the string.
    /// Defaults to <c>0</c>.
    /// </param>
    /// <param name="visibleEnd">
    /// The number of characters to keep visible at the end of the string.
    /// Defaults to <c>4</c>.
    /// </param>
    /// <param name="maskChar">
    /// The character used to replace masked characters. Defaults to <c>'*'</c>.
    /// </param>
    /// <returns>
    /// A new string with the middle portion replaced by <paramref name="maskChar"/>,
    /// or the original string if there is nothing to mask.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="visibleStart"/> or <paramref name="visibleEnd"/> is negative.
    /// </exception>
    /// <example>
    /// <code>
    /// "1234567890".Mask();                          // returns "******7890"
    /// "1234567890".Mask(visibleStart: 2);           // returns "12****7890"
    /// "1234567890".Mask(visibleStart: 2, visibleEnd: 2); // returns "12******90"
    /// "1234567890".Mask(visibleEnd: 0);             // returns "**********"
    /// "1234".Mask();                                // returns "1234"  (nothing to mask)
    /// "secret-token".Mask(maskChar: '#');           // returns "########oken"
    /// </code>
    /// </example>
    public static string Mask(this string value, int visibleStart = 0, int visibleEnd = 4, char maskChar = '*')
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(visibleStart);
        ArgumentOutOfRangeException.ThrowIfNegative(visibleEnd);

        if (value.Length == 0)
            return value;

        int maskLength = value.Length - visibleStart - visibleEnd;

        if (maskLength <= 0)
            return value;

        return string.Create(value.Length, (value, visibleStart, visibleEnd, maskChar, maskLength), static (span, state) =>
        {
            (string val, int start, int end, char mask, int masked) = state;

            val.AsSpan(0, start).CopyTo(span);
            span.Slice(start, masked).Fill(mask);
            val.AsSpan(val.Length - end).CopyTo(span[(start + masked)..]);
        });
    }

    /// <summary>
    /// Masks an email address by partially obscuring the local part (before the <c>@</c>)
    /// while keeping the domain fully visible.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The masking strategy preserves the first two characters of the local part
    /// and replaces the remainder with <paramref name="maskChar"/>, keeping the
    /// domain (including <c>@</c>) fully visible.
    /// </para>
    /// <para>
    /// If the local part has two or fewer characters, it is returned as-is without masking.
    /// No email format validation is performed — if <c>@</c> is not found,
    /// the method falls back to a full <see cref="Mask"/> of the entire string.
    /// </para>
    /// </remarks>
    /// <param name="value">The email address string to mask.</param>
    /// <param name="maskChar">
    /// The character used to replace masked characters. Defaults to <c>'*'</c>.
    /// </param>
    /// <returns>
    /// A masked email string with the local part partially obscured,
    /// or a fully masked string if no <c>@</c> is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "user@example.com".MaskEmail();          // returns "us**@example.com"
    /// "ab@example.com".MaskEmail();            // returns "ab@example.com"   (local too short)
    /// "a@example.com".MaskEmail();             // returns "a@example.com"    (local too short)
    /// "john.doe@company.org".MaskEmail();      // returns "jo******@company.org"
    /// "user@example.com".MaskEmail('#');       // returns "us##@example.com"
    /// "not-an-email".MaskEmail();             // returns "********ail"       (fallback)
    /// </code>
    /// </example>
    public static string MaskEmail(this string value, char maskChar = '*')
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return value;

        int atIndex = value.IndexOf('@', StringComparison.Ordinal);

        if (atIndex < 0)
            return value.Mask(maskChar: maskChar);

        ReadOnlySpan<char> local = value.AsSpan(0, atIndex);
        ReadOnlySpan<char> domain = value.AsSpan(atIndex);

        const int visibleLocalChars = 2;

        if (local.Length <= visibleLocalChars)
            return value;

        int maskedLength = local.Length - visibleLocalChars;

        return string.Create(value.Length, (value, atIndex, visibleLocalChars, maskedLength, maskChar), static (span, state) =>
        {
            (string val, int at, int visible, int masked, char mask) = state;

            val.AsSpan(0, visible).CopyTo(span);
            span.Slice(visible, masked).Fill(mask);
            val.AsSpan(at).CopyTo(span[(visible + masked)..]);
        });
    }

    #endregion Modifiers
}