namespace Contracts;

public class AuctionUpdated
{
    public string Id { get; set; }
    public int Type { get; set; }
    public string Country { get; set; } 
    public string Address { get; set; } 
    public int Bed { get; set; } 
    public double Bath { get; set; }  
    public int Parking { get; set; }
    public int YearBuilt { get; set; }
    public int HOA { get; set; }
}
