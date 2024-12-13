namespace Top2000.Data.Csv.Models;

public class Track
{
    public required int Id { get; init; }

    public required string Title { get; init; }

    public required string Artist { get; init; } 

    public required string SearchTitle { get; init; }
    
    public required string SearchArtist { get; init; }

    public required int RecordedYear { get; init; }

    public DateTime? LastPlayUtc { get; init; }

    public required int FirstEdition { get; init; }
}