using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;
using ZstdSharp.Unsafe;

namespace SearchService.Controller;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery]SearchParams searchParams)
    {
        var query =  DB.PagedSearch<Item, Item>();

        query.Sort(x => x.Ascending(a => a.Type));

        // Check if the search is for a specific param
        // Param: Seller, Country, Type, Bed, Bath
        if(!string.IsNullOrEmpty(searchParams.searchTerm))
        {
            query.Match(Search.Full, searchParams.searchTerm);
        }
        // Adjust the order of items
        query = searchParams.OrderBy switch
        {
            "type" => query.Sort(x => x.Ascending(y => y.Type)),
            "new" => query.Sort(x => x.Descending(y => y.CreatedAt)),
            _ => query.Sort(x => x.Ascending(y => y.AuctionEnd))
        };
        // Create filters
        query = searchParams.FilterBy switch
        {
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(5)
                && x.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };

        if(!string.IsNullOrEmpty(searchParams.Seller))
        {
            query.Match(x => x.Seller == searchParams.Seller);
        }    

        if(!string.IsNullOrEmpty(searchParams.Winner))
        {
            query.Match(x => x.Seller == searchParams.Winner);
        }    

        query.PageNumber(searchParams.PageNumber);
        query.PageSize(searchParams.PageSize);

        var result = await query.ExecuteAsync();

        return Ok(new 
        {
            results = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}