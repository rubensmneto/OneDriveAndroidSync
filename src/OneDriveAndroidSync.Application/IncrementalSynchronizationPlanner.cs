namespace OneDriveAndroidSync.Application;

public sealed class IncrementalSynchronizationPlanner(
    IFileFingerprintIndex index) : ISynchronizationPlanner
{
    public async Task<IReadOnlyList<TransferPlanItem>> CreateAsync(
        IReadOnlyList<FileAnalysisResult> files,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(files);

        var plan = new List<TransferPlanItem>();
        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (file.Classification != FileClassification.Eligible)
            {
                continue;
            }

            var previous = await index.FindAsync(
                file.Fingerprint.FullPath,
                cancellationToken);
            var classification = previous is not null && previous == file.Fingerprint
                ? "Unchanged"
                : "NewOrChanged";

            plan.Add(new TransferPlanItem(
                file.Fingerprint.FullPath,
                null,
                classification));
        }

        return plan;
    }
}
