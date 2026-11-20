using OneDriveAndroidSync.Domain;

namespace OneDriveAndroidSync.Application;

public sealed class FileAnalyzer(
    IFileSourceScanner scanner,
    IFileNameDateParser dateParser) : IFileAnalyzer
{
    public async Task<IReadOnlyList<FileAnalysisResult>> AnalyzeAsync(
        string root,
        DateRange dateRange,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dateRange);

        var results = new List<FileAnalysisResult>();

        await foreach (var fingerprint in scanner.ScanAsync(root, cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var parsed = dateParser.Parse(Path.GetFileName(fingerprint.FullPath));
            var classification = parsed.Status switch
            {
                NameParseStatus.MissingPattern => FileClassification.MissingDate,
                NameParseStatus.InvalidDate => FileClassification.InvalidDate,
                NameParseStatus.Valid when parsed.Value is not null && dateRange.Includes(parsed.Value.Value)
                    => FileClassification.Eligible,
                _ => FileClassification.OutsideDateRange
            };

            results.Add(new FileAnalysisResult(
                fingerprint,
                classification,
                parsed.Value));
        }

        return results;
    }
}
