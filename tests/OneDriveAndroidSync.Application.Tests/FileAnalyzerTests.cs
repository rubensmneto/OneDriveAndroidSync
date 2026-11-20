using OneDriveAndroidSync.Application;
using OneDriveAndroidSync.Domain;
using Xunit;

namespace OneDriveAndroidSync.Application.Tests;

public sealed class FileAnalyzerTests
{
    [Fact]
    public async Task AnalyzeAsync_ClassifiesFilesByParsedDate()
    {
        var root = Directory.CreateTempSubdirectory("onedrive-sync-");
        try
        {
            var eligible = Path.Combine(root.FullName, "20240115_120000.jpg");
            var outside = Path.Combine(root.FullName, "20240201_120000.jpg");
            var invalid = Path.Combine(root.FullName, "20240230_120000.jpg");
            var missing = Path.Combine(root.FullName, "notes.txt");
            foreach (var path in new[] { eligible, outside, invalid, missing })
            {
                await File.WriteAllTextAsync(path, "content");
            }

            var analyzer = new FileAnalyzer(
                new FileSourceScanner(),
                new FileNameDateParser());
            var results = await analyzer.AnalyzeAsync(
                root.FullName,
                new DateRange(new(2024, 1, 1), new(2024, 1, 31)));

            Assert.Equal(FileClassification.Eligible, Find(results, eligible).Classification);
            Assert.Equal(FileClassification.OutsideDateRange, Find(results, outside).Classification);
            Assert.Equal(FileClassification.InvalidDate, Find(results, invalid).Classification);
            Assert.Equal(FileClassification.MissingDate, Find(results, missing).Classification);
        }
        finally
        {
            root.Delete(true);
        }
    }

    private static FileAnalysisResult Find(
        IReadOnlyList<FileAnalysisResult> results,
        string path) => results.Single(result => result.Fingerprint.FullPath == Path.GetFullPath(path));
}
