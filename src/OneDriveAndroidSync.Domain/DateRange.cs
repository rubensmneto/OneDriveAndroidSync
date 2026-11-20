namespace OneDriveAndroidSync.Domain;

public sealed record DateRange
{
    public DateRange(DateOnly start, DateOnly end)
    {
        if (end < start) throw new ArgumentException("A data final deve ser maior ou igual à data inicial.");

        Start = start;
        End = end;
    }

    public DateOnly Start { get; }
    public DateOnly End { get; }

    public bool Includes(DateTime value)
    {
        var date = DateOnly.FromDateTime(value);
        return date >= Start && date <= End;
    }
}
