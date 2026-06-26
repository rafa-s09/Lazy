namespace Lazy.Extensions;

public static class Conversions
{
    #region Enum

    /// <summary>
    /// Converts an <see cref="int"/> to the corresponding value of <typeparamref name="TEnum"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, throws <see cref="ArgumentException"/> if the integer has no defined
    /// counterpart in <typeparamref name="TEnum"/>.
    /// </para>
    /// <para>
    /// When <paramref name="fallbackToFirst"/> is <see langword="true"/>, invalid values
    /// silently return the first defined member of <typeparamref name="TEnum"/> instead
    /// of throwing. If the enum has no members, an <see cref="InvalidOperationException"/>
    /// is thrown regardless.
    /// </para>
    /// </remarks>
    /// <typeparam name="TEnum">The target enum type.</typeparam>
    /// <param name="value">The integer value to convert.</param>
    /// <param name="fallbackToFirst">
    /// When <see langword="true"/>, returns the first defined enum member if
    /// <paramref name="value"/> is not a valid enum value.
    /// When <see langword="false"/> (default), throws <see cref="ArgumentException"/> instead.
    /// </param>
    /// <returns>The <typeparamref name="TEnum"/> member corresponding to <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="value"/> is not defined in <typeparamref name="TEnum"/>
    /// and <paramref name="fallbackToFirst"/> is <see langword="false"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="fallbackToFirst"/> is <see langword="true"/> but
    /// <typeparamref name="TEnum"/> has no defined members.
    /// </exception>
    /// <example>
    /// <code>
    /// public enum Status { Active = 1, Inactive = 2 }
    ///
    /// 1.ToEnum&lt;Status&gt;();                        // returns Status.Active
    /// 2.ToEnum&lt;Status&gt;();                        // returns Status.Inactive
    /// 99.ToEnum&lt;Status&gt;(fallbackToFirst: true);  // returns Status.Active
    /// 99.ToEnum&lt;Status&gt;();                       // throws ArgumentException
    /// </code>
    /// </example>
    public static TEnum ToEnum<TEnum>(this int value, bool fallbackToFirst = false) where TEnum : struct, Enum
    {
        TEnum candidate = (TEnum)(object)value;
        if (Enum.IsDefined(candidate))
            return candidate;

        if (fallbackToFirst)
        {
            TEnum[] values = Enum.GetValues<TEnum>();

            if (values.Length == 0)
                throw new InvalidOperationException($"Cannot fall back to first value: enum {typeof(TEnum).FullName} has no defined members.");

            return values[0];
        }

        throw new ArgumentException($"The value {value} is not defined in enum {typeof(TEnum).FullName}.");
    }

    /// <summary>
    /// Converts a <see cref="byte"/> to the corresponding value of <typeparamref name="TEnum"/>,
    /// optionally returning the first defined member as a fallback for invalid values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>struct</c> constraint on <typeparamref name="TEnum"/> enables the use of
    /// <see cref="Enum.IsDefined{TEnum}(TEnum)"/> and <see cref="Enum.GetValues{TEnum}"/>,
    /// both of which avoid boxing and are more performant than their non-generic counterparts.
    /// </para>
    /// <para>
    /// When <paramref name="fallbackToFirst"/> is <see langword="true"/>, invalid values
    /// silently return the first defined member of <typeparamref name="TEnum"/> instead
    /// of throwing. If the enum has no members, an <see cref="InvalidOperationException"/>
    /// is thrown regardless.
    /// </para>
    /// </remarks>
    /// <typeparam name="TEnum">The target enum type.</typeparam>
    /// <param name="value">The byte value to convert.</param>
    /// <param name="fallbackToFirst">
    /// When <see langword="true"/>, returns the first defined enum member if
    /// <paramref name="value"/> is not a valid enum value.
    /// When <see langword="false"/> (default), throws <see cref="ArgumentException"/> instead.
    /// </param>
    /// <returns>The <typeparamref name="TEnum"/> member corresponding to <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="value"/> is not defined in <typeparamref name="TEnum"/>
    /// and <paramref name="fallbackToFirst"/> is <see langword="false"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="fallbackToFirst"/> is <see langword="true"/> but
    /// <typeparamref name="TEnum"/> has no defined members.
    /// </exception>
    /// <example>
    /// <code>
    /// public enum Status : byte { Active = 1, Inactive = 2 }
    ///
    /// ((byte)1).ToEnum&lt;Status&gt;();                       // returns Status.Active
    /// ((byte)99).ToEnum&lt;Status&gt;(fallbackToFirst: true); // returns Status.Active
    /// ((byte)99).ToEnum&lt;Status&gt;();                      // throws ArgumentException
    /// </code>
    /// </example>
    public static TEnum ToEnum<TEnum>(this byte value, bool fallbackToFirst = false) where TEnum : struct, Enum
    {
        TEnum candidate = (TEnum)(object)value;
        if (Enum.IsDefined(candidate))
            return candidate;

        if (fallbackToFirst)
        {
            TEnum[] values = Enum.GetValues<TEnum>();

            if (values.Length == 0)
                throw new InvalidOperationException($"Cannot fall back to first value: enum {typeof(TEnum).FullName} has no defined members.");

            return values[0];
        }

        throw new ArgumentException($"The value {value} is not defined in enum {typeof(TEnum).FullName}.");
    }

    /// <summary>
    /// Converts an <see cref="int"/> to the corresponding value of <typeparamref name="TEnum"/>,
    /// returning a specified fallback value if the conversion is not valid.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike <see cref="ToEnum{TEnum}(int, bool)"/>, this overload allows a specific
    /// fallback member to be provided instead of defaulting to the first defined member,
    /// giving the caller explicit control over the fallback behavior.
    /// </para>
    /// <para>
    /// The <c>struct</c> constraint on <typeparamref name="TEnum"/> enables the use of
    /// <see cref="Enum.IsDefined{TEnum}(TEnum)"/>, avoiding boxing.
    /// </para>
    /// </remarks>
    /// <typeparam name="TEnum">The target enum type.</typeparam>
    /// <param name="value">The integer value to convert.</param>
    /// <param name="fallback">
    /// The <typeparamref name="TEnum"/> value to return when <paramref name="value"/>
    /// is not defined in the enum.
    /// </param>
    /// <returns>
    /// The <typeparamref name="TEnum"/> member corresponding to <paramref name="value"/>
    /// if defined; otherwise, <paramref name="fallback"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// public enum Status { Active = 1, Inactive = 2, Pending = 3 }
    ///
    /// 1.ToEnum(Status.Pending);   // returns Status.Active
    /// 99.ToEnum(Status.Pending);  // returns Status.Pending
    /// </code>
    /// </example>
    public static TEnum ToEnum<TEnum>(this int value, TEnum fallback) where TEnum : struct, Enum
    {
        TEnum candidate = (TEnum)(object)value;
        return Enum.IsDefined(candidate) ? candidate : fallback;
    }

    /// <summary>
    /// Converts a <see cref="byte"/> to the corresponding value of <typeparamref name="TEnum"/>,
    /// returning a specified fallback value if the conversion is not valid.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="ToEnum{TEnum}(byte, bool)"/>, this overload allows a specific
    /// fallback member to be provided instead of defaulting to the first defined member,
    /// giving the caller explicit control over the fallback behavior.
    /// </remarks>
    /// <typeparam name="TEnum">The target enum type.</typeparam>
    /// <param name="value">The byte value to convert.</param>
    /// <param name="fallback">
    /// The <typeparamref name="TEnum"/> value to return when <paramref name="value"/>
    /// is not defined in the enum.
    /// </param>
    /// <returns>
    /// The <typeparamref name="TEnum"/> member corresponding to <paramref name="value"/>
    /// if defined; otherwise, <paramref name="fallback"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// public enum Status : byte { Active = 1, Inactive = 2, Pending = 3 }
    ///
    /// ((byte)1).ToEnum(Status.Pending);   // returns Status.Active
    /// ((byte)99).ToEnum(Status.Pending);  // returns Status.Pending
    /// </code>
    /// </example>
    public static TEnum ToEnum<TEnum>(this byte value, TEnum fallback) where TEnum : struct, Enum
    {
        TEnum candidate = (TEnum)(object)value;
        return Enum.IsDefined(candidate) ? candidate : fallback;
    }   

    #endregion Enum

    #region Json

    /// <summary>
    /// Deserializes a JSON string into an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="JsonSerializer.Deserialize{T}(string, JsonSerializerOptions?)"/> internally.
    /// Returns <see langword="null"/> if the JSON represents a null value.
    /// For custom serialization behavior (e.g., camelCase, converters), use the
    /// <paramref name="options"/> overload.
    /// </remarks>
    /// <typeparam name="T">The target type to deserialize into.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="options">
    /// Optional <see cref="JsonSerializerOptions"/> to control deserialization behavior.
    /// When <see langword="null"/>, default options are used.
    /// </param>
    /// <returns>
    /// An instance of <typeparamref name="T"/>, or <see langword="null"/> if the JSON
    /// represents a null value or deserialization yields no result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="json"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="JsonException">
    /// Thrown when <paramref name="json"/> is not valid JSON or cannot be deserialized
    /// into <typeparamref name="T"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// record Person(string Name, int Age);
    ///
    /// """{"Name":"Alice","Age":30}""".FromJson&lt;Person&gt;();
    /// // returns Person { Name = "Alice", Age = 30 }
    ///
    /// "null".FromJson&lt;Person&gt;();
    /// // returns null
    /// </code>
    /// </example>
    public static T? FromJson<T>(this string json, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(json);

        return JsonSerializer.Deserialize<T>(json, options);
    }

    /// <summary>
    /// Serializes the value to its JSON string representation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses <see cref="JsonSerializer.Serialize{T}(T, JsonSerializerOptions?)"/> internally.
    /// The generic overload preserves full type information at compile time, ensuring
    /// polymorphic types are serialized correctly without losing derived members.
    /// </para>
    /// <para>
    /// For custom serialization behavior (e.g., camelCase, ignoring nulls, converters),
    /// pass a <see cref="JsonSerializerOptions"/> instance via <paramref name="options"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the value being serialized.</typeparam>
    /// <param name="data">The value to serialize.</param>
    /// <param name="options">
    /// Optional <see cref="JsonSerializerOptions"/> to control serialization behavior.
    /// When <see langword="null"/>, default options are used.
    /// </param>
    /// <returns>A JSON string representation of <paramref name="data"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="JsonException">
    /// Thrown when <paramref name="data"/> cannot be serialized.
    /// </exception>
    /// <example>
    /// <code>
    /// record Person(string Name, int Age);
    ///
    /// new Person("Alice", 30).ToJson();
    /// // returns """{"Name":"Alice","Age":30}"""
    ///
    /// new Person("Alice", 30).ToJson(new JsonSerializerOptions { WriteIndented = true });
    /// // returns a pretty-printed JSON string
    /// </code>
    /// </example>
    public static string ToJson<T>(this T data, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(data);

        return JsonSerializer.Serialize(data, options);
    }

    #endregion Json

    #region XML

    /// <summary>
    /// Serializes the value to its XML string representation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses <see cref="XmlSerializer"/> internally. The type <typeparamref name="T"/>
    /// must be publicly accessible and have a parameterless constructor, as required
    /// by <see cref="XmlSerializer"/>.
    /// </para>
    /// <para>
    /// The output encoding is UTF-8 by default. To customize the XML declaration,
    /// indentation, or namespace handling, provide an <see cref="XmlWriterSettings"/>
    /// instance via <paramref name="settings"/>.
    /// </para>
    /// <para>
    /// <b>Performance note:</b> <see cref="XmlSerializer"/> instances are cached internally
    /// by the runtime when constructed with a type argument alone. Avoid constructing
    /// <see cref="XmlSerializer"/> with extra arguments in hot paths, as those are not cached.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the value being serialized. Must be XML-serializable.</typeparam>
    /// <param name="data">The value to serialize.</param>
    /// <param name="settings">
    /// Optional <see cref="XmlWriterSettings"/> to control output formatting.
    /// When <see langword="null"/>, defaults to indented UTF-8 output with XML declaration omitted.
    /// </param>
    /// <returns>A UTF-8 XML string representation of <paramref name="data"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <typeparamref name="T"/> cannot be serialized by <see cref="XmlSerializer"/>
    /// (e.g., the type is not public or lacks a parameterless constructor).
    /// </exception>
    /// <example>
    /// <code>
    /// public class Person
    /// {
    ///     public string Name { get; set; }
    ///     public int Age { get; set; }
    /// }
    ///
    /// new Person { Name = "Alice", Age = 30 }.ToXml();
    /// // returns:
    /// // &lt;Person&gt;
    /// //   &lt;Name&gt;Alice&lt;/Name&gt;
    /// //   &lt;Age&gt;30&lt;/Age&gt;
    /// // &lt;/Person&gt;
    /// </code>
    /// </example>
    public static string ToXml<T>(this T data, XmlWriterSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(data);

        XmlSerializer serializer = new(typeof(T));

        XmlWriterSettings effectiveSettings = settings ?? new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = true
        };

        using StringWriter stringWriter = new();
        using XmlWriter xmlWriter = XmlWriter.Create(stringWriter, effectiveSettings);

        serializer.Serialize(xmlWriter, data);

        return stringWriter.ToString();
    }

    /// <summary>
    /// Deserializes an XML string into an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses <see cref="XmlSerializer"/> internally. The type <typeparamref name="T"/>
    /// must be publicly accessible and have a parameterless constructor, as required
    /// by <see cref="XmlSerializer"/>.
    /// </para>
    /// <para>
    /// Returns <see langword="null"/> if the XML represents a null or empty element,
    /// or if deserialization yields no result. For stricter null handling, check the
    /// return value before use.
    /// </para>
    /// <para>
    /// <b>Performance note:</b> <see cref="XmlSerializer"/> instances are cached internally
    /// by the runtime when constructed with a type argument alone. Avoid constructing
    /// <see cref="XmlSerializer"/> with extra arguments in hot paths, as those are not cached.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The target type to deserialize into. Must be XML-serializable.</typeparam>
    /// <param name="xml">The XML string to deserialize.</param>
    /// <returns>
    /// An instance of <typeparamref name="T"/>, or <see langword="null"/> if the XML
    /// represents a null value or deserialization yields no result.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="xml"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="xml"/> is malformed or cannot be deserialized
    /// into <typeparamref name="T"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// public class Person
    /// {
    ///     public string Name { get; set; }
    ///     public int Age { get; set; }
    /// }
    ///
    /// """
    /// &lt;Person&gt;
    ///   &lt;Name&gt;Alice&lt;/Name&gt;
    ///   &lt;Age&gt;30&lt;/Age&gt;
    /// &lt;/Person&gt;
    /// """.FromXml&lt;Person&gt;();
    /// // returns Person { Name = "Alice", Age = 30 }
    /// </code>
    /// </example>
    public static T? FromXml<T>(this string xml)
    {
        ArgumentNullException.ThrowIfNull(xml);

        XmlSerializer serializer = new(typeof(T));

        using StringReader stringReader = new(xml);
        using XmlReader xmlReader = XmlReader.Create(stringReader);

        return (T?)serializer.Deserialize(xmlReader);
    }

    #endregion XML

    #region Base64

    /// <summary>
    /// Encodes the string to its Base64 representation using the specified encoding.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses <see cref="Convert.ToBase64String(byte[])"/> internally after encoding
    /// the string bytes with the provided <paramref name="encoding"/>.
    /// </para>
    /// <para>
    /// Defaults to <see cref="Encoding.UTF8"/>, which correctly handles
    /// multi-byte characters such as accented letters and emoji.
    /// </para>
    /// </remarks>
    /// <param name="value">The string to encode.</param>
    /// <param name="encoding">
    /// The character encoding to use when converting the string to bytes.
    /// When <see langword="null"/>, defaults to <see cref="Encoding.UTF8"/>.
    /// </param>
    /// <returns>
    /// A Base64-encoded string representing the UTF-8 (or specified encoding) bytes
    /// of <paramref name="value"/>, or <see cref="string.Empty"/> if
    /// <paramref name="value"/> is empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "hello".ToBase64();              // returns "aGVsbG8="
    /// "São Paulo".ToBase64();          // returns "U8OjbyBQYXVsbw=="
    /// "hello".ToBase64(Encoding.ASCII) // returns "aGVsbG8="
    /// "".ToBase64();                   // returns ""
    /// </code>
    /// </example>
    public static string ToBase64(this string value, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return value;

        encoding ??= Encoding.UTF8;

        return Convert.ToBase64String(encoding.GetBytes(value));
    }

    /// <summary>
    /// Decodes a Base64-encoded string back to its original string representation
    /// using the specified encoding.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses <see cref="Convert.FromBase64String(string)"/> internally and then
    /// decodes the resulting bytes with the provided <paramref name="encoding"/>.
    /// </para>
    /// <para>
    /// The encoding used here must match the one used during <see cref="ToBase64"/>
    /// to guarantee correct round-trip decoding. Defaults to <see cref="Encoding.UTF8"/>.
    /// </para>
    /// </remarks>
    /// <param name="value">The Base64-encoded string to decode.</param>
    /// <param name="encoding">
    /// The character encoding to use when converting bytes back to a string.
    /// When <see langword="null"/>, defaults to <see cref="Encoding.UTF8"/>.
    /// </param>
    /// <returns>
    /// The decoded string, or <see cref="string.Empty"/> if <paramref name="value"/> is empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="FormatException">
    /// Thrown when <paramref name="value"/> is not a valid Base64 string.
    /// </exception>
    /// <example>
    /// <code>
    /// "aGVsbG8=".FromBase64();             // returns "hello"
    /// "U8OjbyBQYXVsbw==".FromBase64();     // returns "São Paulo"
    /// "aGVsbG8=".FromBase64(Encoding.ASCII) // returns "hello"
    /// "".FromBase64();                      // returns ""
    /// "not-base64!!".FromBase64();          // throws FormatException
    /// </code>
    /// </example>
    public static string FromBase64(this string value, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return value;

        encoding ??= Encoding.UTF8;

        return encoding.GetString(Convert.FromBase64String(value));
    }

    /// <summary>
    /// Serializes the value to JSON and encodes the result as a Base64 string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Combines <see cref="ToJson{T}"/> and <see cref="ToBase64"/> in a single operation.
    /// Useful for embedding structured objects in URLs, headers, query strings,
    /// or any transport layer that requires text-safe encoding.
    /// </para>
    /// <para>
    /// The encoding used is always UTF-8. To decode, use <see cref="FromBase64{T}"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the value being serialized.</typeparam>
    /// <param name="value">The value to serialize and encode.</param>
    /// <param name="options">
    /// Optional <see cref="JsonSerializerOptions"/> to control JSON serialization behavior.
    /// When <see langword="null"/>, default options are used.
    /// </param>
    /// <returns>
    /// A Base64-encoded string representing the JSON serialization of <paramref name="value"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="JsonException">
    /// Thrown when <paramref name="value"/> cannot be serialized to JSON.
    /// </exception>
    /// <example>
    /// <code>
    /// record Person(string Name, int Age);
    ///
    /// new Person("Alice", 30).ToBase64();
    /// // returns "eyJOYW1lIjoiQWxpY2UiLCJBZ2UiOjMwfQ=="
    /// </code>
    /// </example>
    public static string ToBase64<T>(this T value, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        string json = value.ToJson(options);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    /// <summary>
    /// Decodes a Base64 string and deserializes the resulting JSON into an instance
    /// of <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Combines <see cref="FromBase64"/> and <see cref="FromJson{T}"/> in a single operation,
    /// reversing the encoding applied by <see cref="ToBase64{T}"/>.
    /// </para>
    /// <para>
    /// The Base64 input must have been encoded using UTF-8. Inputs encoded with
    /// a different encoding will produce incorrect JSON and likely throw a
    /// <see cref="JsonException"/> during deserialization.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The target type to deserialize into.</typeparam>
    /// <param name="value">The Base64-encoded JSON string to decode and deserialize.</param>
    /// <param name="options">
    /// Optional <see cref="JsonSerializerOptions"/> to control JSON deserialization behavior.
    /// When <see langword="null"/>, default options are used.
    /// </param>
    /// <returns>
    /// An instance of <typeparamref name="T"/>, or <see langword="null"/> if the decoded
    /// JSON represents a null value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="FormatException">
    /// Thrown when <paramref name="value"/> is not a valid Base64 string.
    /// </exception>
    /// <exception cref="JsonException">
    /// Thrown when the decoded content is not valid JSON or cannot be deserialized
    /// into <typeparamref name="T"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// record Person(string Name, int Age);
    ///
    /// "eyJOYW1lIjoiQWxpY2UiLCJBZ2UiOjMwfQ==".FromBase64&lt;Person&gt;();
    /// // returns Person { Name = "Alice", Age = 30 }
    ///
    /// "eyJOYW1lIjoiQWxpY2UiLCJBZ2UiOjMwfQ==".FromBase64&lt;Person&gt;();
    /// // round-trips correctly with ToBase64&lt;T&gt;
    /// </code>
    /// </example>
    public static T? FromBase64<T>(this string value, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        string json = Encoding.UTF8.GetString(Convert.FromBase64String(value));
        return json.FromJson<T>(options);
    }

    /// <summary>
    /// Encodes a byte array to its Base64 string representation.
    /// </summary>
    /// <remarks>
    /// Delegates directly to <see cref="Convert.ToBase64String(byte[])"/>,
    /// exposed as an extension for fluent call chaining.
    /// </remarks>
    /// <param name="value">The byte array to encode.</param>
    /// <returns>
    /// A Base64-encoded string representing <paramref name="value"/>,
    /// or <see cref="string.Empty"/> if <paramref name="value"/> is empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// new byte[] { 104, 101, 108, 108, 111 }.ToBase64(); // returns "aGVsbG8="
    /// File.ReadAllBytes("image.png").ToBase64();          // returns Base64 image string
    /// Array.Empty&lt;byte&gt;().ToBase64();                    // returns ""
    /// </code>
    /// </example>
    public static string ToBase64(this byte[] value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return string.Empty;

        return Convert.ToBase64String(value);
    }

    /// <summary>
    /// Decodes a Base64 string back to its original byte array.
    /// </summary>
    /// <remarks>
    /// Delegates directly to <see cref="Convert.FromBase64String(string)"/>,
    /// exposed as an extension for fluent call chaining.
    /// Pair with <see cref="ToBase64(byte[])"/> for a correct round-trip.
    /// </remarks>
    /// <param name="value">The Base64-encoded string to decode.</param>
    /// <returns>
    /// A byte array containing the decoded bytes of <paramref name="value"/>,
    /// or an empty array if <paramref name="value"/> is empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="FormatException">
    /// Thrown when <paramref name="value"/> is not a valid Base64 string.
    /// </exception>
    /// <example>
    /// <code>
    /// "aGVsbG8=".FromBase64();   // returns [ 104, 101, 108, 108, 111 ]
    /// "".FromBase64();            // returns []
    /// "not-base64!!".FromBase64(); // throws FormatException
    /// </code>
    /// </example>
    public static byte[] FromBase64(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return [];

        return Convert.FromBase64String(value);
    }


    /// <summary>
    /// Converts a <see cref="ulong"/> to its Base32 string representation.
    /// </summary>
    /// <param name="value">The <see cref="ulong"/> value to convert.</param>
    /// <returns>A Base32 encoded string representing the <see cref="ulong"/> value.</returns>
    /// <example>
    /// <code>
    /// 12345UL.ToBase32(); // e.g. "C5R"
    /// </code>
    /// </example>
    public static string ToBase32(this ulong value)
    {
        string Base32Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
        Span<char> buffer = stackalloc char[13];
        int pos = buffer.Length;

        do
        {
            buffer[--pos] = Base32Alphabet[(int)(value & 31)];
            value >>= 5;
        }
        while (value != 0);

        return new string(buffer[pos..]);
    }

    #endregion Base64

    #region Byte[] To String

    /// <summary>
    /// Converts the string to a byte array using the specified character encoding.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <param name="encode">
    /// The character encoding to use. Defaults to <see cref="TextEncode.UTF8"/>.
    /// </param>
    /// <returns>
    /// A byte array containing the encoded representation of <paramref name="value"/>,
    /// or an empty array if <paramref name="value"/> is empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// "hello".StringToByteArray();                        // [ 104, 101, 108, 108, 111 ]
    /// "hello".StringToByteArray(TextEncode.ASCII);        // [ 104, 101, 108, 108, 111 ]
    /// "São Paulo".StringToByteArray(TextEncode.UTF8);     // multi-byte UTF-8 sequence
    /// "São Paulo".StringToByteArray(TextEncode.Latin1);   // single-byte Latin1 sequence
    /// "".StringToByteArray();                             // []
    /// </code>
    /// </example>
    public static byte[] StringToByteArray(this string value, TextEncode encode = TextEncode.UTF8)
    {
        ArgumentNullException.ThrowIfNull(value);

        Encoding encoding = encode.ToEncoding();
        return encoding.GetBytes(value);
    }

    /// <summary>
    /// Decodes a byte array back to a string using the specified character encoding.
    /// </summary>
    /// <remarks>
    /// The encoding used here must match the one used during <see cref="ToByteArray"/>
    /// to guarantee correct round-trip decoding. Mismatched encodings may produce
    /// garbled output or throw depending on the encoding's fallback behavior.
    /// </remarks>
    /// <param name="value">The byte array to decode.</param>
    /// <param name="encode">
    /// The character encoding to use. Defaults to <see cref="TextEncode.UTF8"/>.
    /// </param>
    /// <returns>
    /// A string decoded from <paramref name="value"/>,
    /// or <see cref="string.Empty"/> if <paramref name="value"/> is empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="DecoderFallbackException">
    /// Thrown when <paramref name="value"/> contains byte sequences that are invalid
    /// for the specified <paramref name="encode"/>.
    /// </exception>
    /// <example>
    /// <code>
    /// new byte[] { 104, 101, 108, 108, 111 }.ByteArrayToString();               // returns "hello"
    /// new byte[] { 104, 101, 108, 108, 111 }.ByteArrayToString(TextEncode.ASCII); // returns "hello"
    /// Array.Empty&lt;byte&gt;().ByteArrayToString();                                  // returns ""
    /// </code>
    /// </example>
    public static string ByteArrayToString(this byte[] value, TextEncode encode = TextEncode.UTF8)
    {
        ArgumentNullException.ThrowIfNull(value);

        Encoding encoding = encode.ToEncoding();
        return encoding.GetString(value);
    }

    #endregion Byte[] To String

    #region Parses

    #region Signed      

    /// <summary>
    /// Parses the object to a <see cref="byte"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to 0.</param>
    /// <returns>The parsed <see cref="byte"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "255".ToByteOrDefault(); // returns 255
    /// "invalid".ToByteOrDefault(1); // returns 1
    /// </code>
    /// </example>
    public static byte ToByteOrDefault(this object? value, byte defaultValue = 0)
    {
        if (value is null)
            return defaultValue;

        if (value is byte i)
            return i;

        if (value is string s)
            return byte.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out byte result) ? result : defaultValue;

        try
        {
            return Convert.ToByte(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Parses the object to a <see cref="short"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to 0.</param>
    /// <returns>The parsed <see cref="short"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "32000".ToShortOrDefault(); // returns 32000
    /// </code>
    /// </example>
    public static short ToShortOrDefault(this object? value, short defaultValue = 0)
    {
        if (value is null)
            return defaultValue;

        if (value is short i)
            return i;

        if (value is string s)
            return short.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out short result) ? result : defaultValue;

        try
        {
            return Convert.ToInt16(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Parses the object to an <see cref="int"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to 0.</param>
    /// <returns>The parsed <see cref="int"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "42".ToIntOrDefault(); // returns 42
    /// </code>
    /// </example>
    public static int ToIntOrDefault(this object? value, int defaultValue = 0)
    {
        if (value is null)
            return defaultValue;

        if (value is int i)
            return i;

        if (value is string s)        
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : defaultValue;

        try
        {
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Parses the object to a <see cref="long"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to 0.</param>
    /// <returns>The parsed <see cref="long"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "10000000000".ToLongOrDefault(); // returns 10000000000
    /// </code>
    /// </example>
    public static long ToLongOrDefault(this object? value, long defaultValue = 0)
    {
        if (value is null)
            return defaultValue;

        if (value is long i)
            return i;

        if (value is string s)
            return long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out long result) ? result : defaultValue;

        try
        {
            return Convert.ToInt64(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Parses the object to a <see cref="float"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to 0f.</param>
    /// <returns>The parsed <see cref="float"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "3.14".ToFloatOrDefault(); // returns 3.14f
    /// </code>
    /// </example>
    public static float ToFloatOrDefault(this object? value, float defaultValue = 0f)
    {
        if (value is null)
            return defaultValue;

        if (value is float i)
            return i;

        if (value is string s)
            return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float result) ? result : defaultValue;

        try
        {
            return Convert.ToSingle(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Parses the object to a <see cref="double"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to 0.0.</param>
    /// <returns>The parsed <see cref="double"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "2.718".ToDoubleOrDefault(); // returns 2.718
    /// </code>
    /// </example>
    public static double ToDoubleOrDefault(this object? value, double defaultValue = 0.0)
    {
        if (value is null)
            return defaultValue;

        if (value is double i)
            return i;

        if (value is string s)
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double result) ? result : defaultValue;

        try
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Parses the object to a <see cref="decimal"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to 0m.</param>
    /// <returns>The parsed <see cref="decimal"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "10.50".ToDecimalOrDefault(); // returns 10.50m
    /// </code>
    /// </example>
    public static decimal ToDecimalOrDefault(this object? value, decimal defaultValue = 0m)
    {
        if (value is null)
            return defaultValue;

        if (value is decimal i)
            return i;

        if (value is string s)
            return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result) ? result : defaultValue;

        try
        {
            return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion Signed

    #region Unsigned

    /// <summary>
    /// Parses the object to an <see cref="sbyte"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to 0.</param>
    /// <returns>The parsed <see cref="sbyte"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "-5".ToSByteOrDefault(); // returns -5
    /// </code>
    /// </example>
    public static sbyte ToSByteOrDefault(this object? value, sbyte defaultValue = 0)
    {
        if (value is null)
            return defaultValue;

        if (value is sbyte i)
            return i;

        if (value is string s)
            return sbyte.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out sbyte result) ? result : defaultValue;

        try
        {
            return Convert.ToSByte(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Parses the object to a <see cref="ushort"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to 0.</param>
    /// <returns>The parsed <see cref="ushort"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "65000".ToUShortOrDefault(); // returns 65000
    /// </code>
    /// </example>
    public static ushort ToUShortOrDefault(this object? value, ushort defaultValue = 0)
    {
        if (value is null)
            return defaultValue;

        if (value is ushort i)
            return i;

        if (value is string s)
            return ushort.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out ushort result) ? result : defaultValue;

        try
        {
            return Convert.ToUInt16(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Parses the object to a <see cref="uint"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to 0.</param>
    /// <returns>The parsed <see cref="uint"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "3000000000".ToUIntOrDefault(); // returns 3000000000
    /// </code>
    /// </example>
    public static uint ToUIntOrDefault(this object? value, uint defaultValue = 0)
    {
        if (value is null)
            return defaultValue;

        if (value is uint i)
            return i;

        if (value is string s)
            return uint.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint result) ? result : defaultValue;

        try
        {
            return Convert.ToUInt32(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Parses the object to a <see cref="ulong"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to 0.</param>
    /// <returns>The parsed <see cref="ulong"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "10000000000".ToULongOrDefault(); // returns 10000000000
    /// </code>
    /// </example>
    public static ulong ToULongOrDefault(this object? value, ulong defaultValue = 0)
    {
        if (value is null)
            return defaultValue;

        if (value is ulong i)
            return i;

        if (value is string s)
            return ulong.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong result) ? result : defaultValue;

        try
        {
            return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion Unsigned

    #region Special

    /// <summary>
    /// Parses the object to a <see cref="bool"/> or returns the specified default value.
    /// Maps "1", "yes", "y", "t" to true and "0", "no", "n", "f" to false as an extra fallback.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to false.</param>
    /// <returns>The parsed <see cref="bool"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "yes".ToBooleanOrDefault(); // returns true
    /// </code>
    /// </example>
    public static bool ToBooleanOrDefault(this object? value, bool defaultValue = false)
    {
        if (value is null)
            return defaultValue;

        if (value is bool i)
            return i;

        if (value is string s)
        {
            if (bool.TryParse(s, out bool result))
                return result;

            // Extra fallback amigável para padrões comuns de texto em APIs/Bancos de dados
            return s.Trim().ToLowerInvariant() switch
            {
                "1" or "yes" or "y" or "t" => true,
                "0" or "no" or "n" or "f" => false,
                _ => defaultValue
            };
        }

        try
        {
            return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Parses the object to a <see cref="char"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to '\0'.</param>
    /// <returns>The parsed <see cref="char"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "A".ToCharOrDefault(); // returns 'A'
    /// </code>
    /// </example>
    public static char ToCharOrDefault(this object? value, char defaultValue = '\0')
    {
        if (value is null)
            return defaultValue;

        if (value is char c)
            return c;

        if (value is string s && s.Length == 1)
            return s[0];

        try
        {
            return Convert.ToChar(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Converts the object to a <see cref="string"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if conversion fails. Defaults to an empty string.</param>
    /// <returns>The converted <see cref="string"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// ((object)null).ToStringOrDefault("fallback"); // returns "fallback"
    /// </code>
    /// </example>
    public static string ToStringOrDefault(this object? value, string defaultValue = "")
    {
        if (value is null)
            return defaultValue;

        if (value is string s)
            return s;

        try
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Parses the object to a <see cref="DateTime"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to default(DateTime).</param>
    /// <returns>The parsed <see cref="DateTime"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "2024-01-01".ToDateTimeOrDefault(); // returns a parsed DateTime
    /// </code>
    /// </example>
    public static DateTime ToDateTimeOrDefault(this object? value, DateTime defaultValue = default)
    {
        if (value is null)
            return defaultValue;

        if (value is DateTime dt)
            return dt;

        if (value is string s)        
            return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result) ? result : defaultValue;        

        try
        {
            return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Parses the object to a <see cref="DateTimeOffset"/> or returns the specified default value.
    /// </summary>
    /// <param name="value">The object to parse.</param>
    /// <param name="defaultValue">The value to return if parsing fails. Defaults to default(DateTimeOffset).</param>
    /// <returns>The parsed <see cref="DateTimeOffset"/> or the default value.</returns>
    /// <example>
    /// <code>
    /// "2024-01-01T00:00:00Z".ToDateTimeOffsetOrDefault(); // returns a parsed DateTimeOffset
    /// </code>
    /// </example>
    public static DateTimeOffset ToDateTimeOffsetOrDefault(this object? value, DateTimeOffset defaultValue = default)
    {
        if (value is null)
            return defaultValue;

        if (value is DateTimeOffset dto)
            return dto;

        if (value is DateTime dt)
            return new DateTimeOffset(dt);

        if (value is string s)
            return DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result) ? result : defaultValue;
        

        try
        {
            // Convert does not have direct native support for DateTimeOffset, so we map it safely via string/IConvertible.
            if (value is IConvertible convertible)
            {
                string sValue = convertible.ToString(CultureInfo.InvariantCulture);
                return DateTimeOffset.TryParse(sValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result) ? result : defaultValue;
            }

            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion Special

    #endregion Parses

    #region Responsive

    /// <summary>
    /// Maps a <see cref="TextEncode"/> enum to the corresponding <see cref="Encoding"/> instance.
    /// </summary>
    /// <param name="textEncode">The text encode value to convert.</param>
    /// <returns>An <see cref="Encoding"/> representing the given TextEncode.</returns>
    /// <example>
    /// <code>
    /// TextEncode.UTF8.ToEncoding(); // returns Encoding.UTF8
    /// </code>
    /// </example>
    public static Encoding ToEncoding(this TextEncode textEncode)
    {
        return textEncode switch
        {
            TextEncode.ASCII => Encoding.ASCII,
            TextEncode.UTF8 => Encoding.UTF8,
            TextEncode.UTF16 => Encoding.BigEndianUnicode,
            TextEncode.UTF32 => Encoding.UTF32,
            TextEncode.Unicode => Encoding.Unicode,
            TextEncode.Latin1 => Encoding.Latin1,
            _ => Encoding.UTF8,
        };
    }

    #endregion Responsive
}