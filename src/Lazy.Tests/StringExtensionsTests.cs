using Lazy.Extensions;

namespace Lazy.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void RemoveDiacritics_ReturnsStringWithoutAccents()
    {
        string input = "São Paulo é uma cidade maravilhosa";
        string expected = "Sao Paulo e uma cidade maravilhosa";
        Assert.Equal(expected, input.RemoveDiacritics());
    }

    [Fact]
    public void RemoveNonAlphanumeric_ReturnsOnlyLettersAndNumbers()
    {
        string input = "Hello, World! 123";
        Assert.Equal("Hello,World123", input.RemoveNonAlphanumeric().Replace(" ", ""));
        Assert.Equal("Hello, World  123", input.RemoveNonAlphanumeric(useSpace: true));
    }

    [Fact]
    public void Sanitize_RemovesDiacriticsAndNonAlphanumeric()
    {
        string input = "João & Maria - 123";
        Assert.Equal("JoaoMaria123", input.Sanitize().Replace(" ", ""));
        Assert.Equal("Joao   Maria   123", input.Sanitize(useSpace: true));
    }

    [Fact]
    public void TrimSpan_RemovesWhitespaceFromBothEnds()
    {
        string input = "  hello world  ";
        Assert.Equal("hello world", input.TrimSpan());
    }

    [Fact]
    public void TrimEndSpan_RemovesWhitespaceFromEnd()
    {
        string input = "hello world  ";
        Assert.Equal("hello world", input.TrimEndSpan());
    }

    [Fact]
    public void TrimStartSpan_RemovesWhitespaceFromStart()
    {
        string input = "  hello world";
        Assert.Equal("hello world", input.TrimStartSpan());
    }

    [Fact]
    public void ToSentenceCase_FormatsCorrectly()
    {
        string input = "HELLO WORLD";
        Assert.Equal("Hello world", input.ToSentenceCase());
    }

    [Fact]
    public void GetUntil_ReturnsSubstringBeforeCharacter()
    {
        string input = "user@example.com";
        Assert.Equal("user", input.GetUntil('@'));
    }

    [Fact]
    public void GetAfter_ReturnsSubstringAfterCharacter()
    {
        string input = "user@example.com";
        Assert.Equal("example.com", input.GetAfter('@'));
    }

    [Fact]
    public void CountOccurrences_ReturnsCorrectCount()
    {
        string input = "hello hello world";
        Assert.Equal(2, input.CountOccurrences("hello"));
        Assert.Equal(3, input.CountOccurrences('o'));
    }

    [Fact]
    public void GetBetween_ReturnsCorrectSubstring()
    {
        string input = "value = [hello world]";
        Assert.Equal("hello world", input.GetBetween('[', ']'));
        Assert.Equal("hello world", input.GetBetween("[", "]"));
    }

    [Fact]
    public void ContainsAny_ReturnsTrueIfMatchFound()
    {
        string input = "hello";
        Assert.True(input.ContainsAny("xyo"));
        Assert.False(input.ContainsAny("xyz"));
    }

    [Fact]
    public void ContainsIgnoreCase_ReturnsCorrectly()
    {
        string input = "Hello World";
        Assert.True(input.ContainsIgnoreCase("WORLD"));
        Assert.False(input.ContainsIgnoreCase("TEST"));
    }

    [Fact]
    public void StartsWithAny_ReturnsTrueIfMatch()
    {
        string input = "https://example.com";
        Assert.True(input.StartsWithAny("http://", "https://"));
        Assert.False(input.StartsWithAny("ftp://"));
    }

    [Fact]
    public void EndsWithAny_ReturnsTrueIfMatch()
    {
        string input = "image.png";
        Assert.True(input.EndsWithAny(".jpg", ".png"));
        Assert.False(input.EndsWithAny(".gif"));
    }
    [Theory]
    [InlineData("abc123def456", false, "abcdef")]
    [InlineData("SP_1234X", false, "SPX")]
    [InlineData("SP_1234X", true, "SP     X")]
    [InlineData("", false, "")]
    [InlineData(null, false, "")]
    public void ExtractLetters_ShouldReturnExpectedLetters(string? input, bool replaceWithSpace, string expected)
    {
        Assert.Equal(expected, input!.ExtractLetters(replaceWithSpace));
    }

    [Theory]
    [InlineData("abc123def456", false, "123456")]
    [InlineData("SP_1234X", false, "1234")]
    [InlineData("SP_1234X", true, "   1234 ")]
    [InlineData("", false, "")]
    [InlineData(null, false, "")]
    public void ExtractNumbers_ShouldReturnExpectedDigits(string? input, bool replaceWithSpace, string expected)
    {
        Assert.Equal(expected, input!.ExtractNumbers(replaceWithSpace));
    }

    [Theory]
    [InlineData("255", true, (byte)255)]
    [InlineData("abc", false, (byte)0)]
    public void TryParseNumbers_Byte_ReturnsExpectedResult(string input, bool expectedSuccess, byte expectedValue)
    {
        bool success = input.TryParseNumbers(out byte result);
        Assert.Equal(expectedSuccess, success);
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData("127", true, (sbyte)127)]
    [InlineData("abc", false, (sbyte)0)]
    public void TryParseNumbers_SByte_ReturnsExpectedResult(string input, bool expectedSuccess, sbyte expectedValue)
    {
        bool success = input.TryParseNumbers(out sbyte result);
        Assert.Equal(expectedSuccess, success);
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData("12345", true, (short)12345)]
    [InlineData("abc", false, (short)0)]
    public void TryParseNumbers_Short_ReturnsExpectedResult(string input, bool expectedSuccess, short expectedValue)
    {
        bool success = input.TryParseNumbers(out short result);
        Assert.Equal(expectedSuccess, success);
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData("65535", true, (ushort)65535)]
    [InlineData("abc", false, (ushort)0)]
    public void TryParseNumbers_UShort_ReturnsExpectedResult(string input, bool expectedSuccess, ushort expectedValue)
    {
        bool success = input.TryParseNumbers(out ushort result);
        Assert.Equal(expectedSuccess, success);
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData("123", true, 123)]
    [InlineData("abc", false, 0)]
    public void TryParseNumbers_Int_ReturnsExpectedResult(string input, bool expectedSuccess, int expectedValue)
    {
        bool success = input.TryParseNumbers(out int result);
        Assert.Equal(expectedSuccess, success);
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData("4294967295", true, 4294967295u)]
    [InlineData("abc", false, 0u)]
    public void TryParseNumbers_UInt_ReturnsExpectedResult(string input, bool expectedSuccess, uint expectedValue)
    {
        bool success = input.TryParseNumbers(out uint result);
        Assert.Equal(expectedSuccess, success);
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData("1234567890123", true, 1234567890123L)]
    [InlineData("abc", false, 0L)]
    public void TryParseNumbers_Long_ReturnsExpectedResult(string input, bool expectedSuccess, long expectedValue)
    {
        bool success = input.TryParseNumbers(out long result);
        Assert.Equal(expectedSuccess, success);
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void TryParseNumbers_ULong_ReturnsTrueIfValid()
    {
        bool success = "A1234567890123".TryParseNumbers(out ulong result);
        Assert.True(success);
        Assert.Equal(1234567890123ul, result);
    }

    [Theory]
    [InlineData("12.34", true, 12.34f)]
    [InlineData("12,34", true, 12.34f)]
    [InlineData("abc", false, 0f)]
    public void TryParseFloat_ReturnsExpectedResult(string input, bool expectedSuccess, float expectedValue)
    {
        bool success = input.TryParseFloat(out float result);
        Assert.Equal(expectedSuccess, success);
        Assert.Equal(expectedValue, result, 2);
    }

    [Theory]
    [InlineData("12.34", true, 12.34)]
    [InlineData("12,34", true, 12.34)]
    [InlineData("abc", false, 0d)]
    public void TryParseDouble_ReturnsExpectedResult(string input, bool expectedSuccess, double expectedValue)
    {
        bool success = input.TryParseDouble(out double result);
        Assert.Equal(expectedSuccess, success);
        Assert.Equal(expectedValue, result, 2);
    }

    [Theory]
    [InlineData("12.34", true, 12.34)]
    [InlineData("12,34", true, 12.34)]
    [InlineData("abc", false, 0)]
    public void TryParseDecimal_ReturnsExpectedResult(string input, bool expectedSuccess, double expectedValue)
    {
        bool success = input.TryParseDecimal(out decimal result);
        Assert.Equal(expectedSuccess, success);
        Assert.Equal((decimal)expectedValue, result);
    }

    [Fact]
    public void MatchWith_ReturnsTrueIfIdentical()
    {
        string input = "test";
        Assert.True(input.MatchWith("test"));
        Assert.True(input.MatchWith("Test"));
    }

    [Fact]
    public void IsNullOrBlank_ReturnsCorrectly()
    {
        Assert.True((null as string).IsNullOrBlank());
        Assert.True("".IsNullOrBlank());
        Assert.True("   ".IsNullOrBlank());
        Assert.False("test".IsNullOrBlank());
    }

    [Fact]
    public void IsAllSameDigits_ReturnsCorrectly()
    {
        Assert.True("111111".AsSpan().IsAllSameDigits());
        Assert.False("111211".AsSpan().IsAllSameDigits());
    }

    [Fact]
    public void Truncate_ReturnsShortenedString()
    {
        string input = "hello world";
        Assert.Equal("he...", input.Truncate(5, "..."));
        Assert.Equal("hello world", input.Truncate(20, "..."));
    }

    [Fact]
    public void Slugify_ReturnsUrlFriendlyString()
    {
        string input = "C# is Great!";
        Assert.Equal("c-is-great", input.Slugify());
    }

    [Fact]
    public void Repeat_ReturnsRepeatedString()
    {
        Assert.Equal("abcabcabc", "abc".Repeat(3));
        Assert.Equal("", "abc".Repeat(0));
    }

    [Fact]
    public void PadCenter_ReturnsCenteredString()
    {
        Assert.Equal("  hi  ", "hi".PadCenter(6));
        Assert.Equal("--hi--", "hi".PadCenter(6, '-'));
    }

    [Fact]
    public void Mask_ReturnsMaskedString()
    {
        string input = "1234567890";
        Assert.Equal("******7890", input.Mask());
        Assert.Equal("12****7890", input.Mask(2, 4));
    }

    [Fact]
    public void MaskEmail_ReturnsMaskedEmail()
    {
        string input = "user@example.com";
        Assert.Equal("us**@example.com", input.MaskEmail());
        Assert.Equal("ab@example.com", "ab@example.com".MaskEmail());
    }
}
