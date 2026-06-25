using System.Text;
using System.Text.Json;
using System.Xml;
using Lazy.Extensions;

namespace Lazy.Tests;

public class ConversionsTests
{
    public enum Status : byte { Active = 1, Inactive = 2 }

    [Fact]
    public void ToEnum_ReturnsCorrectValue()
    {
        Assert.Equal(Status.Active, ((byte)1).ToEnum<Status>());
        Assert.Equal(Status.Active, ((byte)1).ToEnum<Status>());
        Assert.Equal(Status.Active, ((byte)99).ToEnum<Status>(fallbackToFirst: true));
        Assert.Equal(Status.Inactive, ((byte)99).ToEnum(Status.Inactive));
        Assert.Equal(Status.Inactive, ((byte)99).ToEnum(Status.Inactive));
    }

    [Fact]
    public void ToEnum_Invalid_Throws()
    {
        Assert.Throws<ArgumentException>(() => ((byte)99).ToEnum<Status>());
        Assert.Throws<ArgumentException>(() => ((byte)99).ToEnum<Status>());
    }

    private record Person(string Name, int Age) { public Person() : this("", 0) {} }

    [Fact]
    public void Json_Roundtrip_Successful()
    {
        var person = new Person("Alice", 30);
        string json = person.ToJson();
        var result = json.FromJson<Person>();

        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
        Assert.Equal(30, result.Age);
    }

    public class XmlPerson
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    [Fact]
    public void Xml_Roundtrip_Successful()
    {
        var person = new XmlPerson { Name = "Alice", Age = 30 };
        string xml = person.ToXml();
        var result = xml.FromXml<XmlPerson>();

        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
        Assert.Equal(30, result.Age);
    }

    [Fact]
    public void Base64_String_Roundtrip_Successful()
    {
        string text = "hello";
        string base64 = text.ToBase64();
        Assert.Equal("aGVsbG8=", base64);
        Assert.Equal(text, Conversions.FromBase64(base64, null));
    }

    [Fact]
    public void Base64_Object_Roundtrip_Successful()
    {
        var person = new Person("Alice", 30);
        string base64 = person.ToBase64();
        var result = base64.FromBase64<Person>();

        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
        Assert.Equal(30, result.Age);
    }

    [Fact]
    public void Base64_ByteArray_Roundtrip_Successful()
    {
        byte[] bytes = new byte[] { 104, 101, 108, 108, 111 };
        string base64 = bytes.ToBase64();
        Assert.Equal("aGVsbG8=", base64);
        Assert.Equal<byte>(bytes, Conversions.FromBase64(base64));
    }

    [Fact]
    public void ToBase32_ReturnsCorrectly()
    {
        ulong value = 12345;
        string base32 = value.ToBase32();
        Assert.Equal("C1S", base32);
    }

    [Fact]
    public void ByteArray_String_Roundtrip_Successful()
    {
        string text = "hello";
        byte[] bytes = text.StringToByteArray();
        Assert.Equal(text, bytes.ByteArrayToString());
    }

    [Fact]
    public void ToEncoding_ReturnsCorrectly()
    {
        Assert.Equal(Encoding.UTF8, TextEncode.UTF8.ToEncoding());
        Assert.Equal(Encoding.ASCII, TextEncode.ASCII.ToEncoding());
    }

    [Fact]
    public void ToOrDefault_ParsesCorrectly()
    {
        Assert.Equal((byte)255, "255".ToByteOrDefault());
        Assert.Equal((byte)1, "invalid".ToByteOrDefault(1));

        Assert.Equal((short)32000, "32000".ToShortOrDefault());
        Assert.Equal((short)1, "invalid".ToShortOrDefault(1));

        Assert.Equal(42, "42".ToIntOrDefault());
        Assert.Equal(1, "invalid".ToIntOrDefault(1));

        Assert.Equal(10000000000L, "10000000000".ToLongOrDefault());
        Assert.Equal(1L, "invalid".ToLongOrDefault(1));

        Assert.Equal(3.14f, "3.14".ToFloatOrDefault());
        Assert.Equal(1f, "invalid".ToFloatOrDefault(1));

        Assert.Equal(2.718, "2.718".ToDoubleOrDefault());
        Assert.Equal(1.0, "invalid".ToDoubleOrDefault(1));

        Assert.Equal(10.50m, "10.50".ToDecimalOrDefault());
        Assert.Equal(1m, "invalid".ToDecimalOrDefault(1));

        Assert.Equal((sbyte)-5, "-5".ToSByteOrDefault());
        Assert.Equal((sbyte)1, "invalid".ToSByteOrDefault(1));

        Assert.Equal((ushort)65000, "65000".ToUShortOrDefault());
        Assert.Equal((ushort)1, "invalid".ToUShortOrDefault(1));

        Assert.Equal(3000000000U, "3000000000".ToUIntOrDefault());
        Assert.Equal(1U, "invalid".ToUIntOrDefault(1));

        Assert.Equal(10000000000UL, "10000000000".ToULongOrDefault());
        Assert.Equal(1UL, "invalid".ToULongOrDefault(1));

        Assert.True("yes".ToBooleanOrDefault());
        Assert.False("invalid".ToBooleanOrDefault());

        Assert.Equal('A', "A".ToCharOrDefault());
        Assert.Equal('B', "invalid".ToCharOrDefault('B'));

        Assert.Equal("test", "test".ToStringOrDefault());
        Assert.Equal("fallback", ((object)null).ToStringOrDefault("fallback"));

        DateTime dt = new DateTime(2024, 1, 1);
        Assert.Equal(dt, "2024-01-01".ToDateTimeOrDefault());
        Assert.Equal(dt, "invalid".ToDateTimeOrDefault(dt));

        DateTimeOffset dto = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        Assert.Equal(dto, "2024-01-01T00:00:00Z".ToDateTimeOffsetOrDefault());
        Assert.Equal(dto, "invalid".ToDateTimeOffsetOrDefault(dto));
    }
}
