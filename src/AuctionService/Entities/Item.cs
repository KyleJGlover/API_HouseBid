using System.ComponentModel.DataAnnotations.Schema;
using AuctionService.Entities;

namespace AuctionService;

[Table("Items")]
public class Item
{
    public Guid Id { get; set; }
    // Type of home 
    public HomeTypes Type { get; set; }
    // Number of beds
    public int Bed { get; set; }
    // Number of baths can be decimal  
    public double Bath { get; set; }  
    // Number of parking spots
    public int Parking { get; set; }
    // Year the home was built
    public int YearBuilt { get; set; }
    // Homeowners Association 
    public int HOA { get; set; }
    // URL location of the home images
    public string[] ImageURL { get; set; }

    // NAV Properties (Relational Properties)
    public Auction Auction { get; set; }
    public Guid AuctionId { get; set; }
}
