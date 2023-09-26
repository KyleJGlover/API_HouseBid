using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class DbInitializer
{
    public static void InitDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        SeedData(scope.ServiceProvider.GetService<AuctionDBContext>());
    }
    private static void SeedData(AuctionDBContext context)
    {
        context.Database.Migrate();

        if(context.Auctions.Any())
        {
            Console.WriteLine("Already have data - no need to seed");
            return;
        }

        var auctions = new List<Auction>()
        {
            new() {
                Id = Guid.Parse("fd748200-db04-41bd-9981-2a29ca901be9"),
                Status = Status.Live,
                ReservePrice = 200000,
                Seller = "Kyle",
                AuctionEnd = DateTime.UtcNow.AddDays(10),
                Item = new Item
                {
                    Type = HomeTypes.House,
                    Country = "USA",
                    Address = "1 Kyle Street",
                    Bed = 3,
                    Bath = 2.5,
                    Parking = 2,
                    YearBuilt = 1975,
                    HOA = 500,
                    ImageURL = new string[]
                    { 
                        "https://cdn.pixabay.com/photo/2016/05/06/16/32/car-1376190_960_720.jpg",
                        "https://cdn.pixabay.com/photo/2012/05/29/00/43/car-49278_960_720.jpg",
                    }
                }
            },
            new() {
                Id = Guid.Parse("b0d9f2f8-3346-42cd-998a-9fdd96aaee84"),
                Status = Status.Live,
                ReservePrice = 400000,
                Seller = "Blake",
                AuctionEnd = DateTime.UtcNow.AddDays(5),
                Item = new Item
                {
                    Type = HomeTypes.Condo,
                    Country = "CANADA",
                    Address = "2 Blake Street",
                    Bed = 2,
                    Bath = 1,
                    Parking = 1,
                    YearBuilt = 1998,
                    HOA = 200,
                    ImageURL = new string[]
                    { 
                        "https://cdn.pixabay.com/photo/2012/11/02/13/02/car-63930_960_720.jpg",
                        "https://cdn.pixabay.com/photo/2016/04/17/22/10/mercedes-benz-1335674_960_720.png",
                    }
                }
            },
            new() {
                Id = Guid.Parse("7e497c46-d33d-4cdb-a22d-d60eb5fbae75"),
                Status = Status.Live,
                ReservePrice = 800000,
                Seller = "Alyssa",
                AuctionEnd = DateTime.UtcNow.AddDays(2),
                Item = new Item
                {
                    Type = HomeTypes.Lot,
                    Country = "SPAIN",
                    Address = "3 Alyssa Street",
                    Bed = 5,
                    Bath = 3,
                    Parking = 4,
                    YearBuilt = 2000,
                    HOA = 600,
                    ImageURL = new string[]
                    { 
                        "https://cdn.pixabay.com/photo/2017/08/31/05/47/bmw-2699538_960_720.jpg",
                        "https://cdn.pixabay.com/photo/2017/11/09/01/49/ferrari-458-spider-2932191_960_720.jpg",
                    }
                }
            },
            new() {
                Id = Guid.Parse("b3ff20a4-68b4-4dc5-bd01-d655b4cfcd17"),
                Status = Status.Live,
                ReservePrice = 1000000,
                Seller = "Sierra",
                AuctionEnd = DateTime.UtcNow.AddDays(8),
                Item = new Item
                {
                    Type = HomeTypes.Apartment,
                    Country = "MEXICO",
                    Address = "4 Sierra Street",
                    Bed = 3,
                    Bath = 2,
                    Parking = 2,
                    YearBuilt = 2005,
                    HOA = 300,
                    ImageURL = new string[]
                    { 
                        "https://cdn.pixabay.com/photo/2017/11/08/14/39/ferrari-f430-2930661_960_720.jpg",
                        "https://cdn.pixabay.com/photo/2019/12/26/20/50/audi-r8-4721217_960_720.jpg",
                    }
                }
            }
        };

        context.AddRange(auctions);
        context.SaveChanges();
    }
}
