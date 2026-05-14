namespace Book_Exchange.Models.DTOs.Address;

public class PlaceDistanceDto
{
    public int DistanceMeters { get; set; }
    public double DistanceKm => Math.Round(DistanceMeters / 1000.0, 2);
    public string Duration { get; set; } = string.Empty;
}