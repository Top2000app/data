using System.Globalization;
using System.Reflection;
using Top2000.Data.Csv.Models;

namespace Top2000.Data.Csv;

public interface ITop2000DataProvider
{

}

public class Top2000DataProvider
{
    private const int ColumnCountBeforeEditions = 7;
    private List<Listing> listings = [];
    private List<Track> tracks = [];
    private List<Edition> editions = [];

    private Top2000DataProvider()
    {
    }

    public async Task<Top2000DataContext> LoadFromAssemblyAsync()
    {
        var data = await GetDataFromAssemblyAsync();

        var lines = data.Split(["\r", "\r\n"], StringSplitOptions.RemoveEmptyEntries);
        var metadata = lines[0].Split(';');

        editions = metadata.Skip(ColumnCountBeforeEditions)
            .Select(x => new Edition { Year = int.Parse(x) })
            .ToList();
        
        for (var line = 1; line < lines.Length; line++)
        {
            ParseLine(lines[line]);
        }

        return new Top2000DataContext
        {
            Editions = editions,
            Tracks = tracks,
            Listings = listings
        };
    }

    private void ParseLine(string line)
    {
        var columns = line.Split(";");
        var trackId = int.Parse(columns[0]);
        var firstEdition = ParseListings(trackId, columns);

        tracks.Add(new Track
        {
            Id = trackId,
            Title = columns[1],
            Artist = columns[2],
            SearchTitle = columns[3],
            SearchArtist = columns[4],
            RecordedYear = int.Parse(columns[5]),
            LastPlayUtc = ParsePlayTime(columns),
            FirstEdition = firstEdition
        });
    }

    private static DateTime? ParsePlayTime(string[] columns)
    {
        const string format = "dd-MM-yyyy HH:mm:ss'Z'";
        
        return DateTime.TryParseExact(columns[6], format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var playTime)
            ? playTime
            : null;
    }

    private int ParseListings(int trackId, string[] columns)
    {
        var firstEdition = -1;

        foreach (var edition in editions)
        {
            var index = edition.Year - 1999 + ColumnCountBeforeEditions;
            var position = columns[index].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");

            if (string.IsNullOrWhiteSpace(position))
            {
                continue;
            }

            if (firstEdition == -1)
            {
                firstEdition = edition.Year;
            }

            listings.Add(new Listing
            {
                Edition = edition.Year,
                Position = int.Parse(position),
                TrackId = trackId
            });
        }

        return firstEdition;
    }

    private async Task<string> GetDataFromAssemblyAsync()
    {
        await using var csvFileStream = Assembly
            .GetAssembly(this.GetType())?
            .GetManifestResourceStream("top2000.csv")
            ?? throw new InvalidOperationException("Unable to find 'top2000.csv' in assembly");

        using var streamReader = new StreamReader(csvFileStream);
        return await streamReader.ReadToEndAsync();
    }
}
