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
