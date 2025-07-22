namespace Domain.Common.Dtos;

public class PlaceDto
{
    public string? FsqId { get; set; }           
    public string? Name { get; set; }            
    public string Category { get; set; }   
    public string? Address { get; set; }       
    public double Latitude { get; set; }     
    public double Longitude { get; set; }   
    public int Distance { get; set; }           
}
