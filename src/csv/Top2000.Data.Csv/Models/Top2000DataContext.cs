﻿namespace Top2000.Data.Csv.Models;

public class Top2000DataContext
{
    public required List<Track> Tracks { get; init; }
    public required List<Listing> Listings { get; init;  }
    public required List<Edition> Editions { get; init;  } 
}
