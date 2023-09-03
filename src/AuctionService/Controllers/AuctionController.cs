using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication;

namespace AuctionService;
// Data Validation (Required props), Bind prop sent as arguments
[ApiController][Route("api/auctions")]
public class AuctionController : ControllerBase
{
    private readonly AuctionDBContext _context;
    private readonly IMapper _mapper;

    // Database Dependency injection
    public AuctionController(AuctionDBContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        var auctions = await _context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Type)
            .ToListAsync();
        return _mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if(auction == null) return NotFound();

        return _mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);
        // TODO: Add current user as seller
        auction.Seller = "test";

        _context.Auctions.Add(auction);
        // If changes were more then zero success
        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Could not save the changes tot he database.");

        return CreatedAtAction(nameof(GetAuctionById),
            new {auction.Id}, _mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if(auction == null) return NotFound();

        if(updateAuctionDto.Type < 0 || updateAuctionDto.Type > (int)HomeTypes.COUNT)
            return BadRequest("Warning: problem saving changes!"); 

        //TODO: Check seller == username

        auction.Item.Type = (HomeTypes)(updateAuctionDto.Type ?? (int)auction.Item.Type);
        auction.Item.Bed =  updateAuctionDto.Bed ?? auction.Item.Bed;
        auction.Item.Bath =  updateAuctionDto.Bath ?? auction.Item.Bath;
        auction.Item.Parking =  updateAuctionDto.Parking ?? auction.Item.Parking;
        auction.Item.YearBuilt =  updateAuctionDto.YearBuilt ?? auction.Item.YearBuilt;
        auction.Item.HOA =  updateAuctionDto.HOA ?? auction.Item.HOA;

        var result = await _context.SaveChangesAsync() > 0;

        if(result) return Ok();

        return BadRequest("Warning: problem saving changes!");
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction (Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);

        if(auction == null) return NotFound();

        //TODO: Check seller == username

        _context.Auctions.Remove(auction);

        var result =  await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest();

        return Ok();
    }
}
