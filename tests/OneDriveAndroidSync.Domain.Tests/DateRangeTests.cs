using OneDriveAndroidSync.Domain;

namespace OneDriveAndroidSync.Domain.Tests;

public sealed class DateRangeTests
{
    [Fact] public void Includes_Boundaries() { var range = new DateRange(new(2024, 1, 1), new(2024, 1, 31)); Assert.True(range.Includes(new DateTime(2024, 1, 1))); Assert.True(range.Includes(new DateTime(2024, 1, 31, 23, 59, 59))); }
    [Fact] public void Parser_ReadsExpectedPattern() { var result = new FileNameDateParser().Parse("20240131_235959.jpg"); Assert.Equal(NameParseStatus.Valid, result.Status); Assert.Equal(new DateTime(2024, 1, 31, 23, 59, 59), result.Value); }
}
