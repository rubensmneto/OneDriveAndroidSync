using OneDriveAndroidSync.Domain;
using Xunit;

namespace OneDriveAndroidSync.Domain.Tests;

public sealed class DateRangeTests
{
    [Fact]
    public void Includes_Boundaries()
    {
        var range = new DateRange(new(2024, 1, 1), new(2024, 1, 31));

        Assert.True(range.Includes(new DateTime(2024, 1, 1)));
        Assert.True(range.Includes(new DateTime(2024, 1, 31, 23, 59, 59)));
        Assert.False(range.Includes(new DateTime(2023, 12, 31, 23, 59, 59)));
        Assert.False(range.Includes(new DateTime(2024, 2, 1)));
    }

    [Fact]
    public void Rejects_EndBeforeStart()
    {
        Assert.Throws<ArgumentException>(() => new DateRange(new(2024, 2, 1), new(2024, 1, 31)));
    }

    [Fact]
    public void Parser_ReadsExpectedPattern()
    {
        var result = new FileNameDateParser().Parse("20240131_235959.jpg");

        Assert.Equal(NameParseStatus.Valid, result.Status);
        Assert.Equal(new DateTime(2024, 1, 31, 23, 59, 59), result.Value);
    }

    [Theory]
    [InlineData("photo.jpg", NameParseStatus.MissingPattern)]
    [InlineData("20240230_120000.jpg", NameParseStatus.InvalidDate)]
    [InlineData("20240101_250000.jpg", NameParseStatus.InvalidDate)]
    public void Parser_ClassifiesInvalidNames(string fileName, NameParseStatus expected)
    {
        Assert.Equal(expected, new FileNameDateParser().Parse(fileName).Status);
    }
}
