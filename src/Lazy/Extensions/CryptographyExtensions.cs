namespace Lazy.Extensions;

public static class CryptographyExtensions
{
    #region Checksums 

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

    public static string ToTagHash(this string value)
    {
        ulong hash = value.ToXxHash64();
        return hash.ToBase32(); 
    }

    public static string ToTaggedHash(this string value)
    {
        string tag = value.Sanitize().ToUpperInvariant().Replace(' ', '-');
        return $"{tag}-{value.ToTagHash()}";
    }

    #endregion Checksums

    #region Generative

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