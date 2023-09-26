using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.InitAsync("SearchDB", MongoClientSettings
            .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        // Searchable Parameters
        await DB.Index<Item>()
            .Key(x => x.Seller, KeyType.Text)
            .Key(x => x.Country, KeyType.Text)
            .Key(x => x.Type, KeyType.Text)     
            .Key(x => x.Bed, KeyType.Ascending)
            .Key(x => x.Bath, KeyType.Ascending)
            .CreateAsync();

            var count = await DB.CountAsync<Item>();

            if(count == 0)
            {
                Console.WriteLine("No data - will seed data.");
                var itemData = await File.ReadAllTextAsync("Data/auction.json");

                var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

                var items = JsonSerializer.Deserialize<List<Item>>(itemData,options);

                await DB.SaveAsync(items);
            }
    }
}
