using System.Globalization;
using System.Text.RegularExpressions;

namespace OneDriveAndroidSync.Domain;

public enum NameParseStatus { Valid, MissingPattern, InvalidDate }
public sealed record ParsedFileDate(NameParseStatus Status, DateTime? Value);

public interface IFileNameDateParser { ParsedFileDate Parse(string fileName); }

public sealed class FileNameDateParser : IFileNameDateParser
{
    private static readonly Regex Pattern = new("(?<!\\d)(?<date>\\d{8})_(?<time>\\d{6})(?!\\d)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public ParsedFileDate Parse(string fileName)
    {
        var match = Pattern.Match(fileName);
        if (!match.Success) return new(NameParseStatus.MissingPattern, null);
        var text = $"{match.Groups["date"].Value}_{match.Groups["time"].Value}";
        return DateTime.TryParseExact(text, "yyyyMMdd_HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var value)
            ? new(NameParseStatus.Valid, DateTime.SpecifyKind(value, DateTimeKind.Local))
            : new(NameParseStatus.InvalidDate, null);
    }
}
