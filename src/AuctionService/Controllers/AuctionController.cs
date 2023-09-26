using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using MassTransit.DependencyInjection.Registration;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IPublishEndpoint _publishEndPoint;

    // Database Dependency injection
    public AuctionController(AuctionDBContext context, IMapper mapper, IPublishEndpoint publishEndPoint)
    {
        _context = context;
        _mapper = mapper;
        _publishEndPoint = publishEndPoint;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Type).AsQueryable();

        if(!string.IsNullOrEmpty(date))
        {
            query = query.Where(x => x.UpdateAt
                .CompareTo(DateTime.Parse(date)
                .ToUniversalTime()) > 0);
        }

        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
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

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);
        // Add the user name to the seller param
        auction.Seller = User.Identity.Name;

        // Treated as a transaction (Must work or all fail)
        _context.Auctions.Add(auction);

        var newAuction = _mapper.Map<AuctionDto>(auction);

        await _publishEndPoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

        // If changes were more then zero success
        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Could not save the changes to he database.");

        return CreatedAtAction(nameof(GetAuctionById),
            new {auction.Id}, _mapper.Map<AuctionDto>(auction));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if(auction == null) return NotFound();

        if(updateAuctionDto.Type < 0 || updateAuctionDto.Type > (int)HomeTypes.COUNT)
            return BadRequest("Warning: problem saving changes!"); 


        // If the seller doesnt match 403 response
        if(auction.Seller != User.Identity.Name) return Forbid();

        //TODO: Add functionality for updating ReservePrice
        auction.Item.Country =  updateAuctionDto.Country ?? auction.Item.Country;
        auction.Item.Address =  updateAuctionDto.Address ?? auction.Item.Address;
        auction.Item.Bed =  updateAuctionDto.Bed ?? auction.Item.Bed;
        auction.Item.Bath =  updateAuctionDto.Bath ?? auction.Item.Bath;
        auction.Item.Parking =  updateAuctionDto.Parking ?? auction.Item.Parking;
        auction.Item.YearBuilt =  updateAuctionDto.YearBuilt ?? auction.Item.YearBuilt;
        auction.Item.HOA =  updateAuctionDto.HOA ?? auction.Item.HOA;

        // Publish update event
        await _publishEndPoint.Publish(_mapper.Map<AuctionUpdated>(auction));

        var result = await _context.SaveChangesAsync() > 0;

        if(result) return Ok();

        return BadRequest("Warning: problem saving changes!");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction (Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);

        if(auction == null) return NotFound();
        
        // If the seller doesnt match 403 response
        if(auction.Seller != User.Identity.Name) return Forbid();
        
        _context.Auctions.Remove(auction);

        // Publish Delete event
        await _publishEndPoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

        var result =  await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest();

        return Ok();
    }
}
