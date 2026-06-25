namespace Lazy.Extensions;

public static class CryptographyExtensions
{
    #region Checksums 

    /// <summary>
    /// Computes the CRC32 checksum of the specified string.
    /// </summary>
    /// <param name="value">The string to compute the checksum for.</param>
    /// <param name="tencode">The text encoding to use. Defaults to <see cref="TextEncode.UTF8"/>.</param>
    /// <returns>The CRC32 checksum as a <see cref="uint"/>, or 0 if the string is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <example>
    /// <code>
    /// "hello".ToCrc32();
    /// </code>
    /// </example>
    public static uint ToCrc32(this string value, TextEncode tencode = TextEncode.UTF8)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return 0;

        Encoding encoding = tencode.ToEncoding();
        int byteCount = encoding.GetByteCount(value);
        byte[]? rentedArray = null;
        Span<byte> buffer = byteCount <= 512 ? stackalloc byte[byteCount] : (rentedArray = ArrayPool<byte>.Shared.Rent(byteCount));

        try
        {
            encoding.GetBytes(value, buffer);
            return Crc32.HashToUInt32(buffer);
        }
        finally
        {
            if (rentedArray is not null)
                ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Computes the CRC64 checksum of the specified string.
    /// </summary>
    /// <param name="value">The string to compute the checksum for.</param>
    /// <param name="tencode">The text encoding to use. Defaults to <see cref="TextEncode.UTF8"/>.</param>
    /// <returns>The CRC64 checksum as a <see cref="ulong"/>, or 0 if the string is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <example>
    /// <code>
    /// "hello".ToCrc64();
    /// </code>
    /// </example>
    public static ulong ToCrc64(this string value, TextEncode tencode = TextEncode.UTF8)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return 0;

        Encoding encoding = tencode.ToEncoding();
        int byteCount = encoding.GetByteCount(value);
        byte[]? rentedArray = null;
        Span<byte> buffer = byteCount <= 512 ? stackalloc byte[byteCount] : (rentedArray = ArrayPool<byte>.Shared.Rent(byteCount));

        try
        {
            encoding.GetBytes(value, buffer);
            return Crc64.HashToUInt64(buffer);
        }
        finally
        {
            if (rentedArray is not null)
                ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Computes the XxHash32 hash of the specified string.
    /// </summary>
    /// <param name="value">The string to compute the hash for.</param>
    /// <param name="tencode">The text encoding to use. Defaults to <see cref="TextEncode.UTF8"/>.</param>
    /// <returns>The XxHash32 as a <see cref="uint"/>, or 0 if the string is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <example>
    /// <code>
    /// "hello".ToXxHash32();
    /// </code>
    /// </example>
    public static uint ToXxHash32(this string value, TextEncode tencode = TextEncode.UTF8)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return 0;

        Encoding encoding = tencode.ToEncoding();
        int byteCount = encoding.GetByteCount(value);
        byte[]? rentedArray = null;
        Span<byte> buffer = byteCount <= 512 ? stackalloc byte[byteCount] : (rentedArray = ArrayPool<byte>.Shared.Rent(byteCount));

        try
        {
            encoding.GetBytes(value, buffer);
            return XxHash32.HashToUInt32(buffer);
        }
        finally
        {
            if (rentedArray is not null)
                ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Computes the XxHash64 hash of the specified string.
    /// </summary>
    /// <param name="value">The string to compute the hash for.</param>
    /// <param name="tencode">The text encoding to use. Defaults to <see cref="TextEncode.UTF8"/>.</param>
    /// <returns>The XxHash64 as a <see cref="ulong"/>, or 0 if the string is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <example>
    /// <code>
    /// "hello".ToXxHash64();
    /// </code>
    /// </example>
    public static ulong ToXxHash64(this string value, TextEncode tencode = TextEncode.UTF8)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return 0;

        Encoding encoding = tencode.ToEncoding();
        int byteCount = encoding.GetByteCount(value);
        byte[]? rentedArray = null;
        Span<byte> buffer = byteCount <= 512 ? stackalloc byte[byteCount] : (rentedArray = ArrayPool<byte>.Shared.Rent(byteCount));

        try
        {
            encoding.GetBytes(value, buffer);
            return XxHash64.HashToUInt64(buffer);
        }
        finally
        {
            if (rentedArray is not null)
                ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Computes the XxHash128 hash of the specified string.
    /// </summary>
    /// <param name="value">The string to compute the hash for.</param>
    /// <param name="tencode">The text encoding to use. Defaults to <see cref="TextEncode.UTF8"/>.</param>
    /// <returns>The XxHash128 as a <see cref="UInt128"/>, or 0 if the string is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <example>
    /// <code>
    /// "hello".ToXxHash128();
    /// </code>
    /// </example>
    public static UInt128 ToXxHash128(this string value, TextEncode tencode = TextEncode.UTF8)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return 0;

        Encoding encoding = tencode.ToEncoding();
        int byteCount = encoding.GetByteCount(value);
        byte[]? rentedArray = null;
        Span<byte> buffer = byteCount <= 512 ? stackalloc byte[byteCount] : (rentedArray = ArrayPool<byte>.Shared.Rent(byteCount));

        try
        {
            encoding.GetBytes(value, buffer);
            return  XxHash128.HashToUInt128(buffer);
        }
        finally
        {
            if (rentedArray is not null)
                ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Computes the XxHash3 hash of the specified string.
    /// </summary>
    /// <param name="value">The string to compute the hash for.</param>
    /// <param name="tencode">The text encoding to use. Defaults to <see cref="TextEncode.UTF8"/>.</param>
    /// <returns>The XxHash3 as a <see cref="ulong"/>, or 0 if the string is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <example>
    /// <code>
    /// "hello".ToXxHash3();
    /// </code>
    /// </example>
    public static ulong ToXxHash3(this string value, TextEncode tencode = TextEncode.UTF8)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return 0;

        Encoding encoding = tencode.ToEncoding();
        int byteCount = encoding.GetByteCount(value);
        byte[]? rentedArray = null;
        Span<byte> buffer = byteCount <= 512 ? stackalloc byte[byteCount] : (rentedArray = ArrayPool<byte>.Shared.Rent(byteCount));

        try
        {
            encoding.GetBytes(value, buffer);
            return XxHash3.HashToUInt64(buffer);
        }
        finally
        {
            if (rentedArray is not null)
                ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Computes the XxHash3 hash of the specified string and returns it as a string.
    /// </summary>
    /// <param name="value">The string to compute the hash for.</param>
    /// <param name="tencode">The text encoding to use. Defaults to <see cref="TextEncode.UTF8"/>.</param>
    /// <returns>The XxHash3 hash as a string, or an empty string if the input is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <example>
    /// <code>
    /// "hello".ToXxHash3String();
    /// </code>
    /// </example>
    public static string ToXxHash3String(this string value, TextEncode tencode = TextEncode.UTF8)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return string.Empty;

        Encoding encoding = tencode.ToEncoding();
        int byteCount = encoding.GetByteCount(value);
        byte[]? rentedArray = null;
        Span<byte> buffer = byteCount <= 512 ? stackalloc byte[byteCount] : (rentedArray = ArrayPool<byte>.Shared.Rent(byteCount));

        try
        {
            encoding.GetBytes(value, buffer);
            return XxHash3.Hash(buffer).ByteArrayToString();
        }
        finally
        {
            if (rentedArray is not null)
                ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Computes the MD5 hash of the specified string.
    /// </summary>
    /// <param name="value">The string to compute the hash for.</param>
    /// <param name="tencode">The text encoding to use. Defaults to <see cref="TextEncode.UTF8"/>.</param>
    /// <returns>The MD5 hash as a lowercase hexadecimal string, or an empty string if the input is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <example>
    /// <code>
    /// "hello".ToMd5();
    /// </code>
    /// </example>
    public static string ToMd5(this string value, TextEncode tencode = TextEncode.UTF8)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return string.Empty;

        Encoding encoding = tencode.ToEncoding();
        Span<byte> hashBuffer = stackalloc byte[16];
        int byteCount = encoding.GetByteCount(value);
        byte[]? rentedArray = null;
        Span<byte> inputBuffer = byteCount <= 512 ? stackalloc byte[byteCount] : (rentedArray = ArrayPool<byte>.Shared.Rent(byteCount));

        try
        {
            int written = encoding.GetBytes(value, inputBuffer);
            MD5.HashData(inputBuffer[..written], hashBuffer);
            return Convert.ToHexStringLower(hashBuffer);
        }
        finally
        {
            if (rentedArray is not null)
                ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Computes the SHA-256 hash of the specified string.
    /// </summary>
    /// <param name="value">The string to compute the hash for.</param>
    /// <param name="tencode">The text encoding to use. Defaults to <see cref="TextEncode.UTF8"/>.</param>
    /// <returns>The SHA-256 hash as a lowercase hexadecimal string, or an empty string if the input is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <example>
    /// <code>
    /// "hello".ToSha256();
    /// </code>
    /// </example>
    public static string ToSha256(this string value, TextEncode tencode = TextEncode.UTF8)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length == 0)
            return string.Empty;

        Encoding encoding = tencode.ToEncoding();

        // SHA256 always generates a hash of exactly 32 bytes.
        Span<byte> hashBuffer = stackalloc byte[32];
        int byteCount = encoding.GetByteCount(value);
        byte[]? rentedArray = null;
        Span<byte> inputBuffer = byteCount <= 512 ? stackalloc byte[byteCount] : (rentedArray = ArrayPool<byte>.Shared.Rent(byteCount));

        try
        {
            encoding.GetBytes(value, inputBuffer);
            SHA256.HashData(inputBuffer, hashBuffer);
            return Convert.ToHexString(hashBuffer).ToLowerInvariant();
        }
        finally
        {
            if (rentedArray is not null)
                ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Computes a Base32 encoded TagHash using XxHash64 for the specified string.
    /// </summary>
    /// <param name="value">The string to compute the tag hash for.</param>
    /// <returns>The tag hash as a Base32 string.</returns>
    /// <example>
    /// <code>
    /// "hello".ToTagHash();
    /// </code>
    /// </example>
    public static string ToTagHash(this string value)
    {
        ulong hash = value.ToXxHash64();
        return hash.ToBase32(); 
    }

    /// <summary>
    /// Computes a formatted TaggedHash using a sanitized prefix of the string and its TagHash.
    /// </summary>
    /// <param name="value">The string to compute the tagged hash for.</param>
    /// <returns>A formatted string combining the sanitized value and its tag hash.</returns>
    /// <example>
    /// <code>
    /// "hello world".ToTaggedHash(); // e.g., "HELLO-WORLD-[Hash]"
    /// </code>
    /// </example>
    public static string ToTaggedHash(this string value)
    {
        string tag = value.Sanitize().ToUpperInvariant().Replace(' ', '-');
        return $"{tag}-{value.ToTagHash()}";
    }

    #endregion Checksums

    #region Generative

    /// <summary>
    /// Generates a random cryptographic token of the specified length using the allowed characters.
    /// </summary>
    /// <param name="length">The length of the token to generate.</param>
    /// <param name="allowedChars">The allowed characters to use. Defaults to alphanumeric characters.</param>
    /// <returns>A cryptographically secure random string token.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="length"/> is less than or equal to zero.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="allowedChars"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="allowedChars"/> is an empty string.</exception>
    /// <example>
    /// <code>
    /// CryptographyExtensions.GenerateRandomToken(16);
    /// </code>
    /// </example>
    public static string GenerateRandomToken(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
        ArgumentNullException.ThrowIfNull(allowedChars);

        if (allowedChars.Length == 0)
            throw new ArgumentException("Allowed characters string cannot be empty.", nameof(allowedChars));

        return string.Create(length, (allowedChars, length), static (span, state) =>
        {
            Span<byte> randomBytes = stackalloc byte[state.length];
            RandomNumberGenerator.Fill(randomBytes);

            for (int i = 0; i < span.Length; i++)
            {
                int charIndex = randomBytes[i] % state.allowedChars.Length;
                span[i] = state.allowedChars[charIndex];
            }
        });
    }

    /// <summary>
    /// Generates a short, URL-safe unique identifier based on a newly generated Guid.
    /// </summary>
    /// <returns>A 22-character unique URL-safe string.</returns>
    /// <example>
    /// <code>
    /// CryptographyExtensions.GenerateShortId();
    /// </code>
    /// </example>
    public static string GenerateShortId()
    {
        Guid guid = Guid.NewGuid();
        Span<byte> guidBytes = stackalloc byte[16];
        guid.TryWriteBytes(guidBytes);

        Span<char> base64Buffer = stackalloc char[24];
        Convert.TryToBase64Chars(guidBytes, base64Buffer, out _);

        int validLength = 22;
        for (int i = 0; i < validLength; i++)
        {
            if (base64Buffer[i] == '+') base64Buffer[i] = '-';
            else if (base64Buffer[i] == '/') base64Buffer[i] = '_';
        }

        return new string(base64Buffer[..validLength]);
    }

    #endregion Generative
}