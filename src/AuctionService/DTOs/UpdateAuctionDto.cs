using System.ComponentModel.DataAnnotations;

namespace AuctionService.DTOs;
// TODO: localization
public class UpdateAuctionDto
{
    [Range(0, (int)(HomeTypes.COUNT - 1))]
    public int? Type { get; set; }
    [StringLength(15, MinimumLength = 2)]
    public string Country { get; set; } 
    [StringLength(100, MinimumLength = 2)]
    public string Address { get; set; } 
    public int? Bed { get; set; } 
    public double? Bath { get; set; }  
    public int? Parking { get; set; }
    [Range(1900, 2023)]
    public int? YearBuilt { get; set; }
    public int? HOA { get; set; }
}
