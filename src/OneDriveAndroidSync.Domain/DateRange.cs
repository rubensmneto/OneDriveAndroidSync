namespace OneDriveAndroidSync.Domain;

public sealed record DateRange(DateOnly Start, DateOnly End)
{
    public DateRange : this(Start, End)
    {
        if (End < Start) throw new ArgumentException("A data final deve ser maior ou igual à data inicial.");
    }

    public bool Includes(DateTime value) => DateOnly.FromDateTime(value) is var date && date >= Start && date <= End;
}
