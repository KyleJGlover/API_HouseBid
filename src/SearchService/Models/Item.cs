using MongoDB.Entities;

namespace SearchService.Models;

public class Item : Entity
{
    public int ReservePrice { get; set; }
    public string Seller { get; set; }
    public string Winner { get; set; }
    public int SoldAmount { get; set; }
    public int CurrentHighBid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public DateTime AuctionEnd { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
    public string Country { get; set; }
    public string Address { get; set; }
    public int Bed { get; set; } 
    public double Bath { get; set; }  
    public int Parking { get; set; }
    public int YearBuilt { get; set; }
    public int HOA { get; set; }
    public string[] ImageURL { get; set; }
}
