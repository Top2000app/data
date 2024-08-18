namespace Top2000.Data.Csv.Models;

public class Track
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Artist { get; set; } = string.Empty;

    public int RecordedYear { get; set; } = 1;

    public DateTime? LastPlayUtc { get; set; }

    public int FirstEdition { get; set; } = -1;
}