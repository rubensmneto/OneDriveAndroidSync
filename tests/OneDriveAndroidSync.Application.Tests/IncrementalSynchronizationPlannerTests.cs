using OneDriveAndroidSync.Application;
using OneDriveAndroidSync.Domain;
using Xunit;

namespace OneDriveAndroidSync.Application.Tests;

public sealed class IncrementalSynchronizationPlannerTests
{
    [Fact]
    public async Task CreateAsync_SkipsNonEligibleAndMarksUnchangedFiles()
    {
        var unchanged = new FileFingerprint("C:\\photos\\20240101_120000.jpg", 10, DateTime.UnixEpoch);
        var changed = unchanged with { Size = 20 };
        var files = new[]
        {
            new FileAnalysisResult(unchanged, FileClassification.Eligible, new DateTime(2024, 1, 1)),
            new FileAnalysisResult(changed, FileClassification.Eligible, new DateTime(2024, 1, 1)),
            new FileAnalysisResult(
                new FileFingerprint("C:\\photos\\notes.txt", 5, DateTime.UnixEpoch),
                FileClassification.MissingDate,
                null)
        };
        var planner = new IncrementalSynchronizationPlanner(
            new FakeIndex(unchanged));

        var result = await planner.CreateAsync(files);

        Assert.Equal(2, result.Count);
        Assert.Equal("Unchanged", result[0].Classification);
        Assert.Equal("NewOrChanged", result[1].Classification);
    }

    private sealed class FakeIndex(FileFingerprint? fingerprint) : IFileFingerprintIndex
    {
        public Task<FileFingerprint?> FindAsync(
            string fullPath,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(fingerprint?.FullPath == fullPath ? fingerprint : null);
    }
}
