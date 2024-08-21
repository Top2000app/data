namespace Top2000.Data.ClientDatabase.Models;

public class Edition
{
    [PrimaryKey]
    public int Year { get; set; }
    public DateTime StartUtcDateAndTime { get; set; }
    public DateTime EndUtcDateAndTime { get; set; }
    public bool HasPlayDateAndTime { get; set; }
}
