using System.ComponentModel.DataAnnotations;

namespace AuctionService.DTOs;
// Required Properties for every Auction
// TODO: Add a common folder for sharing all enums and constants
public class CreateAuctionDto
{
    [Required]
    [Range(0, (int)(HomeTypes.COUNT - 1))]
    public int Type { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Country { get; set; }    
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Address { get; set; } 
    [Required]
    public int Bed { get; set; } 
    [Required]
    public double Bath { get; set; }  
    [Required]
    public int Parking { get; set; }
    [Required]
    [Range(1900, 2023)]
    public int YearBuilt { get; set; }
    [Required]
    public int HOA { get; set; }
    [Required]
    public string[] ImageURL { get; set; }
    [Required]
    public int ReservePrice { get; set; }
    [Required]
    public DateTime AuctionEnd { get; set; }
}
