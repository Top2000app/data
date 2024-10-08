﻿namespace Top2000.Data.ClientDatabase.Models;

public class Track
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public int RecordedYear { get; set; }
}
